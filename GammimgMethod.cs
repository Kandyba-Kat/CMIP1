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
using System.Diagnostics;

namespace AtbashCipher
{
    public partial class GammimgMethod : Form
    {
        public GammimgMethod()
        {
            InitializeComponent();
        }

        bool txtFile = false;
        byte[] binInFile = new byte[0];
        byte[] codeFile = new byte[0];
        string gamma = "";
        string inPath = "";

        public static byte[] DoVernam(byte[] inB, byte[] keyB)
        {
            byte[] outB = new byte[inB.Length];

            for (int i = 0; i < outB.Length; i++)
            {
                outB[i] = (byte)(inB[i] ^ keyB[i]);
            }

            return outB;
        }

        public byte[] EncrypFile(byte[] origBytes, string key)
        {
            byte[] keyBytes;
            if (txtFile)
            {
                keyBytes = Encoding.GetEncoding(1251).GetBytes(key);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(key);
            }
            if (origBytes.Length == keyBytes.Length)
            {
                return DoVernam(origBytes, keyBytes);
            }
            else
            {
                return null;
            }
        }

        private void PrepareToCryptFile()
        {
            OutputRTB.Clear();
            if (binInFile.Length != 0)
            {
                if (gamma != "")
                {
                    if (gamma.Length < binInFile.Length)
                    {
                        DialogResult result1 = MessageBox.Show("Длина ключа короче длины текста. Продлить ключ?", "Сообщение", MessageBoxButtons.YesNo);
                        if (result1 == DialogResult.Yes)
                        {
                            int diff = binInFile.Length - gamma.Length;
                            int tempInd = 0;
                            while (diff != 0)
                            {
                                gamma = gamma + gamma[tempInd % gamma.Length];
                                tempInd++;
                                diff--;
                            }                           

                            codeFile = EncrypFile(binInFile, gamma);

                            if (codeFile == null)
                            {
                                MessageBox.Show("Ошибка при переводе в бинарные данные", "Ошибка", MessageBoxButtons.OK);
                            }
                            else
                            {
                                MessageBox.Show("Завершено успешно.", "Сообщение", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Некорректный ключ", "Предупреждение", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        if (gamma.Length > binInFile.Length)
                        {
                            DialogResult result2 = MessageBox.Show("Длина ключа больше длины текста. Обрезать ключ?", "Сообщение", MessageBoxButtons.YesNo);
                            if (result2 == DialogResult.Yes)
                            {
                                gamma = gamma.Substring(0, binInFile.Length);

                                codeFile = EncrypFile(binInFile, gamma);

                                if (codeFile == null)
                                {
                                    MessageBox.Show("Ошибка при переводе в бинарные данные", "Ошибка", MessageBoxButtons.OK);
                                }
                                else
                                {
                                    MessageBox.Show("Завершено успешно.", "Сообщение", MessageBoxButtons.OK);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Некорректный ключ", "Предупреждение", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            codeFile = EncrypFile(binInFile, gamma);

                            if (codeFile == null)
                            {
                                MessageBox.Show("Ошибка при переводе в бинарные данные", "Ошибка", MessageBoxButtons.OK);
                            }
                            else
                            {
                                MessageBox.Show("Завершено успешно.", "Сообщение", MessageBoxButtons.OK);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Отсутствует ключ", "Предупреждение", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Отсутствует входной текст", "Предупреждение", MessageBoxButtons.OK);
            }
        }

        private void PrepareToCryptText()
        {
            OutputRTB.Clear();
            string inputText = InputRTB.Text/*.Replace("\r\n", "\r")*/;
            if (inputText != "")
            {
                if (KeyTB.Text != "")
                {
                    if (KeyTB.Text.Length >= inputText.Length)
                    {
                        if (KeyTB.Text.Length > inputText.Length)
                        {
                            DialogResult result1 = MessageBox.Show("Длина ключа больше длины текста. Обрезать ключ?", "Сообщение", MessageBoxButtons.YesNo);
                            if (result1 == DialogResult.Yes)
                            {
                                KeyTB.Text = KeyTB.Text.Substring(0, inputText.Length);
                                var res = EncrypFile(Encoding.GetEncoding(1251).GetBytes(inputText), KeyTB.Text);
                                if (res == null)
                                {
                                    MessageBox.Show("Ошибка при переводе в бинарные данные", "Ошибка", MessageBoxButtons.OK);
                                }
                                else
                                {
                                    OutputRTB.Text = Encoding.GetEncoding(1251).GetString(res).Replace("\r", "\r\n");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Некорректный ключ", "Предупреждение", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            var res = EncrypFile(Encoding.GetEncoding(1251).GetBytes(inputText), KeyTB.Text);
                            if (res == null)
                            {
                                MessageBox.Show("Ошибка при переводе в бинарные данные", "Ошибка", MessageBoxButtons.OK);
                            }
                            else
                            {
                                OutputRTB.Text = Encoding.GetEncoding(1251).GetString(res).Replace("\r", "\r\n");
                            }
                        }
                    }
                    else
                    {
                        DialogResult result2 = MessageBox.Show("Длина ключа короче длины текста. Продлить ключ?", "Сообщение", MessageBoxButtons.YesNo);
                        if (result2 == DialogResult.Yes)
                        {
                            int diff = InputRTB.Text.Length - KeyTB.Text.Length;
                            int tempInd = 0;
                            while (diff != 0)
                            {
                                KeyTB.Text = KeyTB.Text + KeyTB.Text[tempInd % KeyTB.Text.Length];
                                tempInd++;
                                diff--;
                            }

                            var res = EncrypFile(Encoding.GetEncoding(1251).GetBytes(inputText), KeyTB.Text);
                            if (res == null)
                            {
                                MessageBox.Show("Ошибка при переводе в бинарные данные", "Ошибка", MessageBoxButtons.OK);
                            }
                            else
                            {
                                OutputRTB.Text = Encoding.GetEncoding(1251).GetString(res).Replace("\r", "\r\n");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Некорректный ключ", "Предупреждение", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Отсутствует ключ", "Предупреждение", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Отсутствует входной текст", "Предупреждение", MessageBoxButtons.OK);
            }
        }

        private void GammEncrypBtn_Click(object sender, EventArgs e)
        {
            if (txtFile)
                PrepareToCryptText();
            else
                PrepareToCryptFile();
        }

        private void GamDecrypBtn_Click(object sender, EventArgs e)
        {
            if (txtFile)
                PrepareToCryptText();
            else
                PrepareToCryptFile();
        }

        private void OpenFileStripMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFile.FilterIndex = 2;
            openFile.InitialDirectory = "C:\\Users\\Katya\\Desktop\\Study\\КМЗИ\\12and15";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    inPath = Path.GetExtension(openFile.FileName);
                    InputRTB.Clear();
                    if (inPath != ".txt")
                    {
                        using (FileStream fs = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read))
                        {
                            // преобразуем строку в байты
                            binInFile = new byte[Convert.ToInt32(fs.Length)];
                            // считываем данные
                            fs.Read(binInFile, 0, Convert.ToInt32(fs.Length));
                        }

                        InputRTB.ReadOnly = true;
                        txtFile = false;
                        MessageBox.Show("Файл успешно прочитан");
                    }
                    else
                    {
                        DialogResult result1 = MessageBox.Show("Отображать текст?", "Сообщение", MessageBoxButtons.YesNo);
                        if (result1 == DialogResult.Yes)
                        {
                            using (StreamReader sr = new StreamReader(openFile.FileName))
                            {
                                string temp = "";
                                while ((temp = sr.ReadLine()) != null)
                                {
                                    InputRTB.Text += temp + "\n";
                                }
                                if (temp == null)
                                    InputRTB.Text = InputRTB.Text.Remove(InputRTB.Text.Length - 1, 1);
                            }

                            InputRTB.ReadOnly = false;
                            txtFile = true;
                        }
                        else
                        {
                            using (FileStream fs = new FileStream(openFile.FileName, FileMode.Open, FileAccess.Read))
                            {
                                // преобразуем строку в байты
                                binInFile = new byte[Convert.ToInt32(fs.Length)];
                                // считываем данные
                                fs.Read(binInFile, 0, Convert.ToInt32(fs.Length));
                            }

                            InputRTB.ReadOnly = true;
                            txtFile = false;
                            MessageBox.Show("Файл успешно прочитан");
                        }


                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(string.Format("Файл не может быть прочитан.\n{0}", exc.Message), "Ошибка", MessageBoxButtons.OK);
                }
            }
        }

        private void SaveFileStripMenu_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = inPath;
            saveFile.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFile.FilterIndex = 2;
            saveFile.InitialDirectory = "C:\\Users\\Katya\\Desktop\\Study\\КМЗИ\\12and15";
            if (saveFile.ShowDialog() == DialogResult.OK && saveFile.FileName.Length > 0)
            {
                if (!txtFile)
                {
                    using (BinaryWriter br = new BinaryWriter(new FileStream(saveFile.FileName, FileMode.Create), Encoding.UTF8))
                    {
                        for (int i = 0; i < codeFile.Length; i++)
                        {
                            br.Write(codeFile[i]);
                        }
                        br.Flush();
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(saveFile.FileName, false)) // перезапись файла
                    {
                        sw.Write(OutputRTB.Text);
                    }
                }
                DialogResult result3 = MessageBox.Show("Файл успешно сохранен. Открыть файл?", "Сообщение", MessageBoxButtons.YesNo);
                if (result3 == DialogResult.Yes)
                {
                    Process.Start(saveFile.FileName);
                }
            }
        }

        private void Clear_Input_Click(object sender, EventArgs e)
        {
            InputRTB.Clear();
            InputRTB.ReadOnly = false;
            txtFile = false;
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeyTB.Clear();
            int seed = Convert.ToInt32(paramSnud.Value);
            int A = Convert.ToInt32(ParamAnud.Value);
            int C = Convert.ToInt32(paramCnud.Value);
            gamma = "";
            if (txtFile)
            {
                string input = InputRTB.Text/*.Replace("\r\n", "\r")*/;
                if (input != "")
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        seed = (int)(((A * seed + C) % paramMnud.Value % 256) + 50);
                        KeyTB.Text = KeyTB.Text + (char)seed;
                    }
                }
                else
                {
                    MessageBox.Show("Невозможно построить ключ.\nОтсутствует входной текст.", "Ошибка", MessageBoxButtons.OK);
                }
            }
            else
            {
                if (binInFile.Length == 0)
                {
                    MessageBox.Show("Входной файл отсутствует", "Ошибка", MessageBoxButtons.OK);
                    InputRTB.Clear();
                    InputRTB.ReadOnly = false;
                }
                else
                {
                    for (int i = 0; i < binInFile.Length; i++)
                    {
                        seed = (int)((A * seed + C) % paramMnud.Value % 256);
                        gamma = gamma + (char)seed;
                    }
                    MessageBox.Show("Ключ успешно создан");
                }
            }
        }

        private void Clear_Key_Click(object sender, EventArgs e)
        {
            KeyTB.Clear();
            KeyTB.ReadOnly = false;
        }

        private void ArrowBtn_Click(object sender, EventArgs e)
        {
            if (codeFile != null)
            {
                InputRTB.Clear();
                InputRTB.ReadOnly = false;
                if (txtFile)
                {
                    InputRTB.Text = OutputRTB.Text;
                    OutputRTB.Clear();
                }
                else
                {
                    binInFile = new byte[codeFile.Length];
                    Array.Copy(codeFile, binInFile, codeFile.Length);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveKey = new SaveFileDialog();
            saveKey.DefaultExt = ".txt";
            saveKey.Filter = "Text file|*.txt";
            //saveKey.InitialDirectory = "C:\\Users\\Katya\\Desktop\\Study\\КМЗИ\\12and15";
            if (saveKey.ShowDialog() == DialogResult.OK && saveKey.FileName.Length > 0)
            {
                using (StreamWriter sw = new StreamWriter(saveKey.FileName, false)) // перезапись файла
                {
                    sw.WriteLine(gamma);
                }
                MessageBox.Show("Ключ успешно сохранен");
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openKey = new OpenFileDialog();
            openKey.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openKey.FilterIndex = 2;
            openKey.InitialDirectory = "C:\\Users\\Katya\\Desktop\\Study\\КМЗИ\\12and15";
            if (openKey.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    KeyTB.Clear();
                    if (Path.GetExtension(openKey.FileName) != ".txt")
                    {
                        using (FileStream fs = new FileStream(openKey.FileName, FileMode.Open, FileAccess.Read))
                        {
                            // преобразуем строку в байты
                            binInFile = new byte[Convert.ToInt32(fs.Length)];
                            // считываем данные
                            fs.Read(binInFile, 0, Convert.ToInt32(fs.Length));
                        }

                        KeyTB.ReadOnly = true;
                        MessageBox.Show("Файл успешно прочитан");
                    }
                    else
                    {
                        using (StreamReader sr = new StreamReader(openKey.FileName))
                        {
                            string temp = "";
                            while ((temp = sr.ReadLine()) != null)
                            {
                                KeyTB.Text += temp;
                            }
                        }

                        KeyTB.ReadOnly = false;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(string.Format("Файл не может быть прочитан.\n{0}", exc.Message), "Ошибка", MessageBoxButtons.OK);
                }
            }
        }

        private void KeyTB_TextChanged(object sender, EventArgs e)
        {
            gamma = KeyTB.Text;
        }

        private void ResetParamsLabel_Click(object sender, EventArgs e)
        {
            ParamAnud.Value = 16807;
            paramCnud.Value = 0;
            paramMnud.Value = 2147483647;
            paramSnud.Value = 1;
        }

        private void ClearParamA_Click(object sender, EventArgs e)
        {
            ParamAnud.Value = 0;
        }

        private void ClearParamC_Click(object sender, EventArgs e)
        {
            paramCnud.Value = 0;
        }

        private void ClearParamM_Click(object sender, EventArgs e)
        {
            paramMnud.Value = 2;
        }

        private void ClearParamS_Click(object sender, EventArgs e)
        {
            paramSnud.Value = 0;
        }

        private void InputRTB_TextChanged(object sender, EventArgs e)
        {
            txtFile = true;
        }

        private void Clear_Output_Click(object sender, EventArgs e)
        {
            OutputRTB.Clear();
        }

        private void RandomParamAC_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            int A;
            int C;
            do
            {
                C = rand.Next();
            }
            while (!((isPrime(C)) && (C % 4 == 1) && (C % 3 == 1)));
            do
            {
                A = rand.Next();
            }
            while (!((isPrime(A)) && (A % 4 == 1) && (A % 3 == 1)));
            ParamAnud.Value = A;
            paramCnud.Value = C;
            paramSnud.Value = rand.Next();
        }

        public static bool isPrime(int n)
        {
            // all integers less than 1 (1 is included) are not prime
            if (n <= 1)
                return false;

            // Error in your code: 2 is prime, even if other even numbers aren't
            if (n % 2 == 0)
                return (n == 2);

            // there's no need to loop up to n: sqrt(n) is quite enough
            int max = (int)(Math.Sqrt(n) + 0.1);

            // skip even numbers when looping: i +=2
            for (int i = 3; i <= max; i += 2)
            {
                // the early return the better
                if (n % i == 0)
                    return false;
            }
            return true;
        }
    }
}
