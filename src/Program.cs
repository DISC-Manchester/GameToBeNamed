// See https://aka.ms/new-console-template for more information
using GameToBeNamed.src.renderer;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System.Reflection;
Window game_window = new(new()
{
    Size = new Vector2i(1280, 720),
    Title = Assembly.GetCallingAssembly().GetName().Name,
    Flags = ContextFlags.ForwardCompatible,
});
game_window.Run();
/*GameSqlAccess access = new GameSqlAccess();
Console.WriteLine("Enter your username: ");
string username = Console.ReadLine();
Console.WriteLine("Enter your password: ");
string password = Console.ReadLine();
if (access.login(username, password))
{
    Console.WriteLine("logged in");
}
else
{
    Console.WriteLine("login failed");
}*/
