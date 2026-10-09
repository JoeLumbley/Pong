Imports System.Windows.Forms.AxHost
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Pong.Enums

Public Class MatchManager

    Private winnerText As String = String.Empty

    Public Function GetWinnerText() As String
        Return winnerText
    End Function

    Public Sub SetWinnerText(text As String)
        winnerText = text
    End Sub

    Public Sub StartNewMatch(state As GameStateManager, audio As AudioController, window As WindowManager)
        audio.FadeOutAndStopStartLoop(600)
        audio.FadeOutAndStopPausedLoop(600)

        window.MovePointerOffScreen()

        'speed = 800 * (ClientSize.Height / 1080.0)

        'scoreLeft = 0
        'scoreRight = 0
        'renderer.UpdateScore(scoreLeft, scoreRight, Me.CreateGraphics(), ClientSize)

        'If settings.GetPlayerMode() = 1 Then
        '    leftPlayerName = "You"
        '    rightPlayerName = "CPU"
        '    renderer.UpdatePlayerNames(leftPlayerName, rightPlayerName, Me.CreateGraphics(), ClientSize)
        'Else
        '    leftPlayerName = "Left"
        '    rightPlayerName = "Right"
        '    renderer.UpdatePlayerNames(leftPlayerName, rightPlayerName, Me.CreateGraphics(), ClientSize)
        'End If

        'ResetPaddles()
        'CenterBall()
        'ServeBall(If(rng.Next(0, 2) = 0, -1, 1))

        state.SetCurrentState(GameState.Playing)

        'physicsTimer.Start()

        audio.PlayGamePlayLoop(600)

    End Sub
    'Public Sub EndMatch(audio As AudioController, window As WindowManager, )

    '    audio.FadeOutAndStopGamePlayLoop(600)

    '    window.MovePointerCenterScreen()

    '    speed = 200 * (ClientSize.Height / 1080.0)

    '    CenterBall()
    '    MoveBallRandom()

    '    audio.PlayStartLoop(600)

    'End Sub



End Class
