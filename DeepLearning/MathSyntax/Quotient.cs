using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeepLearning.MathSyntax
{
    class Quotient : SyntaxBlock
    {
        SyntaxBlock A, B;
        public Quotient(SyntaxBlock A, SyntaxBlock B)
        {
            this.A = A;
            this.B = B;
        }

        public double Calculate()
        {
            return A.Calculate() / B.Calculate();
        }

        public SyntaxBlock Derivative(ArgumentValue ArgumentToDerive)
        {
            //(A'*B + -1(B'*A))/(B*B) == (A'*B - B'*A)/B^2
            return new Quotient(new Sum(new Product(A.Derivative(ArgumentToDerive), B),
                new Product(new NumericConstant(-1), new Product(B.Derivative(ArgumentToDerive), A))), new Product(B, B));
        }

        public List<ArgumentValue> GetAllVariables(bool OnlyNonConstants)
        {
            var list = A.GetAllVariables(OnlyNonConstants);
            list.AddRange(B.GetAllVariables(OnlyNonConstants));
            return list;
        }

        public bool IsConstant(ArgumentValue Non_Constant)
        {
            return A.IsConstant(Non_Constant) && B.IsConstant(Non_Constant);
        }

        public string print()
        {
            return "(" + A.print() + " / " + B.print() + ")";
        }

        public SyntaxBlock Simplify()
        {
            A = A.Simplify();
            B = B.Simplify();

            var a = A as NumericConstant;
            var b = B as NumericConstant;

            if (a == null && b == null) //Neither A nor B are numeric constants, return this quotient in its existing state.
                return this;

            if (a != null && b != null) //Both A and B are numeric constants, return new numeric constant that is the quotient of both.
                return new NumericConstant(a.value / b.value);
          
            if (a?.value == 0)
                return new NumericConstant(0);
            
            if (b?.value == 1)
            {
                return A;
            }
            else if(b?.value == 0)
            {
                throw new DivideByZeroException("Can't devide by zero!");
            }
            return this; //No simplification possible, return this quotient in its existing state.
        }

        public XElement Serialize()
        {
            var i = new XElement("Quotient");
            var a = new XElement("A");
            a.Add(A.Serialize());
            var b = new XElement("B");
            b.Add(A.Serialize());
            i.Add(a);
            i.Add(b);
            return i;
        }
    }
}
