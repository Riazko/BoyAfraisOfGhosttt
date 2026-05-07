using System.Drawing;
using BoyAfraidOfGhosts.Models;
using BoyAfraidOfGhosts.Helpers;
using System;

namespace BoyAfraidOfGhosts.Views
{
    public class GameRenderer
    {
        public void Render(Graphics g, GameModel model, bool isFlashActive,
            Vector2D flashDirection, int screenWidth, int screenHeight)
        {
            var player = model.Player;

            int centerX = screenWidth / 2;
            int centerY = screenHeight / 2;

            g.Clear(Color.Black);

            using (Brush sightBrush = new SolidBrush(Color.FromArgb(80, 80, 80)))
            {
                g.FillEllipse(sightBrush,
                    centerX - Settings.SightRadius,
                    centerY - Settings.SightRadius,
                    Settings.SightRadius * 2,
                    Settings.SightRadius * 2);
            }

            // Отрисовка эффекта вспышки (белый конус) - используем isFlashActive и flashDirection
            if (isFlashActive)
            {
                // Вычисляем две границы конуса (угол ± половина угла)
                double halfAngleRad = (Settings.FlashAngle / 2) * System.Math.PI / 180;
                double dirAngle = System.Math.Atan2(flashDirection.Y, flashDirection.X);

                double leftAngle = dirAngle - halfAngleRad;
                double rightAngle = dirAngle + halfAngleRad;

                // Точки на расстоянии FlashRadius
                int tipX = centerX;
                int tipY = centerY;

                int leftX = (int)(centerX + Math.Cos(leftAngle) * Math.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight));
                int leftY = (int)(centerY + Math.Sin(leftAngle) * Math.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight));
                int rightX = (int)(centerX + Math.Cos(rightAngle) * Math.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight));
                int rightY = (int)(centerY + Math.Sin(rightAngle) * Math.Sqrt(screenWidth * screenWidth + screenHeight * screenHeight));

                // Рисуем полупрозрачный белый конус
                using (Brush flashBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255)))
                {
                    PointF[] conePoints = new PointF[]
                    {
                        new PointF(tipX, tipY),
                        new PointF(leftX, leftY),
                        new PointF(rightX, rightY)
                    };
                    g.FillPolygon(flashBrush, conePoints);
                }
            }

            // Рисуем игрока (белый крестик)
            using (Pen pen = new Pen(Color.White, 2))
            {
                g.DrawLine(pen, centerX - 10, centerY, centerX + 10, centerY);
                g.DrawLine(pen, centerX, centerY - 10, centerX, centerY + 10);
            }

            // Текст с координатами
            var debugText = player.Position.ToString();
            using (Font font = new Font("Arial", 12))
            using (Brush brush = new SolidBrush(Color.White))
            {
                g.DrawString(debugText, font, brush, 10, 10);
            }

            // Отрисовка призраков
            foreach (var ghost in model.Ghosts)
            {
                if (!ghost.IsAlive) continue;

                // Проверка видимости призрака (в радиусе обзора)
                double distanceToPlayer = ghost.Position.DistanceTo(player.Position);
                if (distanceToPlayer > Settings.SightRadius) continue;

                // Преобразование мировых координат в экранные
                int ghostScreenX = (int)((ghost.Position.X - player.Position.X) + centerX);
                int ghostScreenY = (int)((ghost.Position.Y - player.Position.Y) + centerY);

                using (Brush ghostBrush = new SolidBrush(Color.Red))
                {
                    g.FillEllipse(ghostBrush, ghostScreenX - 8, ghostScreenY - 8, 16, 16);
                }
            }

            // Game Over экран
            if (model.IsGameOver)
            {
                using (Brush overlayBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                {
                    g.FillRectangle(overlayBrush, 0, 0, screenWidth, screenHeight);
                }

                string gameOverText = "GAME OVER";
                string restartText = "Press R to restart";
                using (Font bigFont = new Font("Arial", 36, FontStyle.Bold))
                using (Font smallFont = new Font("Arial", 18))
                using (Brush whiteBrush = new SolidBrush(Color.White))
                {
                    SizeF textSize = g.MeasureString(gameOverText, bigFont);
                    g.DrawString(gameOverText, bigFont, whiteBrush,
                        (screenWidth - textSize.Width) / 2,
                        screenHeight / 2 - 50);

                    g.DrawString(restartText, smallFont, whiteBrush,
                        (screenWidth - g.MeasureString(restartText, smallFont).Width) / 2,
                        screenHeight / 2 + 20);
                }
            }
        }
    }
}