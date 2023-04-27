using OpenTK.Mathematics;
using OpenTK.Graphics.ES30;
using SquareSmash.client.renderer.opengl;
using Buffer = SquareSmash.client.renderer.opengl.Buffer;
using static SquareSmash.client.renderer.opengl.Shader;

namespace SquareSmash.client.renderer
{
    internal class QuadBatchRenderer
    {
        struct QuadVertex
        {
            public Vector2 Position;
            public Color4 Color;
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        private readonly int vbo;
        private readonly int vao;
        private readonly ShaderHandle sid;
        private readonly List<QuadVertex> quadVertices;
        public QuadBatchRenderer(int Width,int Height)
        {
            this.Width = Width;
            this.Height = Height;
            quadVertices = new List<QuadVertex>();
            sid = Create("#version 330 core\r\nlayout(location = 0) in vec2 position;\r\nlayout(location = 1) in vec4 color;\r\n\r\nout vec4 vertexColor;\r\n\r\nvoid main()\r\n{\r\n    gl_Position = vec4(position, 0.0, 1.0);\r\n    vertexColor = color;\r\n}", 
                                  "#version 330 core\r\nin vec4 vertexColor;\r\nout vec4 fragColor;\r\n\r\nvoid main()\r\n{\r\n    fragColor = vertexColor;\r\n}");
            vbo = Buffer.Gen();
            vao = VertexArray.Gen();
            VertexArray.Bind(vao);
            Buffer.VertexBind(vbo);
            VertexArray.EnableAttrib(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            VertexArray.EnableAttrib(1);
            VertexArray.AttribPointer(1, 4, VertexAttribPointerType.Float, 6 * sizeof(float), 2 * sizeof(float));
            VertexArray.Bind(0);
        }

        public void OnResize(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public void AddQuad(Vector2 position, Vector2 size, Color4 color)
        {
            position.X = (position.X - Width / 2f) / (Width / 2f);
            position.Y = (Height / 2f - position.Y) / (Height / 2f);

            size.X /= Width;
            size.Y /= Height;
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
            VertexArray.Bind(vao);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Count * (6 * sizeof(float)), quadVertices.ToArray(), BufferUsageHint.DynamicDraw);
            Bind(sid);
            VertexArray.DrawVertex(quadVertices.Count);
            VertexArray.Bind(0);
            quadVertices.Clear();
        }
    }
}
