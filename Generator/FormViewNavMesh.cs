using System.Windows.Forms;

namespace ZDoomNavMesh;
using NavmeshBuilder;

public partial class FormViewNavMesh : Form
{
    public FormViewNavMesh() : base()
    {
        InitializeComponent();
    }
    public void View(TNavMesh NavMesh)
    {
        TextBoxNavMesh.Text = NavMesh.ToString();
        Show();
    }
}
