using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AtbashCipher
{
    public partial class VernamCipher : Form
    {
        public VernamCipher()
        {
            InitializeComponent();
        }

        public static string DoVernam(byte[] inB, byte[] keyB)
        {
            byte[] outB = new byte[inB.Length];

            for (int i = 0; i < outB.Length; i++)
            {
                outB[i] = (byte)(inB[i] ^ keyB[i]);           
            }
            var outString = Encoding.GetEncoding(1251).GetString(outB);
            return outString;          
        }

        public string EncrypFile(string origin, string key)
        {
            var origBytes = Encoding.GetEncoding(1251).GetBytes(origin);
            var keyBytes = Encoding.GetEncoding(1251).GetBytes(key);
            if (origBytes.Length == keyBytes.Length)
            {

                return DoVernam(origBytes, keyBytes).Replace("\r", "\r\n");
            }
            else
            {
                return null;
            }           
        }

        private void PrepareToCrypt()
        {
            OutputTB.Clear();
            string inputText = InputTB.Text.Replace("\r\n", "\r");
            if (inputText != "")
            {
                if (KeyTB.Text != "")
                {
                    if (KeyTB.Text.Length >= inputText.Length)
                    {
                        if (KeyTB.Text.Length > inputText.Length)
                        {
                            DialogResult result = MessageBox.Show("Длина ключа больше длины текста. Обрезать ключ?", "Сообщение", MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                KeyTB.Text = KeyTB.Text.Substring(0, inputText.Length);
                                string res = EncrypFile(inputText, KeyTB.Text);
                                if (res == null)
                                {
                                    MessageBox.Show("Ошибка при переводе в бинарные данные", "Ошибка", MessageBoxButtons.OK);
                                }
                                else
                                {
                                    OutputTB.Text = res;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Некорректный ключ", "Предупреждение", MessageBoxButtons.OK);
                            }
                        }
                        else
                        {
                            string res = EncrypFile(inputText, KeyTB.Text);
                            if (res == null)
                            {
                                MessageBox.Show("Ошибка при переводе в бинарные данные", "Ошибка", MessageBoxButtons.OK);
                            }
                            else
                            {
                                OutputTB.Text = res;
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show("Длина ключа короче длины текста", "Ошибка", MessageBoxButtons.OK);
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

        private void VerEncrypBtn_Click(object sender, EventArgs e)
        {
            PrepareToCrypt();
        }

        private void VerDecrypBtn_Click(object sender, EventArgs e)
        {
            PrepareToCrypt();
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = InputTB.Text.Replace("\r\n", "\r");
            if (input != "")
            {
                Random rand = new Random();

                KeyTB.Clear();
                for (int i = 0; i < input.Length; i++)
                {
                    //int cur1 = (int)(Math.Floor(rand.NextDouble() * 100000000) % 256);
                    int cur2 = (int)rand.Next(80, 300);
                    KeyTB.Text += (char)cur2;
                }
            }
            else
            {
                MessageBox.Show("Невозможно построить ключ.\nОтсутствует входной текст.", "Ошибка", MessageBoxButtons.OK);
            }
        }
         
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openKey = new OpenFileDialog();
            openKey.Filter = "Text file|*.txt";
            openKey.DefaultExt = ".txt";
            openKey.InitialDirectory = "C:\\Users\\Katya\\Desktop\\Study\\КМЗИ\\12and15";
            if (openKey.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(openKey.FileName))
                    {
                        KeyTB.Clear();
                        string temp = "";
                        while ((temp = sr.ReadLine()) != null)
                        {
                            KeyTB.Text += temp;
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(string.Format("Файл не может быть прочитан.\n{0}", exc.Message), "Ошибка", MessageBoxButtons.OK);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveKey = new SaveFileDialog();
            saveKey.DefaultExt = ".txt";
            saveKey.Filter = "Text file|*.txt";
            saveKey.InitialDirectory = "C:\\Users\\Katya\\Desktop\\Study\\КМЗИ\\12and15";
            if (saveKey.ShowDialog() == DialogResult.OK && saveKey.FileName.Length > 0)
            {
                using (StreamWriter sw = new StreamWriter(saveKey.FileName, false)) // перезапись файла
                {
                    //sw.WriteLine(KeyTB.Text);
                    sw.Write(KeyTB.Text);
                }
            }
        }

        private void OpenFileStripMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text file|*.txt";
            openFile.DefaultExt = ".txt";
            openFile.InitialDirectory = "C:\\Users\\Katya\\Desktop\\Study\\КМЗИ\\12and15";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(openFile.FileName))
                    {
                        InputTB.Clear();
                        string temp = "";
                        while ((temp = sr.ReadLine()) != null)
                        {
                            InputTB.Text += temp + "\r\n";                           
                        }
                        if (temp == null)
                            InputTB.Text = InputTB.Text.Remove(InputTB.Text.Length - 2, 2);
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
            saveFile.DefaultExt = ".txt";
            saveFile.Filter = "Text file|*.txt";
            saveFile.InitialDirectory = "C:\\Users\\Katya\\Desktop\\Study\\КМЗИ\\12and15";
            if (saveFile.ShowDialog() == DialogResult.OK && saveFile.FileName.Length > 0)
            {
                using (StreamWriter sw = new StreamWriter(saveFile.FileName, false)) // перезапись файла
                {
                    //sw.WriteLine(OutputTB.Text);
                    sw.Write(OutputTB.Text);
                }
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

        private void Clear_Input_Click(object sender, EventArgs e)
        {
            InputTB.Clear();
        }

        private void Clear_Key_Click(object sender, EventArgs e)
        {
            KeyTB.Clear();
        }

        private void ClearOutput_Click(object sender, EventArgs e)
        {
            OutputTB.Clear();
        }
    }
}
