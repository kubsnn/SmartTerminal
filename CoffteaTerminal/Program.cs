using System;
using System.IO;

namespace Cofftea
{
    internal class Program
    {
        public static string AppShortName { get; } = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]) + ".exe";
        public static string AppDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string CurrentDirectory => Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            Settings.Load();
            var solver = new CtfShell();
            solver.Start();
        }
    }
}
