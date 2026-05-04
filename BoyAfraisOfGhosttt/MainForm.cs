using System.Windows.Forms;
using BoyAfraisOfGhosttt.Controllers;

namespace BoyAfraisOfGhosttt.Views
{
    public class MainForm : Form
    {
        private GameController controller;
    
        public MainForm()
        {
            InitializeForm();
            controller = new GameController(this);
        }
    
        private void InitializeForm()
        {
            this.Text = "Boy Afraid of Ghost";
            this.Size = new System.Drawing.Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.Black;
            this.DoubleBuffered = true; 
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
    }
}