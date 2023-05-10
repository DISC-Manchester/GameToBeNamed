using OpenTK.Compute.OpenCL;
using OpenTK.Mathematics;
using SquareSmash.objects.components;
using SquareSmash.objects.components.bricks;
using SquareSmash.objects.components.bricks.types;
using SquareSmash.renderer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SquareSmash.objects
{
    internal class Level : GameObject
    {
        private int bricks_left;
        protected List<Brick> bricks = new();
        protected Ball ball;
        protected readonly Wall left = new(new(0, Client.Instance.Height), 20, Client.Instance.Height * 2);
        protected readonly Wall top = new(new(0, 10), Client.Instance.Width * 2, 20);
        protected readonly Wall right = new(new(Client.Instance.Width - 10, Client.Instance.Height), 20, Client.Instance.Height * 2);

        protected class BrickData
        {
            public string Colour { get; set; } = "";
            public BrickType Type { get; set; } = BrickType.NORMAL;
            [JsonPropertyName("Repeat")]
            public int? LoopNullable { get; set; } = null;
            public int Loop
            {
                get { return LoopNullable ?? 0; }
                set { LoopNullable = value; }
            }
        }

        protected struct LevelData
        {
            public float BaseBallSpeed { get; set; }
            public BrickData[] Bricks { get; set; }
        };

        protected enum BrickDataColour
        {
            DiscPink,
            DiscBlue,
            DiscOrange,
            DiscGreen,
        };

        private Color4 GetColour(BrickDataColour colour)
        {
            return colour switch
            {
                BrickDataColour.DiscPink => Colours.DiscPink,
                BrickDataColour.DiscBlue => Colours.DiscBlue,
                BrickDataColour.DiscOrange => Colours.DiscOrange,
                BrickDataColour.DiscGreen => Colours.DiscGreen,
                _ => throw new ArgumentException("a colour provided in the level is not one implemented in the gmae"),
            };
        }

        private Brick MakeBrick(BrickType type, Vector2 position, string colour_str)
        {
            Color4 colour = GetColour((BrickDataColour)Enum.Parse(typeof(BrickDataColour), colour_str));
            return type switch
            {
                BrickType.AIR => new NormalBrick(position, colour, this),
                BrickType.NORMAL => new NormalBrick(position, colour, this),
                _ => throw new ArgumentException("a type provided in the level is not one implemented in the gmae"),
            };
        }

        private void AddBrick(BrickData brick, ref float x, ref float y)
        {
            bricks.Add(MakeBrick(brick.Type, new(x, y), brick.Colour));
            bricks_left++;
            x += Client.Instance.Width / 11.0f - 16;
            if (x > Client.Instance.Width - 20)
            {
                x = 16;
                y += Brick.Height - 8;
            }
        }

        public Level(Client client, string json_level)
        {
            LevelData data = JsonSerializer.Deserialize<LevelData>(File.ReadAllText(json_level));
            ball = new(client.Paddle, data.BaseBallSpeed);
            uint BRICK_PADDING = 16;
            float x = BRICK_PADDING;
            float y = 120;
            foreach (BrickData brick in data.Bricks)
            {
                if (brick.Loop == 0)
                    AddBrick(brick, ref x, ref y);
                else for (int i = 0; i < brick.Loop; i++)
                        AddBrick(brick, ref x, ref y);
            }
        }
        public Ball GetBall() => ball;
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
