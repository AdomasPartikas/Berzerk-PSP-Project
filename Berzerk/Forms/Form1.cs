using Berzerk.Forms;

namespace Berzerk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Game game = new Game();
            game.FormClosed += (s, args) => this.Close();
            game.Show();
        }
    }
}