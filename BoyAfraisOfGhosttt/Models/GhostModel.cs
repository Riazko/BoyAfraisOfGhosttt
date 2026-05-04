using System;

namespace BoyAfraisOfGhosttt.Models
{
    public class Ghost
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsAlive { get; set; } = true;
        public int TeleportTimer { get; set; }
        private Random rand = new Random();
        private int worldWidth = 2000;
        private int worldHeight = 2000;
        
        public Ghost(int startX, int startY)
        {
            X = startX;
            Y = startY;
            TeleportTimer = rand.Next(60, 180);
        }
        
        public void Update()
        {
            if (TeleportTimer > 0)
                TeleportTimer--;
        }
        
        public bool CanTeleport() => TeleportTimer <= 0;
        
        public void Teleport(int targetX, int targetY)
        {
            X = targetX + rand.Next(-150, 150);
            Y = targetY + rand.Next(-150, 150);
            
            if (X < 50) X = 50;
            if (X > worldWidth - 50) X = worldWidth - 50;
            if (Y < 50) Y = 50;
            if (Y > worldHeight - 50) Y = worldHeight - 50;
            
            TeleportTimer = 180;
        }
    }
}