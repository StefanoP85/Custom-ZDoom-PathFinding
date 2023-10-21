namespace ZDoomNavmesh
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
            this.MenuStripMain = new System.Windows.Forms.MenuStrip();
            this.ToolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemFileOpenLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemView = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemViewMap = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemViewNavmesh = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.TableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.GroupBoxMap = new System.Windows.Forms.GroupBox();
            this.TextBoxMapThing = new System.Windows.Forms.TextBox();
            this.LabelMapThing = new System.Windows.Forms.Label();
            this.TextBoxMapSector = new System.Windows.Forms.TextBox();
            this.LabelMapSector = new System.Windows.Forms.Label();
            this.TextBoxMapSidedef = new System.Windows.Forms.TextBox();
            this.LabelMapSidedef = new System.Windows.Forms.Label();
            this.TextBoxMapLinedef = new System.Windows.Forms.TextBox();
            this.LabelMapLinedef = new System.Windows.Forms.Label();
            this.TextBoxMapVertex = new System.Windows.Forms.TextBox();
            this.LabelMapVertex = new System.Windows.Forms.Label();
            this.TextBoxMapNamespace = new System.Windows.Forms.TextBox();
            this.LabelMapNamespace = new System.Windows.Forms.Label();
            this.TextBoxMapName = new System.Windows.Forms.TextBox();
            this.LabelMapName = new System.Windows.Forms.Label();
            this.DataGridViewMaps = new System.Windows.Forms.DataGridView();
            this.Map = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataGridViewArchives = new System.Windows.Forms.DataGridView();
            this.Archive = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GroupBoxArchive = new System.Windows.Forms.GroupBox();
            this.TextBoxArchiveLastUpdate = new System.Windows.Forms.TextBox();
            this.LabelArchiveLastUpdate = new System.Windows.Forms.Label();
            this.TextBoxArchiveFileSize = new System.Windows.Forms.TextBox();
            this.LabelArchiveFileSize = new System.Windows.Forms.Label();
            this.TextBoxArchiveFileSpec = new System.Windows.Forms.TextBox();
            this.LabelArchiveFileSpec = new System.Windows.Forms.Label();
            this.TextBoxArchiveFileName = new System.Windows.Forms.TextBox();
            this.LabelArchiveFileName = new System.Windows.Forms.Label();
            this.GroupBoxNavmesh = new System.Windows.Forms.GroupBox();
            this.ButtonGenerateNavmesh = new System.Windows.Forms.Button();
            this.TextBoxActorHeight = new System.Windows.Forms.TextBox();
            this.LabelActorHeight = new System.Windows.Forms.Label();
            this.TextBoxActorRadius = new System.Windows.Forms.TextBox();
            this.LabelActorRadius = new System.Windows.Forms.Label();
            this.MenuStripMain.SuspendLayout();
            this.TableLayoutPanelMain.SuspendLayout();
            this.GroupBoxMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewMaps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewArchives)).BeginInit();
            this.GroupBoxArchive.SuspendLayout();
            this.GroupBoxNavmesh.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStripMain
            // 
            this.MenuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemFile,
            this.ToolStripMenuItemView,
            this.ToolStripMenuItemHelp});
            this.MenuStripMain.Location = new System.Drawing.Point(0, 0);
            this.MenuStripMain.Name = "MenuStripMain";
            this.MenuStripMain.Size = new System.Drawing.Size(782, 28);
            this.MenuStripMain.TabIndex = 0;
            this.MenuStripMain.Text = "menuStrip1";
            // 
            // ToolStripMenuItemFile
            // 
            this.ToolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemFileOpen,
            this.ToolStripMenuItemFileOpenLocation,
            this.ToolStripMenuItemFileClose,
            this.ToolStripMenuItemFileSave,
            this.ToolStripMenuItemFileExit});
            this.ToolStripMenuItemFile.Name = "ToolStripMenuItemFile";
            this.ToolStripMenuItemFile.Size = new System.Drawing.Size(46, 24);
            this.ToolStripMenuItemFile.Text = "File";
            // 
            // ToolStripMenuItemFileOpen
            // 
            this.ToolStripMenuItemFileOpen.Name = "ToolStripMenuItemFileOpen";
            this.ToolStripMenuItemFileOpen.Size = new System.Drawing.Size(305, 26);
            this.ToolStripMenuItemFileOpen.Text = "Open archive...";
            this.ToolStripMenuItemFileOpen.Click += new System.EventHandler(this.ToolStripMenuItemFileOpen_Click);
            // 
            // ToolStripMenuItemFileOpenLocation
            // 
            this.ToolStripMenuItemFileOpenLocation.Name = "ToolStripMenuItemFileOpenLocation";
            this.ToolStripMenuItemFileOpenLocation.Size = new System.Drawing.Size(305, 26);
            this.ToolStripMenuItemFileOpenLocation.Text = "Open selected archive location...";
            this.ToolStripMenuItemFileOpenLocation.Click += new System.EventHandler(this.ToolStripMenuItemFileOpenLocation_Click);
            // 
            // ToolStripMenuItemFileClose
            // 
            this.ToolStripMenuItemFileClose.Name = "ToolStripMenuItemFileClose";
            this.ToolStripMenuItemFileClose.Size = new System.Drawing.Size(305, 26);
            this.ToolStripMenuItemFileClose.Text = "Close selected archive";
            this.ToolStripMenuItemFileClose.Click += new System.EventHandler(this.ToolStripMenuItemFileClose_Click);
            // 
            // ToolStripMenuItemFileSave
            // 
            this.ToolStripMenuItemFileSave.Name = "ToolStripMenuItemFileSave";
            this.ToolStripMenuItemFileSave.Size = new System.Drawing.Size(305, 26);
            this.ToolStripMenuItemFileSave.Text = "Save navigation mesh...";
            this.ToolStripMenuItemFileSave.Click += new System.EventHandler(this.ToolStripMenuItemFileSave_Click);
            // 
            // ToolStripMenuItemFileExit
            // 
            this.ToolStripMenuItemFileExit.Name = "ToolStripMenuItemFileExit";
            this.ToolStripMenuItemFileExit.Size = new System.Drawing.Size(305, 26);
            this.ToolStripMenuItemFileExit.Text = "Exit";
            this.ToolStripMenuItemFileExit.Click += new System.EventHandler(this.ToolStripMenuItemFileExit_Click);
            // 
            // ToolStripMenuItemView
            // 
            this.ToolStripMenuItemView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemViewMap,
            this.ToolStripMenuItemViewNavmesh});
            this.ToolStripMenuItemView.Name = "ToolStripMenuItemView";
            this.ToolStripMenuItemView.Size = new System.Drawing.Size(55, 24);
            this.ToolStripMenuItemView.Text = "View";
            // 
            // ToolStripMenuItemViewMap
            // 
            this.ToolStripMenuItemViewMap.Name = "ToolStripMenuItemViewMap";
            this.ToolStripMenuItemViewMap.Size = new System.Drawing.Size(242, 26);
            this.ToolStripMenuItemViewMap.Text = "Selected map...";
            this.ToolStripMenuItemViewMap.Click += new System.EventHandler(this.ToolStripMenuItemViewMap_Click);
            // 
            // ToolStripMenuItemViewNavmesh
            // 
            this.ToolStripMenuItemViewNavmesh.Name = "ToolStripMenuItemViewNavmesh";
            this.ToolStripMenuItemViewNavmesh.Size = new System.Drawing.Size(242, 26);
            this.ToolStripMenuItemViewNavmesh.Text = "Navigation mesh text...";
            this.ToolStripMenuItemViewNavmesh.Click += new System.EventHandler(this.ToolStripMenuItemViewNavmesh_Click);
            // 
            // ToolStripMenuItemHelp
            // 
            this.ToolStripMenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemHelpAbout});
            this.ToolStripMenuItemHelp.Name = "ToolStripMenuItemHelp";
            this.ToolStripMenuItemHelp.Size = new System.Drawing.Size(55, 24);
            this.ToolStripMenuItemHelp.Text = "Help";
            // 
            // ToolStripMenuItemHelpAbout
            // 
            this.ToolStripMenuItemHelpAbout.Name = "ToolStripMenuItemHelpAbout";
            this.ToolStripMenuItemHelpAbout.Size = new System.Drawing.Size(133, 26);
            this.ToolStripMenuItemHelpAbout.Text = "About";
            this.ToolStripMenuItemHelpAbout.Click += new System.EventHandler(this.ToolStripMenuItemHelpAbout_Click);
            // 
            // TableLayoutPanelMain
            // 
            this.TableLayoutPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanelMain.ColumnCount = 3;
            this.TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanelMain.Controls.Add(this.GroupBoxMap, 0, 1);
            this.TableLayoutPanelMain.Controls.Add(this.DataGridViewMaps, 1, 0);
            this.TableLayoutPanelMain.Controls.Add(this.DataGridViewArchives, 0, 0);
            this.TableLayoutPanelMain.Controls.Add(this.GroupBoxArchive, 2, 0);
            this.TableLayoutPanelMain.Controls.Add(this.GroupBoxNavmesh, 2, 2);
            this.TableLayoutPanelMain.Location = new System.Drawing.Point(0, 31);
            this.TableLayoutPanelMain.Name = "TableLayoutPanelMain";
            this.TableLayoutPanelMain.RowCount = 3;
            this.TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.TableLayoutPanelMain.Size = new System.Drawing.Size(782, 522);
            this.TableLayoutPanelMain.TabIndex = 1;
            // 
            // GroupBoxMap
            // 
            this.GroupBoxMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBoxMap.Controls.Add(this.TextBoxMapThing);
            this.GroupBoxMap.Controls.Add(this.LabelMapThing);
            this.GroupBoxMap.Controls.Add(this.TextBoxMapSector);
            this.GroupBoxMap.Controls.Add(this.LabelMapSector);
            this.GroupBoxMap.Controls.Add(this.TextBoxMapSidedef);
            this.GroupBoxMap.Controls.Add(this.LabelMapSidedef);
            this.GroupBoxMap.Controls.Add(this.TextBoxMapLinedef);
            this.GroupBoxMap.Controls.Add(this.LabelMapLinedef);
            this.GroupBoxMap.Controls.Add(this.TextBoxMapVertex);
            this.GroupBoxMap.Controls.Add(this.LabelMapVertex);
            this.GroupBoxMap.Controls.Add(this.TextBoxMapNamespace);
            this.GroupBoxMap.Controls.Add(this.LabelMapNamespace);
            this.GroupBoxMap.Controls.Add(this.TextBoxMapName);
            this.GroupBoxMap.Controls.Add(this.LabelMapName);
            this.GroupBoxMap.Location = new System.Drawing.Point(393, 175);
            this.GroupBoxMap.Name = "GroupBoxMap";
            this.GroupBoxMap.Size = new System.Drawing.Size(386, 166);
            this.GroupBoxMap.TabIndex = 3;
            this.GroupBoxMap.TabStop = false;
            this.GroupBoxMap.Text = "Selected map";
            // 
            // TextBoxMapThing
            // 
            this.TextBoxMapThing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxMapThing.Enabled = false;
            this.TextBoxMapThing.Location = new System.Drawing.Point(278, 128);
            this.TextBoxMapThing.Name = "TextBoxMapThing";
            this.TextBoxMapThing.Size = new System.Drawing.Size(62, 22);
            this.TextBoxMapThing.TabIndex = 13;
            // 
            // LabelMapThing
            // 
            this.LabelMapThing.AutoSize = true;
            this.LabelMapThing.Location = new System.Drawing.Point(278, 108);
            this.LabelMapThing.Name = "LabelMapThing";
            this.LabelMapThing.Size = new System.Drawing.Size(44, 17);
            this.LabelMapThing.TabIndex = 12;
            this.LabelMapThing.Text = "Thing";
            // 
            // TextBoxMapSector
            // 
            this.TextBoxMapSector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxMapSector.Enabled = false;
            this.TextBoxMapSector.Location = new System.Drawing.Point(210, 128);
            this.TextBoxMapSector.Name = "TextBoxMapSector";
            this.TextBoxMapSector.Size = new System.Drawing.Size(62, 22);
            this.TextBoxMapSector.TabIndex = 11;
            // 
            // LabelMapSector
            // 
            this.LabelMapSector.AutoSize = true;
            this.LabelMapSector.Location = new System.Drawing.Point(210, 108);
            this.LabelMapSector.Name = "LabelMapSector";
            this.LabelMapSector.Size = new System.Drawing.Size(49, 17);
            this.LabelMapSector.TabIndex = 10;
            this.LabelMapSector.Text = "Sector";
            // 
            // TextBoxMapSidedef
            // 
            this.TextBoxMapSidedef.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxMapSidedef.Enabled = false;
            this.TextBoxMapSidedef.Location = new System.Drawing.Point(142, 128);
            this.TextBoxMapSidedef.Name = "TextBoxMapSidedef";
            this.TextBoxMapSidedef.Size = new System.Drawing.Size(62, 22);
            this.TextBoxMapSidedef.TabIndex = 9;
            // 
            // LabelMapSidedef
            // 
            this.LabelMapSidedef.AutoSize = true;
            this.LabelMapSidedef.Location = new System.Drawing.Point(142, 108);
            this.LabelMapSidedef.Name = "LabelMapSidedef";
            this.LabelMapSidedef.Size = new System.Drawing.Size(56, 17);
            this.LabelMapSidedef.TabIndex = 8;
            this.LabelMapSidedef.Text = "Sidedef";
            // 
            // TextBoxMapLinedef
            // 
            this.TextBoxMapLinedef.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxMapLinedef.Enabled = false;
            this.TextBoxMapLinedef.Location = new System.Drawing.Point(74, 128);
            this.TextBoxMapLinedef.Name = "TextBoxMapLinedef";
            this.TextBoxMapLinedef.Size = new System.Drawing.Size(62, 22);
            this.TextBoxMapLinedef.TabIndex = 7;
            // 
            // LabelMapLinedef
            // 
            this.LabelMapLinedef.AutoSize = true;
            this.LabelMapLinedef.Location = new System.Drawing.Point(74, 108);
            this.LabelMapLinedef.Name = "LabelMapLinedef";
            this.LabelMapLinedef.Size = new System.Drawing.Size(55, 17);
            this.LabelMapLinedef.TabIndex = 6;
            this.LabelMapLinedef.Text = "Linedef";
            // 
            // TextBoxMapVertex
            // 
            this.TextBoxMapVertex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxMapVertex.Enabled = false;
            this.TextBoxMapVertex.Location = new System.Drawing.Point(6, 128);
            this.TextBoxMapVertex.Name = "TextBoxMapVertex";
            this.TextBoxMapVertex.Size = new System.Drawing.Size(62, 22);
            this.TextBoxMapVertex.TabIndex = 5;
            // 
            // LabelMapVertex
            // 
            this.LabelMapVertex.AutoSize = true;
            this.LabelMapVertex.Location = new System.Drawing.Point(6, 108);
            this.LabelMapVertex.Name = "LabelMapVertex";
            this.LabelMapVertex.Size = new System.Drawing.Size(48, 17);
            this.LabelMapVertex.TabIndex = 4;
            this.LabelMapVertex.Text = "Vertex";
            // 
            // TextBoxMapNamespace
            // 
            this.TextBoxMapNamespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxMapNamespace.Enabled = false;
            this.TextBoxMapNamespace.Location = new System.Drawing.Point(6, 83);
            this.TextBoxMapNamespace.Name = "TextBoxMapNamespace";
            this.TextBoxMapNamespace.Size = new System.Drawing.Size(374, 22);
            this.TextBoxMapNamespace.TabIndex = 3;
            // 
            // LabelMapNamespace
            // 
            this.LabelMapNamespace.AutoSize = true;
            this.LabelMapNamespace.Location = new System.Drawing.Point(6, 63);
            this.LabelMapNamespace.Name = "LabelMapNamespace";
            this.LabelMapNamespace.Size = new System.Drawing.Size(112, 17);
            this.LabelMapNamespace.TabIndex = 2;
            this.LabelMapNamespace.Text = "Map namespace";
            // 
            // TextBoxMapName
            // 
            this.TextBoxMapName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxMapName.Enabled = false;
            this.TextBoxMapName.Location = new System.Drawing.Point(6, 38);
            this.TextBoxMapName.Name = "TextBoxMapName";
            this.TextBoxMapName.Size = new System.Drawing.Size(374, 22);
            this.TextBoxMapName.TabIndex = 1;
            // 
            // LabelMapName
            // 
            this.LabelMapName.AutoSize = true;
            this.LabelMapName.Location = new System.Drawing.Point(6, 18);
            this.LabelMapName.Name = "LabelMapName";
            this.LabelMapName.Size = new System.Drawing.Size(74, 17);
            this.LabelMapName.TabIndex = 0;
            this.LabelMapName.Text = "Map name";
            // 
            // DataGridViewMaps
            // 
            this.DataGridViewMaps.AllowUserToAddRows = false;
            this.DataGridViewMaps.AllowUserToDeleteRows = false;
            this.DataGridViewMaps.AllowUserToResizeRows = false;
            this.DataGridViewMaps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridViewMaps.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.DataGridViewMaps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewMaps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Map});
            this.DataGridViewMaps.Location = new System.Drawing.Point(198, 3);
            this.DataGridViewMaps.MultiSelect = false;
            this.DataGridViewMaps.Name = "DataGridViewMaps";
            this.DataGridViewMaps.ReadOnly = true;
            this.DataGridViewMaps.RowHeadersWidth = 51;
            this.TableLayoutPanelMain.SetRowSpan(this.DataGridViewMaps, 3);
            this.DataGridViewMaps.RowTemplate.Height = 24;
            this.DataGridViewMaps.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DataGridViewMaps.Size = new System.Drawing.Size(189, 516);
            this.DataGridViewMaps.TabIndex = 0;
            this.DataGridViewMaps.SelectionChanged += new System.EventHandler(this.DataGridViewMaps_SelectionChanged);
            // 
            // Map
            // 
            this.Map.HeaderText = "Map";
            this.Map.MinimumWidth = 6;
            this.Map.Name = "Map";
            this.Map.ReadOnly = true;
            this.Map.Width = 64;
            // 
            // DataGridViewArchives
            // 
            this.DataGridViewArchives.AllowUserToAddRows = false;
            this.DataGridViewArchives.AllowUserToDeleteRows = false;
            this.DataGridViewArchives.AllowUserToResizeRows = false;
            this.DataGridViewArchives.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridViewArchives.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.DataGridViewArchives.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewArchives.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Archive});
            this.DataGridViewArchives.Location = new System.Drawing.Point(3, 3);
            this.DataGridViewArchives.MultiSelect = false;
            this.DataGridViewArchives.Name = "DataGridViewArchives";
            this.DataGridViewArchives.ReadOnly = true;
            this.DataGridViewArchives.RowHeadersWidth = 51;
            this.TableLayoutPanelMain.SetRowSpan(this.DataGridViewArchives, 3);
            this.DataGridViewArchives.RowTemplate.Height = 24;
            this.DataGridViewArchives.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DataGridViewArchives.Size = new System.Drawing.Size(189, 516);
            this.DataGridViewArchives.TabIndex = 1;
            this.DataGridViewArchives.SelectionChanged += new System.EventHandler(this.DataGridViewArchives_SelectionChanged);
            // 
            // Archive
            // 
            this.Archive.HeaderText = "Archive";
            this.Archive.MinimumWidth = 6;
            this.Archive.Name = "Archive";
            this.Archive.ReadOnly = true;
            this.Archive.Width = 84;
            // 
            // GroupBoxArchive
            // 
            this.GroupBoxArchive.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBoxArchive.Controls.Add(this.TextBoxArchiveLastUpdate);
            this.GroupBoxArchive.Controls.Add(this.LabelArchiveLastUpdate);
            this.GroupBoxArchive.Controls.Add(this.TextBoxArchiveFileSize);
            this.GroupBoxArchive.Controls.Add(this.LabelArchiveFileSize);
            this.GroupBoxArchive.Controls.Add(this.TextBoxArchiveFileSpec);
            this.GroupBoxArchive.Controls.Add(this.LabelArchiveFileSpec);
            this.GroupBoxArchive.Controls.Add(this.TextBoxArchiveFileName);
            this.GroupBoxArchive.Controls.Add(this.LabelArchiveFileName);
            this.GroupBoxArchive.Location = new System.Drawing.Point(393, 3);
            this.GroupBoxArchive.Name = "GroupBoxArchive";
            this.GroupBoxArchive.Size = new System.Drawing.Size(386, 166);
            this.GroupBoxArchive.TabIndex = 2;
            this.GroupBoxArchive.TabStop = false;
            this.GroupBoxArchive.Text = "Selected archive";
            // 
            // TextBoxArchiveLastUpdate
            // 
            this.TextBoxArchiveLastUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxArchiveLastUpdate.Enabled = false;
            this.TextBoxArchiveLastUpdate.Location = new System.Drawing.Point(189, 128);
            this.TextBoxArchiveLastUpdate.Name = "TextBoxArchiveLastUpdate";
            this.TextBoxArchiveLastUpdate.Size = new System.Drawing.Size(150, 22);
            this.TextBoxArchiveLastUpdate.TabIndex = 7;
            // 
            // LabelArchiveLastUpdate
            // 
            this.LabelArchiveLastUpdate.AutoSize = true;
            this.LabelArchiveLastUpdate.Location = new System.Drawing.Point(189, 108);
            this.LabelArchiveLastUpdate.Name = "LabelArchiveLastUpdate";
            this.LabelArchiveLastUpdate.Size = new System.Drawing.Size(104, 17);
            this.LabelArchiveLastUpdate.TabIndex = 6;
            this.LabelArchiveLastUpdate.Text = "File last update";
            // 
            // TextBoxArchiveFileSize
            // 
            this.TextBoxArchiveFileSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxArchiveFileSize.Enabled = false;
            this.TextBoxArchiveFileSize.Location = new System.Drawing.Point(6, 128);
            this.TextBoxArchiveFileSize.Name = "TextBoxArchiveFileSize";
            this.TextBoxArchiveFileSize.Size = new System.Drawing.Size(150, 22);
            this.TextBoxArchiveFileSize.TabIndex = 5;
            // 
            // LabelArchiveFileSize
            // 
            this.LabelArchiveFileSize.AutoSize = true;
            this.LabelArchiveFileSize.Location = new System.Drawing.Point(6, 108);
            this.LabelArchiveFileSize.Name = "LabelArchiveFileSize";
            this.LabelArchiveFileSize.Size = new System.Drawing.Size(107, 17);
            this.LabelArchiveFileSize.TabIndex = 4;
            this.LabelArchiveFileSize.Text = "File size (bytes)";
            // 
            // TextBoxArchiveFileSpec
            // 
            this.TextBoxArchiveFileSpec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxArchiveFileSpec.Enabled = false;
            this.TextBoxArchiveFileSpec.Location = new System.Drawing.Point(6, 83);
            this.TextBoxArchiveFileSpec.Name = "TextBoxArchiveFileSpec";
            this.TextBoxArchiveFileSpec.Size = new System.Drawing.Size(374, 22);
            this.TextBoxArchiveFileSpec.TabIndex = 3;
            // 
            // LabelArchiveFileSpec
            // 
            this.LabelArchiveFileSpec.AutoSize = true;
            this.LabelArchiveFileSpec.Location = new System.Drawing.Point(6, 63);
            this.LabelArchiveFileSpec.Name = "LabelArchiveFileSpec";
            this.LabelArchiveFileSpec.Size = new System.Drawing.Size(108, 17);
            this.LabelArchiveFileSpec.TabIndex = 2;
            this.LabelArchiveFileSpec.Text = "Archive location";
            // 
            // TextBoxArchiveFileName
            // 
            this.TextBoxArchiveFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxArchiveFileName.Enabled = false;
            this.TextBoxArchiveFileName.Location = new System.Drawing.Point(6, 38);
            this.TextBoxArchiveFileName.Name = "TextBoxArchiveFileName";
            this.TextBoxArchiveFileName.Size = new System.Drawing.Size(374, 22);
            this.TextBoxArchiveFileName.TabIndex = 1;
            // 
            // LabelArchiveFileName
            // 
            this.LabelArchiveFileName.AutoSize = true;
            this.LabelArchiveFileName.Location = new System.Drawing.Point(6, 18);
            this.LabelArchiveFileName.Name = "LabelArchiveFileName";
            this.LabelArchiveFileName.Size = new System.Drawing.Size(94, 17);
            this.LabelArchiveFileName.TabIndex = 0;
            this.LabelArchiveFileName.Text = "Archive name";
            // 
            // GroupBoxNavmesh
            // 
            this.GroupBoxNavmesh.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBoxNavmesh.Controls.Add(this.TextBoxActorRadius);
            this.GroupBoxNavmesh.Controls.Add(this.LabelActorRadius);
            this.GroupBoxNavmesh.Controls.Add(this.TextBoxActorHeight);
            this.GroupBoxNavmesh.Controls.Add(this.LabelActorHeight);
            this.GroupBoxNavmesh.Controls.Add(this.ButtonGenerateNavmesh);
            this.GroupBoxNavmesh.Location = new System.Drawing.Point(393, 347);
            this.GroupBoxNavmesh.Name = "GroupBoxNavmesh";
            this.GroupBoxNavmesh.Size = new System.Drawing.Size(386, 172);
            this.GroupBoxNavmesh.TabIndex = 4;
            this.GroupBoxNavmesh.TabStop = false;
            this.GroupBoxNavmesh.Text = "Navigation mesh";
            // 
            // ButtonGenerateNavmesh
            // 
            this.ButtonGenerateNavmesh.Location = new System.Drawing.Point(6, 122);
            this.ButtonGenerateNavmesh.Name = "ButtonGenerateNavmesh";
            this.ButtonGenerateNavmesh.Size = new System.Drawing.Size(371, 41);
            this.ButtonGenerateNavmesh.TabIndex = 0;
            this.ButtonGenerateNavmesh.Text = "Generate navmesh...";
            this.ButtonGenerateNavmesh.UseVisualStyleBackColor = true;
            this.ButtonGenerateNavmesh.Click += new System.EventHandler(this.ButtonGenerateNavmesh_Click);
            // 
            // TextBoxActorHeight
            // 
            this.TextBoxActorHeight.Location = new System.Drawing.Point(6, 38);
            this.TextBoxActorHeight.MaxLength = 4;
            this.TextBoxActorHeight.Name = "TextBoxActorHeight";
            this.TextBoxActorHeight.Size = new System.Drawing.Size(100, 22);
            this.TextBoxActorHeight.TabIndex = 5;
            this.TextBoxActorHeight.Text = "128";
            this.TextBoxActorHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LabelActorHeight
            // 
            this.LabelActorHeight.AutoSize = true;
            this.LabelActorHeight.Location = new System.Drawing.Point(6, 18);
            this.LabelActorHeight.Name = "LabelActorHeight";
            this.LabelActorHeight.Size = new System.Drawing.Size(91, 17);
            this.LabelActorHeight.TabIndex = 4;
            this.LabelActorHeight.Text = "Actors height";
            // 
            // TextBoxActorRadius
            // 
            this.TextBoxActorRadius.Location = new System.Drawing.Point(6, 83);
            this.TextBoxActorRadius.MaxLength = 4;
            this.TextBoxActorRadius.Name = "TextBoxActorRadius";
            this.TextBoxActorRadius.Size = new System.Drawing.Size(100, 22);
            this.TextBoxActorRadius.TabIndex = 7;
            this.TextBoxActorRadius.Text = "32";
            this.TextBoxActorRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LabelActorRadius
            // 
            this.LabelActorRadius.AutoSize = true;
            this.LabelActorRadius.Location = new System.Drawing.Point(6, 63);
            this.LabelActorRadius.Name = "LabelActorRadius";
            this.LabelActorRadius.Size = new System.Drawing.Size(91, 17);
            this.LabelActorRadius.TabIndex = 6;
            this.LabelActorRadius.Text = "Actors radius";
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.TableLayoutPanelMain);
            this.Controls.Add(this.MenuStripMain);
            this.MainMenuStrip = this.MenuStripMain;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FormMain";
            this.Text = "ZDoom NavMesh builder";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FormMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FormMain_DragEnter);
            this.MenuStripMain.ResumeLayout(false);
            this.MenuStripMain.PerformLayout();
            this.TableLayoutPanelMain.ResumeLayout(false);
            this.GroupBoxMap.ResumeLayout(false);
            this.GroupBoxMap.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewMaps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewArchives)).EndInit();
            this.GroupBoxArchive.ResumeLayout(false);
            this.GroupBoxArchive.PerformLayout();
            this.GroupBoxNavmesh.ResumeLayout(false);
            this.GroupBoxNavmesh.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

