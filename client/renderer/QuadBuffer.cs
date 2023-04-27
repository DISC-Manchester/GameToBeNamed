using Buffer = SquareSmash.client.renderer.opengl.Buffer;
using VertexArray = SquareSmash.client.renderer.opengl.VertexArray;
namespace SquareSmash.client.renderer
{
    public sealed class QuadBuffer : IDisposable
    {
        private readonly int id;
        public QuadBuffer() {
            float[] vertices = {-0.5f, -0.5f, -0.5f, 0.5f, -0.5f, -0.5f,0.5f,  0.5f, -0.5f, 0.5f,  0.5f, -0.5f,-0.5f,  0.5f, -0.5f,-0.5f, -0.5f, -0.5f};
            id = Buffer.Gen();
            Buffer.VertexBind(id);
            Buffer.VertexData(vertices, false);
            VertexArray.EnableAttrib(0);
            VertexArray.AttribPointer(0,3,OpenTK.Graphics.ES30.VertexAttribPointerType.Float,3 * sizeof(float),0);
        }

        public void Dispose()
        {
            VertexArray.DisableAttrib(0);
            Buffer.Delete(id);
        }
    }
}
