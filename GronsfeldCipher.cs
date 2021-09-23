using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AtbashCipher
{
    public partial class GronsfeldCipher : Form
    {
        public GronsfeldCipher()
        {
            InitializeComponent();
        }

        static string enAlLo = "abcdefghijklmnopqrstuvwxyz";
        static string enAlUp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string ruAlLo = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        static string ruAlUp = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

        public static string Gronsfeld_Cipher_Encode(string input, string key)
        {
            input = input.Replace("\r\n", "\r");
            char[] initialText = input.ToCharArray();

            int keyPos = 0;
            for (int curChaInd = 0; curChaInd < initialText.Length; curChaInd++)
            {
                int curShift = key[keyPos];

                for (int alInd = 0; alInd < ruAlLo.Length; alInd++)
                {
                    if (alInd < enAlLo.Length)
                    {
                        if (initialText[curChaInd] == enAlLo[alInd])
                        {
                            initialText[curChaInd] = enAlLo[(alInd + (curShift - '0')) % enAlLo.Length];
                            keyPos++;
                            break;
                        }
                        else if (initialText[curChaInd] == enAlUp[alInd])
                        {
                            initialText[curChaInd] = enAlUp[(alInd + (curShift - '0')) % enAlUp.Length];
                            keyPos++;
                            break;
                        }
                    }
                    if (initialText[curChaInd] == ruAlLo[alInd])
                    {
                        initialText[curChaInd] = ruAlLo[(alInd + (curShift - '0')) % ruAlLo.Length];
                        keyPos++;
                        break;
                    }
                    else if (initialText[curChaInd] == ruAlUp[alInd])
                    {
                        initialText[curChaInd] = ruAlUp[(alInd + (curShift - '0')) % ruAlUp.Length];
                        keyPos++;
                        break;
                    }
                }

                if (keyPos == key.Length)
                    keyPos = 0;
            }

            return new string(initialText).Replace("\r", "\r\n");
        }

        public static string Gronsfeld_Cipher_Decode(string input, string key)
        {
            input = input.Replace("\r\n", "\r");
            char[] initialText = input.ToCharArray();

            int keyPos = 0;
            for (int curChaInd = 0; curChaInd < initialText.Length; curChaInd++)
            {
                int curShift = key[keyPos];

                for (int alInd = 0; alInd < ruAlLo.Length; alInd++)
                {
                    if (alInd < enAlLo.Length)
                    {
                        if (initialText[curChaInd] == enAlLo[alInd])
                        {
                            initialText[curChaInd] = enAlLo[(alInd - (curShift - '0') + enAlLo.Length) % enAlLo.Length];
                            keyPos++;
                            break;
                        }
                        else if (initialText[curChaInd] == enAlUp[alInd])
                        {
                            initialText[curChaInd] = enAlUp[(alInd - (curShift - '0') + enAlUp.Length) % enAlUp.Length];
                            keyPos++;
                            break;
                        }
                    }
                    if (initialText[curChaInd] == ruAlLo[alInd])
                    {
                        initialText[curChaInd] = ruAlLo[(alInd - (curShift - '0') + ruAlLo.Length) % ruAlLo.Length];
                        keyPos++;
                        break;
                    }
                    else if (initialText[curChaInd] == ruAlUp[alInd])
                    {
                        initialText[curChaInd] = ruAlUp[(alInd - (curShift - '0') + ruAlUp.Length) % ruAlUp.Length];
                        keyPos++;
                        break;
                    }
                }

                if (keyPos == key.Length)
                    keyPos = 0;
            }
            return new string(initialText).Replace("\r", "\r\n");
        }

        public static string Key_Check(string key)
        {
            var tmp_key = Regex.Replace(key, "[^0-9]", "");
            return tmp_key;
        }

        private void GronEncrypBtn_Click(object sender, EventArgs e)
        {
            if (KeyTB.Text != String.Empty)
            {
                if (InputTB.Text == String.Empty)
                {
                    MessageBox.Show("Кодирование не выполнено!\nВведите текст для кодирования", "Ошибка", MessageBoxButtons.OK);
                }
                else
                {
                    string tmp_key = Key_Check(KeyTB.Text);
                    if (tmp_key.Length == 0)
                    {
                        MessageBox.Show("Некорректный ключ", "Ошибка");
                    }
                    else
                    {
                        if (tmp_key != KeyTB.Text)
                            KeyTB.Text = tmp_key;
                        OutputTB.Text = Gronsfeld_Cipher_Encode(InputTB.Text, tmp_key);
                    }
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить кодирование!\nВведите ключ", "Ошибка", MessageBoxButtons.OK);
            }
        }

        private void GronDecrypBtn_Click(object sender, EventArgs e)
        {
            if (KeyTB.Text != String.Empty)
            {
                if (InputTB.Text == String.Empty)
                {
                    MessageBox.Show("Кодирование не выполнено!\nВведите текст для кодирования", "Ошибка", MessageBoxButtons.OK);
                }
                else
                {
                    string tmp_key = Key_Check(KeyTB.Text);
                    if (tmp_key.Length == 0)
                    {
                        MessageBox.Show("Некорректный ключ", "Ошибка");
                    }
                    else
                    {
                        if (tmp_key != KeyTB.Text)
                            KeyTB.Text = tmp_key;
                        OutputTB.Text = Gronsfeld_Cipher_Decode(InputTB.Text, tmp_key);
                    }
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить кодирование!\nВведите ключ", "Ошибка", MessageBoxButtons.OK);
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
    }
}
