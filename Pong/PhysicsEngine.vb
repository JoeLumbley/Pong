Imports Pong.Enums
Imports Pong.Rendering

Public Class PhysicsEngine

    Private ReadOnly model As GameModel
    Private ReadOnly audio As AudioController
    Private ReadOnly renderer As Rendering
    'Private ReadOnly state As GameStateManager
    Private ReadOnly form As Form
    'Private ReadOnly settings As SettingsManager
    Private ReadOnly match As MatchManager
    Private ReadOnly window As WindowManager

    Private physicsTimer As Timer
    Private stopwatch As Stopwatch

    Public Sub New(model As GameModel,
                   audio As AudioController,
                   renderer As Rendering,
                   form As Form,
                   match As MatchManager,
                   window As WindowManager)

        Me.model = model
        Me.audio = audio
        Me.renderer = renderer
        'Me.state = state
        Me.form = form
        'Me.settings = settings
        Me.match = match
        Me.window = window

        physicsTimer = New Timer With {.Interval = 15}
        AddHandler physicsTimer.Tick, AddressOf Tick

        stopwatch = New Stopwatch()
        stopwatch.Start()
    End Sub

    ' ============================================================
    ' START / STOP
    ' ============================================================
    Public Sub Start()
        stopwatch.Restart()
        physicsTimer.Start()
    End Sub

    Public Sub StopTimer()
        physicsTimer.Stop()
    End Sub

    ' ============================================================
    ' MAIN PHYSICS LOOP
    ' ============================================================
    Private Sub Tick(sender As Object, e As EventArgs)
        If model.State = GameState.Pause Then Return

        Dim dt As Double = stopwatch.Elapsed.TotalSeconds
        stopwatch.Restart()

        dt = Math.Min(dt, 0.05)

        MoveBall(dt)
        MovePaddles(dt)

        If model.PlayerMode = 1 AndAlso model.State = GameState.Playing Then
            UpdateAI(dt)
        End If

        HandleWallCollisions()
        'HandlePaddleCollisions()
        'CheckScore()

        'TrackPaddleVelocities()

        ''renderer.UpdateBallPosition(model.ballPos)
        ''renderer.UpdatePaddlePositions(model.paddleLeft.Y, model.paddleRight.Y, model.ClientSize)
        'renderer.UpdateTrail()


        'form.Invalidate()





        HandleWallCollisions()
        renderer.UpdateTrail(model)

        Select Case model.State
            Case GameState.StartScreen
                renderer.UpdateStartScreenFX()

            Case GameState.Playing
                'UpdatePaddles(dt)
                MovePaddles(dt)

                If model.PlayerMode = 1 Then
                    UpdateAI(dt)
                End If

                HandlePaddleCollisions()
                CheckScore()

                TrackPaddleVelocities()


                '' Track paddle velocities for spin
                'model.PaddleLeftVelocity = model.PaddleLeft.Y - model.LastPaddleLeftY
                'model.LastPaddleLeftY = model.PaddleLeft.Y

                'model.PaddleRightVelocity = model.PaddleRight.Y - model.LastPaddleRightY
                'model.LastPaddleRightY = model.PaddleRight.Y

            Case GameState.AIDifficulty
                renderer.UpdateStartScreenFX()

            Case GameState.Pause
                    ' No updates needed for Pause screen

            Case GameState.EndScreen
                renderer.UpdateStartScreenFX()

        End Select


        form.Invalidate()

    End Sub

    ' ============================================================
    ' BALL MOVEMENT
    ' ============================================================
    Private Sub MoveBall(dt As Double)
        model.ballPos = New PointF(
            model.ballPos.X + CSng(model.velX * dt),
            model.ballPos.Y + CSng(model.velY * dt)
        )
    End Sub

    ' ============================================================
    ' PADDLE MOVEMENT
    ' ============================================================
    Private Sub MovePaddles(dt As Double)

        Dim left = model.LeftPaddleRect
        Dim right = model.RightPaddleRect

        If model.LeftPaddleUp Then
            left.Y -= CInt(model.PaddleSpeed * dt)
        End If

        If model.LeftPaddleDown Then
            left.Y += CInt(model.PaddleSpeed * dt)
        End If

        If model.PlayerMode = 2 Then
            If model.RightPaddleUp Then
                right.Y -= CInt(model.PaddleSpeed * dt)
            End If

            If model.RightPaddleDown Then
                right.Y += CInt(model.PaddleSpeed * dt)
            End If
        End If

        left.Y = Math.Max(0, Math.Min(model.ClientSize.Height - model.PaddleHeight, left.Y))
        right.Y = Math.Max(0, Math.Min(model.ClientSize.Height - model.PaddleHeight, right.Y))

        model.LeftPaddleRect = left
        model.RightPaddleRect = right
    End Sub

    ' ============================================================
    ' AI MOVEMENT
    ' ============================================================
    Private Sub UpdateAI(dt As Double)

        Dim right = model.RightPaddleRect

        Dim targetY As Single = model.BallPos.Y + model.BallDiameter / 2
        Dim paddleCenter As Single = right.Y + model.PaddleHeight / 2

        Dim factor As Double = model.AIModeFactor * model.AIDifficultyScale

        If targetY < paddleCenter - 20 Then
            right.Y -= CSng(model.PaddleSpeed * dt * factor)
        ElseIf targetY > paddleCenter + 20 Then
            right.Y += CSng(model.PaddleSpeed * dt * factor)
        End If

        right.Y = Math.Max(0, Math.Min(model.ClientSize.Height - model.PaddleHeight, right.Y))

        model.RightPaddleRect = right
    End Sub

    ' ============================================================
    ' WALL COLLISIONS
    ' ============================================================
    Private Sub HandleWallCollisions()

        Dim pos = model.BallPos

        If pos.Y <= 0 Then
            pos.Y = 0
            model.VelY = Math.Abs(model.VelY)
            audio.PlayWithCooldown("bounce", 100)
        ElseIf pos.Y >= model.ClientSize.Height - model.BallDiameter Then
            pos.Y = model.ClientSize.Height - model.BallDiameter
            model.VelY = -Math.Abs(model.VelY)
            audio.PlayWithCooldown("bounce", 100)
        End If

        If model.State = GameState.StartScreen OrElse
           model.State = GameState.EndScreen OrElse
           model.State = GameState.AIDifficulty Then

            If pos.X <= 0 Then
                pos.X = 0
                model.VelX = Math.Abs(model.VelX)
                audio.PlayWithCooldown("bounce", 100)
            ElseIf pos.X >= model.ClientSize.Width - model.BallDiameter Then
                pos.X = model.ClientSize.Width - model.BallDiameter
                model.VelX = -Math.Abs(model.VelX)
                audio.PlayWithCooldown("bounce", 100)
            End If
        End If

        model.BallPos = pos
    End Sub

    ' ============================================================
    ' PADDLE COLLISIONS
    ' ============================================================
    Private Sub HandlePaddleCollisions()

        Dim ballRect As New RectangleF(model.BallPos.X, model.BallPos.Y, model.BallDiameter, model.BallDiameter)
        Dim angle As Double

        If ballRect.IntersectsWith(model.LeftPaddleRect) AndAlso model.VelX < 0 Then
            angle = ComputeLeftPaddleAngle()
            model.VelX = Math.Cos(angle) * model.BallSpeed
            model.VelY = Math.Sin(angle) * model.BallSpeed
            audio.PlayWithCooldown("bounce", 100)
        End If

        If ballRect.IntersectsWith(model.RightPaddleRect) AndAlso model.VelX > 0 Then
            angle = ComputeRightPaddleAngle()
            model.VelX = Math.Cos(angle) * model.BallSpeed
            model.VelY = Math.Sin(angle) * model.BallSpeed
            audio.PlayWithCooldown("bounce", 100)
        End If
    End Sub

    Private Function ComputeLeftPaddleAngle() As Double
        If model.PaddleLeftVelocity < -1 Then
            Return 315 * (Math.PI / 180.0)
        ElseIf model.PaddleLeftVelocity > 1 Then
            Return 45 * (Math.PI / 180.0)
        Else
            Return 0
        End If
    End Function

    Private Function ComputeRightPaddleAngle() As Double
        If model.PaddleRightVelocity < -1 Then
            Return 225 * (Math.PI / 180.0)
        ElseIf model.PaddleRightVelocity > 1 Then
            Return 135 * (Math.PI / 180.0)
        Else
            If model.PlayerMode = 1 Then
                Return 175 * (Math.PI / 180.0)
            Else
                Return 180 * (Math.PI / 180.0)
            End If
        End If
    End Function

    ' ============================================================
    ' SCORING
    ' ============================================================
    Private Sub CheckScore()

        If model.BallPos.X <= 0 Then


            model.ScoreRight += 1
            renderer.UpdateScore(model.ScoreLeft, model.ScoreRight, form, model)

            audio.PlayPoint()
            ResetBall(1)
            ResetPaddles()
            Return
        End If

        If model.BallPos.X >= model.ClientSize.Width - model.BallDiameter Then

            model.ScoreLeft += 1
            renderer.UpdateScore(model.ScoreLeft, model.ScoreRight, form, model)

            audio.PlayPoint()
            ResetBall(-1)
            ResetPaddles()
            Return
        End If

        If model.ScoreLeft >= 10 Then

            ' 1 player wins - You Win!
            ' 2 player wins - Left Wins!

            If model.PlayerMode = 1 Then
                model.WinnerText = "You Win!"
            Else
                model.WinnerText = "Left Wins!"
            End If
            model.WinnerIs = 1

            'model.WinnerText = $"{model.LeftPlayerName} Wins!"

            '


            model.State = GameState.EndScreen
            'EndMatch()
            match.EndMatch(audio, window, model, Me)

        End If

        If model.ScoreRight >= 10 Then
            model.WinnerText = $"{model.RightPlayerName} Wins!"
            model.WinnerIs = 2

            model.State = GameState.EndScreen
            'EndMatch()
            match.EndMatch(audio, window, model, Me)
        End If

    End Sub


    'Public Sub EndMatch()

    '    audio.FadeOutAndStopGamePlayLoop(600)

    '    window.MovePointerCenterScreen()

    '    model.BallSpeed = 200 * (model.ClientSize.Height / 1080.0)

    '    CenterBall()
    '    MoveBallRandom()

    '    audio.PlayStartLoop(600)

    'End Sub



    ' ============================================================
    ' RESET HELPERS
    ' ============================================================
    Private Sub ResetBall(direction As Integer)
        CenterBall()
        ServeBall(direction)
        renderer.ClearTrail()
    End Sub

    Public Sub PrepareNewMatch()
        model.BallSpeed = 800 * (model.ClientSize.Height / 1080.0)
        CenterBall()
        ServeBall(If(model.RNG.Next(0, 2) = 0, -1, 1))
    End Sub

    Public Sub ResetForMenu()
        model.BallSpeed = 200 * (model.ClientSize.Height / 1080.0)
        CenterBall()
        MoveBallRandom()
    End Sub

    Public Sub ResetPaddles()
        Dim left = model.LeftPaddleRect
        Dim right = model.RightPaddleRect

        left.Y = (model.ClientSize.Height - model.PaddleHeight) / 2
        right.Y = (model.ClientSize.Height - model.PaddleHeight) / 2

        model.LeftPaddleRect = left
        model.RightPaddleRect = right
    End Sub

    Public Sub CenterBall()
        model.BallPos = New PointF(
            (model.ClientSize.Width - model.BallDiameter) / 2.0F,
            (model.ClientSize.Height - model.BallDiameter) / 2.0F)
    End Sub

    Public Sub MoveBallRandom()
        Dim angle As Double = model.RNG.NextDouble() * Math.PI * 2.0
        model.VelX = Math.Cos(angle) * model.BallSpeed
        model.VelY = Math.Sin(angle) * model.BallSpeed
    End Sub

    Public Sub ServeBall(direction As Integer)
        Dim angle As Double = model.RNG.NextDouble() * (Math.PI / 3.0) - (Math.PI / 6.0)
        model.VelX = Math.Cos(angle) * model.BallSpeed * direction
        model.VelY = Math.Sin(angle) * model.BallSpeed
    End Sub

    ' ============================================================
    ' PADDLE VELOCITY TRACKING
    ' ============================================================
    Private Sub TrackPaddleVelocities()
        model.PaddleLeftVelocity = model.LeftPaddleRect.Y - model.LastPaddleLeftY
        model.LastPaddleLeftY = model.LeftPaddleRect.Y

        model.PaddleRightVelocity = model.RightPaddleRect.Y - model.LastPaddleRightY
        model.LastPaddleRightY = model.RightPaddleRect.Y
    End Sub

End Class
