using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class GameController
    {
        private GameModel model;
        private MovementController movementController;
        private GhostController ghostController;
        private LightController lightController;

        public GameController()
        {
            model = new GameModel();
            movementController = new MovementController(model);
            ghostController = new GhostController(model);
            lightController = new LightController(model);
        }

        public GameModel GetModel()
        {
            return model;
        }

        public void UpdateMovement(Vector2D direction, float deltaTime)
        {
            movementController.MovePlayer(direction, deltaTime);
            //model.Ghosts.Add(new GhostModel(new Vector2D(60.0, 0.0)));
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
            model = new GameModel();
            movementController = new MovementController(model);
            ghostController = new GhostController(model);
            lightController = new LightController(model);
        }
    }
}