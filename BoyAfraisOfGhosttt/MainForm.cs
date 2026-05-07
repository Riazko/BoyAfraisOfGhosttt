using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BoyAfraidOfGhosts.Controllers;
using BoyAfraidOfGhosts.Helpers;
using BoyAfraidOfGhosts.Views;

namespace BoyAfraidOfGhosts
{
    public partial class MainForm : Form
    {
        private Timer gameTimer;
        private GameController controller;
        private bool upPressed;
        private bool downPressed;
        private bool leftPressed;
        private bool rightPressed;
        private GameRenderer renderer;

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
            SetupTimer();
            SetupController();
            SetupInput();
            SetupRenderer();
        }

        private void SetupForm()
        {
            this.Text = "Ghost Game";
            this.ClientSize = new Size(Settings.GameWidth, Settings.GameHeight);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.BackColor = Color.Black;
        }

        private void SetupTimer()
        {
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
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            this.MouseClick += OnMouseClick;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) upPressed = true;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) downPressed = true;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) leftPressed = true;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) rightPressed = true;
            if (e.KeyCode == Keys.R) controller.RestartGame();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) upPressed = false;
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) downPressed = false;
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) leftPressed = false;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) rightPressed = false;
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            var player = controller.GetModel().Player;
            var worldX = (e.X - Settings.GameWidth / 2) + player.Position.X;
            var worldY = (e.Y - Settings.GameHeight / 2) + player.Position.Y;
            var cursorWorld = new Vector2D(worldX, worldY);
            controller.PerformFlash(cursorWorld);
        }

        private Vector2D GetMoveDirection()
        {
            var x = 0.0;
            var y = 0.0;
            if (leftPressed) x = -1;
            if (rightPressed) x = 1;
            if (upPressed) y = -1;
            if (downPressed) y = 1;
            var raw = new Vector2D(x, y);
            return raw.Length() > 0 ? raw.Normalize() : raw;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            var direction = GetMoveDirection();
            var deltaTime = gameTimer.Interval / 1000f;
            controller.UpdateMovement(direction, deltaTime);
            controller.UpdateGhosts(deltaTime);
            controller.UpdateTimers(deltaTime);
            this.Invalidate();
        }

        private void SetupRenderer()
        {
            renderer = new GameRenderer();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            renderer.Render(e.Graphics, controller.GetModel(), controller.IsFlashEffectActive,
                controller.LastFlashDirection, Settings.GameWidth, Settings.GameHeight);
        }

    }
}