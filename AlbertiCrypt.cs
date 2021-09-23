using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// https://uptosmart.com/magic-letter/ - мешаем буквы в строке онлайн

namespace AtbashCipher
{
    public partial class AlbertiCrypt : Form
    {
        public AlbertiCrypt()
        {
            InitializeComponent();
        }

        public const string OpenAlpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        public const string CloseAlpha_stat = "ёДdiЕvQUЫfцтwОBМЪрKВaюЮOощПШРnpАCEчhNдDЛъszбАЖкZYУXяЬКЙqRуuЩFсjJWbТGMвжtTНмkSIьйзаPСэлБхХгHyпЧcЗфoxЦГФИgЯLЭlrVынmeЁшеи";

        public static string CloseAlpha_current = CloseAlpha_stat;

        public static string Shuffle_OpAlpha(string opAlpha)
        {
            char[] arrOpAlpha = opAlpha.ToCharArray();
            Random rng = new Random();
            int len = arrOpAlpha.Length;
            while (len > 1)
            {
                len--;
                int k = rng.Next(len + 1);
                var value = arrOpAlpha[k];
                arrOpAlpha[k] = arrOpAlpha[len];
                arrOpAlpha[len] = value;
            }
            return new string(arrOpAlpha);
        }

        public static string Alberti_Cipher_Encode(string input, int rotate, int bloLen)
        {
            if (true) {
                input = input.Replace("\r\n", "\r");
                char[] initialText = input.ToCharArray();

                /*for (int curChaInd = 0; curChaInd < initialText.Length; curChaInd++)
                {
                    for (int opAlInd = 0; opAlInd < OpenAlpha.Length; opAlInd++)
                    {
                        if (initialText[curChaInd] == OpenAlpha[opAlInd])
                        {
                            initialText[curChaInd] = CloseAlpha_stat[(opAlInd + (rotate * (curChaInd / bloLen))) % CloseAlpha_stat.Length];
                            break;
                        }
                    }
                }*/
                int curChaInd = 0;
                for (int i = 0; i < initialText.Length; i++)
                {
                    for (int opAlInd = 0; opAlInd < OpenAlpha.Length; opAlInd++)
                    {
                        if (initialText[i] == OpenAlpha[opAlInd])
                        {
                            initialText[i] = CloseAlpha_current[(opAlInd + (rotate * (curChaInd / bloLen))) % CloseAlpha_current.Length];
                            curChaInd++;
                            break;
                        }
                    }
                }
                return new string(initialText).Replace("\r", "\r\n");
            }
            else { }
        }

        public static string Alberti_Cipher_Decode(string input, int rotate, int bloLen)
        {
            input = input.Replace("\r\n", "\r");
            char[] initialText = input.ToCharArray();

            /*for (int curChaInd = 0; curChaInd < initialText.Length; curChaInd++)
            {
                for (int clAlInd = 0; clAlInd < CloseAlpha_stat.Length; clAlInd++)
                {
                    if (initialText[curChaInd] == CloseAlpha_stat[clAlInd])
                    {
                        initialText[curChaInd] = OpenAlpha[(((clAlInd - (rotate * (curChaInd / bloLen))) % OpenAlpha.Length) + OpenAlpha.Length) % OpenAlpha.Length];
                        break;
                    }
                }
            }*/

            int curChaInd = 0;
            for (int i = 0; i < initialText.Length; i++)
            {
                for (int clAlInd = 0; clAlInd < CloseAlpha_current.Length; clAlInd++)
                {
                    if (initialText[i] == CloseAlpha_current[clAlInd])
                    {
                        initialText[i] = OpenAlpha[(((clAlInd - (rotate * (curChaInd / bloLen))) % OpenAlpha.Length) + OpenAlpha.Length) % OpenAlpha.Length];
                        curChaInd++;
                        break;
                    }
                }
            }

            return new string(initialText).Replace("\r", "\r\n");
        }

        private void AlbertiEncrypBtn_Click(object sender, EventArgs e)
        {           
            if (RotateTB.Text != String.Empty)
            {
                if (RotateTB.Text != String.Empty)
                {
                    if (InputTB.Text == String.Empty)
                    {
                        MessageBox.Show("Кодирование не выполнено!\nВведите текст для кодирования", "Ошибка", MessageBoxButtons.OK);
                    }
                    else
                    {
                        if (GenAlChB.Checked)
                            CloseAlpha_current = Shuffle_OpAlpha(OpenAlpha);
                        else CloseAlpha_current = CloseAlpha_stat;
                        OutputTB.Text = Alberti_Cipher_Encode(InputTB.Text, Convert.ToInt32(RotateTB.Text), Convert.ToInt32(BloLenTB.Text));
                    }
                }
                else
                {
                    MessageBox.Show("Невозможно выполнить кодирование!\nВведите длину блока", "Ошибка", MessageBoxButtons.OK);

                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить кодирование!\nВведите поворот диска", "Ошибка", MessageBoxButtons.OK);
            }
        }

        private void AlbertiDecrypBtn_Click(object sender, EventArgs e)
        {
            if (RotateTB.Text != String.Empty)
            {
                if (RotateTB.Text != String.Empty)
                {
                    if (InputTB.Text == String.Empty)
                    {
                        MessageBox.Show("Кодирование не выполнено!\nВведите текст для кодирования", "Ошибка", MessageBoxButtons.OK);
                    }
                    else
                    {
                        OutputTB.Text = Alberti_Cipher_Decode(InputTB.Text, Convert.ToInt32(RotateTB.Text), Convert.ToInt32(BloLenTB.Text));
                    }
                }
                else
                {
                    MessageBox.Show("Невозможно выполнить кодирование!\nВведите длину блока", "Ошибка", MessageBoxButtons.OK);

                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить кодирование!\nВведите поворот диска", "Ошибка", MessageBoxButtons.OK);
            }
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

        private void RotateTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            char curChar = e.KeyChar;
            string pattern = "0123456789";
            e.Handled = true;
            foreach (char p in pattern)
            {

                if (curChar == p)
                {
                    e.Handled = false;
                    break;
                }
            }
            if (curChar == (char)8)
                e.Handled = false;
        }

        private void BloLenTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            char curChar = e.KeyChar;
            string pattern = "0123456789";
            e.Handled = true;
            foreach (char p in pattern)
            {

                if (curChar == p)
                {
                    e.Handled = false;
                    break;
                }
            }
            if (curChar == (char)8)
                e.Handled = false;
        }

        private void ShowCodeAlLabel_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("Открытый ({0}): {1}\nКодовый ({2}): {3}", OpenAlpha.Length, OpenAlpha, CloseAlpha_current.Length, CloseAlpha_current), "Алфавит", MessageBoxButtons.OK);
        }
    }
}
