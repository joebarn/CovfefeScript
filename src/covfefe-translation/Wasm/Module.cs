using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CovfefeScript.Translation.Wasm
{
    public class Module
    {
        public string Wast { internal set; get; }
        public byte[] Wasm { internal set; get; }

    }
}
