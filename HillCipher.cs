using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
//using MathNet.Numerics.LinearAlgebra;

namespace AtbashCipher
{
    public partial class HillCipher : Form
    {
        public HillCipher()
        {
            InitializeComponent();
        }

        public string enAlLo = "abcdefghijklmnopqrstuvwxyz.,!' "; // length = 31
        public string enAlUp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ:;?\"^";
        public string ruAlLo = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя.,! "; // length = 37
        public string ruAlUp = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ:;?\"";
        bool lang; // true - en, false - ru
        bool onceRun = false;

        class Symbol
        {
            public char Value { get; set; }
            public bool FromAlpa { get; set; }
            public bool Register { get; set; } // true - lo, false - up
        }

        public string DefineKeyLang(string key) // определение языка + удаление пробелов из ключа
        {
            if (key == "")
            {
                return null;
            }
            else
            {
                key = key.ToLower();
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
                if (cipherKey == "") return null;
                else if (en == cipherKey.Length && ru == 0) lang = true;
                else if (ru == cipherKey.Length && en == 0) lang = false;
                else if (ru != 0 && en != 0) return null;

                return cipherKey;
            }
        } 

        private int CheckSquare(int lenKey)
        {
            if (lenKey == 1 || lenKey == 2)
            {
                return 2;
            }
            else
            {
                if ((Math.Sqrt(lenKey) % 1) == 0)
                {
                    return Convert.ToInt32(Math.Sqrt(lenKey));
                }
                else
                {
                    int intPartLK = Convert.ToInt32(Math.Truncate(Math.Sqrt(lenKey)));
                    var fracPartLK = Math.Sqrt(lenKey) - intPartLK;
                    if (fracPartLK >= 0.5)
                    {
                        intPartLK++;
                    }
                    return intPartLK;
                }
            }
        }

        private string SetLang(bool l, bool r)
        {
            if (l)
            {
                if (r) return enAlLo;                
                else return enAlUp;
            }
            else
            {
                if (r) return ruAlLo;
                else return ruAlUp;
            }
        }

        private string LeadCorrectKey(int diff, string curKey, string alpha)
        {
            Random rand = new Random();
            while (diff != 0)
            {
                if (diff > 0)
                {
                    curKey = curKey + alpha[rand.Next(0, alpha.Length)];
                    diff--;
                }
                else
                {
                    curKey = curKey.Remove(curKey.Length - 1);
                    diff++;
                }
            }
            return curKey;
        }

        private List<Symbol> FillingInList(string source, string alLo, string alUp, int matSize, out int lenMes)
        {
            List<Symbol> msg = new List<Symbol>();
            lenMes = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (alLo.IndexOf(source[i]) >= 0)
                {
                    msg.Add(new Symbol { Value = source[i], FromAlpa = true, Register = true });
                    lenMes++;
                }
                else if (alUp.IndexOf(source[i]) >= 0)
                {
                    msg.Add(new Symbol { Value = source[i], FromAlpa = true, Register = false });
                    lenMes++;
                }
                else
                {
                    msg.Add(new Symbol { Value = source[i], FromAlpa = false });
                }               
            }
            if (lenMes != 0)
            {   // добавление символа пробела при необходимости
                while (lenMes % matSize != 0)
                {
                    msg.Add(new Symbol { Value = ' ', FromAlpa = true, Register = true });
                    lenMes++;
                }
            }
            return msg;
        }

        private string WorkHillCipher(List<Symbol> message, string alLo, string alUp, double[,] keyMtr)
        {
            string result = "";
            int module = alLo.Length;
            int countCrypSym = 0;
            double[] curBlock;
            int noAl1 = 0;
            int noAl2 = 0;
            int matrixSize = keyMtr.GetUpperBound(0) + 1;
            while (countCrypSym < message.Count)
            {
                curBlock = new double[matrixSize];
                for (int i = 0; i < matrixSize; i++)
                {
                    for (int mesI = countCrypSym + i + noAl1; mesI < message.Count; mesI++)
                    {
                        if (message[mesI].FromAlpa)
                        {
                            if (message[mesI].Register)
                            {
                                curBlock[i] = alLo.IndexOf(message[mesI].Value);
                                break;
                            }
                            else
                            {
                                curBlock[i] = alUp.IndexOf(message[mesI].Value);
                                break;
                            }
                        }
                        else
                        {
                            noAl1++;
                        }
                    }
                }
                curBlock = Multiplication(curBlock, keyMtr);
                for (int i = 0; i < matrixSize; i++)
                {
                    for (int mesI = countCrypSym + i + noAl2; mesI < message.Count; mesI++)
                    {
                        if (message[mesI].FromAlpa)
                        {
                            if (message[mesI].Register)
                            {
                                result += alLo[Convert.ToInt32(curBlock[i] % module)];
                                break;
                            }
                            else
                            {
                                result += alUp[Convert.ToInt32(curBlock[i] % module)];
                                break;
                            }
                        }
                        else
                        {
                            result += message[mesI].Value;
                            noAl2++;
                        }
                    }
                }
                countCrypSym += matrixSize;
            }
            return result.Replace("\r", "\r\n");
        }

        private void HillEncrypBtn_Click(object sender, EventArgs e)
        {
            onceRun = true;
            // определение языка кодирования
            string cipherKey = DefineKeyLang(KeyTB.Text);
            if (cipherKey != null)
            {
                // Подготовка сообщения и ключа
                string Source = InputTB.Text.Replace("\r\n", "\r");
                string curAlLo = SetLang(lang, true);
                string curAlUp = SetLang(lang, false);
                // Приводим длину ключа к такой, чтобы из нее извлекался квадрат. корень
                int matrixSize = CheckSquare(cipherKey.Length);
                cipherKey = LeadCorrectKey(matrixSize * matrixSize - cipherKey.Length, cipherKey, curAlLo);
                // Замена на приведенный к нужному виду ключ
                KeyTB.Text = cipherKey;
                // Предварительная обработка сообщения (Принадлежность алфавиту, регистр символа)
                int lenMes;
                List<Symbol> message = FillingInList(Source, curAlLo, curAlUp, matrixSize, out lenMes);               
                if (lenMes != 0)
                {
                    // Заполнение матрицы-ключа
                    double[,] keyMatrix = new double[matrixSize, matrixSize];
                    for (int i = 0; i < matrixSize; i++)
                    {
                        for (int j = 0; j < matrixSize; j++)
                        {
                            keyMatrix[i, j] = curAlLo.IndexOf(cipherKey[i * matrixSize + j]);
                        }
                    }
                    // Проверка матрицы-ключа на обратимость
                    if (Determinant(keyMatrix) == 0)
                    {
                        MessageBox.Show("Матрица не обратима", "Ошибка");
                    }
                    else
                    {                      
                        OutputTB.Text = WorkHillCipher(message, curAlLo, curAlUp, keyMatrix);
                    }
                }
                else
                {
                    OutputTB.Text = Source.Replace("\r", "\r\n");
                }
            }
            else
            {
                MessageBox.Show("Невозможно определить язык ключа", "Ошибка", MessageBoxButtons.OK);
                KeyTB.Clear();
            }
        }

        private void HillDecrypBtn_Click(object sender, EventArgs e)
        {
            onceRun = true;
            // определение языка кодирования
            string cipherKey = DefineKeyLang(KeyTB.Text);
            if (cipherKey != null)
            {
                // Подготовка сообщения и ключа
                string Source = InputTB.Text.Replace("\r\n", "\r");
                string curAlLo = SetLang(lang, true);
                string curAlUp = SetLang(lang, false);
                // является ли ключ квадратом числа
                int matrixSize = CheckSquare(cipherKey.Length);
                cipherKey = LeadCorrectKey(matrixSize * matrixSize - cipherKey.Length, cipherKey, curAlLo);
                // Замена на приведенный к нужному виду ключ
                KeyTB.Text = cipherKey;
                // Предварительная обработка сообщения (Принадлежность алфавиту, регистр символа)
                int lenMes;
                List<Symbol> message = FillingInList(Source, curAlLo, curAlUp, matrixSize, out lenMes);
                if (lenMes != 0)
                {
                    // Заполнение матрицы-ключа
                    double[,] keyMatrix = new double[matrixSize, matrixSize];
                    for (int i = 0; i < matrixSize; i++)
                    {
                        for (int j = 0; j < matrixSize; j++)
                        {
                            keyMatrix[i, j] = curAlLo.IndexOf(cipherKey[i * matrixSize + j]);
                        }
                    }
                    // Проверка матрицы-ключа на обратимость
                    double det = Determinant(keyMatrix);
                    if (det == 0)
                    {
                        MessageBox.Show("Матрица не обратима", "Ошибка");
                    }
                    else
                    {
                        if (NOD(Convert.ToInt32(det), curAlLo.Length, out int x, out int y) == 1)
                        {   // обратная матрица                           
                            OutputTB.Text = WorkHillCipher(message, curAlLo, curAlUp, InverseMatrix(keyMatrix, det, curAlLo.Length));
                        }
                        else
                        {
                            MessageBox.Show("Определитель матрицы и длина алфавита не взаимно простые", "Ошибка", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    OutputTB.Text = Source.Replace("\r", "\r\n");
                }
            }
            else
            {
                MessageBox.Show("Невозможно определить язык ключа", "Ошибка", MessageBoxButtons.OK);
                KeyTB.Clear();
            }
        }

        private double Determinant(double[,] matrix)
        {
            if ((matrix.GetUpperBound(0) + 1) == 1)
            {
                return matrix[0, 0];
            }
            else if ((matrix.GetUpperBound(0) + 1) == 2)
            {
                return (matrix[0,0]*matrix[1,1] - matrix[0,1]*matrix[1,0]);
            }
            double result = 0;
            for (int j = 0; j < (matrix.GetUpperBound(0) + 1); j++)
            {
                result += (j % 2 == 0 ? 1 : -1) * matrix[0, j] * Determinant(Minor(matrix, 0, j));
            }
            return result;
        }

        private double[,] Minor(double[,] oldMat, int row, int column)
        {
            double[,] minor = new double[oldMat.GetUpperBound(0), oldMat.GetUpperBound(0)];
            int _i = 0;
            for (int i = 0; i < oldMat.GetUpperBound(0) + 1; i++)
            {
                if (i == row)
                {
                    continue;
                }
                int _j = 0;
                for (int j = 0; j < oldMat.GetUpperBound(0) + 1; j++)
                {
                    if (j == column)
                    {
                        continue;
                    }
                    minor[_i, _j] = oldMat[i, j];
                    _j++;
                }
                _i++;
            }
            return minor;
        }

        public double[,] InverseMatrix(double[,] oldMat, double oldDet, int module)
        {
            double[,] invMat = new double[oldMat.GetUpperBound(0) + 1, oldMat.GetUpperBound(0) + 1];
            double invDet = InverseElement(Convert.ToInt32(oldDet), module);

            for (int i = 0; i < oldMat.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < oldMat.GetUpperBound(0) + 1; j++)
                {
                    invMat[i, j] = (((((i + j) % 2 == 0 ? 1 : -1) * Determinant(Minor(oldMat, i, j))) % module * invDet) % module + module) % module;
                }
            }
            return Transpose(invMat);
        }

        public static int NOD(int a, int b, out int x, out int y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }
            int _x, _y;
            int d = NOD(b % a, a, out _x, out _y);
            x = _y - (b / a) * _x;
            y = _x;
            return d;
        }

        public static double InverseElement(int oldNum, int module)
        {
            int x, y;
            int d = NOD(oldNum, module, out x, out y);

            if (oldNum < 0)
            {
                if (x > 0)
                {
                    return x % module;
                }
                else
                {
                    return -(x % module);
                }
            }
            else
            {
                if (x > 0)
                {
                    return x % module;
                }
                else
                {
                    return (module + (x % module)) % module;
                }
            }
        }

        public static double[,] Transpose(double[,] oldMat)
        {
            double[,] transMat = new double[oldMat.GetUpperBound(0) + 1, oldMat.GetUpperBound(0) + 1];
            for (int i = 0; i < oldMat.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < oldMat.GetUpperBound(0) + 1; j++)
                {
                    transMat[j, i] = oldMat[i, j];
                }
            }
            return transMat;
        }

        public double[] Multiplication(double[] m1, double[,] m2) // умножение вектора-строки на матрицу
        {
            if (m1.Length != (m2.GetUpperBound(0) + 1))
            {
                throw new Exception("Умножение невозможно");
            }
            double[] res = new double[m1.Length];
            for (int i = 0; i < m2.GetUpperBound(1) + 1; i++)
            {
                double sum = 0;
                for (int j = 0; j < m1.Length; j++)
                {
                    sum += m1[j] * m2[j, i];
                }
                res[i] = sum;
            }
            return res;
        }

        private void KeyTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            onceRun = false;
            char curChar = e.KeyChar;
            if (!(char.IsLetter(curChar)) && curChar != (char)8 && curChar != ' ')
                e.Handled = true;
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

        private void ShowCipherKeyLabel_Click(object sender, EventArgs e)
        {
            if (KeyTB.Text != "" && onceRun)
            {
                string matr = "";
                string key = KeyTB.Text;
                int matrixSize = Convert.ToInt32(Math.Sqrt(key.Length));
                for (int i = 0; i < matrixSize; i++)
                {
                    for (int j = 0; j < matrixSize; j++)
                    {
                        if (lang) matr += enAlLo.IndexOf(key[i * matrixSize + j]).ToString() + '\t';
                        else matr += ruAlLo.IndexOf(key[i * matrixSize + j]).ToString() + '\t';
                    }
                    matr += "\r\n";
                }
                MessageBox.Show(matr, "Информация", MessageBoxButtons.OK);
            }
            else
                MessageBox.Show("Матрица не может быть создана", "Информация", MessageBoxButtons.OK);
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
                using (StreamWriter sw = new StreamWriter(saveFile.FileName, false)) // перезапись файла
                {
                    sw.WriteLine(OutputTB.Text);
                    sw.Close();
                }
            }
        }
    }
}
