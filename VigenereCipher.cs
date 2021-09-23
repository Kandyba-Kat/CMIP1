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
    public partial class VigenereCipher : Form
    {
        public VigenereCipher()
        {
            InitializeComponent();
        }

        static string enAlLo = "abcdefghijklmnopqrstuvwxyz";
        static string enAlUp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string ruAlLo = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        static string ruAlUp = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

        public static string Key_Check(string key)
        {
            var tmp_key = Regex.Replace(key, "[^a-zA-zа-яА-ЯёЁ]", "");
            return tmp_key;
        }

        public static int Key_Char_to_Int(char curChar)
        {
            int corNum = -1; // corresponding number for the current char
            bool ru = false;

            for (int curInd = 0; curInd < ruAlLo.Length; curInd++)
            {
                if (curInd < enAlLo.Length)
                {
                    if (curChar == enAlLo[curInd])
                    {
                        corNum = curInd;
                        break;
                    }
                    else if (curChar == enAlUp[curInd])
                    {
                        corNum = curInd;
                        break;
                    }
                }
                if (curChar == ruAlLo[curInd])
                {
                    corNum = curInd;
                    ru = true;
                    break;
                }
                else if (curChar == ruAlUp[curInd])
                {
                    corNum = curInd;
                    ru = true;
                    break;
                }
            }

            if (ru)
                corNum = corNum % ruAlLo.Length;
            else if (corNum != -1)
                corNum = corNum % enAlLo.Length;
            return corNum;
        }

        public static string Vigenere_Cipher_Encode(string input, string key)
        {
            input = input.Replace("\r\n", "\r");
            char[] initialText = input.ToCharArray();         

            int keyPos = 0;
            for (int curChaInd = 0; curChaInd < initialText.Length; curChaInd++)
            {
                int curShift = Key_Char_to_Int(key[keyPos]);

                for (int alInd = 0; alInd < ruAlLo.Length; alInd++)
                {
                    if (alInd < enAlLo.Length)
                    {
                        if (initialText[curChaInd] == enAlLo[alInd])
                        {
                            initialText[curChaInd] = enAlLo[(alInd + curShift) % enAlLo.Length];
                            keyPos++;
                            break;
                        }
                        else if (initialText[curChaInd] == enAlUp[alInd])
                        {
                            initialText[curChaInd] = enAlUp[(alInd + curShift) % enAlUp.Length];
                            keyPos++;
                            break;
                        }
                    }
                    if (initialText[curChaInd] == ruAlLo[alInd])
                    {
                        initialText[curChaInd] = ruAlLo[(alInd + curShift) % ruAlLo.Length];
                        keyPos++;
                        break;
                    }
                    else if (initialText[curChaInd] == ruAlUp[alInd])
                    {
                        initialText[curChaInd] = ruAlUp[(alInd + curShift) % ruAlUp.Length];
                        keyPos++;
                        break;
                    }
                }

                if (keyPos == key.Length)
                    keyPos = 0;
            }

            return new string(initialText).Replace("\r", "\r\n");
        }

        public static string Vigenere_Cipher_Decode(string input, string key)
        {
            input = input.Replace("\r\n", "\r");
            char[] initialText = input.ToCharArray();

            int keyPos = 0;
            for (int curChaInd = 0; curChaInd < initialText.Length; curChaInd++)
            {
                int curShift = Key_Char_to_Int(key[keyPos]);

                for (int alInd = 0; alInd < ruAlLo.Length; alInd++)
                {
                    if (alInd < enAlLo.Length)
                    {
                        if (initialText[curChaInd] == enAlLo[alInd])
                        {
                            initialText[curChaInd] = enAlLo[(alInd - (curShift % enAlLo.Length) + enAlLo.Length) % enAlLo.Length];
                            keyPos++;
                            break;
                        }
                        else if (initialText[curChaInd] == enAlUp[alInd])
                        {
                            initialText[curChaInd] = enAlUp[(alInd - (curShift % enAlUp.Length) + enAlUp.Length) % enAlUp.Length];
                            keyPos++;
                            break;
                        }
                    }
                    if (initialText[curChaInd] == ruAlLo[alInd])
                    {
                        initialText[curChaInd] = ruAlLo[(alInd - curShift + ruAlLo.Length) % ruAlLo.Length];
                        keyPos++;
                        break;
                    }
                    else if (initialText[curChaInd] == ruAlUp[alInd])
                    {
                        initialText[curChaInd] = ruAlUp[(alInd - curShift + ruAlUp.Length) % ruAlUp.Length];
                        keyPos++;
                        break;
                    }
                }

                if (keyPos == key.Length)
                    keyPos = 0;
            }

            return new string(initialText).Replace("\r", "\r\n");
        }

        private void VigEncrypBtn_Click(object sender, EventArgs e)
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
                        OutputTB.Text = Vigenere_Cipher_Encode(InputTB.Text, tmp_key);
                    }
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить кодирование!\nВведите ключевое слово", "Ошибка", MessageBoxButtons.OK);
            }
        }

        private void VigDecrypBtn_Click(object sender, EventArgs e)
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
                        OutputTB.Text = Vigenere_Cipher_Decode(InputTB.Text, tmp_key);
                    }
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить кодирование!\nВведите ключевое слово", "Ошибка", MessageBoxButtons.OK);
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

        private void KeyTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            char curChar = e.KeyChar;
            if (!(char.IsLetter(curChar)) && curChar != (char)8)
                e.Handled = true;
        }
    }
}
