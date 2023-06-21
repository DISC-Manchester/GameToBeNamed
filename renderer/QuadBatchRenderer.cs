
using Avalonia.OpenGL;
using OpenTK.Mathematics;
using SquareSmash.utils;
using StbImageSharp;
using System;
using System.Collections.Generic;
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
        private readonly int sid_plain;
        private readonly int tex_id;
        private readonly List<Vertex> quadVertices;
        public static readonly Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), 0.85915492957f, 0.1f, 100f);
        public static readonly Matrix4 viewMatrix = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero, Vector3.UnitY);

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
            quadVertices = new List<Vertex>();
            sid = CreateShader(gl, AssetUtil.ReadEmbeddedFile("assets.shaders.basic_vertex.glsl"), AssetUtil.ReadEmbeddedFile("assets.shaders.basic_fragment.glsl"));
            sid_anti_ghost = CreateShader(gl, AssetUtil.ReadEmbeddedFile("assets.shaders.basic_vertex.glsl"), AssetUtil.ReadEmbeddedFile("assets.shaders.anti_ghost_fragment.glsl"));
            sid_plain = CreateShader(gl, AssetUtil.ReadEmbeddedFile("assets.shaders.basic_vertex.glsl"), AssetUtil.ReadEmbeddedFile("assets.shaders.striped_fragment.glsl"));
            tex_id = CreateTexture(gl, ext, "assets.Textures.mask.png");
            vbo = GL.GenBuffer();
            int[] vaos = new int[1];
            GLE.GenVertexArrays(1, vaos);
            vao = vaos[0];
            GLE.BindVertexArray(vao);
            GL.BindBuffer(GL_ARRAY_BUFFER, vbo);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, GL_FLOAT, 0, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, GL_FLOAT, 0, 7 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, GL_FLOAT, 0, 7 * sizeof(float), 5 * sizeof(float));
            GLE.BindVertexArray(0);
        }

        // pre calculate the vertex so that we don't need to spent cpu cycles at rime-time to calculate the vertex
        // the can be compliantly expensive 
        public static Vertex[] PreMakeQuad(Vector2 position, Vector2 size, Colour3 color)
        {
            var quad = new Vertex[6]; // Use 6 vertices for a quad triangle strip

            // Apply model transformation
            Matrix4 modelTransform = Matrix4.Identity;
            modelTransform *= Matrix4.CreateTranslation(new Vector3(position.X, position.Y, -1.0f));
            modelTransform *= Matrix4.CreateScale(new Vector3(size.X, size.Y, 1.0f));

            /*  Idea of how vertex are laded out 
             *  0,0-------------------------------------0,1
             *     |   ~                                                  |
             *     |                        ~                             |
             *     |                                                ~     |
             *    1,0-------------------------------------1,1
             */

            Vector4 transform_0 = (new Vector4(-1.0f, -1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_0 = new Vector3(transform_0.X, transform_0.Y, transform_0.Z) / transform_0.Z;
            quad[0].Position = scale_down_0.Xy;
            quad[0].Color = color;
            quad[0].Uvs = new Vector2(0.0f, 0.0f);

            Vector4 transform_1 = (new Vector4(1.0f, -1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_1 = new Vector3(transform_1.X, transform_1.Y, transform_1.Z) / transform_1.Z;
            quad[1].Position = scale_down_1.Xy;
            quad[1].Color = color;
            quad[1].Uvs = new Vector2(1.0f, 0.0f);

            Vector4 transform_2 = (new Vector4(-1.0f, 1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_2 = new Vector3(transform_2.X, transform_2.Y, transform_2.Z) / transform_2.Z;
            quad[2].Position = scale_down_2.Xy;
            quad[2].Color = color;
            quad[2].Uvs = new Vector2(0.0f, 1.0f);

            Vector4 transform_3 = (new Vector4(1.0f, 1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_3 = new Vector3(transform_3.X, transform_3.Y, transform_3.Z) / transform_3.Z;
            quad[3].Position = scale_down_3.Xy;
            quad[3].Color = color;
            quad[3].Uvs = new Vector2(1.0f, 1.0f);

            Vector4 transform_4 = (new Vector4(-1.0f, 1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_4 = new Vector3(transform_4.X, transform_4.Y, transform_4.Z) / transform_4.Z;
            quad[4].Position = scale_down_4.Xy;
            quad[4].Color = color;
            quad[4].Uvs = new Vector2(0.0f, 1.0f);

            Vector4 transform_5 = (new Vector4(1.0f, -1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_5 = new Vector3(transform_5.X, transform_5.Y, transform_5.Z) / transform_5.Z;
            quad[5].Position = scale_down_5.Xy;
            quad[5].Color = color;
            quad[5].Uvs = new Vector2(1.0f, 0.0f);
            return quad;
        }

        public void AddQuad(Vertex[] Vertices)
            => quadVertices.AddRange(Vertices);

        public Vertex[] AddQuad(Vector2 position, Vector2 size, Colour3 color)
        {
            var quad = new Vertex[6]; // Use 6 vertices for a quad triangle strip

            // Apply model transformation
            Matrix4 modelTransform = Matrix4.Identity;
            modelTransform *= Matrix4.CreateTranslation(new Vector3(position.X, position.Y, -1.0f));
            modelTransform *= Matrix4.CreateScale(new Vector3(size.X, size.Y, 1.0f));

            Vector4 transform_0 = (new Vector4(-1.0f, -1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_0 = new Vector3(transform_0.X, transform_0.Y, transform_0.Z) / transform_0.Z;
            quad[0].Position = scale_down_0.Xy;
            quad[0].Color = color;
            quad[0].Uvs = new Vector2(0.0f, 0.0f);

            Vector4 transform_1 = (new Vector4(1.0f, -1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_1 = new Vector3(transform_1.X, transform_1.Y, transform_1.Z) / transform_1.Z;
            quad[1].Position = scale_down_1.Xy;
            quad[1].Color = color;
            quad[1].Uvs = new Vector2(1.0f, 0.0f);

            Vector4 transform_2 = (new Vector4(-1.0f, 1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_2 = new Vector3(transform_2.X, transform_2.Y, transform_2.Z) / transform_2.Z;
            quad[2].Position = scale_down_2.Xy;
            quad[2].Color = color;
            quad[2].Uvs = new Vector2(0.0f, 1.0f);

            Vector4 transform_3 = (new Vector4(1.0f, 1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_3 = new Vector3(transform_3.X, transform_3.Y, transform_3.Z) / transform_3.Z;
            quad[3].Position = scale_down_3.Xy;
            quad[3].Color = color;
            quad[3].Uvs = new Vector2(1.0f, 1.0f);

            Vector4 transform_4 = (new Vector4(-1.0f, 1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_4 = new Vector3(transform_4.X, transform_4.Y, transform_4.Z) / transform_4.Z;
            quad[4].Position = scale_down_4.Xy;
            quad[4].Color = color;
            quad[4].Uvs = new Vector2(0.0f, 1.0f);

            Vector4 transform_5 = (new Vector4(1.0f, -1.0f, -1.0f, 1.0f) * projectionMatrix * viewMatrix * modelTransform);
            Vector3 scale_down_5 = new Vector3(transform_5.X, transform_5.Y, transform_5.Z) / transform_5.Z;
            quad[5].Position = scale_down_5.Xy;
            quad[5].Color = color;
            quad[5].Uvs = new Vector2(1.0f, 0.0f);

            quadVertices.AddRange(quad);
            return quad;
        }

        public unsafe void Flush()
        {
            if (quadVertices.Count == 0)
                return;
            GLE.BindVertexArray(vao);
            fixed (void* pdata = quadVertices.ToArray())
                GL.BufferData(GL_ARRAY_BUFFER, quadVertices.Count * 7 * sizeof(float), new IntPtr(pdata), GL_DYNAMIC_DRAW);

            GL.BindTexture(GL_TEXTURE_2D, tex_id);
            GL.UseProgram(sid);
            GL.DrawArrays(GL_TRIANGLES, 0, quadVertices.Count);
            GLE.BindVertexArray(0);
            quadVertices.Clear();
        }

        public unsafe void FlushAntiGhost()
        {
            if (quadVertices.Count == 0)
                return;
            GLE.BindVertexArray(vao);
            fixed (void* pdata = quadVertices.ToArray())
                GL.BufferData(GL_ARRAY_BUFFER, quadVertices.Count * 7 * sizeof(float), new IntPtr(pdata), GL_DYNAMIC_DRAW);
            GL.BindTexture(GL_TEXTURE_2D, tex_id);
            GL.UseProgram(sid_anti_ghost);
            GL.DrawArrays(GL_TRIANGLES, 0, quadVertices.Count);
            GLE.BindVertexArray(0);
            quadVertices.Clear();
        }

        public unsafe void FlushPlain()
        {
            if (quadVertices.Count == 0)
                return;
            GLE.BindVertexArray(vao);
            fixed (void* pdata = quadVertices.ToArray())
                GL.BufferData(GL_ARRAY_BUFFER, quadVertices.Count * 7 * sizeof(float), new IntPtr(pdata), GL_DYNAMIC_DRAW);
            GL.UseProgram(sid_plain);
            GL.DrawArrays(GL_TRIANGLES, 0, quadVertices.Count);
            GLE.BindVertexArray(0);
            quadVertices.Clear();
        }

        public void Dispose()
        {
            quadVertices.Clear();
            GL.DeleteBuffers(1, new[] { vbo });
            GLE.DeleteVertexArrays(1, new[] { vao });
            GL.DeleteProgram(sid);
            GL.DeleteProgram(sid_anti_ghost);
            GL.DeleteProgram(sid_plain);
            GL.DeleteTextures(1, new[] { tex_id });
        }
    }
}