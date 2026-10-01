using WinFormsApplicationContext = System.Windows.Forms.ApplicationContext;

namespace AudioControl
{
    internal sealed class LegacyHostApplicationContext : WinFormsApplicationContext
    {
        private readonly IApplicationHost applicationHost;

        public LegacyHostApplicationContext(IApplicationHostFactory hostFactory)
        {
            applicationHost = hostFactory.Create();
            applicationHost.Start();
        }

        protected override void ExitThreadCore()
        {
            applicationHost.Stop();
            applicationHost.Dispose();
            base.ExitThreadCore();
        }
    }
}
