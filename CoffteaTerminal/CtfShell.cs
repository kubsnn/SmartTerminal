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
            input = new Input(HandleCommand);
            //CoffeeString.InstantPrint = true;
        }

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
                    //CoffeeString.WriteLine("\nAn error occured while reading input. Try again.", ConsoleColor.Red);
                    //Console.WriteLine(e.ToString());
                }
            }
        }
        void HandleCommand(Command cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.Base) || cmd.Base == "INSERT") return;

            if (BasicCommandHandler.CanHandle(cmd.Base)) {
                BasicCommandHandler.Handle(cmd);
                return;
            }

            if (IsExecutable(cmd.Base)) {
                if (cmd.Base[0] == '.') cmd.Base = cmd.Base.Substring(1);
                var exec = new ScriptExecutor(cmd);
                exec.Execute();
                return;
            }

            if (BasicCommandHandler.CheckCmd(cmd)) {
                BasicCommandHandler.RunCmd(cmd);
                return;
            }
            Messages.UnknownCommand(cmd.Base).Print();
        }

        bool IsExecutable(string cmd)
        {
            return File.Exists(Path.Combine(Program.AppDirectory, "apps", cmd) + ".exe")
                || (cmd[0] == '.' && File.Exists(Path.Combine(Program.CurrentDirectory, cmd.Substring(1))));
        }
        void PrintPrefix()
        {
            CoffeeString.Write("CT ", ConsoleColor.Green);
            CoffeeString.Write(Directory.GetCurrentDirectory(), ConsoleColor.DarkGray);
            CoffeeString.Write("> ", ConsoleColor.DarkCyan);
        }
    }
}
