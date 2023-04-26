namespace GameToBeNamed.client.renderer.gui
{
    internal interface IGui
    {
        public void OnLoad();
        public void OnUpdate();
        public void OnRender();
        public void OnUnload();
    }
}
