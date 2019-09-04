using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroevolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Select test:");
            Console.WriteLine("(1)Phrase prediction");
            Console.WriteLine("(2)Sum prediction");
            Console.WriteLine("(3)Snake Game");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    GetWord();
                    break;
                case "2":
                    AddNumbers();
                    break;
                case "3":
                    PlaySnake();
                    break;
            }
        }
        
        static void AddNumbers()
        {
            Console.WriteLine("Begin testing...");
            GeneticAlgorithm test = new GeneticAlgorithm(300, 3, 2, 2);
            for (int _ = 0; _ < 100; _++)
            {
                test.AdvanceGeneration();
            }

            NeuralNetwork candidate = test.bestBot;

            double tempInput = 0;
            double[] inputs = new double[3];
            int count = 0;

            while (tempInput != -0.01)
            {
                Console.WriteLine();
                Console.Write("Enter (-1) to exit");
                Console.WriteLine("Enter input #" + (count + 1) + ": ");
                tempInput = Convert.ToDouble(Console.ReadLine()) / 100;
                inputs[count] = tempInput;
                if (count == 2)
                {
                    count = -1;
                    candidate.SetInputs(inputs);
                    double[] outputs = candidate.Predict();
                    Console.WriteLine();
                    if (outputs[0] < outputs[1])
                    {
                        Console.WriteLine("Bot predicted your numbers add to more than 150!");
                    }
                    else
                    {
                        Console.WriteLine("Bot predicted your numbers do not add to more than 150!");
                    }
                }
                count++;
            }
        }

        static void PlaySnake()
        {
            GeneticAlg_Snake simulation = new GeneticAlg_Snake(250, 16, 10, 4);

            for (int _ = 0; _ < 500; _++)
            {
                simulation.AdvanceGeneration();
            }

            NeuralNetwork bot = simulation.bestBot;

            Console.WriteLine("Play?");
            Console.WriteLine("(Y)Yes");
            Console.WriteLine("(N)No");
            string input = Console.ReadLine();

            while (input.ToUpper() == "Y")
            {
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

                    Console.Clear();
                    game.advance(true);
                    Thread.Sleep(500);
                }

                Console.WriteLine("Play?");
                Console.WriteLine("(Y)Yes");
                Console.WriteLine("(N)No");
                input = Console.ReadLine();
            }
        }

        static void GetWord()
        {
            string target = "";

            while (target != "-1")
            {
                Console.WriteLine("Enter (-1) to exit");
                Console.WriteLine("Set target phrase: ");
                target = Console.ReadLine();
                GeneticAlg_Words test = new GeneticAlg_Words(1000, target.ToUpper());
                for (int i = 0; i < 100; i++)
                {
                    test.AdvanceGeneration();
                }

                Word bestWord = test.bestBot;

                Console.WriteLine();
                Console.WriteLine(bestWord.word);
            }
        }
    }
}
