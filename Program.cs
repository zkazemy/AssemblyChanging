using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AssemblyChanging
{
        public  class Program
    {
        private static string _newAssemblyVersion = String.Empty;
        private static string _newAssemblyFileVersion = String.Empty;
        private static string _sourceDirectory = String.Empty;
        private static string[] _files = null;

        static void Main(string[] args)
        {
            ReadArgs(args);
            if (!Validation()) return;
            ReadAllAssemblyFiles(_sourceDirectory);
            ProcessFiles();
        }

        private static bool Validation()
        {
            var validationResult = true;
            var message = new StringBuilder();
            if (_newAssemblyVersion == String.Empty ||
                _newAssemblyFileVersion == String.Empty ||
                _sourceDirectory == String.Empty)
            {
                message.Append("The parameters '-setAssemblyVersion ,-setAssemblyVersion,-sourceDir ' are mandatory");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
                validationResult = false;
            }

            return validationResult;
        }

        private static void ProcessFiles()
        {

            foreach (var file in _files)
            {
                StreamReader reader = new StreamReader(file);
                StreamWriter writer = new StreamWriter(file + ".out");
                String line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = ProcessLine(line);
                    writer.WriteLine(line);
                }

                reader.Close();
                writer.Close();

                File.Delete(file);
                File.Move(file + ".out", file);
                Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("Successful !");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void  ReadAllAssemblyFiles(string sourceDirectory)
        {
            _files = System.IO.Directory.GetFiles(sourceDirectory,
                "AssemblyInfo.cs", SearchOption.AllDirectories);
        }

        private static void ReadArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-setAssemblyVersion:"))
                {
                    _newAssemblyVersion = args[i].Substring("-setAssemblyVersion:".Length);
                }
                if (args[i].StartsWith("-setAssemblyFileVersion:"))
                {
                    _newAssemblyFileVersion = args[i].Substring("-setAssemblyFileVersion:".Length);
                }
                if (args[i].StartsWith("-sourceDir:"))
                {
                    _sourceDirectory = args[i].Substring("-sourceDir:".Length);
                }
            }

        }

        private static string ProcessLine(string line)
        {
            line = ProcessLinePart(line, "[assembly: AssemblyVersion(\"", _newAssemblyVersion);
            line = ProcessLinePart(line, "[assembly: AssemblyFileVersion(\"", _newAssemblyFileVersion);
            return line;
        }

        private static string ProcessLinePart(string line, string part,string newVersion)
        {
            int spos = line.IndexOf(part);
            if (spos >= 0)
            {
                spos += part.Length;
                int epos = line.IndexOf('"', spos);
                string oldVersion = line.Substring(spos, epos - spos);
                // string newVersion = "";
                bool performChange = newVersion != null;

                if (performChange)
                {
                    StringBuilder str = new StringBuilder(line);
                    str.Remove(spos, epos - spos);
                    str.Insert(spos, newVersion);
                    line = str.ToString();
                }
            }

            return line;
        }

    }
}
