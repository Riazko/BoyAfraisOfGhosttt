using System;
using System.Threading;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;

namespace BoyAfraidOfGhosts.Controllers
{
    public class SpawnerThread : IDisposable
    {
        private readonly GameModel gameModel;
        private readonly Random rand;
        private readonly ManualResetEvent stopSignal;
        private Thread spawnThread;
        private volatile bool isRunning;
        private bool disposed;

        public SpawnerThread(GameModel gameModel)
        {
            this.gameModel = gameModel;
            rand = new Random();
            stopSignal = new ManualResetEvent(false);
            isRunning = false;
        }

        public void Start()
        {
            if (isRunning)
                return;
            isRunning = true;
            stopSignal.Reset();
            spawnThread = new Thread(SpawnLoop)
            {
                IsBackground = true,
                Name = "Ghost spawn thread"
            };
            spawnThread.Start();
        }

        public void Stop()
        {
            if (!isRunning)
                return;
            isRunning = false;
            stopSignal.Set();
            if (spawnThread != null && spawnThread.IsAlive && Thread.CurrentThread != spawnThread)
                spawnThread.Join();
        }

        private void SpawnLoop()
        {
            while (isRunning)
            {
                var interval = GetCurrentSpawnInterval();
                if (stopSignal.WaitOne(interval))
                    break;
                if (!isRunning || gameModel.IsGameOver)
                    continue;
                lock (gameModel.Ghosts)
                {
                    if (gameModel.Ghosts.Count >= gameModel.GetCurrentMaxGhosts())
                        continue;
                    var spawnPosition = GenerateSpawnPosition();
                    var ghostType = GenerateGhostType();
                    gameModel.Ghosts.Add(new GhostModel(spawnPosition, ghostType));
                }
            }
        }

        private int GetCurrentSpawnInterval()
        {
            var interval = Settings.GhostSpawnInterval -
                           (gameModel.Wave - 1) * Settings.SpawnAccelerationPerWave;
            return Math.Max(Settings.MinGhostSpawnInterval, interval);
        }

        private GhostType GenerateGhostType()
        {
            var wave = gameModel.Wave;
            var roll = rand.Next(100);
            if (wave >= 4 && roll < 8)
                return GhostType.Mist;

            if (wave >= 3 && roll < 23)
                return GhostType.Heavy;

            if (wave >= 2 && roll < 45)
                return GhostType.Fast;
            return GhostType.Normal;
        }

        private Vector2D GenerateSpawnPosition()
        {
            for (var attempt = 0; attempt < 40; attempt++)
            {
                var angle = rand.NextDouble() * Math.PI * 2;
                var distance = Settings.SightRadius + rand.Next(80, 240);
                var x = gameModel.Player.Position.X + Math.Cos(angle) * distance;
                var y = gameModel.Player.Position.Y + Math.Sin(angle) * distance;
                var position = new Vector2D(x, y);
                if (Settings.CyclicWorld)
                    position = WorldGenerator.WrapPosition(position);

                if (IsValidSpawnPosition(position))
                    return position;
            }
            return FindAnyFreePosition();
        }

        private bool IsValidSpawnPosition(Vector2D position)
        {
            if (!Settings.CyclicWorld && !gameModel.WorldMap.IsInsideWorld(position))
                return false;

            if (gameModel.WorldMap.IsCircleBlocked(position, Settings.GhostCollisionRadius))
                return false;

            if (position.DistanceTo(gameModel.Player.Position) < Settings.SightRadius + 40)
                return false;
            return true;
        }

        private Vector2D FindAnyFreePosition()
        {
            for (var attempt = 0; attempt < 100; attempt++)
            {
                var x = rand.Next(0, gameModel.WorldMap.WidthInCells);
                var y = rand.Next(0, gameModel.WorldMap.HeightInCells);
                var position = gameModel.WorldMap.CellToWorldCenter(x, y);
                if (IsValidSpawnPosition(position))
                    return position;
            }
            return gameModel.Player.Position;
        }

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            Stop();
            stopSignal.Dispose();
        }
    }
}
