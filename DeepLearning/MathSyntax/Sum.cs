using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeepLearning.MathSyntax
{
    class Sum : SyntaxBlock
    {
        /// <summary>
        /// Creates a sum syntax block which adds argument A and B together.
        /// </summary>
        /// <param name="A">The left side of the sum.</param>
        /// <param name="B">The right side of the sum.</param>
        public Sum(SyntaxBlock A, SyntaxBlock B)
        {
            this.A = A;
            this.B = B;
        }

        SyntaxBlock A, B;

        public string print()
        {
            return ("(" + A.print() + " + " + B.print() + ")");
        }

        public List<ArgumentValue> GetAllVariables(bool OnlyNonConstants)
        {
            List<ArgumentValue> listA, listB;
            listA = A.GetAllVariables(OnlyNonConstants);
            listB = B.GetAllVariables(OnlyNonConstants);
            listA.AddRange(listB);
            return listA;
        }

        public bool IsConstant(ArgumentValue Non_Constant)
        {
            if (A.IsConstant(Non_Constant) && B.IsConstant(Non_Constant))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public SyntaxBlock Derivative(ArgumentValue ArgumentToDerive)
        {
            return new Sum(A.Derivative(ArgumentToDerive), B.Derivative(ArgumentToDerive));
        }

        public SyntaxBlock Simplify()
        {
            A = A.Simplify();
            B = B.Simplify();
            

            var a = A as NumericConstant;
            var b = B as NumericConstant;

            if(a == null && b == null) //Neither A nor B are numeric constants, return this sum in its existing state.
                return this;

            if (a != null && b != null) //Both A and B are numeric constants, return new numeric constant that is the sum of both.
                return new NumericConstant(a.value + b.value);

            if (a?.value == 0)  //if a is zero, return B;
                return B;

            if (b?.value == 0)  //if b is zero, return A;
                return A;

            return this; //No simplification possible, return this sum in its existing state.
        }

        public double Calculate()
        {
            return A.Calculate() + B.Calculate();
        }

        public XElement Serialize()
        {
            var i = new XElement("Sum");
            i.Add(A.Serialize());
            i.Add(B.Serialize());
            return i;
        }
    }
}
