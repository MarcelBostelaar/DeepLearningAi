using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeepLearning.MathSyntax
{
    public class ArgumentValue
    {
        private static int IDCounter = 0;
        /// <summary>
        /// An abstract class which contains the control for the value of a variable or constant variable, as well as the name for printing.
        /// </summary>
        /// <param name="Name">The name to be used for printing, not required for calculations, only for printing. Duplicate names pose no problems for functionality beyond human readability.</param>
        public ArgumentValue(string Name)
        {
            this.Name = Name;
            Value = 0;
            ID = IDCounter;
            IDCounter++;
        }
        public ArgumentValue(double value)
        {
            this.Name = "";
            Value = value;
            ID = IDCounter;
            IDCounter++;
        }
        public ArgumentValue(double value, int ID)
        {
            Name = "";
            Value = value;
            this.ID = ID;
        }
        public string Name { get; private set; }
        public double Value { get; set; }
        public int ID { get; set; }

        public XElement Serialize()
        {
            var thing = new XElement("ArgumentValue");
            thing.Value = ID.ToString();
            return thing;
        }
    }

    //public class ArgumentValue : ArgumentValue
    //{
    //    /// <summary>
    //    /// A class which contains the control for the value of a variable constant, as well as the name for printing.
    //    /// </summary>
    //    /// <param name="Name">The name to be used for printing, not required for calculations, only for printing. Duplicate names pose no problems for functionality beyond human readability.</param>
    //    public ArgumentValue(string Name) : base(Name) { }
    //}
    //public class ArgumentValue : ArgumentValue
    //{
    //    /// <summary>
    //    /// A class which contains the control for the value of a true variable, as well as the name for printing.
    //    /// </summary>
    //    /// <param name="Name">The name to be used for printing, not required for calculations, only for printing. Duplicate names pose no problems for functionality beyond human readability.</param>
    //    public ArgumentValue(string Name) : base(Name) { }
    //}
}
