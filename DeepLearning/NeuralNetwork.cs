using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathSyntax;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace DeepLearning
{
    class NeuralNetwork
    {
        private const double Stepsize = 0.01;

        List<InputNeuron> Input = new List<InputNeuron>();
        List<OutputNeuron> Output = new List<OutputNeuron>();
        List<List<Neuron>> InbetweenLayers = new List<List<Neuron>>();

        List<Tuple<OutputData, SyntaxBlock, List<Tuple<ArgumentValue, SyntaxBlock>>>> OutputFormulas = new List<Tuple<OutputData, SyntaxBlock, List<Tuple<ArgumentValue, SyntaxBlock>>>>() ; //<output-node data, formula for result of output node, [<node-connection, partial derivative>]>

        /// <summary>
        /// Builds the equations for the network. It first calculates the formula for the result of an output node and saves it with the corresponding output node ArgumentValue (the output node's value).
        /// Then it calculates all the partial derivatives for that function and saves it per derived variable.
        /// </summary>
        private void BuildEquations()
        {
            Parallel.ForEach(Output, outputneuron =>
            {
                var resultformula = outputneuron.BuildEquation();
                resultformula = resultformula.Simplify();

                var partial_deritatives = Derivatives.CalculatePartialDerivatives(resultformula);

                OutputFormulas.Add(new Tuple<OutputData, SyntaxBlock, List<Tuple<ArgumentValue, SyntaxBlock>>>(outputneuron.Value, resultformula, partial_deritatives));
            });            
        }

        /// <summary>
        /// Creates a new neural network with random values between -1 and 1, according to specifications.
        /// </summary>
        /// <param name="Inputs">List of ArgumentValue's which are used as the input nodes.</param>
        /// <param name="Outputs">List of ArgumentValue's which are used as the output nodes.</param>
        /// <param name="InbetweenLayersSize">An array of integers. Each element represents one inbetween layer and its value represents the amount of nodes in that layer.</param>
        public NeuralNetwork(List<ArgumentValue> Inputs, List<OutputData> Outputs, int[] InbetweenLayersSize)
        {
            for (int i = 0; i < Outputs.Count; i++)
            {
                Outputs[i].ID = i;
            }

            foreach (var i in Inputs)//Fill the network with nodes
            {
                Input.Add(new InputNeuron(i));
            }
            foreach (var i in Outputs)
            {
                Output.Add(new OutputNeuron(i));
            }
            foreach(var i in InbetweenLayersSize)
            {
                var list = new List<Neuron>();
                for (int x = 0; x < i; x++)
                {
                    list.Add(new Neuron());
                }
                InbetweenLayers.Add(list);
            }
            
            foreach(var i in Input) //link the input layer to the first middle layer
            {
                foreach(var x in InbetweenLayers[0])
                {
                    i.LinkTo(x, RandomVariableValue());
                }
            }

            foreach(var i in InbetweenLayers.Last()) //link the last middle layer to the output layer
            {
                foreach(var x in Output)
                {
                    i.LinkTo(x, RandomVariableValue());
                }
            }

            if (InbetweenLayers.Count > 1) //if theres more than one inbetween layer
            {
                for (int i = 0; i < InbetweenLayers.Count - 1; i++) //for each layer except the last one
                {
                    foreach (var x in InbetweenLayers[i]) //for each node in that layer
                    {
                        foreach (var y in InbetweenLayers[i + 1]) // for each node in the next layer
                        {
                            x.LinkTo(y, RandomVariableValue()); //link them
                        }
                    }
                }
            }

            BuildEquations();
            Save();
        }

        private NeuralNetwork() { }

        public void Save()
        {
            var input = new XElement("Input");
            var output = new XElement("Output");
            var argumentvalues = new XElement("Argumentvalues");

            Dictionary<int, ArgumentValue> argumentValues = new Dictionary<int, ArgumentValue>();

            foreach (var i in Input)
            {
                var inputNeuron = new XElement("InputNeuron");
                inputNeuron.Value = i.Value.ID.ToString();
                input.Add(inputNeuron);
                if (!argumentValues.ContainsKey(i.Value.ID))
                    argumentValues.Add(i.Value.ID, i.Value);
            }

            //
            foreach(var i in OutputFormulas)
            {
                var outputNeuron = new XElement("OutputNeuron");
                var outputFormula = new XElement("OutputFormula");
                outputFormula.Add(i.Item2.Serialize());
                var partialDerivatives = new XElement("PartialDerivatives");
                foreach(var derivative in i.Item3)
                {
                    var id = derivative.Item1.Serialize();

                    if (!argumentValues.ContainsKey(derivative.Item1.ID))
                        argumentValues.Add(derivative.Item1.ID, derivative.Item1);

                    var Formula = new XElement("Formula");
                    Formula.Add(derivative.Item2.Serialize());
                    var theDerivative = new XElement("Derivative");
                    theDerivative.Add(id);
                    theDerivative.Add(Formula);
                    partialDerivatives.Add(theDerivative);
                }

                var ID = new XElement("ID");
                ID.Value = i.Item1.ID.ToString();

                outputNeuron.Add(ID);
                outputNeuron.Add(outputFormula);
                outputNeuron.Add(partialDerivatives);
                output.Add(outputNeuron);
            }
            //

            foreach( var i in argumentValues.Keys)
            {
                var element = new XElement("argumentValue");
                var id = new XElement("ID");
                id.Value = i.ToString();
                var value = new XElement("Value");
                value.Value = argumentValues[i].Value.ToString();
                element.Add(id);
                element.Add(value);
                argumentvalues.Add(element);
            }

            var Root = new XElement("Root");
            Root.Add(input);
            Root.Add(output);
            Root.Add(argumentvalues);

            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.ShowDialog();
            StreamWriter writer = new StreamWriter(save.FileName);
            writer.Write(Root.ToString());
            writer.Close();
        }

        public static NeuralNetwork Load(List<ArgumentValue> Inputs, List<OutputData> Outputs)
        {
            NeuralNetwork NewNeuralNet = new NeuralNetwork();

            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.FileName = "testneuralnet";
            open.ShowDialog();
            StreamReader reader = new StreamReader(open.FileName);
            string wholeThing = reader.ReadToEnd();
            reader.Close();

            var savedFile = XDocument.Parse(wholeThing);
            var input = savedFile.Root.Descendants("Input");
            var output = savedFile.Root.Descendants("Output");
            var argumentvalues = savedFile.Root.Descendants("Argumentvalues");

            var inputneurons = input.Descendants("InputNeuron").ToList();
            if(inputneurons.Count != Inputs.Count)
            {
                throw new Exception("Cant load, inputs do not match up");
            }

            var outputneurons = output.Descendants("OutputNeuron").ToList();
            if(outputneurons.Count != Outputs.Count)
            {
                throw new Exception("Cant load, outputs do not match up");
            }

            var argumentValueList = argumentvalues.Descendants("argumentValue");

            Dictionary<int, ArgumentValue> argumentValuesReal = new Dictionary<int, ArgumentValue>();

            foreach (var i in argumentValueList)
            {
                var id = int.Parse(i.Descendants("ID").First().Value);
                var value = double.Parse(i.Descendants("Value").First().Value);
                argumentValuesReal.Add(id, new ArgumentValue(value, id));
            }

            for (int index = 0; index < Inputs.Count; index++)
            {
                var id = int.Parse(inputneurons[index].Value);
                Inputs[index].ID = id;
                Inputs[index].Value = argumentValuesReal[id].Value;
                argumentValuesReal[id] = Inputs[index];
            }

            for (int index = 0; index < Outputs.Count; index++)
            {
                var currneuron = outputneurons[index];
                var positionID = int.Parse(currneuron.Descendants("ID").First().Value);
                //output data, syntaxblock, List<Tuple<argumentvalue, syntaxblock>>
                var outputFormula = FromXML(currneuron.Descendants("OutputFormula").First().Elements().First(), argumentValuesReal);
                var partialDerivatives = currneuron.Descendants("Derivative");
                List<Tuple<ArgumentValue, SyntaxBlock>> element3 = new List<Tuple<ArgumentValue, SyntaxBlock>>();
                foreach(var derivative in partialDerivatives)
                {
                    var id = int.Parse(derivative.Descendants("ArgumentValue").First().Value);
                    var syntaxblock = FromXML(derivative.Descendants("Formula").First().Elements().First(), argumentValuesReal);
                    element3.Add(new Tuple<ArgumentValue, SyntaxBlock>(argumentValuesReal[id], syntaxblock));
                }
                NewNeuralNet.OutputFormulas.Add(new Tuple<OutputData, SyntaxBlock, List<Tuple<ArgumentValue, SyntaxBlock>>>(Outputs[positionID], outputFormula, element3));
            }


            return NewNeuralNet;
        }

        private static SyntaxBlock FromXML(XElement element, Dictionary<int, ArgumentValue> argumentValues)
        {
            switch (element.Name.LocalName)
            {
                case "Sum":
                    var children = element.Elements().ToList();
                    var A = FromXML(children[0], argumentValues);
                    var B = FromXML(children[1], argumentValues);
                    return new Sum(A, B);
                case "Product":
                    children = element.Elements().ToList();
                    A = FromXML(children[0], argumentValues);
                    B = FromXML(children[1], argumentValues);
                    return new Sum(A, B);
                case "Variable":
                    var id = int.Parse(element.Elements().First().Value);
                    return new Variable(argumentValues[id]);
                case "VariableConstant":
                    id = int.Parse(element.Elements().First().Value);
                    return new VariableConstant(argumentValues[id]);
                default:
                    throw new NotImplementedException();
            }
        }


        Random random = new Random();
        int Counter = 0;
        /// <summary>
        /// Builds a new ArgumentValue with a random value between -1 and 1, to be used for inbetween nodes.
        /// </summary>
        /// <returns>ArgumentValue with a random value between -1 and 1</returns>
        private ArgumentValue RandomVariableValue()
        {
            var i = new ArgumentValue(Counter.ToString());
            Counter++;
            i.Value = (random.NextDouble()-0.5)*2;
            return i;
        }

        public void CalculateResults()
        {
            foreach(var i in OutputFormulas)
            {
                i.Item1.Value = i.Item2.Calculate();
            }
        }

        public void Learn()
        {
            Dictionary<ArgumentValue, double> TotalSlope = new Dictionary<ArgumentValue, double>();
            foreach(var outputnodeFormulas in OutputFormulas)
            {
                foreach(var PartialDerivative in outputnodeFormulas.Item3)
                {
                    double InDict;
                    TotalSlope.TryGetValue(PartialDerivative.Item1, out InDict);
                    if (outputnodeFormulas.Item1.MustBeHigh)
                    {
                        TotalSlope[PartialDerivative.Item1] = InDict - PartialDerivative.Item2.Calculate();
                    }
                    else
                    {
                        TotalSlope[PartialDerivative.Item1] = InDict + PartialDerivative.Item2.Calculate();
                    }
                }
            }

            var keys = TotalSlope.Keys;
            double TotalLenghtSquared = 0;
            foreach(var key in keys)
            {
                TotalLenghtSquared += TotalSlope[key]* TotalSlope[key];
            }
            double TotalLenght = Math.Sqrt(TotalLenghtSquared);
            foreach(var key in keys)
            {
                key.Value -= TotalSlope[key] / TotalLenght * Stepsize;
            }
        }
    }
}
