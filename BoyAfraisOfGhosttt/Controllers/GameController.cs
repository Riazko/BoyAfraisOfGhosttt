using System;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class GameController : IDisposable
    {
        private GameModel model;
        private MovementController movementController;
        private GhostController ghostController;
        private LightController lightController;
        private SpawnerThread spawnerThread;
        private bool disposed;

        public GameController()
        {
            CreateGameObjects();
        }

        private void CreateGameObjects()
        {
            model = new GameModel();
            movementController = new MovementController(model);
            ghostController = new GhostController(model);
            lightController = new LightController(model);
            spawnerThread = new SpawnerThread(model);
            spawnerThread.Start();
        }

        public GameModel GetModel()
        {
            return model;
        }

        public void UpdateMovement(Vector2D direction, float deltaTime)
        {
            movementController.MovePlayer(direction, deltaTime);
        }

        public void UpdatePlayerFacing(bool faceRight)
        {
            movementController.SetFacingDirection(faceRight);
        }

        public void UpdateGhosts(float deltaTime)
        {
            ghostController.UpdateGhosts(deltaTime);
        }

        public void UpdateTimers(float deltaTime)
        {
            lightController.Update(deltaTime);
        }

        public void PerformFlash(Vector2D cursorWorldPosition)
        {
            lightController.PerformFlash(cursorWorldPosition);
        }

        public bool IsFlashEffectActive => lightController.IsFlashEffectActive;
        public Vector2D LastFlashDirection => lightController.LastFlashDirection;

        public void RestartGame()
        {
            if (spawnerThread != null)
                spawnerThread.Stop();
            CreateGameObjects();
        }

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            if (spawnerThread != null)
                spawnerThread.Dispose();
        }
    }
}
