using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameChatBalancer.Wpf;

public interface IAppIconService
{
    ImageSource? GetIcon(string appName, string? persistedExePath);
}

public sealed class ProcessAppIconService : IAppIconService
{
    public ImageSource? GetIcon(string appName, string? persistedExePath)
    {
        if (string.IsNullOrWhiteSpace(appName))
        {
            return GetFallbackIcon();
        }

        if (!string.IsNullOrWhiteSpace(persistedExePath) && File.Exists(persistedExePath))
        {
            var persistedIcon = TryExtractIcon(persistedExePath);
            if (persistedIcon is not null)
            {
                return persistedIcon;
            }
        }

        var normalized = NormalizeName(appName);

        var exePath = TryFindExePath(normalized);
        if (!string.IsNullOrWhiteSpace(exePath))
        {
            var icon = TryExtractIcon(exePath);
            if (icon is not null)
            {
                return icon;
            }
        }

        return GetFallbackIcon();
    }

    private static string? TryFindExePath(string normalizedAppName)
    {
        // 1) exakter Prozessname
        try
        {
            var exact = Process.GetProcessesByName(normalizedAppName).FirstOrDefault();
            if (exact is not null)
            {
                using (exact)
                {
                    var path = TryGetMainModulePath(exact);
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        return path;
                    }
                }
            }
        }
        catch
        {
        }

        // 2) Fuzzy über laufende Prozesse
        try
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    var procName = NormalizeName(process.ProcessName);
                    if (!procName.Contains(normalizedAppName) && !normalizedAppName.Contains(procName))
                    {
                        continue;
                    }

                    var path = TryGetMainModulePath(process);
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        return path;
                    }
                }
                catch
                {
                }
                finally
                {
                    process.Dispose();
                }
            }
        }
        catch
        {
        }

        return null;
    }

    private static string? TryGetMainModulePath(Process process)
    {
        try
        {
            var path = process.MainModule?.FileName;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                return path;
            }
        }
        catch
        {
        }

        return null;
    }

    private static ImageSource? TryExtractIcon(string exePath)
    {
        try
        {
            using var icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
            if (icon is null)
            {
                return null;
            }

            var source = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(16, 16));
            source.Freeze();
            return source;
        }
        catch
        {
            return null;
        }
    }

    private static string NormalizeName(string appName)
    {
        var value = appName.Trim();
        if (value.EndsWith(".exe", System.StringComparison.OrdinalIgnoreCase))
        {
            value = value[..^4];
        }

        return value.ToLowerInvariant();
    }

    private static ImageSource GetFallbackIcon()
    {
        var group = new DrawingGroup();
        group.Children.Add(new GeometryDrawing(
            new SolidColorBrush(Color.FromRgb(35, 54, 82)),
            new Pen(new SolidColorBrush(Color.FromRgb(64, 93, 130)), 1),
            new RectangleGeometry(new Rect(0.5, 0.5, 15, 15), 3, 3)));
        group.Children.Add(new GeometryDrawing(
            new SolidColorBrush(Color.FromRgb(175, 192, 216)),
            null,
            new RectangleGeometry(new Rect(4, 4, 8, 8), 1.5, 1.5)));

        var image = new DrawingImage(group);
        image.Freeze();
        return image;
    }
}
