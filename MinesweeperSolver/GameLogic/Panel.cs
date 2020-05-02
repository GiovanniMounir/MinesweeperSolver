namespace MinesweeperSolver
{
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
}
