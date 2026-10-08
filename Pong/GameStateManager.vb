Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Pong.Enums

Public Class GameStateManager




    Private currentState As GameState = GameState.StartScreen


    Public Function GetCurrentState() As GameState
        Return currentState
    End Function

    Public Sub SetCurrentState(state As GameState)
        currentState = state

        'Select Case state
        '    Case GameState.StartScreen
        '        GoToStartScreen()
        '    Case GameState.AIDifficulty
        '        GoToAIDifficulty()

        'End Select
    End Sub


    'Public Sub GoToStartScreen(match As MatchManager)
    '    currentState = GameState.StartScreen
    '    match.SetWinnerText(String.Empty)
    '    'renderer.SetWinnerText("")
    '    Form1.PlayStartLoop(600)
    '    'Form1.ResetForMenu()
    '    Form1.CenterBall()
    '    Form1.MoveBallRandom()
    '    Form1.MovePointerCenterScreen()
    'End Sub

    Public Sub GoToAIDifficulty()
        currentState = GameState.AIDifficulty
        'Form1.PlayStartLoop(600)
        'Form1.ResetForMenu()
        Form1.CenterBall()
        Form1.MovePointerCenterScreen()
    End Sub

    'Public Sub StartMatch()
    '    Form1.FadeOutStartLoop(600)
    '    Form1.FadeOutPauseLoop(600)

    '    Form1.MovePointerOffScreen()

    '    model.ScoreLeft = 0
    '    model.ScoreRight = 0
    '    renderer.UpdateScore(model.ScoreLeft, model.ScoreRight)

    '    If model.PlayerMode = 1 Then
    '        model.LeftPlayerName = "You"
    '        model.RightPlayerName = "CPU"
    '    Else
    '        model.LeftPlayerName = "Left"
    '        model.RightPlayerName = "Right"
    '    End If

    '    renderer.UpdatePlayerNames(model.LeftPlayerName, model.RightPlayerName)

    '    physics.PrepareNewMatch()

    '    model.State = GameState.Playing
    '    audio.PlayGameplayLoop(600)
    'End Sub

    'Public Sub GoToPause()
    '    audio.FadeOutGameplayLoop(600)
    '    window.MovePointerCenter()

    '    model.PauseSelection = 0
    '    renderer.SetPauseMenuSelection(0)

    '    model.State = GameState.Pause
    '    physics.Stop()
    '    audio.PlayPauseLoop(600)
    'End Sub

    'Public Sub ResumeGame()
    '    audio.FadeOutPauseLoop(600)
    '    window.MovePointerOffScreen()

    '    model.State = GameState.Playing
    '    physics.Start()
    '    audio.PlayGameplayLoop(600)
    'End Sub

    'Public Sub GoToEndScreen(winner As String)
    '    model.State = GameState.EndScreen
    '    model.WinnerText = winner
    '    renderer.SetWinnerText(winner)

    '    audio.FadeOutGameplayLoop(600)
    '    audio.PlayStartLoop(600)

    '    physics.ResetForMenu()
    '    window.MovePointerCenter()
    'End Sub

    'Public Sub QuitToStartScreen()
    '    audio.FadeOutPauseLoop(600)

    '    model.ScoreLeft = 0
    '    model.ScoreRight = 0
    '    model.WinnerText = ""
    '    renderer.SetWinnerText("")

    '    physics.ResetForMenu()

    '    model.State = GameState.StartScreen
    '    audio.PlayStartLoop(600)
    '    physics.Start()
    'End Sub

    'Public Sub QuitGame()
    '    audio.PlayExitSound()
    '    audio.FadeOutAllLoops(700)
    '    physics.Stop()

    '    Dim t As New Timer() With {.Interval = 800}
    '    AddHandler t.Tick,
    '        Sub()
    '            t.Stop()
    '            t.Dispose()
    '            window.CloseForm()
    '        End Sub

    '    t.Start()
    'End Sub

    ' ============================================================
    ' FULLSCREEN TOGGLE
    ' ============================================================
    'Public Sub ToggleFullScreen()
    '    window.ToggleFullScreen(model)
    '    model.IsFullscreen = Not model.IsFullscreen
    'End Sub




End Class
