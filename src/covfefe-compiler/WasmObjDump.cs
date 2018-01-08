using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovfefeScript.Compiler
{
    public class WasmObjDump
    {
        protected string _output;

        public string Output
        {
            get
            {
                return _output;
            }
        }

        protected int Run(string fileName, string arg)
        {
            //compile wat
            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = $"{fileName} {arg}";
            start.FileName = "wasm-objdump.exe";
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.UseShellExecute = false;

            using (Process proc = Process.Start(start))
            {
                _output = proc.StandardOutput.ReadToEnd();
                _output += proc.StandardError.ReadToEnd();
                proc.WaitForExit();
                return proc.ExitCode;

            }
        }



        public int Disassemble(string fileName)
        {
            return Run(fileName, "-d");
        }

        public int Details(string fileName)
        {
            return Run(fileName, "-x");
        }

    }
}
