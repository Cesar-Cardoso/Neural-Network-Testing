using System;

namespace Neuroevolution
{
    public class NeuralNetwork
    {
        public int fitness; //determine how well the agent is performing
        private static readonly Random getrandom = new Random();
        double[] input, hidden, output;
        double[][,] weights;
        private double[][] biases;
        private bool sigmoid;

        public NeuralNetwork(int input, int hidden, int output, bool sigmoid = false)
        {
            this.input = new double[input];
            this.hidden = new double[hidden];
            this.output = new double[output];

            double[,] inputWeights = new double[input, hidden];

            for (int i = 0; i < input; i++)
            {
                for (int j = 0; j < hidden; j++)
                {
                    double range = Math.Sqrt(2 / (double) input);
                    inputWeights[i,j] = sigmoid ? GetRandomNormal() :GetRandomNormal() * range;
                }
            }

            double[] inputBiases = new double[hidden];
            
            for (int i = 0; i < hidden; i++)
            {
                //inputBiases[i] = 0;
                inputBiases[i] = GetRandomNormal();
            }

            double[,] outputWeights = new double[hidden, output];

            for (int i = 0; i < hidden; i++)
            {
                for (int j = 0; j < output; j++)
                {
                    double range = Math.Sqrt((2 / (double) hidden));
                    outputWeights[i, j] = sigmoid ? GetRandomNormal() :GetRandomNormal() * range;
                }
            }

            double[] outputBiases = new double[output];

            for (int i = 0; i < output; i++)
            {
                //outputBiases[i] = 0;
                outputBiases[i] = GetRandomNormal();
            }

            weights = new double[2][,] { inputWeights, outputWeights };
            biases = new double[2][] {inputBiases, outputBiases};
            this.sigmoid = sigmoid;
        }

        public NeuralNetwork Clone()
        {
            NeuralNetwork son = new NeuralNetwork(input.Length, hidden.Length, output.Length);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < weights[i].GetLength(0); j++)
                {
                    for (int z = 0; z < weights[i].GetLength(1); z++)
                    {
                        son.weights[i][j, z] = weights[i][j, z];
                    }
                }

                for (int j = 0; j < biases[i].Length; j++)
                {
                    son.biases[i][j] = biases[i][j];
                }
            }
            return son;
        }

        public NeuralNetwork Procreate(NeuralNetwork parent)
        {
            NeuralNetwork son = new NeuralNetwork(input.Length, hidden.Length, output.Length);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < weights[i].GetLength(0); j++)
                {
                    for (int z = 0; z < weights[i].GetLength(1); z++)
                    {
                        if (getrandom.NextDouble() > 0.5)
                            son.weights[i][j, z] = weights[i][j, z];
                        else
                            son.weights[i][j, z] = parent.weights[i][j, z];
                    }
                }

                for (int j = 0; j < biases[i].Length; j++)
                {
                    if (getrandom.NextDouble() > 0.5)
                        son.biases[i][j] = biases[i][j];
                    else
                        son.biases[i][j] = parent.biases[i][j];
                }
            }
            return son;
        }

        public void SetInputs(double[] input)
        {
            for (int index = 0; index < input.Length; index++)
            {
                this.input[index] = input[index];
            }
        }

        public double[] Predict()
        {
            Func<double, double> activation;

            if (sigmoid)
                activation = Sigmoid;
            
            else
                activation = LReLU;
            
            hidden = CalculateLayer(input, weights[0], biases[0], activation);

            //output = Softmax(hidden, weights[1], biases[1]);

            output = CalculateLayer(hidden, weights[1], biases[1], activation);
            
            return output;
        }

        public void Mutate(double percentage = 1, double chance = 0.01)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < weights[i].GetLength(0); j++)
                {
                    for (int z = 0; z < weights[i].GetLength(1); z++)
                    {
                        if (GetRandomNumber(0,1) < chance)
                        {
                            double temp = weights[i][j, z];
                            double range = Math.Sqrt(2 / (double) weights[i].GetLength(0));
                            weights[i][j, z] += sigmoid ? GetRandomNormal() * percentage :GetRandomNormal() * range * percentage;
                        }
                    }
                }

                for (int j = 0; j < biases[i].Length; j++)
                {
                    if (GetRandomNumber(0,1) < chance)
                    {
                        double temp = biases[i][j];
                        biases[i][j] += GetRandomNormal() * percentage;
                    }
                }
            }
        }

        private static double GetRandomNumber(double min, double max)
        {
            lock (getrandom)
            {
                return (getrandom.NextDouble() * (max - min) + min);
            }
        }

        public double GetRandomNormal()
        {
            double x, y, s = 2;
            do
            {
                x = GetRandomNumber(-1,1);
                y = GetRandomNumber(-1,1);

                s = x * x + y * y;
            } while (s >= 1);

            return x * Math.Sqrt(-2 * Math.Log(s) / s);
        }

        private double[] CalculateLayer(double[] input, double[,] weights, double[] biases, Func<double, double> activation)
        {
            int inputSize = input.Length, outputSize = weights.GetLength(1);
            double[] output = new double[outputSize];

            for (int i = 0; i < outputSize; i++)
            {
                output[i] = 0;
                for (int j = 0; j < inputSize; j++)
                {
                    output[i] += input[j] * weights[j,i]; // note that nested for loops are in reversed order
                }

                output[i] += biases[i];
                output[i] = activation(output[i]);
            }

            return output;
        }
        private double Sigmoid(double value)
        {
            return (Math.Exp(value) / (Math.Exp(value) + 1));
        }

        private double LReLU(double value)
        {
            return value > 0 ? value : 0.1 * value;
        }

        private double[] Softmax(double[] input, double[,] weights, double[] biases)
        {
            int inputSize = input.Length, outputSize = weights.GetLength(1);
            double[] output = new double[outputSize];
            double total = 0;

            for (int i = 0; i < outputSize; i++)
            {
                output[i] = 0;
                for (int j = 0; j < inputSize; j++)
                {
                    output[i] += input[j] * weights[j,i]; // note that nested for loops are in reversed order
                }

                output[i] += biases[i];
                output[i] = Math.Exp(output[i]);
                total += output[i];
            }

            for (int i = 0; i < outputSize; i++)
            {
                output[i] /= total;
            }
            
            return output;
        }
    }
}