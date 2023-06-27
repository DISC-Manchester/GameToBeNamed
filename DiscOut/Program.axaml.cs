using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DiscOut.Avalonia;
using System;

namespace DiscOut
{
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
            Console.Write($"DISCout  Copyright (C) 2023-{DateTime.Now.Year}  DISC (Digital Independent specialist College) \nThis program comes with ABSOLUTELY NO WARRANTY;\n");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        public static AppBuilder BuildAvaloniaApp() => AppBuilderDesktopExtensions.UsePlatformDetect().With(new Win32PlatformOptions { UseWgl = true }).LogToTrace();
    }
}