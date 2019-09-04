using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuroevolution
{
    public class SnakeGame
    {
        public NeuralNetwork brain;
        
        public BoardElements[,] board;
        public Snake snake;
        public Point appleLocation;
        public int boardSize;
        public bool gameInProgress;
        public int xDirection;
        public int yDirection;

        public ulong ticks;
        public ulong points;
        
        public SnakeGame(int boardSize = 10)
        {
            snake = new Snake(new Point(boardSize/2,boardSize/2));
            xDirection = 1;
            yDirection = 0;
            this.boardSize = boardSize;
            board = new BoardElements[boardSize, boardSize];
            gameInProgress = true;
            ticks = 0;
            points = 0;
            
            board[snake.headLocation.X, snake.headLocation.Y] = BoardElements.Snake;
            //3 mines randomly placed
            //generateMine();
            //generateMine();
            //generateMine();
            generateApple();
            //printBoard();
        }

        public void changeDirection(int xDirection, int yDirection)
        {
            if (this.xDirection == -xDirection && this.yDirection == -yDirection)
                return;
            
            this.xDirection = xDirection;
            this.yDirection = yDirection;
        }

        public void advance(bool print = false)
        {
            
            if (!gameInProgress) return;
            Point headMovement = new Point(snake.headLocation.X + xDirection, snake.headLocation.Y + yDirection);
            if (checkHit(headMovement))
            {
                hitEvent(headMovement);
                if (!gameInProgress) return;
            }

            Point p = snake.move(headMovement);
            board[p.X, p.Y] = BoardElements.Empty;
            board[headMovement.X, headMovement.Y] = BoardElements.Snake;
            ticks++;
            if ((int)ticks == boardSize * boardSize + 1)
            {
                gameInProgress = false;
            }
            if (print)
                printBoard();
        }
        

        #region Generators

        public void generateApple()
        {
            appleLocation = generateElement(false);
        }
        
        public void generateMine()
        {
            generateElement(true);
        }
        
        public Point generateElement(bool mineORapple)
        {
            List<Point> pointList = new List<Point>();
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if(board[i,j] == BoardElements.Empty)
                        pointList.Add(new Point(i, j));
                }
            }

            int r = new Random().Next(pointList.Count);
            board[pointList[r].X, pointList[r].Y] = (mineORapple) ? BoardElements.Mine : BoardElements.Apple;
            return pointList[r];
        }
        
        
        #endregion

        #region Hit

        public bool checkHit(Point p)
        {
            if (!validPoint(p)) return true;
            if (board[p.X, p.Y] != BoardElements.Empty) return true;
            return false;
        }
        
        public void hitEvent(Point p)
        {
            if (!validPoint(p)) gameInProgress = false;
            else if (board[p.X, p.Y] == BoardElements.Apple)
            {
                snake.increaseSize();
                points++;
                ticks = 0;
                generateApple();
                //generateMine();
            }
            else gameInProgress = false;
        }

        #endregion

        
        public void printBoard()
        {
            Console.Clear();
            string buffer = "";
            for (int i = 0; i < boardSize; i++)
            {
                buffer = "";
                for (int j = 0; j < boardSize; j++)
                {
                    if(true)
                    {
                        switch (board[i, j])
                        {
                            case BoardElements.Apple:
                                buffer += "\tO";
                                break;
                            case BoardElements.Empty:
                                buffer += "\t.";
                                break;
                            case BoardElements.Mine:
                                buffer += "\tX";
                                break;
                            case BoardElements.Snake:
                                buffer += "\t■";
                                break;
                        }
                    }
                }
                Console.WriteLine(buffer + Environment.NewLine);
            }
        }

        public bool validPoint(Point p)
        {
            return (p.X >= 0 && p.Y >= 0 && p.Y < boardSize && p.X < boardSize);
        }

    }

    public enum BoardElements
    {
        Empty, Apple, Mine, Snake
    }

    public class Snake
    {
        public int length;
        public List<Point> body;
        public Point headLocation;
        public Snake(Point headLocation)
        {
            length = 1;
            body = new List<Point>();
            body.Add(headLocation);
            this.headLocation = headLocation;
        }

        public void increaseSize()
        {
            length++;
            body.Insert(0,body.ElementAt(0));
        }

        public Point move(Point p)
        {
            headLocation = p;
            body.Add(p);
            Point result = body[0];
            body.RemoveAt(0);
            return result;
        }
    }

    public class Point
    {
        public int X, Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}