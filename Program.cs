using ImGuiNET;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SquareSmash.objects;
using SquareSmash.objects.components;
using SquareSmash.renderer;
using SquareSmash.renderer.gui;
using System.Diagnostics;
#if DEBUG
using System.Runtime.InteropServices;
#endif
namespace SquareSmash
{
    internal class Client : GameWindow
    {
#if DEBUG
        private static void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
        {
            if (severity == DebugSeverity.DebugSeverityNotification || severity == DebugSeverity.DebugSeverityLow || severity == DebugSeverity.DebugSeverityMedium)
                return;
            string message = Marshal.PtrToStringAnsi(pMessage, length);
            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
            if (type == DebugType.DebugTypeError)
                throw new InvalidOperationException(message);
        }
#endif

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            float DeltaTime = (float)stopwatch.Elapsed.TotalMilliseconds;
            stopwatch.Restart();
            // Limit the frame time if it's lower than the target frame time
            if (DeltaTime < 16)
            {
                int sleepTime = 16 - (int)DeltaTime;
                // Sleep for the remaining time to limit the frame rate
                Thread.Sleep(sleepTime);
            }
            Controller.Update(this, DeltaTime);
            if (GameRestart)
            {
                if (KeyboardState.IsKeyDown(Keys.Enter))
                {
                    GameRestart = false;
                    level = new(this, "assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
                }
                else
                    return;
            }
            if (Paddle.IsDead())
            {
                Paddle.ResetPaddle();
                LastScore = level.GetBall().GetScore();
                GameRestart = true;
            }
            Paddle.OnUpdate(this, DeltaTime);
            level.OnUpdate(this, DeltaTime);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            Renderer.AddQuad(new(0, Client.Instance.Size.Y), new(20, Client.Instance.Size.Y * 2), new(211, 211, 211));
            Renderer.AddQuad(new(0, 10), new(Client.Instance.Size.X * 2, 20), new(211, 211, 211));
            Renderer.AddQuad(new(Client.Instance.Size.X - 10, Client.Instance.Size.Y), new(20, Client.Instance.Size.Y * 2), new(211, 211, 211));
            Client.Instance.Renderer.FlushPlain();
            Paddle.OnRendering(Renderer);
            level.OnRendering(Renderer);
            var screen_size = ImGui.GetIO().DisplaySize;
            ImGui.SetNextWindowPos(new System.Numerics.Vector2((screen_size.X / 2), (screen_size.Y / 2)), ImGuiCond.Always, new System.Numerics.Vector2(0.5f, 0.5f));
            bool temp = false;
            if (GameRestart)
            {
                ImGui.Begin("Text", ref temp, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus);
                ImGui.SetWindowFontScale(3.0f);
                ImGui.SetWindowSize(new(500, ImGui.GetTextLineHeightWithSpacing() * 2));
                ImGui.Text("\tFinal Score: " + Convert.ToString(LastScore));
                ImGui.Text("Press Enter To Restart");
                ImGui.End();
            }
            else
            {
                if (!level.GetBall().IsAlive())
                {
                    ImGui.Begin("Text", ref temp, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus);
                    ImGui.SetWindowFontScale(3.0f);
                    ImGui.Text("Press Space To Start");
                    ImGui.End();
                }
            }
            Controller.Render();
#if DEBUG
            ImGuiController.CheckGLError("End of frame");
#endif
            SwapBuffers();
        }

        ~Client()
        {
            stopwatch.Stop();
            Renderer.Dispose();
            ImGui.DestroyContext();
        }

        public Client(Vector2i size)
           : base(GameWindowSettings.Default, new()
           {
               Size = size,
#if DEBUG
               Title = "DISCout - Debug Build",
#else
               Title = "DISCout",
#endif
               Flags = ContextFlags.ForwardCompatible
#if DEBUG
               | ContextFlags.Debug,
#else
               ,
#endif
               MinimumSize = size,
               MaximumSize =size
           })
        {
            Instance = this;
#if DEBUG
            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
#endif
            Controller = new();
            Renderer = new();
            Paddle = new();
            level = new(this, "assets.levels.level_1.json");
            stopwatch.Start();
        }

        public void LevelWon()
        {
            CurrentLevel++;
            level = new Level(this, "assets.levels.level_" + Convert.ToString(CurrentLevel) + ".json");
        }

        private int LastScore = 0;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static Client Instance { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public QuadBatchRenderer Renderer { get; private set; }
        public Paddle Paddle { get; private set; }

        private readonly Stopwatch stopwatch = new();
        public bool GameRestart { get; private set; } = false;
        private int CurrentLevel = 1;
        public Level level;
        public ImGuiController Controller;
#if DEBUG
        private static readonly DebugProc DebugMessageDelegate = OnDebugMessage;
#endif
    }
    static class ProgramMain
    {
        public static void Main()
        {
            Client client = new(new(720, 640));
            client.Run();
        }
    }
}