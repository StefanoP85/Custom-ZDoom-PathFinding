namespace ZDoomNavMesh
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
            TextBoxNavMesh = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // TextBoxNavMesh
            // 
            TextBoxNavMesh.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxNavMesh.Location = new System.Drawing.Point(12, 13);
            TextBoxNavMesh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxNavMesh.Multiline = true;
            TextBoxNavMesh.Name = "TextBoxNavMesh";
            TextBoxNavMesh.ReadOnly = true;
            TextBoxNavMesh.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            TextBoxNavMesh.Size = new System.Drawing.Size(676, 364);
            TextBoxNavMesh.TabIndex = 8;
            // 
            // FormViewNavMesh
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(700, 390);
            Controls.Add(TextBoxNavMesh);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MinimumSize = new System.Drawing.Size(718, 437);
            Name = "FormViewNavMesh";
            Text = "View messages from the NavMesh";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxNavMesh;
    }
}