using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using static Avalonia.OpenGL.GlConsts;

namespace SquareSmash.renderer.Windows.Controls
{
    public partial class GameOpenGLControl : OpenGlControlBase
    {
        public QuadBatchRenderer? Renderer { get; private set; }
        public GameOpenGLControl()
            => InitializeComponent();

        protected override unsafe void OnOpenGlRender(GlInterface gl, int fb)
        {
            gl.ClearColor(0, 0, 0, 0);
            gl.Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
            gl.Viewport(0, 0, (int)DiscWindow.Instance.Width - ((int)DiscWindow.Instance.Width / 16), (int)DiscWindow.Instance.Height - ((int)DiscWindow.Instance.Height / 10));
            DiscWindow.Instance.Paddle.OnRendering(Renderer!);
            DiscWindow.Instance.Level.OnRendering(Renderer!);
        }

        protected override void OnOpenGlInit(GlInterface gl, int fb)
            => Renderer = new(gl, new GlExtrasInterface(gl));

        protected override void OnOpenGlDeinit(GlInterface gl, int fb)
            => Renderer!.Dispose();
    }
}
