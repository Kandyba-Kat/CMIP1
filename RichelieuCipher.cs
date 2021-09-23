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
    public partial class RichelieuCipher : Form
    {
        public RichelieuCipher()
        {
            InitializeComponent();
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

        public static bool Key_Check(string key)
        {
            string validСha = "1234567890(),";
            if (key[0] != '(') return false;
            short opened_brackets = 1;
            for (int i = 1; i < key.Length; i++)
            {
                if (validСha.IndexOf(key[i]) == -1) return false;
                if (key[i] == ')') opened_brackets--;
                if (key[i] == '(') opened_brackets++;
                if (opened_brackets > 1 || opened_brackets < 0) return false;
            }
            if (opened_brackets > 0) return false;

            string[] segments = key.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < segments.Length; i++)
            {
                var swaps = segments[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                short[] counter = new short[swaps.Length + 1];
                Array.Clear(counter, 0, swaps.Length + 1);
                for (int j = 0; j < swaps.Length; j++)
                {
                    if (Convert.ToInt32(swaps[j]) > swaps.Length) return false;
                    counter[Convert.ToInt32(swaps[j])]++;
                }
                if (counter[0] != 0) return false;
                for (int j = 1; j <= swaps.Length; j++)
                    if (counter[j] != 1) return false;
            }
            return true;
        }

        public static string Rich_Cipher_Enc_Dec(string input, string key, bool enc)
        {
            input = input.Replace("\r\n", "\r");
            string result = "";
            string[] segments = key.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            int curIndex = 0;
            int curSegment = 0;
            while (curIndex < input.Length)
            {
                var swaps = segments[curSegment].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (curIndex + swaps.Length > input.Length)
                {
                    char[] remainder = input.Skip(curIndex).Take(input.Length - curIndex).ToArray();
                    result += new string(remainder);
                    break;
                }
                char[] text = input.Skip(curIndex).Take(swaps.Length).ToArray();
                char[] encrypted = new char[text.Length];
                for (int i = 0; i < swaps.Length; i++)
                {
                    if (enc) encrypted[Convert.ToInt32(swaps[i]) - 1] = text[i]; // кодирование
                    else encrypted[i] = text[Convert.ToInt32(swaps[i]) - 1]; // раскодирование
                }
                result += new string(encrypted);
                curIndex += swaps.Length;
                if (++curSegment == segments.Length)
                {
                    char[] remainder = input.Skip(curIndex).Take(input.Length - curIndex).ToArray();
                    result += new string(remainder);
                    break;
                }
            }
            return result.Replace("\r", "\r\n");
        }

        private void RichEncrypBtn_Click(object sender, EventArgs e)
        {
            if (KeyTB.Text != String.Empty)
            {
                if (InputTB.Text == String.Empty)
                {
                    MessageBox.Show("Кодирование не выполнено!\nВведите текст для кодирования", "Ошибка", MessageBoxButtons.OK);
                }
                else
                {
                    if (!Key_Check(KeyTB.Text))
                    {
                        MessageBox.Show("Некорректный ключ", "Ошибка");
                    }
                    else
                    {
                        OutputTB.Text = Rich_Cipher_Enc_Dec(InputTB.Text, KeyTB.Text, true);
                    }
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить кодирование!\nВведите ключ", "Ошибка", MessageBoxButtons.OK);
            }
        }

        private void KeyTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            char curChar = e.KeyChar;
            string validСha = "1234567890(),";
            if (validСha.IndexOf(curChar) == -1 && curChar != (char)8)
                e.Handled = true;
        }

        private void RichDecrypBtn_Click(object sender, EventArgs e)
        {
            if (KeyTB.Text != String.Empty)
            {
                if (InputTB.Text == String.Empty)
                {
                    MessageBox.Show("Кодирование не выполнено!\nВведите текст для кодирования", "Ошибка", MessageBoxButtons.OK);
                }
                else
                {
                    if (!Key_Check(KeyTB.Text))
                    {
                        MessageBox.Show("Некорректный ключ", "Ошибка");
                    }
                    else
                    {
                        OutputTB.Text = Rich_Cipher_Enc_Dec(InputTB.Text, KeyTB.Text, false);
                    }
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить кодирование!\nВведите ключ", "Ошибка", MessageBoxButtons.OK);
            }
        }

        private void ExampleKey_Click(object sender, EventArgs e)
        {
            KeyTB.Text = "(2,1,3)(5,1,3,2,4)(4,1,3,2)";
            MessageBox.Show("Ключ представляет собой группы символов - круглые скобки, " +
                "в каждой группе символы переставляются в порядке, указанном цифрами - " +
                "цифры через запятую.\nПробелы не допустимы!", "Справка", MessageBoxButtons.OK);
        }
    }
}
