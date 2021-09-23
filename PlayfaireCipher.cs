using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace AtbashCipher
{
    public partial class PlayfaireCipher : Form
    {
        public PlayfaireCipher()
        {
            InitializeComponent();

        }

        static string enAlLo = "abcdefghiklmnopqrstuvwxyz";
        static string ruAlLo = "абвгдежзийклмнопрстуфхцчшщъыьэюя";
        int dimenAlRow; // строки
        int dimenAlCol; // столбцы
        char[,] matrix;
        bool lang; // true - en, false - ru
        string curAlpha;
        char specChar;

        class Symbol
        {
            public char Value { get; set; }
            public bool FromAlpa { get; set; }
            public int[] Dimension { get; set; }
        }

        List<Symbol> symbols;

        public static string Remove_duplicates(string str)
        {
            return new string(str.ToCharArray().Distinct().ToArray());
        }

        public char[,] Get_matrix(string key)
        {
            int counter = 0;
            key = Remove_duplicates(key + curAlpha);

            matrix = new char[dimenAlRow, dimenAlCol];
            for (int i = 0; i < dimenAlRow; i++)
            {
                for (int j = 0; j < dimenAlCol; j++)
                {
                    matrix[i, j] = key[counter];
                    counter++;
                }
            }
            return matrix;
        }

        private List<Symbol> Get_message(string message)
        {
            symbols = new List<Symbol>();
            for (int i = 0; i < message.Length; i++)
            {
                symbols.Add(new Symbol { Value = message[i], FromAlpa = false });
            }
            int numLetters = 0;
            int lastLetters = -1;
            for (int i = 0; i < symbols.Count; i++)
            {
                if (curAlpha.IndexOf(char.ToLower(symbols[i].Value)) >= 0)
                {
                    symbols[i].FromAlpa = true;
                    numLetters++;
                    lastLetters = i;
                    for (int k = i + 1; k < symbols.Count; k++)
                    {
                        if (curAlpha.IndexOf(char.ToLower(symbols[k].Value)) >= 0)
                        {
                            if (char.ToLower(symbols[i].Value) == char.ToLower(symbols[k].Value))
                            {
                                if (numLetters % 2 != 0)
                                {
                                    if (char.IsLower(symbols[i].Value))
                                        symbols.Insert(i + 1, new Symbol { Value = specChar, FromAlpa = true });
                                    else
                                        symbols.Insert(i + 1, new Symbol { Value = char.ToUpper(specChar), FromAlpa = true });
                                    symbols.Insert(i + 2, new Symbol { Value = (char)129, FromAlpa = false });
                                }
                            }
                            k = symbols.Count;
                        }
                    }
                }
            }
            if (numLetters % 2 != 0 && lastLetters != -1)
            {
                if (char.IsLower(symbols[lastLetters].Value))
                    symbols.Insert(lastLetters + 1, new Symbol { Value = specChar, FromAlpa = true });
                else
                    symbols.Insert(lastLetters + 1, new Symbol { Value = char.ToUpper(specChar), FromAlpa = true });
                symbols.Insert(lastLetters + 2, new Symbol { Value = (char)129, FromAlpa = false });
            }

            return symbols;
        }

        public int[] Get_dimensions(char curChar)
        {
            int[] dimension = new int[2];

            for (int i = 0; i < dimenAlRow; i++)
            {
                for (int j = 0; j < dimenAlCol; j++)
                {
                    if (matrix[i, j] == char.ToLower(curChar))
                    {
                        dimension[0] = i;
                        dimension[1] = j;
                        break;
                    }
                }
            }
            return dimension;
        }

        public string EncryptMsg(string KEY, string MSG)
        {
            if (lang) // en
            {
                curAlpha = enAlLo;
                dimenAlRow = (int)Math.Sqrt(enAlLo.Length);
                dimenAlCol = (int)Math.Sqrt(enAlLo.Length);
                MSG = MSG.Replace("\r\n", "\r").Replace("j", "i").Replace("J", "I");
                specChar = 'x';
            }
            else // ru
            {
                curAlpha = ruAlLo;
                dimenAlRow = 4;
                dimenAlCol = 8;
                MSG = MSG.Replace("\r\n", "\r").Replace("ё", "е").Replace("Ё", "Е");
                specChar = 'х';
            }

            string cipherMsg = "";
            List<Symbol> listMsg = Get_message(MSG);
            char[,] cipherMatrix = Get_matrix(KEY);
            Symbol c1 = new Symbol();
            Symbol c2 = new Symbol();
            for (int curI = 0; curI < listMsg.Count; curI++)
            {
                int oldI = curI;
                if (listMsg[curI].FromAlpa)
                {
                    c1.Value = listMsg[curI].Value;
                    c1.Dimension = Get_dimensions(c1.Value);
                    for (int k = curI + 1; k < listMsg.Count; k++)
                    {
                        if (listMsg[k].FromAlpa)
                        {
                            c2.Value = listMsg[k].Value;
                            c2.Dimension = Get_dimensions(c2.Value);
                            curI = k;
                            k = listMsg.Count;
                        }
                    }
                    if (c1.Dimension[0] == c2.Dimension[0]) // одинаковая строка
                    {
                        c1.Dimension[1] = (c1.Dimension[1] + 1) % dimenAlCol;
                        c2.Dimension[1] = (c2.Dimension[1] + 1) % dimenAlCol;
                    }
                    else if (c1.Dimension[1] == c2.Dimension[1]) // одинаковый столбец
                    {
                        c1.Dimension[0] = (c1.Dimension[0] + 1) % dimenAlRow;
                        c2.Dimension[0] = (c2.Dimension[0] + 1) % dimenAlRow;
                    }
                    else
                    {
                        int temp = c1.Dimension[1];
                        c1.Dimension[1] = c2.Dimension[1];
                        c2.Dimension[1] = temp;
                    }

                    if (char.IsLower(c1.Value))
                    {
                        cipherMsg = cipherMsg + cipherMatrix[c1.Dimension[0], c1.Dimension[1]];
                    }
                    else
                        cipherMsg = cipherMsg + char.ToUpper(cipherMatrix[c1.Dimension[0], c1.Dimension[1]]);
                    if ((curI - oldI) > 1)
                    {
                        for (int n = oldI + 1; n < curI; n++)
                        {
                            cipherMsg = cipherMsg + listMsg[n].Value;
                        }
                    }
                    if (char.IsLower(c2.Value))
                    {
                        cipherMsg = cipherMsg + cipherMatrix[c2.Dimension[0], c2.Dimension[1]];
                    }
                    else
                        cipherMsg = cipherMsg + char.ToUpper(cipherMatrix[c2.Dimension[0], c2.Dimension[1]]);
                }
                else
                {
                    cipherMsg = cipherMsg + listMsg[curI].Value;
                }
            }
            return cipherMsg.Replace("\r", "\r\n");
        }

        public string DecryptMsg(string KEY, string MSG)
        {
            if (lang) // en
            {
                curAlpha = enAlLo;
                dimenAlRow = (int)Math.Sqrt(enAlLo.Length);
                dimenAlCol = (int)Math.Sqrt(enAlLo.Length);
                MSG = MSG.Replace("\r\n", "\r").Replace("j", "i").Replace("J", "I");
                specChar = 'x';
            }
            else // ru
            {
                curAlpha = ruAlLo;
                dimenAlRow = 4;
                dimenAlCol = 8;
                MSG = MSG.Replace("\r\n", "\r").Replace("ё", "е").Replace("Ё", "Е");
                specChar = 'х';
            }

            string cipherMsg = "";

            List<Symbol> listMsg = new List<Symbol>();
            for (int i = 0; i < MSG.Length; i++)
            {
                listMsg.Add(new Symbol { Value = MSG[i], FromAlpa = false });
                if (curAlpha.IndexOf(char.ToLower(listMsg[i].Value)) >= 0)
                {
                    listMsg[i].FromAlpa = true;
                }
            }

            char[,] cipherMatrix = Get_matrix(KEY);
            Symbol c1 = new Symbol();
            Symbol c2 = new Symbol();
            for (int curI = 0; curI < listMsg.Count; curI++)
            {
                int oldI = curI;
                if (listMsg[curI].FromAlpa)
                {
                    c1.Value = listMsg[curI].Value;
                    c1.Dimension = Get_dimensions(c1.Value);
                    for (int k = curI + 1; k < listMsg.Count; k++)
                    {
                        if (listMsg[k].FromAlpa)
                        {
                            c2.Value = listMsg[k].Value;
                            c2.Dimension = Get_dimensions(c2.Value);
                            curI = k;
                            k = listMsg.Count;
                        }
                    }
                    if (c1.Dimension[0] == c2.Dimension[0]) // одинаковая строка
                    {
                        c1.Dimension[1] = (c1.Dimension[1] - 1 + dimenAlCol) % dimenAlCol;
                        c2.Dimension[1] = (c2.Dimension[1] - 1 + dimenAlCol) % dimenAlCol;
                    }
                    else if (c1.Dimension[1] == c2.Dimension[1]) // одинаковый столбец
                    {
                        c1.Dimension[0] = (c1.Dimension[0] - 1 + dimenAlRow) % dimenAlRow;
                        c2.Dimension[0] = (c2.Dimension[0] - 1 + dimenAlRow) % dimenAlRow;
                    }
                    else
                    {
                        int temp = c1.Dimension[1];
                        c1.Dimension[1] = c2.Dimension[1];
                        c2.Dimension[1] = temp;
                    }

                    if (char.IsLower(c1.Value))
                    {
                        cipherMsg = cipherMsg + cipherMatrix[c1.Dimension[0], c1.Dimension[1]];
                    }
                    else
                        cipherMsg = cipherMsg + char.ToUpper(cipherMatrix[c1.Dimension[0], c1.Dimension[1]]);

                    if ((curI - oldI) > 1)
                    {
                        for (int n = oldI + 1; n < curI; n++)
                        {
                            cipherMsg = cipherMsg + listMsg[n].Value;
                        }
                    }

                    if (curI == (listMsg.Count - 1) || listMsg[curI + 1].Value != (char)129) 
                    {
                        if (char.IsLower(c2.Value))
                        {
                            cipherMsg = cipherMsg + cipherMatrix[c2.Dimension[0], c2.Dimension[1]];
                        }
                        else
                            cipherMsg = cipherMsg + char.ToUpper(cipherMatrix[c2.Dimension[0], c2.Dimension[1]]);
                    }
                }
                else if (listMsg[curI].Value != (char)129)
                {
                    cipherMsg = cipherMsg + listMsg[curI].Value;
                }
            }
            return cipherMsg.Replace("\r", "\r\n");
        }

        public string DefineKeyLang(string key)
        {
            key = key.Replace("j", "i").Replace("J", "I").ToLower();
            string cipherKey = "";
            int ru = 0;
            int en = 0;
            for (int i = 0; i < key.Length; i++)
            {
                if (key[i] != ' ')
                {
                    if (enAlLo.IndexOf(key[i]) >= 0) en++;
                    else if (ruAlLo.IndexOf(key[i]) >= 0) ru++;
                    cipherKey = cipherKey + key[i];
                }
            }
            if (en == cipherKey.Length && ru == 0) lang = true;
            else if (ru == cipherKey.Length && en == 0) lang = false;
            else if (ru != 0 && en != 0) return null;

            return cipherKey;
        }

        private void PlayEncrypBtn_Click(object sender, EventArgs e)
        {
            string redKey = DefineKeyLang(KeyTB.Text);
            if (redKey != null)
            {
                OutputTB.Text = EncryptMsg(redKey, InputTB.Text);
            }
            else
            {
                MessageBox.Show("Невозможно определить язык ключа", "Ошибка", MessageBoxButtons.OK);
                KeyTB.Clear();
            }
        }

        private void PlayDecrypBtn_Click(object sender, EventArgs e)
        {
            string redKey = DefineKeyLang(KeyTB.Text);
            if (redKey != null)
            {
                OutputTB.Text = DecryptMsg(redKey, InputTB.Text);
            }
            else
            {
                MessageBox.Show("Невозможно определить язык ключа", "Ошибка", MessageBoxButtons.OK);
                KeyTB.Clear();
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
            if (!(char.IsLetter(curChar)) && curChar != (char)8 && curChar != ' ')
                e.Handled = true;
        }

        private void ShowCipherKeyLabel_Click(object sender, EventArgs e)
        {
            if (matrix != null)
            {
                string matr = "";
                for (int i = 0; i < dimenAlRow; i++)
                {
                    for (int j = 0; j < dimenAlCol; j++)
                    {
                        matr += matrix[i, j].ToString() + '\t';
                    }
                    matr += "\r\n";
                }
                MessageBox.Show(matr, "Информация", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("Матрица не создана", "Информация", MessageBoxButtons.OK);
        }

        private void OpenFileStripMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text file|*.txt";
            openFile.DefaultExt = ".txt";
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(openFile.FileName))
                {
                    InputTB.Clear();
                    InputTB.Text = sr.ReadToEnd();
                }
            }
        }

        private void SaveFileStripMenu_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = ".txt";
            saveFile.Filter = "Text file|*.txt";
            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFile.FileName.Length > 0)
            {
                using(StreamWriter sw = new StreamWriter(saveFile.FileName, false)) // перезапись файла
                {
                    sw.WriteLine(OutputTB.Text);
                    sw.Close();
                }
            }
        }
    }
}
