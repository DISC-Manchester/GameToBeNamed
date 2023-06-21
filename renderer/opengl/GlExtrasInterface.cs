using System;
namespace Avalonia.OpenGL;

public class GlExtrasInterface : GlInterfaceBase<GlInterface.GlContextInfo>
{
    public GlExtrasInterface(GlInterface gl) : base(gl.GetProcAddress, gl.ContextInfo) { }

    [GlEntryPoint("glGenVertexArrays")]
    public GlGenVertexArrays GenVertexArrays { get; } = null!;
    public unsafe delegate void GlGenVertexArrays(int count, int[] arrays);

    [GlEntryPoint("glGenTextures")]
    public GlGenTextures GenTextures { get; } = null!;
    public unsafe delegate void GlGenTextures(int count, int* textures);

    [GlEntryPoint("glDeleteVertexArrays")]
    public GlDeleteVertexArrays DeleteVertexArrays { get; } = null!;
    public unsafe delegate void GlDeleteVertexArrays(int count, int[] buffers);

    [GlEntryPoint("glDeleteTextures")]
    public GlDeleteTextures DeleteTextures { get; } = null!;
    public unsafe delegate void GlDeleteTextures(int count, int* textures);

    [GlEntryPoint("glBufferSubData")]
    public GlBufferSubData BufferSubData { get; } = null!;
    public unsafe delegate void GlBufferSubData(int buffer, int offset, int size, void* data);

    [GlEntryPoint("glTexSubImage2D")]
    public GlTexSubImage2D TexSubImage2D { get; } = null!;
    public unsafe delegate void GlTexSubImage2D(int target, int level, int xOff, int yOff, int w, int h, int format, int type, void* pixels);

    [GlEntryPoint("glBindVertexArray")]
    public GlBindVertexArray BindVertexArray { get; } = null!;
    public unsafe delegate void GlBindVertexArray(int array);

    [GlEntryPoint("glGenerateMipmap")]
    public GlGenerateMipmap GenerateMipmap { get; } = null!;
    public unsafe delegate void GlGenerateMipmap(int target);
}