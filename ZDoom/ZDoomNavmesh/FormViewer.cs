using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZDoomNavmesh
{
    public partial class FormViewer : Form
    {
        private TMapDefinition MapDefinition;
        private TNavMesh NavMesh;
        public FormViewer() : base()
        {
            InitializeComponent();
            this.BackColor = Color.Black;
            this.DoubleBuffered = true;
        }
        public void View(TMapDefinition MapDefinition, TNavMesh NavMesh)
        {
            if (MapDefinition is null)
                Close();
            this.MapDefinition = MapDefinition;
            this.NavMesh = NavMesh;
            Show();
        }
        private void PanelViewer_Paint(object Sender, PaintEventArgs E)
        {
            Panel P = Sender as Panel;
            Rectangle TargetRectangle = P.ClientRectangle;
            using (BufferedGraphicsContext BGC = new BufferedGraphicsContext())
            { 
                using (BufferedGraphics BG = BGC.Allocate(E.Graphics, TargetRectangle))
                {
                    Graphics G = BG.Graphics;
                    G.FillRectangle(new SolidBrush(Color.FromArgb(0, Color.Black)), P.DisplayRectangle);
                    Pen BluePen = new Pen(Color.Blue, 1);
                    Pen GrayPen = new Pen(Color.Gray, 3);
                    Pen RedPen = new Pen(Color.Red, 1);
                    Pen WhitePen = new Pen(Color.White, 3);
                    int MaxX = Int32.MinValue;
                    int MaxY = Int32.MinValue;
                    int MinX = Int32.MaxValue;
                    int MinY = Int32.MaxValue;
                    for (int I = 0; I < MapDefinition.MapVertex.Count; I++)
                    {
                        if (MaxX < MapDefinition.MapVertex[I].X)
                            MaxX = MapDefinition.MapVertex[I].X;
                        if (MaxY < MapDefinition.MapVertex[I].Y)
                            MaxY = MapDefinition.MapVertex[I].Y;
                        if (MinX > MapDefinition.MapVertex[I].X)
                            MinX = MapDefinition.MapVertex[I].X;
                        if (MinY > MapDefinition.MapVertex[I].Y)
                            MinY = MapDefinition.MapVertex[I].Y;
                    }
                    int ScaleFactorX = (MaxX - MinX) / P.Width;
                    int ScaleFactorY = (MaxY - MinY) / P.Height;
                    int ScaleFactor = Math.Max(ScaleFactorX, ScaleFactorY);
                    foreach (TMapLinedef MapLinedef in MapDefinition.MapLinedef)
                    {
                        Point V1 = new Point((MapDefinition.MapVertex[MapLinedef.V1].X - MinX) / ScaleFactor, P.Height - (MapDefinition.MapVertex[MapLinedef.V1].Y - MinY) / ScaleFactor);
                        Point V2 = new Point((MapDefinition.MapVertex[MapLinedef.V2].X - MinX) / ScaleFactor, P.Height - (MapDefinition.MapVertex[MapLinedef.V2].Y - MinY) / ScaleFactor);
                        if (MapLinedef.Blocking)
                            G.DrawLine(WhitePen, V1, V2);
                        else
                            G.DrawLine(GrayPen, V1, V2);
                    }
                    if (!(NavMesh is null))
                        foreach (TLine Line in NavMesh.Lines)
                        {
                            Point V1 = new Point((Line.A.X - MinX) / ScaleFactor, P.Height - (Line.A.Y - MinY) / ScaleFactor);
                            Point V2 = new Point((Line.B.X - MinX) / ScaleFactor, P.Height - (Line.B.Y - MinY) / ScaleFactor);
                            if (Line.Portal >= 0)
                                G.DrawLine(BluePen, V1, V2);
                            else
                                G.DrawLine(RedPen, V1, V2);
                        }
                    BG.Render(E.Graphics);
                }
            }
        }
    }
}
