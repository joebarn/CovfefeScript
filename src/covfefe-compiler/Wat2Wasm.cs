using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovfefeScript.Compiler
{
    public class Wat2Wasm
    {
        protected string _output;

        public string Output
        {
            get
            {
                return _output;
            }
        }

        public int CompileFile(string fileName)
        {
            //compile wat
            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = $"{fileName} -o out.wasm";
            start.FileName = "wat2wasm.exe";
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

    }
}
