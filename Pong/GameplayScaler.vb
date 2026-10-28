Imports System.Drawing
Imports Pong.Enums

Public Class GameplayScaler

    Public Const ReferenceHeight As Integer = 1080

    Public Sub Scale(model As GameModel)

        ScaleBall(model)
        ScaleMovement(model)

    End Sub

    Public Sub ScaleBall(model As GameModel)

        model.BallDiameter =
            CInt(model.ClientSize.Height / 18.0F)

    End Sub

    Public Sub ScaleMovement(model As GameModel)

        Dim scale As Double =
            model.ClientSize.Height / CDbl(ReferenceHeight)

        model.PaddleSpeed =
            CInt(700 * Math.Sqrt(scale))

        Select Case model.State

            Case GameState.StartScreen,
                 GameState.EndScreen,
                 GameState.AIDifficulty

                model.BallSpeed = 75 * scale

            Case Else

                model.BallSpeed = 800 * scale

        End Select

        model.AIDifficultyScale = scale

    End Sub

End Class