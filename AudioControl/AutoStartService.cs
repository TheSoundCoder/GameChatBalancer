using Microsoft.Win32;

namespace AudioControl
{
    public sealed class AutoStartService : IAutoStartService
    {
        private const string RunKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AppName = "GameChatBalancer";

        public bool IsEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
                var currentValue = key?.GetValue(AppName) as string;
                return !string.IsNullOrWhiteSpace(currentValue);
            }
            catch
            {
                return false;
            }
        }

        public bool TrySetEnabled(bool enabled, out string? errorMessage)
        {
            errorMessage = null;

            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
                                ?? Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true);

                if (key == null)
                {
                    errorMessage = "Run registry key could not be opened.";
                    return false;
                }

                if (enabled)
                {
                    var exePath = Environment.ProcessPath;
                    if (string.IsNullOrWhiteSpace(exePath))
                    {
                        errorMessage = "Executable path could not be determined.";
                        return false;
                    }

                    key.SetValue(AppName, $"\"{exePath}\"");
                }
                else
                {
                    key.DeleteValue(AppName, throwOnMissingValue: false);
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
