Option Explicit

Private Type TPoint
    X As Integer
    Y As Integer
End Type

Private Type TLine
    A As Integer
    B As Integer
End Type

Private Type TPolygon
    Points() As Integer
    Hole As Boolean
End Type

Private Function Clamp(ByVal Value As Integer, Min As Integer, Max As Integer) As Integer
    If Value > Max Then
        Clamp = Max
    ElseIf Value < Min Then
        Clamp = Min
    Else
        Clamp = Value
    End If
End Function

Private Function NormalizeAngle(ByVal Angle As Single) As Single
    Do While Angle < 0
        Angle = Angle + WorksheetFunction.Pi * 2
    Loop
    Do While Angle > WorksheetFunction.Pi * 2
        Angle = Angle - WorksheetFunction.Pi * 2
    Loop
    NormalizeAngle = Angle
End Function

Private Function VBAHelper_IsConvex(ByVal P1X As Single, ByVal P1Y As Single, ByVal P2X As Single, ByVal P2Y As Single, ByVal P3X As Single, ByVal P3Y As Single) As Boolean
    VBAHelper_IsConvex = (P3Y - P1Y) * (P2X - P1X) - (P3X - P1X) * (P2Y - P1Y) > 0
End Function

Private Function IsConvex(ByRef P1 As TPoint, ByRef P2 As TPoint, ByRef P3 As TPoint) As Boolean
    IsConvex = VBAHelper_IsConvex(P1.X, P1.Y, P2.X, P2.Y, P3.X, P3.Y)
End Function

Private Function IsInside(ByRef P1 As TPoint, ByRef P2 As TPoint, ByRef P3 As TPoint, ByRef Point As TPoint) As Boolean
    If IsConvex(P1, Point, P2) Then
        IsInside = False
    ElseIf IsConvex(P2, Point, P3) Then
        IsInside = False
    ElseIf IsConvex(P3, Point, P1) Then
        IsInside = False
    Else
        IsInside = True
    End If
End Function

Private Function InCone(ByRef P1 As TPoint, ByRef P2 As TPoint, ByRef P3 As TPoint, ByRef Point As TPoint) As Boolean
    If IsConvex(P1, P2, P3) Then
        If Not IsConvex(P1, P2, Point) Then
            InCone = False
        ElseIf Not IsConvex(P2, P3, Point) Then
            InCone = False
        Else
            InCone = True
        End If
    Else
        If IsConvex(P1, P2, Point) Then
            InCone = True
        ElseIf IsConvex(P2, P3, Point) Then
            InCone = True
        Else
            InCone = False
        End If
    End If
End Function

Private Function GetPolygonGroups(ByRef Points() As TPoint, ByRef Lines() As TLine, ByRef Polygons() As TPolygon) As Boolean
    Const MaxStack = 128
    Dim PolygonsCount As Integer
    Dim PointCardinality() As Integer
    Dim PointCount As Integer
    Dim LineCount As Integer
    Dim I As Integer
    Dim J As Integer
    Dim NotFound As Boolean
    Dim HasMoreIterations As Boolean

    ' Initialization.
    PolygonsCount = 0
    PointCount = UBound(Points) - LBound(Points) + 1
    LineCount = UBound(Lines) - LBound(Lines) + 1
    ReDim PointCardinality(PointCount - 1)
    ' Safety check.
    If PointCount < 3 Then
        GetPolygonGroups = False
        Exit Function
    End If

    ' Arrays of open points and lines.
    Dim AllPointsVisited As Boolean
    Dim OpenPoints() As Boolean
    Dim OpenLines() As Boolean
    ReDim OpenPoints(PointCount - 1)
    For I = 0 To PointCount - 1
        OpenPoints(I) = True
    Next
    ReDim OpenLines(LineCount - 1)
    For I = 0 To LineCount - 1
        OpenLines(I) = True
    Next
    ' Queue of points.
    Dim PointQueue() As Integer
    Dim PointQueueHead As Integer
    Dim PointQueueTail As Integer
    ReDim PointQueue(PointCount - 1)
    PointQueueHead = 0
    PointQueueTail = 0
    ' Stack of meshes.
    Dim MeshStackPoints() As Integer
    Dim MeshStackCount() As Integer
    Dim MeshStackTOS As Integer ' Top Of Stack.
    ReDim MeshStackPoints(MaxStack, PointCount - 1)
    ReDim MeshStackCount(MaxStack)
    ' General purpose point index variables.
    Dim CurrentPoint As Integer
    Dim FirstPoint As Integer
    Dim NextPoint As Integer
    Dim OtherPoint As Integer
    Dim PreviousPoint As Integer
    Dim SecondPoint As Integer
    Dim TestPoint As Integer
    ' General purpose line index variables.
    Dim NextLine As Integer
    ' General purpose distance variables.
    Dim NextDifference As Integer
    Dim TestDifference As Integer
    ' General purpose angle variables.
    Dim NextAngle As Single
    Dim PreviousAngle As Single
    Dim TestAngle As Single
    ' Outer perimeter variables.
    Dim OuterPerimeterPoints() As Integer
    Dim OuterPerimeterPointCount As Integer
    Dim OuterPerimeterPointStart As Integer
    ReDim OuterPerimeterPoints(PointCount - 1)
    ' Chord variables.
    Dim InnerChordPoints() As Integer
    Dim InnerChordPointCount As Integer
    Dim InnerChordPointLoop As Integer
    Dim OuterChordPoints() As Integer
    Dim OuterChordPointCount As Integer
    Dim ChordBegin As Integer
    Dim ChordEnd As Integer
    ReDim InnerChordPoints(PointCount - 1)
    ReDim OuterChordPoints(PointCount - 1)

    Do
        ' Find the left-most and bottom-most open point.
        FirstPoint = -1
        For I = 0 To PointCount - 1
            If OpenPoints(I) Then
                If FirstPoint < 0 Then
                    FirstPoint = I
                Else
                    If (Points(I).X < Points(FirstPoint).X) Or ((Points(I).X = Points(FirstPoint).X) And (Points(I).Y < Points(FirstPoint).Y)) Then
                        FirstPoint = I
                    End If
                End If
            End If
        Next
        PointQueue(0) = FirstPoint
        PointQueueTail = 1
        ' Walk through all lines from the first point.
        Do While PointQueueTail > PointQueueHead
            CurrentPoint = PointQueue(PointQueueTail - 1)
            If PointQueueTail = 1 Then
                ' Search the second point.
                NextPoint = -1
                For I = 0 To LineCount - 1
                    If (OpenLines(I)) And ((Lines(I).A = CurrentPoint) Or (Lines(I).B = CurrentPoint)) Then
                        If Lines(I).A = CurrentPoint Then
                            TestPoint = Lines(I).B
                        Else
                            TestPoint = Lines(I).A
                        End If
                        TestDifference = Points(TestPoint).Y - Points(CurrentPoint).Y
                        If NextPoint < 0 Then
                            NextPoint = TestPoint
                            NextDifference = TestDifference
                            NextLine = I
                        Else
                            If Points(TestPoint).Y < Points(NextPoint).Y Then
                                NextPoint = TestPoint
                                NextDifference = TestDifference
                                NextLine = I
                            End If
                        End If
                    End If
                Next
            Else
                PreviousPoint = PointQueue(PointQueueTail - 2)
                PreviousAngle = WorksheetFunction.Atan2(Points(CurrentPoint).X - Points(PreviousPoint).X, Points(CurrentPoint).Y - Points(PreviousPoint).Y)
                ' Search the next points.
                NextPoint = -1
                For I = 0 To LineCount - 1
                    If (OpenLines(I)) And ((Lines(I).A = CurrentPoint) Or (Lines(I).B = CurrentPoint)) Then
                        If Lines(I).A = CurrentPoint Then
                            TestPoint = Lines(I).B
                        Else
                            TestPoint = Lines(I).A
                        End If
                            If TestPoint <> PreviousPoint Then
                            TestAngle = WorksheetFunction.Atan2(Points(TestPoint).X - Points(CurrentPoint).X, Points(TestPoint).Y - Points(CurrentPoint).Y) - PreviousAngle
                            If TestAngle < -WorksheetFunction.Pi Then
                                TestAngle = TestAngle + WorksheetFunction.Pi * 2
                            End If
                            If NextPoint < 0 Then
                                NextPoint = TestPoint
                                NextAngle = TestAngle
                                NextLine = I
                            Else
                                If TestAngle < NextAngle Then
                                    NextPoint = TestPoint
                                    NextAngle = TestAngle
                                    NextLine = I
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            If NextPoint >= 0 Then
                NotFound = True
                OuterPerimeterPointStart = PointQueueHead
                Do While (NotFound) And (OuterPerimeterPointStart < PointQueueTail)
                    If PointQueue(OuterPerimeterPointStart) = NextPoint Then
                        NotFound = False
                    Else
                        OuterPerimeterPointStart = OuterPerimeterPointStart + 1
                    End If
                Loop
                If NotFound Then
                    PointQueue(PointQueueTail) = NextPoint
                    PointQueueTail = PointQueueTail + 1
                Else
                    OuterPerimeterPointCount = 0
                    For I = OuterPerimeterPointStart To PointQueueTail - 1
                        OuterPerimeterPoints(OuterPerimeterPointCount) = PointQueue(I)
                        OuterPerimeterPointCount = OuterPerimeterPointCount + 1
                    Next
                    ' Push the outer perimeter on the stack.
                    MeshStackTOS = 0
                    MeshStackCount(MeshStackTOS) = OuterPerimeterPointCount
                    For I = 0 To OuterPerimeterPointCount - 1
                        MeshStackPoints(MeshStackTOS, I) = OuterPerimeterPoints(I)
                    Next
                    Do While MeshStackTOS >= 0
                        ' Calculate the point cardinality.
                        For I = 0 To MeshStackCount(MeshStackTOS) - 1
                            PointCardinality(I) = 2 ' The default previous and next lines attached to each point.
                        Next
                        For I = 0 To MeshStackCount(MeshStackTOS) - 1
                            CurrentPoint = MeshStackPoints(MeshStackTOS, I)
                            NextPoint = MeshStackPoints(MeshStackTOS, (I + 1) Mod MeshStackCount(MeshStackTOS))
                            PreviousPoint = MeshStackPoints(MeshStackTOS, (I + MeshStackCount(MeshStackTOS) - 1) Mod MeshStackCount(MeshStackTOS))
                            For J = 0 To LineCount - 1
                                If ((Lines(J).A = CurrentPoint) And (Lines(J).B <> NextPoint) And (Lines(J).B <> PreviousPoint)) Or ((Lines(J).B = CurrentPoint) And (Lines(J).A <> NextPoint) And (Lines(J).A <> PreviousPoint)) Then
                                    If Lines(J).A = CurrentPoint Then
                                        TestPoint = Lines(J).B
                                    Else
                                        TestPoint = Lines(J).A
                                    End If
                                    If InCone(Points(PreviousPoint), Points(CurrentPoint), Points(NextPoint), Points(TestPoint)) Then
                                        PointCardinality(I) = PointCardinality(I) + 1
                                    End If
                                End If
                            Next
                        Next
                        ' Search the left-most and bottom-most point of cardinality greather than 2, if exists.
                        InnerChordPointLoop = -1
                        FirstPoint = -1
                        For I = 0 To MeshStackCount(MeshStackTOS) - 1
                            If PointCardinality(I) > 2 Then
                                TestPoint = MeshStackPoints(MeshStackTOS, I)
                                If FirstPoint < 0 Then
                                    FirstPoint = TestPoint
                                Else
                                    If (Points(TestPoint).X < Points(FirstPoint).X) Or ((Points(TestPoint).X = Points(FirstPoint).X) And (Points(TestPoint).Y < Points(FirstPoint).Y)) Then
                                        FirstPoint = TestPoint
                                    End If
                                End If
                            End If
                        Next
                        If FirstPoint >= 0 Then
                            InnerChordPoints(0) = FirstPoint
                            InnerChordPointCount = 1
                            HasMoreIterations = True
                            Do While HasMoreIterations
                                CurrentPoint = InnerChordPoints(InnerChordPointCount - 1)
                                If InnerChordPointCount = 1 Then
                                    NotFound = True
                                    I = 0
                                    Do While (NotFound) And (I < MeshStackCount(MeshStackTOS))
                                        If MeshStackPoints(MeshStackTOS, I) = FirstPoint Then
                                            NotFound = False
                                        Else
                                            I = I + 1
                                        End If
                                    Loop
                                    OtherPoint = MeshStackPoints(MeshStackTOS, (I + 1) Mod MeshStackCount(MeshStackTOS))
                                    PreviousPoint = MeshStackPoints(MeshStackTOS, (I + MeshStackCount(MeshStackTOS) - 1) Mod MeshStackCount(MeshStackTOS))
                                    PreviousAngle = WorksheetFunction.Atan2(Points(CurrentPoint).X - Points(PreviousPoint).X, Points(CurrentPoint).Y - Points(PreviousPoint).Y)
                                    NextAngle = WorksheetFunction.Atan2(Points(OtherPoint).X - Points(CurrentPoint).X, Points(OtherPoint).Y - Points(CurrentPoint).Y)
                                    NextPoint = -1
                                    For I = 0 To LineCount - 1
                                        If (OpenLines(I)) And ((Lines(I).A = CurrentPoint) Or (Lines(I).B = CurrentPoint)) Then
                                            If Lines(I).A = CurrentPoint Then
                                                TestPoint = Lines(I).B
                                            Else
                                                TestPoint = Lines(I).A
                                            End If
                                            If (TestPoint <> PreviousPoint) And (TestPoint <> OtherPoint) And (InCone(Points(PreviousPoint), Points(CurrentPoint), Points(OtherPoint), Points(TestPoint))) Then
                                                TestAngle = WorksheetFunction.Atan2(Points(TestPoint).X - Points(FirstPoint).X, Points(TestPoint).Y - Points(FirstPoint).Y) - PreviousAngle
                                                If TestAngle < -WorksheetFunction.Pi Then
                                                    TestAngle = TestAngle + WorksheetFunction.Pi * 2
                                                End If
                                                If NextPoint < 0 Then
                                                    NextPoint = TestPoint
                                                    NextAngle = TestAngle
                                                    NextLine = I
                                                Else
                                                    If TestAngle < NextAngle Then
                                                        NextPoint = TestPoint
                                                        NextAngle = TestAngle
                                                        NextLine = I
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Next
                                Else
                                    PreviousPoint = InnerChordPoints(InnerChordPointCount - 2)
                                    PreviousAngle = WorksheetFunction.Atan2(Points(CurrentPoint).X - Points(PreviousPoint).X, Points(CurrentPoint).Y - Points(PreviousPoint).Y)
                                    NextPoint = -1
                                    For I = 0 To LineCount - 1
                                        If (OpenLines(I)) And ((Lines(I).A = CurrentPoint) Or (Lines(I).B = CurrentPoint)) Then
                                            If Lines(I).A = CurrentPoint Then
                                                TestPoint = Lines(I).B
                                            Else
                                                TestPoint = Lines(I).A
                                            End If
                                            If TestPoint <> PreviousPoint Then
                                                TestAngle = WorksheetFunction.Atan2(Points(TestPoint).X - Points(FirstPoint).X, Points(TestPoint).Y - Points(FirstPoint).Y) - PreviousAngle
                                                If TestAngle < -WorksheetFunction.Pi Then
                                                    TestAngle = TestAngle + WorksheetFunction.Pi * 2
                                                End If
                                                If NextPoint < 0 Then
                                                    NextPoint = TestPoint
                                                    NextAngle = TestAngle
                                                    NextLine = I
                                                Else
                                                    If TestAngle < NextAngle Then
                                                        NextPoint = TestPoint
                                                        NextAngle = TestAngle
                                                        NextLine = I
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                                If NextPoint >= 0 Then
                                    OpenLines(NextLine) = False
                                    InnerChordPoints(InnerChordPointCount) = NextPoint
                                    InnerChordPointCount = InnerChordPointCount + 1
                                    NotFound = True
                                    I = 0
                                    Do While (NotFound) And (I < MeshStackCount(MeshStackTOS))
                                        If MeshStackPoints(MeshStackTOS, I) = NextPoint Then
                                            NotFound = False
                                        Else
                                            I = I + 1
                                        End If
                                    Loop
                                    If NotFound Then
                                        NotFound = True
                                        I = 1
                                        Do While (NotFound) And (I < InnerChordPointCount - 1)
                                            If InnerChordPoints(I) = NextPoint Then
                                                NotFound = False
                                            Else
                                                I = I + 1
                                            End If
                                        Loop
                                        If NotFound Then
                                            OpenPoints(NextPoint) = False
                                        Else
                                            InnerChordPointLoop = I
                                            HasMoreIterations = False
                                        End If
                                    Else
                                        HasMoreIterations = False
                                    End If
                                Else
                                    OpenLines(NextLine) = False
                                    AllPointsVisited = True
                                    For I = 0 To LineCount - 1
                                        If (OpenLines(I)) And ((Lines(I).A = CurrentPoint) Or (Lines(I).B = CurrentPoint)) Then
                                            AllPointsVisited = False
                                        End If
                                    Next
                                    If AllPointsVisited Then
                                        OpenPoints(CurrentPoint) = False
                                        InnerChordPointCount = InnerChordPointCount - 1
                                    End If
                                    If InnerChordPointCount = 0 Then
                                        HasMoreIterations = False
                                    End If
                                End If
                            Loop
                            If InnerChordPointCount > 0 Then
                                If InnerChordPointLoop > 0 Then
                                    Stop
                                Else
                                    ChordBegin = 0
                                    Do While MeshStackPoints(MeshStackTOS, ChordBegin) <> InnerChordPoints(0)
                                        ChordBegin = ChordBegin + 1
                                    Loop
                                    PreviousPoint = MeshStackPoints(MeshStackTOS, (ChordBegin + MeshStackCount(MeshStackTOS) - 1) Mod MeshStackCount(MeshStackTOS))
                                    ChordEnd = MeshStackCount(MeshStackTOS) - 1
                                    Do While MeshStackPoints(MeshStackTOS, ChordEnd) <> InnerChordPoints(InnerChordPointCount - 1)
                                        ChordEnd = ChordEnd - 1
                                    Loop
                                    NextPoint = MeshStackPoints(MeshStackTOS, (ChordEnd + 1) Mod MeshStackCount(MeshStackTOS))
                                    OuterChordPointCount = ChordEnd - ChordBegin + 1
                                    If OuterChordPointCount < 0 Then
                                        OuterChordPointCount = OuterChordPointCount + MeshStackCount(MeshStackTOS)
                                        For I = ChordBegin To ChordEnd + MeshStackCount(MeshStackTOS)
                                            OuterChordPoints(I - ChordBegin) = MeshStackPoints(MeshStackTOS, I Mod MeshStackCount(MeshStackTOS))
                                        Next
                                    Else
                                        For I = ChordBegin To ChordEnd
                                            OuterChordPoints(I - ChordBegin) = MeshStackPoints(MeshStackTOS, I)
                                        Next
                                    End If
                                    If InnerChordPointCount > OuterChordPointCount Then
                                        MeshStackCount(MeshStackTOS) = MeshStackCount(MeshStackTOS) - OuterChordPointCount + InnerChordPointCount
                                        For I = MeshStackCount(MeshStackTOS) - 1 To ChordEnd Step -1
                                            MeshStackPoints(MeshStackTOS, I + InnerChordPointCount - OuterChordPointCount) = MeshStackPoints(MeshStackTOS, I)
                                        Next
                                    ElseIf InnerChordPointCount < OuterChordPointCount Then
                                        MeshStackCount(MeshStackTOS) = MeshStackCount(MeshStackTOS) - OuterChordPointCount + InnerChordPointCount
                                        If ChordBegin > ChordEnd Then
                                            For I = ChordEnd To ChordBegin
                                                 MeshStackPoints(MeshStackTOS, I - ChordEnd) = MeshStackPoints(MeshStackTOS, I)
                                            Next
                                        Else
                                            For I = ChordBegin + 1 To MeshStackCount(MeshStackTOS) - 1
                                                MeshStackPoints(MeshStackTOS, I) = MeshStackPoints(MeshStackTOS, I + OuterChordPointCount - InnerChordPointCount)
                                            Next
                                        End If
                                    End If
                                    For I = 1 To InnerChordPointCount - 2
                                        MeshStackPoints(MeshStackTOS, (ChordBegin + I) Mod MeshStackCount(MeshStackTOS)) = InnerChordPoints(I)
                                    Next
                                    MeshStackTOS = MeshStackTOS + 1
                                    MeshStackCount(MeshStackTOS) = InnerChordPointCount + OuterChordPointCount - 2
                                    TestAngle = WorksheetFunction.Atan2(Points(NextPoint).X - Points(InnerChordPoints(InnerChordPointCount - 1)).X, Points(NextPoint).Y - Points(InnerChordPoints(InnerChordPointCount - 1)).Y)
                                    If TestAngle < 0 Then
                                        For I = 0 To OuterChordPointCount - 1
                                            MeshStackPoints(MeshStackTOS, I) = OuterChordPoints(I)
                                        Next
                                        For I = 1 To InnerChordPointCount - 2
                                            MeshStackPoints(MeshStackTOS, OuterChordPointCount + I - 1) = InnerChordPoints(InnerChordPointCount - I - 1)
                                        Next
                                    Else
                                        For I = 0 To InnerChordPointCount - 1
                                            MeshStackPoints(MeshStackTOS, I) = InnerChordPoints(InnerChordPointCount - I - 1)
                                        Next
                                        For I = 1 To OuterChordPointCount - 2
                                            MeshStackPoints(MeshStackTOS, InnerChordPointCount + I - 1) = OuterChordPoints(I)
                                        Next
                                    End If
                                End If
                            Else
                                Stop
                            End If
                        Else
                            ReDim Preserve Polygons(PolygonsCount)
                            ReDim Polygons(PolygonsCount).Points(MeshStackCount(MeshStackTOS) - 1)
                            For I = 0 To MeshStackCount(MeshStackTOS) - 1
                                Polygons(PolygonsCount).Points(I) = MeshStackPoints(MeshStackTOS, I)
                            Next
                            PolygonsCount = PolygonsCount + 1
                            MeshStackTOS = MeshStackTOS - 1
                        End If
                    Loop
                    For J = OuterPerimeterPointStart To PointQueueTail
                        For I = 0 To LineCount - 1
                            If ((Lines(I).A = OuterPerimeterPoints((J + OuterPerimeterPointCount - 1) Mod OuterPerimeterPointCount)) And (Lines(I).B = OuterPerimeterPoints(J Mod OuterPerimeterPointCount))) Or ((Lines(I).A = OuterPerimeterPoints(J Mod OuterPerimeterPointCount)) And (Lines(I).B = OuterPerimeterPoints((J + OuterPerimeterPointCount - 1) Mod OuterPerimeterPointCount))) Then
                                OpenLines(I) = False
                            End If
                        Next
                    Next
                    For J = 0 To OuterPerimeterPointCount - 1
                        AllPointsVisited = True
                        For I = 0 To LineCount - 1
                            If (OpenLines(I)) And ((Lines(I).A = OuterPerimeterPoints(J)) Or (Lines(I).B = OuterPerimeterPoints(J))) Then
                                AllPointsVisited = False
                            End If
                        Next
                        If AllPointsVisited Then
                            OpenPoints(OuterPerimeterPoints(J)) = False
                        End If
                    Next
                    PointQueueTail = OuterPerimeterPointStart
                End If
            Else
                OpenLines(NextLine) = False
                AllPointsVisited = True
                For I = 0 To LineCount - 1
                    If (OpenLines(I)) And ((Lines(I).A = CurrentPoint) Or (Lines(I).B = CurrentPoint)) Then
                        AllPointsVisited = False
                    End If
                Next
                If AllPointsVisited Then
                    OpenPoints(CurrentPoint) = False
                    PointQueueTail = PointQueueTail - 1
                End If
            End If
        Loop
        ' Check if all points are closed.
        AllPointsVisited = True
        For I = 0 To PointCount - 1
            If OpenPoints(I) Then
                AllPointsVisited = False
                Exit For
            End If
        Next
    Loop Until AllPointsVisited





    GetPolygonGroups = True
End Function

Public Sub Process()
    Dim Destination As Worksheet
    Dim I As Integer
    Dim LineCount As Integer
    Dim Lines() As TLine
    Dim PointCount As Integer
    Dim PointIndex() As Integer
    Dim Points() As TPoint
    Dim Polygons() As TPolygon
    Dim Row As Integer
    Dim Source As Worksheet
    Dim Text As String
    Set Source = ThisWorkbook.Sheets.Item("Input")
    Set Destination = ThisWorkbook.Sheets.Item("Output")
    PointCount = 0
    Row = 2
    Do While Source.Cells(Row, 2).Value <> ""
        ReDim Preserve Points(PointCount)
        ReDim Preserve PointIndex(Source.Cells(Row, 1).Value)
        Points(PointCount).X = Source.Cells(Row, 2).Value
        Points(PointCount).Y = Source.Cells(Row, 3).Value
        PointIndex(Source.Cells(Row, 1).Value) = PointCount
        PointCount = PointCount + 1
        Row = Row + 1
        'If Row = 20 Then Exit Do
    Loop
    LineCount = 0
    Row = 2
    Do While Source.Cells(Row, 6).Value <> ""
        ReDim Preserve Lines(LineCount)
        Lines(LineCount).A = PointIndex(Source.Cells(Row, 6).Value)
        Lines(LineCount).B = PointIndex(Source.Cells(Row, 7).Value)
        LineCount = LineCount + 1
        Row = Row + 1
        'If Row = 32 Then Exit Do
    Loop
    Destination.Cells.Clear
    Destination.Cells(1, 1).Value = "Polygon"
    Destination.Cells(1, 2).Value = "Points"
    Destination.Cells(1, 3).Value = "Coordinates"
    If GetPolygonGroups(Points, Lines, Polygons) Then
        For Row = LBound(Polygons) To UBound(Polygons)
            Destination.Cells(2 + Row, 1).Value = Row + 1
            Text = ""
            For I = LBound(Polygons(Row).Points) To UBound(Polygons(Row).Points)
                If Text <> "" Then
                    Text = Text & ", "
                End If
                Text = Text & Polygons(Row).Points(I)
            Next
            Destination.Cells(2 + Row, 2).Value = Text
            Text = ""
            For I = LBound(Polygons(Row).Points) To UBound(Polygons(Row).Points)
                If Text <> "" Then
                    Text = Text & ", "
                End If
                Text = Text & "(" & Points(Polygons(Row).Points(I)).X & "," & Points(Polygons(Row).Points(I)).Y & ")"
            Next
            Destination.Cells(2 + Row, 3).Value = Text
        Next
    End If
End Sub
