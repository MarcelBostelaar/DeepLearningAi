using System;
using System.Collections.Generic;
using System.IO;
using Deeplearning2;


namespace testing
{
    static class Program
    {
        [STAThreadAttribute]
        static void Main()
        {
            NeuralNetwork net = new NeuralNetwork(new int[] { 10, 15, 10 }, ActivationFunction.LogisticFunction);
            Matrix input = new Matrix(1, 10);
            Matrix ideal = new Matrix(1, 10);
            net.Learn(input, ideal);
        }
    }
}
