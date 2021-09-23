using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AtbashCipher
{
    public partial class FrequencyAnalysis : Form
    {
        public FrequencyAnalysis()
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

        private void InputRTB_TextChanged(object sender, EventArgs e)
        {
            ShiftNUD.Value = 1;
            ShiftNUD.Value = 0;
            Key1TB.Clear();
            Key2TB.Clear();
            Key3TB.Clear();
            Create_Table_Chart();
        }
        // Пересчет пезультата по введенному в ShiftNUD сдвигу
        private void ShiftNUD_ValueChanged(object sender, EventArgs e)
        {
            int cur_shift = Convert.ToInt32(ShiftNUD.Value);
            char[] ciphertext = InputRTB.Text.ToCharArray();
            int shiftRu = (ruAlLo.Length - (cur_shift % ruAlLo.Length)) % ruAlLo.Length;
            int shiftEn = (enAlLo.Length - (cur_shift % enAlLo.Length)) % enAlLo.Length;

            for (int indSymb = 0; indSymb < ciphertext.Length; indSymb++)
            {
                ciphertext[indSymb] = ReplacingChar(ciphertext[indSymb], shiftRu, shiftEn, false);
            } // end for (int i = 0; i < ciphertext.Length; i++)

            OutputRTB.Text = new string(ciphertext);
            Create_Table_Chart();
        }

        private void Create_Table_Chart()
        {
            int[] ruLetters; // массивы с количеством вхождений для каждой буквы
            int[] enLetters;
            // Подсчет количества русских и англ. букв
            int countRu;
            int countEn;
            CountingEntrys(InputRTB.Text.ToLower(), out countRu, out countEn, out ruLetters, out enLetters);

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
                    dataGridView.Rows[curInd].Cells[3].Value = Math.Round((double)ruLetters[i]/countRu, 4);
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

        // Подсчет количества русских и англ. букв
        private void CountingEntrys(string curText, out int countRu, out int countEn, out int[] ruLetters, out int[] enLetters)
        {
            countRu = 0;
            countEn = 0;
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
                    countRu++;
                    ruLetters[ruAlLo.IndexOf(curText[i])]++;
                }
                else if (enAlLo.IndexOf(curText[i]) >= 0)
                {
                    countEn++;
                    enLetters[enAlLo.IndexOf(curText[i])]++;
                }
            }
        }

        private char ReplacingChar(char cur_char, int shiftRu, int shiftEn, bool calc)
        {
            if (enAlLo.IndexOf(cur_char) >= 0)
            {
                if (calc)
                    return enAlLo[(enAlLo.IndexOf(cur_char) - shiftEn + enAlLo.Length) % enAlLo.Length];
                else
                    return enAlLo[(enAlLo.IndexOf(cur_char) + shiftEn) % enAlLo.Length];
            }
            else if (enAlUp.IndexOf(cur_char) >= 0)
            {
                if (calc)
                    return enAlUp[(enAlUp.IndexOf(cur_char) - shiftEn + enAlUp.Length) % enAlUp.Length];
                else
                    return enAlUp[(enAlUp.IndexOf(cur_char) + shiftEn) % enAlUp.Length];
            }
            else if (ruAlLo.IndexOf(cur_char) >= 0)
            {
                if (calc)
                    return ruAlLo[(ruAlLo.IndexOf(cur_char) - shiftRu + ruAlLo.Length) % ruAlLo.Length];
                else
                    return ruAlLo[(ruAlLo.IndexOf(cur_char) + shiftRu) % ruAlLo.Length];
            }
            else if (ruAlUp.IndexOf(cur_char) >= 0)
            {
                if (calc)
                    return ruAlUp[(ruAlUp.IndexOf(cur_char) - shiftRu + ruAlUp.Length) % ruAlUp.Length];
                else
                    return ruAlUp[(ruAlUp.IndexOf(cur_char) + shiftRu) % ruAlUp.Length];
            }
            else
            {
                return cur_char;
            }
        }

        private void СalculateBtn_Click(object sender, EventArgs e)
        {
            if (CheckBoxSingleLet.Checked || CheckBoxBigram.Checked || CheckBoxInvalidComb.Checked || CheckBoxLetPosition.Checked)
            {                
                double[] probabiShift = new double[ruAlLo.Length]; // Вероятности для каждого сдвига алфавита

                for (int indShift = 0; indShift < probabiShift.Length; indShift++)
                {
                    char[] ciphertext = InputRTB.Text.ToCharArray();
                    int shiftRu = indShift;
                    int shiftEn = indShift % enAlLo.Length;

                    for (int indSymb = 0; indSymb < ciphertext.Length; indSymb++)
                    {
                        ciphertext[indSymb] = ReplacingChar(ciphertext[indSymb], shiftRu, shiftEn, true);
                    } // end for (int i = 0; i < ciphertext.Length; i++)

                    string potenText = new string(ciphertext).ToLower();
                    // Анализ потенциально верной строки
                    //double errorRate = (double)ErrorNUD.Value;
                    int[] ruLetters; // массивы с количеством вхождений для каждой буквы
                    int[] enLetters;
                    // Подсчет количества русских и англ. букв
                    int countRu;
                    int countEn;
                    CountingEntrys(potenText, out countRu, out countEn, out ruLetters, out enLetters);

                    // Проверки для русских букв из текста
                    if (countRu != 0)
                    {
                        if (CheckBoxSingleLet.Checked) probabiShift[indShift] = CheckSingleLetter(true, ruLetters, countRu);

                        if (CheckBoxBigram.Checked)
                        {   //Дополнительные баллы вероятности за популярные биграммы
                            if (potenText.IndexOf("ст") >= 0) probabiShift[indShift] += 0.00081;
                            if (potenText.IndexOf("ен") >= 0) probabiShift[indShift] += 0.00064;
                            if (potenText.IndexOf("ов") >= 0) probabiShift[indShift] += 0.00062;                           
                            if (potenText.IndexOf("но") >= 0) probabiShift[indShift] += 0.00062;
                            if (potenText.IndexOf("ни") >= 0) probabiShift[indShift] += 0.00061;
                            if (potenText.IndexOf("на") >= 0) probabiShift[indShift] += 0.00061;
                            if (potenText.IndexOf("ра") >= 0) probabiShift[indShift] += 0.00058;
                            if (potenText.IndexOf("ко") >= 0) probabiShift[indShift] += 0.00056;
                            if (potenText.IndexOf("то") >= 0) probabiShift[indShift] += 0.00052;
                            if (potenText.IndexOf("ро") >= 0) probabiShift[indShift] += 0.00052;
                            if (potenText.IndexOf("ан") >= 0) probabiShift[indShift] += 0.00052;
                            if (potenText.IndexOf("ос") >= 0) probabiShift[indShift] += 0.00050;
                            if (potenText.IndexOf("по") >= 0) probabiShift[indShift] += 0.00048;
                            if (potenText.IndexOf("го") >= 0) probabiShift[indShift] += 0.00046;
                            if (potenText.IndexOf("ер") >= 0) probabiShift[indShift] += 0.00046;
                            if (potenText.IndexOf("од") >= 0) probabiShift[indShift] += 0.00044;
                            if (potenText.IndexOf("ре") >= 0) probabiShift[indShift] += 0.00044;
                            if (potenText.IndexOf("ор") >= 0) probabiShift[indShift] += 0.00043;
                            if (potenText.IndexOf("пр") >= 0) probabiShift[indShift] += 0.00040;
                            if (potenText.IndexOf("во") >= 0) probabiShift[indShift] += 0.00039;
                        }

                        if (CheckBoxInvalidComb.Checked) probabiShift[indShift] = CheckInvalidComb(true, potenText, probabiShift[indShift]);

                        if (CheckBoxLetPosition.Checked) probabiShift[indShift] = CheckLetterPosition(true, potenText, probabiShift[indShift]);
                    }
                    // Проверки для английских букв из текста
                    if (countEn != 0)
                    {
                        if (countRu == 0)
                        {
                            for (int i = enAlLo.Length; i < ruAlLo.Length; i++)
                                probabiShift[i] = -1;
                        }
                        if (CheckBoxSingleLet.Checked) probabiShift[indShift] = CheckSingleLetter(false, enLetters, countEn);

                        if (CheckBoxBigram.Checked)
                        {   //Дополнительные баллы вероятности за популярные биграммы
                            if (potenText.IndexOf("th") >= 0) probabiShift[indShift] += 0.00136;
                            if (potenText.IndexOf("he") >= 0) probabiShift[indShift] += 0.00117;
                            if (potenText.IndexOf("in") >= 0) probabiShift[indShift] += 0.00102;
                            if (potenText.IndexOf("er") >= 0) probabiShift[indShift] += 0.00089;
                            if (potenText.IndexOf("an") >= 0) probabiShift[indShift] += 0.00081;
                            if (potenText.IndexOf("re") >= 0) probabiShift[indShift] += 0.00071;
                            if (potenText.IndexOf("es") >= 0) probabiShift[indShift] += 0.00066;
                            if (potenText.IndexOf("on") >= 0) probabiShift[indShift] += 0.00066;
                            if (potenText.IndexOf("st") >= 0) probabiShift[indShift] += 0.00063;
                            if (potenText.IndexOf("nt") >= 0) probabiShift[indShift] += 0.00059;
                            if (potenText.IndexOf("en") >= 0) probabiShift[indShift] += 0.00057;
                            if (potenText.IndexOf("at") >= 0) probabiShift[indShift] += 0.00056;
                            if (potenText.IndexOf("ed") >= 0) probabiShift[indShift] += 0.00054;
                            if (potenText.IndexOf("nd") >= 0) probabiShift[indShift] += 0.00054;
                            if (potenText.IndexOf("to") >= 0) probabiShift[indShift] += 0.00054;
                            if (potenText.IndexOf("or") >= 0) probabiShift[indShift] += 0.00053;
                            if (potenText.IndexOf("ea") >= 0) probabiShift[indShift] += 0.00050;
                            if (potenText.IndexOf("ti") >= 0) probabiShift[indShift] += 0.00050;
                            if (potenText.IndexOf("ar") >= 0) probabiShift[indShift] += 0.00049;
                            if (potenText.IndexOf("te") >= 0) probabiShift[indShift] += 0.00049;
                        }

                        if (CheckBoxInvalidComb.Checked) probabiShift[indShift] = CheckInvalidComb(false, potenText, probabiShift[indShift]);

                        if (CheckBoxLetPosition.Checked) probabiShift[indShift] = CheckLetterPosition(false, potenText, probabiShift[indShift]);
                    }
                } // end for (int indShift = 0; indShift < probabiShift.Length; indShift++)

                // Дополнительный массив для сортировки шифров
                int[] forSortProbShift = new int[probabiShift.Length];
                for (int i = 0; i < forSortProbShift.Length; i++) // заполнение forSortProbShift
                {
                    if (probabiShift[i] == -1)
                        forSortProbShift[i] = -1;
                    else
                        forSortProbShift[i] = i;
                }
                // сортировка
                for (int i = 0; i < forSortProbShift.Length - 1; i++)
                {
                    for (int j = i + 1; j < forSortProbShift.Length; j++)
                    {
                        if (probabiShift[i] > probabiShift[j])
                        {
                            double temp1 = probabiShift[i];
                            probabiShift[i] = probabiShift[j];
                            probabiShift[j] = temp1;

                            int temp2 = forSortProbShift[i];
                            forSortProbShift[i] = forSortProbShift[j];
                            forSortProbShift[j] = temp2;
                        }
                    }
                } // конец сортировки

                // Вывод результатов
                if (forSortProbShift[forSortProbShift.Length - 1] != -1)
                {
                    Key1TB.Text = Convert.ToString(forSortProbShift[forSortProbShift.Length - 1]);
                }
                else
                {
                    Key1TB.Clear();
                    MessageBox.Show("Не удалось определить ключ", "Справка");
                }
                if (forSortProbShift[forSortProbShift.Length - 2] != -1)
                {
                    Key2TB.Text = Convert.ToString(forSortProbShift[forSortProbShift.Length - 2]);
                }
                else
                {
                    Key2TB.Clear();
                }
                if (forSortProbShift[forSortProbShift.Length - 3] != -1)
                {
                    Key3TB.Text = Convert.ToString(forSortProbShift[forSortProbShift.Length - 3]);
                }
                else
                {
                    Key3TB.Clear();
                }
            }
            else
            {
                MessageBox.Show("Не выбрано ни одного параметра", "Ошибка");
            }
        }
        
        private double CheckSingleLetter(bool lang, int[] countLets, int allCount)
        {
            string alpha;
            double[] letFreqs;
            double errorRate = (double)ErrorNUD.Value;
            if (lang)
            {
                alpha = ruAlLo;
                letFreqs = ruLetFreqs;
            }
            else
            {
                alpha = enAlLo;
                letFreqs = enLetFreqs;
            }
            // За каждую букву с частотой, отличной от статистической не менее чем на погрешность, добавляем частотность в квадрате к вероятности использования данного сдвига
            double prob = 0;
            for (int i = 0; i < alpha.Length; i++)
            {
                double frq = Math.Round((double)countLets[i]/allCount, 5);

                if ((frq - errorRate * letFreqs[i] <= letFreqs[i]) && (frq + errorRate * letFreqs[i] >= letFreqs[i]))
                {
                    prob += (letFreqs[i]) * (letFreqs[i]); 
                }
            }
            return prob;
        }

        private double CheckInvalidComb(bool lang, string curText, double retu)
        {   //За каждое недопустимое сочетание обнуляем вероятность шифра
            string[] forbidPairs;
            if (lang)
            {
                forbidPairs = new string[] { "аъ", "аы", "аь", "бй", "вй", "вэ", "гй", "гф", "гх", "гъ", "гь", "гэ", "дй", "еъ", "еы", "еь", "еэ", "ёа", "ёе", "ёё", "ёи", "ёо", "ёу", "ёф",
                "ёъ", "ёъ", "ёы", "ёь", "ёэ", "ёя", "жё", "жф", "жх", "жш", "жщ", "жы", "жэ", "зй", "зщ", "иъ", "иы", "иь", "йа", "йё", "йж", "йъ", "йы", "йь", "йэ", "кй", "кщ", "къ", "кь",
                "лй", "лъ", "лэ", "мй", "мъ", "нй", "оъ", "оы", "оь", "пв", "пг", "пж", "пз", "пй", "пъ", "ръ", "сй", "тй", "уъ", "уы", "уь", "фб", "фж", "фз", "фй", "фп", "фх", "фц", "фъ",
                "фэ", "хё", "хж", "хй", "хщ", "хы", "хь", "хю", "хя", "цб", "цё", "цж", "цй", "цф", "цх", "цч", "цщ", "цъ", "ць", "цэ", "цю", "ця", "чб", "чг", "чз", "чй", "чп", "чф", "чщ",
                "чъ", "чы", "чэ", "чю", "чя", "шд", "шж", "шз", "шй", "шш", "шщ", "шъ", "шы", "шэ", "щб", "щг", "щд", "щж", "щз", "щй", "щл", "щп", "щт", "щф", "щх", "щц", "щч", "щш", "щщ",
                "щъ", "щы", "щэ", "щю", "щя", "ъа", "ъб", "ъв", "ъг", "ъд", "ъж", "ъз", "ъи", "ъё", "ък", "ъл", "ъм", "ън", "ъо", "ъп", "ър", "ъс", "ът", "ъу", "ъф", "ъх", "ъц", "ъч", "ъш",
                "ъщ", "ъъ", "ъы", "ъь", "ъэ", "ыа", "ыё", "ыо", "ыф", "ыъ", "ыы", "ыь", "ыэ", "ьа", "ьй", "ьл", "ьу", "ьъ", "ьы", "ьь", "эа", "эе", "эё", "эц", "эч", "эъ", "эы", "эь", "ээ",
                "эю", "юу", "юъ", "юы", "юь", "яа", "яё", "яо", "яъ", "яы", "яь", "яэ" };
            }
            else
            {
                forbidPairs = new string[] { "qa", "qb", "qc", "qd", "qe", "qf", "qg", "qh", "qi", "qj", "qk", "ql", "qm", "qn", "qo", "qp", "qq", "qr", "qs", "qt", "qv", "qw", "qx", "qy",
                "qz", "jx", "xz", "zx", "jz", "jx", "vx", "bx", "wx", "kx", "cx", "jq", "vq", "xq", "bq", "zq", "jy", "px", "zj", "hx", "bz", "jv", "pz", "xj", "kz" };
            }

            for (int i = 0; i < forbidPairs.Length; i++)
            {
                if (curText.IndexOf(forbidPairs[i]) >= 0)
                    return -1;
            }
            return retu;
        }

        private double CheckLetterPosition(bool lang, string curText, double retu)
        { // За каждую недопустимую букву в начале или конце слова обнуляем вероятность шифра
            var words = curText.Split(new char[] { ' ', '.', ',', ':', ';', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);
            string ruMayBeAlone = "авикосуя";
            string enMayBeAlone = "ai";
            for (int i = 0; i < words.Length; i++)
            {
                if (lang)
                {
                    if (words[i][0] == 'ъ' || words[i][0] == 'ь' || words[i][0] == 'ы') return -1; // для первой буквы
                    if (words[i][words[i].Length - 1] == 'ъ') return -1; // для последней буквы
                    if (words[i].Length == 1)
                    { // Одиночные буквы
                        if (ruMayBeAlone.IndexOf(words[i][0]) < 0) return -1;
                    }
                }
                else
                {
                    if (words[i][words[i].Length - 1] == 'v') return -1; //Последняя буква - v
                    if (words[i][words[i].Length - 1] == 'q') return -1; //Последняя буква - q

                    if (words[i].Length == 1)
                    {
                        if (enMayBeAlone.IndexOf(words[i][0]) < 0) return -1;
                    }
                }
            }
            return retu;
        }

        // Применение результата работы программы в поле предполагаемого сдвига
        private void ApplyK1Btn_Click(object sender, EventArgs e)
        {
            if (Key1TB.Text != "")
                ShiftNUD.Value = Convert.ToInt32(Key1TB.Text);
        }

        private void ApplyK2Btn_Click(object sender, EventArgs e)
        {
            if (Key2TB.Text != "")
                ShiftNUD.Value = Convert.ToInt32(Key2TB.Text);
        }

        private void ApplyK3Btn_Click(object sender, EventArgs e)
        {
            if (Key3TB.Text != "")
                ShiftNUD.Value = Convert.ToInt32(Key3TB.Text);
        }

        private void ClearInputLabel_Click(object sender, EventArgs e)
        {
            if (InputRTB.Text != "")
                InputRTB.Clear();
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
                    InputRTB.Clear();
                    InputRTB.Text = sr.ReadToEnd();
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
                    sw.WriteLine(OutputRTB.Text);
                    sw.Close();
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////
        string intendedAlpha = "";

        private void InputRTBSimp_TextChanged(object sender, EventArgs e)
        {
            ProposalTB.Clear();
            int[] ruLetters; // массивы с количеством вхождений для каждой буквы
            int[] enLetters;
            // Подсчет количества русских и англ. букв
            int countRu;
            int countEn;
            CountingEntrys(InputRTBSimp.Text.ToLower(), out countRu, out countEn, out ruLetters, out enLetters);

            double[] freqsInputRu = new double[ruLetters.Length];
            if (countRu > 0)
            {
                for (int i = 0; i < freqsInputRu.Length; i++)
                {
                    freqsInputRu[i] = (double)ruLetters[i] / countRu;
                }
            }

            double[] freqsInputEn = new double[enLetters.Length];
            if (countEn > 0)
            {
                for (int i = 0; i < freqsInputEn.Length; i++)
                {
                    freqsInputEn[i] = (double)enLetters[i] / countEn;
                }
            }

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
                    dataGridView.Rows[curInd].Cells[3].Value = Math.Round(freqsInputRu[i], 4);
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
                    dataGridView.Rows[curInd].Cells[3].Value = Math.Round(freqsInputEn[i], 4);
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
                    chart.Series[0].Points.AddXY(ruAlLo[i].ToString(), Math.Round(freqsInputRu[i], 4));
                }
            }
            if (countEn > 0)
            {
                for (int i = 0; i < enLetters.Length; i++)
                {
                    chart.Series[0].Points.AddXY(enAlLo[i].ToString(), Math.Round(freqsInputEn[i], 4));
                }
            }

            //сортировка
            if (countRu > 0)
            {
                double[] forSortFreq = (double[])ruLetFreqs.Clone();
                char[] forSortAlpha = ruAlLo.ToCharArray();
                char[] forSortInput = ruAlLo.ToCharArray();
                /*for (int c = 0; c < forSortInput.Length; c++)
                {
                    if (ruLetters[c] == 0)
                    {
                        forSortInput[c] = (char)129;
                    }
                }*/
                // сортировка
                for (int i = 0; i < forSortFreq.Length - 1; i++)
                {
                    for (int j = i + 1; j < forSortFreq.Length; j++)
                    {
                        double temp1;
                        char temp2;
                        if (forSortFreq[i] < forSortFreq[j])
                        {
                            temp1 = forSortFreq[i];
                            forSortFreq[i] = forSortFreq[j];
                            forSortFreq[j] = temp1;

                            temp2 = forSortAlpha[i];
                            forSortAlpha[i] = forSortAlpha[j];
                            forSortAlpha[j] = temp2;
                        }
                        if (freqsInputRu[i] < freqsInputRu[j])
                        {
                            temp1 = freqsInputRu[i];
                            freqsInputRu[i] = freqsInputRu[j];
                            freqsInputRu[j] = temp1;

                            temp2 = forSortInput[i];
                            forSortInput[i] = forSortInput[j];
                            forSortInput[j] = temp2;

                        }
                    }
                } // конец сортировки

                string sortedIn = new string(forSortInput);
                int ind = -1;
                intendedAlpha = "";
                for (int k = 0; k < ruAlLo.Length; k++)
                {
                    if (sortedIn.IndexOf(ruAlLo[k]) >= 0)
                    {
                        ind = sortedIn.IndexOf(ruAlLo[k]);
                        ProposalTB.Text += forSortInput[ind].ToString() + "  ->  " + forSortAlpha[ind].ToString() + "\r\n";
                        intendedAlpha += forSortAlpha[ind];
                    }
                }
            }

            if (countEn > 0)
            {
                double[] forSortFreq = (double[])enLetFreqs.Clone();
                char[] forSortAlpha = enAlLo.ToCharArray();
                char[] forSortInput = enAlLo.ToCharArray();
                /*for (int c = 0; c < forSortInput.Length; c++)
                {
                    if (ruLetters[c] == 0)
                    {
                        forSortInput[c] = (char)129;
                    }
                }*/
                // сортировка
                for (int i = 0; i < forSortFreq.Length - 1; i++)
                {
                    for (int j = i + 1; j < forSortFreq.Length; j++)
                    {
                        double temp1;
                        char temp2;
                        if (forSortFreq[i] < forSortFreq[j])
                        {
                            temp1 = forSortFreq[i];
                            forSortFreq[i] = forSortFreq[j];
                            forSortFreq[j] = temp1;

                            temp2 = forSortAlpha[i];
                            forSortAlpha[i] = forSortAlpha[j];
                            forSortAlpha[j] = temp2;
                        }
                        if (freqsInputEn[i] < freqsInputEn[j])
                        {
                            temp1 = freqsInputEn[i];
                            freqsInputEn[i] = freqsInputEn[j];
                            freqsInputEn[j] = temp1;

                            temp2 = forSortInput[i];
                            forSortInput[i] = forSortInput[j];
                            forSortInput[j] = temp2;

                        }
                    }
                } // конец сортировки

                string sortedIn = new string(forSortInput);
                int ind = -1;
                intendedAlpha = "";
                for (int k = 0; k < enAlLo.Length; k++)
                {
                    if (sortedIn.IndexOf(enAlLo[k]) >= 0)
                    {
                        ind = sortedIn.IndexOf(enAlLo[k]);
                        ProposalTB.Text += forSortInput[ind].ToString() + "  ->  " + forSortAlpha[ind].ToString() + "\r\n";
                        intendedAlpha += forSortAlpha[ind];
                    }
                }
            }
        }

        private void ReplaceLetterBtn_Click(object sender, EventArgs e)
        {
            OutputRTBSimp.Clear();

            for (int indSymb = 0; indSymb < InputRTBSimp.Text.Length; indSymb++)
            {
                OutputRTBSimp.Text = InputRTBSimp.Text.Replace(Convert.ToChar(OldLet.Text.ToLower()), Convert.ToChar(NewLet.Text.ToLower())).Replace(Convert.ToChar(OldLet.Text.ToUpper()), Convert.ToChar(NewLet.Text.ToUpper()));
            }
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

        private void ArrowBtn_Click(object sender, EventArgs e)
        {
            if (OutputRTBSimp.Text != "")
            {
                InputRTBSimp.Clear();
                InputRTBSimp.Text = OutputRTBSimp.Text;
            }
        }

        private void ClearInputSimp_Click(object sender, EventArgs e)
        {
            if (InputRTBSimp.Text != "")
                InputRTBSimp.Clear();
        }

        private void ReplaceAllBtn_Click(object sender, EventArgs e)
        {
            OutputRTBSimp.Clear();

            for (int indLet = 0; indLet < InputRTBSimp.Text.Length; indLet++)
            {
                char curLet = InputRTBSimp.Text[indLet];
                bool reg = Char.IsUpper(curLet);
                curLet = Convert.ToChar(curLet.ToString().ToLower());
                int tempInd;
                string curAlpha;
                if (intendedAlpha.Length == ruAlLo.Length)
                {
                    curAlpha = ruAlLo;
                }
                else
                {
                    curAlpha = enAlLo;
                }
                if (curAlpha.IndexOf(curLet) >= 0)
                {
                    tempInd = curAlpha.IndexOf(curLet);
                    if (reg) // Заглавная буква
                    {
                        OutputRTBSimp.Text += Convert.ToChar(intendedAlpha[tempInd].ToString().ToUpper());
                    }
                    else // прописная
                    {
                        OutputRTBSimp.Text += Convert.ToChar(intendedAlpha[tempInd].ToString().ToLower());
                    }                   
                }
                else
                {
                    OutputRTBSimp.Text += curLet;
                }
            }
        }

        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            CodeTB.Text = intendedAlpha;
        }

        private void Clear_Original_Click(object sender, EventArgs e)
        {
            if (OutputRTBSimp.Text != "")
                OutputRTBSimp.Clear();
        }
    }
}
