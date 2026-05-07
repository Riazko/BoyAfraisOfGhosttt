using System;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class LightController
    {
        private GameModel gameModel;
        private float flashEffectTimer;
        private Vector2D lastFlashDirection;
        private GreedyFlashSelector greedySelector;

        public LightController(GameModel gameModel)
        {
            this.gameModel = gameModel;
            flashEffectTimer = 0;
            lastFlashDirection = new Vector2D(1, 0);
            greedySelector = new GreedyFlashSelector();
        }

        public void Update(float deltaTime)
        {
            if (gameModel.Player.FlashCooldown > 0)
            {
                gameModel.Player.FlashCooldown -= deltaTime;
                if (gameModel.Player.FlashCooldown < 0)
                    gameModel.Player.FlashCooldown = 0;
            }
            if (flashEffectTimer > 0)
                flashEffectTimer -= deltaTime;
        }

        public void PerformFlash(Vector2D cursorWorldPosition)
        {
            if (gameModel.IsGameOver || gameModel.Player.FlashCooldown > 0)
                return;

            var flashDirection = cursorWorldPosition.Subtract(gameModel.Player.Position);
            if (flashDirection.Length() < 0.01)
                return;
            flashDirection = flashDirection.Normalize();

            lastFlashDirection = flashDirection;
            flashEffectTimer = 0.1f;

            var halfAngle = Settings.FlashAngle / 2;
            var ghostsToKill = greedySelector.SelectGhostsToKill(gameModel.Ghosts, gameModel.Player.Position,
                flashDirection, halfAngle);

            foreach (var ghost in ghostsToKill)
            {
                ghost.IsAlive = false;
            }
            gameModel.Player.FlashCooldown = Settings.FlashCooldown;
        }

        public bool IsFlashEffectActive => flashEffectTimer > 0;
        public Vector2D LastFlashDirection => lastFlashDirection;
    }
}