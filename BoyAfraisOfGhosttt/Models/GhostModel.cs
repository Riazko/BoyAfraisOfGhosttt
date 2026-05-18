using System.Collections.Generic;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public class GhostModel
    {
        public Vector2D Position { get; set; }
        public bool IsAlive { get; set; }
        public List<Vector2D> CurrentPath { get; set; }
        public float PathUpdateTimer { get; set; }
        public GhostType Type { get; private set; }
        public int Health { get; private set; }
        public float Speed { get; private set; }
        public int ScoreReward { get; private set; }

        public GhostModel(Vector2D position) : this(position, GhostType.Normal)
        {
        }

        public GhostModel(Vector2D position, GhostType type)
        {
            Position = position;
            Type = type;
            IsAlive = true;
            CurrentPath = new List<Vector2D>();
            PathUpdateTimer = 0;
            Health = GetStartHealth(type);
            Speed = GetSpeed(type);
            ScoreReward = GetScoreReward(type);
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive)
                return;

            Health -= damage;
            if (Health <= 0)
                IsAlive = false;
        }

        private int GetStartHealth(GhostType type)
        {
            switch (type)
            {
                case GhostType.Heavy:
                    return 2;
                default:
                    return 1;
            }
        }

        private float GetSpeed(GhostType type)
        {
            switch (type)
            {
                case GhostType.Fast:
                    return Settings.FastGhostSpeed;
                case GhostType.Heavy:
                    return Settings.HeavyGhostSpeed;
                case GhostType.Mist:
                    return Settings.MistGhostSpeed;
                default:
                    return Settings.GhostSpeed;
            }
        }

        private int GetScoreReward(GhostType type)
        {
            switch (type)
            {
                case GhostType.Fast:
                    return Settings.FastGhostScore;
                case GhostType.Heavy:
                    return Settings.HeavyGhostScore;
                case GhostType.Mist:
                    return Settings.MistGhostScore;
                default:
                    return Settings.NormalGhostScore;
            }
        }
    }
}