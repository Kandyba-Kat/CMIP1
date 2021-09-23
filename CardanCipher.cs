using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AtbashCipher
{
    public partial class CardanCipher : Form
    {
        public CardanCipher()
        {
            InitializeComponent();
        }

        int[,] G;

        static string inputTB;

        bool withTrash = false;

        public static int[,] CreateGrille(int n, bool trash)
        {
            int[,] G = new int[n, n];
            int[,] tmp = new int[n / 2, n / 2];
            for (int i = 0; i < n / 2; i++)
            {
                for (int j = 0; j < n / 2; j++)
                {
                    tmp[i, j] = i * (n / 2) + j;
                    G[i, j] = tmp[i, j] + 2;
                }
            }
            for (int k = 0; k < 3; k++)
            {
                G = Rotate(G, n, false);
                for (int i = 0; i < n / 2; i++)
                {
                    for (int j = 0; j < n / 2; j++)
                        G[i, j] = tmp[i, j] + 2;
                }
            }
            Random random = new Random();
            int r;
            int c;
            int end; // количество "дырок"
            if (trash) end = random.Next(1, (n / 2 * n / 2 + 1));
            else end = (n / 2) * (n / 2);
            for (int k = 0; k < end; k++)
            {
                r = random.Next(0, 4); // выбор одного из подквадратов
                c = 0; // нужный подквадрат
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (G[i, j] == (k + 2))
                        {
                            if (r == c)
                            {
                                G[i, j] = 1;
                                c++;
                            }
                            else
                            {
                                G[i, j] = 0;
                                c++;
                            }
                        }
                    }
                }
            }
            int pr = 0; // число нулей в решетке для случая "с мусором"
            if (trash)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (G[i, j] != 1 & G[i, j] != 0)
                            G[i, j] = 0;
                        if (G[i, j] == 0) pr++;
                    }
                }
                if (pr == n * n) CreateGrille(n, trash);
            }         
            return G;
        }

        public static int[,] Rotate(int[,] G, int n, bool isClockwise)
        {
            int[,] resG = new int[n, n];
            if (isClockwise)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                        resG[i, j] = G[n - 1 - j, i];
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                        resG[n - 1 - j, i] = G[i, j];
                }
            }
            return resG;
        }

        public static string Encrypt(string msg, int[,] G, int n, bool trash, bool isClockwise)
        {
            string result = "";
            char[,] T = new char[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    T[i, j] = (char)0;
            }
            int c = 0; // обработанные символы строки
            for (int k = 0; k < 4; k++) // повороты решетки
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (G[i, j] == 1)
                        {
                            if (c < msg.Length)
                            {
                                T[i, j] = msg[c];
                                c++;
                            }
                            else break;
                        }
                    }
                }
                G = Rotate(G, n, isClockwise);
            }
            if (trash)
            {
                Random random = new Random();
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (T[i, j] == 0)
                            T[i, j] = inputTB[random.Next(0, inputTB.Length)];
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    result += T[i, j];
            }
            return result;
        }

        public static string Decrypt(string msg, int[,] G, int n, bool isClockwise)
        {
            string result = "";
            char[,] T = new char[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    T[i, j] = (char)0;
            }
            int c = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    T[i, j] = msg[c];
                    c++;
                }
            }
            for (int k = 0; k < 4; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (G[i, j] == 1)
                            result += T[i, j];
                    }
                }
                G = Rotate(G, n, isClockwise);
            }
            return result;
        }

        public static bool CheckNumberCB(string n)
        {
            string pattern = "0123456789";
            if (n[0] == '0') return false;
            if ((Convert.ToInt32(n) % 2) == 0)
            {
                int cnt = 0;
                foreach (char c in n)
                {
                    foreach (char p in pattern)
                    {
                        if (c == p)
                        {
                            cnt++;
                            break;
                        }
                    }
                }
                if (cnt == n.Length) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        private void CardanEncrypBtn_Click(object sender, EventArgs e)
        {
            if (CheckNumberCB(NumberCB.Text))
            {
                inputTB = InputTB.Text.Replace("\r\n", "\r");
                string result = "";
                int demension = Convert.ToInt32(NumberCB.Text) * Convert.ToInt32(NumberCB.Text);
                if (!TrashCkB.Checked)
                {
                    if (inputTB.Length >= demension)
                    {
                        int counter = inputTB.Length / demension; // число циклов кодирования "без мусора"
                        if (!saveGrilleCB.Checked)
                            G = CreateGrille(Convert.ToInt32(NumberCB.Text), TrashCkB.Checked);
                        string part_Input;
                        for (int c = 0; c < counter; c++)
                        {
                            part_Input = inputTB.Substring(c* demension, demension);
                            result += Encrypt(part_Input, G, Convert.ToInt32(NumberCB.Text), TrashCkB.Checked, true);
                        }
                        if ((inputTB.Length % demension) != 0)
                        {
                            withTrash = true;
                            part_Input = inputTB.Substring(demension * counter);
                            result += Encrypt(part_Input, G, Convert.ToInt32(NumberCB.Text), true, true);
                        }
                        OutputTB.Text = result.Replace("\r", "\r\n");
                    }
                    else
                    {
                        if (!saveGrilleCB.Checked)
                            G = CreateGrille(Convert.ToInt32(NumberCB.Text), TrashCkB.Checked);
                        OutputTB.Text = Encrypt(InputTB.Text, G, Convert.ToInt32(NumberCB.Text), true, true);
                    }
                }
                else
                {
                    if (InputTB.Text.Length <= Convert.ToInt32(NumberCB.Text) * Convert.ToInt32(NumberCB.Text))
                    {
                        if (!saveGrilleCB.Checked)
                            G = CreateGrille(Convert.ToInt32(NumberCB.Text), TrashCkB.Checked);
                        OutputTB.Text = Encrypt(InputTB.Text, G, Convert.ToInt32(NumberCB.Text), TrashCkB.Checked, true);
                    }
                    else MessageBox.Show("Длина сообщения превышает размерность решетки", "Ошибка", MessageBoxButtons.OK);
                }
            }
            else MessageBox.Show("Неверная размерность решетки", "Ошибка", MessageBoxButtons.OK);
        }

        private void CardanDecrypBtn_Click(object sender, EventArgs e)
        {
            string decrypted = InputTB.Text.Replace("\r\n", "\r");
            string result = "";
            int demension = Convert.ToInt32(NumberCB.Text) * Convert.ToInt32(NumberCB.Text);

            int counter = (decrypted.Length / demension); // число циклов кодирования "без мусора"
            if (withTrash) counter--;

            string part_Input;
            for (int c = 0; c < counter; c++)
            {
                part_Input = decrypted.Substring(c * demension, demension);
                result += Decrypt(part_Input, G, Convert.ToInt32(NumberCB.Text), true);
            }
            if (withTrash)
            {
                part_Input = decrypted.Substring(demension * counter);
                result += Decrypt(part_Input, G, Convert.ToInt32(NumberCB.Text), true);
            }
            OutputTB.Text = result.Replace("\r", "\r\n");
        }

        private void ArrowBtn_Click(object sender, EventArgs e)
        {
            if (OutputTB.Text != "")
            {
                InputTB.Clear();
                InputTB.Text = OutputTB.Text;
                OutputTB.Clear();
            }
        }

        private void Show_Grille_Label_Click(object sender, EventArgs e)
        {
            string gr = "";
            for (int i = 0; i < G.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < G.GetUpperBound(0) + 1; j++)
                {
                    gr += G[i,j].ToString() + ' ';
                }
                gr += "\r\n";
            }
            MessageBox.Show(gr, "Информация", MessageBoxButtons.OK);
        }
    }
}
