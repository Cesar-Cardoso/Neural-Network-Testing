using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroevolution
{
    public class GeneticAlgorithm
    {
        private static readonly Random getrandom = new Random();
        int generation;
        NeuralNetwork[] bots;
        NeuralNetwork[] nextGeneration;
        public NeuralNetwork bestBot;
        static int TEST_CASES = 1000;

        public GeneticAlgorithm(int population, int inputs, int hidden, int outputs)
        {
            generation = 0;
            bots = new NeuralNetwork[population];
            for (int index = 0; index < population; index++)
            {
                bots[index] = new NeuralNetwork(inputs, hidden, outputs);
            }
            
            nextGeneration = new NeuralNetwork[population];

            bestBot = bots[0];
        }

        public void AdvanceGeneration()
        {
            generation++;
            
            Console.WriteLine("Moving to generation " + generation + "!");
            Console.WriteLine();

            int totalFitness = RunSimulation();

            for (int i = 0; i < bots.Length; i++)
            {
                NeuralNetwork[] parents = new NeuralNetwork[2];
                parents[0] = GetParent(totalFitness);
                parents[1] = GetParent(totalFitness);
                nextGeneration[i] = parents[0].Procreate(parents[1]);
                nextGeneration[i].Mutate();
            }

            Console.WriteLine("Best fitness value: " + bestBot.fitness);

            Console.WriteLine("Average fitness value: " + totalFitness / bots.Length);

            nextGeneration.CopyTo(bots, 0);
        }

        private NeuralNetwork GetParent(int totalFitness)
        {
            int relFitness = 0, selection = getrandom.Next(0, totalFitness);
            for (int index = 0; index < bots.Length; index++)
            {
                relFitness += bots[index].fitness;
                if (selection <= relFitness)
                    return bots[index];
            }

            Console.WriteLine("Not finding anything, check this out!");
            Console.WriteLine("Relative Fitness = " + relFitness);
            Console.WriteLine("Selection = " + selection);
            Thread.Sleep(1000);
            return GetParent(totalFitness);
        }

        private int RunSimulation()
        {
            int totalFitness = 0;
            
            foreach (NeuralNetwork bot in bots)
            {
                GetFitness(bot);
                totalFitness += bot.fitness;
                if (bot.fitness > bestBot.fitness)
                    bestBot = bot;
            }

            return totalFitness;
        }
        
        private void GetFitness(NeuralNetwork bot)
        {
            double[] inputs = new double[3];

            double sum = 0;
            
            for (int i = 0; i < TEST_CASES; i++)
            {
                /* modify depending on what you want to predict, this is all that changes from problem to problem
                 ------------------------------------------------------------------------------------------------*/
                for (int j = 0; j < 3; j++)
                {
                    inputs[j] = getrandom.NextDouble();
                    sum += inputs[j];
                }
                
                bot.SetInputs(inputs);
                double[] outputs = bot.Predict();

                if ((outputs[1] > outputs[0] && sum > 1.5) || (outputs[1] < outputs[0] && sum < 1.5))
                    bot.fitness += 1;

                sum = 0;
                
                /* stay inside this for loop 
                ------------------------------------------------------------------------------------------------*/
            }
        }
    }
}
