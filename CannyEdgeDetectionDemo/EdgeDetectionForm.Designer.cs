namespace CannyEdgeDetectionDemo
{
    partial class EdgeDetectionForm
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
            this.EdgeDetectionImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.EdgeDetectionImage)).BeginInit();
            this.SuspendLayout();
            // 
            // EdgeDetectionImage
            // 
            this.EdgeDetectionImage.Location = new System.Drawing.Point(12, 12);
            this.EdgeDetectionImage.Name = "EdgeDetectionImage";
            this.EdgeDetectionImage.Size = new System.Drawing.Size(100, 50);
            this.EdgeDetectionImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.EdgeDetectionImage.TabIndex = 0;
            this.EdgeDetectionImage.TabStop = false;
            // 
            // EdgeDetectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.EdgeDetectionImage);
            this.Name = "EdgeDetectionForm";
            this.Text = "EdgeDetectionForm";
            ((System.ComponentModel.ISupportInitialize)(this.EdgeDetectionImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox EdgeDetectionImage;
    }
}