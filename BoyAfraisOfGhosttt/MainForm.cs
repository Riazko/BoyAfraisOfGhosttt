using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BoyAfraidOfGhosts.Controllers;
using BoyAfraidOfGhosts.Helpers;
using BoyAfraidOfGhosts.Views;

namespace BoyAfraidOfGhosts
{
    public partial class MainForm : Form
    {
        private Timer gameTimer;
        private Stopwatch gameStopwatch;
        private GameController controller;
        private bool upPressed;
        private bool downPressed;
        private bool leftPressed;
        private bool rightPressed;
        private GameRenderer renderer;
        private Point lastMousePosition;
        private bool hasMousePosition;

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
            SetupController();
            SetupInput();
            SetupRenderer();
            SetupTimer();
        }

        private void SetupForm()
        {
            Text = "Ghost Game";
            ClientSize = new Size(Settings.GameWidth, Settings.GameHeight);
            DoubleBuffered = true;
            KeyPreview = true;
            BackColor = Color.Black;
        }

        private void SetupTimer()
        {
            gameStopwatch = Stopwatch.StartNew();
            gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void SetupController()
        {
            controller = new GameController();
        }

        private void SetupInput()
        {
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            MouseClick += OnMouseClick;
            MouseMove += OnMouseMove;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) upPressed = true;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) downPressed = true;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) leftPressed = true;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) rightPressed = true;
            if (e.KeyCode == Keys.R)
            {
                controller.RestartGame();
                UpdatePlayerFacing();
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) upPressed = false;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) downPressed = false;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) leftPressed = false;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) rightPressed = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            lastMousePosition = e.Location;
            hasMousePosition = true;
            UpdatePlayerFacing();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            lastMousePosition = e.Location;
            hasMousePosition = true;
            UpdatePlayerFacing();
            controller.PerformFlash(ScreenToWorld(e.Location));
        }

        private Vector2D ScreenToWorld(Point screenPoint)
        {
            var player = controller.GetModel().Player;
            var worldX = (screenPoint.X - ClientSize.Width / 2.0) + player.Position.X;
            var worldY = (screenPoint.Y - ClientSize.Height / 2.0) + player.Position.Y;
            return new Vector2D(worldX, worldY);
        }

        private void UpdatePlayerFacing()
        {
            if (!hasMousePosition)
                return;
            var faceRight = lastMousePosition.X >= ClientSize.Width / 2;
            controller.UpdatePlayerFacing(faceRight);
        }

        private Vector2D GetMoveDirection()
        {
            var x = 0.0;
            var y = 0.0;
            if (leftPressed) x -= 1;
            if (rightPressed) x += 1;
            if (upPressed) y -= 1;
            if (downPressed) y += 1;
            var raw = new Vector2D(x, y);
            return raw.Length() > 0 ? raw.Normalize() : raw;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            float deltaTime = (float)gameStopwatch.Elapsed.TotalSeconds;
            gameStopwatch.Restart();
            if (deltaTime > 0.05f)
                deltaTime = 0.05f;

            var direction = GetMoveDirection();
            controller.UpdateMovement(direction, deltaTime);
            UpdatePlayerFacing();
            controller.UpdateGhosts(deltaTime);
            controller.UpdateTimers(deltaTime);
            Invalidate();
        }

        private void SetupRenderer()
        {
            renderer = new GameRenderer();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            renderer.Render(
                e.Graphics,
                controller.GetModel(),
                controller.IsFlashEffectActive,
                controller.LastFlashDirection,
                ClientSize.Width,
                ClientSize.Height);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (gameTimer != null)
                gameTimer.Stop();

            if (renderer != null)
                renderer.Dispose();

            if (controller != null)
                controller.Dispose();
            base.OnFormClosed(e);
        }
    }
}
