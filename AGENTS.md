# AGENTS.md

Notes for anyone, human or AI agent, changing this code. `README.md` covers the game, its features and controls;
this file covers how the code is put together and the rules that keep it working.

## What this is

A recreation of Elite (Acornsoft, 1984, by David Braben and Ian Bell) in C# on .NET 9 and Avalonia 11 (11.3.22). The
ships, stations and planets are wireframes with hidden-line removal, drawn with Avalonia's `DrawingContext`; there
is no 3D library and no sound. The galaxy is generated from the original three-word seed algorithm.

## Commands

```bash
dotnet build AVAElite.sln
dotnet run --project AVAElite          # play it; there are no command-line arguments
```

There are no tests yet.

## Layout

All code is under `AVAElite/`.

- `MainWindow.axaml.cs`: the game loop. A 16 ms `DispatcherTimer` measures the real elapsed time (capped at 0.1 s),
  updates the `InputManager`, calls `Update(dt, input)` on the current screen and redraws.
- `Views/GameCanvas.cs`: a plain control whose `Render` hands the `DrawingContext` to the current screen.
- `Core/`
  - `GameState.cs`: the commander (credits and fuel in tenths, cargo, equipment, lasers, kills, legal status) and
    `CurrentScreen`.
  - `InputManager.cs`: held keys and keys pressed this frame (`IsDown`, `JustPressed`), by Avalonia `Key`.
  - `MathUtil.cs`: the `Vector3` and `Matrix3x3` types.
  - `RandomSeed.cs`: `EliteSeed`, the original 3x16-bit seed with `Twist` and `NextGalaxy`.
- `Universe/`: `GalaxyGenerator` (256 systems per galaxy from the seed), `StarSystem`, `CommodityTable` (the 17
  goods) and `Market` (prices and quantities per system).
- `Models/`: `ShipModel.Create` builds a model from vertices and faces, computing face normals and the edges with
  their two faces. `ShipDefinitions` holds the ships, station, missile, asteroid and cargo can.
- `Flight/`: `SpaceScene` (the player's position, orientation, speed, shields and the entities in the system) and
  `SpaceEntity`.
- `Rendering/`: `Renderer3D.DrawShip` (perspective projection, near clipping, back-face test per edge) and
  `HudRenderer` (dashboard and scanner).
- `Screens/`: one class per screen, each an `IGameScreen`. `FlightScreen` holds the flight controls, combat and the
  NPC AI (`UpdateAI`). The others are the title, hyperspace, market, equipment, status, galaxy map, short-range
  chart and system data screens.
- `Images/` (at the root): the README's screenshot.

## Rules that keep it working

- **Screens switch by replacing `CurrentScreen`.** Each screen sets `_state.CurrentScreen = new OtherScreen(...)` on
  its function key and passes the `FlightScreen` along so flight can resume. A new screen implements `IGameScreen`
  and is wired into the key handling of the screens that lead to it.
- **Time steps vary.** Updates take the real frame time `dt`, so scale movement and timers by `dt`.
- **The galaxy must match the original.** `EliteSeed` and `GalaxyGenerator` reproduce the 1984 algorithm (Lave is
  system 7 of galaxy 1). Changing the bit operations there changes every system name and position.
- **Fixed seeds elsewhere.** Starfields and the title use `new Random(42)`; `SpaceScene` and `Market` seed `Random`
  from the system's coordinates, so a system always looks and trades the same. Keep new randomness seeded the same
  way where repeatability matters.
- **Hidden lines need closed models.** Edges are drawn only when one of their two faces points at the camera, and
  normals come from each face's vertex order. A new model's faces must be convex and wound consistently, or
  edges vanish or show through.
- **Pitch sign.** In `FlightScreen`, S rotates by a positive angle about X and W by a negative one. Keep keys and
  any new input on that convention.
- **No saves.** Nothing is written to disk; a new game starts as Commander Jameson at Lave.

## Making a change

- Check changes by running the game; there are no automated tests.
- Keep the README's controls table, features list and project structure in step with the code.

## CI and releases

`.github/workflows/build.yml` builds every push and pull request with .NET 9. It then publishes self-contained
single-file builds for win-x64/x86/arm64, linux-x64/arm64/arm and osx-x64/arm64, kept as workflow artifacts. Pushes
to `main` replace the assets of the `latest` pre-release, and a `v*` tag makes a proper release. After a release
it asks the portfolio site to rebuild, if the `SITE_REFRESH_TOKEN` secret is set.
