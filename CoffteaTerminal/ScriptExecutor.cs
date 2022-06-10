using Cofftea.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cofftea
{
    internal class ScriptExecutor
    {
        ProcessStartInfo info;
        Stopwatch stopwatch;
        bool redirect;
        public ScriptExecutor(Command cmd, bool redirect = false)
        {
            this.redirect = redirect;
            string path = cmd.Base;
            string args = cmd.ToString().Substring(cmd.Base.Length);
            stopwatch = new Stopwatch();
            if (path.EndsWith(".py")) {
                Load(path, args);
                return;
            }

            string p1 = Path.Combine(Program.AppDirectory, "apps", path + ".exe");
            path = Path.Combine(Program.CurrentDirectory, path);


            if (File.Exists(p1)) path = p1;

            info = new ProcessStartInfo(path, args); 
            info.UseShellExecute = false;
            info.WorkingDirectory = Program.CurrentDirectory;
            
        }
        void Load(string path, string args)
        {
            string p1 = Path.Combine(Program.AppDirectory, "apps", path);
            path = Path.Combine(Program.CurrentDirectory, path);

            if (File.Exists(p1)) path = p1;

            info = new ProcessStartInfo("python", '"' + Path.Combine(Program.AppDirectory, "apps", path) + '"' + " " + args);
            info.UseShellExecute = false;
            info.WorkingDirectory = Program.CurrentDirectory;
        }
        public void Execute()
        {
            bool prev = Console.CursorVisible;
            Console.CursorVisible = true;
            SetRedirect(redirect);

            if (redirect) {
                ExecuteRedirected();
                return;
            }

            stopwatch.Start();
            var p = Process.Start(info);
            
            p.WaitForExit();
            stopwatch.Stop();

            var cs = new CoffeeString() ;
            cs.Add("user time:  ");
            cs.Add($"{stopwatch.ElapsedMilliseconds:N0}", ConsoleColor.Yellow);
            cs.AddLine(" ms");
            cs.Print();
            Console.CursorVisible = prev;
        }
        public void ExecuteRedirected()
        {
            SetRedirect(true);

            var p = Process.Start(info);
            p.WaitForExit();

            CoffeeString.Write(p.StandardOutput.ReadToEnd());
        }
        void SetRedirect(bool flag)
        {
            info.RedirectStandardInput = flag;
            info.RedirectStandardOutput = flag;
            info.RedirectStandardError = flag;
        }
    } 
}
