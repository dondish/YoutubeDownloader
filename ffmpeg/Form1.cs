using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutubeDownloader
{
    public partial class Form1 : Form
    {
        private Process downloadproc = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void URLTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                new Uri(URLTextBox.Text);
                errorProvider1.SetError(URLTextBox, null);
            }
            catch (UriFormatException er)
            {
                errorProvider1.SetError(URLTextBox, "This Url is not valid!");
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderDiag.ShowDialog() == DialogResult.OK)
            {
                string path = folderDiag.SelectedPath;
                Console.WriteLine(path);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void download_Click(object sender, EventArgs e)
        {
            label3.Visible = false;
            string path = folderDiag.SelectedPath;
            if (path == "")
            {
                errorProvider1.SetError(button1, "Path Not Defined!");
                return;
            }
            errorProvider1.SetError(button1, null);
            Console.WriteLine(errorProvider1.GetError(URLTextBox));
            if (errorProvider1.GetError(URLTextBox) != "")
            {
                errorProvider1.SetError(URLTextBox, "This Url is not valid!");
                return;
            }
            if (URLTextBox.Text == "")
            {
                errorProvider1.SetError(URLTextBox, "URL is required!");
                return;
            }
            errorProvider1.SetError(URLTextBox, null);
            download.Enabled = false;
            RadioButton b = Controls.OfType<RadioButton>()
                .FirstOrDefault(r => r.Checked);
            if (b == null)
            {
                errorProvider1.SetError(download, "No encoding was selected!");
                return;
            }
            errorProvider1.SetError(download, null);
            string extension = b.Text;

            Console.WriteLine(path + "\\" + @"%(title)s-%(uploader)s-%(id)s" + "." + extension);
            DownloadProg.Visible = true;
            label5.Visible = true;
            Cancel.Visible = true;
            downloader.RunWorkerAsync(new string[]{extension, path + "\\" + @"%(title)s-%(uploader)s-%(id)s.%(ext)s", URLTextBox.Text});
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void downloader_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] data = (string[]) e.Argument;
            string encoding = data[0];
            string dest = data[1];
            string videourl = data[2];
            string arg = String.Format("-f best[ext={0}]/(bestvideo+bestaudio)/best -o \"{1}.%(ext)s\" {2}", encoding, dest, videourl);
            ProcessStartInfo startInfo = new ProcessStartInfo("youtube_dl.exe", arg);
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            downloadproc = new Process { StartInfo = startInfo };
            BackgroundWorker worker = sender as BackgroundWorker;

            downloadproc.Start();

            while (!downloadproc.HasExited||!downloadproc.StandardOutput.EndOfStream)
            {
                string line = downloadproc.StandardOutput.ReadLine();
                if (line == null || !line.Contains("%"))
                    continue;
                string dat = line.Substring(11);
                worker.ReportProgress((int)Double.Parse(dat.Substring(0, dat.LastIndexOf("%"))), dat);
                Thread.Sleep(1);
            }
        }

        private void downloader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadProg.Value = e.ProgressPercentage;
            label5.Text = (string) e.UserState;
        }

        private void downloader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label5.Visible = false;
            DownloadProg.Visible = false;
            DownloadProg.Value = 0;
            label3.Visible = true;
            download.Enabled = true;
            Cancel.Visible = false;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            downloadproc.Kill();
        }
    }
}
