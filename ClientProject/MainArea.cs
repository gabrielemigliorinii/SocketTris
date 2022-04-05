using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ClientProject
{
    public partial class MainArea : Form
    {
        Client client;
        bool canClick;

        public MainArea()
        {
            InitializeComponent();
            client = new Client();
            canClick = true;
        }

        private void MainArea_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            label1.Select();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Inserire nickname" && textBox1.ForeColor == Color.Gray)
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.ForeColor = Color.Gray;
                textBox1.Text = "Inserire nickname";
            }
        }

        private bool tuttoInserito()
        {
            if (textBox1.Text != "" && textBox1.Text != "Inserire nickname") return true; return false;
        }

        private void inizia()
        {
            client.start();
            Close();
            new TrisArea(client).ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Select();
            if (canClick)
            {
                if (tuttoInserito())
                {
                    canClick = false;
                    labelATTESA.Visible = true;
                    labelATTESA.Text = "Attendi . . .";
                    textBox1.ReadOnly = true;
                    client.Nickname = textBox1.Text;
                    Thread thr = new Thread(new ThreadStart(inizia));
                    thr.Start();
                }
                else
                {
                    labelATTESA.Visible = true;
                    labelATTESA.Text = ("Inserire il nickname per avviare la partita");
                }
            }
            else
            {
                labelATTESA.Visible = true;
                labelATTESA.Text = ("Sei in gia' in attesa di una partita");
            }
        }

        private void labelATTESA_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void MainArea_Click(object sender, EventArgs e)
        {
            label1.Select();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            label1.Select();
            MessageBox.Show(DateTime.Now.ToString(), "Adesso");
        }
    }
}
