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
    public partial class CaesarCipher : Form
    {
        public CaesarCipher()
        {
            InitializeComponent();
        }

        private void shiftTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool check = false;
            string permittedSymbols = "0123456789-";
            for (int i = 0; i < permittedSymbols.Length; i++)
            {
                if (e.KeyChar == permittedSymbols[i] || e.KeyChar == (char)Keys.Back)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                e.Handled = true;
            }
        }

        private void CaesarEncrypBtn_Click(object sender, EventArgs e)
        {
            if (shiftTB.Text == String.Empty)
            {
                MessageBox.Show("Обязательное поле не заполнено", "Ошибка", MessageBoxButtons.OK);
            }
            else
            {
                OutputTB.Text = Caesar_Cipher(InputTB.Text, Convert.ToInt32(shiftTB.Text), true);
            }
        }

        public static string Caesar_Cipher(string input, int n, bool btn)
        {
            string result = "";
            string enAlpaUp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string enAlpaLo = "abcdefghijklmnopqrstuvwxyz";
            string ruAlpaUp = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            string ruAlpaLo = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            int shift;
            foreach (char x in input)
            {
                shift = 0;
                bool f = false;
                for (int i = 0; i < ruAlpaUp.Length; i++)
                {
                    if (i < enAlpaUp.Length)
                    {
                        if (x == enAlpaUp[i])
                        {
                            shift = CheckBtn(btn, n, i, enAlpaUp.Length);

                            result += enAlpaUp[shift];
                            f = true;
                            break;
                        }
                        else if (x == enAlpaLo[i])
                        {
                            shift = CheckBtn(btn, n, i, enAlpaLo.Length);

                            result += enAlpaLo[shift];
                            f = true;
                            break;
                        }
                    }
                    if (x == ruAlpaUp[i])
                    {
                        shift = CheckBtn(btn, n, i, ruAlpaUp.Length);

                        result += ruAlpaUp[shift];
                        f = true;
                        break;
                    }
                    else if (x == ruAlpaLo[i])
                    {
                        shift = CheckBtn(btn, n, i, ruAlpaLo.Length);

                        result += ruAlpaLo[shift];
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

        public static int CheckBtn(bool b, int n, int i, int len)
        {
            int s = 0;
            if (b) 
            {
                if (n < 0)
                {
                    s = (i + len + n) % len;
                    if (s < 0)
                    {
                        s = len + s;
                    }
                }
                else
                {
                    s = (i + n) % len;
                }
            }
            else
            { s = (i - n) % len; }
            return s;
        }

        private void CaesarDecrypBtn_Click(object sender, EventArgs e)
        {
            if (shiftTB.Text == String.Empty)
            {
                MessageBox.Show("Обязательное поле не заполнено", "Ошибка", MessageBoxButtons.OK);
            }
            else
            {
                OutputTB.Text = Caesar_Cipher(InputTB.Text, Convert.ToInt32(shiftTB.Text), false);
            }
        }
    }
}
