using System.Collections.Generic;
using BoyAfraidOfGhosts.Helpers;
using System;

namespace BoyAfraidOfGhosts.Models
{
    public class GameModel
    {
        public PlayerModel Player { get; set; }
        public List<GhostModel> Ghosts { get; set; }
        public bool IsGameOver { get; set; }

        public GameModel()
        {
            Player = new PlayerModel();
            Ghosts = new List<GhostModel>();
            CreateStartGhosts(Ghosts, Player);
            IsGameOver = false;
        }

        public void CreateStartGhosts(List<GhostModel> ghosts, PlayerModel player)
        {
            var rand = new Random();
            for (int i = 0; i < 3; i++)
            {
                var angle = rand.NextDouble() * Math.PI * 2;
                var distance = rand.Next(150, 350);
                var x = player.Position.X + Math.Cos(angle) * distance;
                var y = player.Position.Y + Math.Sin(angle) * distance;
                ghosts.Add(new GhostModel(new Vector2D(x, y)));
            }
        }
    }
}