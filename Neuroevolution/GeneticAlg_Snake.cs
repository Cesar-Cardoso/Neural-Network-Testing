using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroevolution
{
    public class GeneticAlg_Snake
    {
        private static readonly Random getrandom = new Random();
        int generation;
        NeuralNetwork[] bots;
        NeuralNetwork[] procreatePool;
        public NeuralNetwork bestBot;
        private int totalFitness;
        private int totalPowFitness;
        static int TEST_CASES = 2;
        private static int MAX_FITNESS = 100;
        private static int POWER = 2;

        public GeneticAlg_Snake(int population, int inputs, int hidden, int outputs)
        {
            generation = 0;
            bots = new NeuralNetwork[population];
            for (int index = 0; index < population; index++)
            {
                bots[index] = new NeuralNetwork(inputs, hidden, outputs);
            }

            procreatePool = new NeuralNetwork[population];

            bestBot = bots[0];
        }

        public void AdvanceGeneration()
        {
            generation++;

            Console.WriteLine("Moving to generation " + generation + "!");
            Console.WriteLine();

            RunSimulation();

            for (int i = 0; i < bots.Length; i++)
            {
                NeuralNetwork[] parents = new NeuralNetwork[2];
                parents[0] = GetParent();
                parents[1] = GetParent();
                procreatePool[i] = parents[0].Procreate(parents[1]);
                procreatePool[i].Mutate(5,0.01);
            }

            Console.WriteLine("Best fitness value: " + (double)bestBot.fitness / TEST_CASES);

            Console.WriteLine("Average fitness value: " + (double)totalFitness / (bots.Length * TEST_CASES));

            procreatePool.CopyTo(bots, 0);
        }

        private NeuralNetwork GetParent()
        {
            int relFitness = 0, selection = getrandom.Next(0, totalPowFitness);
            for (int index = 0; index < bots.Length; index++)
            {
                relFitness += (int)Math.Pow(bots[index].fitness, POWER);
                if (selection <= relFitness)
                    return bots[index];
            }
            return bestBot;
        }

        private void RunSimulation()
        {
            totalFitness = 0;
            totalPowFitness = 0;
            
            GetFitness(bestBot, (generation % 30 == 0 && generation>=150));

            totalFitness += bestBot.fitness;
            totalPowFitness += (int)Math.Pow(bestBot.fitness, POWER);

            foreach (NeuralNetwork bot in bots)
            {
                GetFitness(bot);
                totalFitness += bot.fitness;
                totalPowFitness += (int)Math.Pow(bot.fitness, POWER);
                if (bot.fitness > bestBot.fitness)
                {
                    bestBot = bot.Clone();
                    bestBot.fitness = bot.fitness;
                }
            }
        }

        private void GetFitness(NeuralNetwork bot, bool visible = false)
        {
            bot.fitness = 0;
            
            for (int i = 0; i < TEST_CASES; i++)
            {
                /* modify depending on what you want to predict, this is all that changes from problem to problem
                 ------------------------------------------------------------------------------------------------*/
                SnakeGame game = new SnakeGame(10);
                int size = game.boardSize;
                while (game.gameInProgress)
                {
                    double[] inputs = new double[16];

                    int[][] directions = new int[8][];
                    int idx = 0;

                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            if (x != 0 || y != 0)
                                directions[idx++] = new int[] {x, y};
                        }
                    }

                    for (int j = 0; j < 8; j++)
                    {
                        int[] position = new int[2] {game.snake.headLocation.X, game.snake.headLocation.Y};

                        double distance = 1;

                        inputs[2 * j + 1] = 0;
                        bool foundSnake = false;

                        while (true)
                        {
                            position[0] += directions[j][0];
                            position[1] += directions[j][1];

                            if (position[0] == -1 || position[0] == size || position[1] == -1 || position[1] == size)
                            {
                                if (!foundSnake)
                                    inputs[2 * j] = 1.0 / (distance * distance);
                                break;
                            }

                            BoardElements current = game.board[position[0], position[1]];

                            switch (current)
                            {
                                //case BoardElements.Mine:
                                case BoardElements.Snake:
                                    if (!foundSnake)
                                    {
                                        inputs[2 * j] = 1.0 / (distance * distance);
                                        foundSnake = true;
                                    }
                                    break;

                                case BoardElements.Apple:
                                    inputs[2 * j + 1] = 1.0;
                                    break;
                            }
                            distance++;

                            if (foundSnake)
                                break;
                        }
                    }

                    bot.SetInputs(inputs);

                    double[] outputs = bot.Predict();

                    int index = 0;
                    double max = outputs[0];

                    for (int k = 0; k < 4; k++)
                    {
                        if (outputs[k] > max)
                        {
                            index = k;
                            max = outputs[k];
                        }
                    }

                    switch (index)
                    {
                        case 0:
                            game.changeDirection(-1, 0);
                            break;
                        case 1:
                            game.changeDirection(1, 0);
                            break;
                        case 2:
                            game.changeDirection(0, -1);
                            break;
                        case 3:
                            game.changeDirection(0, 1);
                            break;
                        default:
                            break;
                    }

                    if (visible)
                    {
                        Console.CursorVisible = false;
                        game.advance(true);
                        Thread.Sleep(200);
                    }
                    else
                        game.advance();
                }

                bot.fitness += (int) game.points;
            }

            /* stay inside this for loop 
            ------------------------------------------------------------------------------------------------*/
        }
    }
}
