using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deeplearning2
{
    public class NeuralNetwork
    {
        Func<double, double> activationFunction;
        Func<double, double> activationDerivative;
        int SizeInputVector, SizeOutputVector;
        Matrix[] matrices;
        public NeuralNetwork(IEnumerable<int> layerSizes, ActivationFunction activationFunction)
        {
            switch (activationFunction)
            {
                case ActivationFunction.LogisticFunction:
                    this.activationFunction = x =>
                    {
                        return 1 / (1 + Math.Pow(Math.E, -x));
                    };
                    this.activationDerivative = x =>
                    {
                        return x * (1 - x);
                    };
                    break;
                default:
                    throw new NotImplementedException();
            }

            SizeInputVector = layerSizes.First();
            SizeOutputVector = layerSizes.Last();
            matrices = new Matrix[layerSizes.Count()-1];
            for (int i = 0; i < matrices.Length; i++)
            {
                matrices[i] = new Matrix(layerSizes.ElementAt(i), layerSizes.ElementAt(i + 1));
            }
        }

        private void CalculateAllDerivatives(Matrix inputVector, Matrix idealVector)
        {
            Matrix[] inputvectors = new Matrix[matrices.Length + 2];
            inputvectors[0] = inputVector;

            for (int matrixindex = 0; matrixindex < matrices.Length; matrixindex++)
            {
                inputvectors[matrixindex + 1] = Matrix.ApplyFunc(
                        matrices[matrixindex].Calculate(inputvectors[matrixindex]), activationFunction);
            }

            //not needed?
            //double E_total = Matrix.SubtractVectors(inputvectors.Last(), idealVector); //absolute squared or something?

            Matrix[] smallDelta = new Matrix[matrices.Length];

            smallDelta[smallDelta.Length - 1] = Matrix.HadamardMultiply((inputvectors[inputvectors.Length - 1] - idealVector), Matrix.ApplyFunc(matrices[matrices.Length-1].Calculate(inputvectors[inputvectors.Length-2]), activationDerivative));
            for (int i = smallDelta.Length-2; i >= 0; i++)
            {
                Matrix.HadamardMultiply(
                    matrices[i+1].Transpose().Calculate(smallDelta[i+1]), 
                    matrices[i].Calculate(inputvectors[i-1-1])
                    );
            }

            Matrix[] gradients = new Matrix[matrices.Length];
            for (int i = 0; i < gradients.Length; i++)
            {
                gradients[i] = smallDelta[i].Multiply(inputvectors[i].Transpose());
            }
        }


        public void Learn(Matrix inputVector, Matrix idealVector)
        {
            if (inputVector.height != SizeInputVector || idealVector.height != SizeOutputVector)
                throw new Exception("Vectors do not match");
            CalculateAllDerivatives(inputVector, idealVector);
        }
    }
}
