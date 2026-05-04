using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BoyAfraisOfGhosttt.Controllers
{
    public class GameController
    {
        private GameModel model;
        private Timer gameTimer;
        private Form form;
        private bool[] keys = new bool[256];
        
        public GameController(Form targetForm)
        {
            form = targetForm;
            model = new GameModel(form.ClientSize.Width, form.ClientSize.Height);
            
            form.Paint += OnPaint;
            form.KeyDown += OnKeyDown;
            form.KeyUp += OnKeyUp;
            form.Resize += OnResize;
            
            gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }
        
        private void GameLoop(object sender, EventArgs e)
        {
            if (model.IsGameOver) return;
            MovePlayer();
            
            model.Player.Update();
            
            UpdateGhosts();
            
            CheckCollisions();
            
            model.World.SpawnNewGhostIfNeeded(model.MaxGhosts);
            
            form.Invalidate();
        }
        
        private void MovePlayer()
        {
            int speed = 5;
            
            if (keys[(int)Keys.W]) model.Player.Y -= speed;
            if (keys[(int)Keys.S]) model.Player.Y += speed;
            if (keys[(int)Keys.A]) model.Player.X -= speed;
            if (keys[(int)Keys.D]) model.Player.X += speed;
            
            if (keys[(int)Keys.Space]) 
            {
                UseLight();
            }
            if (model.Player.X < 50) model.Player.X = 50;
            if (model.Player.X > model.World.Width - 50) model.Player.X = model.World.Width - 50;
            if (model.Player.Y < 50) model.Player.Y = 50;
            if (model.Player.Y > model.World.Height - 50) model.Player.Y = model.World.Height - 50;
        }
        
        private void UseLight()
        {
            if (model.Player.IsLightReady)
            {
                model.Player.UseLight();
                KillGhostsInCone();
            }
        }
        
        private void KillGhostsInCone()
        {
            int lightRange = 100;
            
            foreach (var ghost in model.World.Ghosts)
            {
                if (!ghost.IsAlive) continue;
                double distance = Math.Sqrt(Math.Pow(ghost.X - model.Player.X, 2) + 
                                            Math.Pow(ghost.Y - model.Player.Y, 2));
                if (distance < lightRange)
                {
                    ghost.IsAlive = false;
                    model.World.Score += 10;
                }
            }
            
            model.World.RemoveDeadGhosts();
        }
        
        private void UpdateGhosts()
        {
            foreach (var ghost in model.World.Ghosts)
            {
                ghost.Update();
                
                if (ghost.CanTeleport())
                {
                    ghost.Teleport(model.Player.X, model.Player.Y);
                }
            }
        }
        
        private void CheckCollisions()
        {
            foreach (var ghost in model.World.Ghosts)
            {
                if (!ghost.IsAlive) continue;
                
                double distance = Math.Sqrt(Math.Pow(ghost.X - model.Player.X, 2) + 
                                           Math.Pow(ghost.Y - model.Player.Y, 2));
                
                if (distance < 20) 
                {
                    model.IsGameOver = true;
                    gameTimer.Stop();
                    MessageBox.Show($"Game Over! Score: {model.World.Score}", "Игра окончена");
                    return;
                }
            }
        }
        
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            g.Clear(Color.Black);
            
            using (SolidBrush grayBrush = new SolidBrush(Color.FromArgb(80, 80, 80)))
            {
                g.FillEllipse(grayBrush, 
                    model.Player.X - model.Player.Radius,
                    model.Player.Y - model.Player.Radius,
                    model.Player.Radius * 2,
                    model.Player.Radius * 2);
            }
            
            foreach (var ghost in model.World.Ghosts)
            {
                if (ghost.IsAlive && IsInRadius(ghost.X, ghost.Y))
                {
                    using (SolidBrush ghostBrush = new SolidBrush(Color.Red))
                    {
                        g.FillRectangle(ghostBrush, ghost.X - 10, ghost.Y - 10, 20, 20);
                    }
                }
            }
            
            using (SolidBrush playerBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(playerBrush, model.Player.X - 10, model.Player.Y - 10, 20, 20);
            }
            
            DrawUI(g);
        }
        
        private bool IsInRadius(int x, int y)
        {
            double distance = Math.Sqrt(Math.Pow(x - model.Player.X, 2) + 
                                       Math.Pow(y - model.Player.Y, 2));
            return distance < model.Player.Radius;
        }
        
        private void DrawUI(Graphics g)
        {
            using (Font font = new Font("Arial", 16))
            using (SolidBrush whiteBrush = new SolidBrush(Color.White))
            {
                g.DrawString($"Score: {model.World.Score}", font, whiteBrush, 10, 10);
                if (!model.Player.IsLightReady)
                {
                    float cooldownPercent = 1 - (float)model.Player.LightCooldown / model.Player.LightCooldownMax;
                    g.DrawString($"Light: {(int)(cooldownPercent * 100)}%", font, whiteBrush, 10, 40);
                }
                else
                {
                    g.DrawString("Light: READY!", font, Brushes.LightGreen, 10, 40);
                }
            }
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode < keys.Length)
                keys[(int)e.KeyCode] = true;
        }
        
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode < keys.Length)
                keys[(int)e.KeyCode] = false;
        }
        
        private void OnResize(object sender, EventArgs e)
        {
            form.Invalidate();
        }
    }
}