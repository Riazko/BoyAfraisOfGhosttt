using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class MovementController
    {
        private readonly GameModel gameModel;

        public MovementController(GameModel gameModel)
        {
            this.gameModel = gameModel;
        }

        public void MovePlayer(Vector2D direction, float deltaTime)
        {
            if (gameModel.IsGameOver || (direction.X == 0 && direction.Y == 0))
                return;

            var norm = direction.Normalize();
            var moveX = norm.X * Settings.PlayerSpeed * deltaTime;
            var moveY = norm.Y * Settings.PlayerSpeed * deltaTime;
            var currentPosition = gameModel.Player.Position;
            var fullMovePosition = new Vector2D(currentPosition.X + moveX, currentPosition.Y + moveY);

            if (Settings.CyclicWorld)
                fullMovePosition = WorldGenerator.WrapPosition(fullMovePosition);

            if (!gameModel.WorldMap.IsCircleBlocked(fullMovePosition, Settings.PlayerCollisionRadius))
            {
                gameModel.Player.Position = fullMovePosition;
            }
            else
            {
                TryMoveByAxis(currentPosition, moveX, 0);
                TryMoveByAxis(gameModel.Player.Position, 0, moveY);
            }
        }

        public void SetFacingDirection(bool faceRight)
        {
            if (gameModel.IsGameOver)
                return;

            gameModel.Player.FacingDirection = faceRight ? 1 : -1;
            gameModel.Player.Direction = faceRight ? 0 : System.Math.PI;
        }

        private void TryMoveByAxis(Vector2D startPosition, double moveX, double moveY)
        {
            var candidate = new Vector2D(startPosition.X + moveX, startPosition.Y + moveY);
            if (Settings.CyclicWorld)
                candidate = WorldGenerator.WrapPosition(candidate);

            if (!gameModel.WorldMap.IsCircleBlocked(candidate, Settings.PlayerCollisionRadius))
                gameModel.Player.Position = candidate;
        }
    }
}
