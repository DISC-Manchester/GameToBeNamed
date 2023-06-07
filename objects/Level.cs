using OpenTK.Mathematics;
using SquareSmash.objects.components;
using SquareSmash.objects.components.bricks;
using SquareSmash.objects.components.bricks.types;
using SquareSmash.renderer.Windows;
using SquareSmash.utils;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SquareSmash.objects
{
    public class Level
    {
        private int bricks_left;
        protected List<Brick> bricks = new();
        protected Ball ball;

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

        private Brick MakeBrick(BrickType type, Vector2 position, string colour_str)
        {
            Colour3 colour = (BrickDataColour)Enum.Parse(typeof(BrickDataColour), colour_str) switch
            {
                BrickDataColour.DiscPink => new(251, 0, 250),
                BrickDataColour.DiscBlue => new(0, 192, 237),
                BrickDataColour.DiscOrange => new(249, 185, 0),
                BrickDataColour.DiscGreen => new(0, 238, 0),
                _ => throw new ArgumentException("a colour provided in the level is not one implemented in the gmae"),
            };
            return type switch
            {
                BrickType.AIR => new NormalBrick(position, colour, this),
                BrickType.NORMAL => new NormalBrick(position, colour, this),
                BrickType.LIFE => new LifeBrick(position, this),
                _ => throw new ArgumentException("a type provided in the level is not one implemented in the gmae"),
            };
        }

        private void AddBrick(BrickData brick, ref float x, ref float y)
        {
            bricks.Add(MakeBrick(brick.Type, new(x, y), brick.Colour));
            bricks_left++;
            x += 6f;
            if (x > 39.5f)
            {
                x = -39f;
                y -= 6;
            }
        }

        public Level(DiscWindow DiscWindow, string json_level)
        {
            LevelData data = JsonSerializer.Deserialize<LevelData>(AssetUtil.ReadEmbeddedFile(json_level));
            ball = new(DiscWindow.Paddle, data.BaseBallSpeed);
            float x = -39f;
            float y = 100;
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

        public void OnBrickDeath(object sender, EventArgs e) => bricks_left--;

        public void OnRendering(object sender)
        {
            foreach (Brick brick in bricks)
                brick.OnRendering(sender);
            DiscWindow.Instance.GLRenderer.Flush();
            ball.OnRendering(sender);
            DiscWindow.Instance.GLRenderer.FlushAntiGhost();
        }

        private bool send = false;
        public void OnUpdate(object sender, float DeltaTime)
        {
            if (bricks_left <= 0)
            {
                if (!send)
                    ((DiscWindow)sender).LevelWon();
                send = true;
                return;
            }
            foreach (Brick brick in bricks)
                brick.OnUpdate();
            ball.OnUpdate(new Tuple<Level, DiscWindow>(this, (DiscWindow)sender), DeltaTime);
        }
    }
}
