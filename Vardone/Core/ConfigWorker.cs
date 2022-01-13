using System.Reflection;
using Microsoft.Win32;

namespace Vardone.Core
{
    public static class ConfigWorker
    {
        private static readonly string FilePath = MainWindow.PATH + @"\config.ini";
        private static readonly IniFile IniFile = new(FilePath);
        public static string GetToken()
        {
            var read = IniFile.Read("Token", "UserPreferences");
            return string.IsNullOrWhiteSpace(read) ? null : read;
        }
        public static void SetToken(string token) => IniFile.Write("Token", token, "UserPreferences");

        public static bool GetStartMinimized()
        {
            var read = IniFile.Read("StartMinimized", "Preferences");
            return !string.IsNullOrWhiteSpace(read) && bool.Parse(read);
        }

        public static void SetStartMinimized(bool value) => IniFile.Write("StartMinimized", value.ToString(), "Preferences");

        public static void SetAutostart(bool value)
        {
            using var subKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            if (subKey is null) return;
            try
            {
                var exePath = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");
                if (value) subKey.SetValue("Vardone", (object)$"\"{exePath}\" -autostart");
                else subKey.DeleteValue("Vardone");
            }
            catch
            {
                // ignored
            }
        }
    }
}