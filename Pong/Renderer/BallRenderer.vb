
Imports System.Drawing.Drawing2D

Public Class BallRenderer

    Private ReadOnly resources As GraphicsResources

    Public Sub New(resources As GraphicsResources)

        Me.resources = resources

    End Sub

    Public Sub Draw(
        g As Graphics,
        model As GameModel)

        ' Ball
        g.FillEllipse(
            resources.BallBrush,
            model.BallPos.X,
            model.BallPos.Y,
            model.BallDiameter,
            model.BallDiameter)

        ' Shadow
        Using shadowPath As New GraphicsPath()

            shadowPath.AddEllipse(
                model.BallPos.X + model.BallDiameter * 0.1F,
                model.BallPos.Y + model.BallDiameter * 0.1F,
                model.BallDiameter,
                model.BallDiameter)

            Using shadowBrush As New PathGradientBrush(shadowPath)

                shadowBrush.CenterColor = Color.FromArgb(173, Color.Black)
                shadowBrush.SurroundColors = {Color.FromArgb(0, Color.Black)}

                g.FillPath(shadowBrush, shadowPath)

            End Using

        End Using

        ' Highlight
        Dim highlightSize As Single = model.BallDiameter * 0.24F
        Dim highlightX As Single = model.BallPos.X + model.BallDiameter * 0.2F
        Dim highlightY As Single = model.BallPos.Y + model.BallDiameter * 0.16F

        Using highlightBrush As New SolidBrush(
            Color.FromArgb(64, Color.White))

            g.FillEllipse(
                highlightBrush,
                highlightX,
                highlightY,
                highlightSize,
                highlightSize)

        End Using

    End Sub

End Class