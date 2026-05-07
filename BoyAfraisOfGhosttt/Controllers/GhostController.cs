using System;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class GhostController
    {
        private GameModel gameModel;

        public GhostController(GameModel gameModel)
        {
            this.gameModel = gameModel;
        }

        public void UpdateGhosts(float deltaTime)
        {
            if (gameModel.IsGameOver) 
                return;

            for (var i = gameModel.Ghosts.Count - 1; i >= 0; i--)
            {
                var ghost = gameModel.Ghosts[i];
                if (!ghost.IsAlive)
                {
                    gameModel.Ghosts.RemoveAt(i);
                    continue;
                }

                var directionToPlayer = gameModel.Player.Position.Subtract(ghost.Position);
                var distance = directionToPlayer.Length();

                if (distance < 0.01) 
                    continue;

                var norm = directionToPlayer.Normalize();

                var newX = ghost.Position.X + norm.X * Settings.GhostSpeed * deltaTime;
                var newY = ghost.Position.Y + norm.Y * Settings.GhostSpeed * deltaTime;
                ghost.Position = new Vector2D(newX, newY);

                if (ghost.Position.DistanceTo(gameModel.Player.Position) < 20)
                {
                    gameModel.IsGameOver = true;
                }
            }
        }

        public void SpawnGhost(Vector2D position)
        {
            if (gameModel.Ghosts.Count < Settings.MaxGhosts)
            {
                gameModel.Ghosts.Add(new GhostModel(position));
            }
        }
    }
}