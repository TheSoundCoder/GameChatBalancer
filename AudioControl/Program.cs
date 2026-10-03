using GameChatBalancer.Wpf;
using WpfApplication = System.Windows.Application;
using WpfShutdownMode = System.Windows.ShutdownMode;
using WinFormsApplication = System.Windows.Forms.Application;

namespace AudioControl
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            if (WpfApplication.Current == null)
            {
                var wpfApp = new App
                {
                    ShutdownMode = WpfShutdownMode.OnExplicitShutdown
                };

                wpfApp.InitializeComponent();
            }

            IApplicationHostFactory hostFactory = new LegacyApplicationHostFactory();
            WinFormsApplication.Run(new LegacyHostApplicationContext(hostFactory));
        }
    }
}