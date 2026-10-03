using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;

namespace GameChatBalancer.Wpf;

public partial class SystrayPopupWindow : Window
{
    private readonly Action openAppAction;
    private readonly Action exitAction;
    private readonly Func<bool> startWithWindowsProvider;
    private readonly Action<bool> startWithWindowsSetter;
    private readonly Func<bool> invertControlProvider;
    private readonly Action<bool> invertControlSetter;
    private float currentBalanceValue = 50f;
    private bool suppressHardwareOptionEvents;

    public SystrayPopupWindow()
        : this(() => { }, () => { }, () => false, _ => { }, () => false, _ => { })
    {
    }

    public SystrayPopupWindow(
        Action openAppAction,
        Action exitAction,
        Func<bool> startWithWindowsProvider,
        Action<bool> startWithWindowsSetter,
        Func<bool> invertControlProvider,
        Action<bool> invertControlSetter)
    {
        this.openAppAction = openAppAction;
        this.exitAction = exitAction;
        this.startWithWindowsProvider = startWithWindowsProvider;
        this.startWithWindowsSetter = startWithWindowsSetter;
        this.invertControlProvider = invertControlProvider;
        this.invertControlSetter = invertControlSetter;

        InitializeComponent();

        Deactivated += (_, _) => Hide();

        Loaded += (_, _) =>
        {
            TryApplyHeaderIconFromFile();

            var toggle = GetStartWithWindowsToggle();
            if (toggle != null)
            {
                toggle.IsChecked = startWithWindowsProvider();
            }

            InitializeHardwareOptions();
            UpdateIndicatorPosition();
        };
    }

    private void TryApplyHeaderIconFromFile()
    {
        try
        {
            if (popupHeaderIcon is null)
            {
                return;
            }

            var iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "GCB_icon.png");
            if (!File.Exists(iconPath))
            {
                return;
            }

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
            bitmap.EndInit();
            bitmap.Freeze();

            popupHeaderIcon.Source = bitmap;
        }
        catch
        {
        }
    }

    public void SetConnectedState(bool connected, string? currentPort = null)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => SetConnectedState(connected, currentPort));
            return;
        }

        statusDot.Fill = connected
            ? (System.Windows.Media.Brush)FindResource("Brush.Success")
            : (System.Windows.Media.Brush)FindResource("Brush.Warning");

        statusTitle.Text = connected ? "Arduino: Connected" : "Arduino: Disconnected";
        statusSubtitle.Text = connected
            ? "Hardware Control"
            : "Software Control";

        connectedControlsSection.Visibility = connected ? Visibility.Visible : Visibility.Collapsed;
        disconnectedControlsSection.Visibility = connected ? Visibility.Collapsed : Visibility.Visible;
    }

    public void SetBalance(float displayVolume, float gamePercent, float chatPercent)
    {
        currentBalanceValue = Math.Clamp(displayVolume, 0f, 100f);
        txtGamePercent.Text = $"{MathF.Round(gamePercent):0}%";
        txtChatPercent.Text = $"{MathF.Round(chatPercent):0}%";
        UpdateIndicatorPosition();
    }

    public void SetStartWithWindowsState(bool enabled)
    {
        var toggle = GetStartWithWindowsToggle();
        if (toggle != null)
        {
            toggle.IsChecked = enabled;
        }
    }

    public void SetInvertControlState(bool invert)
    {
        if (invertControlToggle == null)
        {
            return;
        }

        suppressHardwareOptionEvents = true;
        try
        {
            invertControlToggle.IsChecked = invert;
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }
    }

    private void OpenAppButton_Click(object sender, RoutedEventArgs e)
    {
        openAppAction();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        exitAction();
    }

    private void StartWithWindowsToggle_Changed(object sender, RoutedEventArgs e)
    {
        var toggle = sender as ToggleButton ?? GetStartWithWindowsToggle();
        if (toggle != null)
        {
            startWithWindowsSetter(toggle.IsChecked == true);
        }
    }

    private void InvertControlToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (suppressHardwareOptionEvents)
        {
            return;
        }

        invertControlSetter(invertControlToggle.IsChecked == true);
    }

    private void SystrayPopupWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
        {
            return;
        }

        Hide();
        e.Handled = true;
    }

    private void BalanceTrackHost_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateIndicatorPosition();
    }

    private void UpdateIndicatorPosition()
    {
        if (balanceTrackHost == null || balanceIndicator == null)
        {
            return;
        }

        var trackWidth = Math.Max(0d, balanceTrackHost.ActualWidth);
        if (trackWidth <= 0)
        {
            return;
        }

        var normalized = Math.Clamp(currentBalanceValue, 0f, 100f) / 100d;
        var indicatorWidth = balanceIndicator.Width;
        var x = (trackWidth * normalized) - (indicatorWidth / 2d);
        x = Math.Clamp(x, 0d, Math.Max(0d, trackWidth - indicatorWidth));
        Canvas.SetLeft(balanceIndicator, x);
    }

    private void InitializeHardwareOptions()
    {
        SetInvertControlState(invertControlProvider());
    }

    private ToggleButton? GetStartWithWindowsToggle()
    {
        return FindName("startWithWindowsToggle") as ToggleButton;
    }
}
