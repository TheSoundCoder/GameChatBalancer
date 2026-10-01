using System;
using System.Windows;
using System.Windows.Threading;
using GameChatBalancer.Wpf;
using WpfApplication = System.Windows.Application;

namespace AudioControl
{
    internal sealed class WpfOverlayService : IOverlayService
    {
        private readonly Dispatcher dispatcher;
        private readonly DispatcherTimer autoHideTimer;
        private readonly Action<string>? logAction;
        private OverlayWindow? overlayWindow;
        private bool enabled = true;
        private string position = "BottomCenter";
        private double opacity = 0.9;
        private bool faulted;

        public WpfOverlayService(TimeSpan autoHideDuration, Action<string>? logAction = null)
        {
            dispatcher = WpfApplication.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
            this.logAction = logAction;
            autoHideTimer = new DispatcherTimer(autoHideDuration, DispatcherPriority.Normal, AutoHideTimer_Tick, dispatcher)
            {
                IsEnabled = false
            };
        }

        public void Show(float gameLevel, float chatLevel)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(() => Show(gameLevel, chatLevel));
                return;
            }

            if (!enabled)
            {
                return;
            }

            ExecuteSafely(() =>
            {
                EnsureWindow();
                overlayWindow!.SetLevels(gameLevel, chatLevel);
                overlayWindow.SetOverlayOpacity(opacity);
                overlayWindow.SetPosition(position);
                overlayWindow.ShowOverlay();
                RestartAutoHideTimer();
            }, "Show");
        }

        public void Update(float gameLevel, float chatLevel)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(() => Update(gameLevel, chatLevel));
                return;
            }

            if (!enabled)
            {
                return;
            }

            ExecuteSafely(() =>
            {
                EnsureWindow();
                overlayWindow!.SetLevels(gameLevel, chatLevel);
                overlayWindow.SetOverlayOpacity(opacity);
                overlayWindow.SetPosition(position);
                if (!overlayWindow.IsVisible)
                {
                    overlayWindow.ShowOverlay();
                }

                RestartAutoHideTimer();
            }, "Update");
        }

        public void Hide()
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(Hide);
                return;
            }

            ExecuteSafely(() =>
            {
                autoHideTimer.Stop();
                overlayWindow?.HideOverlay();
            }, "Hide");
        }

        public void ApplyConfiguration(bool enabled, int durationSeconds, string position, double opacity)
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(() => ApplyConfiguration(enabled, durationSeconds, position, opacity));
                return;
            }

            this.enabled = enabled;
            this.position = string.IsNullOrWhiteSpace(position) ? "BottomCenter" : position;
            this.opacity = Math.Clamp(opacity, 0.2, 1.0);

            var clampedDuration = Math.Clamp(durationSeconds, 1, 10);
            autoHideTimer.Interval = TimeSpan.FromSeconds(clampedDuration);

            if (!enabled)
            {
                Hide();
                return;
            }

            ExecuteSafely(() =>
            {
                if (overlayWindow != null)
                {
                    overlayWindow.SetOverlayOpacity(this.opacity);
                    overlayWindow.SetPosition(this.position);
                }
            }, "ApplyConfiguration");
        }

        public void Dispose()
        {
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke(Dispose);
                return;
            }

            autoHideTimer.Stop();
            if (overlayWindow != null)
            {
                overlayWindow.Close();
                overlayWindow = null;
            }
        }

        private void AutoHideTimer_Tick(object? sender, EventArgs e)
        {
            Hide();
        }

        private void EnsureWindow()
        {
            if (overlayWindow != null)
            {
                return;
            }

            overlayWindow = new OverlayWindow();
            overlayWindow.Closed += (_, _) => overlayWindow = null;
        }

        private void RestartAutoHideTimer()
        {
            autoHideTimer.Stop();
            autoHideTimer.Start();
        }

        private void ExecuteSafely(Action action, string context)
        {
            if (faulted)
            {
                return;
            }

            try
            {
                action();
            }
            catch (Exception ex)
            {
                faulted = true;
                enabled = false;
                autoHideTimer.Stop();

                try
                {
                    overlayWindow?.Close();
                }
                catch
                {
                }

                overlayWindow = null;
                logAction?.Invoke($"Overlay disabled after {context} failure: {ex.Message}");
            }
        }
    }
}
