using System;
using System.IO;
using System.Reflection;
using System.Text;
using NAudio.Wave;
using System.Threading;
using DiscOut.Renderer;
using System.Numerics;
namespace DiscOut.Util;
// Contains utility functions related to sound
internal static class SoundUtils
{
    private static readonly Stream BOUNCE_DATA = AssetUtil.OpenEmbeddedFile("assets.sounds.bounce.wav");
    public static readonly WaveFileReader BOUNCE_SOUND = new(BOUNCE_DATA);
    private static readonly Stream CLICK_DATA = AssetUtil.OpenEmbeddedFile("assets.sounds.click.wav");
    public static readonly WaveFileReader CLICK_SOUND = new(CLICK_DATA);
    // Plays a sound using the specified IWaveProvider
    public static void PlaySound(IWaveProvider stream)
    {
        ThreadPool.QueueUserWorkItem(delegate
        {
            using WaveOutEvent player = new();
            player.Init(stream);
            player.Play();
            while (player.PlaybackState != PlaybackState.Stopped) ;
            player.Dispose();
        });
        // Reset the position to the beginning if the stream is a WaveFileReader
        if (stream.GetType() == typeof(WaveFileReader))
            ((WaveFileReader)stream).Position = 0;
    }
    // Cleans up the sound resources
    public static void CleanUp()
    {
        BOUNCE_SOUND.Dispose();
        BOUNCE_DATA.Dispose();
        CLICK_SOUND.Dispose();
        CLICK_DATA.Dispose();
    }
}
// Contains utility functions related to vertices
internal static class VertexUtils
{
    // Pre-calculate the vertices for a quad and assign color and UV coordinates
    public static Vertex[] PreMakeQuad(float x, float y, float width, float height, float color)
    {
        var quad = new Vertex[6]; // Use 6 vertices for a quad triangle strip
        UpdateQuad(x, y, width, height, ref quad, 0);
        quad[0].ColorID = color;
        quad[0].UvID = 0;
        quad[1].ColorID = color;
        quad[1].UvID = 1;
        quad[2].ColorID = color;
        quad[2].UvID = 2;
        quad[3].ColorID = color;
        quad[3].UvID = 3;
        quad[4].ColorID = color;
        quad[4].UvID = 2;
        quad[5].ColorID = color;
        quad[5].UvID = 1;
        return quad;
    }
    // Pre-calculate the vertices for a quad and assign color and UV coordinates to an existing array of vertices
    public static void PreMakeQuad(float x, float y, float width, float height, float color, ref Vertex[] vertices, int index = 0)
    {
        UpdateQuad(x, y, width, height, ref vertices, index);
        vertices[index].ColorID = color;
        vertices[index].UvID = 0;
        vertices[index + 1].ColorID = color;
        vertices[index + 1].UvID = 1;
        vertices[index + 2].ColorID = color;
        vertices[index + 2].UvID = 2;
        vertices[index + 3].ColorID = color;
        vertices[index + 3].UvID = 3;
        vertices[index + 4].ColorID = color;
        vertices[index + 4].UvID = 2;
        vertices[index + 5].ColorID = color;
        vertices[index + 5].UvID = 1;
    }
    // Pre-calculate the vertices for a quad based on the specified parameters
    private static readonly Vector3 PerTransform_0 = new(-3, -2, -4.1982f);
    private static readonly Vector3 PerTransform_1 = new(3, -2, -4.1982f);
    private static readonly Vector3 PerTransform_2 = new(-3, 2, -4.1982f);
    private static readonly Vector3 PerTransform_3 = new(3, 2, -4.1982f);
    private static readonly Vector3 PerTransform_4 = new(-3, 2, -4.1982f);
    private static readonly Vector3 PerTransform_5 = new(3, -2, -4.1982f);
    public static Vertex[] UpdateQuad(float x, float y, float width, float height, ref Vertex[] quad, int index = 0)
    {
        // Apply model transformation
        Matrix4x4 modelTransform = Matrix4x4.CreateTranslation(x, y, -1);
        modelTransform *= Matrix4x4.CreateScale(width, height, 1);
        // Calculate transformed vertices
        Vector3 transform_0 = Vector3.Transform(PerTransform_0, modelTransform);
        quad[index].X = transform_0.X / transform_0.Z;
        quad[index].Y = transform_0.Y / transform_0.Z;

        Vector3 transform_1 = Vector3.Transform(PerTransform_1, modelTransform);
        quad[index + 1].X = transform_1.X / transform_1.Z;
        quad[index + 1].Y = transform_1.Y / transform_1.Z;

        Vector3 transform_2 = Vector3.Transform(PerTransform_2, modelTransform);
        quad[index + 2].X = transform_2.X / transform_2.Z;
        quad[index + 2].Y = transform_2.Y / transform_2.Z;

        Vector3 transform_3 = Vector3.Transform(PerTransform_3, modelTransform);
        quad[index + 3].X = transform_3.X / transform_3.Z;
        quad[index + 3].Y = transform_3.Y / transform_3.Z;

        Vector3 transform_4 = Vector3.Transform(PerTransform_4, modelTransform);
        quad[index + 4].X = transform_4.X / transform_4.Z;
        quad[index + 4].Y = transform_4.Y / transform_4.Z;

        Vector3 transform_5 = Vector3.Transform(PerTransform_5, modelTransform);
        quad[index + 5].X = transform_5.X / transform_5.Z;
        quad[index + 5].Y = transform_5.Y / transform_5.Z;
        return quad;
    }
}
// Contains utility functions related to collision detection
internal static class CollisonUtil
{
    // Tests for AABB collision between two objects
    public static bool TestAABB(Aabb obj_0, Aabb obj_1)
    {
        // Check if there is a gap between the two AABBs along any axis
        if (obj_0.MaxX < obj_1.MinX || obj_0.MinX > obj_1.MaxX) return false; // No overlap along X-axis
        if (obj_0.MaxY < obj_1.MinY || obj_0.MinY > obj_1.MaxY) return false; // No overlap along Y-axis
        return true; // Overlapping along both X-axis and Y-axis
    }
}
// Contains utility functions related to assets
internal static class AssetUtil
{
    private static readonly Assembly exe_base = typeof(Program).GetTypeInfo().Assembly;
    // Reads the content of an embedded file as a string
    public static string ReadEmbeddedFile(string path)
    {
        using Stream? stream = exe_base.GetManifestResourceStream("SquareSmash." + path)
            ?? throw new FieldAccessException("could not find embedded file:" + path);

        using StreamReader reader = new(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
    // Opens an embedded file as a stream
    public static Stream OpenEmbeddedFile(string path)
        => exe_base.GetManifestResourceStream("SquareSmash." + path)
            ?? throw new FieldAccessException("could not find embedded file:" + path);
    // Opens a file as a stream
    public static Stream OpenFile(string path, bool create = true)
        => File.Open(Path.GetDirectoryName(exe_base.Location) + '/' + path, create ? FileMode.OpenOrCreate : FileMode.Open);
}