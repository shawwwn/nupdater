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
        public const string SOURCE_VERSION = "beta_2";
        public const string DEST_VERSION = "beta_3";
        public string gamePath;

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
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set windows props
            this.Text += " [" + SOURCE_VERSION + " -> " + DEST_VERSION + "]";
            statusBar.Text = "Initializing...";

            // set window height
            this.Height = 170;

            //check game
            string checkMsg = "[Game Check] - ";
            string gamePath = Utils.getGamePath();
            if (gamePath == "")
            {
                log(checkMsg + "Error - nirvana not found.");
                statusBar.Text = "Error: Nirvana Not Found!"; ;
                updateButton.Enabled = false;
                return;
            }
            else
                log(checkMsg + "Success!");
            // check version
            string version = Utils.getGameVersion(gamePath);
            checkMsg = "[Version Check] - ";
            if (version != SOURCE_VERSION)
            {
                log(checkMsg + "Error - source version: " + SOURCE_VERSION + ", target version: " + version + ".");
                statusBar.Text = "Error: Wrong Version!";
                updateButton.Enabled = false;
                MessageBox.Show("This patch is for Nirvana (" + SOURCE_VERSION + "), but your local version is (" + version + ").", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                log(checkMsg + "Success!");
                statusBar.Text = "Ready to Patch!";
                log("Ready to Patch - " + gamePath);
            }

            // patch start
        }

        private void detailButton_Click(object sender, EventArgs e)
        {
            if (this.Height == 280)
            {
                this.Height = 170;
                detailButton.Text = "Detail ˅";
            }
            else
            {
                this.Height = 280;
                detailButton.Text = "Detail ˄";
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
            Updater patcher = new Updater(this, "update.mpq", "Nirvana.mpq", true);
            patcher.DeleteAll();
            patcher.AddAll();
            patcher.Flush();
            patcher.Compact();
            patcher.Close();
            statusBar.Text = "Finish!";
            updateButton.Enabled = true;
            exitButton.Enabled = true;
            detailButton.Enabled = true;
            log("[Update Finish]");
            DialogResult result = MessageBox.Show("Update Finished!\nDo you wish to exit?", "Finish", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                Application.DoEvents();
                this.Close();
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
