Imports System.Drawing
Imports Pong.Enums

Public Class Layout

    Public Sub InitializePaddles(size As Size, model As GameModel)
        Dim paddleHeight As Integer = size.Height \ 8
        Dim paddleWidth As Integer = size.Width \ 50

        model.PaddleHeight = paddleHeight
        model.PaddleWidth = paddleWidth
        model.LeftPaddleRect = New Rectangle(
            size.Width \ 50,
            (size.Height - paddleHeight) \ 2,
            paddleWidth,
            paddleHeight)
        model.RightPaddleRect = New Rectangle(
            size.Width - size.Width \ 50 - paddleWidth,
            (size.Height - paddleHeight) \ 2,
            paddleWidth,
            paddleHeight)
    End Sub

    Public Sub Resize(size As Size, model As GameModel)
        model.ClientSize = size
        model.BallDiameter = CInt(size.Height / 18.0F)
        model.PaddleSpeed = CInt(700 * Math.Sqrt(size.Height / 1080.0))

        If model.State = GameState.StartScreen OrElse
           model.State = GameState.EndScreen OrElse
           model.State = GameState.AIDifficulty Then
            model.BallSpeed = 200 * (size.Height / 1080.0)
        Else
            model.BallSpeed = 800 * (size.Height / 1080.0)
        End If

        RescalePaddles(size, model)
        model.AIDifficultyScale = size.Height / 1080.0
    End Sub

    Public Sub RescalePaddles(size As Size, model As GameModel)
        Dim paddleHeight As Integer = size.Height \ 8
        Dim paddleWidth As Integer = size.Height \ 30
        Dim paddleX As Integer = CInt(size.Height / 25.0F)
        Dim paddleY As Integer = (size.Height - paddleHeight) \ 2

        model.PaddleHeight = paddleHeight
        model.PaddleWidth = paddleWidth
        model.LeftPaddleRect = New Rectangle(paddleX, paddleY, paddleWidth, paddleHeight)
        model.RightPaddleRect = New Rectangle(
            size.Width - paddleX - paddleWidth,
            paddleY,
            paddleWidth,
            paddleHeight)
    End Sub

    Public Sub ScaleBall(model As GameModel)
        model.BallDiameter = CInt(model.ClientSize.Height / 18.0F)
    End Sub

    Public Sub ScaleMovement(model As GameModel)
        model.PaddleSpeed = CInt(700 * Math.Sqrt(model.ClientSize.Height / 1080.0))

        If model.State = GameState.StartScreen OrElse
           model.State = GameState.EndScreen OrElse
           model.State = GameState.AIDifficulty Then
            model.BallSpeed = 200 * (model.ClientSize.Height / 1080.0)
        Else
            model.BallSpeed = 800 * (model.ClientSize.Height / 1080.0)
        End If

        model.AIDifficultyScale = model.ClientSize.Height / 2000.0
    End Sub

End Class
