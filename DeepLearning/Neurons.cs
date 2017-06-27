
using DeepLearning.MathSyntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearning
{
    class Neuron
    {
        public Neuron()
        {
            Out = new List<NeuronConnection>();
            In = new List<NeuronConnection>();
        }

        public void LinkTo(Neuron TargetNeuron, ArgumentValue Value)
        {
            var connection = new NeuronConnection(this, TargetNeuron, Value);
            Out.Add(connection);
            TargetNeuron.In.Add(connection);
        }

        public virtual SyntaxBlock BuildEquation()
        {
            SyntaxBlock Sum = new NumericConstant(0);
            foreach (var incoming in In)
            {
                Sum = new Sum(Sum, incoming.BuildEquation());
            }
            return Sum;
        }

        public List<NeuronConnection> Out, In;
    }

    class InputNeuron : Neuron
    {
        public override SyntaxBlock BuildEquation()
        {
            return new VariableConstant(Value);
        }

        public InputNeuron(ArgumentValue Value)
        {
            this.Value = Value;
        }
        public ArgumentValue Value { get; private set; }
    }

    class OutputNeuron : Neuron
    {
        public OutputData Value { get; private set; }
        public OutputNeuron(OutputData Value)
        {
            this.Value = Value;
        }
    }

    class NeuronConnection
    {
        public NeuronConnection(Neuron From, Neuron To, ArgumentValue value)
        {
            this.From = From;
            this.To = To;
            this.value = value;
        }
        public ArgumentValue value { get; private set; }
        public Neuron From, To;


        public SyntaxBlock BuildEquation()
        {
            return new Product(new Variable(value), From.BuildEquation());
        }
    }

}
