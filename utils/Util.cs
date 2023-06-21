using SquareSmash.renderer;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using OpenTK.Mathematics;

namespace SquareSmash.utils
{

    internal static class VertexUtils
    {
        public static readonly Matrix4 ScreenMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), 0.85915492957f, 0.1f, 100f) 
                                                                                           * Matrix4.LookAt(new(0.0f, 0.0f, 5.0f), Vector3.Zero, Vector3.UnitY);

        private static readonly Vector4 pre_transform_0 = (new Vector4(-1.0f, -1.0f, -1.0f, 1.0f) * ScreenMatrix);
        private static readonly Vector4 pre_transform_1 = (new Vector4(1.0f, -1.0f, -1.0f, 1.0f) * ScreenMatrix);
        // pre calculate the vertex so that we don't need to spent cpu cycles at rime-time to calculate the vertex
        // the can be compliantly expensive 
        public static Vertex[] PreMakeQuad(Vector2 position, Vector2 size, float color)
        {
            var quad = new Vertex[6]; // Use 6 vertices for a quad triangle strip
            UpdateQuad(position, size, ref quad);
            quad[0].Color = color;
            quad[0].Uvs = new(0.0f, 0.0f);
            quad[1].Color = color;
            quad[1].Uvs = new(1.0f, 0.0f);
            quad[2].Color = color;
            quad[2].Uvs = new(0.0f, 1.0f);
            quad[3].Color = color;
            quad[3].Uvs = new(1.0f, 1.0f);
            quad[4].Color = color;
            quad[4].Uvs = new(0.0f, 1.0f);
            quad[5].Color = color;
            quad[5].Uvs = new(1.0f, 0.0f);
            return quad;
        }

        // pre calculate the vertex so that we don't need to spent cpu cycles at rime-time to calculate the vertex
        // the can be compliantly expensive 
        public static Vertex[] UpdateQuad(Vector2 position, Vector2 size, ref Vertex[] quad)
        {
            // Apply model transformation
            Matrix4 modelTransform = Matrix4.Identity;
            modelTransform *= Matrix4.CreateTranslation(new Vector3(position.X, position.Y, -1.0f));
            modelTransform *= Matrix4.CreateScale(new Vector3(size.X, size.Y, 1.0f));

            Vector4 transform_0 = pre_transform_0 * modelTransform;
            Vector3 scale_down_0 = new Vector3(transform_0.X, transform_0.Y, transform_0.Z) / transform_0.Z;
            quad[0].Position = scale_down_0.Xy;

            Vector4 transform_1 = pre_transform_1 * modelTransform;
            Vector3 scale_down_1 = new Vector3(transform_1.X, transform_1.Y, transform_1.Z) / transform_1.Z;
            quad[1].Position = scale_down_1.Xy;

            Vector4 transform_2 = (new Vector4(-1.0f, 1.0f, -1.0f, 1.0f) * ScreenMatrix * modelTransform);
            Vector3 scale_down_2 = new Vector3(transform_2.X, transform_2.Y, transform_2.Z) / transform_2.Z;
            quad[2].Position = scale_down_2.Xy;

            Vector4 transform_3 = (new Vector4(1.0f, 1.0f, -1.0f, 1.0f) * ScreenMatrix * modelTransform);
            Vector3 scale_down_3 = new Vector3(transform_3.X, transform_3.Y, transform_3.Z) / transform_3.Z;
            quad[3].Position = scale_down_3.Xy;

            Vector4 transform_4 = (new Vector4(-1.0f, 1.0f, -1.0f, 1.0f) * ScreenMatrix * modelTransform);
            Vector3 scale_down_4 = new Vector3(transform_4.X, transform_4.Y, transform_4.Z) / transform_4.Z;
            quad[4].Position = scale_down_4.Xy;

            Vector4 transform_5 = (new Vector4(1.0f, -1.0f, -1.0f, 1.0f) * ScreenMatrix * modelTransform);
            Vector3 scale_down_5 = new Vector3(transform_5.X, transform_5.Y, transform_5.Z) / transform_5.Z;
            quad[5].Position = scale_down_5.Xy;
            return quad;
        }
    }

    internal static class CollisonUtil
    {
        public static bool TestAABB(Aabb obj_0, Aabb obj_1)
        {
            // Check if there is a gap between the two AABBs along any axis
            if (obj_0.Max.X < obj_1.Min.X || obj_0.Min.X > obj_1.Max.X) return false; // No overlap along X-axis
            if (obj_0.Max.Y < obj_1.Min.Y || obj_0.Min.Y > obj_1.Max.Y) return false; // No overlap along Y-axis
            return true; // Overlapping along both X-axis and Y-axis
        }
    }

    internal static class AssetUtil
    {
        private static readonly Assembly exe_base = typeof(Program).GetTypeInfo().Assembly;

        public static string ReadEmbeddedFile(string path)
        {
            Stream? stream = exe_base.GetManifestResourceStream("SquareSmash." + path) ?? throw new FieldAccessException("could not find embedded file:" + path);
            stream.Position = 0;
            using StreamReader reader = new(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }


        public static Stream OpenEmbeddedFile(string path)
        {
            Stream? stream = exe_base.GetManifestResourceStream("SquareSmash." + path) ?? throw new FieldAccessException("could not find embedded file:" + path);
            stream.Position = 0;
            return stream;
        }

        public static Stream OpenFile(string path, bool create = true)
        {
            Stream stream = File.Open(Path.GetDirectoryName(exe_base.Location) + '/' + path, create ? FileMode.OpenOrCreate : FileMode.Open)!;
            stream.Position = 0;
            return stream;
        }

    }
}
