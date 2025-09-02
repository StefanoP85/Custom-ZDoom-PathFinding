Option Explicit

Public Sub BuildChart()
    Dim NewChart As Chart
    Dim NewSeries As Series
    Dim Row As Integer
    Dim Source As Worksheet
    On Error Resume Next
    Application.DisplayAlerts = False
    ThisWorkbook.Sheets.Item("GInput").Delete
    Application.DisplayAlerts = True
    Set Source = ThisWorkbook.Sheets.Item("Input")
    Source.Cells(1, 16).Select
    Row = 2
    Set NewChart = Source.Shapes.AddChart2(240, xlXYScatterLines).Chart
    NewChart.Location Where:=xlLocationAsNewSheet, Name:="GInput"
    Sheets("GInput").Move After:=Source
    NewChart.Select
    ActiveChart.ChartTitle.Delete
    ActiveChart.Legend.Delete
    Do While ActiveChart.FullSeriesCollection.Count > 0
        ActiveChart.FullSeriesCollection.Item(1).Delete
    Loop
    Do While Source.Cells(Row, 10).Value <> ""
        Set NewSeries = ActiveChart.SeriesCollection.NewSeries
        NewSeries.Name = "=Input!$J$" & Row
        NewSeries.XValues = "=Input!$K$" & Row & ":$L$" & Row
        NewSeries.Values = "=Input!$M$" & Row & ":$N$" & Row
        NewSeries.Format.Fill.Visible = msoTrue
        NewSeries.Format.Fill.ForeColor.ObjectThemeColor = msoThemeColorText1
        NewSeries.Format.Fill.ForeColor.TintAndShade = 0
        NewSeries.Format.Fill.ForeColor.Brightness = 0
        NewSeries.Format.Fill.ForeColor.RGB = RGB(0, 0, 0)
        NewSeries.Format.Fill.Transparency = 0
        NewSeries.Format.Fill.Solid
        NewSeries.Format.Line.Visible = msoTrue
        NewSeries.Format.Line.ForeColor.RGB = RGB(0, 0, 0)
        NewSeries.Format.Line.Transparency = 0
        Row = Row + 1
    Loop
    ' Update the markers.
    For Each NewSeries In ActiveChart.FullSeriesCollection
        NewSeries.Format.Fill.ForeColor.ObjectThemeColor = msoThemeColorText1
        NewSeries.Format.Fill.ForeColor.TintAndShade = 0
        NewSeries.Format.Fill.ForeColor.Brightness = 0
        NewSeries.Format.Fill.ForeColor.RGB = RGB(0, 0, 0)
    Next
End Sub

Sub VisualizePolygons()
    Dim Coordinates() As String
    Dim I As Integer
    Dim NewShape As Shape
    Dim Points As Integer
    Dim Row As Integer
    Dim ShapeBuilder As FreeformBuilder
    Dim Source As Worksheet
    Polygonizer.Process
    Set Source = ThisWorkbook.Sheets.Item("Output")
    For Each NewShape In Source.Shapes
        NewShape.Delete
    Next
    Row = 2
    Do While Source.Cells(Row, 2).Value <> ""
        Coordinates = Split(Replace(Replace(Source.Cells(Row, 3).Value, "(", ""), ")", ""), ",")
        Points = (UBound(Coordinates) - LBound(Coordinates) + 1) \ 2
        Set ShapeBuilder = Source.Shapes.BuildFreeform(msoEditingAuto, Coordinates(0) / 2, Coordinates(1) / 2 + 300)
        For I = 1 To Points - 1
            ShapeBuilder.AddNodes msoSegmentLine, msoEditingAuto, Coordinates(I * 2) / 2, Coordinates(I * 2 + 1) / 2 + 300
        Next
        ShapeBuilder.AddNodes msoSegmentLine, msoEditingAuto, Coordinates(0) / 2, Coordinates(1) / 2 + 300
        Set NewShape = ShapeBuilder.ConvertToShape
        NewShape.Fill.ForeColor.RGB = RGB(Rnd * 256, Rnd * 256, Rnd * 256)
        Row = Row + 1
    Loop
End Sub
