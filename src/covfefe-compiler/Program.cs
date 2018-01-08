using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CovfefeScript.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            //check arguments
            string input;

            if (args.Length == 1)
            {
                input = args[0]; // "in.wat"; //args[0]
            }
            else
            {
                input = "in.wat";
                //Console.WriteLine("bad number of arguments");
                //return;
            }

            //file exists?
            if (!File.Exists(input))
            {
                Console.WriteLine($"can't find file {input}");
                return;
            }


            //wat file or cvf file?
            string ext = Path.GetExtension(input);


            //if cvf we need to compile to wat here

            //compile wat
            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = $"{input} -o out.wasm";
            start.FileName = "wat2wasm.exe";
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            int exitCode;

            using (Process proc = Process.Start(start))
            {
                proc.WaitForExit();
                exitCode = proc.ExitCode;
            }

            if (exitCode == 0)
            {
                Console.WriteLine("wat compiled okay");


                byte[] wasmBytes = File.ReadAllBytes("out.wasm");

                string wasmHex = "";
                bool comma = false;

                foreach (byte b in wasmBytes)
                {
                    if (comma)
                    {
                        wasmHex += ", ";
                    }
                    else
                    {
                        comma = true;
                    }

                    wasmHex += $"0x{b.ToString("X2")}";
                }

                string html = null;
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CovfefeScript.Compiler.index.html"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }

                string src = "";
                if (ext == ".cvf")
                {
                    src = "http://i0.kym-cdn.com/photos/images/facebook/000/862/065/0e9.jpg";
                }
                else
                {
                    src = "http://webassembly.org/css/webassembly.svg";
                }

                html = html.Replace("{wasm}", wasmHex);
                html = html.Replace("{src}", src);

                File.WriteAllText("index.html", html);
                Console.WriteLine("index.html written");

            }
            else
            {
                Console.WriteLine("wat2wasm failed");
            }
        }
    }
}
