using Cofftea.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofftea
{
    internal class CtfShell
    {
        Input input;
        public CtfShell()
        {
            CoffeeString.Prefix = @"@:\Cofftea\> ";
            input = new Input(HandleCommandRedirected);
            //CoffeeString.InstantPrint = true;
        }
        void HandleCommandRedirected(Command cmd) => HandleCommand(cmd, true);
        public void Start()
        {

            MainLoop();
        }
        void MainLoop()
        {
            while (true) {
                try {
                    PrintPrefix();
                    string line = CoffeeString.ReadLine();
                    var cmd = input.GetCommand(line);
                    HandleCommand(cmd);
                } catch(Exception e) {
                    CoffeeString.WriteLine("\nAn error occured while reading input. Try again.", ConsoleColor.Red);
                    //Console.WriteLine(e.ToString());
                }
            }
        }
        void HandleCommand(Command cmd, bool redirect = false)
        {
            if (string.IsNullOrWhiteSpace(cmd.Base) || cmd.Base == "INSERT") return;

            if (CheckIfRedirectOutputToFile(cmd)) {
                SaveOutputToFile(cmd);
                return;
            }

            if (BasicCommandHandler.CanHandle(cmd.Base)) {
                BasicCommandHandler.Handle(cmd);
                return;
            }

            if (IsExecutable(cmd.Base)) {
                if (cmd.Base[0] == '.') cmd.Base = cmd.Base.Substring(1);
                var exec = new ScriptExecutor(cmd, redirect);
                exec.Execute();
                return;
            }

            foreach (var script in Program.Handlers) {
                bool result = script(cmd);
                if (result) return;
            }

            if (BasicCommandHandler.CheckCmd(cmd)) {
                BasicCommandHandler.RunCmd(cmd, redirect);
                return;
            }
            Messages.UnknownCommand(cmd.Base).Print();
        }

        bool IsExecutable(in string cmd)
        {
            return File.Exists(Path.Combine(Program.AppDirectory, "apps", cmd) + ".exe")
                || File.Exists(Path.Combine(Program.AppDirectory, "apps", cmd))
                || (cmd[0] == '.' && File.Exists(Path.Combine(Program.CurrentDirectory, cmd.Substring(1))));
        }
        void PrintPrefix()
        {
            CoffeeString.Write("CT ", ConsoleColor.Green);
            CoffeeString.Write(Directory.GetCurrentDirectory(), ConsoleColor.DarkGray);
            CoffeeString.Write("> ", ConsoleColor.DarkCyan);
        }
        bool CheckIfRedirectOutputToFile(in Command cmd)
        {
            return cmd.Keys.Contains(">") || cmd.Keys.Contains(">>");
        }
        void SaveOutputToFile(Command cmd)
        {
            bool append = false;
            string filepath = string.Empty;
            for (int i = 0; i < cmd.Keys.Count && i < cmd.Values.Count; ++i) {
                if (cmd.Keys[i] == ">") {
                    filepath = cmd.Values[i];
                    cmd.Keys.RemoveAt(i);
                    cmd.Values.RemoveAt(i);
                    break;
                }
                if (cmd.Keys[i] == ">>") {
                    cmd.Keys.RemoveAt(i);
                    filepath = cmd.Values[i];
                    cmd.Values.RemoveAt(i);
                    append = true;
                    break;
                }
            }
            if (filepath == string.Empty) {
                cmd.Keys.Remove(">");
                cmd.Keys.Remove(">>");
                HandleCommand(cmd, false);
                return;
            }

            WriteOutputToFile(cmd, filepath, append);
        }
        void WriteOutputToFile(Command cmd, in string path, bool append)
        {
            string result = CoffeeString.RedirectOutputToString(() => HandleCommand(cmd, true));
            if (result[result.Length - 1] == '\b') result = result.Substring(0, result.Length - 1);

            using (var file = new StreamWriter(path, append)) {
                file.Write(result);
            }
        }
    }
}
