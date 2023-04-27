using SquareSmash.client.objects.components;
using OpenTK.Graphics.ES30;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Reflection;

namespace SquareSmash.client
{
    internal class Client : GameWindow
    {
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        public Paddle paddle;

        public delegate void OnRenderingEventHandler(object sender, EventArgs e);
        public event OnRenderingEventHandler OnRendering;
        public Client()
           : base(GameWindowSettings.Default, new()
           {
               Size = new Vector2i(1280, 720),
               Title = Assembly.GetCallingAssembly().GetName().Name,
               Flags = ContextFlags.ForwardCompatible,
           })
        {
            Width = 1280; 
            Height = 720;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            paddle = new();
            OnRendering += paddle.OnRendering;
            GL.ClearColor(0.0f, 0.55f, 0.55f, 1.0f);
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            Width = e.Width;
            Height = e.Height;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (OnRendering is not null)
                OnRendering(this, EventArgs.Empty);
            SwapBuffers();
        }
    }
}
