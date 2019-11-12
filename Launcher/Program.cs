using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Launcher.ViewModels;
using Launcher.Views;
#if Windows
using Microsoft.Win32;

#endif

namespace Launcher
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
#if Windows
            // write in registry for custom http protocol
            using (var mainKey = Registry.CurrentUser.CreateSubKey(@"fubr"))
            {
                mainKey.SetValue("", "URL: fubr");
                mainKey.SetValue("URL Protocol", "");
                var shellKey = mainKey.CreateSubKey("shell");
                var openKey = shellKey.CreateSubKey("command");
                openKey.SetValue("", AppDomain.CurrentDomain.BaseDirectory +
                                     "External\\LoginPassTrough\\win10-x64\\LoginScript.exe");
            }
#endif
            BuildAvaloniaApp().Start(AppMain, args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
        }

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
//            var window = new MainWindow
//            {
//                DataContext = new MainWindowViewModel()
//            };

            var loginDataContext = new LoginViewModel();

            var window = new Login
            {
                DataContext = loginDataContext
            };

            loginDataContext.UserHasLoggedIn += (o, eventArgs) => window.Close();

            app.Run(window);
        }
    }
}