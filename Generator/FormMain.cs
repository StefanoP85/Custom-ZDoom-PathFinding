using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ZDoomNavMesh;
using NavmeshBuilder;

public partial class FormMain : Form
{
    private List<TArchive> Archives;
    private TNavMesh? NavMesh;
    public FormMain() : base()
    {
        InitializeComponent();
        Archives = new List<TArchive>();
    }
    private void AddArchive(string FileName)
    {
        TArchive Archive = new TArchive();
        try
        {
            if (Archive.Load(FileName))
            {
                Archives.Add(Archive);
                Archives.Sort(delegate (TArchive A, TArchive B) { return string.Compare(A.FileName, B.FileName, StringComparison.Ordinal); });
                UpdateArchives();
            }
        }
        catch (Exception Ex)
        {
            MessageBox.Show(Ex.Message);
        }
    }
    private void UpdateArchives()
    {
        DataGridViewArchives.Rows.Clear();
        foreach (TArchive LoadedArchive in Archives)
            DataGridViewArchives.Rows.Add(LoadedArchive.FileName);
    }
    private void UpdateMaps()
    {
        DataGridViewMaps.Rows.Clear();
        if (!(DataGridViewArchives.CurrentCell is null))
        {
            int ArchiveIndex = DataGridViewArchives.CurrentCell.RowIndex;
            TArchive Archive = Archives[ArchiveIndex];
            foreach (TMapDefinition MapDefinition in Archive.MapDefinitions)
                DataGridViewMaps.Rows.Add(MapDefinition.MapName);
        }
    }
    private void ButtonGenerateNavmesh_Click(object Sender, EventArgs E)
    {
        ushort ActorHeight;
        if (!UInt16.TryParse(TextBoxActorHeight.Text, out ActorHeight))
        {
            MessageBox.Show("Invalid actor height!");
            return;
        }
        if ((ActorHeight == 0) || (ActorHeight > 256))
        {
            MessageBox.Show("Actor height should be > 0 and <= 256!");
            return;
        }
        ushort ActorRadius;
        if (!UInt16.TryParse(TextBoxActorRadius.Text, out ActorRadius))
        {
            MessageBox.Show("Invalid actor radius!");
            return;
        }
        if ((ActorHeight == 0) || (ActorHeight > 256))
        {
            MessageBox.Show("Actor radius should be > 0 and <= 256!");
            return;
        }
        if (!(DataGridViewArchives.CurrentCell is null) && (!(DataGridViewMaps.CurrentCell is null)))
        {
            int ArchiveIndex = DataGridViewArchives.CurrentCell.RowIndex;
            TArchive Archive = Archives[ArchiveIndex];
            int MapIndex = DataGridViewMaps.CurrentCell.RowIndex;
            TMapDefinition MapDefinition = Archive.MapDefinitions[MapIndex];
            TNavMeshSettings NavMeshSettings = new TNavMeshSettings(ActorHeight, ActorRadius);
            NavMesh = new TNavMesh();
            NavMesh.Build(NavMeshSettings, MapDefinition);
            if (NavMesh.Messages.Count > 1)
                MessageBox.Show("Navigation mesh built with messages!");
            FormViewMap Viewer = new FormViewMap();
            Viewer.View(MapDefinition, NavMesh);
        }
    }
    private void DataGridViewArchives_SelectionChanged(object Sender, EventArgs E)
    {
        NavMesh = null;
        if (DataGridViewArchives.CurrentCell is null)
        {
            TextBoxArchiveFileName.Text = "";
            TextBoxArchiveFileSize.Text = "";
            TextBoxArchiveFileSpec.Text = "";
            TextBoxArchiveLastUpdate.Text = "";
        }
        else
        {
            int ArchiveIndex = DataGridViewArchives.CurrentCell.RowIndex;
            TArchive Archive = Archives[ArchiveIndex];
            TextBoxArchiveFileName.Text = Archive.FileName;
            TextBoxArchiveFileSize.Text = Convert.ToString(Archive.FileSize);
            TextBoxArchiveFileSpec.Text = Archive.FileSpec;
            TextBoxArchiveLastUpdate.Text = Convert.ToString(Archive.LastUpdate);
        }
        UpdateMaps();
    }
    private void DataGridViewMaps_SelectionChanged(object Sender, EventArgs E)
    {
        NavMesh = null;
        if ((DataGridViewArchives.CurrentCell is null) || (DataGridViewMaps.CurrentCell is null))
        {
            TextBoxMapName.Text = "";
            TextBoxMapNamespace.Text = "";
            TextBoxMapVertex.Text = "";
            TextBoxMapLinedef.Text = "";
            TextBoxMapSidedef.Text = "";
            TextBoxMapSector.Text = "";
            TextBoxMapThing.Text = "";
            ButtonGenerateNavmesh.Enabled = false;
        }
        else
        {
            int ArchiveIndex = DataGridViewArchives.CurrentCell.RowIndex;
            TArchive Archive = Archives[ArchiveIndex];
            int MapIndex = DataGridViewMaps.CurrentCell.RowIndex;
            TMapDefinition MapDefinition = Archive.MapDefinitions[MapIndex];
            TextBoxMapName.Text = MapDefinition.MapName;
            TextBoxMapNamespace.Text = MapDefinition.MapNamespaceText();
            TextBoxMapVertex.Text = Convert.ToString(MapDefinition.MapVertex.Count);
            TextBoxMapLinedef.Text = Convert.ToString(MapDefinition.MapLinedef.Count);
            TextBoxMapSidedef.Text = Convert.ToString(MapDefinition.MapSidedef.Count);
            TextBoxMapSector.Text = Convert.ToString(MapDefinition.MapSector.Count);
            TextBoxMapThing.Text = Convert.ToString(MapDefinition.MapThing.Count);
            ButtonGenerateNavmesh.Enabled = true;
        }
    }
    private void FormMain_DragDrop(object Sender, DragEventArgs E)
    {
        if (E.Data is not null)
        {
            string[]? Files = E.Data.GetData(DataFormats.FileDrop) as string[];
            if (Files is not null)
                foreach (string FileName in Files)
                    AddArchive(FileName);
        }
    }
    private void FormMain_DragEnter(object Sender, DragEventArgs E)
    {
        E.Effect = DragDropEffects.Move;
    }
    private void ToolStripMenuItemFileClose_Click(object Sender, EventArgs E)
    {
        foreach (DataGridViewRow Item in DataGridViewArchives.SelectedRows)
        {
            Archives.RemoveAt(Item.Index);
            DataGridViewArchives.Rows.RemoveAt(Item.Index);
        }
    }
    private void ToolStripMenuItemFileExit_Click(object Sender, EventArgs E)
    {
        Application.Exit();
    }
    private void ToolStripMenuItemFileOpen_Click(object Sender, EventArgs E)
    {
        OpenFileDialog OpenArchive = new OpenFileDialog();
        if (OpenArchive.ShowDialog() == DialogResult.OK)
            AddArchive(OpenArchive.FileName);
    }
    private void ToolStripMenuItemFileOpenLocation_Click(object Sender, EventArgs E)
    {
        if (!(DataGridViewArchives.CurrentCell is null))
        {
            int ArchiveIndex = DataGridViewArchives.CurrentCell.RowIndex;
            TArchive Archive = Archives[ArchiveIndex];
            string FilePath = Path.GetDirectoryName(Archive.FileSpec) ?? "";
            ProcessStartInfo StartInfo = new ProcessStartInfo("explorer.exe", FilePath);
            Process.Start(StartInfo);
        }
    }
    private void ToolStripMenuItemFileSave_Click(object Sender, EventArgs E)
    {
        if (NavMesh is null)
        {
            MessageBox.Show("NavMesh not created yet!");
            return;
        }
        SaveFileDialog SaveDialog = new SaveFileDialog();
        SaveDialog.DefaultExt = ".TXT";
        SaveDialog.Title = "Save NavMesh";
        if (SaveDialog.ShowDialog() == DialogResult.OK)
            using (StreamWriter Writer = new StreamWriter(SaveDialog.FileName))
            {
                Writer.WriteLine(NavMesh.ToString());
                Writer.Flush();
                Writer.Close();
            }
    }
    private void ToolStripMenuItemHelpAbout_Click(object Sender, EventArgs E)
    {
        new FormAbout().Show();
    }
    private void ToolStripMenuItemViewMap_Click(object Sender, EventArgs E)
    {
        if (DataGridViewArchives.CurrentCell is null)
            return;
        if (DataGridViewMaps.CurrentCell is null)
            return;
        int ArchiveIndex = DataGridViewArchives.CurrentCell.RowIndex;
        TArchive Archive = Archives[ArchiveIndex];
        int MapIndex = DataGridViewMaps.CurrentCell.RowIndex;
        TMapDefinition MapDefinition = Archive.MapDefinitions[MapIndex];
        FormViewMap Viewer = new FormViewMap();
        Viewer.View(MapDefinition, NavMesh);
    }
    private void ToolStripMenuItemViewMessages_Click(object sender, EventArgs e)
    {
        if (NavMesh is null)
        {
            MessageBox.Show("NavMesh not created yet!");
            return;
        }
        FormViewMessages Viewer = new FormViewMessages();
        Viewer.View(NavMesh);
    }
    private void ToolStripMenuItemViewNavmesh_Click(object Sender, EventArgs E)
    {
        if (NavMesh is null)
        {
            MessageBox.Show("NavMesh not created yet!");
            return;
        }
        FormViewNavMesh Viewer = new FormViewNavMesh();
        Viewer.View(NavMesh);
    }
}
