namespace AudioControl
{
    public interface ISerialHostBridge : IDiagnosticsSink
    {
        bool IsDispatchRequired { get; }
        bool CanDispatch { get; }
        void Dispatch(Action action);

        void SetSystrayCom(string value);
        void SetConnected(bool connected);
        void SetInitialized(bool initialized);
        void RefreshComPortOptions();

        void SendNoiseReductionValue();
        void ConfirmNoiseReduction();
        void ApplyHardwareVolume(float volume);
    }
}
