using OpenTK.Graphics.ES30;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SquareSmash.objects;
using SquareSmash.objects.components;
using SquareSmash.renderer;

namespace SquareSmash
{
    internal class Client : GameWindow
    {
        public static readonly int Width = 720;
        public static readonly int Height = 640;
        private long PreviousTime { get; set; }
        public QuadBatchRenderer Renderer { get; private set; }
        public Paddle paddle;
        public Level level;
        /*private static readonly DebugProc DebugMessageDelegate = OnDebugMessage;
        private static void OnDebugMessage(DebugSource source,DebugType type,int id,DebugSeverity severity,int length,IntPtr pMessage,IntPtr pUserParam)
        {
            string message = Marshal.PtrToStringAnsi(pMessage, length);
            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
            if (type == DebugType.DebugTypeError)
                throw new Exception(message);
        }*/

        public Client()
           : base(GameWindowSettings.Default, new()
           {
               Size = new(Width, Height),
               Title = "DISCout",
               Flags = ContextFlags.ForwardCompatible,
               MinimumSize = new(Width, Height),
               MaximumSize = new(Width, Height)
           })
        {
            //GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            Renderer = new();
            paddle = new();
            level = new();
            PreviousTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            GL.ClearColor(0, 0, 0, 1);
        }
        public void LevelWon()
        {
            level = new Level();
        }

        protected override void OnUnload()
        {
            Renderer.Flush();
            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long DeltaTime = (currentTime - PreviousTime);
            PreviousTime = currentTime;
            paddle?.OnUpdate(this, DeltaTime);
            level?.OnUpdate(this, DeltaTime);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            paddle?.OnRendering(Renderer);
            level?.OnRendering(Renderer);
            Renderer.Flush();
            SwapBuffers();
        }
    }
    static class ProgramMain
    {
        public static void Main()
        {
            Client client = new();
            client.Run();
        }
    }
}