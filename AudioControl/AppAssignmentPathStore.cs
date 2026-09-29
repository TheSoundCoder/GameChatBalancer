using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AudioControl
{
    internal sealed class AppAssignmentPathStore
    {
        private readonly string filePath;
        private readonly Dictionary<string, string> paths;

        public AppAssignmentPathStore()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GameChatBalancer");
            Directory.CreateDirectory(dir);
            filePath = Path.Combine(dir, "app-paths.json");
            paths = Load();
        }

        public string? GetPath(string appName)
        {
            if (string.IsNullOrWhiteSpace(appName))
            {
                return null;
            }

            return paths.TryGetValue(Normalize(appName), out var path) ? path : null;
        }

        public void SyncAssignedApps(IEnumerable<string> gameApps, IEnumerable<string> chatApps)
        {
            var assigned = gameApps
                .Concat(chatApps)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(Normalize)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var obsolete = paths.Keys.Where(k => !assigned.Contains(k)).ToList();
            foreach (var key in obsolete)
            {
                paths.Remove(key);
            }

            foreach (var app in assigned)
            {
                if (paths.ContainsKey(app))
                {
                    continue;
                }

                var resolved = TryResolveExePath(app);
                if (!string.IsNullOrWhiteSpace(resolved))
                {
                    paths[app] = resolved;
                }
            }

            Save();
        }

        private Dictionary<string, string> Load()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }

                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                return data is null
                    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(data, StringComparer.OrdinalIgnoreCase);
            }
            catch
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(paths, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch
            {
            }
        }

        private static string Normalize(string name)
        {
            var value = name.Trim();
            if (value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                value = value[..^4];
            }

            return value;
        }

        private static string? TryResolveExePath(string normalizedName)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(normalizedName))
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
    }
}
