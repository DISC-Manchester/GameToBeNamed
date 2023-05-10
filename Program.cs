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
using System.Runtime.InteropServices;

namespace SquareSmash
{
    internal class Client : GameWindow
    {
        private static void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUserParam)
        {
            if (severity == DebugSeverity.DebugSeverityNotification || severity == DebugSeverity.DebugSeverityLow || severity == DebugSeverity.DebugSeverityMedium)
                return;
            string message = Marshal.PtrToStringAnsi(pMessage, length);
            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
            if (type == DebugType.DebugTypeError)
                throw new InvalidOperationException(message);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            Controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            Controller.PressChar((char)e.Unicode);
            PreviousTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            GL.ClearColor(0, 0, 0, 1);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            ImGuiController.MouseScroll(e.Offset);
        }

        protected override void OnUnload()
        {
            Renderer.Flush();
            ImGui.DestroyContext();
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long DeltaTime = (currentTime - PreviousTime);
            Controller.Update(this, DeltaTime / 1000.0f);
            PreviousTime = currentTime;
            if (GameRestart)
            {
                if (KeyboardState.IsKeyDown(Keys.Enter))
                {
                    GameRestart = false;
                    level = new Level(this);
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
            Paddle.OnRendering(Renderer);
            level.OnRendering(Renderer);
            Renderer.Flush();
            var screen_size = ImGui.GetIO().DisplaySize;
            ImGui.SetNextWindowPos(new System.Numerics.Vector2((screen_size.X / 2), (screen_size.Y / 2)), ImGuiCond.Always, new System.Numerics.Vector2(0.5f, 0.5f));
            bool temp = false;
            if (GameRestart)
            {
                ImGui.Begin("Text", ref temp, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus);
                ImGui.SetWindowFontScale(3.0f);
                ImGui.SetWindowSize(new(500, ImGui.GetTextLineHeightWithSpacing() * 2));
                ImGui.Text("Press Space To Restart");
                ImGui.Text("\tFinal Score: " + Convert.ToString(LastScore));
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
            ImGuiController.CheckGLError("End of frame");
            SwapBuffers();
        }

        public Client(int WidthIn, int HeightIn)
           : base(GameWindowSettings.Default, new()
           {
               Size = new(WidthIn, HeightIn),
               Title = "DISCout",
               Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
               MinimumSize = new(WidthIn, HeightIn),
               MaximumSize = new(WidthIn, HeightIn)
           })
        {
            Instance = this;
            GameRestart = false;
            Width = WidthIn;
            Height = HeightIn;
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);
            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            GL.ClearColor(Color4.Black);
            Controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            Renderer = new();
            Paddle = new();
            level = new(this);
            PreviousTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public void LevelWon()
        {
            level = new Level(this);
        }

        private int LastScore = 0;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static Client Instance { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Width { get; private set; }
        public int Height { get; private set; }
        private long PreviousTime { get; set; }
        public QuadBatchRenderer Renderer { get; private set; }
        public Paddle Paddle { get; private set; }
        public bool GameRestart { get; private set; }
        public Level level;
        public ImGuiController Controller;
        private static readonly DebugProc DebugMessageDelegate = OnDebugMessage;
    }
    static class ProgramMain
    {
        public static void Main()
        {
            Client client = new(720, 640);
            client.Run();
        }
    }
}