namespace BoyAfraidOfGhosts.Models
{
    public class PathfindingNode
    {
        public int GridX { get; set; }
        public int GridY { get; set; }
        public double GCost { get; set; }
        public double HCost { get; set; }
        public PathfindingNode Parent { get; set; }
        public double FCost => GCost + HCost;

        public PathfindingNode(int x, int y)
        {
            GridX = x;
            GridY = y;
            GCost = 0;
            HCost = 0;
            Parent = null;
        }
    }
}