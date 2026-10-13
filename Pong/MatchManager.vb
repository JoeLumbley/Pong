Imports System.Windows.Forms.AxHost
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Pong.Enums

Public Class MatchManager

    Private ReadOnly form As Form
    Private ReadOnly model As GameModel
    Private ReadOnly renderer As Rendering
    Private ReadOnly settings As SettingsManager
    Private ReadOnly state As GameStateManager
    Private ReadOnly audio As AudioController
    Private ReadOnly window As WindowManager
    Private ReadOnly physics As PhysicsEngine

    'Private winnerText As String = String.Empty


    ' -------------------------------
    '  Random
    ' -------------------------------
    Private rng As New Random()


    Public Sub New(form As Form,
                   model As GameModel,
                   renderer As Rendering,
                   settings As SettingsManager,
                   state As GameStateManager,
                   audio As AudioController,
                   window As WindowManager,
                   physics As PhysicsEngine)

        Me.form = form
        Me.model = model
        Me.renderer = renderer
        Me.settings = settings
        Me.state = state
        Me.audio = audio
        Me.window = window
        Me.physics = physics

    End Sub


    'Public Function GetWinnerText() As String
    '    Return winnerText
    'End Function

    'Public Sub SetWinnerText(text As String)
    '    winnerText = text
    'End Sub

    Public Sub StartNewMatch()
        audio.FadeOutAndStopStartLoop(600)
        audio.FadeOutAndStopPausedLoop(600)

        Window.MovePointerOffScreen()

        model.BallSpeed = 800 * (model.ClientSize.Height / 1080.0)


        model.ScoreLeft = 0
        model.ScoreRight = 0


        renderer.UpdateScore(model.ScoreLeft, model.ScoreRight)

        'model.ScoreLeft = 0
        'model.ScoreRight = 0
        'renderer.UpdateScore(0, 0, form.CreateGraphics(), form.ClientSize)

        If model.PlayerMode = 1 Then

            model.LeftPlayerName = "You"
            model.RightPlayerName = "CPU"
            renderer.UpdatePlayerNames(model.LeftPlayerName, model.RightPlayerName)

        Else

            model.LeftPlayerName = "Left"
            model.RightPlayerName = "Right"
            renderer.UpdatePlayerNames(model.LeftPlayerName, model.RightPlayerName)

        End If

        physics.ResetPaddles()
        physics.CenterBall()
        physics.ServeBall(If(rng.Next(0, 2) = 0, -1, 1))

        model.State = GameState.Playing

        physics.Start()

        audio.PlayGamePlayLoop(600)

    End Sub


    Public Sub EndMatch()

        audio.FadeOutAndStopGamePlayLoop(600)

        window.MovePointerCenterScreen()

        model.BallSpeed = 200 * (model.ClientSize.Height / 1080.0)

        physics.CenterBall()
        physics.MoveBallRandom()

        audio.PlayStartLoop(600)

    End Sub



End Class
