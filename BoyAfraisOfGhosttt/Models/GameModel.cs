using System;
using System.Collections.Generic;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Models
{
    public class GameModel
    {
        private readonly Random random;

        public PlayerModel Player { get; set; }
        public List<GhostModel> Ghosts { get; private set; }
        public WorldMap WorldMap { get; private set; }

        public bool IsGameOver { get; set; }

        public int Score { get; private set; }
        public int KilledGhosts { get; private set; }
        public int Wave { get; private set; }

        public GameModel()
        {
            random = new Random();
            Player = new PlayerModel();
            Ghosts = new List<GhostModel>();
            WorldMap = new WorldMap(Settings.MapGridWidth, Settings.MapGridHeight, Settings.TileSize);
            Score = 0;
            KilledGhosts = 0;
            Wave = 1;
            CreateStartGhosts(Ghosts, Player);
            IsGameOver = false;
        }

        public void CreateStartGhosts(List<GhostModel> ghosts, PlayerModel player)
        {
            for (var i = 0; i < 3; i++)
            {
                ghosts.Add(new GhostModel(GenerateStartGhostPosition(player), GhostType.Normal));
            }
        }

        private Vector2D GenerateStartGhostPosition(PlayerModel player)
        {
            for (var attempt = 0; attempt < 60; attempt++)
            {
                var angle = random.NextDouble() * Math.PI * 2;
                var distance = random.Next(180, 380);
                var position = new Vector2D(
                    player.Position.X + Math.Cos(angle) * distance,
                    player.Position.Y + Math.Sin(angle) * distance);

                if (Settings.CyclicWorld)
                    position = WorldGenerator.WrapPosition(position);

                if (!WorldMap.IsCircleBlocked(position, Settings.GhostCollisionRadius))
                    return position;
            }
            return player.Position.Add(new Vector2D(Settings.SightRadius + 80, 0));
        }

        public void RegisterGhostKill(GhostModel ghost)
        {
            if (ghost == null)
                return;
            KilledGhosts++;
            Score += ghost.ScoreReward;
            Wave = 1 + KilledGhosts / Settings.KillsPerWave;
        }

        public int GetCurrentMaxGhosts()
        {
            var waveBonus = (Wave - 1) * Settings.MaxGhostsPerWaveBonus;
            var currentMax = Settings.MaxGhosts + waveBonus;
            return Math.Min(currentMax, Settings.AbsoluteMaxGhosts);
        }
    }
}
