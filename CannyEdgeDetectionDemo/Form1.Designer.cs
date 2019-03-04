namespace CannyEdgeDetectionDemo
{
    partial class MainForm
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
            this.LoadImageBtn = new System.Windows.Forms.Button();
            this.LoadedImageBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.LoadedImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadImageBtn
            // 
            this.LoadImageBtn.Location = new System.Drawing.Point(12, 12);
            this.LoadImageBtn.Name = "LoadImageBtn";
            this.LoadImageBtn.Size = new System.Drawing.Size(348, 96);
            this.LoadImageBtn.TabIndex = 0;
            this.LoadImageBtn.Text = "Load Image";
            this.LoadImageBtn.UseVisualStyleBackColor = true;
            this.LoadImageBtn.Click += new System.EventHandler(this.LoadImageBtn_Click);
            // 
            // LoadedImageBox
            // 
            this.LoadedImageBox.Location = new System.Drawing.Point(12, 114);
            this.LoadedImageBox.Name = "LoadedImageBox";
            this.LoadedImageBox.Size = new System.Drawing.Size(761, 595);
            this.LoadedImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.LoadedImageBox.TabIndex = 1;
            this.LoadedImageBox.TabStop = false;
            this.LoadedImageBox.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.LoadedImageBox_LoadCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(785, 721);
            this.Controls.Add(this.LoadedImageBox);
            this.Controls.Add(this.LoadImageBtn);
            this.Name = "MainForm";
            this.Text = "Canny Edge Detection Demo";
            ((System.ComponentModel.ISupportInitialize)(this.LoadedImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadImageBtn;
        private System.Windows.Forms.PictureBox LoadedImageBox;
    }
}

