using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace ClientProject
{
    public partial class TrisArea : Form
    {
        Client client;
        Label[] caselle;

        public TrisArea(Client client)
        {
            InitializeComponent();
            this.client = client;
            caselle = new Label[12];
        }

        private void TrisArea_Load(object sender, EventArgs e)
        {
            STATO.Visible = true;
            CheckForIllegalCrossThreadCalls = false;

            client.gestioneInformazioni();

            client.setTurno();
            setTable();
            setArrayCaselle();

            PLAYER1.Text = "|" + client.My + "| |" + client.Nickname + "|";
            PLAYER2.Text = "|" + client.notMy() + "| |" + client.Avversario + "|";
            if (!client.Turno)
            {
                Thread thr = new Thread(new ThreadStart(inAttesa));
                thr.Start();
            }
            setSTATO("E' IL TUO TURNO", Color.DeepSkyBlue);
        }

        private void setSTATO(string text, Color color)
        {
            STATO.Text = text;
            STATO.BackColor = color;
        }

        /*
        private void inAttesa()
        {
            int avversario = int.Parse(client.ricevi());
            caselle[avversario].Text = client.notMy();
            caselle[avversario].ForeColor = Color.Aqua;
            client.cambiaTurno();
        }*/

        private void setTable()
        {
            C0.ForeColor = C1.ForeColor = C2.ForeColor = Color.FromArgb(0, 64, 64);
            C3.ForeColor = C4.ForeColor = C5.ForeColor = Color.FromArgb(0, 64, 64);
            C6.ForeColor = C7.ForeColor = C8.ForeColor = Color.FromArgb(0, 64, 64);
        }

        private void setArrayCaselle()
        {
            caselle[0] = C0; caselle[1] = C1;
            caselle[2] = C2; caselle[3] = C3;
            caselle[4] = C4; caselle[5] = C5;
            caselle[6] = C6; caselle[7] = C7;
            caselle[8] = C8;
        }

        /*
        private void inAttesa()
        {
            int avversario = int.Parse(client.ricevi());
            caselle[avversario].Text = client.notMy();
            caselle[avversario].ForeColor = Color.Aqua;
            client.cambiaTurno();
        }*/

        private void setCasella(int index)
        {
            caselle[index].Text = client.notMy();
            caselle[index].ForeColor = Color.Aqua;
        }

        private void inAttesa()
        {
            setSTATO("ATTENDI MOSSA", Color.Yellow);
            string[] risposta = client.ricevi().Split(',');
            switch (risposta[1])
            {
                case "IN CORSO":
                    int indexAvversario = int.Parse(risposta[0]);
                    setCasella(indexAvversario);
                    client.cambiaTurno();
                    setSTATO("E' IL TUO TURNO", Color.DeepSkyBlue);
                    break;

                case "VINTO":
                    int[] cv = getIntVincitrici(risposta[2].Split('_'));
                    for (int i = 0; i < 3; i++)
                        caselle[cv[i]].ForeColor = Color.FromArgb(0, 255, 158);
                    setSTATO("HAI VINTO", Color.FromArgb(0, 255, 158));
                    break;

                case "PERSO":
                    int index = int.Parse(risposta[0]);
                    setCasella(index);
                    int[] cs = getIntVincitrici(risposta[2].Split('_'));
                    for (int i = 0; i < 3; i++)
                        caselle[cs[i]].ForeColor = Color.FromArgb(255, 0, 64);
                    client.invia("-1");
                    setSTATO("HAI PERSO", Color.FromArgb(255, 0, 64));
                    STATO.ForeColor = Color.White;
                    break;

                case "PAREGGIO":
                    try { client.invia("-1"); } catch (Exception E) { Console.WriteLine("Error:\n\n"+E); }
                    int pos = int.Parse(risposta[0]);
                    setCasella(pos);
                    setSTATO("PAREGGIO", Color.Aqua);
                    break;

                case null:
                    MessageBox.Show("Eccezione");
                    break;
            }
        }

        private int[] getIntVincitrici(string[] v)
        {
            int[] x = new int[3];
            for (int i = 0; i < 3; i++) x[i] = int.Parse(v[i]); return x;
        }

        private void submethod(int index)
        {
            client.mossa(index);
            inAttesa();
        }

        private void CClick(object sender, EventArgs e)
        {
            Label obj = (Label)sender;
            if (client.Turno && obj.Text == "")
            {
                client.cambiaTurno();
                obj.Text = client.My;
                obj.ForeColor = Color.Aqua;
                int index = int.Parse(obj.Name.Substring(1));

                Thread thr = new Thread(new ThreadStart(() => submethod(index)));
                thr.Start();
            }
        }

        private void TrisArea_FormClosing(object sender, FormClosingEventArgs e)
        {
            new MainArea().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            new MainArea().Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //new Thread(new client.invia("MESSAGGIO:"+messaggio.Text);
        }
    }
}
