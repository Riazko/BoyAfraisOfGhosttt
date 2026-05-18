using System;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class GhostController
    {
        private readonly GameModel gameModel;
        private readonly PathfindingController pathfinder;
        private readonly Random random;
        private float globalPathUpdateCooldown;
        private const float GlobalPathUpdateInterval = 0.3f;

        public GhostController(GameModel gameModel)
        {
            this.gameModel = gameModel;
            pathfinder = new PathfindingController(gameModel);
            random = new Random();
        }

        public void UpdateGhosts(float deltaTime)
        {
            if (gameModel.IsGameOver)
                return;

            pathfinder.UpdateHeatMap();
            globalPathUpdateCooldown -= deltaTime;
            lock (gameModel.Ghosts)
            {
                for (var i = gameModel.Ghosts.Count - 1; i >= 0; i--)
                {
                    var ghost = gameModel.Ghosts[i];
                    if (!ghost.IsAlive)
                    {
                        gameModel.Ghosts.RemoveAt(i);
                        continue;
                    }

                    ghost.PathUpdateTimer -= deltaTime;
                    bool shouldUpdatePath = ghost.PathUpdateTimer <= 0 || ghost.CurrentPath.Count == 0;
                    if (shouldUpdatePath && globalPathUpdateCooldown <= 0)
                    {
                        var distanceToPlayer = ghost.Position.DistanceTo(gameModel.Player.Position);
                        Vector2D pathTarget;
                        if (distanceToPlayer > Settings.FlashRadius)
                            pathTarget = pathfinder.GetBestSurroundTarget(ghost);
                        else
                            pathTarget = gameModel.Player.Position;
                        ghost.CurrentPath = pathfinder.FindPath(ghost.Position, pathTarget, ghost);
                        ghost.PathUpdateTimer = 0.5f + (float)random.NextDouble() * 0.3f;
                    }

                    if (ghost.CurrentPath.Count > 0)
                    {
                        MoveGhostByPath(ghost, deltaTime);
                    }
                    else
                    {
                        MoveGhostStraightToPlayer(ghost, deltaTime);
                    }

                    if (Settings.CyclicWorld)
                        ghost.Position = WorldGenerator.WrapPosition(ghost.Position);

                    if (ghost.Position.DistanceTo(gameModel.Player.Position) < 20)
                        gameModel.IsGameOver = true;
                }
            }
            if (globalPathUpdateCooldown <= 0)
                globalPathUpdateCooldown = GlobalPathUpdateInterval;
        }

        private void MoveGhostByPath(GhostModel ghost, float deltaTime)
        {
            var targetPosition = ghost.CurrentPath[0];
            double distanceToTarget = ghost.Position.DistanceTo(targetPosition);
            if (distanceToTarget < 15)
            {
                ghost.CurrentPath.RemoveAt(0);
                return;
            }
            MoveGhostToPosition(ghost, targetPosition, deltaTime);
        }

        private void MoveGhostStraightToPlayer(GhostModel ghost, float deltaTime)
        {
            MoveGhostToPosition(ghost, gameModel.Player.Position, deltaTime);
        }

        private void MoveGhostToPosition(GhostModel ghost, Vector2D targetPosition, float deltaTime)
        {
            var directionToTarget = targetPosition.Subtract(ghost.Position);
            var norm = directionToTarget.Normalize();
            var newX = ghost.Position.X + norm.X * ghost.Speed * deltaTime;
            var newY = ghost.Position.Y + norm.Y * ghost.Speed * deltaTime;
            ghost.Position = new Vector2D(newX, newY);
        }

        public void SpawnGhost(Vector2D position)
        {
            lock (gameModel.Ghosts)
            {
                if (gameModel.Ghosts.Count < Settings.MaxGhosts)
                    gameModel.Ghosts.Add(new GhostModel(position));
            }
        }

        public void ClearPathCache()
        {
            pathfinder.ClearCache();
        }
    }
}
