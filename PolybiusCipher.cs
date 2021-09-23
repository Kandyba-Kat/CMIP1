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
    public partial class PolybiusCipher : Form
    {
        public PolybiusCipher()
        {
            InitializeComponent();
        }

        public static char[,] enAlpaUp = { { 'A', 'B', 'C', 'D', 'E' },
                                           { 'F', 'G', 'H', 'I', 'K' },
                                           { 'L', 'M', 'N', 'O', 'P' },
                                           { 'Q', 'R', 'S', 'T', 'U' },
                                           { 'V', 'W', 'X', 'Y', 'Z' } };

        public static char[,] enAlpaLo = { { 'a', 'b', 'c', 'd', 'e' },
                                           { 'f', 'g', 'h', 'i', 'k' },
                                           { 'l', 'm', 'n', 'o', 'p' },
                                           { 'q', 'r', 's', 't', 'u' },
                                           { 'v', 'w', 'x', 'y', 'z' } };

        public static char[][] ruAlpaUp = new char[6][]
        {
            new char[] { 'А', 'Б', 'В', 'Г', 'Д', 'Е' },
            new char[] { 'Ё', 'Ж', 'З', 'И', 'Й', 'К' },
            new char[] { 'Л', 'М', 'Н', 'О', 'П', 'Р' },
            new char[] { 'С', 'Т', 'У', 'Ф', 'Х', 'Ц' },
            new char[] { 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь' },
            new char[] { 'Э', 'Ю', 'Я', (char)129, (char)149, (char)152 }
        };

        public static char[][] ruAlpaLo = new char[6][]
        {
            new char[] { 'а', 'б', 'в', 'г', 'д', 'е' },
            new char[] { 'ё', 'ж', 'з', 'и', 'й', 'к' },
            new char[] { 'л', 'м', 'н', 'о', 'п', 'р' },
            new char[] { 'с', 'т', 'у', 'ф', 'х', 'ц' },
            new char[] { 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь' },
            new char[] { 'э', 'ю', 'я', (char)141, (char)143, (char)157 }
        };

        public static string Split_into_words(string input, bool btn)
        {
            string result = "";
            string word = "";
            bool f;
            for (int k = 0; k < input.Length; k++)
            {
                f = false;
                for (int i = 0; i < enAlpaUp.GetUpperBound(1) + 1; i++)
                {
                    for (int j = 0; j < enAlpaUp.GetUpperBound(0) + 1; j++)
                    {
                        if (input[k] == enAlpaUp[i, j] || input[k] == enAlpaLo[i, j])
                        {
                            word += input[k];
                            f = true;
                            break;
                        }
                    }
                }
                for (int i = 0; i < ruAlpaUp.Length; i++)
                {
                    for (int j = 0; j < ruAlpaUp[i].Length; j++)
                    {
                        if (input[k] == ruAlpaUp[i][j] || input[k] == ruAlpaLo[i][j])
                        {
                            word += input[k];
                            f = true;
                            break;
                        }
                    }
                }
                if (!f)
                {
                    if (btn) result += Polybius_Cipher_Encode(word);
                    else result += Polybius_Cipher_Decode(word);
                    word = "";
                    result += input[k];
                }
                if (k == input.Length - 1)
                {
                    if (btn) result += Polybius_Cipher_Encode(word);
                    else result += Polybius_Cipher_Decode(word);
                }
            }
            return result;
        }

        public static string Polybius_Cipher_Encode(string input)
        {
            string result = "";
            int len = input.Length;
            int[] x = new int[len]; // горизонталь
            int[] y = new int[len]; // вертикаль
            int[] z = new int[2 * len];
            int[] cha = new int[len];
            int numCha = 0;
            for (int k = 0; k < len; k++) // символы входной строки
            {
                for (int i = 0; i < enAlpaUp.GetUpperBound(1) + 1; i++) // строки
                {
                    for (int j = 0; j < enAlpaUp.GetUpperBound(0) + 1; j++) // столбцы
                    {
                        if (input[k] == enAlpaUp[i, j] || input[k] == 'J')
                        {
                            if (input[k] != 'J')
                            {
                                x[k] = j;
                                y[k] = i;
                            }
                            else // символ J принимаем за I
                            {
                                x[k] = 3;
                                y[k] = 1;
                            }
                            cha[numCha] = 1;
                            numCha++;
                            break;
                        }
                        else if (input[k] == enAlpaLo[i, j] || input[k] == 'j')
                        {
                            if (input[k] != 'j')
                            {
                                x[k] = j;
                                y[k] = i;
                            }
                            else // символ j принимаем за i
                            {
                                x[k] = 3;
                                y[k] = 1;
                            }
                            cha[numCha] = 2;
                            numCha++;
                            break;
                        }
                    }
                }

                for (int i = 0; i < ruAlpaUp.Length; i++)
                {
                    for (int j = 0; j < ruAlpaUp[i].Length; j++)
                    {
                        if (input[k] == ruAlpaUp[i][j])
                        {
                            x[k] = j;
                            y[k] = i;
                            cha[numCha] = 3;
                            numCha++;
                            break;
                        }
                        else if (input[k] == ruAlpaLo[i][j])
                        {
                            x[k] = j;
                            y[k] = i;
                            cha[numCha] = 4;
                            numCha++;
                            break;
                        }
                    }
                }
            }

            for (int k = 0; k < len; k++)
                z[k] = x[k];
            for (int k = len; k < 2 * len; k++)
                z[k] = y[k - len];

            for (int k = 0; k < len; k++)
            {
                x[k] = z[k * 2];
                y[k] = z[k * 2 + 1];
            }

            for (int k = 0; k < len; k++)
            {
                switch (cha[k])
                {
                    case 1:
                        result += enAlpaUp[y[k], x[k]];
                        break;
                    case 2:
                        result += enAlpaLo[y[k], x[k]];
                        break;
                    case 3:
                        result += ruAlpaUp[y[k]][x[k]];
                        break;
                    case 4:
                        result += ruAlpaLo[y[k]][x[k]];
                        break;
                }
            }

            return result;
        }

        public static string Polybius_Cipher_Decode(string encodedStr)
        {
            string resultDecode = "";
            int len = encodedStr.Length;
            int[] z = new int[2 * len];
            int[] x = new int[len]; // горизонталь
            int[] y = new int[len]; // вертикаль
            int[] cha = new int[len];
            int numCha = 0;

            for (int k = 0; k < len; k++)
            {
                for (int i = 0; i < enAlpaUp.GetUpperBound(1) + 1; i++)
                {
                    for (int j = 0; j < enAlpaUp.GetUpperBound(0) + 1; j++)
                    {
                        if (encodedStr[k] == enAlpaUp[i, j])
                        {
                            z[2 * k] = j;
                            z[2 * k + 1] = i;
                            cha[numCha] = 1;
                            numCha++;
                            break;
                        }
                        else if (encodedStr[k] == enAlpaLo[i, j])
                        {
                            z[2 * k] = j;
                            z[2 * k + 1] = i;
                            cha[numCha] = 2;
                            numCha++;
                            break;
                        }
                    }
                }

                for (int i = 0; i < ruAlpaUp.Length; i++)
                {
                    for (int j = 0; j < ruAlpaUp[i].Length; j++)
                    {
                        if (encodedStr[k] == ruAlpaUp[i][j])
                        {
                            z[2 * k] = j;
                            z[2 * k + 1] = i;
                            cha[numCha] = 3;
                            numCha++;
                            break;
                        }
                        else if (encodedStr[k] == ruAlpaLo[i][j])
                        {
                            z[2 * k] = j;
                            z[2 * k + 1] = i;
                            cha[numCha] = 4;
                            numCha++;
                            break;
                        }
                    }
                }
            }

            for (int k = 0; k < len; k++)
                x[k] = z[k];
            for (int k = len; k < 2 * len; k++)
                y[k - len] = z[k];

            for (int k = 0; k < len; k++)
            {
                switch (cha[k])
                {
                    case 1:
                        resultDecode += enAlpaUp[y[k], x[k]];
                        break;
                    case 2:
                        resultDecode += enAlpaLo[y[k], x[k]];
                        break;
                    case 3:
                        resultDecode += ruAlpaUp[y[k]][x[k]];
                        break;
                    case 4:
                        resultDecode += ruAlpaLo[y[k]][x[k]];
                        break;
                }
            }
            return resultDecode;
        }

        private void PolybEncrypBtn_Click(object sender, EventArgs e)
        {
            OutputTB.Text = Split_into_words(InputTB.Text, true);
        }

        private void PolybDecrypBtn_Click(object sender, EventArgs e)
        {
            OutputTB.Text = Split_into_words(InputTB.Text, false);
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
