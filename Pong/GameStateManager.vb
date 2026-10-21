

Imports Pong.Enums

Public Class GameStateManager

    Private ReadOnly model As GameModel
    Private ReadOnly audio As AudioController
    Private ReadOnly physics As PhysicsEngine
    Private ReadOnly window As WindowManager

    Public Sub New(model As GameModel,
                   audio As AudioController,
                   physics As PhysicsEngine,
                   window As WindowManager)

        Me.model = model
        Me.audio = audio
        Me.physics = physics
        Me.window = window


        AddHandler physics.MatchWon, AddressOf EndMatch


    End Sub

#Region "Properties"

    Public ReadOnly Property CurrentState As GameState
        Get
            Return model.State
        End Get
    End Property

#End Region

#Region "State Transitions"

    Public Sub GoToStartScreen()

        ResetScores()

        model.WinnerText = String.Empty
        model.WinnerIs = 0

        PrepareMenuMode()

        SetState(GameState.StartScreen)

    End Sub

    Public Sub GoToAIDifficulty()

        PrepareMenuMode()

        SetState(GameState.AIDifficulty)

    End Sub

    Public Sub StartMatch()

        StopMenuAudio()

        window.MovePointerOffScreen()

        ResetScores()

        ConfigurePlayerNames()

        physics.PrepareNewMatch()

        SetState(GameState.Playing)

        physics.Start()

        audio.PlayGamePlayLoop(600)

    End Sub

    Public Sub PauseGame()

        If model.State <> GameState.Playing Then Return

        physics.Stop()

        audio.FadeOutAndStopGamePlayLoop(600)

        ClearMovementFlags()

        model.PauseSelection = PauseMenu.ResumeMatch

        window.MovePointerCenterScreen()

        SetState(GameState.Pause)

        audio.PlayPausedLoop(600)

    End Sub

    Public Sub ResumeGame()

        If model.State <> GameState.Pause Then Return

        audio.FadeOutAndStopPausedLoop(600)

        window.MovePointerOffScreen()

        SetState(GameState.Playing)

        physics.Start()

        audio.PlayGamePlayLoop(600)

    End Sub

    Public Sub EndMatch(winnerId As Integer,
                        winnerText As String)

        model.WinnerIs = winnerId
        model.WinnerText = winnerText

        physics.ResetForEnd()

        window.MovePointerCenterScreen()

        audio.FadeOutAndStopGamePlayLoop(600)

        SetState(GameState.EndScreen)

        audio.PlayStartLoop(600)

    End Sub

    Public Sub QuitToStartScreen()

        ResetScores()

        model.WinnerText = String.Empty
        model.WinnerIs = 0

        PrepareEnd()

        SetState(GameState.StartScreen)

        physics.Start()

    End Sub

#End Region

#Region "Helpers"

    Private Sub SetState(state As GameState)

        model.State = state

    End Sub

    Private Sub ResetScores()

        model.ScoreLeft = 0
        model.ScoreRight = 0

    End Sub

    Private Sub ConfigurePlayerNames()

        If model.PlayerMode = NumberOfPlayers.OnePlayer Then

            model.LeftPlayerName = "You"
            model.RightPlayerName = "CPU"

        Else

            model.LeftPlayerName = "Left"
            model.RightPlayerName = "Right"

        End If

    End Sub

    Private Sub PrepareMenuMode()

        physics.ResetForMenu()

        'window.MovePointerCenterScreen()

        StopGameplayAudio()

        audio.PlayStartLoop(600)

    End Sub


    Private Sub PrepareEnd()

        physics.ResetForEnd()

        'window.MovePointerCenterScreen()

        StopGameplayAudio()

        audio.PlayStartLoop(600)

    End Sub


    Private Sub StopGameplayAudio()

        audio.FadeOutAndStopGamePlayLoop(600)
        audio.FadeOutAndStopPausedLoop(600)

    End Sub

    Private Sub StopMenuAudio()

        audio.FadeOutAndStopStartLoop(600)
        audio.FadeOutAndStopPausedLoop(600)

    End Sub

    Private Sub ClearMovementFlags()

        model.Input.LeftUp = False
        model.Input.LeftDown = False

        model.Input.RightUp = False
        model.Input.RightDown = False

    End Sub

#End Region

End Class


