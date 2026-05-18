namespace BoyAfraidOfGhosts
{
    public static class Settings
    {
        public const int MapGridWidth = 50;
        public const int MapGridHeight = 50;
        public const int TileSize = 40;

        public const double WorldWidth = MapGridWidth * TileSize;
        public const double WorldHeight = MapGridHeight * TileSize;

        public const int WallClusterCount = 35;
        public const int WallMinLength = 3;
        public const int WallMaxLength = 8;
        public const int StartSafeZoneCells = 5;

        public const int GameWidth = 800;
        public const int GameHeight = 600;

        public const float PlayerSpeed = 200f;
        public const float PlayerCollisionRadius = 16f;

        public const float GhostSpeed = 150f;
        public const float FastGhostSpeed = 230f;
        public const float HeavyGhostSpeed = 95f;
        public const float MistGhostSpeed = 125f;
        public const float GhostCollisionRadius = 16f;

        public const int NormalGhostScore = 10;
        public const int FastGhostScore = 20;
        public const int HeavyGhostScore = 35;
        public const int MistGhostScore = 50;

        public const float SightRadius = 150f;
        public const float FlashRadius = 1200f;
        public const float FlashAngle = 30f;
        public const float FlashCooldown = 1.5f;

        public const int MaxFlashKills = 5;

        public const int GhostSpawnInterval = 1000;
        public const int MinGhostSpawnInterval = 350;
        public const int SpawnAccelerationPerWave = 80;

        public const int MaxGhosts = 10;
        public const int AbsoluteMaxGhosts = 45;
        public const int MaxGhostsPerWaveBonus = 2;

        public const int KillsPerWave = 8;

        public const bool CyclicWorld = true;

        public const int HeatMapGhostInfluenceRadius = 4;
        public const double HeatMapGhostPenalty = 6.0;
        public const double HeatMapPathWeight = 0.35;

        public const int SurroundMinDistanceCells = 2;
        public const int SurroundMaxDistanceCells = 4;

        public const double SurroundPressureWeight = 2.0;
        public const double SurroundDistanceWeight = 0.15;

        public const bool DebugDrawHeatMap = false;

        public const string PlayerImageFileName = "personaj.png";
        public const string GhostReferencesFolder = "ReferencesGhost";
        public const string NormalGhostImageFileName = "RedGhost.png";
        public const string FastGhostImageFileName = "FiolGhost.png";
        public const string HeavyGhostImageFileName = "GreenGhost.png";
        public const string MistGhostImageFileName = "BlueGhost.png";
        public const int PlayerImageSize = 36;
        public const int GhostImageSize = 32;
    }
}
