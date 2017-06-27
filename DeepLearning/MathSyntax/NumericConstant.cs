using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeepLearning.MathSyntax
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

        public bool IsConstant(ArgumentValue Non_Constant)
        {
            return true;
        }

        public SyntaxBlock Derivative(ArgumentValue ArgumentToDerive)
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

        public XElement Serialize()
        {
            var i = new XElement("NumericConstant");
            i.Value = value.ToString();
            return i;
        }
    }
}
