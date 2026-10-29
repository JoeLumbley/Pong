''Public Class BallTrailRenderer

''    ' ============================================================
''    '   TRAIL SYSTEM
''    ' ============================================================
''    Private Const TrailLength As Integer = 20
''    Private trailPoints(TrailLength - 1) As PointF
''    Private trailSizes(TrailLength - 1) As Integer
''    Private trailOffsets(TrailLength - 1) As Single
''    Private trailAlpha(TrailLength - 1) As Integer
''    Private trailBrushes(TrailLength - 1) As SolidBrush
''    Private trailIndex As Integer = -1
''    Private trailCount As Integer = 0


''    Public Sub InitTrails(model As GameModel)
''        For i As Integer = 0 To TrailLength - 1
''            Dim size As Integer = model.BallDiameter - (TrailLength - i) * 2
''            If size < 10 Then size = 10

''            trailSizes(i) = size
''            trailOffsets(i) = CSng((model.BallDiameter - size) / 2.0F)

''            Dim t As Double = i / CDbl(TrailLength)
''            Dim alpha As Integer = CInt(16 * t * t)
''            trailAlpha(i) = alpha

''            trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
''        Next
''    End Sub


''    Public Sub UpdateTrail(model As GameModel)
''        If trailPoints Is Nothing Then Exit Sub
''        If model Is Nothing Then Return

''        trailIndex = (trailIndex + 1) Mod TrailLength
''        trailPoints(trailIndex) = model.BallPos

''        If trailCount < TrailLength Then
''            trailCount += 1
''        End If

''        If trailCount = 1 Then
''            trailSizes(0) = model.BallDiameter
''            trailOffsets(0) = 0
''            trailAlpha(0) = 16
''            Exit Sub
''        End If

''        For i As Integer = 0 To trailCount - 1
''            Dim t As Double = 1.0 - (i / (trailCount - 1))
''            trailSizes(i) = CInt(model.BallDiameter * (0.5 + 0.5 * t))
''            trailOffsets(i) = CSng(model.BallDiameter * (0.25 * (1 - t)))
''            trailAlpha(i) = CInt(16 * (0.2 + 0.8 * t))
''        Next
''    End Sub



''    Private Sub DrawTrail(g As Graphics)
''        If trailCount <= 0 Then Exit Sub

''        For i As Integer = 0 To trailCount - 1
''            Dim index As Integer = (trailIndex - i + TrailLength) Mod TrailLength
''            Dim p As PointF = trailPoints(index)
''            Dim offset As Single = trailOffsets(i)
''            Dim alpha As Integer = trailAlpha(i)

''            If alpha <= 0 Then Continue For

''            Dim baseColor As Color = Color.FromArgb(alpha, 0, 191, 255)
''            trailBrushes(i).Color = baseColor

''            g.FillEllipse(trailBrushes(i),
''                          p.X + offset,
''                          p.Y + offset,
''                          trailSizes(i),
''                          trailSizes(i))
''        Next
''    End Sub









''    Public Sub ClearTrail()
''        trailCount = 0
''        trailIndex = -1
''        For i As Integer = 0 To TrailLength - 1
''            trailSizes(i) = 0
''            trailOffsets(i) = 0
''            trailAlpha(i) = 0
''        Next
''    End Sub




''End Class
'Public Class BallTrailRenderer

'    ' ============================================================
'    '   TRAIL SYSTEM
'    ' ============================================================
'    Private Const TrailLength As Integer = 20
'    Private trailPoints(TrailLength - 1) As PointF
'    Private trailSizes(TrailLength - 1) As Integer
'    Private trailOffsets(TrailLength - 1) As Single
'    Private trailAlpha(TrailLength - 1) As Integer
'    Private trailBrushes(TrailLength - 1) As SolidBrush
'    Private trailIndex As Integer = -1
'    Private trailCount As Integer = 0


'    Public Sub Update(model As GameModel)

'        If model Is Nothing Then Return

'        trailIndex = (trailIndex + 1) Mod TrailLength
'        trailPoints(trailIndex) = model.BallPos

'        If trailCount < TrailLength Then
'            trailCount += 1
'        End If

'        If trailCount = 1 Then

'            trailSizes(0) = model.BallDiameter
'            trailOffsets(0) = 0
'            trailAlpha(0) = 16

'            Return

'        End If

'        For i As Integer = 0 To trailCount - 1

'            Dim t As Double =
'                1.0 - (i / (trailCount - 1))

'            trailSizes(i) =
'                CInt(model.BallDiameter * (0.5 + 0.5 * t))

'            trailOffsets(i) =
'                CSng(model.BallDiameter * (0.25 * (1 - t)))

'            trailAlpha(i) =
'                CInt(16 * (0.2 + 0.8 * t))

'        Next

'    End Sub

'    Public Sub Draw(g As Graphics)

'        If trailCount <= 0 Then Return

'        For i As Integer = 0 To trailCount - 1

'            Dim index As Integer =
'                (trailIndex - i + TrailLength) Mod TrailLength

'            Dim p As PointF = trailPoints(index)

'            Dim offset As Single = trailOffsets(i)

'            Dim alpha As Integer = trailAlpha(i)

'            If alpha <= 0 Then Continue For

'            trailBrushes(i).Color =
'                Color.FromArgb(alpha, 0, 191, 255)

'            g.FillEllipse(
'                trailBrushes(i),
'                p.X + offset,
'                p.Y + offset,
'                trailSizes(i),
'                trailSizes(i))

'        Next

'    End Sub

'    Public Sub Reset()

'        trailCount = 0
'        trailIndex = -1

'        For i As Integer = 0 To TrailLength - 1

'            trailSizes(i) = 0
'            trailOffsets(i) = 0
'            trailAlpha(i) = 0

'        Next

'    End Sub

'End Class



Imports System.Drawing

Public Class BallTrailRenderer
    Implements IDisposable

    Private Const TrailLength As Integer = 6

    Private ReadOnly trailPoints(TrailLength - 1) As PointF
    Private ReadOnly trailSizes(TrailLength - 1) As Integer
    Private ReadOnly trailOffsets(TrailLength - 1) As Single
    Private ReadOnly trailAlpha(TrailLength - 1) As Integer

    Private ReadOnly trailBrushes(TrailLength - 1) As SolidBrush

    Private trailIndex As Integer = -1
    Private trailCount As Integer = 0

    Public Sub New()

        For i As Integer = 0 To TrailLength - 1

            trailBrushes(i) =
                New SolidBrush(Color.Transparent)

            trailSizes(i) = 10

        Next

    End Sub

    Public Sub InitTrails(model As GameModel)

        If model Is Nothing Then Return

        For i As Integer = 0 To TrailLength - 1

            Dim size As Integer =
                model.BallDiameter -
                (TrailLength - i) * 2

            If size < 10 Then
                size = 10
            End If

            trailSizes(i) = size

            trailOffsets(i) =
                CSng((model.BallDiameter - size) / 2.0F)

            Dim t As Double =
                i / CDbl(TrailLength)

            Dim alpha As Integer =
                CInt(16 * t * t)

            trailAlpha(i) = alpha

            trailBrushes(i).Color =
                Color.FromArgb(
                    alpha,
                    0,
                    191,
                    255)

        Next

    End Sub


    Public Sub Update(model As GameModel)

        If model Is Nothing Then Return

        trailIndex = (trailIndex + 1) Mod TrailLength
        trailPoints(trailIndex) = model.BallPos

        If trailCount < TrailLength Then
            trailCount += 1
        End If

        If trailCount = 1 Then

            trailSizes(0) = model.BallDiameter
            trailOffsets(0) = 0
            'trailAlpha(0) = 16
            trailAlpha(0) = 6

            Return

        End If

        For i As Integer = 0 To trailCount - 1

            Dim t As Double =
                1.0 - (i / (trailCount - 1))

            trailSizes(i) =
                CInt(model.BallDiameter * (0.5 + 0.5 * t))

            trailOffsets(i) =
                CSng(model.BallDiameter * (0.25 * (1 - t)))

            'trailAlpha(i) =
            '    CInt(16 * (0.2 + 0.8 * t))
            trailAlpha(i) =
                CInt(6 * (0.2 + 0.8 * t))

        Next

    End Sub

    Public Sub Draw(g As Graphics)

        If trailCount <= 0 Then Return

        For i As Integer = 0 To trailCount - 1

            Dim index As Integer =
                (trailIndex - i + TrailLength) Mod TrailLength

            Dim p As PointF = trailPoints(index)

            Dim offset As Single = trailOffsets(i)

            Dim alpha As Integer = trailAlpha(i)

            If alpha <= 0 Then Continue For

            trailBrushes(i).Color =
                Color.FromArgb(alpha, 0, 191, 255)

            g.FillEllipse(
                trailBrushes(i),
                p.X + offset,
                p.Y + offset,
                trailSizes(i),
                trailSizes(i))

        Next

    End Sub

    Public Sub Reset()

        trailCount = 0
        trailIndex = -1

        For i As Integer = 0 To TrailLength - 1

            trailSizes(i) = 0
            trailOffsets(i) = 0
            trailAlpha(i) = 0

        Next

    End Sub




















    Public Sub Dispose() Implements IDisposable.Dispose

        For i As Integer = 0 To TrailLength - 1

            trailBrushes(i)?.Dispose()

        Next

    End Sub

End Class



