using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeepLearning.MathSyntax
{
    class Variable : Abstract_variable
    {
        /// <summary>
        /// Creates a variable.
        /// </summary>
        /// <param name="Argument">An instance of the ArgumentValue class with which the value is controlled.</param>
        public Variable(ArgumentValue Argument) : base(Argument, false) { }

        public override SyntaxBlock Derivative(ArgumentValue ArgumentToDerive)
        {
            if(ArgumentToDerive != Argument)
            {
                return new NumericConstant(0);
            }
            return new NumericConstant(1);
        }

        public override List<ArgumentValue> GetAllVariables(bool OnlyNonConstants)
        {
            var list = new List<ArgumentValue>();
            list.Add(Argument);
            return list;
        }

        public override bool IsConstant(ArgumentValue Non_Constant)
        {
            if(Argument != Non_Constant)
            {
                return true;
            }
            return false;
        }

        public override XElement Serialize()
        {
            var i = new XElement("Variable");
            i.Add(Argument.Serialize());
            return i;
        }
    }
 }
