using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CovfefeScript.Translation.Ast;
using CovfefeScript.Translation.Ast.Irony;

namespace CovfefeScript.Translation.Wasm
{
    public class Assembler
    {
        protected StringBuilder _wast = new StringBuilder();
        protected int _level = 0;
        protected string _tabs = "";

        protected int _blockLevel = 0;

        protected void Indent()
        {
            _level++;
            _tabs = new string('\t', _level);
        }

        protected void Dedent()
        {
            _level--;
            _tabs = new string('\t', _level);
        }

        protected void WriteLine(string text = "")
        {
            _wast.Append($"{_tabs}{text}\r\n");
        }

        protected string GetText()
        {
            return _wast.ToString();
        }

        public string Assemble(SourceFileAstNode sourceFile)
        {
            WriteLine("(module");

            Indent();

            WriteLine("(func $print (import \"imports\" \"print\") (param i32))");

            foreach (var trigger in sourceFile?.ChildNodes)
            {
                if (trigger is TriggerAstNode)
                {
                    var label = trigger[typeof(LabelAstNode)] as LabelAstNode;
                    var parameters = trigger[typeof(ParametersAstNode)] as ParametersAstNode;
                    var statements = trigger.Find(typeof(StatementAstNode));

                    //get params
                    List<string> parmList = new List<string>(); ;
                    foreach (var p in parameters.ChildNodes)
                    {
                        if (p is LabelAstNode)
                        {
                            parmList.Add(((LabelAstNode)p).Value);
                        }
                    }
                    string parms = string.Join(" ", parmList.Select(p => $"(param ${p} i32)"));

                    //get locals (look for cucks)
                    List<string> localList = new List<string>(); ;
                    foreach (var statement in statements)
                    {
                        if (statement.ChildNodes[0] is CuckAstNode)
                        {
                            localList.Add(((LabelAstNode)statement.ChildNodes[0].ChildNodes[0]).Value);
                        }
                    }
                    string locals = string.Join(" ", localList.Select(l => $"(local ${l} i32)"));


                    WriteLine();
                    WriteLine($"(func ${label.Value} {parms} (result i32) {locals}");

                    Indent();
                    WriteLine($"i32.const 0"); //default return value

                    foreach (var statement in statements)
                    {
                        Assemble(statement);
                    }

                    Dedent();

                    WriteLine($")");


                }
            }

            WriteLine();
            WriteLine("(export \"main\" (func $main))");

            WriteLine();

            Dedent();

            WriteLine(")");

            return GetText();
        }


        protected void Assemble(AstNodeBase statement)
        {
            var instruction = statement.ChildNodes[0];

            if (instruction is CuckAstNode)
            {
                if (instruction.ChildNodes.Count == 2)
                {
                    WriteLine();
                }
            }
            else
            {
                WriteLine();
            }

            if (instruction is CuckAstNode || instruction is AssignmentAstNode)
            {

                if (instruction.ChildNodes.Count == 2)
                {
                    if (instruction is CuckAstNode)
                    {
                        WriteLine(";; cuck assignment");
                    }
                    else
                    {
                        WriteLine(";; assignment");
                    }

                    Assemble(((LValueAstNode)instruction.ChildNodes[1]));
                    WriteLine($"set_local ${((LabelAstNode)instruction.ChildNodes[0]).Value}");
                }
            }
            else if (instruction is BingAstNode || instruction is BongAstNode)
            {
                WriteLine(";; bing/bong");
                WriteLine($"get_local ${((LabelAstNode)instruction.ChildNodes[0]).Value}");
                WriteLine($"i32.const 1");

                if (instruction is BingAstNode)
                {
                    WriteLine($"i32.add");
                }
                else
                {
                    WriteLine($"i32.sub");
                }

                WriteLine($"set_local ${((LabelAstNode)instruction.ChildNodes[0]).Value}");
            }
            else if (instruction is ShitPostAstNode)
            {
                WriteLine(";; shitpost");
                Assemble(((LValueAstNode)instruction.ChildNodes[0]));
                WriteLine($"call $print");
            }
            else if (instruction is BtfoAstNode)
            {
                WriteLine(";; btfo");
                WriteLine($"drop");
                Assemble(((LValueAstNode)instruction.ChildNodes[0]));
                WriteLine($"return");

            }
            else if (instruction is GrabAstNode)
            {
                WriteLine(";; grab");
                WriteLine($"set_local ${((LabelAstNode)instruction.ChildNodes[0]).Value}");
            }
            else if (instruction is CallAstNode)
            {
                WriteLine(";; call");

                var arguments = instruction[typeof(ArgumentsAstNode)] as ArgumentsAstNode;

                foreach (var argugment in arguments.ChildNodes)
                {
                    Assemble(((LValueAstNode)argugment));
                }

                WriteLine($"call ${((LabelAstNode)instruction.ChildNodes[0]).Value}");

            }
            else if (instruction is CaterpillarAstNode)
            {
                WriteLine(";; caterpillar");

                WriteLine($"block $block{_blockLevel}");
                Indent();

                WriteLine($"loop $loop{_blockLevel}");
                Indent();

                _blockLevel++;
                var statements = instruction.Find(typeof(StatementAstNode));

                foreach (var nestedStatement in statements)
                {
                    Assemble(nestedStatement);
                }
                _blockLevel--;

                WriteLine("br 0");

                Dedent();
                WriteLine("end");

                Dedent();
                WriteLine("end");

            }
            else if (instruction is YerFiredAstNode)
            {
                WriteLine(";; yerfired");
                WriteLine($"br $block{_blockLevel-1}");
            }
            else if (instruction is IfAstNode)
            {
                WriteLine(";; if");
                Assemble((LValueAstNode)instruction.ChildNodes[0]);
                Assemble((LValueAstNode)instruction.ChildNodes[2]);

                //equals is the only test
                WriteLine("i32.eq");
                WriteLine("if");
                Indent();

                var statements = instruction.Find(typeof(StatementAstNode));

                foreach (var nestedStatement in statements)
                {
                    Assemble(nestedStatement);
                }

                Dedent();
                WriteLine("end");

            }


        }


        protected void Assemble(LValueAstNode lValue)
        {
            //foo
            //5
            //5+foo
            //foo+5
            //foo+foo
            //5+5

            if (lValue.ChildNodes.Count==1)
            {
                AssembleValueOrLable(lValue.ChildNodes[0]);
            }
            else
            {
                AssembleValueOrLable(lValue.ChildNodes[0]);
                AssembleValueOrLable(lValue.ChildNodes[2]);

                //only two
                if (((OperatorAstNode)lValue.ChildNodes[1]).Value=="+++")
                {
                    WriteLine("i32.add");
                }
                else
                {
                    WriteLine("i32.sub");
                }

            }

        }

        protected void AssembleValueOrLable(AstNodeBase node)
        {
            if (node is LabelAstNode)
            {
                WriteLine($"get_local ${((LabelAstNode)node).Value}");
            }
            else
            {
                WriteLine($"i32.const {((IntegerAstNode)node).Value}");
            }

        }

    }
}
