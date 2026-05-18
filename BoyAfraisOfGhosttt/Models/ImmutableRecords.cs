using System.Collections.Generic;
using System.Linq;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public sealed class ImmutablePlayerModel
    {
        public Vector2D Position { get; private set; }
        public float FlashCooldown { get; private set; }
        public double Direction { get; private set; }
        public int FacingDirection { get; private set; }

        public ImmutablePlayerModel(
            Vector2D position,
            float flashCooldown,
            double direction,
            int facingDirection)
        {
            Position = position;
            FlashCooldown = flashCooldown;
            Direction = direction;
            FacingDirection = facingDirection;
        }

        public static ImmutablePlayerModel From(PlayerModel player)
        {
            return new ImmutablePlayerModel(
                player.Position,
                player.FlashCooldown,
                player.Direction,
                player.FacingDirection);
        }

        public ImmutablePlayerModel WithPosition(Vector2D position)
        {
            return new ImmutablePlayerModel(
                position,
                FlashCooldown,
                Direction,
                FacingDirection);
        }

        public ImmutablePlayerModel WithFlashCooldown(float flashCooldown)
        {
            return new ImmutablePlayerModel(
                Position,
                flashCooldown,
                Direction,
                FacingDirection);
        }

        public ImmutablePlayerModel WithDirection(double direction)
        {
            return new ImmutablePlayerModel(
                Position,
                FlashCooldown,
                direction,
                FacingDirection);
        }

        public ImmutablePlayerModel WithFacingDirection(int facingDirection)
        {
            return new ImmutablePlayerModel(
                Position,
                FlashCooldown,
                Direction,
                facingDirection);
        }
    }

    public sealed class ImmutableGhostModel
    {
        public Vector2D Position { get; private set; }
        public bool IsAlive { get; private set; }
        public IReadOnlyList<Vector2D> CurrentPath { get; private set; }
        public float PathUpdateTimer { get; private set; }

        public ImmutableGhostModel(
            Vector2D position,
            bool isAlive,
            IReadOnlyList<Vector2D> currentPath,
            float pathUpdateTimer)
        {
            Position = position;
            IsAlive = isAlive;
            CurrentPath = currentPath;
            PathUpdateTimer = pathUpdateTimer;
        }

        public static ImmutableGhostModel From(GhostModel ghost)
        {
            return new ImmutableGhostModel(
                ghost.Position,
                ghost.IsAlive,
                ghost.CurrentPath.ToList(),
                ghost.PathUpdateTimer);
        }

        public ImmutableGhostModel WithPosition(Vector2D position)
        {
            return new ImmutableGhostModel(
                position,
                IsAlive,
                CurrentPath,
                PathUpdateTimer);
        }

        public ImmutableGhostModel WithIsAlive(bool isAlive)
        {
            return new ImmutableGhostModel(
                Position,
                isAlive,
                CurrentPath,
                PathUpdateTimer);
        }

        public ImmutableGhostModel WithCurrentPath(IReadOnlyList<Vector2D> currentPath)
        {
            return new ImmutableGhostModel(
                Position,
                IsAlive,
                currentPath,
                PathUpdateTimer);
        }

        public ImmutableGhostModel WithPathUpdateTimer(float pathUpdateTimer)
        {
            return new ImmutableGhostModel(
                Position,
                IsAlive,
                CurrentPath,
                pathUpdateTimer);
        }
    }
}
