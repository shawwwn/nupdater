namespace NUpdate
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.statusBar = new System.Windows.Forms.Label();
            this.detailInfoBar = new System.Windows.Forms.TextBox();
            this.detailButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(14, 41);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(413, 49);
            this.progressBar1.TabIndex = 1;
            // 
            // statusBar
            // 
            this.statusBar.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBar.Location = new System.Drawing.Point(10, 11);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(409, 26);
            this.statusBar.TabIndex = 2;
            this.statusBar.Text = "Status";
            this.statusBar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // detailInfoBar
            // 
            this.detailInfoBar.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.detailInfoBar.Location = new System.Drawing.Point(14, 165);
            this.detailInfoBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.detailInfoBar.Multiline = true;
            this.detailInfoBar.Name = "detailInfoBar";
            this.detailInfoBar.ReadOnly = true;
            this.detailInfoBar.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.detailInfoBar.Size = new System.Drawing.Size(413, 75);
            this.detailInfoBar.TabIndex = 3;
            // 
            // detailButton
            // 
            this.detailButton.Location = new System.Drawing.Point(336, 101);
            this.detailButton.Name = "detailButton";
            this.detailButton.Size = new System.Drawing.Size(91, 30);
            this.detailButton.TabIndex = 4;
            this.detailButton.Text = "Detail ˅";
            this.detailButton.UseVisualStyleBackColor = true;
            this.detailButton.Click += new System.EventHandler(this.detailButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(14, 101);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(90, 30);
            this.updateButton.TabIndex = 5;
            this.updateButton.Text = "Patch";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Location = new System.Drawing.Point(110, 101);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(91, 30);
            this.exitButton.TabIndex = 6;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 253);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.detailButton);
            this.Controls.Add(this.detailInfoBar);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.progressBar1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nirvana Patcher";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label statusBar;
        private System.Windows.Forms.TextBox detailInfoBar;
        private System.Windows.Forms.Button detailButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button exitButton;
    }
}

