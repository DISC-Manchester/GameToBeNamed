namespace SquareSmash.renderer
{
    internal interface IRenderer
    {
        public void PreRender();
        public void Render(object item);
        public void PostRender();
    }
}
