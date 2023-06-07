using SquareSmash.renderer;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace SquareSmash.utils
{

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
            Stream? stream = exe_base.GetManifestResourceStream("SquareSmash." + path);
            if (stream == null)
                throw new FieldAccessException("could not find embedded file:" + path);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }


        public static Stream OpenEmbeddedFile(string path)
        {
            Stream? stream = exe_base.GetManifestResourceStream("SquareSmash." + path);
            if (stream == null)
                throw new FieldAccessException("could not find embedded file:" + path);
            stream.Position = 0;
            return stream;
        }

        public static Stream OpenFile(string path,bool create = true)
        {
                Stream  stream = File.Open(Path.GetDirectoryName(exe_base.Location) + '/' + path, create ? FileMode.OpenOrCreate: FileMode.Open)!;
                stream.Position = 0;
                return stream;
        }

    }
}
