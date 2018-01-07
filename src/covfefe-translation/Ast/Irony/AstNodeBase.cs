using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;

namespace CovfefeScript.Translation.Ast.Irony
{
    public abstract class AstNodeBase : IBrowsableAstNode
    {
        protected string _parseTreeText;
        protected BnfTerm _bnfTerm;
        protected SourceSpan _sourceSpan;
        protected AstNodeBase _parent;
        protected List<AstNodeBase> _childNodes = new List<AstNodeBase>();

        public override string ToString()
        {
            return $"{_bnfTerm.ToString()}";
        }

        public SourceSpan SourceSpan
        {
            get
            {
                return _sourceSpan;
            }
        }

        public int Position
        {
            get
            {
                return _sourceSpan.Location.Position;
            }
        }

        public IEnumerable GetChildNodes()
        {
            return _childNodes;
        }

        public List<AstNodeBase> ChildNodes
        {
            get
            {
                return _childNodes;
            }
        }

        public AstNodeBase[] Find(Type type)
        {
            List<AstNodeBase> found = new List<AstNodeBase>();

            foreach (var childNode in _childNodes)
            {
                if (childNode.GetType() == type)
                {
                    found.Add(childNode);
                }
            }

            return found.ToArray();
        }

        public AstNodeBase this[Type type]
        {
            get
            {
                foreach (var childNode in _childNodes)
                {
                    if (type.IsAssignableFrom(childNode.GetType()))
                    {
                        return childNode;
                    }
                }

                return null;
            }
        }
    }
}
