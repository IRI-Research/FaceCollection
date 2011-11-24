namespace VideoWallProject
{
    partial class PanelPreview
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelVideo = new System.Windows.Forms.Panel();
            this.progressBarPreview = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // panelVideo
            // 
            this.panelVideo.Location = new System.Drawing.Point(0, 0);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(800, 600);
            this.panelVideo.TabIndex = 0;
            // 
            // progressBarPreview
            // 
            this.progressBarPreview.BackColor = System.Drawing.Color.PaleTurquoise;
            this.progressBarPreview.ForeColor = System.Drawing.Color.Teal;
            this.progressBarPreview.Location = new System.Drawing.Point(-1, 601);
            this.progressBarPreview.Name = "progressBarPreview";
            this.progressBarPreview.Size = new System.Drawing.Size(800, 30);
            this.progressBarPreview.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarPreview.TabIndex = 1;
            // 
            // PanelPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 630);
            this.ControlBox = false;
            this.Controls.Add(this.progressBarPreview);
            this.Controls.Add(this.panelVideo);
            this.Name = "PanelPreview";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelVideo;
        private System.Windows.Forms.ProgressBar progressBarPreview;
    }
}