using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZDoomNavMesh;

using NavmeshBuilder;

public partial class FormViewMap : Form
{
    private TMapDefinition? MapDefinition { get; set; }
    private TNavMesh? NavMesh { get; set; }
    private Nullable<int> MinX { get; set; }
    private Nullable<int> MinY { get; set; }
    private Nullable<int> MaxX { get; set; }
    private Nullable<int> MaxY { get; set; }
    private int PosX { get; set; }
    private int PosY { get; set; }
    private int ZoomFactor { get; set; }
    public FormViewMap() : base()
    {
        InitializeComponent();
        PosX = 0;
        PosY = 0;
        ZoomFactor = 3;
    }
    protected override bool ProcessCmdKey(ref Message Msg, Keys KeyData)
    {
        bool Repaint = false;
        switch (KeyData)
        {
            case Keys.Home:
                PosX = 32768;
                PosY = 32768;
                Repaint = true;
                break;
            case Keys.Left:
                PosX = Math.Clamp(PosX - (16 << ZoomFactor), 0, 65536);
                Repaint = true;
                break;
            case Keys.Right:
                PosX = Math.Clamp(PosX + (16 << ZoomFactor), 0, 65536);
                Repaint = true;
                break;
            case Keys.Down:
                PosY = Math.Clamp(PosY + (16 << ZoomFactor), 0, 65536);
                Repaint = true;
                break;
            case Keys.Up:
                PosY = Math.Clamp(PosY - (16 << ZoomFactor), 0, 65536);
                Repaint = true;
                break;
            case Keys.Shift | Keys.Left:
                PosX = Math.Clamp(PosX - (1 << ZoomFactor), 0, 65536);
                Repaint = true;
                break;
            case Keys.Shift | Keys.Right:
                PosX = Math.Clamp(PosX + (1 << ZoomFactor), 0, 65536);
                Repaint = true;
                break;
            case Keys.Shift | Keys.Down:
                PosY = Math.Clamp(PosY + (1 << ZoomFactor), 0, 65536);
                Repaint = true;
                break;
            case Keys.Shift | Keys.Up:
                PosY = Math.Clamp(PosY - (1 << ZoomFactor), 0, 65536);
                Repaint = true;
                break;
        }
        if (Repaint)
            PictureBoxMap.Invalidate();
        return base.ProcessCmdKey(ref Msg, KeyData);
    }
    public void View(TMapDefinition MapDefinition, TNavMesh? NavMesh)
    {
        this.MapDefinition = MapDefinition ?? throw new ArgumentNullException(nameof(MapDefinition));
        this.NavMesh = NavMesh;
        if (NavMesh is null)
        {
            CheckBoxViewNavMesh.Checked = false;
            CheckBoxViewNavMesh.Enabled = false;
        }
        else
            CheckBoxViewNavMesh.Enabled = true;
        foreach (TMapVertex MapVertex in MapDefinition.MapVertex)
        {
            if ((!MaxX.HasValue) || (MaxX.Value < MapVertex.X))
                MaxX = MapVertex.X;
            if ((!MaxY.HasValue) || (MaxY.Value < MapVertex.Y))
                MaxY = MapVertex.Y;
            if ((!MinX.HasValue) || (MinX.Value > MapVertex.X))
                MinX = MapVertex.X;
            if ((!MinY.HasValue) || (MinY.Value > MapVertex.Y))
                MinY = MapVertex.Y;
        }
        PosX = 32768;
        PosY = 32768;
        Show();
    }
    private void ButtonZoomIn_Click(object Sender, EventArgs E)
    {
        ZoomFactor--;
        if (ZoomFactor < 1)
            ZoomFactor = 1;
        PictureBoxMap.Invalidate();
    }
    private void ButtonZoomOut_Click(object Sender, EventArgs E)
    {
        ZoomFactor++;
        if (ZoomFactor > 12)
            ZoomFactor = 12;
        PictureBoxMap.Invalidate();
    }
    private void CheckBoxViewNavMesh_CheckedChanged(object Sender, EventArgs E)
    {
        PictureBoxMap.Invalidate();
    }
    private void PictureBoxMap_MouseMove(object Sender, MouseEventArgs E) 
    {
        PictureBoxMap.Invalidate();
    }
    private void PictureBoxMap_Paint(object Sender, PaintEventArgs E)
    {
        TextBoxZoom.Text = "1 : " + Convert.ToString(1 << ZoomFactor);
        int ViewMinX = 0;
        int ViewMinY = 0;
        int ViewMaxX = ViewMinX + PictureBoxMap.Width;
        int ViewMaxY = ViewMinY + PictureBoxMap.Height;
        if (MapDefinition is null)
            return;
        Pen BluePen = new Pen(Color.Blue);
        Pen GrayPen = new Pen(Color.Gray);
        Pen GreenPen = new Pen(Color.Green, 3);
        Pen RedPen = new Pen(Color.Red);
        Pen WhitePen = new Pen(Color.White);
        Pen YellowPen = new Pen(Color.Yellow, 3);
        // Line clipping, by Cohen–Sutherland Algorithm.
        const int INSIDE = 0b0000;
        const int LEFT = 0b0001;
        const int RIGHT = 0b0010;
        const int BOTTOM = 0b0100;
        const int TOP = 0b1000;
        Func<int, int, int> ComputeRC = (X, Y) => {
            int Result = INSIDE;
            if (X < ViewMinX)
                Result |= LEFT;
            if (X > ViewMaxX)
                Result |= RIGHT;
            if (Y < ViewMinY)
                Result |= BOTTOM;
            if (Y > ViewMaxY)
                Result |= TOP;
            return Result;
        };
        Func<int, int, int, int, bool> ComputeVisible = (X1, Y1, X2, Y2) =>
        {
            bool Result = false;
            int RC1 = ComputeRC(X1, Y1);
            int RC2 = ComputeRC(X2, Y2);
            while (true)
            {
                if ((RC1 == 0) && (RC2 == 0))
                {
                    // Both endpoints are visible.
                    Result = true;
                    break;
                }
                else
                {
                    if ((RC1 & RC2) != 0)
                    {
                        // Both endpoints are not visible, in same region.
                        break;
                    }
                    else
                    {
                        // Some segment of line lies within the screen port.
                        int RC;
                        double X;
                        double Y;
                        // At least one endpoint is outside the screen port, pick it.
                        if (RC1 != 0)
                            RC = RC1;
                        else
                            RC = RC2;
                        // Find intersection point using formulas y = y1 + slope * (x - x1), x = x1 + (1 / slope) * (y - y1).
                        if ((RC & TOP) != 0)
                        {
                            // Point is above the screen port.
                            X = X1 + (X2 - X1) * (ViewMaxY - Y1) / (Y2 - Y1);
                            Y = ViewMaxY;
                        }
                        else
                            if ((RC & BOTTOM) != 0)
                        {
                            // Point is below the screen port.
                            X = X1 + (X2 - X1) * (ViewMaxY - Y1) / (Y2 - Y1);
                            Y = ViewMinY;
                        }
                        else
                                if ((RC & RIGHT) != 0)
                        {
                            // Point is to the right of the screen port.
                            Y = Y1 + (Y2 - X1) * (ViewMaxY - X1) / (X2 - X1);
                            X = ViewMaxX;
                        }
                        else
                                    if ((RC & LEFT) != 0)
                        {
                            // Point is to the left of the screen port.
                            Y = Y1 + (X2 - X1) * (ViewMinY - X1) / (X2 - X1);
                            X = ViewMinX;
                        }
                        else
                        {
                            X = 0;
                            Y = 0;
                        }
                        if (RC == RC1)
                        {
                            X1 = Convert.ToInt32(X);
                            Y1 = Convert.ToInt32(Y);
                            RC1 = ComputeRC(X1, Y1);
                        }
                        else
                        {
                            X2 = Convert.ToInt32(X);
                            Y2 = Convert.ToInt32(Y);
                            RC2 = ComputeRC(X2, Y2);
                        }
                    }
                }
            }
            return Result;
        };
        foreach (TMapLinedef MapLinedef in MapDefinition.MapLinedef)
        {
            TMapVertex V1 = MapDefinition.MapVertex[MapLinedef.V1];
            TMapVertex V2 = MapDefinition.MapVertex[MapLinedef.V2];
            int V1X = (V1.X - PosX + 32768) >> ZoomFactor;
            int V1Y = ((-V1.Y) - PosY + 32768) >> ZoomFactor;
            int V2X = (V2.X - PosX + 32768) >> ZoomFactor;
            int V2Y = ((-V2.Y) - PosY + 32768) >> ZoomFactor;
            if (ComputeVisible(V1X, V1Y, V2X, V2Y))
            /*if (
                ((V1X >= ViewMinX) && (V1X <= ViewMaxX) && (V1Y >= ViewMinY) && (V1Y <= ViewMaxY))
                || ((V2X >= ViewMinX) && (V2X <= ViewMaxX) && (V2Y >= ViewMinY) && (V2Y <= ViewMaxY))
                || ((V1X < ViewMinX) && (V2X > ViewMaxX) && (V1Y >= ViewMinY) && (V2Y <= ViewMaxY))
                || ((V1X >= ViewMinX) && (V2X <= ViewMaxX) && (V1Y < ViewMinY) && (V2Y > ViewMaxY))
            )*/
            {
                if (MapLinedef.Blocking)
                    E.Graphics.DrawLine(WhitePen, V1X, V1Y, V2X, V2Y);
                else
                    E.Graphics.DrawLine(GrayPen, V1X, V1Y, V2X, V2Y);
            }
        }
        if ((NavMesh is not null) && (CheckBoxViewNavMesh.Checked))
        {
            foreach (TLine Line in NavMesh.Lines)
            {
                int V1X = (Line.A.X - PosX + 32768) >> ZoomFactor;
                int V1Y = ((-Line.A.Y) - PosY + 32768) >> ZoomFactor;
                int V2X = (Line.B.X - PosX + 32768) >> ZoomFactor;
                int V2Y = ((-Line.B.Y) - PosY + 32768) >> ZoomFactor;
                /*if (
                    ((V1X >= ViewMinX) && (V1X <= ViewMaxX) && (V1Y >= ViewMinY) && (V1Y <= ViewMaxY))
                    || ((V2X >= ViewMinX) && (V2X <= ViewMaxX) && (V2Y >= ViewMinY) && (V2Y <= ViewMaxY))
                    || ((V1X < ViewMinX) && (V2X > ViewMaxX) && (V1Y >= ViewMinY) && (V2Y <= ViewMaxY))
                    || ((V1X >= ViewMinX) && (V2X <= ViewMaxX) && (V1Y < ViewMinY) && (V2Y > ViewMaxY))
                )*/
                if (ComputeVisible(V1X, V1Y, V2X, V2Y))
                {
                    if (Line.Portal >= 0)
                        E.Graphics.DrawLine(BluePen, V1X, V1Y, V2X, V2Y);
                    else
                        E.Graphics.DrawLine(RedPen, V1X, V1Y, V2X, V2Y);
                }
            }
            int MouseX = PictureBoxMap.PointToClient(Cursor.Position).X;
            int MouseY = PictureBoxMap.PointToClient(Cursor.Position).Y;
            int PointX = (MouseX << ZoomFactor) + PosX - 32768;
            int PointY = (-(MouseY << ZoomFactor) + PosY - 32768) + ((32768 - PosY) << 1);
            TPoint Point = new TPoint(PointX, PointY);
            int CellX = ((Point.X + 32768) >> 8) - NavMesh.OffsetCellX;
            int CellY = ((Point.Y + 32768) >> 8) - NavMesh.OffsetCellY;
            bool NotFound = true;
            int PolygonIndex = 0;
            if ((CellX >= 0) && (CellX < NavMesh.FNumCellX) && (CellY >= 0) && (CellY < NavMesh.FNumCellY))
            {
                int I = 0;
                while ((NotFound) && (I < NavMesh.Cells![CellY, CellX].Count)) 
                {
                    bool PointInsidePolygon = false;
                    PolygonIndex = NavMesh.Cells[CellY, CellX][I];
                    TPoint Point1 = NavMesh.Polygons[PolygonIndex].Lines[0].A;
                    foreach (TLine Line in NavMesh.Polygons[PolygonIndex].Lines)
                    {
                        TPoint Point2 = Line.B;
                        if (Point.Y > Math.Min(Point1.Y, Point2.Y))
                            if (Point.Y <= Math.Max(Point1.Y, Point2.Y))
                                if (Point.X <= Math.Max(Point1.X, Point2.X))
                                {
                                    double IntersectionX = (Point.Y - Point1.Y) * (Point2.X - Point1.X) / (Point2.Y - Point1.Y) + Point1.X;
                                    if ((Point1.X == Point2.X) || (Point.X <= IntersectionX))
                                        PointInsidePolygon = !PointInsidePolygon;
                                }
                        Point1 = Point2;
                    }
                    if (PointInsidePolygon)
                        NotFound = false;
                    else
                        I++;
                }
            }
            if (!NotFound)
            {
                TPolygon Polygon = NavMesh.Polygons[PolygonIndex];
                for (int LineIndex = 0; LineIndex < Polygon.Lines.Count; LineIndex++) 
                {
                    TLine Line = Polygon.Lines[LineIndex];
                    int V1X = (Line.A.X - PosX + 32768) >> ZoomFactor;
                    int V1Y = ((-Line.A.Y) - PosY + 32768) >> ZoomFactor;
                    int V2X = (Line.B.X - PosX + 32768) >> ZoomFactor;
                    int V2Y = ((-Line.B.Y) - PosY + 32768) >> ZoomFactor;
                    if (
                        ((V1X >= ViewMinX) && (V1X <= ViewMaxX) && (V1Y >= ViewMinY) && (V1Y <= ViewMaxY))
                        || ((V2X >= ViewMinX) && (V2X <= ViewMaxX) && (V2Y >= ViewMinY) && (V2Y <= ViewMaxY))
                        || ((V1X < ViewMinX) && (V2X > ViewMaxX) && (V1Y >= ViewMinY) && (V2Y <= ViewMaxY))
                        || ((V1X >= ViewMinX) && (V2X <= ViewMaxX) && (V1Y < ViewMinY) && (V2Y > ViewMaxY))
                    )
                    {
                        if (Polygon.BlockingLines.Contains(LineIndex))
                            E.Graphics.DrawLine(YellowPen, V1X, V1Y, V2X, V2Y);
                        else
                            E.Graphics.DrawLine(GreenPen, V1X, V1Y, V2X, V2Y);
                    }
                }
            }
        }
    }
}
