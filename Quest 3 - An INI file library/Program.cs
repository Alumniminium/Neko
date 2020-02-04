using System;

namespace MyIniFile
{
    class Program
    {
        static void Main(string[] args)
        {
            string iniPath = "C:\\Users\\sina\\Desktop\\input";
            string iniFalloutName = "Fallout.ini";
            string iniFalloutPrefsName = "FalloutPrefs.ini";
            string iniFalloutPath = Path.Combine(iniPath, iniFalloutName);
            string iniFalloutprefsPath = Path.Combine(iniPath, iniFalloutPrefsName);
            string header = "[General]";
            string cfg = "fDialogueHeadYawExaggeration";
            string value = "asd";


            IniFile ini = new IniFile(iniFalloutPath);
            ini.Load();
            ini.Display();
            ini.DisplayHeader(header);
            Console.WriteLine(ini.GetValue(header, cfg));
            ini.SetValue(value, header, cfg);
            ini.Save();
        }
    }
}
