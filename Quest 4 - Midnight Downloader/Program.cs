using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MMD
{
    class Program
    {
        public static string ConfigFileName = "Config.ini";
        public static IniFile Ini = new IniFile(ConfigFileName);
        public static string PlaylistFilePath = $@"C:\Users\{Environment.UserName}\Desktop\DownloadList.txt";
        public static string OutputPath = $@"C:\Users\{Environment.UserName}\Desktop\MMD Downloads\";
        public static int DownloadStartTimeHour = 2;
        public static int DownloadEndTimeHour = 8;
        public static Dictionary<string, bool> Downloads = new Dictionary<string, bool>();

        static void Main()
        {
            LoadConfig();
            CheckTime();
            LoadDownloadList();
            StartDownloading();
        }


        private static void CheckTime()
        {
            if (DateTime.Now.TimeOfDay.Hours > DownloadEndTimeHour)
            {
                Console.WriteLine($"Free Downloading starts at {DownloadStartTimeHour} and ends at {DownloadEndTimeHour}. It's currently {DateTime.Now.ToShortTimeString()}");
                Console.WriteLine("Continue downloading? (Y/n)");
                var response = Console.ReadLine().ToUpper();
                if (response == "N")
                    SaveAndExit();
                if (response != "Y")
                    CheckTime();
            }
        }


        private static void LoadDownloadList()
        {
            var downloadLinks = File.ReadAllLines(PlaylistFilePath);
            for (int i = 0; i < downloadLinks.Length; i++)
                Downloads.Add(downloadLinks[i], false);
        }

        private static void StartDownloading()
        {

            foreach (var kvp in Downloads)
            {
                var ytdlBinary = "youtube-dl"; // on linux there's no .exe
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    ytdlBinary += ".exe"; // but on windows there is

                var process = Process.Start(ytdlBinary, $@"--geo-bypass -f bestaudio --output ""{OutputPath}%(title)s.%(ext)s"" " + kvp.Key);
                process.WaitForExit();
                Downloads[kvp.Key] = true;

                if (DateTime.Now.Hour >= DownloadEndTimeHour)
                    CheckTime();
            }
        }

        private static void LoadConfig()
        {
            if (File.Exists(ConfigFileName))
            {
                Console.WriteLine("Loading Configuration file...");
                Ini.Load();

                PlaylistFilePath = Ini.GetValue("[MMD]", "PlaylistFilePath", PlaylistFilePath);
                Console.WriteLine("Using Download List from " + PlaylistFilePath);

                if (!File.Exists(PlaylistFilePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"But {PlaylistFilePath} doesn't exist, so I'm shutting down.");
                    Console.ResetColor();
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                OutputPath = Ini.GetValue("[MMD]", "OutputPath" , OutputPath);
                Directory.CreateDirectory(OutputPath); // if it exists it does nothing
                Console.WriteLine("Saving music at " + OutputPath);

                DownloadStartTimeHour = int.Parse(Ini.GetValue("[MMD]", "DownloadStartHour",DownloadStartTimeHour));
                Console.WriteLine("Start downloading at " + DownloadStartTimeHour + ":00");

                DownloadEndTimeHour = int.Parse(Ini.GetValue("[MMD]", "DownloadEndTimeHour",DownloadEndTimeHour));
                Console.WriteLine("Stop downloading at " + DownloadEndTimeHour + ":00");
            }
        }
        private static void SaveAndExit()
        {
            string filesLeft = "";
            foreach (var kvp in Downloads)
            {
                if (kvp.Value == false)
                    filesLeft += kvp.Key + Environment.NewLine;
            }
            File.WriteAllText(PlaylistFilePath, filesLeft);
            Ini.Save();
        }
    }
}
