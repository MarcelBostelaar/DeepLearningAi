using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathSyntax
{
    public abstract class ArgumentValue
    {
        /// <summary>
        /// An abstract class which contains the control for the value of a variable or constant variable, as well as the name for printing.
        /// </summary>
        /// <param name="Name">The name to be used for printing, not required for calculations, only for printing. Duplicate names pose no problems for functionality beyond human readability.</param>
        public ArgumentValue(string Name)
        {
            this.Name = Name;
            Value = 0;
        }
        public string Name { get; private set; }
        public double Value { get; set; }
    }

    public class ConstantArgumentValue : ArgumentValue
    {
        /// <summary>
        /// A class which contains the control for the value of a variable constant, as well as the name for printing.
        /// </summary>
        /// <param name="Name">The name to be used for printing, not required for calculations, only for printing. Duplicate names pose no problems for functionality beyond human readability.</param>
        public ConstantArgumentValue(string Name) : base(Name) { }
    }
    public class VariableArgumentValue : ArgumentValue
    {
        /// <summary>
        /// A class which contains the control for the value of a true variable, as well as the name for printing.
        /// </summary>
        /// <param name="Name">The name to be used for printing, not required for calculations, only for printing. Duplicate names pose no problems for functionality beyond human readability.</param>
        public VariableArgumentValue(string Name) : base(Name) { }
    }
}
