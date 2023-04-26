using GameToBeNamed.client;
using GameToBeNamed.server;
Console.WriteLine("mode ('server'|'client'[default]):");
string? mode = Console.ReadLine();
mode ??= "client";
if (mode == "") mode = "client";
if (mode == "client")
{
    Console.WriteLine("mode selected: client");
    Client c = new();
    c.Run(client);
}
else if (mode == "server")
{
    Console.WriteLine("mode selected: server");
    Server s = new();
    _ = s.RunAsync();
}
else Console.WriteLine("mode not accepted");
Console.ReadLine();