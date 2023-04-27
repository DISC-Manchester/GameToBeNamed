using SquareSmash.client.network;
using SquareSmash.share;

namespace SquareSmash.server
{
    internal class Server
    {
        public Server() { }

        public async Task RunAsync()
        {
            GameDataBaseAccess access = new();
            ErrorOrData data = await access.LoginAsync("test", NetUtil.MD5Hash("TestingPassword445"));
            if (data.Unwrap() is not string && data.Unwrap() is not false)
            {
                Console.WriteLine("logged in");
            }
            else
            {
                Console.WriteLine("login failed");
            }
        }
    }
}
