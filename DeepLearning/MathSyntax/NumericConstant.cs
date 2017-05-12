using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSyntax
{
    class NumericConstant : SyntaxBlock
    {
        /// <summary>
        /// Creates a numeric constant. Value is unchangable once created.
        /// </summary>
        /// <param name="Value">The value for the constant.</param>
        public NumericConstant(double Value)
        {
            value = Value;
        }
        public double value { get; private set; }
        
        public string print()
        {
            return value.ToString();
        }

        public List<ArgumentValue> GetAllVariables(bool OnlyNonConstants)
        {
            return new List<ArgumentValue>();
        }

        public bool IsConstant(VariableArgumentValue Non_Constant)
        {
            return true;
        }

        public SyntaxBlock Derivative(VariableArgumentValue ArgumentToDerive)
        {
            return new NumericConstant(0);
        }

        public SyntaxBlock Simplify()
        {
            return this;
        }

        public double Calculate()
        {
            return value;
        }
    }
}
