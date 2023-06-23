using Avalonia.OpenGL;
using SquareSmash.utils;
using StbImageSharp;
using System;
using System.IO;
using static Avalonia.OpenGL.GlConsts;

namespace SquareSmash.renderer
{
    public sealed class QuadBatchRenderer : IDisposable
    {
        private readonly GlInterface GL;
        private readonly GlExtrasInterface GLE;
        private readonly int vbo;
        private readonly int vao;
        private readonly int sid;
        private readonly int sid_anti_ghost;
        private readonly int tex_id;
        private readonly Vertex[] quadVertices = new Vertex[714];
        private int vertex_index = 0;

        private static unsafe int CreateTexture(GlInterface GL, GlExtrasInterface EGL, string path)
        {
            int[] texs = new int[1];
            GL.GenTextures(1, texs);
            int id = texs[0];
            GL.ActiveTexture(GL_TEXTURE0);
            GL.BindTexture(GL_TEXTURE_2D, id);
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult image = ImageResult.FromStream(AssetUtil.OpenEmbeddedFile(path), ColorComponents.RedGreenBlueAlpha);
            fixed (void* pdata = image.Data)
                GL.TexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, image.Width, image.Height, 0, GL_RGBA, GL_UNSIGNED_BYTE, new IntPtr(pdata));
            GL.TexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            GL.TexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            GL.TexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
            GL.TexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
            EGL.GenerateMipmap(GL_TEXTURE_2D);
            return id;
        }

        private static int CreateShader(GlInterface GL, string vertex_source, string fragment_source)
        {
            int vid = GL.CreateShader(GL_VERTEX_SHADER);
            string vertexShaderInfoLog = GL.CompileShaderAndGetError(vid, vertex_source);
            if (!string.IsNullOrWhiteSpace(vertexShaderInfoLog))
                throw new FileLoadException($"Vertex shader compilation failed: {vertexShaderInfoLog}");

            int fid = GL.CreateShader(GL_FRAGMENT_SHADER);
            string fragmentShaderInfoLog = GL.CompileShaderAndGetError(fid, fragment_source);
            if (!string.IsNullOrWhiteSpace(fragmentShaderInfoLog))
                throw new FileLoadException($"Fragment shader compilation failed: {fragmentShaderInfoLog}");

            int id = GL.CreateProgram();
            GL.AttachShader(id, vid);
            GL.AttachShader(id, fid);
            string programLinkInfoLog = GL.LinkProgramAndGetError(id);
            if (!string.IsNullOrWhiteSpace(programLinkInfoLog))
                Console.WriteLine($"Shader program linking has problems: {programLinkInfoLog}");
            GL.DeleteShader(vid);
            GL.DeleteShader(fid);
            return id;
        }

        public QuadBatchRenderer(GlInterface gl, GlExtrasInterface ext)
        {
            GL = gl;
            GLE = ext;
            sid = CreateShader(gl, AssetUtil.ReadEmbeddedFile("assets.shaders.basic_vertex.glsl"), AssetUtil.ReadEmbeddedFile("assets.shaders.basic_fragment.glsl"));
            sid_anti_ghost = CreateShader(gl, AssetUtil.ReadEmbeddedFile("assets.shaders.basic_vertex.glsl"), AssetUtil.ReadEmbeddedFile("assets.shaders.anti_ghost_fragment.glsl"));
            tex_id = CreateTexture(gl, ext, "assets.Textures.mask.png");
            vbo = GL.GenBuffer();
            int[] vaos = new int[1];
            GLE.GenVertexArrays(1, vaos);
            vao = vaos[0];
            GLE.BindVertexArray(vao);
            GL.BindBuffer(GL_ARRAY_BUFFER, vbo);
            GL.VertexAttribPointer(0, 2, GL_FLOAT, 0, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 1, GL_FLOAT, 0, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 1, GL_FLOAT, 0, 4 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            GLE.BindVertexArray(0);
        }

        public void AddQuad(Vertex[] Vertices)
        {
            int i = 0;
            for (; vertex_index < 714 && i != Vertices.Length;)
                quadVertices[vertex_index++] = Vertices[i++];
        }

        public Vertex[] AddQuad(float x, float y, float width, float height, float color)
        {
            var quad = VertexUtils.PreMakeQuad(x, y, width, height, color);
            AddQuad(quad);
            return quad;
        }

        private void ShaderFlush(int id)
        {
            GLE.BindVertexArray(vao);
            unsafe
            {
                fixed (void* pdata = quadVertices)
                    GL.BufferData(GL_ARRAY_BUFFER, quadVertices.Length * (4 * sizeof(float)), new IntPtr(pdata), GL_DYNAMIC_DRAW);
            }
            GL.BindTexture(GL_TEXTURE_2D, tex_id);
            GL.UseProgram(id);
            GL.DrawArrays(GL_TRIANGLES, 0, quadVertices.Length);
            GLE.BindVertexArray(0);
            vertex_index = 0;
            for (int i = 0; i < quadVertices.Length; i++)
            {
                quadVertices[i].UvID = 0f;
                quadVertices[i].ColorID = 0f;
                quadVertices[i].X = 0f;
                quadVertices[i].Y = 0f;
            }
        }

        public void Flush() => ShaderFlush(sid);
        public void FlushAntiGhost() => ShaderFlush(sid_anti_ghost);

        public void Dispose()
        {
            GL.DeleteBuffers(1, new[] { vbo });
            GLE.DeleteVertexArrays(1, new[] { vao });
            GL.DeleteProgram(sid);
            GL.DeleteProgram(sid_anti_ghost);
            GL.DeleteTextures(1, new[] { tex_id });
        }
    }
}