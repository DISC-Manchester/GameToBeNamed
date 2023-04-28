using SquareSmash.client.objects.components;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Reflection;
using SquareSmash.client.renderer;
using System.Runtime.InteropServices;
using SquareSmash.client.objects;
using SquareSmash.client.objects.levels;

namespace SquareSmash.client
{
    internal class Client : GameWindow
    {
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        private double PreviousTime { get; set; }

        public QuadBatchRenderer Renderer { get; private set; }
        public Paddle paddle;
        public List<Wall> walls  = new();
        public Level level;

        public delegate void OnRenderingEventHandler(object sender, EventArgs e);
        public event OnRenderingEventHandler OnRendering;

        public delegate void OnUpdatingEventHandler(object sender, UpdateEventArgs e);
        public event OnUpdatingEventHandler OnUpdating;

        private static readonly DebugProc DebugMessageDelegate = OnDebugMessage;
        private static void OnDebugMessage(DebugSource source,DebugType type,int id,DebugSeverity severity,int length,IntPtr pMessage,IntPtr pUserParam)
        {
            string message = Marshal.PtrToStringAnsi(pMessage, length);
            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
            if (type == DebugType.DebugTypeError)
                throw new Exception(message);
        }

        public Client()
           : base(GameWindowSettings.Default, new()
           {
               Size = new Vector2i(720, 640),
               Title = Assembly.GetCallingAssembly().GetName().Name,
               Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
           })
        {
            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            Width = 720;
            Height = 640;
            Renderer = new(Width, Height);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            paddle = new();
            level = new();
            OnRendering += paddle.OnRendering;
            OnUpdating += paddle.OnUpdate;
            OnRendering += level.OnRendering;
            OnUpdating += level.OnUpdate;
            walls.Add(new(new(0, Height), 20, Height * 2));
            walls.Add(new(new(Width - 10, Height), 20, Height * 2));
            walls.Add(new(new(0, 10), Width * 2, 20));
            foreach (Wall wall in walls)
                OnRendering += wall.OnRendering;
            PreviousTime = this.RenderTime;
            GL.ClearColor(0, 0, 0, 1);
        }

        protected override void OnUnload()
        {
            Renderer.Flush();
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            Width = e.Width;
            Height = e.Height;
            GL.Viewport(0, 0, Width, Height);
            Renderer.OnResize(Width, Height);
            paddle.ResetPaddle();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            double currentTime = this.RenderTime;
            double deltaTime = (currentTime - PreviousTime) * 1000;
            PreviousTime = currentTime;
            if (OnUpdating is not null)
                OnUpdating(this, new(deltaTime));
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (OnRendering is not null)
                OnRendering(Renderer, EventArgs.Empty);
            Renderer.Flush();
            SwapBuffers();
        }
    }
}
