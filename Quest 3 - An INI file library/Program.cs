using System;

namespace MyIniFile
{
    class Program
    {
        static void Main(string[] args)
        {
            const string iniPath = "C:\\Users\\sina\\Desktop\\input";
            const string iniFalloutName = "Fallout.ini";
            const string iniFalloutPrefsName = "FalloutPrefs.ini";
            string iniFalloutPath = Path.Combine(iniPath, iniFalloutName);
            string iniFalloutprefsPath = Path.Combine(iniPath, iniFalloutPrefsName);
            string header = "[General]";
            string cfg = "fDialogueHeadYawExaggeration";
            string value = "asd";


            IniFile ini_Fallout = new IniFile(iniFalloutPath);
            ini_Fallout.Load();
            Console.ReadLine();
            ini_Fallout.Display();
            Console.ReadLine();
            ini_Fallout.DisplayHeader(header);
            Console.ReadLine();
            Console.WriteLine(ini_Fallout.GetValue(header, cfg));
            ini_Fallout.SetValue(value, header, cfg);
            ini_Fallout.Save();

        }
    }
}
