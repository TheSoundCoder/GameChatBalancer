namespace AudioControl
{
    internal interface IApplicationHost : IDisposable
    {
        void Start();
        void Stop();
    }
}
