using System.Windows.Forms;

namespace ZDoomNavmesh
{
    public partial class FormViewNavMesh : Form
    {
        private TNavMesh NavMesh;
        public FormViewNavMesh() : base()
        {
            InitializeComponent();
        }
        public void View(TNavMesh NavMesh)
        {
            if (NavMesh is null)
                Close();
            this.NavMesh = NavMesh;
            TextBoxNavMesh.Text = NavMesh.ToString();
            Show();
        }
    }
}
