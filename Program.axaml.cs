using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SquareSmash.renderer.Windows;
using System;

namespace SquareSmash;

public partial class Program : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new DiscWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }


}

public static class Entry
{
    [STAThread]
    public static void Main(string[] args)
    {
        /*foreach(string s in Assembly.GetExecutingAssembly().GetManifestResourceNames())
        {
            Console.WriteLine(s);
        }*/
        Console.Write($"DISCout  Copyright (C) 2023-{DateTime.Now.Year}  DISC (Digital Independant specialist College) \nThis program comes with ABSOLUTELY NO WARRANTY;\n");
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<Program>().UsePlatformDetect().With(new Win32PlatformOptions { UseWgl = true }).LogToTrace();
}