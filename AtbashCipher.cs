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
    public partial class AtbashCipher : Form
    {
        public AtbashCipher()
        {
            InitializeComponent();
        }

        private void AtbashEncrypBtn_Click(object sender, EventArgs e)
        {            
            OutputTB.Text = Atbash_Cipher(InputTB.Text);
        }

        public static string Atbash_Cipher(string input)
        {
            string result = "";
            string enAlpaUp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string enAlpaLo = "abcdefghijklmnopqrstuvwxyz";
            string ruAlpaUp = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            string ruAlpaLo = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            foreach (char x in input)
            {
                bool f = false;
                for (int i = 0; i < ruAlpaUp.Length; i++)
                {
                    if (i < enAlpaUp.Length)
                    {
                        if (x == enAlpaUp[i])
                        {
                            result += enAlpaUp[enAlpaUp.Length - i - 1];
                            f = true;
                            break;
                        }
                        else if (x == enAlpaLo[i])
                        {
                            result += enAlpaLo[enAlpaUp.Length - i - 1];
                            f = true;
                            break;
                        }
                    }
                    if (x == ruAlpaUp[i])
                    {
                        result += ruAlpaUp[ruAlpaUp.Length - i - 1];
                        f = true;
                        break;
                    }
                    else if (x == ruAlpaLo[i])
                    {
                        result += ruAlpaLo[ruAlpaLo.Length - i - 1];
                        f = true;
                        break;
                    }
                } 
                if (!f)
                {
                    result += x;
                }
            }
            return result;
        }

        private void InputTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool check = false;
            string permittedSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyzАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789.,!?-:;()\" ";
            for (int i = 0; i < permittedSymbols.Length; i++)
            {
                if (e.KeyChar == permittedSymbols[i] || e.KeyChar == (char)Keys.Back || ModifierKeys == Keys.Control || e.KeyChar == (char)Keys.Enter)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                e.Handled = true;
                MessageBox.Show(
                "Недопустимый символ",
                "Ошибка",
                MessageBoxButtons.OK);
            }
        }

        private void AtbashDecrypBtn_Click(object sender, EventArgs e)
        {
            OutputTB.Text = Atbash_Cipher(InputTB.Text);
        }
    }
}
