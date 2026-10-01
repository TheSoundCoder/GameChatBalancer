namespace AudioControl
{
    public interface IDiagnosticsSink
    {
        bool DebugEnabled { get; }
        void Log(string message);
    }
}
