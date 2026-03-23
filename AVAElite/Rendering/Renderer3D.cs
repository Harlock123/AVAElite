using System;
using Avalonia;
using Avalonia.Media;
using AVAElite.Core;
using AVAElite.Models;

namespace AVAElite.Rendering;

public class Renderer3D
{
    private const double FocalLength = 400;
    private const double NearClip = 1.0;

    public static void DrawShip(
        DrawingContext ctx,
        ShipModel model,
        Vector3 position,
        Matrix3x3 orientation,
        Vector3 cameraPosition,
        Matrix3x3 cameraOrientation,
        double centerX,
        double centerY,
        Pen pen)
    {

        var invCamera = cameraOrientation.Transpose();

        // Transform all vertices to view space
        var viewVerts = new Vector3[model.Vertices.Length];
        var screenPts = new Point[model.Vertices.Length];
        var vertexVisible = new bool[model.Vertices.Length];

        for (int i = 0; i < model.Vertices.Length; i++)
        {
            // Model -> World -> View
            var world = orientation.Transform(model.Vertices[i]) + position;
            var view = invCamera.Transform(world - cameraPosition);
            viewVerts[i] = view;

            if (view.Z > NearClip)
            {
                vertexVisible[i] = true;
                double sx = centerX + (view.X / view.Z) * FocalLength;
                double sy = centerY - (view.Y / view.Z) * FocalLength;
                screenPts[i] = new Point(sx, sy);
            }
        }

        // Back-face culling: determine which faces are visible
        var faceVisible = new bool[model.Faces.Length];
        for (int i = 0; i < model.Faces.Length; i++)
        {
            var face = model.Faces[i];
            // Transform face normal to view space
            var normalView = invCamera.Transform(orientation.Transform(face.Normal));

            // Use first vertex of face as reference point
            var vertexView = viewVerts[face.VertexIndices[0]];

            // Face is visible when normal points toward camera (dot product < 0)
            faceVisible[i] = Vector3.Dot(normalView, vertexView) < 0;
        }

        // Draw visible edges
        foreach (var edge in model.Edges)
        {
            // Edge is visible if at least one adjacent face is visible
            bool edgeVisible = false;
            if (edge.Face1 >= 0 && faceVisible[edge.Face1]) edgeVisible = true;
            if (edge.Face2 >= 0 && faceVisible[edge.Face2]) edgeVisible = true;

            // For edges with only one face (open meshes), always draw if face is visible
            if (edge.Face1 >= 0 && edge.Face2 == -1 && faceVisible[edge.Face1]) edgeVisible = true;
            if (edge.Face2 >= 0 && edge.Face1 == -1 && faceVisible[edge.Face2]) edgeVisible = true;

            if (!edgeVisible) continue;
            if (!vertexVisible[edge.Start] || !vertexVisible[edge.End]) continue;

            ctx.DrawLine(pen, screenPts[edge.Start], screenPts[edge.End]);
        }
    }

    public static void DrawStarfield(DrawingContext ctx, double width, double height, double[] starX, double[] starY, IBrush brush)
    {
        var pen = new Pen(brush, 1);
        for (int i = 0; i < starX.Length; i++)
        {
            double x = starX[i] % width;
            double y = starY[i] % height;
            if (x < 0) x += width;
            if (y < 0) y += height;
            ctx.DrawLine(pen, new Point(x, y), new Point(x + 1, y));
        }
    }
}
