using System;
using System.Collections.Generic;
using System.Linq;

/*
 *  matrix solver 
 * 
 * I devided the work into three main classes below Main(Panel, GameBoard and GameSolver Class)
 * 
 * */

namespace ConsoleApp1
{
    class Program
    {


        static void Main()
        {

            int[,] input = new int[,]
            {
                    {-2, -2, -2, -2, -2, -2, -2, -2, -2},
                    { 1,  1, -2, -2, -2, -2, -2, -2, -2},
                    { 0,  1, -2, -2,  1,  1,  2,  2, -2},
                    { 0,  1, -2, -2,  1,  0,  1,  1, -2},
                    { 0,  2,  4, -2,  2,  0,  1, -1,  1},
                    { 0,  1, -1, -1,  2,  0,  1,  1,  1},
                    { 0,  1,  3, -1,  2,  0,  0,  0,  0},
                    { 0,  0,  1,  1,  1,  0,  1,  1,  1},
                    { 0,  0,  0,  0,  0,  0,  1, -1, -2}
                };

            
            
            GameSolver solver = new GameSolver(input.GetLength(0), input.GetLength(1), input);
            solver.Solve();

            List<Panel> flags = new List<Panel>();
            flags = solver.GetFlags();
           

            List<Panel> will_reveal = new List<Panel>();
            will_reveal = solver.GetRevealed();


            //outputting result to console
            Console.WriteLine("panels that will be flagged:");
            foreach(var flag in flags)
            {
                Console.WriteLine("(" + flag.X +", "+ flag.Y + ")");
            }
            Console.WriteLine("panels that will be revealed:");
            foreach(var reveal in will_reveal)
            {
                Console.WriteLine("(" + reveal.X + ", " + reveal.Y + ")");
            }


        }


        //class represents a single square in the board i call them panels
        public class Panel
        {
            public int X { get; set; }
            public int Y { get; set; }
    
            public int AdjacentMines { get; set; }
            public bool IsRevealed { get; set; }
            public bool IsFlagged { get; set; }

            public Panel(int x, int y)
            {
                X = x;
                Y = y;
                IsRevealed = false;
                IsFlagged = false;
            }

        }


        //class represents the whole game board
        public class GameBoard
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public List<Panel> Panels { get; set; }


            public GameBoard(int width, int height, int[,] arr)
            {
                Width = width;
                Height = height;
                Panels = new List<Panel>();

                //fill the game panels with the input array
                for (int i = 0; i < height; i++) 
                {
                    for (int j = 0; j < width; j++)
                    {
                        Panel p = new Panel(j, i);
                        if (arr[i, j] >= 0 && arr[i, j] <= 7)
                        {
                            p.IsRevealed = true;
                            p.AdjacentMines = arr[i, j];
                        }
                        else if (arr[i, j] == -1)
                        {
                            p.IsFlagged = true;
                        }
                        Panels.Add(p);
                    }
                }
               
                
            }


            //used this method for debugging and testing 
            public void ShowPanels(List<Panel> panels)
            {
                int t = 1;
                foreach (var p in panels)
                {
                    if (p.IsRevealed)
                    {
                        Console.Write("{0}, ", p.AdjacentMines);
                    }
                    else if (p.IsFlagged)
                    {
                        Console.Write("-1, ");
                    }
                    else
                    {
                        Console.Write("-2, ");
                    }
                    if (t == Width)
                    {
                        Console.WriteLine("");
                        t = 0;
                    }
                    t++;
                }
            }


            //get all the neighbor panels with a default depth of 1 (the 8 immediate neighbors)
            public List<Panel> GetNeighbors(int x, int y, int depth = 1)
            {
                var nearbyPanels = Panels.Where(panel => panel.X >= (x - depth) && panel.X <= (x + depth)
                                                     && panel.Y >= (y - depth) && panel.Y <= (y + depth));
                var currentPanel = Panels.Where(panel => panel.X == x && panel.Y == y);
                return nearbyPanels.Except(currentPanel).ToList();
            }

            
            public void RevealPanel(int x, int y)
            {
                var selectedPanel = Panels.First(panel => panel.X == x && panel.Y == y);
                selectedPanel.IsRevealed = true;
                selectedPanel.IsFlagged = false; //Revealed panels cannot be flagged
            }

            public void FlagPanel(int x, int y)
            {
                var panel = Panels.Where(z => z.X == x && z.Y == y).First();
                if (!panel.IsRevealed)
                {
                    panel.IsFlagged = true;
                }
            }

        }



        //class represents the methods for solver
        public class GameSolver
        {
            public GameBoard InputBoard { get; set; }
            public GameBoard OutputBoard { get; set; }

            public GameSolver(int height, int width, int[,] arr)
            {
                //to compare between the input and the output panels
                //we will make changes to the OutputBoard panels and compare it to the Inputboard panels
                InputBoard = new GameBoard(width, height, arr);
                OutputBoard = new GameBoard(width, height, arr);
            }

            //flag the panels that are needed to be falgged
            public void FlagObviousMines()
            {
                var numberPanels = InputBoard.Panels.Where(x => x.IsRevealed && x.AdjacentMines > 0);
                foreach (var panel in numberPanels)
                {
                    //For each revealed number panel on the board, get its neighbors.
                    var neighborPanels = InputBoard.GetNeighbors(panel.X, panel.Y);

                    //If the total number of hidden panels == the number of mines revealed by this panel...
                    if (neighborPanels.Count(x => !x.IsRevealed) == panel.AdjacentMines)
                    {
                        //All those adjacent hidden panels must be mines, so flag them.
                        foreach (var neighbor in neighborPanels.Where(x => !x.IsRevealed))
                        {
                            OutputBoard.FlagPanel(neighbor.X, neighbor.Y);
                        }
                    }
                }
            }

            //reveal the panels that are needed to be revealed
            public void ObviousNumbers()
            {
                var numberedPanels = InputBoard.Panels.Where(x => x.IsRevealed && x.AdjacentMines > 0);
                foreach (var numberPanel in numberedPanels)
                {
                    //Foreach number panel
                    var neighborPanels = InputBoard.GetNeighbors(numberPanel.X, numberPanel.Y);

                    //Get all of that panel's flagged neighbors
                    var flaggedNeighbors = neighborPanels.Where(x => x.IsFlagged);

                    //If the number of flagged neighbors equals the number in the current panel...
                    if (flaggedNeighbors.Count() == numberPanel.AdjacentMines)
                    {
                        //All hidden neighbors must *not* have mines in them, so reveal them.
                        foreach (var hiddenPanel in neighborPanels.Where(x => !x.IsRevealed && !x.IsFlagged))
                        {
                            OutputBoard.RevealPanel(hiddenPanel.X, hiddenPanel.Y);
                        }
                    }
                }
            }

            //applay the flag and reveal methods and check if there is no solve 
            public void Solve()
            {

             FlagObviousMines();
             ObviousNumbers();

                // check if there is no solve
                int change_counter = 0;
                for (int i = 0; i < OutputBoard.Height * OutputBoard.Width; i++)
                {
                    
                    if  (OutputBoard.Panels[i].IsRevealed == InputBoard.Panels[i].IsRevealed && OutputBoard.Panels[i].IsFlagged == InputBoard.Panels[i].IsFlagged)
                    {
                        change_counter++;
                    }

                }
                if (change_counter == (OutputBoard.Height * OutputBoard.Width))
                {
                    Console.WriteLine("There is no solve");
                }


            }

            //return a list of the new falgged panels 
            public List<Panel> GetFlags()
            {
               
                List<Panel> result = new List<Panel>();
               for(int i = 0; i < OutputBoard.Height * OutputBoard.Width; i++)
                {
                    if(OutputBoard.Panels[i].IsFlagged && !InputBoard.Panels[i].IsFlagged)
                    {
                        result.Add(OutputBoard.Panels[i]);
                    }
                }
                return result;
            }


            //return a list of the new revealed panels
            public List<Panel> GetRevealed()
            {
                List<Panel> result = new List<Panel>();
                for (int i = 0; i < OutputBoard.Height * OutputBoard.Width; i++)
                {
                    if (OutputBoard.Panels[i].IsRevealed && !InputBoard.Panels[i].IsRevealed)
                    {
                        result.Add(OutputBoard.Panels[i]);
                    }
                }
                return result;
            }
        }

    }
    

}