using System;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class MovementController
    {
        private GameModel gameModel;

        public MovementController(GameModel gameModel)
        {
           this.gameModel = gameModel;
        }

        public void MovePlayer(Vector2D direction, float deltaTime)
        {
            if (gameModel.IsGameOver || (direction.X == 0 && direction.Y == 0))
                return ;

            var norm = direction.Normalize();
            var newX = gameModel.Player.Position.X + norm.X * Settings.PlayerSpeed * deltaTime;
            var newY = gameModel.Player.Position.Y + norm.Y * Settings.PlayerSpeed * deltaTime;
            gameModel.Player.Position = new Vector2D(newX, newY);

            if (norm.X != 0 || norm.Y != 0)
            {
                gameModel.Player.Direction = Math.Atan2(norm.Y, norm.X);
            }
        }


    }
}