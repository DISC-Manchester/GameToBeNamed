using GameToBeNamed.client.renderer.gui;
using OpenTK.Graphics.ES30;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.ComponentModel;

namespace GameToBeNamed.client.renderer
{
    internal class Window : GameWindow
    {
        private IGui? ActiveUi;
        public Window(NativeWindowSettings nativeWindowSettings)
           : base(GameWindowSettings.Default, nativeWindowSettings)
        {
        }

        public void changeUi(IGui NewUi)
        {
            ActiveUi?.OnUnload();
            ActiveUi = NewUi;
            ActiveUi.OnLoad();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.0f, 0.55f, 0.55f, 1.0f);
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            ActiveUi?.OnRender();
            SwapBuffers();
        }
    }
}