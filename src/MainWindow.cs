using System.Diagnostics;
using System.Net;

namespace Golden_Apple
{
    public partial class MainWindow : Form
    {
        private WebClient Client = new WebClient();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            Client.DownloadProgressChanged += Client_DownloadProgressChanged;
        }
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        private async Task BgTask()
        {
            while (true)
            {
                if (!File.Exists("config.txt"))
                    File.WriteAllText("config.txt", "0");
                try
                {
                    string Result = Client.DownloadString(new Uri("https://apps.goncermor.com/GoldenApple/Alfa0/update.txt"));
                    if (Result.Split("<::>")[0] != File.ReadAllText("config.txt"))
                    {
                        this.Show();
                        await Task.Delay(1000);
                        Client.DownloadFileAsync(new Uri(Result.Split("<::>")[1]), "update.exe");
                        while (Client.IsBusy)
                        {
                            await Task.Delay(50);
                            Application.DoEvents();
                        }
                        await Task.Delay(1000);
                        File.WriteAllText("config.txt", Result.Split("<::>")[0]);
                        Process.Start("update.exe");
                        this.Hide();
                    }
                }
                catch (Exception e)
                {
                    this.Hide();
                }
                await Task.Delay(5000);
            }
        }
        private void MainWindow_Shown(object sender, EventArgs e)
        {
            this.Hide();
            _ = BgTask();
        }
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
