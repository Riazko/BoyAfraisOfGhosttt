using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;
using System;

namespace BoyAfraidOfGhosts.Controllers
{
    public class LightController
    {
        private readonly GameModel gameModel;
        private float flashEffectTimer;
        private Vector2D lastFlashDirection;
        private readonly GreedyFlashSelector greedySelector;

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
            {
                flashEffectTimer -= deltaTime;
                if (flashEffectTimer < 0)
                    flashEffectTimer = 0;
            }
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
            gameModel.Player.Direction = Math.Atan2(flashDirection.Y, flashDirection.X);
            flashEffectTimer = 0.1f;
            var halfAngle = Settings.FlashAngle / 2;
            lock (gameModel.Ghosts)
            {
                var ghostsToHit = greedySelector.SelectGhostsToKill(
                    gameModel.Ghosts,
                    gameModel.Player.Position,
                    flashDirection,
                    halfAngle);
                foreach (var ghost in ghostsToHit)
                {
                    bool wasAlive = ghost.IsAlive;
                    ghost.TakeDamage(1);
                    if (wasAlive && !ghost.IsAlive)
                    {
                        gameModel.RegisterGhostKill(ghost);
                    }
                }
            }
            gameModel.Player.FlashCooldown = Settings.FlashCooldown;
        }

        public bool IsFlashEffectActive => flashEffectTimer > 0;
        public Vector2D LastFlashDirection => lastFlashDirection;
    }
}
