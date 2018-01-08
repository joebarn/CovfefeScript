using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using CovfefeScript.Translation.Ast;
using CovfefeScript.Translation.Ast.Irony;

/* 
 
 [foo].[ha] = 1 AND [foo].[ha] = 1

*/

namespace CovfefeScript.Translation
{
    [Language("CovfefeScript", "1.0", "A Fantastic Language")]
    public class CovfefeGrammar : Grammar
    {
        public override void BuildAst(LanguageData language, ParseTree parseTree)
        {
            if (LanguageFlags.IsSet(LanguageFlags.CreateAst))
            {
                var astContext = new AstContext(language);
                astContext.DefaultNodeType = typeof(DefaultAstNode);
                astContext.DefaultLiteralNodeType = typeof(DefaultAstNode);
                astContext.DefaultIdentifierNodeType = typeof(DefaultAstNode);


                var astBuilder = new AstBuilder(astContext);
                astBuilder.BuildAst(parseTree);
            }
        }


        public CovfefeGrammar() : base(false) // true means case sensitive
        {
            GrammarComments = @"A Fantastic Language. Case-insensitive.";

            //comments
            CommentTerminal eolComment = new CommentTerminal("eolComment", "//", "\n", "\r");
            NonGrammarTerminals.Add(eolComment);

            //numbers
            NumberLiteral integer = new NumberLiteral("integer", NumberOptions.IntOnly|NumberOptions.AllowSign, typeof(IntegerAstNode));
            integer.DefaultIntTypes = new TypeCode[] { TypeCode.Int32 };
            integer.AddPrefix("0b", NumberOptions.Binary);
            integer.AddPrefix("0x", NumberOptions.Hex);

            //identifier
            IdentifierTerminal label_terminal = new IdentifierTerminal("label_terminal");

            //keywords
            KeyTerm TRIGGER = ToTerm("trigger");
            KeyTerm OPENBRACE = ToTerm("{");
            KeyTerm CLOSEBRACE = ToTerm("}");
            KeyTerm CUCK = ToTerm("cuck");
            KeyTerm GRAB = ToTerm("grab");
            KeyTerm ASSIGNOR = ToTerm("=");
            KeyTerm ADD = ToTerm("+++");
            KeyTerm SUB = ToTerm("---");
            KeyTerm BING = ToTerm("bing");
            KeyTerm BONG = ToTerm("bong");
            KeyTerm SHITPOST = ToTerm("shitpost");
            KeyTerm BTFO = ToTerm("btfo");
            KeyTerm YERFIRED = ToTerm("yerfired");
            KeyTerm IF = ToTerm("if");
            KeyTerm OPENPAREN = ToTerm("(");
            KeyTerm CLOSEPAREN = ToTerm(")");
            KeyTerm EQUALS = ToTerm("==");
            KeyTerm CATERPILLAR = ToTerm("caterpillar");
            KeyTerm IMPORT = ToTerm("import");



            //label
            NonTerminal label = new NonTerminal("label", typeof(LabelAstNode));
            label.Rule = label_terminal;

            //labels
            NonTerminal labels = new NonTerminal("labels");
            labels.Rule = MakeStarRule(labels, "," + label);

            //operators
            NonTerminal @operator = new NonTerminal("operator", typeof(OperatorAstNode));
            @operator.Rule = ADD | SUB;

            //lValue
            NonTerminal lValue = new NonTerminal("lValue", typeof(LValueAstNode));
            lValue.Rule = (label | integer) | ((label | integer) + @operator + (label | integer));

            //lValues
            NonTerminal lValues = new NonTerminal("lValues");
            lValues.Rule = MakeStarRule(lValues, "," + lValue);

            //assignment
            NonTerminal assignment = new NonTerminal("assignment", typeof(AssignmentAstNode));
            assignment.Rule = label + ASSIGNOR + lValue;

            //cuck
            NonTerminal cuck = new NonTerminal("cuck", typeof(CuckAstNode));
            cuck.Rule = CUCK + label + (ASSIGNOR + lValue | Empty);

            //pop
            NonTerminal pop = new NonTerminal("grab", typeof(GrabAstNode));
            pop.Rule = GRAB + label;

            //bing
            NonTerminal bing = new NonTerminal("bing", typeof(BingAstNode));
            bing.Rule = BING + label;

            //bong
            NonTerminal bong = new NonTerminal("bong", typeof(BongAstNode));
            bong.Rule = BONG + label;

            //shitpost
            NonTerminal shitpost = new NonTerminal("shitpost", typeof(ShitPostAstNode));
            //shitpost.Rule = SHITPOST + ((OPENPAREN + lValue + CLOSEPAREN) | Empty);
            shitpost.Rule = SHITPOST + OPENPAREN + lValue + CLOSEPAREN;

            //btfo
            NonTerminal btfo = new NonTerminal("btfo", typeof(BtfoAstNode));
            btfo.Rule = BTFO + ( (OPENPAREN + lValue +CLOSEPAREN) | Empty);

            //yerfired
            NonTerminal yerfired = new NonTerminal("yerfired", typeof(YerFiredAstNode));
            yerfired.Rule = YERFIRED;

            //statements
            NonTerminal statements = new NonTerminal("statements");

            //test
            NonTerminal test = new NonTerminal("test", typeof(TestAstNode));
            @test.Rule = EQUALS;

            //if
            NonTerminal @if = new NonTerminal("if", typeof(IfAstNode));
            @if.Rule = IF + OPENPAREN + lValue + test + lValue + CLOSEPAREN + OPENBRACE + statements + CLOSEBRACE;

            //caterpillar
            NonTerminal caterpillar = new NonTerminal("caterpillar", typeof(CaterpillarAstNode));
            caterpillar.Rule = CATERPILLAR + OPENBRACE + statements + CLOSEBRACE;

            //arguments
            NonTerminal arguments = new NonTerminal("arguments", typeof(ArgumentsAstNode));
            arguments.Rule = (lValue | Empty) + lValues;

            //call
            NonTerminal call = new NonTerminal("call", typeof(CallAstNode));
            call.Rule = label + OPENPAREN + arguments + CLOSEPAREN;

            //statement
            NonTerminal statement = new NonTerminal("statement", typeof(StatementAstNode));
            statement.Rule =  cuck | assignment | bing | bong | shitpost | btfo | yerfired | @if | caterpillar | call | pop ;

            //build statements rule last to include blocks
            statements.Rule = MakeStarRule(statements, statement);

            //parameters
            NonTerminal parameters = new NonTerminal("parameters", typeof(ParametersAstNode));
            parameters.Rule = (label | Empty) + labels;

            //trigger
            NonTerminal trigger = new NonTerminal("trigger", typeof(TriggerAstNode));
            trigger.Rule = TRIGGER + label + OPENPAREN + parameters + CLOSEPAREN + OPENBRACE + (statements) + CLOSEBRACE;

            //triggers
            NonTerminal triggers = new NonTerminal("triggers");
            triggers.Rule = MakeStarRule(triggers, trigger);

            /*
            //import
            NonTerminal import = new NonTerminal("import", typeof(ImportAstNode));
            import.Rule = IMPORT + label + OPENPAREN + parameters + CLOSEPAREN;

            //imports
            NonTerminal imports = new NonTerminal("imports");
            imports.Rule = MakeStarRule(imports, import);
            */

            //file structure
            NonTerminal source_file = new NonTerminal("source_file", typeof(SourceFileAstNode));
            source_file.Rule = triggers;

            Root = source_file;

            LanguageFlags |= LanguageFlags.CreateAst;
        }
    }
}

