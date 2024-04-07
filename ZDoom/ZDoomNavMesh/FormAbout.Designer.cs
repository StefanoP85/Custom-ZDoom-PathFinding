namespace ZDoomNavMesh
{
    partial class FormAbout
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
            LabelTitle = new System.Windows.Forms.Label();
            LabelAbout0 = new System.Windows.Forms.Label();
            LabelAbout1 = new System.Windows.Forms.Label();
            LabelAbout2 = new System.Windows.Forms.Label();
            LabelAbout3 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // LabelTitle
            // 
            LabelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            LabelTitle.Location = new System.Drawing.Point(12, 9);
            LabelTitle.Name = "LabelTitle";
            LabelTitle.Size = new System.Drawing.Size(598, 48);
            LabelTitle.TabIndex = 0;
            LabelTitle.Text = "ZDoom Navigation Mesh builder";
            LabelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabelAbout0
            // 
            LabelAbout0.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            LabelAbout0.Location = new System.Drawing.Point(12, 89);
            LabelAbout0.Name = "LabelAbout0";
            LabelAbout0.Size = new System.Drawing.Size(598, 48);
            LabelAbout0.TabIndex = 1;
            LabelAbout0.Text = "This application creates NavMesh from ZDoom maps and saves them in text form";
            LabelAbout0.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelAbout1
            // 
            LabelAbout1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            LabelAbout1.Location = new System.Drawing.Point(12, 169);
            LabelAbout1.Name = "LabelAbout1";
            LabelAbout1.Size = new System.Drawing.Size(598, 48);
            LabelAbout1.TabIndex = 2;
            LabelAbout1.Text = "Valid maps supported are WAD, PK3, ZIP and TXT text files (text for UDMF only)";
            LabelAbout1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelAbout2
            // 
            LabelAbout2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            LabelAbout2.Location = new System.Drawing.Point(12, 249);
            LabelAbout2.Name = "LabelAbout2";
            LabelAbout2.Size = new System.Drawing.Size(598, 48);
            LabelAbout2.TabIndex = 3;
            LabelAbout2.Text = "Source code and Wiki documents at the following link:";
            LabelAbout2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelAbout3
            // 
            LabelAbout3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            LabelAbout3.Location = new System.Drawing.Point(12, 329);
            LabelAbout3.Name = "LabelAbout3";
            LabelAbout3.Size = new System.Drawing.Size(598, 48);
            LabelAbout3.TabIndex = 4;
            LabelAbout3.Text = "https://github.com/StefanoP85/Custom-ZDoom-PathFinding";
            LabelAbout3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormAbout
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(622, 433);
            Controls.Add(LabelAbout3);
            Controls.Add(LabelAbout2);
            Controls.Add(LabelAbout1);
            Controls.Add(LabelAbout0);
            Controls.Add(LabelTitle);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            MinimumSize = new System.Drawing.Size(640, 480);
            Name = "FormAbout";
            Text = "ZDoom NavMesh Builder About";
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label LabelTitle;
        private System.Windows.Forms.Label LabelAbout0;
        private System.Windows.Forms.Label LabelAbout1;
        private System.Windows.Forms.Label LabelAbout2;
        private System.Windows.Forms.Label LabelAbout3;
    }
}