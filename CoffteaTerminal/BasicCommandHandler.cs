using Cofftea.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cofftea
{
    internal class BasicCommandHandler
    {
        static string[] commands = { "cd", "ls", "cls", "reload", "echo", "pwd", "exit" };
        public static bool CanHandle(string cmd)
        {
            return commands.Contains(cmd);
        }
        public static void Handle(Command cmd)
        {
            if (cmd.Base == "cd") ChangeDir(cmd.Args);
            else if (cmd.Base == "ls") ListFiles();
            else if (cmd.Base == "cls") Console.Clear();
            else if (cmd.Base == "pwd") PrintWorkingDirectory();
            else if (cmd.Base == "echo") Echo(cmd);
            else if (cmd.Base == "exit") Environment.Exit(0);
            else if (cmd.Base == "reload") Settings.Reload();
        }
        public static bool CheckCmd(Command cmd)
        {
            string command = cmd.Base.ToLower();
            return command == "cmd" || command == "powershell";
        }
        public static void RunCmd(Command cmd)
        {
            if (cmd.Base == "cmd") cmd.Base = Path.Combine(Environment.SystemDirectory, "cmd.exe");
            else cmd.Base = Path.Combine(Environment.SystemDirectory, @"WindowsPowerShell\v1.0\powershell.exe");
            
            
            var executor = new ScriptExecutor(cmd);
            executor.Execute();
        }
        static void Echo(Command cmd)
        {
            CoffeeString.Write("'");
            foreach (string arg in cmd.Args) {
                CoffeeString.Write(arg + " ");
            }
            CoffeeString.Write("'");
            CoffeeString.WriteLine();
        }
        static void ChangeDir(List<string> args)
        {
            if (args.Count != 1) {
                Messages.WrongArgNumber(args.Count, 1).Print();
                return;
            }

            string path = Path.GetFullPath(Path.Combine(Program.CurrentDirectory, args[0]));
            if (!Directory.Exists(path)) {
                CoffeeString.WriteLine("This directory does NOT exist!", ConsoleColor.Red);
                return;
            }
            Directory.SetCurrentDirectory(path);

            CoffeeString.HintsClear("cd");

            var dirs = from dir in Directory.GetDirectories(path)
                       select Path.GetRelativePath(Program.CurrentDirectory, dir);
                        
            foreach (string dir in dirs) {
                CoffeeString.AddHintElement("cd", dir.Contains(' ') ? $"\"{dir}\"" : dir);
            }

            var files = (from file in Directory.GetFiles(path)
                        where file.EndsWith(".exe")
                        let p = Path.GetRelativePath(Program.CurrentDirectory, file)
                        select "." + (p.Contains(' ') ? '"' + p + '"' : p)).ToList();

            files.Remove("." + Program.AppShortName);

            CoffeeString.TempCommands["cd"].Clear();
            CoffeeString.TempCommands["cd"] = files;
        }
        static void ListFiles()
        {
            var dirs = from dir in Directory.GetDirectories(Program.CurrentDirectory)
                       select Path.GetRelativePath(Program.CurrentDirectory, dir);
            var files = from file in Directory.GetFiles(Program.CurrentDirectory)
                        let ext = Path.GetExtension(file)
                        orderby ext
                        select Path.GetRelativePath(Program.CurrentDirectory, file);

            var cs = new CoffeeString();

            cs.Add(string.Join("   ", dirs), ConsoleColor.Blue);
            if (dirs.Count() > 0) cs.Add("   ");

            foreach (var file in files) {
                var color = GetFileColor(file);
                cs.Add(file + "   ", color);
            }
            cs.AddLine();
            cs.Print();
        }
        static void PrintWorkingDirectory()
        {
            CoffeeString.WriteLine(Program.CurrentDirectory, ConsoleColor.DarkCyan);
        }
        static ConsoleColor GetFileColor(string file)
        {
            if (file.EndsWith(".exe")) return ConsoleColor.Green;
            if (file.EndsWith(".py")) return ConsoleColor.DarkGreen;
            if (file.EndsWith(".lnk")) return ConsoleColor.DarkGray;
            if (file.EndsWith(".7z") || file.EndsWith(".zip") || file.EndsWith(".rar")) return ConsoleColor.DarkYellow;
            if (file.EndsWith(".jpeg") || file.EndsWith(".png") || file.EndsWith(".jpg")) return ConsoleColor.Yellow;
            if (file.EndsWith(".tiff") || file.EndsWith(".bmp") || file.EndsWith(".ico")) return ConsoleColor.Yellow;
            if (file.EndsWith(".json") || file.EndsWith(".xml")) return ConsoleColor.White;
            

            return ConsoleColor.Gray;
        }
    }
}