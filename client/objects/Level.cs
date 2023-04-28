using OpenTK.Mathematics;
using SquareSmash.client.objects.components;
using SquareSmash.client.renderer;

namespace SquareSmash.client.objects
{
    internal class Level
    : GameObject
    {
        private static readonly uint MAX_BRICKS = 112;
        private static readonly uint BRICK_PADDING = 16;
        protected List<Brick> bricks = new();
        protected Ball ball = new();
        public Level()
        {
            float x = BRICK_PADDING;
            float y = 40;
            for (uint i = 0; i < MAX_BRICKS; i++)
            {
                Color4 colour;
                if (i < 28)
                    colour = Colours.DiscPink;
                else if (i < 56)
                    colour = Colours.DiscBlue;
                else if (i < 84)
                    colour = Colours.DiscOrange;
                else 
                    colour = Colours.DiscGreen;

                bricks.Add(new(new(x, y), colour));
                x += (Client.Width / 11.0f) - BRICK_PADDING;
                if (x > Client.Width - 20)
                {
                    x = BRICK_PADDING;
                    y += Brick.Height - 8;
                }
            }
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public List<Brick> GetBricks() => bricks;

        public override void OnRendering(object sender, EventArgs e)
        {
            foreach (Brick brick in bricks)
                brick.OnRendering(sender, e);
            ball.OnRendering(sender, e);
        }

        public override void OnUpdate(object sender, UpdateEventArgs e)
        {
            foreach (Brick brick in bricks)
                brick.OnUpdate(sender, e);
            ball.OnUpdate(new Tuple<Level, Client>(this,(Client)sender), e);
        }
    }
}
