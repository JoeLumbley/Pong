Imports System.Drawing

Public Class PaddleLayout

    Public Sub Initialize(size As Size,
                          model As GameModel)

        Dim paddleHeight As Integer =
            size.Height \ 8

        Dim paddleWidth As Integer =
            size.Height \ 30

        Dim paddleX As Integer =
            CInt(size.Height / 25.0F)

        Dim paddleY As Integer =
            (size.Height - paddleHeight) \ 2

        model.PaddleHeight = paddleHeight
        model.PaddleWidth = paddleWidth

        model.LeftPaddleRect =
            New Rectangle(
                paddleX,
                paddleY,
                paddleWidth,
                paddleHeight)

        model.RightPaddleRect =
            New Rectangle(
                size.Width - paddleX - paddleWidth,
                paddleY,
                paddleWidth,
                paddleHeight)

    End Sub

    Public Sub Resize(size As Size,
                      model As GameModel)

        Initialize(size, model)

    End Sub

End Class