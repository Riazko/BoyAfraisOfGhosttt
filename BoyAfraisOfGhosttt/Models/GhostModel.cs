using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public class GhostModel
    {
        public Vector2D Position { get; set; }
        public bool IsAlive { get; set; }

        public GhostModel(Vector2D vector)
        {
            Position = vector;
            IsAlive = true;
        }

    }
}