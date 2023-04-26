using GameToBeNamed.client.renderer;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System.Reflection;

namespace GameToBeNamed.client
{
    internal class Client
    {
        private readonly Window game_window;
        public Client() {
            game_window = new(new()
            {
                Size = new Vector2i(1280, 720),
                Title = Assembly.GetCallingAssembly().GetName().Name,
                Flags = ContextFlags.ForwardCompatible,
            });
        }

        public void Run()
        {
            game_window.Run();
        }
    }
}
