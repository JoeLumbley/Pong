Imports Pong.Enums

Public Class MatchManager

    ' -------------------------------
    '  Random
    ' -------------------------------
    Private rng As New Random()

    Public Sub EndMatch(audio As AudioController,
                        window As WindowManager,
                        model As GameModel,
                        physics As PhysicsEngine)


        audio.FadeOutAndStopGamePlayLoop(600)

        window.MovePointerCenterScreen()

        model.BallSpeed = 200 * (model.ClientSize.Height / 1080.0)

        physics.CenterBall()
        physics.MoveBallRandom()

        audio.PlayStartLoop(600)

    End Sub


    Public Sub StartNewMatch(audio As AudioController,
                             window As WindowManager,
                             model As GameModel,
                             physics As PhysicsEngine,
                             renderer As Rendering,
                             form As Form)

        audio.FadeOutAndStopStartLoop(600)

        audio.FadeOutAndStopPausedLoop(600)

        window.MovePointerOffScreen()

        model.BallSpeed = 800 * (model.ClientSize.Height / 1080.0)



        model.ScoreLeft = 0
        model.ScoreRight = 0

        renderer.UpdateScore(model.ScoreLeft, model.ScoreRight, form, model)

        'renderer.UpdateScore(model.ScoreLeft, model.ScoreRight, Me.CreateGraphics(), ClientSize)

        If model.PlayerMode = NumberOfPlayers.OnePlayer Then

            model.LeftPlayerName = "You"
            model.RightPlayerName = "CPU"
            renderer.UpdatePlayerNames(model.LeftPlayerName, model.RightPlayerName, form, model)

        Else

            model.LeftPlayerName = "Left"
            model.RightPlayerName = "Right"
            renderer.UpdatePlayerNames(model.LeftPlayerName, model.RightPlayerName, form, model)

        End If

        physics.ResetPaddles()
        physics.CenterBall()
        physics.ServeBall(If(rng.Next(0, 2) = 0, -1, 1))

        model.State = GameState.Playing

        physics.Start()

        audio.PlayGamePlayLoop(600)

    End Sub

    Public Sub PauseGame(audio As AudioController,
                         window As WindowManager,
                         physics As PhysicsEngine,
                         model As GameModel)

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


    Public Sub ResumeGame(audio As AudioController,
                          window As WindowManager,
                          physics As PhysicsEngine,
                          model As GameModel)

        audio.FadeOutAndStopPausedLoop(600)

        window.MovePointerOffScreen()

        model.State = GameState.Playing

        physics.Start()

        audio.PlayGamePlayLoop(600)

    End Sub

    Public Sub Quit2StartScreen(audio As AudioController,
                                physics As PhysicsEngine,
                                model As GameModel,
                                renderer As Rendering,
                                form As Form)

        audio.FadeOutAndStopPausedLoop(600)

        model.BallSpeed = 200 * (model.ClientSize.Height / 1080.0)
        model.WinnerText = String.Empty


        model.ScoreLeft = 0
        model.ScoreRight = 0

        renderer.UpdateScore(model.ScoreLeft, model.ScoreRight, form, model)


        physics.CenterBall()
        physics.MoveBallRandom()

        model.State = GameState.StartScreen

        physics.Start()

        audio.PlayStartLoop(600)

    End Sub



End Class
