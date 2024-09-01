namespace ZDoomNavMesh
{
    partial class FormMain
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
            MenuStripMain = new System.Windows.Forms.MenuStrip();
            ToolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemFileOpenLocation = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemFileClose = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemFileSave = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemFileExit = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemView = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemViewMap = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemViewNavmesh = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            ToolStripMenuItemHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            TableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            GroupBoxMap = new System.Windows.Forms.GroupBox();
            TextBoxMapThing = new System.Windows.Forms.TextBox();
            LabelMapThing = new System.Windows.Forms.Label();
            TextBoxMapSector = new System.Windows.Forms.TextBox();
            LabelMapSector = new System.Windows.Forms.Label();
            TextBoxMapSidedef = new System.Windows.Forms.TextBox();
            LabelMapSidedef = new System.Windows.Forms.Label();
            TextBoxMapLinedef = new System.Windows.Forms.TextBox();
            LabelMapLinedef = new System.Windows.Forms.Label();
            TextBoxMapVertex = new System.Windows.Forms.TextBox();
            LabelMapVertex = new System.Windows.Forms.Label();
            TextBoxMapNamespace = new System.Windows.Forms.TextBox();
            LabelMapNamespace = new System.Windows.Forms.Label();
            TextBoxMapName = new System.Windows.Forms.TextBox();
            LabelMapName = new System.Windows.Forms.Label();
            DataGridViewMaps = new System.Windows.Forms.DataGridView();
            Map = new System.Windows.Forms.DataGridViewTextBoxColumn();
            DataGridViewArchives = new System.Windows.Forms.DataGridView();
            Archive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            GroupBoxArchive = new System.Windows.Forms.GroupBox();
            TextBoxArchiveLastUpdate = new System.Windows.Forms.TextBox();
            LabelArchiveLastUpdate = new System.Windows.Forms.Label();
            TextBoxArchiveFileSize = new System.Windows.Forms.TextBox();
            LabelArchiveFileSize = new System.Windows.Forms.Label();
            TextBoxArchiveFileSpec = new System.Windows.Forms.TextBox();
            LabelArchiveFileSpec = new System.Windows.Forms.Label();
            TextBoxArchiveFileName = new System.Windows.Forms.TextBox();
            LabelArchiveFileName = new System.Windows.Forms.Label();
            GroupBoxNavmesh = new System.Windows.Forms.GroupBox();
            TextBoxActorRadius = new System.Windows.Forms.TextBox();
            LabelActorRadius = new System.Windows.Forms.Label();
            TextBoxActorHeight = new System.Windows.Forms.TextBox();
            LabelActorHeight = new System.Windows.Forms.Label();
            ButtonGenerateNavmesh = new System.Windows.Forms.Button();
            ToolStripMenuItemViewMessages = new System.Windows.Forms.ToolStripMenuItem();
            MenuStripMain.SuspendLayout();
            TableLayoutPanelMain.SuspendLayout();
            GroupBoxMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridViewMaps).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DataGridViewArchives).BeginInit();
            GroupBoxArchive.SuspendLayout();
            GroupBoxNavmesh.SuspendLayout();
            SuspendLayout();
            // 
            // MenuStripMain
            // 
            MenuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            MenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { ToolStripMenuItemFile, ToolStripMenuItemView, ToolStripMenuItemHelp });
            MenuStripMain.Location = new System.Drawing.Point(0, 0);
            MenuStripMain.Name = "MenuStripMain";
            MenuStripMain.Size = new System.Drawing.Size(782, 28);
            MenuStripMain.TabIndex = 0;
            MenuStripMain.Text = "menuStrip1";
            // 
            // ToolStripMenuItemFile
            // 
            ToolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { ToolStripMenuItemFileOpen, ToolStripMenuItemFileOpenLocation, ToolStripMenuItemFileClose, ToolStripMenuItemFileSave, ToolStripMenuItemFileExit });
            ToolStripMenuItemFile.Name = "ToolStripMenuItemFile";
            ToolStripMenuItemFile.Size = new System.Drawing.Size(46, 24);
            ToolStripMenuItemFile.Text = "File";
            // 
            // ToolStripMenuItemFileOpen
            // 
            ToolStripMenuItemFileOpen.Name = "ToolStripMenuItemFileOpen";
            ToolStripMenuItemFileOpen.Size = new System.Drawing.Size(305, 26);
            ToolStripMenuItemFileOpen.Text = "Open archive...";
            ToolStripMenuItemFileOpen.Click += ToolStripMenuItemFileOpen_Click;
            // 
            // ToolStripMenuItemFileOpenLocation
            // 
            ToolStripMenuItemFileOpenLocation.Name = "ToolStripMenuItemFileOpenLocation";
            ToolStripMenuItemFileOpenLocation.Size = new System.Drawing.Size(305, 26);
            ToolStripMenuItemFileOpenLocation.Text = "Open selected archive location...";
            ToolStripMenuItemFileOpenLocation.Click += ToolStripMenuItemFileOpenLocation_Click;
            // 
            // ToolStripMenuItemFileClose
            // 
            ToolStripMenuItemFileClose.Name = "ToolStripMenuItemFileClose";
            ToolStripMenuItemFileClose.Size = new System.Drawing.Size(305, 26);
            ToolStripMenuItemFileClose.Text = "Close selected archive";
            ToolStripMenuItemFileClose.Click += ToolStripMenuItemFileClose_Click;
            // 
            // ToolStripMenuItemFileSave
            // 
            ToolStripMenuItemFileSave.Name = "ToolStripMenuItemFileSave";
            ToolStripMenuItemFileSave.Size = new System.Drawing.Size(305, 26);
            ToolStripMenuItemFileSave.Text = "Save navigation mesh...";
            ToolStripMenuItemFileSave.Click += ToolStripMenuItemFileSave_Click;
            // 
            // ToolStripMenuItemFileExit
            // 
            ToolStripMenuItemFileExit.Name = "ToolStripMenuItemFileExit";
            ToolStripMenuItemFileExit.Size = new System.Drawing.Size(305, 26);
            ToolStripMenuItemFileExit.Text = "Exit";
            ToolStripMenuItemFileExit.Click += ToolStripMenuItemFileExit_Click;
            // 
            // ToolStripMenuItemView
            // 
            ToolStripMenuItemView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { ToolStripMenuItemViewMap, ToolStripMenuItemViewNavmesh, ToolStripMenuItemViewMessages });
            ToolStripMenuItemView.Name = "ToolStripMenuItemView";
            ToolStripMenuItemView.Size = new System.Drawing.Size(55, 24);
            ToolStripMenuItemView.Text = "View";
            // 
            // ToolStripMenuItemViewMap
            // 
            ToolStripMenuItemViewMap.Name = "ToolStripMenuItemViewMap";
            ToolStripMenuItemViewMap.Size = new System.Drawing.Size(242, 26);
            ToolStripMenuItemViewMap.Text = "Selected map...";
            ToolStripMenuItemViewMap.Click += ToolStripMenuItemViewMap_Click;
            // 
            // ToolStripMenuItemViewNavmesh
            // 
            ToolStripMenuItemViewNavmesh.Name = "ToolStripMenuItemViewNavmesh";
            ToolStripMenuItemViewNavmesh.Size = new System.Drawing.Size(242, 26);
            ToolStripMenuItemViewNavmesh.Text = "Navigation mesh text...";
            ToolStripMenuItemViewNavmesh.Click += ToolStripMenuItemViewNavmesh_Click;
            // 
            // ToolStripMenuItemHelp
            // 
            ToolStripMenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { ToolStripMenuItemHelpAbout });
            ToolStripMenuItemHelp.Name = "ToolStripMenuItemHelp";
            ToolStripMenuItemHelp.Size = new System.Drawing.Size(55, 24);
            ToolStripMenuItemHelp.Text = "Help";
            // 
            // ToolStripMenuItemHelpAbout
            // 
            ToolStripMenuItemHelpAbout.Name = "ToolStripMenuItemHelpAbout";
            ToolStripMenuItemHelpAbout.Size = new System.Drawing.Size(133, 26);
            ToolStripMenuItemHelpAbout.Text = "About";
            ToolStripMenuItemHelpAbout.Click += ToolStripMenuItemHelpAbout_Click;
            // 
            // TableLayoutPanelMain
            // 
            TableLayoutPanelMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TableLayoutPanelMain.ColumnCount = 3;
            TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TableLayoutPanelMain.Controls.Add(GroupBoxMap, 0, 1);
            TableLayoutPanelMain.Controls.Add(DataGridViewMaps, 1, 0);
            TableLayoutPanelMain.Controls.Add(DataGridViewArchives, 0, 0);
            TableLayoutPanelMain.Controls.Add(GroupBoxArchive, 2, 0);
            TableLayoutPanelMain.Controls.Add(GroupBoxNavmesh, 2, 2);
            TableLayoutPanelMain.Location = new System.Drawing.Point(0, 31);
            TableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TableLayoutPanelMain.Name = "TableLayoutPanelMain";
            TableLayoutPanelMain.RowCount = 3;
            TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            TableLayoutPanelMain.Size = new System.Drawing.Size(782, 521);
            TableLayoutPanelMain.TabIndex = 1;
            // 
            // GroupBoxMap
            // 
            GroupBoxMap.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            GroupBoxMap.Controls.Add(TextBoxMapThing);
            GroupBoxMap.Controls.Add(LabelMapThing);
            GroupBoxMap.Controls.Add(TextBoxMapSector);
            GroupBoxMap.Controls.Add(LabelMapSector);
            GroupBoxMap.Controls.Add(TextBoxMapSidedef);
            GroupBoxMap.Controls.Add(LabelMapSidedef);
            GroupBoxMap.Controls.Add(TextBoxMapLinedef);
            GroupBoxMap.Controls.Add(LabelMapLinedef);
            GroupBoxMap.Controls.Add(TextBoxMapVertex);
            GroupBoxMap.Controls.Add(LabelMapVertex);
            GroupBoxMap.Controls.Add(TextBoxMapNamespace);
            GroupBoxMap.Controls.Add(LabelMapNamespace);
            GroupBoxMap.Controls.Add(TextBoxMapName);
            GroupBoxMap.Controls.Add(LabelMapName);
            GroupBoxMap.Location = new System.Drawing.Point(393, 175);
            GroupBoxMap.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupBoxMap.Name = "GroupBoxMap";
            GroupBoxMap.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupBoxMap.Size = new System.Drawing.Size(386, 163);
            GroupBoxMap.TabIndex = 3;
            GroupBoxMap.TabStop = false;
            GroupBoxMap.Text = "Selected map";
            // 
            // TextBoxMapThing
            // 
            TextBoxMapThing.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxMapThing.Enabled = false;
            TextBoxMapThing.Location = new System.Drawing.Point(278, 128);
            TextBoxMapThing.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxMapThing.Name = "TextBoxMapThing";
            TextBoxMapThing.Size = new System.Drawing.Size(62, 22);
            TextBoxMapThing.TabIndex = 13;
            // 
            // LabelMapThing
            // 
            LabelMapThing.AutoSize = true;
            LabelMapThing.Location = new System.Drawing.Point(278, 108);
            LabelMapThing.Name = "LabelMapThing";
            LabelMapThing.Size = new System.Drawing.Size(41, 16);
            LabelMapThing.TabIndex = 12;
            LabelMapThing.Text = "Thing";
            // 
            // TextBoxMapSector
            // 
            TextBoxMapSector.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxMapSector.Enabled = false;
            TextBoxMapSector.Location = new System.Drawing.Point(210, 128);
            TextBoxMapSector.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxMapSector.Name = "TextBoxMapSector";
            TextBoxMapSector.Size = new System.Drawing.Size(62, 22);
            TextBoxMapSector.TabIndex = 11;
            // 
            // LabelMapSector
            // 
            LabelMapSector.AutoSize = true;
            LabelMapSector.Location = new System.Drawing.Point(210, 108);
            LabelMapSector.Name = "LabelMapSector";
            LabelMapSector.Size = new System.Drawing.Size(46, 16);
            LabelMapSector.TabIndex = 10;
            LabelMapSector.Text = "Sector";
            // 
            // TextBoxMapSidedef
            // 
            TextBoxMapSidedef.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxMapSidedef.Enabled = false;
            TextBoxMapSidedef.Location = new System.Drawing.Point(142, 128);
            TextBoxMapSidedef.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxMapSidedef.Name = "TextBoxMapSidedef";
            TextBoxMapSidedef.Size = new System.Drawing.Size(62, 22);
            TextBoxMapSidedef.TabIndex = 9;
            // 
            // LabelMapSidedef
            // 
            LabelMapSidedef.AutoSize = true;
            LabelMapSidedef.Location = new System.Drawing.Point(142, 108);
            LabelMapSidedef.Name = "LabelMapSidedef";
            LabelMapSidedef.Size = new System.Drawing.Size(54, 16);
            LabelMapSidedef.TabIndex = 8;
            LabelMapSidedef.Text = "Sidedef";
            // 
            // TextBoxMapLinedef
            // 
            TextBoxMapLinedef.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxMapLinedef.Enabled = false;
            TextBoxMapLinedef.Location = new System.Drawing.Point(74, 128);
            TextBoxMapLinedef.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxMapLinedef.Name = "TextBoxMapLinedef";
            TextBoxMapLinedef.Size = new System.Drawing.Size(62, 22);
            TextBoxMapLinedef.TabIndex = 7;
            // 
            // LabelMapLinedef
            // 
            LabelMapLinedef.AutoSize = true;
            LabelMapLinedef.Location = new System.Drawing.Point(74, 108);
            LabelMapLinedef.Name = "LabelMapLinedef";
            LabelMapLinedef.Size = new System.Drawing.Size(51, 16);
            LabelMapLinedef.TabIndex = 6;
            LabelMapLinedef.Text = "Linedef";
            // 
            // TextBoxMapVertex
            // 
            TextBoxMapVertex.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxMapVertex.Enabled = false;
            TextBoxMapVertex.Location = new System.Drawing.Point(6, 128);
            TextBoxMapVertex.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxMapVertex.Name = "TextBoxMapVertex";
            TextBoxMapVertex.Size = new System.Drawing.Size(62, 22);
            TextBoxMapVertex.TabIndex = 5;
            // 
            // LabelMapVertex
            // 
            LabelMapVertex.AutoSize = true;
            LabelMapVertex.Location = new System.Drawing.Point(6, 108);
            LabelMapVertex.Name = "LabelMapVertex";
            LabelMapVertex.Size = new System.Drawing.Size(45, 16);
            LabelMapVertex.TabIndex = 4;
            LabelMapVertex.Text = "Vertex";
            // 
            // TextBoxMapNamespace
            // 
            TextBoxMapNamespace.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxMapNamespace.Enabled = false;
            TextBoxMapNamespace.Location = new System.Drawing.Point(6, 84);
            TextBoxMapNamespace.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxMapNamespace.Name = "TextBoxMapNamespace";
            TextBoxMapNamespace.Size = new System.Drawing.Size(374, 22);
            TextBoxMapNamespace.TabIndex = 3;
            // 
            // LabelMapNamespace
            // 
            LabelMapNamespace.AutoSize = true;
            LabelMapNamespace.Location = new System.Drawing.Point(6, 63);
            LabelMapNamespace.Name = "LabelMapNamespace";
            LabelMapNamespace.Size = new System.Drawing.Size(109, 16);
            LabelMapNamespace.TabIndex = 2;
            LabelMapNamespace.Text = "Map namespace";
            // 
            // TextBoxMapName
            // 
            TextBoxMapName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxMapName.Enabled = false;
            TextBoxMapName.Location = new System.Drawing.Point(6, 38);
            TextBoxMapName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxMapName.Name = "TextBoxMapName";
            TextBoxMapName.Size = new System.Drawing.Size(374, 22);
            TextBoxMapName.TabIndex = 1;
            // 
            // LabelMapName
            // 
            LabelMapName.AutoSize = true;
            LabelMapName.Location = new System.Drawing.Point(6, 18);
            LabelMapName.Name = "LabelMapName";
            LabelMapName.Size = new System.Drawing.Size(71, 16);
            LabelMapName.TabIndex = 0;
            LabelMapName.Text = "Map name";
            // 
            // DataGridViewMaps
            // 
            DataGridViewMaps.AllowUserToAddRows = false;
            DataGridViewMaps.AllowUserToDeleteRows = false;
            DataGridViewMaps.AllowUserToResizeRows = false;
            DataGridViewMaps.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DataGridViewMaps.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            DataGridViewMaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridViewMaps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Map });
            DataGridViewMaps.Location = new System.Drawing.Point(198, 4);
            DataGridViewMaps.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            DataGridViewMaps.MultiSelect = false;
            DataGridViewMaps.Name = "DataGridViewMaps";
            DataGridViewMaps.ReadOnly = true;
            DataGridViewMaps.RowHeadersWidth = 51;
            TableLayoutPanelMain.SetRowSpan(DataGridViewMaps, 3);
            DataGridViewMaps.RowTemplate.Height = 24;
            DataGridViewMaps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            DataGridViewMaps.Size = new System.Drawing.Size(189, 513);
            DataGridViewMaps.TabIndex = 0;
            DataGridViewMaps.SelectionChanged += DataGridViewMaps_SelectionChanged;
            // 
            // Map
            // 
            Map.HeaderText = "Map";
            Map.MinimumWidth = 6;
            Map.Name = "Map";
            Map.ReadOnly = true;
            Map.Width = 63;
            // 
            // DataGridViewArchives
            // 
            DataGridViewArchives.AllowUserToAddRows = false;
            DataGridViewArchives.AllowUserToDeleteRows = false;
            DataGridViewArchives.AllowUserToResizeRows = false;
            DataGridViewArchives.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            DataGridViewArchives.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            DataGridViewArchives.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DataGridViewArchives.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Archive });
            DataGridViewArchives.Location = new System.Drawing.Point(3, 4);
            DataGridViewArchives.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            DataGridViewArchives.MultiSelect = false;
            DataGridViewArchives.Name = "DataGridViewArchives";
            DataGridViewArchives.ReadOnly = true;
            DataGridViewArchives.RowHeadersWidth = 51;
            TableLayoutPanelMain.SetRowSpan(DataGridViewArchives, 3);
            DataGridViewArchives.RowTemplate.Height = 24;
            DataGridViewArchives.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            DataGridViewArchives.Size = new System.Drawing.Size(189, 513);
            DataGridViewArchives.TabIndex = 1;
            DataGridViewArchives.SelectionChanged += DataGridViewArchives_SelectionChanged;
            // 
            // Archive
            // 
            Archive.HeaderText = "Archive";
            Archive.MinimumWidth = 6;
            Archive.Name = "Archive";
            Archive.ReadOnly = true;
            Archive.Width = 81;
            // 
            // GroupBoxArchive
            // 
            GroupBoxArchive.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            GroupBoxArchive.Controls.Add(TextBoxArchiveLastUpdate);
            GroupBoxArchive.Controls.Add(LabelArchiveLastUpdate);
            GroupBoxArchive.Controls.Add(TextBoxArchiveFileSize);
            GroupBoxArchive.Controls.Add(LabelArchiveFileSize);
            GroupBoxArchive.Controls.Add(TextBoxArchiveFileSpec);
            GroupBoxArchive.Controls.Add(LabelArchiveFileSpec);
            GroupBoxArchive.Controls.Add(TextBoxArchiveFileName);
            GroupBoxArchive.Controls.Add(LabelArchiveFileName);
            GroupBoxArchive.Location = new System.Drawing.Point(393, 4);
            GroupBoxArchive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupBoxArchive.Name = "GroupBoxArchive";
            GroupBoxArchive.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupBoxArchive.Size = new System.Drawing.Size(386, 163);
            GroupBoxArchive.TabIndex = 2;
            GroupBoxArchive.TabStop = false;
            GroupBoxArchive.Text = "Selected archive";
            // 
            // TextBoxArchiveLastUpdate
            // 
            TextBoxArchiveLastUpdate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxArchiveLastUpdate.Enabled = false;
            TextBoxArchiveLastUpdate.Location = new System.Drawing.Point(189, 128);
            TextBoxArchiveLastUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxArchiveLastUpdate.Name = "TextBoxArchiveLastUpdate";
            TextBoxArchiveLastUpdate.Size = new System.Drawing.Size(150, 22);
            TextBoxArchiveLastUpdate.TabIndex = 7;
            // 
            // LabelArchiveLastUpdate
            // 
            LabelArchiveLastUpdate.AutoSize = true;
            LabelArchiveLastUpdate.Location = new System.Drawing.Point(189, 108);
            LabelArchiveLastUpdate.Name = "LabelArchiveLastUpdate";
            LabelArchiveLastUpdate.Size = new System.Drawing.Size(98, 16);
            LabelArchiveLastUpdate.TabIndex = 6;
            LabelArchiveLastUpdate.Text = "File last update";
            // 
            // TextBoxArchiveFileSize
            // 
            TextBoxArchiveFileSize.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxArchiveFileSize.Enabled = false;
            TextBoxArchiveFileSize.Location = new System.Drawing.Point(6, 128);
            TextBoxArchiveFileSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxArchiveFileSize.Name = "TextBoxArchiveFileSize";
            TextBoxArchiveFileSize.Size = new System.Drawing.Size(150, 22);
            TextBoxArchiveFileSize.TabIndex = 5;
            // 
            // LabelArchiveFileSize
            // 
            LabelArchiveFileSize.AutoSize = true;
            LabelArchiveFileSize.Location = new System.Drawing.Point(6, 108);
            LabelArchiveFileSize.Name = "LabelArchiveFileSize";
            LabelArchiveFileSize.Size = new System.Drawing.Size(100, 16);
            LabelArchiveFileSize.TabIndex = 4;
            LabelArchiveFileSize.Text = "File size (bytes)";
            // 
            // TextBoxArchiveFileSpec
            // 
            TextBoxArchiveFileSpec.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxArchiveFileSpec.Enabled = false;
            TextBoxArchiveFileSpec.Location = new System.Drawing.Point(6, 84);
            TextBoxArchiveFileSpec.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxArchiveFileSpec.Name = "TextBoxArchiveFileSpec";
            TextBoxArchiveFileSpec.Size = new System.Drawing.Size(374, 22);
            TextBoxArchiveFileSpec.TabIndex = 3;
            // 
            // LabelArchiveFileSpec
            // 
            LabelArchiveFileSpec.AutoSize = true;
            LabelArchiveFileSpec.Location = new System.Drawing.Point(6, 63);
            LabelArchiveFileSpec.Name = "LabelArchiveFileSpec";
            LabelArchiveFileSpec.Size = new System.Drawing.Size(102, 16);
            LabelArchiveFileSpec.TabIndex = 2;
            LabelArchiveFileSpec.Text = "Archive location";
            // 
            // TextBoxArchiveFileName
            // 
            TextBoxArchiveFileName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            TextBoxArchiveFileName.Enabled = false;
            TextBoxArchiveFileName.Location = new System.Drawing.Point(6, 38);
            TextBoxArchiveFileName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxArchiveFileName.Name = "TextBoxArchiveFileName";
            TextBoxArchiveFileName.Size = new System.Drawing.Size(374, 22);
            TextBoxArchiveFileName.TabIndex = 1;
            // 
            // LabelArchiveFileName
            // 
            LabelArchiveFileName.AutoSize = true;
            LabelArchiveFileName.Location = new System.Drawing.Point(6, 18);
            LabelArchiveFileName.Name = "LabelArchiveFileName";
            LabelArchiveFileName.Size = new System.Drawing.Size(89, 16);
            LabelArchiveFileName.TabIndex = 0;
            LabelArchiveFileName.Text = "Archive name";
            // 
            // GroupBoxNavmesh
            // 
            GroupBoxNavmesh.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            GroupBoxNavmesh.Controls.Add(TextBoxActorRadius);
            GroupBoxNavmesh.Controls.Add(LabelActorRadius);
            GroupBoxNavmesh.Controls.Add(TextBoxActorHeight);
            GroupBoxNavmesh.Controls.Add(LabelActorHeight);
            GroupBoxNavmesh.Controls.Add(ButtonGenerateNavmesh);
            GroupBoxNavmesh.Location = new System.Drawing.Point(393, 346);
            GroupBoxNavmesh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupBoxNavmesh.Name = "GroupBoxNavmesh";
            GroupBoxNavmesh.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupBoxNavmesh.Size = new System.Drawing.Size(386, 171);
            GroupBoxNavmesh.TabIndex = 4;
            GroupBoxNavmesh.TabStop = false;
            GroupBoxNavmesh.Text = "Navigation mesh";
            // 
            // TextBoxActorRadius
            // 
            TextBoxActorRadius.Location = new System.Drawing.Point(6, 84);
            TextBoxActorRadius.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxActorRadius.MaxLength = 4;
            TextBoxActorRadius.Name = "TextBoxActorRadius";
            TextBoxActorRadius.Size = new System.Drawing.Size(100, 22);
            TextBoxActorRadius.TabIndex = 7;
            TextBoxActorRadius.Text = "32";
            TextBoxActorRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LabelActorRadius
            // 
            LabelActorRadius.AutoSize = true;
            LabelActorRadius.Location = new System.Drawing.Point(6, 63);
            LabelActorRadius.Name = "LabelActorRadius";
            LabelActorRadius.Size = new System.Drawing.Size(85, 16);
            LabelActorRadius.TabIndex = 6;
            LabelActorRadius.Text = "Actors radius";
            // 
            // TextBoxActorHeight
            // 
            TextBoxActorHeight.Location = new System.Drawing.Point(6, 38);
            TextBoxActorHeight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            TextBoxActorHeight.MaxLength = 4;
            TextBoxActorHeight.Name = "TextBoxActorHeight";
            TextBoxActorHeight.Size = new System.Drawing.Size(100, 22);
            TextBoxActorHeight.TabIndex = 5;
            TextBoxActorHeight.Text = "128";
            TextBoxActorHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LabelActorHeight
            // 
            LabelActorHeight.AutoSize = true;
            LabelActorHeight.Location = new System.Drawing.Point(6, 18);
            LabelActorHeight.Name = "LabelActorHeight";
            LabelActorHeight.Size = new System.Drawing.Size(84, 16);
            LabelActorHeight.TabIndex = 4;
            LabelActorHeight.Text = "Actors height";
            // 
            // ButtonGenerateNavmesh
            // 
            ButtonGenerateNavmesh.Location = new System.Drawing.Point(6, 122);
            ButtonGenerateNavmesh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            ButtonGenerateNavmesh.Name = "ButtonGenerateNavmesh";
            ButtonGenerateNavmesh.Size = new System.Drawing.Size(371, 41);
            ButtonGenerateNavmesh.TabIndex = 0;
            ButtonGenerateNavmesh.Text = "Generate navmesh...";
            ButtonGenerateNavmesh.UseVisualStyleBackColor = true;
            ButtonGenerateNavmesh.Click += ButtonGenerateNavmesh_Click;
            // 
            // ToolStripMenuItemViewMessages
            // 
            ToolStripMenuItemViewMessages.Name = "ToolStripMenuItemViewMessages";
            ToolStripMenuItemViewMessages.Size = new System.Drawing.Size(242, 26);
            ToolStripMenuItemViewMessages.Text = "Build messages...";
            ToolStripMenuItemViewMessages.Click += ToolStripMenuItemViewMessages_Click;
            // 
            // FormMain
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(782, 553);
            Controls.Add(TableLayoutPanelMain);
            Controls.Add(MenuStripMain);
            Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            MainMenuStrip = MenuStripMain;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MinimumSize = new System.Drawing.Size(800, 600);
            Name = "FormMain";
            Text = "ZDoom NavMesh builder";
            DragDrop += FormMain_DragDrop;
            DragEnter += FormMain_DragEnter;
            MenuStripMain.ResumeLayout(false);
            MenuStripMain.PerformLayout();
            TableLayoutPanelMain.ResumeLayout(false);
            GroupBoxMap.ResumeLayout(false);
            GroupBoxMap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DataGridViewMaps).EndInit();
            ((System.ComponentModel.ISupportInitialize)DataGridViewArchives).EndInit();
            GroupBoxArchive.ResumeLayout(false);
            GroupBoxArchive.PerformLayout();
            GroupBoxNavmesh.ResumeLayout(false);
            GroupBoxNavmesh.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStripMain;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFileOpen;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFileOpenLocation;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFileClose;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFileSave;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFileExit;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemView;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemViewMap;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemViewNavmesh;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemHelpAbout;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanelMain;
        private System.Windows.Forms.DataGridView DataGridViewMaps;
        private System.Windows.Forms.DataGridView DataGridViewArchives;
        private System.Windows.Forms.GroupBox GroupBoxArchive;
        private System.Windows.Forms.TextBox TextBoxArchiveFileSize;
        private System.Windows.Forms.Label LabelArchiveFileSize;
        private System.Windows.Forms.TextBox TextBoxArchiveFileSpec;
        private System.Windows.Forms.Label LabelArchiveFileSpec;
        private System.Windows.Forms.TextBox TextBoxArchiveFileName;
        private System.Windows.Forms.Label LabelArchiveFileName;
        private System.Windows.Forms.TextBox TextBoxArchiveLastUpdate;
        private System.Windows.Forms.Label LabelArchiveLastUpdate;
        private System.Windows.Forms.GroupBox GroupBoxMap;
        private System.Windows.Forms.TextBox TextBoxMapVertex;
        private System.Windows.Forms.Label LabelMapVertex;
        private System.Windows.Forms.TextBox TextBoxMapNamespace;
        private System.Windows.Forms.Label LabelMapNamespace;
        private System.Windows.Forms.TextBox TextBoxMapName;
        private System.Windows.Forms.Label LabelMapName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Map;
        private System.Windows.Forms.DataGridViewTextBoxColumn Archive;
        private System.Windows.Forms.TextBox TextBoxMapSidedef;
        private System.Windows.Forms.Label LabelMapSidedef;
        private System.Windows.Forms.TextBox TextBoxMapLinedef;
        private System.Windows.Forms.Label LabelMapLinedef;
        private System.Windows.Forms.TextBox TextBoxMapThing;
        private System.Windows.Forms.Label LabelMapThing;
        private System.Windows.Forms.TextBox TextBoxMapSector;
        private System.Windows.Forms.Label LabelMapSector;
        private System.Windows.Forms.GroupBox GroupBoxNavmesh;
        private System.Windows.Forms.Button ButtonGenerateNavmesh;
        private System.Windows.Forms.TextBox TextBoxActorHeight;
        private System.Windows.Forms.Label LabelActorHeight;
        private System.Windows.Forms.TextBox TextBoxActorRadius;
        private System.Windows.Forms.Label LabelActorRadius;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemViewMessages;
    }
}