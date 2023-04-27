using SquareSmash.client.renderer;
using OpenTK.Mathematics;
using VertexArray = SquareSmash.client.renderer.opengl.VertexArray;
namespace SquareSmash.client.objects.components
{
    internal class Paddle : GameObjects
    {
        public readonly int id;
        public readonly QuadBuffer quat;
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Paddle()
        {
            Width = 80;
            Height = 20;

            Position = new Vector2
            {
                X = Client.Width / 2 - Width / 2,
                Y = Client.Height - 20
            };
            id = VertexArray.Gen();
            VertexArray.Bind(id);
            quat = new();
            VertexArray.Bind(0);
        }

        public void resetPaddle()
        {
            Position = new Vector2
            {
                X = Client.Width / 2 - Width / 2,
                Y = Client.Height - 20
            };
        }

        public override void OnRendering(object sender, EventArgs e)
        {
            VertexArray.Bind(id);
            VertexArray.DrawVertex(6);
        }

        public override void Dispose()
        {
            quat.Dispose();
            VertexArray.Delete(id);
        }
    }
}
