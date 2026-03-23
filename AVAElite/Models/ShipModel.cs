using System;
using System.Collections.Generic;
using System.Linq;
using AVAElite.Core;

namespace AVAElite.Models;

public class ShipFace
{
    public int[] VertexIndices { get; set; } = [];
    public Vector3 Normal { get; set; }
}

public class ShipEdge
{
    public int Start { get; set; }
    public int End { get; set; }
    public int Face1 { get; set; } = -1;
    public int Face2 { get; set; } = -1;
}

public class ShipModel
{
    public string Name { get; set; } = "";
    public Vector3[] Vertices { get; set; } = [];
    public ShipFace[] Faces { get; set; } = [];
    public ShipEdge[] Edges { get; set; } = [];
    public double BoundingRadius { get; set; }

    public int MaxSpeed { get; set; } = 20;
    public int MaxEnergy { get; set; } = 100;
    public int MissileCount { get; set; }
    public int LaserPower { get; set; }
    public int Bounty { get; set; }
    public int CargoCapacity { get; set; }

    public static ShipModel Create(string name, Vector3[] vertices, int[][] faceVertexIndices)
    {
        var centroid = Vector3.Zero;
        foreach (var v in vertices)
            centroid = centroid + v;
        centroid = centroid / vertices.Length;

        var faces = new ShipFace[faceVertexIndices.Length];
        for (int i = 0; i < faceVertexIndices.Length; i++)
        {
            var indices = (int[])faceVertexIndices[i].Clone();
            var v0 = vertices[indices[0]];
            var v1 = vertices[indices[1]];
            var v2 = vertices[indices[2]];

            var edge1 = v1 - v0;
            var edge2 = v2 - v0;
            var normal = Vector3.Cross(edge1, edge2).Normalized();

            var faceCentroid = Vector3.Zero;
            foreach (var idx in indices)
                faceCentroid = faceCentroid + vertices[idx];
            faceCentroid = faceCentroid / indices.Length;

            var toFace = faceCentroid - centroid;
            if (Vector3.Dot(normal, toFace) < 0)
            {
                normal = -normal;
                Array.Reverse(indices);
            }

            faces[i] = new ShipFace { VertexIndices = indices, Normal = normal };
        }

        var edgeDict = new Dictionary<(int, int), ShipEdge>();
        for (int fi = 0; fi < faces.Length; fi++)
        {
            var indices = faces[fi].VertexIndices;
            for (int j = 0; j < indices.Length; j++)
            {
                int a = indices[j];
                int b = indices[(j + 1) % indices.Length];
                var key = (Math.Min(a, b), Math.Max(a, b));

                if (!edgeDict.TryGetValue(key, out var edge))
                {
                    edge = new ShipEdge { Start = key.Item1, End = key.Item2, Face1 = fi };
                    edgeDict[key] = edge;
                }
                else if (edge.Face2 == -1)
                {
                    edge.Face2 = fi;
                }
            }
        }

        double maxR = 0;
        foreach (var v in vertices)
        {
            var r = (v - centroid).Length;
            if (r > maxR) maxR = r;
        }

        return new ShipModel
        {
            Name = name,
            Vertices = vertices,
            Faces = faces,
            Edges = edgeDict.Values.ToArray(),
            BoundingRadius = maxR,
        };
    }
}
