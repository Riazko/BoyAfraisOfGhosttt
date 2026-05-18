using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using BoyAfraidOfGhosts.Helpers;
using BoyAfraidOfGhosts.Models;

namespace BoyAfraidOfGhosts.Views
{
    public class GameRenderer : IDisposable
    {
        private readonly Image playerImage;
        private readonly Dictionary<GhostType, Image> ghostImages;
        private bool disposed;

        public GameRenderer()
        {
            playerImage = LoadImageSafe(Settings.PlayerImageFileName);
            ghostImages = LoadGhostImages();
        }

        public void Render(
            Graphics g,
            GameModel model,
            bool isFlashActive,
            Vector2D flashDirection,
            int screenWidth,
            int screenHeight)
        {
            var player = model.Player;
            var centerX = screenWidth / 2;
            var centerY = screenHeight / 2;

            g.Clear(Color.Black);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawSightCircle(g, centerX, centerY);
            DrawWorldMap(g, model, centerX, centerY);
            if (isFlashActive)
                DrawFlashCone(g, centerX, centerY, flashDirection);

            DrawPlayer(g, player, centerX, centerY);
            DrawGhosts(g, model, centerX, centerY);
            DrawDebugText(g, player);
            DrawHud(g, model);
            if (model.IsGameOver)
                DrawGameOver(g, model, screenWidth, screenHeight);
        }

        private void DrawSightCircle(Graphics g, int centerX, int centerY)
        {
            using (Brush sightBrush = new SolidBrush(Color.FromArgb(80, 80, 80)))
            {
                g.FillEllipse(
                    sightBrush,
                    centerX - Settings.SightRadius,
                    centerY - Settings.SightRadius,
                    Settings.SightRadius * 2,
                    Settings.SightRadius * 2);
            }
        }

        private void DrawWorldMap(Graphics g, GameModel model, int centerX, int centerY)
        {
            var player = model.Player;
            var map = model.WorldMap;

            using (Brush wallBrush = new SolidBrush(Color.FromArgb(150, 90, 90, 100)))
            using (Pen wallPen = new Pen(Color.FromArgb(220, 140, 140, 160), 1))
            {
                for (var x = 0; x < map.WidthInCells; x++)
                {
                    for (var y = 0; y < map.HeightInCells; y++)
                    {
                        if (!map.IsWall(x, y))
                            continue;

                        var tileCenter = map.CellToWorldCenter(x, y);
                        var distanceToPlayer = tileCenter.DistanceTo(player.Position);
                        if (distanceToPlayer > Settings.SightRadius + Settings.TileSize)
                            continue;

                        var screenX = (int)((tileCenter.X - player.Position.X) + centerX);
                        var screenY = (int)((tileCenter.Y - player.Position.Y) + centerY);
                        var size = Settings.TileSize;
                        var rect = new Rectangle(
                            screenX - size / 2,
                            screenY - size / 2,
                            size,
                            size);

                        g.FillRectangle(wallBrush, rect);
                        g.DrawRectangle(wallPen, rect);
                    }
                }
            }
        }

        private void DrawFlashCone(Graphics g, int centerX, int centerY, Vector2D flashDirection)
        {
            if (flashDirection.Length() < 0.01)
                return;

            flashDirection = flashDirection.Normalize();
            var halfAngleRad = (Settings.FlashAngle / 2) * Math.PI / 180;
            var dirAngle = Math.Atan2(flashDirection.Y, flashDirection.X);
            var leftAngle = dirAngle - halfAngleRad;
            var rightAngle = dirAngle + halfAngleRad;
            var leftX = (int)(centerX + Math.Cos(leftAngle) * Settings.FlashRadius);
            var leftY = (int)(centerY + Math.Sin(leftAngle) * Settings.FlashRadius);
            var rightX = (int)(centerX + Math.Cos(rightAngle) * Settings.FlashRadius);
            var rightY = (int)(centerY + Math.Sin(rightAngle) * Settings.FlashRadius);
            PointF[] conePoints =
            {
                new PointF(centerX, centerY),
                new PointF(leftX, leftY),
                new PointF(rightX, rightY)
            };

            using (Brush flashBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
                g.FillPolygon(flashBrush, conePoints);

            using (Pen flashBorderPen = new Pen(Color.FromArgb(180, 255, 255, 255), 2))
                g.DrawPolygon(flashBorderPen, conePoints);
        }

        private void DrawPlayer(Graphics g, PlayerModel player, int centerX, int centerY)
        {
            if (playerImage != null)
            {
                DrawImageCenteredFlipped(
                    g,
                    playerImage,
                    centerX,
                    centerY,
                    Settings.PlayerImageSize,
                    Settings.PlayerImageSize,
                    player.FacingDirection);

                return;
            }
            DrawFallbackPlayer(g, centerX, centerY, player.Direction);
        }

        private void DrawFallbackPlayer(Graphics g, int centerX, int centerY, double direction)
        {
            const int bodyRadius = 8;
            const int noseLength = 18;

            var facing = Math.Cos(direction) >= 0 ? 1 : -1;
            var noseX = centerX + facing * noseLength;
            var noseY = centerY;
            Point[] triangle =
            {
                new Point(noseX, noseY),
                new Point(centerX - facing * bodyRadius, centerY - bodyRadius),
                new Point(centerX - facing * bodyRadius, centerY + bodyRadius)
            };

            using (Brush brush = new SolidBrush(Color.White))
                g.FillPolygon(brush, triangle);

            using (Pen pen = new Pen(Color.White, 2))
                g.DrawEllipse(pen, centerX - bodyRadius, centerY - bodyRadius, bodyRadius * 2, bodyRadius * 2);
        }

        private void DrawGhosts(Graphics g, GameModel model, int centerX, int centerY)
        {
            List<GhostModel> ghostsSnapshot;

            lock (model.Ghosts)
            {
                ghostsSnapshot = model.Ghosts.ToList();
            }

            foreach (var ghost in ghostsSnapshot.Where(ghost => ghost.IsAlive))
            {
                var distanceToPlayer = ghost.Position.DistanceTo(model.Player.Position);
                if (distanceToPlayer > Settings.SightRadius)
                    continue;

                var ghostScreenX = (int)((ghost.Position.X - model.Player.Position.X) + centerX);
                var ghostScreenY = (int)((ghost.Position.Y - model.Player.Position.Y) + centerY);
                DrawGhost(g, ghost, ghostScreenX, ghostScreenY);
            }
        }

        private void DrawGhost(Graphics g, GhostModel ghost, int screenX, int screenY)
        {
            var ghostColor = GetGhostColor(ghost.Type);
            var size = GetGhostSize(ghost.Type);
            var ghostImage = GetGhostImage(ghost.Type);
            if (ghostImage != null)
            {
                DrawImageCentered(g, ghostImage, screenX, screenY, size, size);
                return;
            }

            using (Brush ghostBrush = new SolidBrush(ghostColor))
            {
                g.FillEllipse(
                    ghostBrush,
                    screenX - size / 2,
                    screenY - size / 2,
                    size,
                    size);
            }

            using (Pen outlinePen = new Pen(Color.White, 1))
            {
                g.DrawEllipse(
                    outlinePen,
                    screenX - size / 2,
                    screenY - size / 2,
                    size,
                    size);
            }
            DrawGhostSpecialMark(g, ghost, screenX, screenY, size);
        }

        private void DrawGhostSpecialMark(Graphics g, GhostModel ghost, int screenX, int screenY, int size)
        {
            if (ghost.Type == GhostType.Heavy)
                DrawHeavyGhostMark(g, ghost, screenX, screenY);
            else if (ghost.Type == GhostType.Fast)
                DrawFastGhostMark(g, screenX, screenY);
            else if (ghost.Type == GhostType.Mist)
                DrawMistGhostMark(g, screenX, screenY, size);
        }

        private void DrawHeavyGhostMark(Graphics g, GhostModel ghost, int screenX, int screenY)
        {
            using (Pen pen = new Pen(Color.White, 2))
            {
                g.DrawLine(pen, screenX - 8, screenY - 8, screenX + 8, screenY + 8);
                g.DrawLine(pen, screenX + 8, screenY - 8, screenX - 8, screenY + 8);
            }
            if (ghost.Health == 1)
            {
                using (Brush brush = new SolidBrush(Color.Yellow))
                    g.FillEllipse(brush, screenX - 4, screenY - 4, 8, 8);
            }
        }

        private void DrawFastGhostMark(Graphics g, int screenX, int screenY)
        {
            using (Pen pen = new Pen(Color.White, 2))
            {
                Point[] points =
                {
                    new Point(screenX - 5, screenY - 8),
                    new Point(screenX + 5, screenY),
                    new Point(screenX - 2, screenY),
                    new Point(screenX + 5, screenY + 8)
                };
                g.DrawLines(pen, points);
            }
        }

        private void DrawMistGhostMark(Graphics g, int screenX, int screenY, int size)
        {
            using (Pen pen = new Pen(Color.White, 1))
            {
                g.DrawArc(
                    pen,
                    screenX - size / 2 - 4,
                    screenY - size / 2 - 4,
                    size + 8,
                    size + 8,
                    20,
                    300);
            }
        }

        private Color GetGhostColor(GhostType type)
        {
            switch (type)
            {
                case GhostType.Fast:
                    return Color.Magenta;
                case GhostType.Heavy:
                    return Color.DarkRed;
                case GhostType.Mist:
                    return Color.LightSkyBlue;
                default:
                    return Color.Red;
            }
        }

        private int GetGhostSize(GhostType type)
        {
            switch (type)
            {
                case GhostType.Fast:
                    return Settings.GhostImageSize - 4;
                case GhostType.Heavy:
                    return Settings.GhostImageSize + 10;
                default:
                    return Settings.GhostImageSize;
            }
        }

        private void DrawDebugText(Graphics g, PlayerModel player)
        {
            var debugText = "Player position: " + player.Position +
                               " | Direction: " + player.Direction.ToString("F2");
            using (Font font = new Font("Arial", 12))
            using (Brush brush = new SolidBrush(Color.White))
                g.DrawString(debugText, font, brush, 10, 10);
        }

        private void DrawHud(Graphics g, GameModel model)
        {
            int ghostCount;
            lock (model.Ghosts)
            {
                ghostCount = model.Ghosts.Count;
            }

            var flashText = model.Player.FlashCooldown <= 0
                ? "Ready"
                : model.Player.FlashCooldown.ToString("F1") + " sec";
            var hudText =
                "Score: " + model.Score + "\n" +
                "Kills: " + model.KilledGhosts + "\n" +
                "Wave: " + model.Wave + "\n" +
                "Ghosts: " + ghostCount + "/" + model.GetCurrentMaxGhosts() + "\n" +
                "Flash: " + flashText + "\n" +
                "Types:\n" +
                "Red - normal\n" +
                "Purple - fast\n" +
                "Green - heavy\n" +
                "Blue - mist";
            using (Font font = new Font("Arial", 12, FontStyle.Bold))
            using (Brush shadowBrush = new SolidBrush(Color.Black))
            using (Brush textBrush = new SolidBrush(Color.White))
            {
                g.DrawString(hudText, font, shadowBrush, 11, 36);
                g.DrawString(hudText, font, textBrush, 10, 35);
            }
        }

        private void DrawGameOver(Graphics g, GameModel model, int screenWidth, int screenHeight)
        {
            using (Brush overlayBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                g.FillRectangle(overlayBrush, 0, 0, screenWidth, screenHeight);

            var gameOverText = "GAME OVER";
            var scoreText = "Score: " + model.Score;
            var killsText = "Kills: " + model.KilledGhosts;
            var waveText = "Wave: " + model.Wave;
            var restartText = "Press R to restart";

            using (Font bigFont = new Font("Arial", 36, FontStyle.Bold))
            using (Font mediumFont = new Font("Arial", 18, FontStyle.Bold))
            using (Font smallFont = new Font("Arial", 16))
            using (Brush whiteBrush = new SolidBrush(Color.White))
            {
                DrawCenteredText(g, gameOverText, bigFont, whiteBrush, screenWidth, screenHeight / 2 - 90);
                DrawCenteredText(g, scoreText, mediumFont, whiteBrush, screenWidth, screenHeight / 2 - 25);
                DrawCenteredText(g, killsText, smallFont, whiteBrush, screenWidth, screenHeight / 2 + 10);
                DrawCenteredText(g, waveText, smallFont, whiteBrush, screenWidth, screenHeight / 2 + 40);
                DrawCenteredText(g, restartText, smallFont, whiteBrush, screenWidth, screenHeight / 2 + 85);
            }
        }

        private void DrawCenteredText(Graphics g, string text, Font font, Brush brush, int screenWidth, int y)
        {
            SizeF textSize = g.MeasureString(text, font);
            g.DrawString(text, font, brush, (screenWidth - textSize.Width) / 2, y);
        }

        private void DrawImageCentered(Graphics g, Image image, int centerX, int centerY, int width, int height)
        {
            g.DrawImage(image, centerX - width / 2, centerY - height / 2, width, height);
        }

        private void DrawImageCenteredFlipped(
            Graphics g,
            Image image,
            int centerX,
            int centerY,
            int width,
            int height,
            int facingDirection)
        {
            if (facingDirection >= 0)
            {
                DrawImageCentered(g, image, centerX, centerY, width, height);
                return;
            }
            GraphicsState state = g.Save();
            g.TranslateTransform(centerX, centerY);
            g.ScaleTransform(-1, 1);
            g.DrawImage(image, -width / 2, -height / 2, width, height);
            g.Restore(state);
        }

        private Dictionary<GhostType, Image> LoadGhostImages()
        {
            var result = new Dictionary<GhostType, Image>();
            AddGhostImage(result, GhostType.Normal, Settings.NormalGhostImageFileName);
            AddGhostImage(result, GhostType.Fast, Settings.FastGhostImageFileName);
            AddGhostImage(result, GhostType.Heavy, Settings.HeavyGhostImageFileName);
            AddGhostImage(result, GhostType.Mist, Settings.MistGhostImageFileName);
            return result;
        }

        private void AddGhostImage(Dictionary<GhostType, Image> images, GhostType type, string fileName)
        {
            var image = LoadImageSafe(fileName);
            if (image != null)
                images[type] = image;
        }

        private Image GetGhostImage(GhostType type)
        {
            Image image;
            if (ghostImages.TryGetValue(type, out image))
                return image;
            return null;
        }

        private Image LoadImageSafe(string fileName)
        {
            var path = FindAssetPath(fileName);
            if (path == null)
                return null;
            return Image.FromFile(path);
        }

        private string FindAssetPath(string fileName)
        {
            string[] startDirectories =
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Directory.GetCurrentDirectory()
            };

            foreach (var startDirectory in startDirectories)
            {
                var currentDirectory = startDirectory;
                for (var i = 0; i < 5 && !string.IsNullOrEmpty(currentDirectory); i++)
                {
                    var directPath = Path.Combine(currentDirectory, fileName);
                    if (File.Exists(directPath))
                        return directPath;

                    var assetsPath = Path.Combine(currentDirectory, "Assets", fileName);
                    if (File.Exists(assetsPath))
                        return assetsPath;

                    var ghostPath = Path.Combine(currentDirectory, Settings.GhostReferencesFolder, fileName);
                    if (File.Exists(ghostPath))
                        return ghostPath;

                    var assetsGhostPath = Path.Combine(currentDirectory, "Assets", Settings.GhostReferencesFolder, fileName);
                    if (File.Exists(assetsGhostPath))
                        return assetsGhostPath;

                    DirectoryInfo parentDirectory = Directory.GetParent(currentDirectory);
                    currentDirectory = parentDirectory == null ? null : parentDirectory.FullName;
                }
            }
            return null;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            if (playerImage != null)
                playerImage.Dispose();

            foreach (var image in ghostImages.Values)
                image.Dispose();
        }
    }
}
