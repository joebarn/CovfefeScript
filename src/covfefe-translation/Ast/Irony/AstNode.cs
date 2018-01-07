using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;

namespace CovfefeScript.Translation.Ast.Irony
{
    public abstract class AstNode : AstNodeBase, IAstNodeInit
    {
        protected bool _includeKeyTerms = false;

        protected virtual void OnInit(ParseTreeNode parseTreeNode)
        {

        }

        public virtual void Init(AstContext context, ParseTreeNode parseTreeNode)
        {
            _parseTreeText = parseTreeNode.ToString();
            _bnfTerm = parseTreeNode.Term;
            _sourceSpan = parseTreeNode.Span;

            //parseTreeNode.AstNode = this;

            if (parseTreeNode.Term.Name == "integer")
            {
                int x = 5;
            }

            AddAllAstNodes(_childNodes, parseTreeNode);
            OnInit(parseTreeNode);
        }

        protected void AddImmediateAstNodes(List<AstNodeBase> childNodes, ParseTreeNode parseTreeNode)
        {
            foreach (ParseTreeNode ptn in parseTreeNode.ChildNodes)
            {
                if (ptn.AstNode != null)
                {
                    if (ptn.AstNode is AstNode)
                    {
                        childNodes.Add((AstNode)ptn.AstNode);
                    }
                    else
                    {
                        AddImmediateAstNodes(childNodes, ptn);
                    }
                }
                else
                {
                    if (_includeKeyTerms)
                    {
                        childNodes.Add(new KeyTermAstNode(ptn.ToString(), ptn.Term, ptn.Span));
                    }
                }
            }

        }

        protected void AddAllAstNodes(List<AstNodeBase> childNodes, ParseTreeNode parseTreeNode)
        {


            foreach (ParseTreeNode ptn in parseTreeNode.ChildNodes)
            {
                if (ptn.AstNode != null)
                {
                    var ast = ptn.AstNode as AstNode;

                    if (ast != null)
                    {
                        if (ast._parent == null)
                        {
                            ast._parent = this;
                            childNodes.Add((AstNode)ptn.AstNode);
                        }

                    }
                }
                else
                {
                    if (_includeKeyTerms)
                    {
                        childNodes.Add(new KeyTermAstNode(ptn.ToString(), ptn.Term, ptn.Span));
                    }
                }

                AddAllAstNodes(childNodes, ptn);

            }


        }


    }
}
