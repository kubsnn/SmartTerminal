using Cofftea.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofftea.scripts
{
    internal class StringGenerator
    {
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()_+-=[]{}|;':,./<>?";
        public static bool Handle(Command cmd)
        {
            if (cmd.Base.ToUpper() != "STRING") return false;

            if (cmd.Args.Contains("--help")) {
                Help();
                return true;
            }
            
            GenerateRandomString(cmd).PrintEnumerated(0, true, true);
            return true;
        }
        static CoffeeString GenerateRandomString(Command cmd)
        {

            var alphabetBuilder = new StringBuilder();
            foreach (string arg in cmd.Args) {
                string upper = arg.ToUpper();
                if (upper == "--LOWERCASE" || upper == "-L") alphabetBuilder.Append(lowercase);
                if (upper == "--UPPERCASE" || upper == "-U") alphabetBuilder.Append(uppercase);
                if (upper == "--DIGITS" || upper == "-D") alphabetBuilder.Append(digits);
                if (upper == "--SPECIAL" || upper == "-S") alphabetBuilder.Append(specialChars);
                if (upper == "--ALL" || upper == "-A") alphabetBuilder.Append(lowercase + uppercase + digits + specialChars);
            }
            string alphabet = alphabetBuilder.ToString();

            if (string.IsNullOrEmpty(alphabet)) return CoffeeString.Empty;

            int count = 1;
            int length = 10;
            for (int i = 0; i < cmd.Values.Count; ++i) {
                if (cmd.Keys[i] == "--length") {
                    if(!int.TryParse(cmd.Values[i], out int result)) {
                        return CoffeeString.Empty;
                    }
                    length = result;
                    break;
                }
                if (cmd.Keys[i] == "--count") {
                    if (!int.TryParse(cmd.Values[i], out int result)) {
                        return CoffeeString.Empty;
                    }
                    count = result;
                }
            }
            var random = new Random();

            var cs = new CoffeeString();
            for (int j = 0; j < count; ++j) {
                var sb = new StringBuilder();
                for (int i = 0; i < length; i++) {
                    sb.Append(alphabet[random.Next(alphabet.Length)]);
                }
                cs.AddLine(sb.ToString());
            }
            return cs;
        }
        static void Help()
        {
            var cs = new CoffeeString();
            cs.Add("--lowercase|-l", ConsoleColor.DarkCyan);
            cs.AddLine("\t\t\t\tadds lowercase letters to alphabet");
            cs.Add("--uppercase|-u", ConsoleColor.DarkCyan);
            cs.AddLine("\t\t\t\tadds uppercase letters to alphabet", ConsoleColor.Gray);
            cs.Add("--digits|-d", ConsoleColor.DarkCyan);
            cs.AddLine("\t\t\t\tadds digits to alphabet");
            cs.Add("--special|-s", ConsoleColor.DarkCyan);
            cs.AddLine("\t\t\t\tadds special characters to alphabet", ConsoleColor.Gray);
            cs.Add("--all|-a", ConsoleColor.DarkCyan);
            cs.AddLine("\t\t\t\tgenerate a random string with all characters");
            cs.Add("--length=<int>", ConsoleColor.DarkCyan);
            cs.AddLine("\t\t\t\tsets the length of the string", ConsoleColor.Gray);
            cs.Add("--count=<int>", ConsoleColor.DarkCyan);
            cs.AddLine("\t\t\t\tsets the number of strings to generate");
            cs.Print();
        }
    }
}
