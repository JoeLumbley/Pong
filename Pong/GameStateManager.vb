'Imports Pong.Enums

'Public Class GameStateManager



'End Class



Imports Pong.Enums

Public Class GameStateManager

    Private ReadOnly model As GameModel
    Private ReadOnly audio As AudioController
    Private ReadOnly physics As PhysicsEngine
    Private ReadOnly match As MatchManager
    Private ReadOnly window As WindowManager

    Public Sub New(model As GameModel,
                   audio As AudioController,
                   physics As PhysicsEngine,
                   match As MatchManager,
                   window As WindowManager)

        Me.model = model
        Me.audio = audio
        Me.physics = physics
        Me.match = match
        Me.window = window
    End Sub

    ' ============================================================
    ' STATE TRANSITIONS
    ' ============================================================

    Public Sub GoToStartScreen()
        model.State = GameState.StartScreen

        model.ScoreLeft = 0
        model.ScoreRight = 0
        model.WinnerText = ""
        model.WinnerIs = 0

        physics.ResetForMenu()
        window.MovePointerCenterScreen()

        audio.FadeOutAndStopGamePlayLoop(600)
        audio.FadeOutAndStopPausedLoop(600)
        audio.PlayStartLoop(600)
    End Sub

    Public Sub GoToAIDifficulty()
        model.State = GameState.AIDifficulty

        physics.ResetForMenu()
        window.MovePointerCenterScreen()

        audio.FadeOutAndStopGamePlayLoop(600)
        audio.FadeOutAndStopPausedLoop(600)
        audio.PlayStartLoop(600)
    End Sub

    Public Sub StartMatch()
        audio.FadeOutAndStopStartLoop(600)
        audio.FadeOutAndStopPausedLoop(600)

        window.MovePointerOffScreen()

        model.ScoreLeft = 0
        model.ScoreRight = 0

        If model.PlayerMode = NumberOfPlayers.OnePlayer Then
            model.LeftPlayerName = "You"
            model.RightPlayerName = "CPU"
        Else
            model.LeftPlayerName = "Left"
            model.RightPlayerName = "Right"
        End If

        physics.PrepareNewMatch()

        model.State = GameState.Playing
        physics.Start()
        audio.PlayGamePlayLoop(600)
    End Sub

    Public Sub PauseGame()
        If model.State <> GameState.Playing Then Return

        physics.StopTimer()
        audio.FadeOutAndStopGamePlayLoop(600)

        model.LeftPaddleUp = False
        model.LeftPaddleDown = False
        model.RightPaddleUp = False
        model.RightPaddleDown = False

        model.PauseSelection = PauseMenu.ResumeMatch

        window.MovePointerCenterScreen()

        model.State = GameState.Pause
        audio.PlayPausedLoop(600)
    End Sub

    Public Sub ResumeGame()
        If model.State <> GameState.Pause Then Return

        audio.FadeOutAndStopPausedLoop(600)
        window.MovePointerOffScreen()

        model.State = GameState.Playing
        physics.Start()
        audio.PlayGamePlayLoop(600)
    End Sub

    Public Sub EndMatch()
        model.State = GameState.EndScreen

        physics.ResetForMenu()
        window.MovePointerCenterScreen()

        audio.FadeOutAndStopGamePlayLoop(600)
        audio.PlayStartLoop(600)
    End Sub

    Public Sub QuitToStartScreen()
        audio.FadeOutAndStopPausedLoop(600)

        model.ScoreLeft = 0
        model.ScoreRight = 0
        model.WinnerText = ""

        physics.ResetForMenu()

        model.State = GameState.StartScreen
        physics.Start()
        audio.PlayStartLoop(600)
    End Sub

    Public Sub ToggleFullScreen()
        window.ToggleFullScreen()
        model.IsFullscreen = Not model.IsFullscreen
    End Sub

End Class
