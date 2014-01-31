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

        // progress bar control funcs
        public void setProgressBarMax(int steps) { progressBar1.Maximum = steps; }
        public void setProgressBarVal(int value) { progressBar1.Value = value; }
        public int getProgressBarVal() { return progressBar1.Value; }
        public void incProgressBar() { progressBar1.Value++; }

        public void log(string msg)
        {
            detailInfoBar.AppendText(msg + Environment.NewLine);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set windows title
            this.Text += " [" + SOURCE_VERSION + " -> " + DEST_VERSION + "]";

            // set window height
            this.Height = 170;

            //check game
            string checkMsg = "Game Check - ";
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
            checkMsg = "Version Check - ";
            if (version != SOURCE_VERSION)
            {
                log(checkMsg + "Error - source version: " + SOURCE_VERSION + ", local version: " + version + ".");
                statusBar.Text = "Error: Wrong Version!";
                updateButton.Enabled = false;
                MessageBox.Show("This patch is for [" + SOURCE_VERSION + "], but your local version is [" + version + "].");
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

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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
            log("Start Patch...");
            Updater patcher = new Updater(this, "update.mpq", "Nirvana.mpq");
            patcher.Delete(patcher.deletelist);
            patcher.AddAll();
            patcher.Flush();
            patcher.Close();
        }
    }
}
