namespace VideoCompressor
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        string inputFilePath = "";
        string outputFilePath = "";
        string selectedQuality = "İyi"; // Default kalite seçeneği

        public Form1()
        {
            InitializeComponent();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            // Kalite seçeneklerini comboBox'a ekleyelim
            qualityComboBox.Items.Add("En İyi");
            qualityComboBox.Items.Add("İyi");
            qualityComboBox.Items.Add("Orta");
            qualityComboBox.Items.Add("Kötü");
            qualityComboBox.SelectedIndex = 1; // Default "İyi"
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Video Files (*.mp4;*.avi;*.mkv)|*.mp4;*.avi;*.mkv",
                Title = "Sıkıştırmak için bir video seçin"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                inputFilePath = openFileDialog.FileName;
                outputFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath), "compressed_" + Path.GetFileName(inputFilePath));
                label1.Text = "Seçilen Dosya: " + inputFilePath;
            }
        }

        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("Lütfen önce bir video seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kaliteyi ComboBox'tan alıyoruz
            selectedQuality = qualityComboBox.SelectedItem.ToString();

            // Arka planda işlemi başlatıyoruz
            backgroundWorker.RunWorkerAsync();
            button2.Text = "İşlem Bitti";
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string ffmpegPath = @"C:\Users\user\Downloads\ffmpeg-master-latest-win64-gpl\bin\ffmpeg.exe"; // FFmpeg yolunu buraya ekleyin
            string crfValue = GetCrfValue(selectedQuality); // Kaliteye göre CRF değeri

            string arguments = $"-i \"{inputFilePath}\" -vcodec libx264 -crf {crfValue} -preset medium \"{outputFilePath}\"";

            Process process = new Process();
            process.StartInfo.FileName = ffmpegPath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;

            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetCrfValue(string quality)
        {
            // Kaliteye göre CRF değerini döndürür
            switch (quality)
            {
                case "En İyi":
                    return "18"; // En yüksek kalite
                case "İyi":
                    return "23"; // Orta kalite
                case "Orta":
                    return "28"; // Düşük kalite
                case "Kötü":
                    return "30"; // En düşük kalite
                default:
                    return "23"; // Varsayılan "İyi" kalitesi
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Video sıkıştırma tamamlandığında mesaj göster
            MessageBox.Show("Video sıkıştırma işlemi tamamlandı.\nÇıktı: " + outputFilePath, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
