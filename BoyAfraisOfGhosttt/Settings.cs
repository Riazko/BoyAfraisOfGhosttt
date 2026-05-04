namespace BoyAfraisOfGhosttt
{
    public static class Settings
    {
        public const int GameWidth = 800;
        public const int GameHeight = 600;

        // Движение
        public const float PlayerSpeed = 200f;      // пикселей/сек
        public const float GhostSpeed = 100f;       // пикселей/сек

        // Обзор
        public const float SightRadius = 150f;      // серый радиус
        public const float FlashRadius = 120f;      // дальность вспышки
        public const float FlashAngle = 30f;        // градусы
        public const float FlashCooldown = 3f;      // секунды

        // Игровая логика
        public const int MaxFlashKills = 3;         // макс убийств за вспышку
        public const int GhostSpawnInterval = 2000; // мс
        public const int MaxGhosts = 20;
        public const bool CyclicWorld = true;       // цикличная карта
    }
}