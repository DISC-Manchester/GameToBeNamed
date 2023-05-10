using OpenTK.Mathematics;
using OpenTK.Graphics.ES30;
using SquareSmash.renderer.opengl;
using SquareSmash.Utils;

namespace SquareSmash.renderer
{
    internal sealed class QuadBatchRenderer : IDisposable
    {
        struct QuadVertex
        {
#pragma warning disable S4487 // Unread "private" fields should be removed
            public Vector2 Position;
            public Color4 Color;
#pragma warning restore S4487 // Unread "private" fields should be removed
        }
        private readonly int vbo;
        private readonly VertexArray array;
        private readonly ShaderHandle sid;
        private readonly List<QuadVertex> quadVertices;
        public QuadBatchRenderer()
        {
            quadVertices = new List<QuadVertex>();
            sid = Shader.Create("#version 330 core\r\nlayout(location = 0) in vec2 position;\r\nlayout(location = 1) in vec4 color;\r\n\r\nout vec4 vertexColor;\r\n\r\nvoid main()\r\n{\r\n    gl_Position = vec4(position, 0.0, 1.0);\r\n    vertexColor = color;\r\n}",
                                  "#version 330 core\r\nin vec4 vertexColor;\r\nout vec4 fragColor;\r\n\r\nvoid main()\r\n{\r\n    fragColor = vertexColor;\r\n}");
            vbo = GL.GenBuffer();
            array = new();
            array.Bind();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 2 * sizeof(float));
            VertexArray.UnBind();
        }

        public void AddQuad(Vector2 position, Vector2 size, Color4 color)
        {
            CollisionUtil.ToWorldSpace(ref position, ref size);
            var quad = new QuadVertex[6]; // Use 6 vertices for a quad triangle strip

            quad[0].Position = position;
            quad[0].Color = color;

            quad[1].Position = new Vector2(position.X + size.X, position.Y);
            quad[1].Color = color;

            quad[2].Position = new Vector2(position.X, position.Y + size.Y);
            quad[2].Color = color;

            quad[3].Position = new Vector2(position.X + size.X, position.Y + size.Y);
            quad[3].Color = color;

            quad[4].Position = new Vector2(position.X, position.Y + size.Y);
            quad[4].Color = color;

            quad[5].Position = new Vector2(position.X + size.X, position.Y);
            quad[5].Color = color;

            quadVertices.AddRange(quad);
        }

        public void Flush()
        {
            if (quadVertices.Count == 0)
                return;
            array.Bind();
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Count * 6 * sizeof(float), quadVertices.ToArray(), BufferUsageHint.DynamicDraw);
            Shader.Bind(sid);
            GL.DrawArrays(PrimitiveType.Triangles, 0, quadVertices.Count);
            VertexArray.UnBind();
            quadVertices.Clear();
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vbo);
            Shader.Delete(sid);
        }
    }
}
