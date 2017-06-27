using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeepLearning.MathSyntax
{
    class Product : SyntaxBlock
    {
        SyntaxBlock A, B;
        /// <summary>
        /// Creates a product syntax block, which multiplies argument A and B together.
        /// </summary>
        /// <param name="A">The left side of the product.</param>
        /// <param name="B">The right side of the product.</param>
        public Product(SyntaxBlock A, SyntaxBlock B)
        {
            this.A = A;
            this.B = B;
        }

        public double Calculate()
        {
            return A.Calculate() * B.Calculate();
        }

        public SyntaxBlock Derivative(ArgumentValue ArgumentToDerive)
        {
            return new Sum(new Product(A.Derivative(ArgumentToDerive), B), new Product(A, B.Derivative(ArgumentToDerive)));
        }

        public List<ArgumentValue> GetAllVariables(bool OnlyNonConstants)
        {
            var lista = A.GetAllVariables(OnlyNonConstants);
            lista.AddRange(B.GetAllVariables(OnlyNonConstants));
            return lista;
        }

        public bool IsConstant(ArgumentValue Non_Constant)
        {
            if (A.IsConstant(Non_Constant) && B.IsConstant(Non_Constant))
                return true;
            return false;
        }

        public string print()
        {
            return ("(" + A.print() + " * " + B.print() + " )");
        }

        public SyntaxBlock Simplify()
        {
            A = A.Simplify();
            B = B.Simplify();


            var a = A as NumericConstant;
            var b = B as NumericConstant;


            if (a == null && b == null) //Neither A nor B are numeric constants, return this product in its existing state.
                return this;

            if (a != null && b != null) //Both A and B are numeric constants, return new numeric constant that is the product of both.
                return new NumericConstant(a.value * b.value);

            if (a?.value == 0)  //if a is zero, return zero;
                return new NumericConstant(0);
            if (b?.value == 0)  //if b is zero, return zero;
                return new NumericConstant(0);


            if (a?.value == 1)  //if a is one, return B;
                return B;
            if (b?.value == 1)  //if b is one, return A;
                return A;

            return this; //No simplification possible, return this sum in its existing state.
        }

        public XElement Serialize()
        {
            var i = new XElement("Product");
            i.Add(A.Serialize());
            i.Add(B.Serialize());
            return i;
        }
    }
}
