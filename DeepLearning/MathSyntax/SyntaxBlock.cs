using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeepLearning.MathSyntax
{
    /// <summary>
    /// The SyntaxBlock interface which all MathSyntax elements implement.
    /// </summary>
    public interface SyntaxBlock
    {
        /// <summary>
        /// Returns whether it is constant, marking one of the variables as non-constant. Used in derivative calculation.
        /// </summary>
        /// <param name="Non_Constant">The ArgumentValue which you desire to not be treated as a constant.</param>
        /// <returns></returns>
        bool IsConstant(ArgumentValue Non_Constant);
        /// <summary>
        /// Prints the formula.
        /// </summary>
        /// <returns></returns>
        string print();
        /// <summary>
        /// Gets all the variables in the formula.
        /// </summary>
        /// <param name="OnlyNonConstants">True to only get non-constant variables, false to get all variables.</param>
        /// <returns>A list with all variables in a function, <b>with duplicates.</b></returns>
        List<ArgumentValue> GetAllVariables(bool OnlyNonConstants);
        /// <summary>
        /// Calculates a derivative function of variable "ArgumentToDerive"
        /// </summary>
        /// <param name="ArgumentToDerive">The variable for which to calculate the derivative.</param>
        /// <returns>A derivative function of "ArgumentToDerive"</returns>
        SyntaxBlock Derivative(ArgumentValue ArgumentToDerive);
        /// <summary>
        /// Simplifies the formula
        /// </summary>
        /// <returns>Returns the value with which replace itself</returns>
        SyntaxBlock Simplify();
        /// <summary>
        /// Calculates the formula with the current values in the arguments.
        /// </summary>
        /// <returns>The solution</returns>
        double Calculate();
        /// <summary>
        /// Serializes elements and all its child elements to x elements.
        /// </summary>
        /// <returns>An x element representing this mathsyntax block</returns>
        XElement Serialize();
    }
}
