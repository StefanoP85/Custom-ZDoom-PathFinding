namespace ZDoomNavmesh
{
    partial class FormViewNavMesh
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
            this.TextBoxNavMesh = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TextBoxNavMesh
            // 
            this.TextBoxNavMesh.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxNavMesh.Location = new System.Drawing.Point(12, 12);
            this.TextBoxNavMesh.Multiline = true;
            this.TextBoxNavMesh.Name = "TextBoxNavMesh";
            this.TextBoxNavMesh.ReadOnly = true;
            this.TextBoxNavMesh.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxNavMesh.Size = new System.Drawing.Size(598, 409);
            this.TextBoxNavMesh.TabIndex = 0;
            // 
            // FormViewNavMesh
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 433);
            this.Controls.Add(this.TextBoxNavMesh);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "FormViewNavMesh";
            this.Text = "Navigation mesh";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxNavMesh;
    }
}