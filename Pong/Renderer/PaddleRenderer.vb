Public Class PaddleRenderer

    Private ReadOnly resources As GraphicsResources

    Public Sub New(resources As GraphicsResources)
        Me.resources = resources
    End Sub

    Public Sub Draw(g As Graphics, model As GameModel)

        g.FillRectangle(
            resources.PaddleBrush,
            model.LeftPaddleRect)

        g.FillRectangle(
            resources.PaddleBrush,
            model.RightPaddleRect)

    End Sub

End Class
