using ReplicateDirectory;
using ReplicateApp.Helpers;
using System.ComponentModel;

namespace ReplicateApp
{
    public partial class Form1 : Form
    {
        BackgroundWorker worker = new BackgroundWorker();
        public Form1()
        {
            InitializeComponent();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            worker.ProgressChanged += Worker_ProgressChanged;
            worker.DoWork += Worker_DoWork;
        }

        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            ReplicateFile();
        }

        private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            lblPercentage.Text = progressBar1.Value.ToString() + "%";
            if (progressBar1.Value == 100)
            {
                progressBar1.Value = 0;
                MessageBox.Show("Replicate Complete!");
            }
        }

        async void ReplicateFile()
        {
            
        }

        public void FolderBrowserDialogUpdateTextBox(TextBox textBox)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox.Text = fbd.SelectedPath;

                }
            }
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogUpdateTextBox(Source);
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            FolderBrowserDialogUpdateTextBox(Target);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            //Closes Application on button Exit click
            if (MessageBox.Show("This will close the application. Confirm?", "Close Application", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MessageBox.Show("The application has been closed successfully.", "Application Closed!", MessageBoxButtons.OK);
                Application.Exit();
            }
            else
            {
                this.Activate();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //worker.RunWorkerAsync();
            if (!string.IsNullOrEmpty(Source.Text) && !string.IsNullOrEmpty(Target.Text))
            {
                DirectoryInfo source = new DirectoryInfo(Source.Text);
                DirectoryInfo target = new DirectoryInfo(Target.Text);

                string result = "";
                button1.Enabled = false;

                await Task.Run(() => CopyTask());
                void CopyTask()
                {
                    CopyFunctions copyFunc = new CopyFunctions();
                    copyFunc.FileUpdate += AddProgressToListView;
                    result = copyFunc.CopyDirectory(source, target);
                }
                ListViewItem lvi = new ListViewItem();

                if (result == "Success")
                {
                    lvi.Text = "Copying completed.";
                    LogsList.Items.Add(lvi);
                    MessageBox.Show("Replecated successfully.");
                }
                else if (result == "Same directory")
                {
                    lvi.Text = "The directories chosen are the same.";
                    LogsList.Items.Add(lvi);
                    MessageBox.Show("The directories chosen are the same.");
                }
                else if (result == "Error")
                {
                    lvi.Text = "There was an error.";
                    LogsList.Items.Add(lvi);
                    MessageBox.Show("There was an error.");
                }
            }
            else
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = "Folder not chosen.";
                LogsList.Items.Add(lvi);
            }
            button1.Enabled = true;
        }

        delegate void AddListViewItemCallback(string text);

        public void AddProgressToListView(string message)
        {
            ThreadHelper.AddViewItem(this, LogsList, message);
        }
    }
}