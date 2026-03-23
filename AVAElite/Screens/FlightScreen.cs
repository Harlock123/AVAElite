using System;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Flight;
using AVAElite.Models;
using AVAElite.Rendering;
using AVAElite.Universe;

namespace AVAElite.Screens;

public class FlightScreen : IGameScreen
{
    private readonly GameState _state;
    private readonly SpaceScene _scene;
    private StarSystem[] _galaxy = null!;
    private string _currentSystemName = "";
    private double _stationRotation;
    private readonly Random _rng = new();

    private readonly double[] _starX;
    private readonly double[] _starY;
    private readonly double[] _starZ;

    private static readonly Pen ShipPen = new(new SolidColorBrush(Color.FromRgb(0, 255, 0)), 1.2);
    private static readonly Pen HostilePen = new(new SolidColorBrush(Color.FromRgb(255, 80, 80)), 1.2);
    private static readonly Pen StationPen = new(new SolidColorBrush(Color.FromRgb(255, 255, 255)), 1.5);
    private static readonly Pen MissilePen = new(new SolidColorBrush(Color.FromRgb(255, 255, 0)), 1);
    private static readonly IBrush GreenBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
    private static readonly IBrush RedBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

    public FlightScreen(GameState state)
    {
        _state = state;
        _scene = new SpaceScene();

        _galaxy = GalaxyGenerator.Generate(state.CurrentGalaxy);
        var system = _galaxy[state.CurrentSystem];
        _currentSystemName = system.Name;

        if (state.Docked)
        {
            _scene.SpawnFromHyperspace(system);
            _scene.SpawnFromStation();
            state.Docked = false;
        }
        else
        {
            _scene.SpawnFromHyperspace(system);
        }

        // Generate star positions on a unit sphere
        var rng = new Random(42);
        _starX = new double[120];
        _starY = new double[120];
        _starZ = new double[120];
        for (int i = 0; i < _starX.Length; i++)
        {
            // Random direction on unit sphere
            double theta = rng.NextDouble() * Math.PI * 2;
            double phi = Math.Acos(2 * rng.NextDouble() - 1);
            _starX[i] = Math.Sin(phi) * Math.Cos(theta);
            _starY[i] = Math.Sin(phi) * Math.Sin(theta);
            _starZ[i] = Math.Cos(phi);
        }
    }

    public void Update(double dt, InputManager input)
    {
        HandleFlightInput(dt, input);
        UpdateScene(dt);
        UpdateAI(dt);
        CheckCombat(dt, input);
        CheckDocking();
        CheckPlayerDeath();
    }

    private void HandleFlightInput(double dt, InputManager input)
    {
        double pitchRate = 1.5;
        double rollRate = 1.5;
        double accel = 15.0;

        // Pitch
        if (input.IsDown(Key.S) || input.IsDown(Key.W))
        {
            double dir = input.IsDown(Key.S) ? 1 : -1;
            var rot = Matrix3x3.RotationX(dir * pitchRate * dt);
            _scene.PlayerOrientation = _scene.PlayerOrientation * rot;
        }

        // Roll
        if (input.IsDown(Key.OemComma) || input.IsDown(Key.OemPeriod))
        {
            double dir = input.IsDown(Key.OemComma) ? 1 : -1;
            var rot = Matrix3x3.RotationZ(dir * rollRate * dt);
            _scene.PlayerOrientation = _scene.PlayerOrientation * rot;
        }

        // Speed: Z = faster, X = slower (Tab doesn't work on macOS)
        if (input.IsDown(Key.Z))
            _scene.PlayerSpeed = Math.Min(_scene.PlayerSpeed + accel * dt, 40);
        if (input.IsDown(Key.X))
            _scene.PlayerSpeed = Math.Max(_scene.PlayerSpeed - accel * dt, 0);

        // View direction
        if (input.JustPressed(Key.D1)) _scene.ViewDirection = 0;
        if (input.JustPressed(Key.D2)) _scene.ViewDirection = 1;
        if (input.JustPressed(Key.D3)) _scene.ViewDirection = 2;
        if (input.JustPressed(Key.D4)) _scene.ViewDirection = 3;

        // Missile targeting
        if (input.JustPressed(Key.T))
        {
            if (!_scene.MissileArmed && _state.MissileCount > 0)
            {
                _scene.MissileArmed = true;
                // Target nearest hostile
                _scene.MissileTarget = _scene.Entities
                    .Where(e => e.Type == EntityType.Ship && !e.IsDead)
                    .OrderBy(e => (e.Position - _scene.PlayerPosition).LengthSquared)
                    .FirstOrDefault();
            }
            else
            {
                _scene.MissileArmed = false;
                _scene.MissileTarget = null;
            }
        }

        // Fire missile
        if (input.JustPressed(Key.M) && _scene.MissileArmed && _state.MissileCount > 0 && _scene.MissileTarget != null)
        {
            _state.MissileCount--;
            var missile = new SpaceEntity
            {
                Model = ShipDefinitions.Missile,
                Position = _scene.PlayerPosition + _scene.PlayerForward * 20,
                Orientation = _scene.PlayerOrientation,
                Speed = 44,
                Type = EntityType.Missile,
                Energy = 2,
                MaxEnergy = 2,
                AiTimer = 0,
            };
            _scene.Entities.Add(missile);
            _scene.MissileArmed = false;
        }

        // ECM
        if (input.JustPressed(Key.E) && _state.HasECM)
        {
            foreach (var e in _scene.Entities.Where(e => e.Type == EntityType.Missile))
                e.IsDead = true;
        }

        // Energy bomb
        if (input.JustPressed(Key.B) && _state.HasEnergyBomb)
        {
            _state.HasEnergyBomb = false;
            foreach (var e in _scene.Entities.Where(e => e.Type == EntityType.Ship && !e.IsDead))
            {
                e.Energy = 0;
                e.IsDead = true;
                if (e.IsHostile) _state.Kills++;
            }
        }

        // Screen navigation
        if (input.JustPressed(Key.F5))
            _state.CurrentScreen = new StatusScreen(_state, _galaxy, this);
        if (input.JustPressed(Key.F6))
            _state.CurrentScreen = new GalaxyMapScreen(_state, _galaxy, this);
        if (input.JustPressed(Key.F7))
            _state.CurrentScreen = new ShortRangeScreen(_state, _galaxy, this);
        if (input.JustPressed(Key.F8))
            _state.CurrentScreen = new SystemInfoScreen(_state, _galaxy, this);

        // Hyperspace
        if (input.JustPressed(Key.J) && _state.SelectedSystem != _state.CurrentSystem)
        {
            TryHyperspace();
        }

        // Galactic hyperspace
        if (input.JustPressed(Key.H) && _state.HasGalacticHyperdrive)
        {
            _state.HasGalacticHyperdrive = false;
            _state.CurrentGalaxy = (_state.CurrentGalaxy + 1) % 8;
            _galaxy = GalaxyGenerator.Generate(_state.CurrentGalaxy);
            _state.CurrentSystem = 0;
            _state.SelectedSystem = 0;
            var target = _galaxy[0];
            _currentSystemName = target.Name;
            _state.CurrentScreen = new HyperspaceScreen(_state, $"Galaxy {_state.CurrentGalaxy + 1}: {target.Name}", () =>
            {
                _scene.SpawnFromHyperspace(target);
                _state.CurrentScreen = this;
            });
        }
    }

    private void UpdateScene(double dt)
    {
        // Move player
        _scene.PlayerPosition = _scene.PlayerPosition + _scene.PlayerForward * _scene.PlayerSpeed * dt * 50;

        // Energy regeneration
        _scene.PlayerEnergy = Math.Min(_scene.PlayerEnergy + dt * 3, 255);
        _scene.PlayerFrontShield = Math.Min(_scene.PlayerFrontShield + dt * 2, 255);
        _scene.PlayerRearShield = Math.Min(_scene.PlayerRearShield + dt * 2, 255);
        _scene.LaserTemp = Math.Max(_scene.LaserTemp - dt * 0.3, 0);
        _scene.CabinTemp = Math.Max(_scene.CabinTemp - dt * 0.1, 0);
        _scene.LaserCooldown = Math.Max(_scene.LaserCooldown - dt, 0);
        _scene._dockingCooldown = Math.Max(_scene._dockingCooldown - dt, 0);

        // Rotate station
        _stationRotation += dt * 0.2;
        if (_scene.Station != null)
        {
            _scene.Station.Orientation = Matrix3x3.RotationZ(_stationRotation) * Matrix3x3.RotationX(_stationRotation * 0.3);
        }

        // Update entities
        foreach (var entity in _scene.Entities)
        {
            if (entity.IsDead) continue;
            entity.UpdatePosition(dt * 50);
        }

        // Handle missiles
        foreach (var entity in _scene.Entities.Where(e => e.Type == EntityType.Missile && !e.IsDead))
        {
            entity.AiTimer += dt;
            if (entity.AiTimer > 10) { entity.IsDead = true; continue; }

            // Home in on target (if this is a player missile targeting something)
            if (_scene.MissileTarget != null && !_scene.MissileTarget.IsDead)
            {
                var toTarget = (_scene.MissileTarget.Position - entity.Position).Normalized();
                entity.RotateToward(toTarget, 3.0, dt);
                entity.Position = entity.Position + entity.Forward * entity.Speed * dt * 50;

                // Check hit
                double dist = (entity.Position - _scene.MissileTarget.Position).Length;
                if (dist < 100)
                {
                    _scene.MissileTarget.Energy -= 80;
                    if (_scene.MissileTarget.Energy <= 0)
                    {
                        _scene.MissileTarget.IsDead = true;
                        _state.Kills++;
                    }
                    entity.IsDead = true;
                }
            }
        }

        // Remove dead entities (keep station)
        _scene.Entities.RemoveAll(e => e.IsDead && e.Type != EntityType.Station);
    }

    private void UpdateAI(double dt)
    {
        foreach (var entity in _scene.Entities)
        {
            if (entity.IsDead || entity.Type != EntityType.Ship) continue;

            entity.AiTimer += dt;

            switch (entity.Behavior)
            {
                case AiBehavior.Trader:
                    // Fly toward station
                    if (_scene.Station != null)
                    {
                        var toStation = (_scene.Station.Position - entity.Position).Normalized();
                        entity.RotateToward(toStation, 0.5, dt);
                    }
                    break;

                case AiBehavior.Pirate:
                    // Fly toward player and attack
                    var toPlayer = (_scene.PlayerPosition - entity.Position).Normalized();
                    entity.RotateToward(toPlayer, 1.0, dt);

                    double distToPlayer = (entity.Position - _scene.PlayerPosition).Length;
                    if (distToPlayer < 3000 && entity.AiTimer > 2.0)
                    {
                        // Fire at player
                        var aimDot = Vector3.Dot(entity.Forward, toPlayer);
                        if (aimDot > 0.95)
                        {
                            ApplyDamageToPlayer(entity.Model.LaserPower * dt * 10);
                            entity.AiTimer = 0;
                        }
                    }
                    break;

                case AiBehavior.Police:
                    if (_state.LegalStatus > 0)
                    {
                        var toP = (_scene.PlayerPosition - entity.Position).Normalized();
                        entity.RotateToward(toP, 1.0, dt);
                    }
                    break;

                case AiBehavior.BountyHunter:
                    if (_state.LegalStatus >= 32)
                    {
                        var toBH = (_scene.PlayerPosition - entity.Position).Normalized();
                        entity.RotateToward(toBH, 1.2, dt);
                    }
                    break;
            }
        }
    }

    private void CheckCombat(double dt, InputManager input)
    {
        // Laser firing
        if (input.IsDown(Key.Space) && _scene.LaserCooldown <= 0 && _scene.LaserTemp < 0.95)
        {
            FireLaser();
        }
    }

    private void FireLaser()
    {
        int laserPower = _scene.ViewDirection switch
        {
            0 => _state.FrontLaser,
            1 => _state.RearLaser,
            2 => _state.LeftLaser,
            3 => _state.RightLaser,
            _ => 0,
        };

        if (laserPower == 0) return;

        double damage = laserPower switch
        {
            1 => 15,  // pulse
            2 => 25,  // beam
            3 => 10,  // mining
            4 => 40,  // military
            _ => 0,
        };

        double energyCost = laserPower switch
        {
            1 => 5,
            2 => 10,
            3 => 3,
            4 => 15,
            _ => 0,
        };

        if (_scene.PlayerEnergy < energyCost) return;
        _scene.PlayerEnergy -= energyCost;
        _scene.LaserTemp = Math.Min(_scene.LaserTemp + 0.08, 1.0);
        _scene.LaserCooldown = laserPower == 2 ? 0.05 : 0.15; // beam fires faster

        // Get the firing direction based on view
        Vector3 fireDir = _scene.ViewDirection switch
        {
            0 => _scene.PlayerForward,
            1 => -_scene.PlayerForward,
            2 => -_scene.PlayerRight,
            3 => _scene.PlayerRight,
            _ => _scene.PlayerForward,
        };

        // Ray-cast: find nearest entity in the firing direction
        SpaceEntity? hit = null;
        double bestDist = double.MaxValue;
        double maxRange = 5000;

        foreach (var entity in _scene.Entities)
        {
            if (entity.IsDead || entity.Type == EntityType.Station) continue;

            var toEntity = entity.Position - _scene.PlayerPosition;
            double dist = toEntity.Length;
            if (dist > maxRange) continue;

            var dirToEntity = toEntity.Normalized();
            double dot = Vector3.Dot(fireDir, dirToEntity);
            if (dot < 0.97) continue; // ~14 degree cone

            // Check if entity is close enough to the ray
            double perpDist = Vector3.Cross(fireDir, toEntity).Length;
            if (perpDist < entity.Model.BoundingRadius * 2 && dist < bestDist)
            {
                bestDist = dist;
                hit = entity;
            }
        }

        if (hit != null)
        {
            hit.Energy -= damage;
            if (hit.Energy <= 0)
            {
                hit.IsDead = true;
                if (hit.Behavior == AiBehavior.Pirate || hit.IsHostile)
                    _state.Kills++;
                else if (hit.Behavior == AiBehavior.Police || hit.Behavior == AiBehavior.Trader)
                    _state.LegalStatus = Math.Min(_state.LegalStatus + 64, 255);
            }
            else if (!hit.IsHostile && hit.Behavior == AiBehavior.Trader)
            {
                hit.Behavior = AiBehavior.Pirate;
                hit.IsHostile = true;
            }
        }
    }

    private void ApplyDamageToPlayer(double damage)
    {
        if (_scene.PlayerFrontShield > 0)
        {
            _scene.PlayerFrontShield -= damage;
            if (_scene.PlayerFrontShield < 0)
            {
                damage = -_scene.PlayerFrontShield;
                _scene.PlayerFrontShield = 0;
            }
            else return;
        }
        _scene.PlayerEnergy -= damage;
    }

    private void CheckDocking()
    {
        if (_scene.Station == null) return;
        if (_scene._dockingCooldown > 0) return;

        double dist = (_scene.PlayerPosition - _scene.Station.Position).Length;
        if (dist < 200)
        {
            // Simple docking: close enough and roughly aligned
            var toStation = (_scene.Station.Position - _scene.PlayerPosition).Normalized();
            double alignment = Math.Abs(Vector3.Dot(_scene.PlayerForward, toStation));
            if (alignment > 0.8)
            {
                _state.Docked = true;
                _state.CurrentScreen = new StatusScreen(_state, _galaxy, this);
            }
        }
    }

    private void TryHyperspace()
    {
        var current = _galaxy[_state.CurrentSystem];
        var target = _galaxy[_state.SelectedSystem];
        double dist = current.DistanceTo(target);
        double fuelNeeded = dist * 10; // tenths of LY

        if (_state.Fuel >= (int)fuelNeeded)
        {
            _state.Fuel -= (int)fuelNeeded;
            _state.CurrentSystem = _state.SelectedSystem;
            _currentSystemName = target.Name;

            // Decay legal status
            if (_state.LegalStatus > 0)
                _state.LegalStatus = Math.Max(0, _state.LegalStatus - 16);

            // Show hyperspace transition
            _state.CurrentScreen = new HyperspaceScreen(_state, target.Name, () =>
            {
                _scene.SpawnFromHyperspace(target);
                _state.CurrentScreen = this;
            });
        }
    }

    public void CheckPlayerDeath()
    {
        if (_scene.PlayerEnergy <= 0)
        {
            // Player destroyed — reset to Lave
            _state.Credits = Math.Max(_state.Credits / 2, 1000);
            _state.Fuel = 70;
            _state.CurrentSystem = 7; // Lave
            _state.SelectedSystem = 7;
            _state.LegalStatus = 0;
            _state.MissileCount = 3;
            _state.Docked = true;
            _state.HasECM = false;
            _state.HasFuelScoop = false;
            _state.HasEnergyBomb = false;
            _state.HasDockingComputer = false;
            _state.HasGalacticHyperdrive = false;
            _state.HasEscapePod = false;
            _state.HasLargeCargoBay = false;
            _state.FrontLaser = 1;
            _state.RearLaser = 0;
            _state.LeftLaser = 0;
            _state.RightLaser = 0;
            for (int i = 0; i < 17; i++) _state.Cargo[i] = 0;

            _state.CurrentScreen = new TitleScreen(_state, () =>
            {
                _galaxy = GalaxyGenerator.Generate(_state.CurrentGalaxy);
                var system = _galaxy[_state.CurrentSystem];
                _currentSystemName = system.Name;
                _scene.SpawnFromHyperspace(system);
                _scene.SpawnFromStation();
                _state.Docked = false;
                _state.CurrentScreen = this;
            });
        }
    }

    public void Render(DrawingContext ctx, double w, double h)
    {
        // Black background
        ctx.DrawRectangle(Brushes.Black, null, new Rect(0, 0, w, h));

        double viewTop = 30;
        double viewBottom = h - 150;
        double viewW = w - 20;
        double viewH = viewBottom - viewTop;

        // Clip to viewport
        using (ctx.PushClip(new Rect(10, viewTop, viewW, viewH)))
        {
            // Get camera transform for current view direction
            var camOri = GetCameraOrientation();
            var camPos = _scene.PlayerPosition;

            // Stars projected from 3D unit sphere
            var invCam = camOri.Transpose();
            double sCX = w / 2, sCY = viewTop + viewH / 2;
            var starPen = new Pen(new SolidColorBrush(Color.FromRgb(200, 200, 200)), 1);
            for (int i = 0; i < _starX.Length; i++)
            {
                var dir = new Vector3(_starX[i], _starY[i], _starZ[i]);
                var viewDir = invCam.Transform(dir);
                if (viewDir.Z > 0.01)
                {
                    double sx = sCX + (viewDir.X / viewDir.Z) * 400;
                    double sy = sCY - (viewDir.Y / viewDir.Z) * 400;
                    if (sx > 5 && sx < w - 5 && sy > viewTop && sy < viewBottom)
                        ctx.DrawLine(starPen, new Point(sx, sy), new Point(sx + 1, sy));
                }
            }

            // Draw planet (simple circle)
            DrawPlanet(ctx, camPos, camOri, w, h, viewTop, viewBottom);

            // Draw all entities
            foreach (var entity in _scene.Entities)
            {
                if (entity.IsDead) continue;

                var rel = entity.Position - camPos;
                var viewPos = camOri.Transpose().Transform(rel);
                if (viewPos.Z < 1) continue; // behind camera

                double dist = viewPos.Length;
                if (dist > 50000) continue; // too far

                Pen pen = entity.Type switch
                {
                    EntityType.Station => StationPen,
                    EntityType.Missile => MissilePen,
                    _ => entity.IsHostile ? HostilePen : ShipPen,
                };

                Renderer3D.DrawShip(ctx, entity.Model, entity.Position, entity.Orientation,
                    camPos, camOri, w / 2, viewTop + viewH / 2, pen);
            }

            // Draw laser beam when firing
            if (_scene.LaserCooldown > 0.05)
            {
                DrawLaserBeam(ctx, w, viewTop, viewH);
            }
        }

        // HUD
        HudRenderer.DrawFlightHud(ctx, w, h,
            _scene.PlayerSpeed, 40,
            _scene.PlayerEnergy, 255,
            _scene.PlayerFrontShield, _scene.PlayerRearShield,
            _state.MissileCount, _state.MaxMissiles, _scene.MissileArmed,
            0, _scene.CabinTemp, _scene.LaserTemp,
            _scene.ViewDirection, _currentSystemName);

        // Scanner blips
        double scanCX = w / 2;
        double scanCY = viewBottom + 60;
        double scanRX = 120;
        double scanRY = 40;

        foreach (var entity in _scene.Entities)
        {
            if (entity.IsDead || entity.Type == EntityType.Missile) continue;

            var rel = entity.Position - _scene.PlayerPosition;
            var local = _scene.PlayerOrientation.Transpose().Transform(rel);
            double dist = local.Length;
            if (dist > 20000 || dist < 1) continue;

            double rx = local.X / 20000;
            double ry = local.Y / 20000;
            double rz = local.Z / 20000;

            HudRenderer.DrawScannerBlip(ctx, scanCX, scanCY, scanRX, scanRY,
                rx, ry, rz, entity.IsHostile);
        }

        // Compass (pointing to station)
        if (_scene.Station != null)
        {
            var toStation = _scene.Station.Position - _scene.PlayerPosition;
            var localDir = _scene.PlayerOrientation.Transpose().Transform(toStation.Normalized());
            HudRenderer.DrawCompass(ctx, w - 50, viewBottom + 60, 20, localDir);
        }
    }

    private Matrix3x3 GetCameraOrientation()
    {
        return _scene.ViewDirection switch
        {
            1 => _scene.PlayerOrientation * Matrix3x3.RotationY(Math.PI),
            2 => _scene.PlayerOrientation * Matrix3x3.RotationY(Math.PI / 2),
            3 => _scene.PlayerOrientation * Matrix3x3.RotationY(-Math.PI / 2),
            _ => _scene.PlayerOrientation,
        };
    }

    private void DrawPlanet(DrawingContext ctx, Vector3 camPos, Matrix3x3 camOri,
        double w, double h, double viewTop, double viewBottom)
    {
        var rel = _scene.PlanetPosition - camPos;
        var viewPos = camOri.Transpose().Transform(rel);
        if (viewPos.Z < 1) return;

        double cx = w / 2 + (viewPos.X / viewPos.Z) * 400;
        double cy = (viewTop + viewBottom) / 2 - (viewPos.Y / viewPos.Z) * 400;
        double r = (_scene.PlanetRadius / viewPos.Z) * 400;

        if (r < 1 || r > 2000) return;

        var planetPen = new Pen(new SolidColorBrush(Color.FromRgb(0, 200, 0)), 1.5);
        ctx.DrawEllipse(null, planetPen, new Point(cx, cy), r, r);
    }

    private void DrawLaserBeam(DrawingContext ctx, double w, double viewTop, double viewH)
    {
        double cx = w / 2;
        double cy = viewTop + viewH / 2;

        int laserPower = _scene.ViewDirection switch
        {
            0 => _state.FrontLaser,
            1 => _state.RearLaser,
            2 => _state.LeftLaser,
            3 => _state.RightLaser,
            _ => 0,
        };
        if (laserPower == 0) return;

        var color = laserPower switch
        {
            4 => Color.FromRgb(255, 100, 100),
            2 => Color.FromRgb(255, 255, 100),
            _ => Color.FromRgb(255, 255, 255),
        };
        var laserPen = new Pen(new SolidColorBrush(color), laserPower >= 2 ? 2 : 1);

        // Draw converging laser beams
        double spread = 40;
        ctx.DrawLine(laserPen, new Point(cx - spread, cy + 20), new Point(cx, cy - 80));
        ctx.DrawLine(laserPen, new Point(cx + spread, cy + 20), new Point(cx, cy - 80));
    }
}
