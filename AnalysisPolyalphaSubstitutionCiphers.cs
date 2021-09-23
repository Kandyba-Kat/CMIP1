using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace AtbashCipher
{
    public partial class AnalysisPolyalphaSubstitutionCiphers : Form
    {
        public AnalysisPolyalphaSubstitutionCiphers()
        {
            InitializeComponent();
        }

        public string enAlLo = "abcdefghijklmnopqrstuvwxyz"; // length = 26
        public string enAlUp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public string ruAlLo = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя"; // length = 33
        public string ruAlUp = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        // Распределение вероятностей букв в русских текстах
        public double[] ruLetFreqs = { 0.062, 0.014, 0.038, 0.013, 0.025, 0.072, 0.0001, 0.007, 0.016, 0.062, 0.01, 0.028, 0.035, 0.026, 0.053, 0.09,
                                       0.023, 0.04, 0.045, 0.053, 0.021, 0.002, 0.009, 0.004, 0.012, 0.006, 0.003, 0.0004, 0.016, 0.014, 0.003, 0.006, 0.018};
        // Распределение вероятностей букв в английских текстах
        public double[] enLetFreqs = {0.081, 0.016, 0.032, 0.036, 0.123, 0.023, 0.016, 0.051, 0.071, 0.001, 0.005, 0.04, 0.022, 0.072,
                                      0.079, 0.023, 0.002, 0.06, 0.066, 0.096, 0.031, 0.009, 0.02, 0.002, 0.019, 0.001};
        public string alphaLo = "";
        public string alphaUp = "";

        // Подсчет количества русских и англ. букв
        private void CountingLetters(string curText, out int countRu, out int countEn)
        {
            countRu = 0;
            countEn = 0;

            curText = curText.ToLower();

            for (int i = 0; i < curText.Length; i++)
            {
                if (ruAlLo.IndexOf(curText[i]) >= 0)
                {
                    countRu++;
                }
                else if (enAlLo.IndexOf(curText[i]) >= 0)
                {
                    countEn++;
                }
            }
        }

        // Подсчет количества вхождений каждой русской и англ. буквы
        private void CountingEntrys(string curText, out int[] ruLetters, out int[] enLetters)
        {
            ruLetters = new int[ruAlLo.Length]; // массивы с количеством вхождений для каждой буквы
            enLetters = new int[enAlLo.Length];
            for (int i = 0; i < ruAlLo.Length; i++)
            {
                if (i < enAlLo.Length)
                {
                    enLetters[i] = 0;
                }
                ruLetters[i] = 0;
            }
            for (int i = 0; i < curText.Length; i++)
            {
                if (ruAlLo.IndexOf(curText[i]) >= 0)
                {
                    ruLetters[ruAlLo.IndexOf(curText[i])]++;
                }
                else if (enAlLo.IndexOf(curText[i]) >= 0)
                {
                    enLetters[enAlLo.IndexOf(curText[i])]++;
                }
            }
        }

        private void Create_Table_Chart()
        {
            int[] ruLetters; // массивы с количеством вхождений для каждой буквы
            int[] enLetters;
            CountingEntrys(OutputRTB.Text.ToLower(), out ruLetters, out enLetters);
            // Подсчет количества русских и англ. букв
            int countRu;
            int countEn;
            CountingLetters(OutputRTB.Text, out countRu, out countEn);

            dataGridView.ColumnCount = 5;
            dataGridView.RowCount = 0;
            dataGridView.Columns[0].HeaderText = "Lang";
            dataGridView.Columns[1].HeaderText = "Letter";
            dataGridView.Columns[2].HeaderText = "Count";
            dataGridView.Columns[3].HeaderText = "Letter freq";
            dataGridView.Columns[4].HeaderText = "Freq average";
            int curInd = 0;
            for (int i = 0; i < ruLetters.Length; i++)
            {
                if (ruLetters[i] > 0)
                {
                    dataGridView.RowCount++;
                    dataGridView.Rows[curInd].Cells[0].Value = "ru";
                    dataGridView.Rows[curInd].Cells[1].Value = ruAlLo[i];
                    dataGridView.Rows[curInd].Cells[2].Value = ruLetters[i];
                    dataGridView.Rows[curInd].Cells[3].Value = Math.Round((double)ruLetters[i] / countRu, 4);
                    dataGridView.Rows[curInd].Cells[4].Value = Math.Round(ruLetFreqs[i], 4);
                    curInd++;
                }
            }
            for (int i = 0; i < enLetters.Length; i++)
            {
                if (enLetters[i] > 0)
                {
                    dataGridView.RowCount++;
                    dataGridView.Rows[curInd].Cells[0].Value = "en";
                    dataGridView.Rows[curInd].Cells[1].Value = enAlLo[i];
                    dataGridView.Rows[curInd].Cells[2].Value = enLetters[i];
                    dataGridView.Rows[curInd].Cells[3].Value = Math.Round((double)enLetters[i] / countEn, 4);
                    dataGridView.Rows[curInd].Cells[4].Value = Math.Round(enLetFreqs[i], 4);
                    curInd++;
                }
            }
            // Заполнение графика
            chart.Series[0].Points.Clear();
            if (countRu > 0)
            {
                for (int i = 0; i < ruLetters.Length; i++)
                {
                    chart.Series[0].Points.AddXY(ruAlLo[i].ToString(), Math.Round((double)ruLetters[i] / countRu, 4));
                }
            }
            if (countEn > 0)
            {
                for (int i = 0; i < enLetters.Length; i++)
                {
                    chart.Series[0].Points.AddXY(enAlLo[i].ToString(), Math.Round((double)enLetters[i] / countEn, 4));
                }
            }
        }

        private void OpenFileStripMenu_Click(object sender, EventArgs e)
        {
            InputRTB.Clear();
            OutputRTB.Clear();
            Key1TB.Clear();
            CurrentKeyTB.Clear();
            KeyLenNUD.Value = 0;

            string strFile = "";

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text file|*.txt";
            openFile.DefaultExt = ".txt";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(openFile.FileName))
                    {
                        strFile = sr.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Файл не может быть открыт. Ошибка: " + ex.Message);
                }
            }

            // Подсчет количества русских и англ. букв
            int countRu;
            int countEn;
            CountingLetters(strFile, out countRu, out countEn);
            if (countRu * countEn > 0)
            {
                MessageBox.Show("Использование разных языков не допускается.", "Ошибка");
            }
            else
            {
                if (countRu > 0)
                {
                    alphaLo = ruAlLo;
                    alphaUp = ruAlUp;
                }
                else if (countEn > 0)
                {
                    alphaLo = enAlLo;
                    alphaUp = enAlUp;
                }

                InputRTB.Text = strFile;
            }
        }

        private void SaveFileStripMenu_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = ".txt";
            saveFile.Filter = "Text file|*.txt";
            if (saveFile.ShowDialog() == DialogResult.OK && saveFile.FileName.Length > 0)
            {
                using (StreamWriter sw = new StreamWriter(saveFile.FileName, false)) // перезапись файла
                {
                    sw.WriteLine(OutputRTB.Text);
                }
            }
        }

        private void CurrentKeyTB_TextChanged(object sender, EventArgs e)
        {
            if (CurrentKeyTB.Text.Length > 0)
            {
                string result = "";
                int j = 0;
                for (int indLetIn = 0; indLetIn < InputRTB.Text.Length; indLetIn++)
                {
                    int curIndLet = alphaLo.IndexOf(InputRTB.Text[indLetIn]);
                    int shiftLet = (alphaLo.IndexOf(CurrentKeyTB.Text[j])) % alphaLo.Length;
                    if (curIndLet != -1) // строчная буква
                    {
                        result += alphaLo[(curIndLet - shiftLet + alphaLo.Length) % alphaLo.Length];
                        j = (j + 1) % CurrentKeyTB.Text.Length;
                    }
                    else
                    {
                        curIndLet = alphaUp.IndexOf(InputRTB.Text[indLetIn]);
                        if (curIndLet != -1) // заглавная буква
                        {
                            result += alphaUp[(curIndLet - shiftLet + alphaUp.Length) % alphaUp.Length];
                            j = (j + 1) % CurrentKeyTB.Text.Length;
                        }
                        else // символы, которых нет в алфвите
                        {
                            result += InputRTB.Text[indLetIn];
                        }
                    }
                }
                OutputRTB.Text = result;
            }
            else
            {
                OutputRTB.Clear();
            }
        }

        private void OutputRTB_TextChanged(object sender, EventArgs e)
        {
            Create_Table_Chart();
        }

        private void InputRTB_TextChanged(object sender, EventArgs e)
        {
            OutputRTB.Clear();
            Key1TB.Clear();
            KeyLenNUD.Value = 0;

            // Подсчет количества русских и англ. букв
            int countRu;
            int countEn;
            CountingLetters(InputRTB.Text, out countRu, out countEn);
            if (countRu * countEn > 0)
            {
                InputRTB.Clear();
                MessageBox.Show("Использование смешения языков не допускается.", "Ошибка");
            }
            else
            {
                if (countRu > 0)
                {
                    alphaLo = ruAlLo;
                    alphaUp = ruAlUp;
                }
                else if (countEn > 0)
                {
                    alphaLo = enAlLo;
                    alphaUp = enAlUp;
                }

                string tmp = CurrentKeyTB.Text;
                CurrentKeyTB.Clear();
                CurrentKeyTB.Text = tmp;
            }
        }

        private void СalculateBtn_Click(object sender, EventArgs e)
        {
            KeyLenNUD.Value = 0;
            if (MethodCB.Text == "Метод индекса совпадений" && InputRTB.Text.Length > 0)
            {
                int posLen = 1;
                while (posLen < 20)
                {
                    string inMes = InputRTB.Text.ToLower();
                    string tmpSubstr = ""; // подстрока из символов
                    int j = 0;
                    for (int indLen = 0; indLen < inMes.Length; indLen++)
                    {
                        if (j % posLen == 0 && alphaLo.IndexOf(inMes[indLen]) != -1)
                        {
                            tmpSubstr += inMes[indLen];
                            j++;
                        }
                        else if (j % posLen != 0 && alphaLo.IndexOf(inMes[indLen]) != -1)
                        {
                            j++;
                        }
                    }
                    // подсчитаем индекс
                    double index = 0;
                    int[] times = new int[alphaLo.Length];
                    Array.Clear(times, 0, times.Length);
                    // количество вхождений каждой буквы в подстроке
                    int[] ruLetters;
                    int[] enLetters;
                    CountingEntrys(tmpSubstr, out ruLetters, out enLetters);
                    if (times.Length == ruLetters.Length)
                    {
                        Array.Copy(ruLetters, times, alphaLo.Length);
                    }
                    else
                    {
                        Array.Copy(enLetters, times, alphaLo.Length);
                    }
                    for (int i = 0; i < times.Length; i++)
                    {
                        index += times[i] * (times[i] - 1);
                    }
                    index = index / (double)(tmpSubstr.Length * (tmpSubstr.Length - 1));
                    if (alphaLo.Length == ruAlLo.Length)
                    {
                        if (index >= 0.0553)
                        {
                            KeyLenNUD.Value = posLen;
                        }
                    }
                    else
                    {
                        if (index >= 0.0667)
                        {
                            KeyLenNUD.Value = posLen;
                        }
                    }
                    posLen++;
                }
            }
            if (MethodCB.Text == "Автокорреляционный метод" && InputRTB.Text.Length > 0)
            {
                string inMes = InputRTB.Text.ToLower();
                for (int indLen = 0; indLen < inMes.Length; indLen++) // Удаляем разделители
                {
                    if (alphaLo.IndexOf(inMes[indLen]) < 0)
                    {
                        string tmpDel = "";
                        tmpDel += inMes[indLen];
                        inMes = inMes.Replace(tmpDel, "");
                    }
                }
                for (int posLen = 1; posLen < 20; posLen++)
                {
                    string shiftMes = inMes.Substring(posLen) + inMes.Substring(0, posLen);
                    int nt = 0; // совпадения
                    for (int i = 0; i < inMes.Length; i++)
                    {
                        if (inMes[i] == shiftMes[i]) nt++;
                    }
                    double gammat = nt / (double)(inMes.Length - posLen);
                    if (alphaLo.Length == ruAlLo.Length)
                    {
                        if (gammat >= 0.0553)
                        {
                            KeyLenNUD.Value = posLen;
                            break;
                        }
                    }
                    else
                    {
                        if (gammat >= 0.0667)
                        {
                            KeyLenNUD.Value = posLen;
                            break;
                        }
                    }
                }
            }
            if (MethodCB.Text == "Тест Казиски" && InputRTB.Text.Length > 0)
            {
                string inMes = InputRTB.Text.ToLower();
                for (int i = 0; i < inMes.Length; i++)
                {
                    if (alphaLo.IndexOf(inMes[i]) < 0)
                    {
                        string tmpDel = "";
                        tmpDel += inMes[i];
                        inMes = inMes.Replace(tmpDel, "");
                    }
                }
                string checkedSubstr = "";
                List<int> posLen = new List<int>();
                // если значение -1, то проверяем, иначе - нет
                for (int i = 0; i < inMes.Length/(3-1); i++)
                {
                    string substr = inMes.Substring(i, 3);
                    if (checkedSubstr.IndexOf(substr) == -1)
                    {
                        // кол-во вхождений подстроки
                        int num = 0;
                        int j = 0;
                        while (inMes.IndexOf(substr, j) != -1)
                        {
                            j = inMes.IndexOf(substr, j) + 1;
                            num++;
                        }

                        if (num >= 3)
                        {
                            checkedSubstr += substr + " ";
                            List<int> posDiff = new List<int>();
                            int prev = inMes.IndexOf(substr); // первое вхождение подстроки
                            j = prev + 1; // поиск начиная со второго вхождения
                            while (inMes.IndexOf(substr, j) != -1)
                            {
                                int curInd = inMes.IndexOf(substr, j);
                                posDiff.Add(curInd - 1);
                                prev = curInd;
                                j = curInd + 1;
                            }
                            int curNOD = posDiff[0];
                            for (int n = 0; n < posDiff.Count; n++)
                            {
                                curNOD = NOD(curNOD, posDiff[n]);
                            }
                            if (curNOD > 1)
                            {
                                posLen.Add(curNOD);
                            }
                        }
                    }
                }
                if (posLen.Count > 0)
                {
                    posLen.Sort();
                    int[] posLenFinal = new int[20];
                    int len = 0;
                    while (len < posLen.Count() && posLen[len] < 20)
                    {
                        posLenFinal[posLen[len]]++;
                        len++;
                    }
                    for (len = 0; len < 20; len++)
                    {
                        posLenFinal[len] = posLenFinal[len] * len;
                    }
                    for (len = 0; len < 20; len++)
                    {
                        if (posLenFinal[len] == posLenFinal.Max())
                        {
                            KeyLenNUD.Value = len;
                        }
                    }
                }
            }
        }

        public static int NOD(int a, int b)
        {
            if (a == 0)
            {
                return b;
            }
            return NOD(b % a, a);
        }

        private void RunK1Btn_Click(object sender, EventArgs e)
        {
            int keyLen = Convert.ToInt32(KeyLenNUD.Value);
            string inMes = InputRTB.Text.ToLower();
            for (int indLen = 0; indLen < inMes.Length; indLen++) // удаление того, чего нет в алфавитах
            {
                if (alphaLo.IndexOf(inMes[indLen]) < 0)
                {
                    string tmpDel = "";
                    tmpDel += inMes[indLen];
                    inMes = inMes.Replace(tmpDel, "");
                }
            }
            if (keyLen < 1 || inMes.Length < keyLen) { }
            else
            {
                string[] substr = new string[keyLen]; // разобьем текст на подстроки длины keyLen
                for (int k = 0; k < keyLen; k++)
                {
                    string tmp = "";
                    int j = k;
                    while (j < inMes.Length)
                    {
                        tmp += inMes[j];
                        j += keyLen;
                    }
                    substr[k] = tmp;
                }
                string result = "";
                // находим ключ для каждого шифра Цезаря отдельно
                for (int k = 0; k < keyLen; k++)
                {
                    // кол-во вхождений каждой буквы
                    int[] times = new int[alphaLo.Length];
                    Array.Clear(times, 0, times.Length);
                    for (int i = 0; i < substr[k].Length; i++)
                    {
                        if (alphaLo.IndexOf(substr[k][i]) >= 0)
                        {
                            times[alphaLo.IndexOf(substr[k][i])]++;
                        }
                    }
                    // найдем самую часто встречающуюся букву, запомним ее индекс
                    int index = -1;
                    for (int i = 0; i < times.Length; i++)
                    {
                        if (times[i] == times.Max())
                        {
                            index = i;
                        }
                    }
                    // номер самой часто встречающейся статистически буквы
                    int indStat = alphaLo.IndexOf('e'); // англ. алфавит
                    if (alphaLo.IndexOf('о') >= 0)
                    {
                        indStat = alphaLo.IndexOf('о'); // русский алфавит
                    }
                    result += alphaLo[(index - indStat + alphaLo.Length) % alphaLo.Length];
                }
                Key1TB.Text = result;
            }
        }

        private void ApplyK1Btn_Click(object sender, EventArgs e)
        {
            CurrentKeyTB.Text = Key1TB.Text;
        }

        private void label15_Click(object sender, EventArgs e)
        {
            double[] forSortFreq = (double[])ruLetFreqs.Clone(); ;
            char[] forSortAlpha = ruAlLo.ToCharArray();
            // сортировка
            for (int i = 0; i < forSortFreq.Length - 1; i++)
            {
                for (int j = i + 1; j < forSortFreq.Length; j++)
                {
                    if (forSortFreq[i] < forSortFreq[j])
                    {
                        double temp1 = forSortFreq[i];
                        forSortFreq[i] = forSortFreq[j];
                        forSortFreq[j] = temp1;

                        char temp2 = forSortAlpha[i];
                        forSortAlpha[i] = forSortAlpha[j];
                        forSortAlpha[j] = temp2;
                    }
                }
            } // конец сортировки

            string matr = "";
            for (int i = 0; i < forSortFreq.Length; i++)
            {
                matr += forSortAlpha[i].ToString() + '\t' + forSortFreq[i].ToString() + "\r\n";
            }
            MessageBox.Show(matr, "Информация", MessageBoxButtons.OK);
        }

        private void label16_Click(object sender, EventArgs e)
        {
            double[] forSortFreq = (double[])enLetFreqs.Clone(); ;
            char[] forSortAlpha = enAlLo.ToCharArray();
            // сортировка
            for (int i = 0; i < forSortFreq.Length - 1; i++)
            {
                for (int j = i + 1; j < forSortFreq.Length; j++)
                {
                    if (forSortFreq[i] < forSortFreq[j])
                    {
                        double temp1 = forSortFreq[i];
                        forSortFreq[i] = forSortFreq[j];
                        forSortFreq[j] = temp1;

                        char temp2 = forSortAlpha[i];
                        forSortAlpha[i] = forSortAlpha[j];
                        forSortAlpha[j] = temp2;
                    }
                }
            } // конец сортировки

            string matr = "";
            for (int i = 0; i < forSortFreq.Length; i++)
            {
                matr += forSortAlpha[i].ToString() + '\t' + forSortFreq[i].ToString() + "\r\n";
            }
            MessageBox.Show(matr, "Информация", MessageBoxButtons.OK);
        }

        private void ClearInputLabel_Click(object sender, EventArgs e)
        {
            if (InputRTB.Text != "")
                InputRTB.Clear();
        }
    }
}
