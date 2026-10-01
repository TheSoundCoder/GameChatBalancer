using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace GameChatBalancer.Wpf;

public partial class MainWindow : Window
{
    private const int WmHotkey = 0x0312;
    private const int HotkeyIdDecrease = 0x5101;
    private const int HotkeyIdIncrease = 0x5102;
    private const int HotkeyIdCenter = 0x5103;
    private const uint ModControl = 0x0002;
    private const uint ModShift = 0x0004;
    private const uint VkLeft = 0x25;
    private const uint VkRight = 0x27;
    private const uint VkDown = 0x28;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private float currentArduinoValue = 50f;
    private readonly Func<(bool Connected, string CurrentPort)> hardwareStatusProvider;
    private readonly Func<string> noiseReductionProvider;
    private readonly Action<string> noiseReductionSetter;
    private readonly Func<bool> invertControlProvider;
    private readonly Action<bool> invertControlSetter;
    private readonly Func<string> gameAppsProvider;
    private readonly Action<string> gameAppsSetter;
    private readonly Func<string> chatAppsProvider;
    private readonly Action<string> chatAppsSetter;
    private readonly Func<IEnumerable<string>> availableAppsProvider;
    private readonly Func<string, string?> assignedAppPathProvider;
    private readonly Action rescanAudioSessionsAction;
    private readonly Func<IEnumerable<string>> debugMessagesProvider;
    private readonly Action<string> debugLogAction;
    private readonly Action<bool> debugModeSetter;
    private readonly Action<float> softwareBalanceSetter;
    private readonly Func<bool> startWithWindowsProvider;
    private readonly Action<bool> startWithWindowsSetter;
    private readonly Func<bool> overlayEnabledProvider;
    private readonly Action<bool> overlayEnabledSetter;
    private readonly Func<int> overlayDurationProvider;
    private readonly Action<int> overlayDurationSetter;
    private readonly Func<string> overlayPositionProvider;
    private readonly Action<string> overlayPositionSetter;
    private readonly Func<double> overlayOpacityProvider;
    private readonly Action<double> overlayOpacitySetter;
    private bool showDebugOption;
    private readonly IAppIconService appIconService;
    private bool suppressHardwareOptionEvents;
    private readonly DispatcherTimer noiseReductionAckTimeoutTimer = new() { Interval = TimeSpan.FromSeconds(3) };
    private string? pendingNoiseReductionLevel;
    private Point? dragStartPoint;
    private readonly ObservableCollection<AppListItem> gameItems = new();
    private readonly ObservableCollection<AppListItem> chatItems = new();
    private readonly ObservableCollection<AppListItem> availableItems = new();
    private readonly ObservableCollection<string> debugMessages = new();
    private bool softwareHotkeysRegistered;
    private bool softwareHotkeysDesired;
    private IntPtr windowHandle;
    private HwndSource? hwndSource;
    private bool suppressPlaceholderDrop;

    public MainWindow(
        Func<(bool Connected, string CurrentPort)> hardwareStatusProvider,
        Func<string> noiseReductionProvider,
        Action<string> noiseReductionSetter,
        Func<bool> invertControlProvider,
        Action<bool> invertControlSetter)
        : this(
            hardwareStatusProvider,
            noiseReductionProvider,
            noiseReductionSetter,
            invertControlProvider,
            invertControlSetter,
            () => string.Empty,
            _ => { },
            () => string.Empty,
            _ => { },
            () => Enumerable.Empty<string>(),
            _ => null,
            () => { },
            () => Enumerable.Empty<string>(),
            _ => { },
            _ => { },
            _ => { },
            () => false,
            _ => { },
            () => true,
            _ => { },
            () => 3,
            _ => { },
            () => "BottomCenter",
            _ => { },
            () => 0.9,
            _ => { },
            false)
    {
    }

    public MainWindow(
        Func<(bool Connected, string CurrentPort)> hardwareStatusProvider,
        Func<string> noiseReductionProvider,
        Action<string> noiseReductionSetter,
        Func<bool> invertControlProvider,
        Action<bool> invertControlSetter,
        Func<string> gameAppsProvider,
        Action<string> gameAppsSetter,
        Func<string> chatAppsProvider,
        Action<string> chatAppsSetter,
        Func<IEnumerable<string>> availableAppsProvider,
        Func<string, string?> assignedAppPathProvider,
        Action rescanAudioSessionsAction,
        Func<IEnumerable<string>> debugMessagesProvider,
        Action<string> debugLogAction,
        Action<bool> debugModeSetter,
        Action<float> softwareBalanceSetter,
        Func<bool> startWithWindowsProvider,
        Action<bool> startWithWindowsSetter,
        Func<bool> overlayEnabledProvider,
        Action<bool> overlayEnabledSetter,
        Func<int> overlayDurationProvider,
        Action<int> overlayDurationSetter,
        Func<string> overlayPositionProvider,
        Action<string> overlayPositionSetter,
        Func<double> overlayOpacityProvider,
        Action<double> overlayOpacitySetter,
        bool showDebugOption)
    {
        this.hardwareStatusProvider = hardwareStatusProvider;
        this.noiseReductionProvider = noiseReductionProvider;
        this.noiseReductionSetter = noiseReductionSetter;
        this.invertControlProvider = invertControlProvider;
        this.invertControlSetter = invertControlSetter;
        this.gameAppsProvider = gameAppsProvider;
        this.gameAppsSetter = gameAppsSetter;
        this.chatAppsProvider = chatAppsProvider;
        this.chatAppsSetter = chatAppsSetter;
        this.availableAppsProvider = availableAppsProvider;
        this.assignedAppPathProvider = assignedAppPathProvider;
        this.rescanAudioSessionsAction = rescanAudioSessionsAction;
        this.debugMessagesProvider = debugMessagesProvider;
        this.debugLogAction = debugLogAction;
        this.debugModeSetter = debugModeSetter;
        this.softwareBalanceSetter = softwareBalanceSetter;
        this.startWithWindowsProvider = startWithWindowsProvider;
        this.startWithWindowsSetter = startWithWindowsSetter;
        this.overlayEnabledProvider = overlayEnabledProvider;
        this.overlayEnabledSetter = overlayEnabledSetter;
        this.overlayDurationProvider = overlayDurationProvider;
        this.overlayDurationSetter = overlayDurationSetter;
        this.overlayPositionProvider = overlayPositionProvider;
        this.overlayPositionSetter = overlayPositionSetter;
        this.overlayOpacityProvider = overlayOpacityProvider;
        this.overlayOpacitySetter = overlayOpacitySetter;
        this.showDebugOption = showDebugOption;
        appIconService = new ProcessAppIconService();

        InitializeComponent();

        IsVisibleChanged += (_, _) => UpdateHardwareStatus();
        Loaded += (_, _) => UpdateIndicatorPosition(currentArduinoValue);
        Loaded += (_, _) => UpdateNavigationSections();
        Loaded += (_, _) => UpdateHardwareStatus();
        Loaded += (_, _) => InitializeHardwareOptions();
        Loaded += (_, _) => InitializeAppAssignments();
        Loaded += (_, _) => InitializeStartWithWindowsOption();
        Loaded += (_, _) => InitializeOverlayOptions();
        Loaded += (_, _) => RefreshDebugMessages();
        SourceInitialized += MainWindow_SourceInitialized;
        Closed += MainWindow_Closed;
        debugMessagesList.ItemsSource = debugMessages;

        noiseReductionAckTimeoutTimer.Tick += NoiseReductionAckTimeoutTimer_Tick;
        UpdateAudioBalanceUi(currentArduinoValue);
    }

    private void InitializeStartWithWindowsOption()
    {
        suppressHardwareOptionEvents = true;
        try
        {
            tglStartWithWindows.IsChecked = startWithWindowsProvider();
            if (tglSettingsStartWithWindows is not null)
            {
                tglSettingsStartWithWindows.IsChecked = tglStartWithWindows.IsChecked;
            }
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }
    }

    private void InitializeOverlayOptions()
    {
        suppressHardwareOptionEvents = true;
        try
        {
            tglOverlayEnabled.IsChecked = overlayEnabledProvider();

            var configuredDuration = overlayDurationProvider().ToString();
            var durationItem = cmbOverlayDuration.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => string.Equals(item.Tag?.ToString(), configuredDuration, StringComparison.OrdinalIgnoreCase));
            if (durationItem != null)
            {
                cmbOverlayDuration.SelectedItem = durationItem;
            }

            var configuredPosition = overlayPositionProvider();
            var positionItem = cmbOverlayPosition.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => string.Equals(item.Content?.ToString(), configuredPosition, StringComparison.OrdinalIgnoreCase));
            if (positionItem != null)
            {
                cmbOverlayPosition.SelectedItem = positionItem;
            }

            var opacityPercent = (int)Math.Round(Math.Clamp(overlayOpacityProvider(), 0.2, 1.0) * 100d);
            sldOverlayOpacity.Value = opacityPercent;
            txtOverlayOpacityValue.Text = $"{opacityPercent}%";
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }
    }

    private void MainWindow_SourceInitialized(object? sender, EventArgs e)
    {
        windowHandle = new WindowInteropHelper(this).Handle;
        hwndSource = HwndSource.FromHwnd(windowHandle);
        hwndSource?.AddHook(WndProc);
        UpdateSoftwareControlHotkeys(hardwareStatusProvider().Connected);
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        UnregisterSoftwareControlHotkeys();
        if (hwndSource is not null)
        {
            hwndSource.RemoveHook(WndProc);
            hwndSource = null;
        }

        windowHandle = IntPtr.Zero;
    }

    private void UpdateSoftwareControlHotkeys(bool connected)
    {
        softwareHotkeysDesired = !connected;
        ApplySoftwareControlHotkeyRegistration();
    }

    private void ApplySoftwareControlHotkeyRegistration()
    {
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        if (softwareHotkeysDesired)
        {
            RegisterSoftwareControlHotkeys();
            return;
        }

        UnregisterSoftwareControlHotkeys();
    }

    private void RegisterSoftwareControlHotkeys()
    {
        if (softwareHotkeysRegistered)
        {
            return;
        }

        var modifiers = ModControl | ModShift;
        var decreaseRegistered = RegisterHotKey(windowHandle, HotkeyIdDecrease, modifiers, VkLeft);
        var increaseRegistered = RegisterHotKey(windowHandle, HotkeyIdIncrease, modifiers, VkRight);
        var centerRegistered = RegisterHotKey(windowHandle, HotkeyIdCenter, modifiers, VkDown);

        if (decreaseRegistered && increaseRegistered && centerRegistered)
        {
            softwareHotkeysRegistered = true;
            debugLogAction("SoftwareControl: Hotkeys registriert (Ctrl+Shift+Left/Right/Down).");
            return;
        }

        if (decreaseRegistered)
        {
            UnregisterHotKey(windowHandle, HotkeyIdDecrease);
        }

        if (increaseRegistered)
        {
            UnregisterHotKey(windowHandle, HotkeyIdIncrease);
        }

        if (centerRegistered)
        {
            UnregisterHotKey(windowHandle, HotkeyIdCenter);
        }

        var error = Marshal.GetLastWin32Error();
        debugLogAction($"SoftwareControl: Hotkey-Registrierung fehlgeschlagen (Win32={error}).");
    }

    private void UnregisterSoftwareControlHotkeys()
    {
        if (!softwareHotkeysRegistered || windowHandle == IntPtr.Zero)
        {
            softwareHotkeysRegistered = false;
            return;
        }

        UnregisterHotKey(windowHandle, HotkeyIdDecrease);
        UnregisterHotKey(windowHandle, HotkeyIdIncrease);
        UnregisterHotKey(windowHandle, HotkeyIdCenter);
        softwareHotkeysRegistered = false;
        debugLogAction("SoftwareControl: Hotkeys deregistriert.");
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WmHotkey)
        {
            var hotkeyId = wParam.ToInt32();
            if (hotkeyId == HotkeyIdDecrease)
            {
                debugLogAction("SoftwareControl: Hotkey Ctrl+Shift+Left erkannt.");
                ApplySoftwareControlStep(-5f);
                handled = true;
            }
            else if (hotkeyId == HotkeyIdIncrease)
            {
                debugLogAction("SoftwareControl: Hotkey Ctrl+Shift+Right erkannt.");
                ApplySoftwareControlStep(5f);
                handled = true;
            }
            else if (hotkeyId == HotkeyIdCenter)
            {
                debugLogAction("SoftwareControl: Hotkey Ctrl+Shift+Down erkannt.");
                ApplySoftwareControlCenter();
                handled = true;
            }
        }

        return IntPtr.Zero;
    }

    private void ApplySoftwareControlStep(float delta)
    {
        var current = Math.Clamp(currentArduinoValue, 0f, 100f);
        var isOnFivePercentGrid = Math.Abs(current % 5f) < 0.01f || Math.Abs((current % 5f) - 5f) < 0.01f;

        float nextValue;
        if (delta < 0f)
        {
            nextValue = isOnFivePercentGrid
                ? current - 5f
                : MathF.Floor(current / 5f) * 5f;
        }
        else
        {
            nextValue = isOnFivePercentGrid
                ? current + 5f
                : MathF.Ceiling(current / 5f) * 5f;
        }

        nextValue = Math.Clamp(nextValue, 0f, 100f);
        if (Math.Abs(nextValue - currentArduinoValue) < 0.01f)
        {
            return;
        }

        UpdateAudioBalanceUi(nextValue);
        softwareBalanceSetter(nextValue);
    }

    private void ApplySoftwareControlCenter()
    {
        const float centerValue = 50f;
        if (Math.Abs(currentArduinoValue - centerValue) < 0.01f)
        {
            return;
        }

        UpdateAudioBalanceUi(centerValue);
        softwareBalanceSetter(centerValue);
    }

    private void InitializeHardwareOptions()
    {
        suppressHardwareOptionEvents = true;
        try
        {
            cmbNoiseReduction.ItemsSource = new[] { "Off", "Low", "Medium", "High" };
            if (cmbSettingsNoiseReduction is not null)
            {
                cmbSettingsNoiseReduction.ItemsSource = new[] { "Off", "Low", "Medium", "High" };
            }

            var configuredNoiseReduction = noiseReductionProvider();
            cmbNoiseReduction.SelectedItem = cmbNoiseReduction.Items.Cast<string>()
                .FirstOrDefault(item => string.Equals(item, configuredNoiseReduction, StringComparison.OrdinalIgnoreCase))
                ?? "High";
            if (cmbSettingsNoiseReduction is not null)
            {
                cmbSettingsNoiseReduction.SelectedItem = cmbSettingsNoiseReduction.Items.Cast<string>()
                    .FirstOrDefault(item => string.Equals(item, configuredNoiseReduction, StringComparison.OrdinalIgnoreCase))
                    ?? "High";
            }

            tglInvertControl.IsChecked = invertControlProvider();
            if (tglSettingsInvertControl is not null)
            {
                tglSettingsInvertControl.IsChecked = tglInvertControl.IsChecked;
            }
            SetNoiseReductionConfirmed(cmbNoiseReduction.SelectedItem?.ToString() ?? "High");
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }
    }

    private void InitializeAppAssignments()
    {
        gameItems.Clear();
        chatItems.Clear();
        availableItems.Clear();

        var gameApps = ParseCsv(gameAppsProvider()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var chatApps = ParseCsv(chatAppsProvider()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        var allApps = gameApps
            .Concat(chatApps)
            .Concat((availableAppsProvider() ?? Enumerable.Empty<string>())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item.Trim())
                .Where(item => !item.Equals("Idle", StringComparison.OrdinalIgnoreCase)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var name in gameApps)
        {
            gameItems.Add(new AppListItem(name, appIconService.GetIcon(name, assignedAppPathProvider(name))));
        }

        foreach (var name in chatApps)
        {
            chatItems.Add(new AppListItem(name, appIconService.GetIcon(name, assignedAppPathProvider(name))));
        }

        var assigned = new HashSet<string>(gameApps.Concat(chatApps), StringComparer.OrdinalIgnoreCase);
        foreach (var app in allApps
                     .Where(app => !assigned.Contains(app))
                     .OrderBy(app => app, StringComparer.OrdinalIgnoreCase))
        {
            availableItems.Add(new AppListItem(app, appIconService.GetIcon(app, null)));
        }

        gameAppsList.ItemsSource = gameItems;
        chatAppsList.ItemsSource = chatItems;
        availableAppsList.ItemsSource = availableItems;

        EnsureDropPlaceholder(gameAppsList);
        EnsureDropPlaceholder(chatAppsList);
        EnsureDropPlaceholder(availableAppsList);
    }

    private static IEnumerable<string> ParseCsv(string? csv)
    {
        return (csv ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim())
            .Where(item => item.Length > 0);
    }

    private sealed record AppListItem(string Name, ImageSource? Icon);

    private sealed record DropPlaceholderItem(string Text);

    private void AssignmentList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        dragStartPoint = e.GetPosition(null);
    }

    private void AssignmentList_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed || dragStartPoint is null)
        {
            return;
        }

        var currentPosition = e.GetPosition(null);
        if (Math.Abs(currentPosition.X - dragStartPoint.Value.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(currentPosition.Y - dragStartPoint.Value.Y) < SystemParameters.MinimumVerticalDragDistance)
        {
            return;
        }

        if (sender is not ListBox sourceList || sourceList.SelectedItem is not AppListItem selectedItem)
        {
            return;
        }

        DragDrop.DoDragDrop(sourceList, selectedItem, DragDropEffects.Move);
    }

    private void AssignmentList_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(typeof(AppListItem)) ? DragDropEffects.Move : DragDropEffects.None;
        e.Handled = true;
    }

    private void AssignmentList_Drop(object sender, DragEventArgs e)
    {
        if (sender is not ListBox targetList || !e.Data.GetDataPresent(typeof(AppListItem)))
        {
            return;
        }

        suppressPlaceholderDrop = true;
        try
        {
            RemoveDropPlaceholder(targetList);

            var droppedItem = (AppListItem)e.Data.GetData(typeof(AppListItem))!;
            var sourceList = FindSourceListForItem(droppedItem);
            if (sourceList is null || sourceList == targetList)
            {
                return;
            }

            RemoveItemFromList(sourceList, droppedItem.Name);
            EnsureDropPlaceholder(sourceList);

            var targetCollection = GetCollectionForList(targetList);
            if (targetCollection.Any(item => string.Equals(item.Name, droppedItem.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            targetCollection.Add(droppedItem);
            PersistAssignments();
        }
        finally
        {
            suppressPlaceholderDrop = false;
            EnsureDropPlaceholder(gameAppsList);
            EnsureDropPlaceholder(chatAppsList);
            EnsureDropPlaceholder(availableAppsList);
        }
    }

    private ListBox? FindSourceListForItem(AppListItem item)
    {
        if (gameItems.Any(x => string.Equals(x.Name, item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return gameAppsList;
        }

        if (chatItems.Any(x => string.Equals(x.Name, item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return chatAppsList;
        }

        if (availableItems.Any(x => string.Equals(x.Name, item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return availableAppsList;
        }

        return null;
    }

    private void RemoveItemFromList(ListBox list, string appName)
    {
        var collection = GetCollectionForList(list);
        var existing = collection.FirstOrDefault(x => string.Equals(x.Name, appName, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            collection.Remove(existing);
        }
    }

    private ObservableCollection<AppListItem> GetCollectionForList(ListBox list)
    {
        if (list == gameAppsList)
        {
            return gameItems;
        }

        if (list == chatAppsList)
        {
            return chatItems;
        }

        return availableItems;
    }

    private void EnsureDropPlaceholder(ListBox list)
    {
        // ItemsSource dauerhaft beibehalten. Kein Platzhalter-Item in Items,
        // damit kein Konflikt zwischen DisplayMemberPath und ItemTemplate entsteht.
        _ = list;
    }

    private void RemoveDropPlaceholder(ListBox list)
    {
        // Kein dynamischer Platzhalter mehr vorhanden.
        _ = list;
    }

    private void PersistAssignments()
    {
        var gameCsv = string.Join(",", gameItems.Select(x => x.Name));
        var chatCsv = string.Join(",", chatItems.Select(x => x.Name));

        gameAppsSetter(gameCsv);
        chatAppsSetter(chatCsv);
    }

    private static IEnumerable<T> FindDescendants<T>(DependencyObject root) where T : DependencyObject
    {
        if (root is null)
        {
            yield break;
        }

        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T typed)
            {
                yield return typed;
            }

            foreach (var descendant in FindDescendants<T>(child))
            {
                yield return descendant;
            }
        }
    }

    private void UpdateHardwareStatus()
    {
        var hardwareStatus = hardwareStatusProvider();
        var connected = hardwareStatus.Connected;
        txtArduinoStatus.Text = connected ? "Arduino: Connected" : "Arduino: Disconnected";
        hardwareStatusDot.Fill = connected
            ? new SolidColorBrush(Color.FromRgb(34, 197, 94))
            : new SolidColorBrush(Color.FromRgb(245, 158, 11));
        hardwareStatusDot.ToolTip = connected ? "Connected" : "Disconnected";
        UpdateSoftwareControlHotkeys(connected);

        if (hardwareControlsSection is not null)
        {
            hardwareControlsSection.Visibility = connected ? Visibility.Visible : Visibility.Collapsed;
        }

        if (softwareControlSection is not null)
        {
            softwareControlSection.Visibility = connected ? Visibility.Collapsed : Visibility.Visible;
        }

        if (txtSettingsHardwareStatus is not null)
        {
            txtSettingsHardwareStatus.Text = connected ? "Arduino: Connected" : "Arduino: Disconnected";
        }

        if (settingsHardwareStatusDot is not null)
        {
            settingsHardwareStatusDot.Fill = connected
                ? new SolidColorBrush(Color.FromRgb(34, 197, 94))
                : new SolidColorBrush(Color.FromRgb(245, 158, 11));
        }

        if (txtSettingsControlMode is not null)
        {
            txtSettingsControlMode.Text = connected ? "Hardware Control active" : "Software Control active";
        }

        if (txtSettingsHotkeysState is not null)
        {
            txtSettingsHotkeysState.Text = connected
                ? "Status: disabled while hardware is connected"
                : "Status: enabled (Ctrl+Shift+Left/Right)";
        }
    }

    private void NavigationRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        UpdateNavigationSections();
    }

    private void UpdateNavigationSections()
    {
        var homeSelected = navHome?.IsChecked == true;
        var settingsSelected = navSettings?.IsChecked == true;
        var aboutSelected = navAbout?.IsChecked == true;
        var debugSelected = showDebugOption && navDebug?.IsChecked == true;

        if (navDebug is not null)
        {
            navDebug.Visibility = showDebugOption ? Visibility.Visible : Visibility.Collapsed;
            if (!showDebugOption)
            {
                navDebug.IsChecked = false;
            }
        }

        if (homeHeaderSection is not null)
        {
            homeHeaderSection.Visibility = homeSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        if (homeAssignmentsSection is not null)
        {
            homeAssignmentsSection.Visibility = homeSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        if (settingsSection is not null)
        {
            settingsSection.Visibility = settingsSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        if (aboutSection is not null)
        {
            aboutSection.Visibility = aboutSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        if (debugSection is not null)
        {
            debugSection.Visibility = debugSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        if (debugSelected)
        {
            RefreshDebugMessages();
        }

        debugModeSetter(debugSelected);
    }

    public void SetShowDebugOption(bool show)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => SetShowDebugOption(show));
            return;
        }

        if (showDebugOption == show)
        {
            return;
        }

        showDebugOption = show;
        UpdateNavigationSections();
    }

    public void SetHardwareStatus()
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(SetHardwareStatus);
            return;
        }

        UpdateHardwareStatus();
    }

    private void NoiseReductionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (suppressHardwareOptionEvents)
        {
            return;
        }

        var sourceCombo = sender as ComboBox;
        if (sourceCombo?.SelectedItem is not string selectedNoiseReduction)
        {
            return;
        }

        suppressHardwareOptionEvents = true;
        try
        {
            if (sourceCombo != cmbNoiseReduction && cmbNoiseReduction is not null)
            {
                cmbNoiseReduction.SelectedItem = selectedNoiseReduction;
            }

            if (sourceCombo != cmbSettingsNoiseReduction && cmbSettingsNoiseReduction is not null)
            {
                cmbSettingsNoiseReduction.SelectedItem = selectedNoiseReduction;
            }
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }

        SetNoiseReductionApplying(selectedNoiseReduction);
        noiseReductionSetter(selectedNoiseReduction);
    }

    private void InvertControlToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (suppressHardwareOptionEvents)
        {
            return;
        }

        var sourceToggle = sender as ToggleButton;
        var checkedState = sourceToggle?.IsChecked == true;

        suppressHardwareOptionEvents = true;
        try
        {
            if (sourceToggle != tglInvertControl && tglInvertControl is not null)
            {
                tglInvertControl.IsChecked = checkedState;
            }

            if (sourceToggle != tglSettingsInvertControl && tglSettingsInvertControl is not null)
            {
                tglSettingsInvertControl.IsChecked = checkedState;
            }
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }

        invertControlSetter(checkedState);
    }

    private void StartWithWindowsToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (suppressHardwareOptionEvents)
        {
            return;
        }

        var sourceToggle = sender as ToggleButton;
        var checkedState = sourceToggle?.IsChecked == true;

        suppressHardwareOptionEvents = true;
        try
        {
            if (sourceToggle != tglStartWithWindows && tglStartWithWindows is not null)
            {
                tglStartWithWindows.IsChecked = checkedState;
            }

            if (sourceToggle != tglSettingsStartWithWindows && tglSettingsStartWithWindows is not null)
            {
                tglSettingsStartWithWindows.IsChecked = checkedState;
            }
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }

        startWithWindowsSetter(checkedState);
    }

    private void OverlayEnabledToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (suppressHardwareOptionEvents)
        {
            return;
        }

        overlayEnabledSetter(tglOverlayEnabled.IsChecked == true);
    }

    private void OverlayDurationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (suppressHardwareOptionEvents)
        {
            return;
        }

        if (cmbOverlayDuration.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content?.ToString(), out var durationSeconds))
        {
            overlayDurationSetter(durationSeconds);
        }
    }

    private void OverlayPositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (suppressHardwareOptionEvents)
        {
            return;
        }

        if (cmbOverlayPosition.SelectedItem is ComboBoxItem selectedItem)
        {
            var position = selectedItem.Content?.ToString();
            if (!string.IsNullOrWhiteSpace(position))
            {
                overlayPositionSetter(position);
            }
        }
    }

    private void OverlayOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var opacityPercent = (int)Math.Round(e.NewValue);

        if (txtOverlayOpacityValue is null)
        {
            return;
        }

        txtOverlayOpacityValue.Text = $"{opacityPercent}%";

        if (suppressHardwareOptionEvents)
        {
            return;
        }

        overlayOpacitySetter?.Invoke(Math.Clamp(opacityPercent / 100d, 0.2, 1.0));
    }

    public void SetStartWithWindowsState(bool enabled)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => SetStartWithWindowsState(enabled));
            return;
        }

        suppressHardwareOptionEvents = true;
        try
        {
            tglStartWithWindows.IsChecked = enabled;
            if (tglSettingsStartWithWindows is not null)
            {
                tglSettingsStartWithWindows.IsChecked = enabled;
            }
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }
    }

    public void SetInvertControlState(bool enabled)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => SetInvertControlState(enabled));
            return;
        }

        suppressHardwareOptionEvents = true;
        try
        {
            tglInvertControl.IsChecked = enabled;
            if (tglSettingsInvertControl is not null)
            {
                tglSettingsInvertControl.IsChecked = enabled;
            }
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }
    }

    public void SetNoiseReductionSelection(string level)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => SetNoiseReductionSelection(level));
            return;
        }

        suppressHardwareOptionEvents = true;
        try
        {
            var normalized = string.IsNullOrWhiteSpace(level) ? "High" : level.Trim();
            var selected = cmbNoiseReduction.Items
                .Cast<string>()
                .FirstOrDefault(item => string.Equals(item, normalized, StringComparison.OrdinalIgnoreCase))
                ?? "High";

            cmbNoiseReduction.SelectedItem = selected;
            if (cmbSettingsNoiseReduction is not null)
            {
                cmbSettingsNoiseReduction.SelectedItem = selected;
            }
        }
        finally
        {
            suppressHardwareOptionEvents = false;
        }
    }

    public void SetNoiseReductionApplying(string level)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => SetNoiseReductionApplying(level));
            return;
        }

        pendingNoiseReductionLevel = level;
        noiseReductionAckTimeoutTimer.Stop();
        noiseReductionAckTimeoutTimer.Start();

        noiseReductionStateDot.Fill = new SolidColorBrush(Color.FromRgb(59, 130, 246));
        txtNoiseReductionStateTitle.Text = "Applying...";
        txtNoiseReductionStateDetail.Text = $"Sending {level} to Arduino";
    }

    public void SetNoiseReductionConfirmed(string level)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => SetNoiseReductionConfirmed(level));
            return;
        }

        pendingNoiseReductionLevel = null;
        noiseReductionAckTimeoutTimer.Stop();
        noiseReductionStateDot.Fill = new SolidColorBrush(Color.FromRgb(34, 197, 94));
        txtNoiseReductionStateTitle.Text = $"{level} active";
        txtNoiseReductionStateDetail.Text = "Confirmed by Arduino";
    }

    public void SetNoiseReductionError(string level)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(() => SetNoiseReductionError(level));
            return;
        }

        pendingNoiseReductionLevel = null;
        noiseReductionAckTimeoutTimer.Stop();
        noiseReductionStateDot.Fill = new SolidColorBrush(Color.FromRgb(244, 63, 94));
        txtNoiseReductionStateTitle.Text = "Couldn't apply setting";
        txtNoiseReductionStateDetail.Text = $"No confirmation for {level}";
    }

    private void NoiseReductionAckTimeoutTimer_Tick(object? sender, EventArgs e)
    {
        noiseReductionAckTimeoutTimer.Stop();
        if (!string.IsNullOrWhiteSpace(pendingNoiseReductionLevel))
        {
            SetNoiseReductionError(pendingNoiseReductionLevel);
        }
    }

    private void BalanceTrackHost_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateIndicatorPosition(currentArduinoValue);
    }

    private void UpdateAudioBalanceUi(float arduinoValue)
    {
        currentArduinoValue = Math.Clamp(arduinoValue, 0f, 100f);
        var balance = CalculateBalance(currentArduinoValue);

        txtArduinoValue.Text = MathF.Round(balance.DisplayVolume).ToString("0");
        txtGamePercent.Text = $"{MathF.Round(balance.GameVolume):0}%";
        txtChatPercent.Text = $"{MathF.Round(balance.ChatVolume):0}%";

        UpdateIndicatorPosition(balance.DisplayVolume);
    }

    private static BalanceResult CalculateBalance(float inputVolume)
    {
        var volume = Math.Clamp(inputVolume, 0f, 100f);

        float gameVolume;
        float chatVolume;

        if (volume < 50)
        {
            gameVolume = volume * 2;
            chatVolume = 100;
        }
        else if (volume > 50)
        {
            gameVolume = 100;
            chatVolume = 100 - ((volume - 50) * 2);
        }
        else
        {
            gameVolume = 100;
            chatVolume = 100;
        }

        return new BalanceResult(volume, gameVolume, chatVolume);
    }

    private void UpdateIndicatorPosition(float arduinoValue)
    {
        var clampedValue = Math.Clamp(arduinoValue, 0f, 100f);
        var trackWidth = Math.Max(0d, balanceTrackHost.ActualWidth);
        if (trackWidth <= 0)
        {
            return;
        }

        var normalized = clampedValue / 100d;
        var indicatorWidth = arduinoIndicator.Width;
        var x = (trackWidth * normalized) - (indicatorWidth / 2d);
        x = Math.Clamp(x, 0d, Math.Max(0d, trackWidth - indicatorWidth));
        Canvas.SetLeft(arduinoIndicator, x);
    }

    private readonly record struct BalanceResult(float DisplayVolume, float GameVolume, float ChatVolume);

    public void SetArduinoValue(float arduinoValue)
    {
        UpdateAudioBalanceUi(arduinoValue);
    }

    public void RefreshAppAssignments()
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(RefreshAppAssignments);
            return;
        }

        InitializeAppAssignments();
    }

    private void RescanAudioSessionsButton_Click(object sender, RoutedEventArgs e)
    {
        rescanAudioSessionsAction();
        RefreshAppAssignments();
    }

    public void RefreshDebugMessages()
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.BeginInvoke(RefreshDebugMessages);
            return;
        }

        var latestMessages = debugMessagesProvider().ToList();
        if (debugMessages.SequenceEqual(latestMessages))
        {
            return;
        }

        debugMessages.Clear();
        foreach (var message in latestMessages)
        {
            debugMessages.Add(message);
        }
    }
}
// Bitte die Overlay-Duration-Logik konsistent auf Sekunden umstellen:
// 1) In OverlayDurationSlider_ValueChanged (oder äquivalenter Methode):
//    - Wert als int durationSeconds lesen
//    - Textanzeige als "{durationSeconds} s"
//    - overlayDurationSetter(durationSeconds)
// 2) Beim Laden/Initialisieren der Overlay-Dauer:
//    - Providerwert als Sekunden behandeln
//    - Clamp auf sinnvollen Sekundenbereich (1..10)
// 3) Alle verbleibenden "ms"-Texte/Variablennamen in diesem Kontext auf Sekunden semantisch anpassen.