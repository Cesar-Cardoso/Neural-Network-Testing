# Neural-Network-Testing

This is my implementation of a neural network structure. The agents learn through a neuro-evolution algorithm that I created.

How the algorithm works:
An initial population of agents with random parameters is created, then for each generation:

->the agents' fitness(how good they are at performing the task at hand) is calculated for the entire population

->new agents are created, inheriting the properties of two other agents from the previous generation. The "parents", are determined based on how high their fitness value is (high fitness = high chance of having "children"). Each new agent created has a probability of mutating (randomly altering the value) some of the inherited parameters, this helps introduce variation.

->the same process is repeated with the new generation


Understanding the numbers:
After each generation, the following will print:

Best fitness value: #    this is the fitness value for the best performing agent in this generation

Average fitness value: # this is the average of all fitness values in this generation

Moving to generation #!  this is the new generation number


There are three simple tests that show what the neural network is capable of:

(1)Phrase prediction:

The user types a phrase. A population of random phrases with the same size will be created. The fitness of each phrase is assigned based on how close they are to the original. After a few generations, almost all phrases in the population should be equal to the target phrase (never all of them because of the mutations). *this is the only test that doesn't use neural networks, as it only tests the neuro-evolution algorithm


(2)Sum prediction:

The algorithm will run first, training the agents. Afterwards, the user types in 3 numbers, and the agent will predict if the sum of these numbers is greater than 150. The fitness of the agents is determined by whether they predicted correctly or not. Keep in mind that the agent is NOT adding these numbers, it is predicting solely based on the experience of previous generations.


(3)Snake Game:

The algorithm will run first, training the agents. After generation 150, the agents can play the game decently well. You'll be able to see the best agent playing every 30 generations. The fitness of the agent is determined by how big it got before dying. The agents only "see" the closest object in 8 directions around them (as opposed to us that can see the whole map), therefore even after many generations they are not quite capable of completely winning the game. These agents show signs of complex decision patterns and strategy.
