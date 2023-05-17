using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using SquareSmash.renderer.opengl;
using SquareSmash.utils;
namespace SquareSmash.renderer
{
    internal sealed class QuadBatchRenderer : IDisposable
    {
        private readonly int vbo;
        private readonly VertexArray array;
        private readonly ShaderHandle sid;
        private readonly ShaderHandle sid_anti_ghost;
        private readonly ShaderHandle sid_plain;
        private readonly int tex_id;
        private readonly List<Vertex> quadVertices;

        public ShaderHandle GetShader() => sid;
        public QuadBatchRenderer()
        {
            quadVertices = new List<Vertex>();
            
            sid = Shader.Create(AssetUtil.ReadEmbeddedFile("assets.shaders.basic_vertex.glsl"), AssetUtil.ReadEmbeddedFile("assets.shaders.basic_fragment.glsl"));
            sid_anti_ghost = Shader.Create(AssetUtil.ReadEmbeddedFile("assets.shaders.basic_vertex.glsl"), AssetUtil.ReadEmbeddedFile("assets.shaders.anti_ghost_fragment.glsl"));
            sid_plain = Shader.Create(AssetUtil.ReadEmbeddedFile("assets.shaders.basic_vertex.glsl"), AssetUtil.ReadEmbeddedFile("assets.shaders.striped_fragment.glsl"));
            tex_id = Texture.Create("assets.Textures.mask.png");
            vbo = GL.GenBuffer();
            array = new VertexArray();
            array.Bind();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 7 * sizeof(float), 5 * sizeof(float));
            VertexArray.UnBind();
        }

        public void AddQuad(Vector2 position, Vector2 size, Colour3 color)
        {
            CollisionUtil.ToWorldSpace(ref position, ref size);
            var quad = new Vertex[6]; // Use 6 vertices for a quad triangle strip

            quad[0].Position = position;
            quad[0].Color = color;
            quad[0].Uvs = new Vector2(0.0f, 0.0f);

            quad[1].Position = new Vector2(position.X + size.X, position.Y);
            quad[1].Color = color;
            quad[1].Uvs = new Vector2(1.0f, 0.0f);

            quad[2].Position = new Vector2(position.X, position.Y + size.Y);
            quad[2].Color = color;
            quad[2].Uvs = new Vector2(0.0f, 1.0f);

            quad[3].Position = new Vector2(position.X + size.X, position.Y + size.Y);
            quad[3].Color = color;
            quad[3].Uvs = new Vector2(1.0f, 1.0f);

            quad[4].Position = new Vector2(position.X, position.Y + size.Y);
            quad[4].Color = color;
            quad[4].Uvs = new Vector2(0.0f, 1.0f);

            quad[5].Position = new Vector2(position.X + size.X, position.Y);
            quad[5].Color = color;
            quad[5].Uvs = new Vector2(1.0f, 0.0f);

            quadVertices.AddRange(quad);
        }

        public void Flush()
        {
            if (quadVertices.Count == 0)
                return;
            array.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Count * 7 * sizeof(float), quadVertices.ToArray(), BufferUsageHint.DynamicDraw);
            Texture.Bind(tex_id);
            Shader.Bind(sid);
            GL.DrawArrays(PrimitiveType.Triangles, 0, quadVertices.Count);
            VertexArray.UnBind();
            quadVertices.Clear();
        }

        public void FlushAntiGhost()
        {
            if (quadVertices.Count == 0)
                return;
            array.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Count * 7 * sizeof(float), quadVertices.ToArray(), BufferUsageHint.DynamicDraw);
            Texture.Bind(tex_id);
            Shader.Bind(sid_anti_ghost);
            GL.DrawArrays(PrimitiveType.Triangles, 0, quadVertices.Count);
            VertexArray.UnBind();
            quadVertices.Clear();
        }

        public void FlushPlain()
        {
            if (quadVertices.Count == 0)
                return;
            array.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Count * 7 * sizeof(float), quadVertices.ToArray(), BufferUsageHint.DynamicDraw);
            Shader.Bind(sid_plain);
            GL.DrawArrays(PrimitiveType.Triangles, 0, quadVertices.Count);
            VertexArray.UnBind();
            quadVertices.Clear();
        }

        public void Dispose()
        {
            quadVertices.Clear();
            GL.DeleteBuffer(vbo);
            Shader.Delete(sid);
            Shader.Delete(sid_anti_ghost);
            Shader.Delete(sid_plain);
            Texture.Delete(tex_id);
        }
    }
}