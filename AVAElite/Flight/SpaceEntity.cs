using AVAElite.Core;
using AVAElite.Models;

namespace AVAElite.Flight;

public enum EntityType
{
    Ship, Station, Missile, Asteroid, CargoPod, EscapePod, Planet
}

public enum AiBehavior
{
    None, Trader, Pirate, BountyHunter, Police, Thargoid, Shuttle, Fleeing
}

public class SpaceEntity
{
    public ShipModel Model { get; set; } = null!;
    public Vector3 Position { get; set; }
    public Matrix3x3 Orientation { get; set; } = Matrix3x3.Identity;
    public double Speed { get; set; }
    public double Energy { get; set; } = 100;
    public double MaxEnergy { get; set; } = 100;
    public EntityType Type { get; set; } = EntityType.Ship;
    public AiBehavior Behavior { get; set; } = AiBehavior.None;
    public bool IsHostile { get; set; }
    public bool IsDead { get; set; }
    public int Bounty { get; set; }
    public double AiTimer { get; set; }

    public Vector3 Forward => new(Orientation.M02, Orientation.M12, Orientation.M22);

    public void UpdatePosition(double dt)
    {
        if (Speed != 0)
        {
            Position = Position + Forward * Speed * dt;
        }
    }

    public void RotateToward(Vector3 targetDir, double rate, double dt)
    {
        var forward = Forward;
        var cross = Vector3.Cross(forward, targetDir);
        double dot = Vector3.Dot(forward, targetDir);

        if (cross.LengthSquared < 0.0001) return;

        var axis = cross.Normalized();
        double angle = rate * dt;
        if (dot > 0.99) angle *= 0.1; // slow down when nearly aligned

        // Simple rotation toward target
        double pitchAmount = axis.X * angle;
        double yawAmount = axis.Y * angle;

        var pitchRot = Matrix3x3.RotationX(pitchAmount);
        var yawRot = Matrix3x3.RotationY(yawAmount);
        Orientation = Orientation * pitchRot * yawRot;
    }
}
