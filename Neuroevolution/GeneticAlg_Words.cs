using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroevolution
{
    public class Word
    {
        private static readonly Random getrandom = new Random();
        public int fitness = 0;
        public int length;
        public char[] word;

        public Word(int length)
        {
            this.length = length;
            word = RandomString(length);
            for (int i = 0; i < word.Length; i++)
            {
                if (getrandom.NextDouble() < 1.0 / 27)
                    word[i] = ' ';
            }
        }

        public Word Clone()
        {
            Word newWord = new Word(length);

            for (int i = 0; i < length; i++)
            {
                newWord.word[i] = word[i];
            }

            return newWord;
        }

        public Word Procreate(Word parent)
        {
            Word newWord = new Word(length);

            for (int i = 0; i < length; i++)
            {
                if (getrandom.NextDouble() > 0.5)
                {
                    newWord.word[i] = word[i];
                }
                else
                {
                    newWord.word[i] = parent.word[i];
                }
            }

            return newWord;
        }

        public void Mutate(double chance = 0.01)
        {
            for (int i = 0; i < length; i++)
            {
                if (getrandom.NextDouble() < chance)
                {
                    if (getrandom.NextDouble() < 1.0 / 27)
                        word[i] = ' ';
                    else 
                        word[i] = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * getrandom.NextDouble() + 65)));
                }
            }
        }
        
        public char[] RandomString(int size)
        {
            char[] result = new char[size];
            char ch;  
            for (int i = 0; i < size; i++)  
            {  
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * getrandom.NextDouble() + 65)));  
                result[i] = ch;  
            }

            return result;
        }  
    }
    
    
    public class GeneticAlg_Words
    {
        private static readonly Random getrandom = new Random();
        int generation;
        Word[] bots;
        Word[] procreatePool;
        public Word bestBot;
        public string target;

        public GeneticAlg_Words(int population, string target)
        {
            generation = 0;
            bots = new Word[population];
            for (int index = 0; index < population; index++)
            {
                bots[index] = new Word(target.Length);
            }
            
            procreatePool = new Word[population];

            bestBot = bots[0];

            this.target = target;
        }

        public void AdvanceGeneration()
        {
            generation++;
            
            Console.WriteLine("Moving to generation " + generation + "!");
            Console.WriteLine();

            int totalFitness = RunSimulation();

            for (int i = 0; i < bots.Length; i++)
            {
                Word[] parents = new Word[2];
                parents[0] = GetParent(totalFitness);
                parents[1] = GetParent(totalFitness);
                procreatePool[i] = parents[0].Procreate(parents[1]);
                procreatePool[i].Mutate();
            }

            Console.WriteLine("Best fitness value: " + bestBot.fitness);

            Console.WriteLine("Average fitness value: " + totalFitness / bots.Length);

            procreatePool.CopyTo(bots, 0);
        }

        private Word GetParent(int totalFitness)
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
            
            foreach (Word bot in bots)
            {
                GetFitness(bot);
                totalFitness += bot.fitness;
                if (bot.fitness > bestBot.fitness)
                    bestBot = bot;
            }

            return totalFitness;
        }
        
        private void GetFitness(Word bot)
        {
            /* modify depending on what you want to predict, this is all that changes from problem to problem
             ------------------------------------------------------------------------------------------------*/
            for (int i = 0; i < bot.length; i++)
            {
                if (bot.word[i] == target[i])
                    bot.fitness++;
            }
            /* stay inside this for loop
             ------------------------------------------------------------------------------------------------*/
        }
    }
}
