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
    public partial class ScytaleCipher : Form
    {
        public ScytaleCipher()
        {
            InitializeComponent();
        }

        private void ScytaleEncrypBtn_Click(object sender, EventArgs e)
        {
            if (numOfRowsTB.Text == String.Empty)
            {
                MessageBox.Show("Обязательное поле не заполнено", "Ошибка", MessageBoxButtons.OK);
            }
            else
            {
                if (Convert.ToInt32(numOfRowsTB.Text) < 0)
                {
                    MessageBox.Show("Недопустимое значение", "Ошибка", MessageBoxButtons.OK);
                    numOfRowsTB.Clear();
                }
                else
                {
                    OutputTB.Text = Scytale_Cipher_Encode(InputTB.Text, Convert.ToInt32(numOfRowsTB.Text));
                }
            }
        }

        public static string Scytale_Cipher_Encode(string input, int numOfRows)
        {
            string resultEncode = "";
            if (numOfRows >= input.Length || numOfRows <= 0) { return input; }
            else
            {
               
                while(input.Length % numOfRows != 0)
                {
                    input += "*";
                }
                int numOfCols = input.Length / numOfRows;

                for (int i = 0; i < numOfCols; i++)
                {
                    for (int j = i; j < input.Length; j += numOfCols)
                    {
                        resultEncode += input[j];
                    }
                }
            }
            return resultEncode;
        }

        public static string Scytale_Cipher_Decode(string encodedStr, int numOfRows)
        {
            string resultDecode = "";
            int numOfCols = encodedStr.Length / numOfRows;
            resultDecode = Scytale_Cipher_Encode(encodedStr, numOfCols);
            return resultDecode;
        }

        private void numOfRowsTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool check = false;
            string permittedSymbols = "0123456789";
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

        private void ScytaleDecrypBtn_Click(object sender, EventArgs e)
        {
            if (numOfRowsTB.Text == String.Empty)
            {
                MessageBox.Show("Обязательное поле не заполнено", "Ошибка", MessageBoxButtons.OK);
            }
            else
            {
                if (Convert.ToInt32(numOfRowsTB.Text) < 0)
                {
                    MessageBox.Show("Недопустимое значение", "Ошибка", MessageBoxButtons.OK);
                    numOfRowsTB.Clear();
                }
                else
                {
                    //OutputTB.Text = Scytale_Cipher_Decode(InputTB.Text, Convert.ToInt32(numOfRowsTB.Text));
                    string output = Scytale_Cipher_Decode(InputTB.Text, Convert.ToInt32(numOfRowsTB.Text));
                    string tmp = "";
                    foreach (char x in output)
                    {
                        if (x != '*')
                        {
                            tmp += x;
                        }
                    }
                    OutputTB.Text = tmp;
                }
            }          
        }
    }
}
