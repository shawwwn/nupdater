using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MpqLib;
using System.Reflection;
using System.Resources;
using System.IO;

namespace NUpdate
{
    public partial class Form1 : Form
    {
        // language bool
        private bool is_zhCN;

        public string SOURCE_VERSION;
        public string DEST_VERSION;
        public const string PATCH_MPQ_NAME = "update.mpq";
        public string appPath;
        public string gamePath;
        public string mpqPath;

        // progress bar setting funcs
        public void setProgressBarMax(int steps) { progressBar1.Maximum = steps; }
        public void setProgressBarVal(int value) { progressBar1.Value = value; }
        public int getProgressBarVal() { return progressBar1.Value; }
        public void incProgressBar() { progressBar1.Value++; }

        // logging funcs
        public void log(string msg) { detailInfoBar.AppendText(msg + Environment.NewLine); }
        public void setStatus(string msg) { statusBar.Text = msg; }

        public Form1()
        {
            is_zhCN = System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh-CN";
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            appPath = Application.StartupPath + '\\';
            statusBar.Text = "Initializing...";
            gamePath = Utils.getGamePath();
            mpqPath = gamePath + "Nirvana.mpq";

            // set version variables
            Updater patcher = new Updater(null, PATCH_MPQ_NAME, mpqPath, gamePath, true);
            SOURCE_VERSION = patcher.GetSourceVersion();
            DEST_VERSION = patcher.GetDestVersion();
            patcher.Close();

            // set windows props
            //this.Text += " [" + SOURCE_VERSION + " -> " + DEST_VERSION + "]";

            // set window height
            this.Height = 170;

            //check game
            string checkMsg = "[Game Check] - ";
            if (gamePath == "")
            {
                log(checkMsg + "Error - nirvana not found.");
                if (!is_zhCN)
                {
                    statusBar.Text = "Error: Nirvana Not Found!";
                }
                else
                {
                    statusBar.Text = "错误: 找不到Nirvana!";
                }
                updateButton.Enabled = false;
                return;
            }
            else
                log(checkMsg + "Success!");

            // check version
            checkMsg = "[Version Check] - ";
            string version = Utils.getGameVersion(gamePath);
            //if (version.Trim() == "") version = "beta";
            //string[] srcVersions=SOURCE_VERSION.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            if (version != SOURCE_VERSION)
            //if (!srcVersions.Contains(version))
            {
                log(checkMsg + "Error - source version: " + SOURCE_VERSION + ", target version: " + version + ".");
                if (!is_zhCN)
                {
                    statusBar.Text = "Error: Wrong Version!";
                }
                else
                {
                    statusBar.Text = "错误: 该补丁不支持当前版本!";
                }
                updateButton.Enabled = false;
                if (!is_zhCN)
                {
                    MessageBox.Show("This patch is for Nirvana (" + SOURCE_VERSION + "), but your local version is (" + version + ").", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("该补丁需要 Nirvana (" + SOURCE_VERSION + ") 版本, 您当前的版本为 (" + version + ").", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }
            else
            {
                log(checkMsg + "Success!");
                if (!is_zhCN)
                {
                    statusBar.Text = "Ready to Patch!";
                }
                else
                {
                    statusBar.Text = "补丁准备就绪!";
                }
                log("Ready to Patch - " + gamePath);
            }

            // patch start
        }

        private void detailButton_Click(object sender, EventArgs e)
        {
            if (this.Height == 280)
            {
                this.Height = 170;
                if (!is_zhCN)
                {
                    detailButton.Text = "Detail ˅";
                }
                else
                {
                    detailButton.Text = "详细进程 ˅";
                }
            }
            else
            {
                this.Height = 280;
                if (!is_zhCN)
                {
                    detailButton.Text = "Detail ˄";
                }
                else
                {
                    detailButton.Text = "详细进程 ˄";
                }
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            log("[Update Start]");
            statusBar.Text = "Patching...";
            detailInfoBar.Text = "";    // reset log text
            updateButton.Enabled = false;
            exitButton.Enabled = false;
            detailButton.Enabled = false;
            Updater patcher = new Updater(this, PATCH_MPQ_NAME, mpqPath, gamePath, true);
            patcher.DeleteAll();
            patcher.ExportAll();
            patcher.AddAll();
            patcher.Flush();
            patcher.Compact();
            patcher.Close();
            patcher.EncryptMpq();
            if (!is_zhCN) statusBar.Text = "Finish!";
            else statusBar.Text = "完成!";
            updateButton.Enabled = true;
            exitButton.Enabled = true;
            detailButton.Enabled = true;
            log("[Update Finish]");
            updateButton.Enabled = false;
            DialogResult result;
            if (!is_zhCN)
            {
                result = MessageBox.Show("Update Finished!\nDo you wish to exit?", "Finish", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                result = MessageBox.Show("更新完成!是否退出?", "完成", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            if (result == DialogResult.OK)
            {
                Application.DoEvents();
                this.Close();
                this.Dispose();
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            this.Close();
            this.Dispose();
        }
    }
}
