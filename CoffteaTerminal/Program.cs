using Cofftea.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cofftea
{
    internal class Program
    {
        public delegate bool CommandHandler(Command cmd);
        public static List<CommandHandler> Handlers;
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
