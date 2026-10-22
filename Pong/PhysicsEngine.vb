Imports Pong.Enums

Public Class PhysicsEngine



    Private ReadOnly model As GameModel
    Private ReadOnly audio As AudioController


    Private ReadOnly physicsTimer As Timer
    Private ReadOnly stopwatch As Stopwatch



    Private Const WinningScore As Integer = 10

    Public Event MatchWon(winnerId As Integer,
                          winnerText As String)

    Public Event FrameUpdated()
    Public Event WallImpact(location As PointF)




    Public Sub New(model As GameModel,
               audio As AudioController)

        Me.model = model
        Me.audio = audio

        physicsTimer = New Timer With {
            .Interval = 15
        }

        AddHandler physicsTimer.Tick,
        AddressOf Tick

        stopwatch = New Stopwatch()

    End Sub



    ' ============================================================
    ' START / STOP
    ' ============================================================
    Public Sub Start()
        stopwatch.Restart()
        physicsTimer.Start()
    End Sub



    Public Sub [Stop]()

        physicsTimer.Stop()

    End Sub


    ' ============================================================
    ' MAIN PHYSICS LOOP
    '' ============================================================


    Private Sub Tick(
        sender As Object,
        e As EventArgs)

        If model.State = GameState.Pause Then Return

        Dim dt As Double =
            stopwatch.Elapsed.TotalSeconds

        stopwatch.Restart()

        dt = Math.Min(dt, 0.05)


        MoveBall(dt)

        HandleWallCollisions()

        Select Case model.State

            Case GameState.Playing

                MovePaddles(dt)

                If model.PlayerMode = NumberOfPlayers.OnePlayer Then

                    UpdateAI(dt)

                End If

                HandlePaddleCollisions()

                CheckScore()

                TrackPaddleVelocities()

        End Select

        RaiseEvent FrameUpdated()


    End Sub




    ' ============================================================
    ' BALL MOVEMENT
    ' ============================================================
    Private Sub MoveBall(dt As Double)
        model.BallPos = New PointF(
                        model.BallPos.X + CSng(model.VelX * dt),
                        model.BallPos.Y + CSng(model.VelY * dt))

    End Sub

    ' ============================================================
    ' PADDLE MOVEMENT
    ' ============================================================
    Private Sub MovePaddles(dt As Double)

        Dim left = model.LeftPaddleRect
        Dim right = model.RightPaddleRect

        If model.Input.LeftUp Then
            left.Y -= CInt(model.PaddleSpeed * dt)
        End If

        If model.Input.LeftDown Then
            left.Y += CInt(model.PaddleSpeed * dt)
        End If

        If model.PlayerMode = NumberOfPlayers.TwoPlayers Then
            If model.Input.RightUp Then
                right.Y -= CInt(model.PaddleSpeed * dt)
            End If

            If model.Input.RightDown Then
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
    'Private Sub UpdateAI(dt As Double)

    '    Dim right = model.RightPaddleRect

    '    Dim targetY As Single = model.BallPos.Y + model.BallDiameter / 2
    '    Dim paddleCenter As Single = right.Y + model.PaddleHeight / 2

    '    Dim factor As Double = Math.Max(model.AIModeFactor * model.AIDifficultyScale, 0.25)


    '    If targetY < paddleCenter Then
    '        right.Y -= CSng(model.PaddleSpeed * dt * factor)
    '    ElseIf targetY > paddleCenter Then
    '        right.Y += CSng(model.PaddleSpeed * dt * factor)
    '    End If


    '    right.Y = Math.Max(0, Math.Min(model.ClientSize.Height - model.PaddleHeight, right.Y))

    '    model.RightPaddleRect = right
    'End Sub



    Private Const AI_DEADZONE As Single = 8.0F

    Private Sub UpdateAI(dt As Double)

        Dim right = model.RightPaddleRect

        Dim targetY As Single =
        model.BallPos.Y +
        model.BallDiameter / 2

        Dim paddleCenter As Single =
        right.Y +
        model.PaddleHeight / 2

        Dim factor As Double =
        Math.Max(
            model.AIModeFactor * model.AIDifficultyScale,
            0.25
        )

        Dim difference As Single =
        targetY - paddleCenter

        If Math.Abs(difference) > AI_DEADZONE Then

            Dim movement As Single =
            CSng(model.PaddleSpeed * dt * factor)

            right.Y +=
            Math.Sign(difference) * movement
        End If

        right.Y = Math.Max(
        0,
        Math.Min(
                model.ClientSize.Height - model.PaddleHeight,
                right.Y
            )
        )

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
            RaiseEvent WallImpact(New PointF(
                pos.X + model.BallDiameter / 2.0F,
                0))
        ElseIf pos.Y >= model.ClientSize.Height - model.BallDiameter Then
            pos.Y = model.ClientSize.Height - model.BallDiameter
            model.VelY = -Math.Abs(model.VelY)
            audio.PlayWithCooldown("bounce", 100)
            RaiseEvent WallImpact(New PointF(
                pos.X + model.BallDiameter / 2.0F,
                model.ClientSize.Height))
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
            If model.PlayerMode = NumberOfPlayers.OnePlayer Then
                Return 175 * (Math.PI / 180.0)
            Else
                Return 180 * (Math.PI / 180.0)
            End If
        End If
    End Function

    ' ============================================================
    ' SCORING
    '' ============================================================

    Private Sub CheckScore()

        If model.BallPos.X <= 0 Then


            AwardPoint(True)

            Return

        End If

        If model.BallPos.X >= model.ClientSize.Width - model.BallDiameter Then


            AwardPoint(False)

            Return

        End If

        If model.ScoreLeft >= WinningScore Then


            RaiseEvent MatchWon(1,
                                If(model.PlayerMode = NumberOfPlayers.OnePlayer,
                                "You Win!", "Left Wins!"))

            Return

        End If

        If model.ScoreRight >= WinningScore Then


            RaiseEvent MatchWon(2,
                                $"{model.RightPlayerName} Wins!")

            Return

        End If

    End Sub


    Private Sub AwardPoint(scoreRight As Boolean)

        If scoreRight Then
            model.ScoreRight += 1
            ResetBall(1)
        Else
            model.ScoreLeft += 1
            ResetBall(-1)
        End If

        audio.PlayPoint()

        ResetPaddles()

    End Sub


    ' ============================================================
    ' RESET HELPERS
    ' ============================================================
    Private Sub ResetBall(direction As Integer)
        CenterBall()
        ServeBall(direction)
    End Sub

    Public Sub PrepareNewMatch()

        model.BallSpeed =
        800 * (model.ClientSize.Height / 1080.0)

        ResetPaddles()

        CenterBall()

        ServeBall(
        If(model.RNG.Next(0, 2) = 0, -1, 1))

    End Sub


    Public Sub ResetForMenu()

        model.BallSpeed =
        200 * (model.ClientSize.Height / 1080.0)

        ResetPaddles()


    End Sub


    Public Sub ResetForEnd()

        model.BallSpeed =
        200 * (model.ClientSize.Height / 1080.0)

        ResetPaddles()

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
