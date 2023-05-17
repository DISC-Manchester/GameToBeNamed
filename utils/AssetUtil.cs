using System.IO;
using System.Reflection;
using System.Text;

namespace SquareSmash.utils
{
    internal static class AssetUtil
    {
        private static  readonly Assembly exe_base = typeof(ProgramMain).GetTypeInfo().Assembly;

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

    }
}
