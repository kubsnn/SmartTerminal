using Cofftea.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cofftea
{
    internal static class Settings
    {
        static Input input;
        public static void Load()
        {
            Program.Handlers = new List<Program.CommandHandler>();
            LoadHandlers();
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
            };

            Thread.Sleep(200);
            input = new Input(__Handler);
            CoffeeString.InstantPrint = true;
            LoadLocalFiles();
            LoadBuiltInHints();

        }

        public static void Reload()
        {
            Load();
            CoffeeString.WriteLine("Reload complete!", ConsoleColor.Magenta, 0, true, ConsoleColor.DarkMagenta);
        }

        private static void LoadBuiltInHints()
        {
            string json = Encoding.ASCII.GetString(Resources.hints).Replace("?", "");

            var data = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);

            foreach (var item in data) {
                foreach (string hint in item.Value) {
                    CoffeeString.AddHintElement(item.Key, hint);
                }
            }
        }
        static void LoadLocalFiles()
        {
            CoffeeString.TempCommands["cd"] = new List<string>();
            BasicCommandHandler.Handle(input.GetCommand($"cd \"{Program.CurrentDirectory}\""));

            var files = (from file in Directory.GetFiles(Program.AppDirectory)
                         where file.EndsWith(".exe")
                         let p = Path.GetRelativePath(Program.AppDirectory, file)
                         select p.Contains(' ') ? '"' + p + '"' : p).ToList();
            files.Remove(Program.AppShortName);


            CoffeeString.TempCommands["nearby"] = files;
        }
        static void LoadHandlers()
        {
            Program.Handlers.Add(scripts.StringGenerator.Handle);
        }
        static void __Handler(Command _) { }
    }
}
