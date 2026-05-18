using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public class PlayerModel
    {
        public Vector2D Position { get; set; }
        public float FlashCooldown { get; set; }
        public double Direction { get; set; }
        public int FacingDirection { get; set; }

        public PlayerModel()
        {
            Position = new Vector2D(0, 0);
            FlashCooldown = 0;
            Direction = 0;
            FacingDirection = 1;
        }
    }
}
