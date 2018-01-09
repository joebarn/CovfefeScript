using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wasm2Html
{
    public class HtmlRunner
    {
        public static void Generate(string fileName, bool covfefeMode)
        {
            byte[] wasmBytes = File.ReadAllBytes(fileName);

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
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Wasm2Html.index.html"))
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            string title = "";
            string src = "";
            if (covfefeMode)
            {
                title = "MAGA!!!";
                src = "http://i0.kym-cdn.com/photos/images/facebook/000/862/065/0e9.jpg";
            }
            else
            {
                title = "Wasm2Html Runner";
                src = "http://webassembly.org/css/webassembly.svg";
            }

            html = html.Replace("{wasm}", wasmHex);
            html = html.Replace("{title}", title);
            html = html.Replace("{src}", src);

            File.WriteAllText("index.html", html);
        }
    }
}
