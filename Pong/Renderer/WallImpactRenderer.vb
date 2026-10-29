


'Imports System.Drawing.Drawing2D

'Public Class WallImpactRenderer

'    'Private Const OuterRadius As Single = 50.0F
'    'Private Const InnerRadius As Single = 30.0F
'    Private Const OuterRadius As Single = 5.0F
'    Private Const InnerRadius As Single = 3.0F

'    Public Sub Draw(
'        g As Graphics,
'        effect As WallImpactEffect)

'        If effect Is Nothing Then Return
'        If Not effect.IsActive Then Return

'        Dim alpha As Integer =
'            CInt(255 * effect.Alpha)

'        If alpha <= 0 Then Return

'        Dim progress As Single = effect.Progress

'        DrawRing(
'            g,
'            effect.WallImpactPt,
'            InnerRadius * progress,
'            CInt(alpha * 0.8F),
'            4.0F)

'        DrawRing(
'            g,
'            effect.WallImpactPt,
'            OuterRadius * progress,
'            alpha,
'            3.0F)

'    End Sub

'    Private Sub DrawRing(
'        g As Graphics,
'        center As PointF,
'        radius As Single,
'        alpha As Integer,
'        thickness As Single)

'        If alpha <= 0 Then Return
'        If radius <= 0 Then Return

'        Using pen As New Pen(
'            Color.FromArgb(alpha, Color.White),
'            thickness)

'            pen.StartCap = LineCap.Round
'            pen.EndCap = LineCap.Round

'            g.DrawEllipse(
'                pen,
'                center.X - radius,
'                center.Y - radius,
'                radius * 2,
'                radius * 2)

'        End Using

'    End Sub

'End Class


Imports System.Drawing.Drawing2D

Public Class WallImpactRenderer

    Public Property OuterRadius As Single = 5.0F
    Public Property InnerRadius As Single = 3.0F

    Public Sub Draw(
        g As Graphics,
        effect As WallImpactEffect)

        If effect Is Nothing Then Return
        If Not effect.IsActive Then Return

        Dim alpha As Integer =
            CInt(255 * effect.Alpha)

        If alpha <= 0 Then Return

        Dim progress As Single = effect.Progress

        DrawRing(
            g,
            effect.WallImpactPt,
            InnerRadius * progress,
            CInt(alpha * 0.8F),
            4.0F)

        DrawRing(
            g,
            effect.WallImpactPt,
            OuterRadius * progress,
            alpha,
            3.0F)

    End Sub

    Private Sub DrawRing(
        g As Graphics,
        center As PointF,
        radius As Single,
        alpha As Integer,
        thickness As Single)

        If alpha <= 0 Then Return
        If radius <= 0 Then Return

        Using pen As New Pen(
            Color.FromArgb(alpha, Color.White),
            thickness)

            pen.StartCap = LineCap.Round
            pen.EndCap = LineCap.Round

            g.DrawEllipse(
                pen,
                center.X - radius,
                center.Y - radius,
                radius * 2,
                radius * 2)

        End Using

    End Sub

End Class