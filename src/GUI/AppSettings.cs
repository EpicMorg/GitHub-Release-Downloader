using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHub_Release_Downloader
{
    /// <summary>
    ///  Form state kept between runs in %APPDATA%. The token is stored as a DPAPI blob
    ///  bound to the current Windows user, everything else as plain JSON.
    /// </summary>
    internal sealed class AppSettings
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public string Url { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public bool AddSubFolders { get; set; } = true;

        public bool LatestOnly { get; set; }

        public bool PreReleases { get; set; }

        public bool Sources { get; set; }

        public bool Overwrite { get; set; }

        /// <summary>Base64 of the DPAPI-protected token; null when no token was saved.</summary>
        public string? ProtectedToken { get; set; }

        [JsonIgnore]
        public string Token
        {
            get => Unprotect(ProtectedToken);
            set => ProtectedToken = Protect(value);
        }

        public static string FilePath => System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "EpicMorg",
            "GitHub Release Downloader",
            "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new AppSettings();
                }

                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch (Exception)
            {
                // A corrupt or unreadable file must not keep the app from starting.
                return new AppSettings();
            }
        }

        public bool TrySave(out string? error)
        {
            try
            {
                var directory = System.IO.Path.GetDirectoryName(FilePath)!;
                Directory.CreateDirectory(directory);
                File.WriteAllText(FilePath, JsonSerializer.Serialize(this, SerializerOptions));

                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static string? Protect(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var blob = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(value), null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(blob);
        }

        private static string Unprotect(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            try
            {
                var blob = ProtectedData.Unprotect(
                    Convert.FromBase64String(value), null, DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(blob);
            }
            catch (Exception)
            {
                // Different Windows user, different machine or a reinstalled profile:
                // the blob can no longer be decrypted, so ask for the token again.
                return string.Empty;
            }
        }
    }
}
