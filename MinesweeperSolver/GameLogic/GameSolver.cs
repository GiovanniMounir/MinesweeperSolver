using System;
using System.Collections.Generic;
using System.Linq;

namespace MinesweeperSolver
{

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
                    foreach (var neighbor in neighborPanels.Where(x => !x.IsRevealed && !x.IsFlagged))
                    {
                        OutputBoard.FlagPanel(neighbor.X, neighbor.Y);
                    }
                }
            }
        }



        public void HardSolve()
        {

            var numberPanels = InputBoard.Panels.Where(x => x.IsRevealed && x.AdjacentMines > 0);
            //Making groups of unrevealed panels and the mines they contain
            HashSet<KeyValuePair<List<Panel>, int>> groups = new HashSet<KeyValuePair<List<Panel>, int>>();

            foreach (var panel in numberPanels)
            {
                //For each revealed number panel on the board, get its neighbors.
                var neighborPanels = InputBoard.GetNeighbors(panel.X, panel.Y);

                //If the total number of hidden panels == the number of mines revealed by this panel...
                int minesleft = panel.AdjacentMines - neighborPanels.Count(x => x.IsFlagged);
                if (panel.AdjacentMines > 0)
                {

                    groups.Add(new KeyValuePair<List<Panel>, int>(neighborPanels.Where(x => !x.IsRevealed && !x.IsFlagged).OrderBy(x => x.X).ToList(), minesleft));
                }

            }
            var grps = groups.ToList();

            for (int i = 0; i < groups.Count(); i++)
            {
                for (int j = i + 1; j < groups.Count(); j++)
                {

                    var grpintrsct = grps[i].Key.Intersect(grps[j].Key).ToList();

                    if (grpintrsct.Count() == 0) continue;
                    //if a group has same number of mines and it's subset has also same number of mines then difference list is to be revealed
                    if (grps[i].Value == grps[j].Value)
                    {
                        //if grps[i] is subset of grps[j]
                        if (grpintrsct.Count() == grps[i].Key.Count())
                        {
                            var grpdiff = grps[j].Key.Except(grps[i].Key);

                            foreach (var toberevealedpanel in grpdiff)
                            {
                                OutputBoard.RevealPanel(toberevealedpanel.X, toberevealedpanel.Y);

                            }
                        }
                        else if (grpintrsct.Count() == grps[j].Key.Count() && grps[j].Key.Count() < grps[i].Key.Count())
                        {
                            var grpdiff = grps[i].Key.Except(grps[j].Key);

                            foreach (var toberevealedpanel in grpdiff)
                            {
                                OutputBoard.RevealPanel(toberevealedpanel.X, toberevealedpanel.Y);

                            }
                        }




                    }
                    else
                    {

                        if (grpintrsct.Count() == grps[i].Key.Count())
                        {
                            if (grps[j].Key.Count() - grps[i].Key.Count() == grps[j].Value - grps[i].Value)
                            {
                                var grpdiff = grps[j].Key.Except(grps[i].Key);

                                foreach (var tobeflaggedpanel in grpdiff)
                                {
                                    OutputBoard.FlagPanel(tobeflaggedpanel.X, tobeflaggedpanel.Y);

                                }
                            }
                        }
                        else if (grpintrsct.Count() == grps[j].Key.Count())
                        {
                            if (grps[i].Key.Count() - grps[j].Key.Count() == grps[i].Value - grps[j].Value)
                            {
                                var grpdiff = grps[i].Key.Except(grps[j].Key);

                                foreach (var tobeflaggedpanel in grpdiff)
                                {
                                    OutputBoard.FlagPanel(tobeflaggedpanel.X, tobeflaggedpanel.Y);

                                }
                            }
                        }
                        else
                        {
                            if (grpintrsct.Count() == 1) continue;
                            if(grps[i].Value ==1)
                            {
                                if( (grps[j].Value -1) == (grps[j].Key.Count() - grpintrsct.Count()) )
                                {
                                    var grpdiff = grps[j].Key.Except(grpintrsct);

                                    foreach (var tobeflaggedpanel in grpdiff)
                                    {
                                        OutputBoard.FlagPanel(tobeflaggedpanel.X, tobeflaggedpanel.Y);

                                    }
                                }
                            }
                            else if (grps[j].Value == 1)
                            {
                                if ((grps[i].Value - 1) == (grps[i].Key.Count() - grpintrsct.Count()))
                                {
                                    var grpdiff = grps[i].Key.Except(grpintrsct);

                                    foreach (var tobeflaggedpanel in grpdiff)
                                    {
                                        OutputBoard.FlagPanel(tobeflaggedpanel.X, tobeflaggedpanel.Y);

                                    }
                                }
                            }
                        }

                    }






                }

            }
            return;
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
        public bool Solve()
        {

            FlagObviousMines();
            ObviousNumbers();
            if (!OutputBoard.solvedanything)
            {
                //System.Windows.Forms.MessageBox.Show("Hard Solving beginned");
                HardSolve();
            }
            return OutputBoard.solvedanything;

        }

        //return a list of the new falgged panels 
        public List<Panel> GetFlags()
        {

            List<Panel> result = new List<Panel>();
            for (int i = 0; i < OutputBoard.Height * OutputBoard.Width; i++)
            {
                if (OutputBoard.Panels[i].IsFlagged && !InputBoard.Panels[i].IsFlagged)
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
