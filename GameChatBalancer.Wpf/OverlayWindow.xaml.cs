using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace GameChatBalancer.Wpf;

public partial class OverlayWindow : Window
{
    private const int GwlExStyle = -20;
    private const int WsExTransparent = 0x20;
    private const int WsExToolwindow = 0x80;
    private const int WsExNoactivate = 0x08000000;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private string overlayPosition = "BottomCenter";
    private float pendingGameLevel = 100f;
    private float pendingChatLevel = 100f;

    public OverlayWindow()
    {
        InitializeComponent();
        SourceInitialized += OverlayWindow_SourceInitialized;
        Loaded += (_, _) => PositionOnPrimaryScreen();
        Loaded += (_, _) => ApplyBarWidths();
        SizeChanged += (_, _) => ApplyBarWidths();
        IsHitTestVisible = false;
        ShowInTaskbar = false;
    }

    public void SetPosition(string position)
    {
        overlayPosition = string.IsNullOrWhiteSpace(position) ? "BottomCenter" : position;
        PositionOnPrimaryScreen();
    }

    public void SetOverlayOpacity(double opacity)
    {
        Opacity = Math.Clamp(opacity, 0.2, 1.0);
    }

    public void SetLevels(float gameLevel, float chatLevel)
    {
        var game = Math.Clamp(MathF.Round(gameLevel), 0f, 100f);
        var chat = Math.Clamp(MathF.Round(chatLevel), 0f, 100f);

        pendingGameLevel = game;
        pendingChatLevel = chat;

        GameValueText.Text = $"🎮 {game:0}%";
        ChatValueText.Text = $"💬 {chat:0}%";

        ApplyBarWidths();
    }

    public void ShowOverlay()
    {
        PositionOnPrimaryScreen();
        if (!IsVisible)
        {
            Show();
        }

        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(ApplyBarWidths));
    }

    public void HideOverlay()
    {
        Hide();
    }

    private void OverlayWindow_SourceInitialized(object? sender, EventArgs e)
    {
        var helper = new WindowInteropHelper(this);
        var extendedStyle = GetWindowLong(helper.Handle, GwlExStyle);
        extendedStyle |= WsExTransparent;
        extendedStyle |= WsExToolwindow;
        extendedStyle |= WsExNoactivate;
        SetWindowLong(helper.Handle, GwlExStyle, extendedStyle);
    }

    private void PositionOnPrimaryScreen()
    {
        var workingArea = SystemParameters.WorkArea;
        const double margin = 24d;

        var left = workingArea.Left + margin;
        var top = workingArea.Top + margin;

        switch (overlayPosition)
        {
            case "TopLeft":
                left = workingArea.Left + margin;
                top = workingArea.Top + margin;
                break;
            case "TopCenter":
                left = workingArea.Left + (workingArea.Width - Width) / 2d;
                top = workingArea.Top + margin;
                break;
            case "TopRight":
                left = workingArea.Right - Width - margin;
                top = workingArea.Top + margin;
                break;
            case "CenterLeft":
                left = workingArea.Left + margin;
                top = workingArea.Top + (workingArea.Height - Height) / 2d;
                break;
            case "Center":
                left = workingArea.Left + (workingArea.Width - Width) / 2d;
                top = workingArea.Top + (workingArea.Height - Height) / 2d;
                break;
            case "CenterRight":
                left = workingArea.Right - Width - margin;
                top = workingArea.Top + (workingArea.Height - Height) / 2d;
                break;
            case "BottomLeft":
                left = workingArea.Left + margin;
                top = workingArea.Bottom - Height - margin;
                break;
            case "BottomCenter":
                left = workingArea.Left + (workingArea.Width - Width) / 2d;
                top = workingArea.Bottom - Height - margin;
                break;
            case "BottomRight":
                left = workingArea.Right - Width - margin;
                top = workingArea.Bottom - Height - margin;
                break;
        }

        Left = left;
        Top = top;
    }

    private void ApplyBarWidths()
    {
        var gameTrackWidth = Math.Max(0d, GameTrack.ActualWidth);
        var chatTrackWidth = Math.Max(0d, ChatTrack.ActualWidth);

        GameBar.Width = gameTrackWidth * (pendingGameLevel / 100d);
        ChatBar.Width = chatTrackWidth * (pendingChatLevel / 100d);
    }
}
