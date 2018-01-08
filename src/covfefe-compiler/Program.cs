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
            string wat;

            if (args.Length == 1)
            {
                input = args[0]; // "in.wat"; //args[0]
            }
            else
            {
                input = "test.cvf";// "in.wat";
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

            if (ext==".cvf")
            {
                //if cvf we need to compile to wat here
                var cvf = File.ReadAllText(input);
                var file = Translation.CovfefeParser.Parse(cvf)?.AstNode;
                var wast = new Translation.Wasm.Assembler().Assemble((Translation.Ast.SourceFileAstNode)file);

                File.WriteAllText("in.wat", wast);
                wat = "in.wat";
            }
            else
            {
                wat = input;
            }


            //compile wat
            var wat2wasm = new Wat2Wasm();

            int exitCode=wat2wasm.CompileFile(wat);


            if (exitCode == 0)
            {
                Console.WriteLine("wat compiled okay");

                HtmlRunner.Generate("out.wasm", ext == ".cvf");

                Console.WriteLine("index.html written");

                var dump = new WasmObjDump();
                dump.Disassemble("out.wasm");
                Console.WriteLine(dump.Output);

            }
            else
            {
                Console.WriteLine("wat2wasm failed");
                Console.WriteLine(wat2wasm.Output);
            }
        }
    }
}
