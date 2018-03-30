using System;
using System.IO;

namespace DataVault.Storage.IO
{
    internal static class IOHelper
    {
        private static object _lock = new object();

        public static string GetAppPath()
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;

            if (appPath.ToLower().Contains("debug"))
                appPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(appPath).ToString()).ToString()).ToString();

            return appPath;
        }

        public static string CreateDirectoryIfNotExists()
        {
            string appPath = GetAppPath();

            string directoryPath = Path.Combine(appPath, "VaultData");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return directoryPath;
        }

        public static void CreateBackUp(string filePath, string type)
        {
            string backUpPath = $"{filePath}.backup";

            if (File.Exists(backUpPath)) return;

            File.Copy(filePath, backUpPath);
        }

        public static void RestoreFromBackUp(string[] files)
        {
            lock (_lock)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (File.Exists($"{files[i]}.backup"))
                    {
                        File.Replace($"{files[i]}.backup", files[i], files[i] + ".restored");
                    }
                }
            }
        }

        public static string CreateFileIfNotExists(string type, bool createBackUp)
        {
            try
            {
                if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");

                string directoryPath = CreateDirectoryIfNotExists();
                string filePath = $"{directoryPath}\\{type}.txt";

                if (!File.Exists(filePath))
                {
                    using (FileStream stream = File.Create(filePath, 4096, FileOptions.Asynchronous)){ }
                }

                if (createBackUp) CreateBackUp(filePath, type);

                return filePath;
            }
            catch(Exception e)
            {
                throw new IOException("Something went wrong on entity file creation", e);
            }
        }

        public static void DeleteBackUps(string[] files)
        {
            lock (_lock)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (File.Exists($"{files[i]}.backup")) File.Delete($"{files[i]}.backup");
                    if (File.Exists($"{files[i]}.restored")) File.Delete($"{files[i]}.restored");
                }
            }
        }
    }
}
