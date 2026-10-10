Imports Pong.Enums
Imports Pong.Rendering

Public Class PhysicsEngine

    Private ReadOnly model As GameModel
    Private ReadOnly audio As AudioController
    Private ReadOnly renderer As Rendering
    Private ReadOnly state As GameStateManager

    Private physicsTimer As Timer
    Private stopwatch As Stopwatch

    Public Sub New(model As GameModel,
                   audio As AudioController,
                   renderer As Rendering,
                   state As GameStateManager)

        Me.model = model
        Me.audio = audio
        Me.renderer = renderer
        Me.state = state

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
        HandlePaddleCollisions()
        CheckScore()

        TrackPaddleVelocities()

        renderer.UpdateBallPosition(model.ballPos)
        renderer.UpdatePaddlePositions(model.paddleLeft.Y, model.paddleRight.Y, model.ClientSize)
        renderer.UpdateTrail()
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

        Dim left = model.paddleLeft
        Dim right = model.paddleRight

        If model.LeftPaddleUp Then
            left.Y -= CInt(model.paddleSpeed * dt)
        End If

        If model.LeftPaddleDown Then
            left.Y += CInt(model.paddleSpeed * dt)
        End If

        If model.PlayerMode = 2 Then
            If model.RightPaddleUp Then
                right.Y -= CInt(model.paddleSpeed * dt)
            End If

            If model.RightPaddleDown Then
                right.Y += CInt(model.paddleSpeed * dt)
            End If
        End If

        left.Y = Math.Max(0, Math.Min(model.ClientSize.Height - model.paddleHeight, left.Y))
        right.Y = Math.Max(0, Math.Min(model.ClientSize.Height - model.paddleHeight, right.Y))

        model.paddleLeft = left
        model.paddleRight = right
    End Sub

    ' ============================================================
    ' AI MOVEMENT
    ' ============================================================
    Private Sub UpdateAI(dt As Double)

        Dim right = model.paddleRight

        Dim targetY As Single = model.ballPos.Y + model.ballDiameter / 2
        Dim paddleCenter As Single = right.Y + model.paddleHeight / 2

        Dim factor As Double = model.aiModeFactor * model.AIDifficultyScale

        If targetY < paddleCenter - 20 Then
            right.Y -= CSng(model.paddleSpeed * dt * factor)
        ElseIf targetY > paddleCenter + 20 Then
            right.Y += CSng(model.paddleSpeed * dt * factor)
        End If

        right.Y = Math.Max(0, Math.Min(model.ClientSize.Height - model.paddleHeight, right.Y))

        model.paddleRight = right
    End Sub

    ' ============================================================
    ' WALL COLLISIONS
    ' ============================================================
    Private Sub HandleWallCollisions()

        Dim pos = model.ballPos

        If pos.Y <= 0 Then
            pos.Y = 0
            model.velY = Math.Abs(model.velY)
            audio.PlayWithCooldown("bounce", 100)
        ElseIf pos.Y >= model.ClientSize.Height - model.ballDiameter Then
            pos.Y = model.ClientSize.Height - model.ballDiameter
            model.velY = -Math.Abs(model.velY)
            audio.PlayWithCooldown("bounce", 100)
        End If

        If model.State = GameState.StartScreen OrElse
           model.State = GameState.EndScreen OrElse
           model.State = GameState.AIDifficulty Then

            If pos.X <= 0 Then
                pos.X = 0
                model.velX = Math.Abs(model.velX)
                audio.PlayWithCooldown("bounce", 100)
            ElseIf pos.X >= model.ClientSize.Width - model.ballDiameter Then
                pos.X = model.ClientSize.Width - model.ballDiameter
                model.velX = -Math.Abs(model.velX)
                audio.PlayWithCooldown("bounce", 100)
            End If
        End If

        model.ballPos = pos
    End Sub

    ' ============================================================
    ' PADDLE COLLISIONS
    ' ============================================================
    Private Sub HandlePaddleCollisions()

        Dim ballRect As New RectangleF(model.ballPos.X, model.ballPos.Y, model.ballDiameter, model.ballDiameter)
        Dim angle As Double

        If ballRect.IntersectsWith(model.paddleLeft) AndAlso model.velX < 0 Then
            angle = ComputeLeftPaddleAngle()
            model.velX = Math.Cos(angle) * model.BallSpeed
            model.velY = Math.Sin(angle) * model.BallSpeed
            audio.PlayWithCooldown("bounce", 100)
        End If

        If ballRect.IntersectsWith(model.paddleRight) AndAlso model.velX > 0 Then
            angle = ComputeRightPaddleAngle()
            model.velX = Math.Cos(angle) * model.BallSpeed
            model.velY = Math.Sin(angle) * model.BallSpeed
            audio.PlayWithCooldown("bounce", 100)
        End If
    End Sub

    Private Function ComputeLeftPaddleAngle() As Double
        If model.paddleLeftVelocity < -1 Then
            Return 315 * (Math.PI / 180.0)
        ElseIf model.paddleLeftVelocity > 1 Then
            Return 45 * (Math.PI / 180.0)
        Else
            Return 0
        End If
    End Function

    Private Function ComputeRightPaddleAngle() As Double
        If model.paddleRightVelocity < -1 Then
            Return 225 * (Math.PI / 180.0)
        ElseIf model.paddleRightVelocity > 1 Then
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

        If model.ballPos.X <= 0 Then
            model.scoreRight += 1
            audio.PlayPoint()
            ResetBall(1)
            ResetPaddles()
            Return
        End If

        If model.ballPos.X >= model.ClientSize.Width - model.ballDiameter Then
            model.scoreLeft += 1
            audio.PlayPoint()
            ResetBall(-1)
            ResetPaddles()
            Return
        End If

        If model.scoreLeft >= 10 Then
            model.WinnerText = $"{model.LeftPlayerName} Wins!"
            state.SetCurrentState(GameState.EndScreen)
        End If

        If model.scoreRight >= 10 Then
            model.WinnerText = $"{model.RightPlayerName} Wins!"
            state.SetCurrentState(GameState.EndScreen)
        End If

    End Sub

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
        ServeBall(If(model.rng.Next(0, 2) = 0, -1, 1))
    End Sub

    Public Sub ResetForMenu()
        model.BallSpeed = 200 * (model.ClientSize.Height / 1080.0)
        CenterBall()
        MoveBallRandom()
    End Sub

    Public Sub ResetPaddles()
        Dim left = model.PaddleLeft
        Dim right = model.PaddleRight

        left.Y = (model.ClientSize.Height - model.PaddleHeight) / 2
        right.Y = (model.ClientSize.Height - model.PaddleHeight) / 2

        model.PaddleLeft = left
        model.PaddleRight = right
    End Sub

    Public Sub CenterBall()
        model.BallPos = New PointF(
            (model.ClientSize.Width - model.BallDiameter) / 2.0F,
            (model.ClientSize.Height - model.BallDiameter) / 2.0F)
    End Sub

    Private Sub MoveBallRandom()
        Dim angle As Double = model.rng.NextDouble() * Math.PI * 2.0
        model.velX = Math.Cos(angle) * model.BallSpeed
        model.velY = Math.Sin(angle) * model.BallSpeed
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
        model.paddleLeftVelocity = model.paddleLeft.Y - model.lastPaddleLeftY
        model.lastPaddleLeftY = model.paddleLeft.Y

        model.paddleRightVelocity = model.paddleRight.Y - model.lastPaddleRightY
        model.lastPaddleRightY = model.paddleRight.Y
    End Sub

End Class
