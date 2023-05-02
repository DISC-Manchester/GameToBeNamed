using OpenTK.Mathematics;
using SquareSmash.objects.components;
using SquareSmash.renderer;
namespace SquareSmash.objects
{
    internal class Level : GameObject
    {
        private int bricks_left = 112;
        protected List<Brick> bricks = new();
        protected Ball ball = new();
        protected Wall left;
        protected Wall top;
        protected Wall right;
        public Level()
        {
            left = new(new(0, Client.Height), 20, Client.Height * 2);
            top = new(new(0, 10), Client.Width * 2, 20);
            right = new(new(Client.Width - 10, Client.Height), 20, Client.Height * 2);
            uint BRICK_PADDING = 16;
            float x = BRICK_PADDING;
            float y = 120;
            for (uint i = 0; i < 112; i++)
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

                bricks.Add(new(new(x, y), colour, this));
                x += Client.Width / 11.0f - BRICK_PADDING;
                if (x > Client.Width - 20)
                {
                    x = BRICK_PADDING;
                    y += Brick.Height - 8;
                }
            }
        }

        public List<Brick> GetBricks() => bricks;
        public Wall GetWallLeft() => left;
        public Wall GetWallTop() => top;
        public Wall GetWallRight() => right;

        public void OnBrickDeath(object sender, EventArgs e) => bricks_left--;

        public override void OnRendering(object sender)
        {
            left.OnRendering(sender);
            top.OnRendering(sender);
            right.OnRendering(sender);
            foreach (Brick brick in bricks)
                brick.OnRendering(sender);
            ball.OnRendering(sender);
        }

        private bool send = false;
        public override void OnUpdate(object sender, long DeltaTime)
        {
            if (bricks_left <= 0)
            {
                if (!send)
                    ((Client)sender).LevelWon();
                send = true;
                return;
            }
            ball.OnUpdate(new Tuple<Level, Client>(this, (Client)sender), DeltaTime);
        }
    }
}
