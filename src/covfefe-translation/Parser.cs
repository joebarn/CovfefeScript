using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace CovfefeScript.Translation
{
    public class CovfefeParser
    {
        public static ParseTreeNode Parse(string covfefe)
        {
            Grammar grammar = new CovfefeGrammar();

            LanguageData language = new LanguageData(grammar);

            Parser parser = new Parser(language);

            ParseTree parseTree = parser.Parse(covfefe);

            ParseTreeNode root = parseTree.Root;

            return root;
        }
    }
}
