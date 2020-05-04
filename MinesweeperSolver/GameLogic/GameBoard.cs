using System;
using System.Collections.Generic;
using System.Linq;

namespace MinesweeperSolver
{
    //class represents the whole game board
    public class GameBoard
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Panel> Panels { get; set; }
        public bool solvedanything { get; set; }

        public GameBoard(int width, int height, int[,] arr)
        {
            solvedanything = false;
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

            solvedanything = true;
        }

        public void FlagPanel(int x, int y)
        {
            var panel = Panels.Where(z => z.X == x && z.Y == y).First();
            if (!panel.IsRevealed)
            {
                panel.IsFlagged = true;
                solvedanything = true;
            }

        }

    }


}
