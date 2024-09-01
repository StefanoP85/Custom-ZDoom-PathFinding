using System;
using System.Windows.Forms;

namespace ZDoomNavMesh;
using NavmeshBuilder;

public partial class FormViewMessages : Form
{
    public FormViewMessages() : base()
    {
        InitializeComponent();
    }
    public void View(TNavMesh NavMesh)
    {
        TextBoxNavMesh.Text = string.Join(Environment.NewLine, NavMesh.Messages.ToArray());
        Show();
    }
}
