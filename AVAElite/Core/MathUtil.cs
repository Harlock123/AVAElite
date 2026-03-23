using System;

namespace AVAElite.Core;

public struct Vector3
{
    public double X, Y, Z;

    public Vector3(double x, double y, double z)
    {
        X = x; Y = y; Z = z;
    }

    public static Vector3 Zero => new(0, 0, 0);
    public static Vector3 UnitX => new(1, 0, 0);
    public static Vector3 UnitY => new(0, 1, 0);
    public static Vector3 UnitZ => new(0, 0, 1);

    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);
    public double LengthSquared => X * X + Y * Y + Z * Z;

    public Vector3 Normalized()
    {
        var len = Length;
        if (len < 1e-10) return Zero;
        return new Vector3(X / len, Y / len, Z / len);
    }

    public static double Dot(Vector3 a, Vector3 b) =>
        a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Vector3 Cross(Vector3 a, Vector3 b) => new(
        a.Y * b.Z - a.Z * b.Y,
        a.Z * b.X - a.X * b.Z,
        a.X * b.Y - a.Y * b.X
    );

    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3 operator *(Vector3 a, double s) => new(a.X * s, a.Y * s, a.Z * s);
    public static Vector3 operator *(double s, Vector3 a) => new(a.X * s, a.Y * s, a.Z * s);
    public static Vector3 operator /(Vector3 a, double s) => new(a.X / s, a.Y / s, a.Z / s);
    public static Vector3 operator -(Vector3 a) => new(-a.X, -a.Y, -a.Z);

    public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2})";
}

public struct Matrix3x3
{
    public double M00, M01, M02;
    public double M10, M11, M12;
    public double M20, M21, M22;

    public static Matrix3x3 Identity => new()
    {
        M00 = 1, M11 = 1, M22 = 1
    };

    public Vector3 Row0 => new(M00, M01, M02);
    public Vector3 Row1 => new(M10, M11, M12);
    public Vector3 Row2 => new(M20, M21, M22);

    public static Matrix3x3 FromRows(Vector3 r0, Vector3 r1, Vector3 r2) => new()
    {
        M00 = r0.X, M01 = r0.Y, M02 = r0.Z,
        M10 = r1.X, M11 = r1.Y, M12 = r1.Z,
        M20 = r2.X, M21 = r2.Y, M22 = r2.Z,
    };

    public Vector3 Transform(Vector3 v) => new(
        M00 * v.X + M01 * v.Y + M02 * v.Z,
        M10 * v.X + M11 * v.Y + M12 * v.Z,
        M20 * v.X + M21 * v.Y + M22 * v.Z
    );

    public Matrix3x3 Transpose() => new()
    {
        M00 = this.M00, M01 = this.M10, M02 = this.M20,
        M10 = this.M01, M11 = this.M11, M12 = this.M21,
        M20 = this.M02, M21 = this.M12, M22 = this.M22,
    };

    public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b) => new()
    {
        M00 = a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20,
        M01 = a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21,
        M02 = a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22,
        M10 = a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20,
        M11 = a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21,
        M12 = a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22,
        M20 = a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20,
        M21 = a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21,
        M22 = a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22,
    };

    public static Matrix3x3 RotationX(double angle)
    {
        var c = Math.Cos(angle);
        var s = Math.Sin(angle);
        return new()
        {
            M00 = 1, M01 = 0, M02 = 0,
            M10 = 0, M11 = c, M12 = -s,
            M20 = 0, M21 = s, M22 = c,
        };
    }

    public static Matrix3x3 RotationY(double angle)
    {
        var c = Math.Cos(angle);
        var s = Math.Sin(angle);
        return new()
        {
            M00 = c, M01 = 0, M02 = s,
            M10 = 0, M11 = 1, M12 = 0,
            M20 = -s, M21 = 0, M22 = c,
        };
    }

    public static Matrix3x3 RotationZ(double angle)
    {
        var c = Math.Cos(angle);
        var s = Math.Sin(angle);
        return new()
        {
            M00 = c, M01 = -s, M02 = 0,
            M10 = s, M11 = c, M12 = 0,
            M20 = 0, M21 = 0, M22 = 1,
        };
    }
}
