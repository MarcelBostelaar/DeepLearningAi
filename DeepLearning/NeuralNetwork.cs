using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepLearning.MathSyntax;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Collections.Concurrent;
using System.Globalization;

namespace DeepLearning
{
    public class NeuralNetwork
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
            if(InbetweenLayersSize.Length < 1)
            {
                throw new Exception("Neuralnet requires at least one inbeteen layer.");
            }
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
        }

        private NeuralNetwork() { }
        
        public void SaveMatrix()
        {
            List<double[,]> matrices = new List<double[,]>();

            var firstMatrix = new double[Input.Count, InbetweenLayers[0].Count];
            for (int x = 0; x < Input.Count; x++)
            {
                foreach(var connection in Input[x].Out)
                {
                    var y = InbetweenLayers[0].IndexOf(connection.To);
                    firstMatrix[x, y] = connection.value.Value;
                }
            }

            matrices.Add(firstMatrix);

            for (int i = 0; i < InbetweenLayers.Count-1; i++)
            {
                var matrix = new double[InbetweenLayers[i].Count, InbetweenLayers[i + 1].Count];
                for (int x = 0; x < InbetweenLayers[i].Count; x++)
                {
                    foreach(var connection in InbetweenLayers[i][x].Out)
                    {
                        var y = InbetweenLayers[i + 1].IndexOf(connection.To);
                        matrix[x, y] = connection.value.Value;
                    }
                }
                matrices.Add(matrix);
            }

            var lastMatrix = new double[InbetweenLayers.Last().Count, Output.Count];
            for (int x = 0; x < InbetweenLayers.Last().Count; x++)
            {
                foreach (var connection in InbetweenLayers.Last()[x].Out)
                {
                    var y = Output.IndexOf((OutputNeuron)connection.To);
                    lastMatrix[x, y] = connection.value.Value;
                }
            }
            matrices.Add(lastMatrix);

            string File = "";
            foreach(var matrix in matrices)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    for (int x = 0; x < matrix.GetLength(0); x++)
                    {
                        File += matrix[x, y].ToString(CultureInfo.InvariantCulture);
                        File += ";";
                    }
                    File = File.TrimEnd(';');
                    File += "\n";
                }
                File += "\n";
            }

            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.Filter = "Comma Seperated Values (*.csv)|*.csv";
            save.ShowDialog();
            StreamWriter writer = new StreamWriter(save.FileName);
            writer.Write(File);
            writer.Close();
        }

        public static NeuralNetwork LoadMatrix(List<ArgumentValue> Inputs, List<OutputData> Outputs)
        {
            NeuralNetwork newNeuralNet = new NeuralNetwork();

            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();
            open.Filter = "Comma Seperated Values (*.csv)|*.csv";
            //open.FileName = "matrixtest.csv";
            open.ShowDialog();
            StreamReader reader = new StreamReader(open.FileName);
            string wholeThing = reader.ReadToEnd();
            reader.Close();

            var lines = wholeThing.Split('\n');

            var splitElements = from line in lines select line.Split(';');

            int lastSize = 1;

            List<List<List<string>>> stringMatrices = new List<List<List<string>>>();

            foreach(var line in splitElements)
            {
                if (line.Length <= 1)
                {
                    lastSize = line.Length;
                }
                else
                {
                    if (line.Length != lastSize)
                    {
                        stringMatrices.Add(new List<List<string>>());
                    }
                    lastSize = line.Length;
                    stringMatrices.Last().Add(line.ToList());
                }
            }

            List<double[,]> matrices = new List<double[,]>();

            foreach(var i in stringMatrices)
            {
                var matrix = new double[i[0].Count, i.Count];
                for (int x = 0; x < matrix.GetLength(0); x++)
                {
                    for (int y = 0; y < matrix.GetLength(1); y++)
                    {
                        matrix[x, y] = double.Parse(i[y][x], CultureInfo.InvariantCulture);
                    }
                }
                matrices.Add(matrix);
            }

            if (Inputs.Count != matrices[0].GetLength(0))
            {
                throw new Exception("Inputs do not match with loaded matrix");
            }

            if (Outputs.Count != matrices.Last().GetLength(1))
            {
                throw new Exception("Outputs do not match with loaded matrix");
            }


            foreach (var i in Inputs)//Fill the network with nodes
            {
                newNeuralNet.Input.Add(new InputNeuron(i));
            }
            foreach (var i in Outputs)//Fill the network with nodes
            {
                newNeuralNet.Output.Add(new OutputNeuron(i));
            }

            //Adds inbetweenlayers for all the inbetween layers
            for (int i = 0; i < matrices.Count-1; i++)
            {
                newNeuralNet.InbetweenLayers.Add(new List<Neuron>());
                for (int y = 0; y < matrices[i].GetLength(1); y++)
                {
                    newNeuralNet.InbetweenLayers[i].Add(new Neuron());
                }
            }

            for (int x = 0; x < matrices[0].GetLength(0); x++)
            {
                for (int y = 0; y < matrices[0].GetLength(1); y++)
                {
                    newNeuralNet.Input[x].LinkTo(newNeuralNet.InbetweenLayers[0][y], new ArgumentValue(matrices[0][x, y]));
                }
            }

            for (int i = 0; i < newNeuralNet.InbetweenLayers.Count-1; i++)
            {
                for (int y = 0; y < matrices[i+1].GetLength(1); y++)
                {
                    for (int x = 0; x < matrices[i + 1].GetLength(0); x++)
                    {
                        newNeuralNet.InbetweenLayers[i][x].LinkTo(newNeuralNet.InbetweenLayers[i + 1][y], new ArgumentValue(matrices[i][x, y]));
                    }
                }
            }

            for (int x = 0; x < matrices.Last().GetLength(0); x++)
            {
                for (int y = 0; y < matrices.Last().GetLength(1); y++)
                {
                    newNeuralNet.InbetweenLayers.Last()[x].LinkTo(newNeuralNet.Output[y], new ArgumentValue(matrices.Last()[x, y]));
                }
            }

            newNeuralNet.BuildEquations();

            return newNeuralNet;
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
            Parallel.ForEach(OutputFormulas, i =>
                i.Item1.Value = i.Item2.Calculate()
            );
        }

        public void Learn()
        {
            ConcurrentDictionary<ArgumentValue, double> TotalSlope = new ConcurrentDictionary<ArgumentValue, double>();
            foreach(var outputnodeFormulas in OutputFormulas)
            {
                Parallel.ForEach(outputnodeFormulas.Item3, PartialDerivative =>
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
                });
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
