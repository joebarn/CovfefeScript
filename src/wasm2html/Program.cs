using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasm2Html
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName;
            bool covfefeMode = false;

            if (args.Length >= 1)
            {
                fileName = args[0];

                if (args.Length >= 2)
                {
                    covfefeMode = bool.Parse(args[1]);
                }

                HtmlRunner.Generate(fileName, covfefeMode);

            }

        }
    }
}
