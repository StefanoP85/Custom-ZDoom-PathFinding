namespace ZDoomNavMesh
{
    partial class FormViewMap
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
            TableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            PictureBoxMap = new System.Windows.Forms.PictureBox();
            FlowLayoutPanelControls = new System.Windows.Forms.FlowLayoutPanel();
            LabelZoom = new System.Windows.Forms.Label();
            TextBoxZoom = new System.Windows.Forms.TextBox();
            ButtonZoomIn = new System.Windows.Forms.Button();
            ButtonZoomOut = new System.Windows.Forms.Button();
            CheckBoxViewNavMesh = new System.Windows.Forms.CheckBox();
            TableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBoxMap).BeginInit();
            FlowLayoutPanelControls.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanelMain
            // 
            TableLayoutPanelMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TableLayoutPanelMain.ColumnCount = 1;
            TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            TableLayoutPanelMain.Controls.Add(PictureBoxMap, 0, 1);
            TableLayoutPanelMain.Controls.Add(FlowLayoutPanelControls, 0, 0);
            TableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            TableLayoutPanelMain.Name = "TableLayoutPanelMain";
            TableLayoutPanelMain.RowCount = 2;
            TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            TableLayoutPanelMain.Size = new System.Drawing.Size(622, 429);
            TableLayoutPanelMain.TabIndex = 1;
            // 
            // PictureBoxMap
            // 
            PictureBoxMap.BackColor = System.Drawing.Color.Black;
            PictureBoxMap.Dock = System.Windows.Forms.DockStyle.Fill;
            PictureBoxMap.Location = new System.Drawing.Point(3, 51);
            PictureBoxMap.Name = "PictureBoxMap";
            PictureBoxMap.Size = new System.Drawing.Size(616, 375);
            PictureBoxMap.TabIndex = 1;
            PictureBoxMap.TabStop = false;
            PictureBoxMap.Paint += PictureBoxMap_Paint;
            PictureBoxMap.MouseMove += PictureBoxMap_MouseMove;
            // 
            // FlowLayoutPanelControls
            // 
            FlowLayoutPanelControls.Controls.Add(LabelZoom);
            FlowLayoutPanelControls.Controls.Add(TextBoxZoom);
            FlowLayoutPanelControls.Controls.Add(ButtonZoomIn);
            FlowLayoutPanelControls.Controls.Add(ButtonZoomOut);
            FlowLayoutPanelControls.Controls.Add(CheckBoxViewNavMesh);
            FlowLayoutPanelControls.Dock = System.Windows.Forms.DockStyle.Fill;
            FlowLayoutPanelControls.Location = new System.Drawing.Point(3, 3);
            FlowLayoutPanelControls.Name = "FlowLayoutPanelControls";
            FlowLayoutPanelControls.Size = new System.Drawing.Size(616, 42);
            FlowLayoutPanelControls.TabIndex = 2;
            // 
            // LabelZoom
            // 
            LabelZoom.Location = new System.Drawing.Point(3, 0);
            LabelZoom.Name = "LabelZoom";
            LabelZoom.Size = new System.Drawing.Size(82, 32);
            LabelZoom.TabIndex = 1;
            LabelZoom.Text = "Zoom factor";
            LabelZoom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextBoxZoom
            // 
            TextBoxZoom.Enabled = false;
            TextBoxZoom.Location = new System.Drawing.Point(91, 3);
            TextBoxZoom.Name = "TextBoxZoom";
            TextBoxZoom.Size = new System.Drawing.Size(82, 32);
            TextBoxZoom.TabIndex = 2;
            // 
            // ButtonZoomIn
            // 
            ButtonZoomIn.Location = new System.Drawing.Point(179, 3);
            ButtonZoomIn.Name = "ButtonZoomIn";
            ButtonZoomIn.Size = new System.Drawing.Size(82, 32);
            ButtonZoomIn.TabIndex = 3;
            ButtonZoomIn.Text = "+ ZOOM";
            ButtonZoomIn.UseVisualStyleBackColor = true;
            ButtonZoomIn.Click += ButtonZoomIn_Click;
            // 
            // ButtonZoomOut
            // 
            ButtonZoomOut.Location = new System.Drawing.Point(267, 3);
            ButtonZoomOut.Name = "ButtonZoomOut";
            ButtonZoomOut.Size = new System.Drawing.Size(82, 32);
            ButtonZoomOut.TabIndex = 4;
            ButtonZoomOut.Text = "- ZOOM";
            ButtonZoomOut.UseVisualStyleBackColor = true;
            ButtonZoomOut.Click += ButtonZoomOut_Click;
            // 
            // CheckBoxViewNavMesh
            // 
            CheckBoxViewNavMesh.Enabled = false;
            CheckBoxViewNavMesh.Location = new System.Drawing.Point(355, 3);
            CheckBoxViewNavMesh.Name = "CheckBoxViewNavMesh";
            CheckBoxViewNavMesh.Size = new System.Drawing.Size(119, 32);
            CheckBoxViewNavMesh.TabIndex = 0;
            CheckBoxViewNavMesh.Text = "View NavMesh";
            CheckBoxViewNavMesh.UseVisualStyleBackColor = true;
            CheckBoxViewNavMesh.CheckedChanged += CheckBoxViewNavMesh_CheckedChanged;
            // 
            // FormViewMap
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(622, 433);
            Controls.Add(TableLayoutPanelMain);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            KeyPreview = true;
            Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            MinimumSize = new System.Drawing.Size(640, 480);
            Name = "FormViewMap";
            Text = "View map";
            TableLayoutPanelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PictureBoxMap).EndInit();
            FlowLayoutPanelControls.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanelMain;
        private System.Windows.Forms.PictureBox PictureBoxMap;
        private System.Windows.Forms.FlowLayoutPanel FlowLayoutPanelControls;
        private System.Windows.Forms.Label LabelZoom;
        private System.Windows.Forms.TextBox TextBoxZoom;
        private System.Windows.Forms.Button ButtonZoomIn;
        private System.Windows.Forms.Button ButtonZoomOut;
        private System.Windows.Forms.CheckBox CheckBoxViewNavMesh;
    }
}