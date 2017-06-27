using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeepLearning.MathSyntax
{
    class VariableConstant : Abstract_variable
    {
        /// <summary>
        /// Creates a variable constant. Acts as a constant that can have its value changed.
        /// </summary>
        /// <param name="Argument">An instance of the ArgumentValue class with which the value is controlled.</param>
        public VariableConstant(ArgumentValue Argument) : base(Argument, true) { }

        public override SyntaxBlock Derivative(ArgumentValue ArgumentToDerive)
        {
            return new NumericConstant(0);
        }

        public override List<ArgumentValue> GetAllVariables(bool OnlyNonConstants)
        {
            var list = new List<ArgumentValue>();
            if (!OnlyNonConstants)
            {
                list.Add(Argument);
            }
            return list;
        }

        public override bool IsConstant(ArgumentValue Non_Constant)
        {
            return true;
        }

        public override XElement Serialize()
        {
            var i = new XElement("VariableConstant");
            i.Add(Argument.Serialize());
            return i;
        }
    }
}
