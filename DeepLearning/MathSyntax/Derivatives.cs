using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSyntax
{
    public static class Derivatives
    {
        /// <summary>
        /// Calculates all the partial derivatives of a given formula.
        /// </summary>
        /// <param name="Formula">The formula for which to calculate all partial derivatives.</param>
        /// <returns>A list of tuples, containing the VariableArgumentValue for which it was calculated (item1) and the formula itself(item2).</returns>
        public static List<Tuple<VariableArgumentValue, SyntaxBlock>> CalculatePartialDerivatives(SyntaxBlock Formula)
        {
            var AllVariableVariables = Formula.GetAllVariables(true);
            var distinctVariables = AllVariableVariables.Distinct();
            List<Tuple<VariableArgumentValue, SyntaxBlock>> PartialDerivatives = new List<Tuple<VariableArgumentValue, SyntaxBlock>>();
            
            foreach (ArgumentValue i in distinctVariables) {
                PartialDerivatives.Add(new Tuple<VariableArgumentValue, SyntaxBlock>((VariableArgumentValue)i, Formula.Derivative((VariableArgumentValue)i).Simplify()));
            }
            return PartialDerivatives;
        }
        /// <summary>
        /// Calculates the derivative of a simple formula with only one variable.
        /// </summary>
        /// <param name="Formula">The formula for which to calculate the derivative for.</param>
        /// <returns>The derives formula.</returns>
        /// <exception cref="DerivativeException">Throws a DerivativeException when the formula contains more than one variable.</exception>  
        public static SyntaxBlock CalculateDerivative(SyntaxBlock Formula)
        {
            var listOfPartials = CalculatePartialDerivatives(Formula);
            if(listOfPartials.Count > 1)
            {
                throw new DerivativeException("Formula has more than one variable");
            }
            if (listOfPartials.Count < 1)
            {
                throw new DerivativeException("Formula has no derivatives");
            }
            return listOfPartials[0].Item2;
        }
    }
}
