Imports System.Drawing.Drawing2D
Imports System.IO

Public Class Form1

    ' -------------------------------
    '  Game State
    ' -------------------------------
    Private Enum GameState
        StartScreen
        Playing
        EndScreen
        Pause
    End Enum

    Private currentState As GameState = GameState.StartScreen
    Private winnerText As String = String.Empty

    ' -------------------------------
    '  Player Mode
    ' -------------------------------
    Private playerMode As Integer = 1       ' 1 = Single Player (AI), 2 = Two Players
    Private selectedOption As Integer = 0   ' 0 = "1 Player", 1 = "2 Players"

    ' -------------------------------
    '  Engine State
    ' -------------------------------
    Private ballPos As PointF
    Private ballDiameter As Integer = 60

    Private velX As Double
    Private velY As Double
    Private speed As Double = 200

    Private physicsTimer As New Timer()
    Private sw As New Stopwatch()

    ' -------------------------------
    '  FPS Tracking
    ' -------------------------------
    Private frameCount As Integer = 0
    Private fps As Integer = 0
    Private fpsTimer As New Stopwatch()

    ' -------------------------------
    '  GDI Resources
    ' -------------------------------
    Private ballBrush As SolidBrush
    Private fpsBrush As SolidBrush
    Private fpsFont As Font
    Private trailBrushes As SolidBrush()
    Private paddleBrush As SolidBrush

    ' -------------------------------
    '  Trail System
    ' -------------------------------
    Private trail As New List(Of PointF)
    Private trailLength As Integer = 25
    Private trailSizes As Integer()
    Private trailOffsets As Single()
    Private trailAlpha As Integer()

    Private lastPlay As New Dictionary(Of String, Double)

    ' -------------------------------
    '  Pong State
    ' -------------------------------
    Private paddleLeft As RectangleF
    Private paddleRight As RectangleF

    Private paddleWidth As Integer = 32
    Private paddleHeight As Integer = 128
    Private paddleSpeed As Integer = 700

    Private moveUpLeft As Boolean
    Private moveDownLeft As Boolean
    Private moveUpRight As Boolean
    Private moveDownRight As Boolean

    Private scoreLeft As Integer = 0
    Private scoreRight As Integer = 0

    ' -------------------------------
    '  Start Screen FX
    ' -------------------------------
    Private titleAlpha As Integer = 0
    Private titleFadeIn As Boolean = True
    Private blinkVisible As Boolean = True
    Private blinkTimer As New Stopwatch()


    Private rng As New Random()

    Private lastPaddleLeftY As Single
    Dim paddleLeftVelocity As Single
    Dim paddleRightVelocity As Single
    Dim lastPaddleRightY As Single

    Public Sub New()

        'Me.InitializeComponent()

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer, True)

        Me.DoubleBuffered = True
        Me.BackColor = Color.Black
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.WindowState = FormWindowState.Maximized

        '' Center ball
        'ballPos = New PointF((ClientSize.Width - ballDiameter) / 2,
        '                     (ClientSize.Height - ballDiameter) / 2)

        CenterBall()

        '' Random direction
        'Dim rnd As New Random()
        'Dim angle As Double = rnd.NextDouble() * Math.PI * 2
        'velX = Math.Cos(angle) * speed
        'velY = Math.Sin(angle) * speed

        MoveBallRandom()

        physicsTimer.Interval = 15
        AddHandler physicsTimer.Tick, AddressOf PhysicsTick

        sw.Start()
        fpsTimer.Start()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        InitAudio()
        InitGraphics()
        InitTrails()
        InitPaddles()
        InitPhysics()

        blinkTimer.Start()
    End Sub

    Private Sub InitPhysics()
        physicsTimer.Start()
    End Sub

    Private Sub InitGraphics()
        ballBrush = New SolidBrush(Color.DeepSkyBlue)
        fpsBrush = New SolidBrush(Color.White)
        fpsFont = New Font("Segoe UI", 14, FontStyle.Bold)
        paddleBrush = New SolidBrush(Color.White)
    End Sub

    Private Sub InitTrails()
        trailSizes = New Integer(trailLength - 1) {}
        trailOffsets = New Single(trailLength - 1) {}

        For i As Integer = 0 To trailLength - 1
            Dim size As Integer = ballDiameter - (trailLength - i) * 2
            If size < 10 Then size = 10

            trailSizes(i) = size
            trailOffsets(i) = CSng((ballDiameter - size) / 2)
        Next

        trailAlpha = New Integer(trailLength - 1) {}
        For i As Integer = 0 To trailLength - 1
            Dim t As Double = i / trailLength
            trailAlpha(i) = CInt(32 * t * t)
        Next

        trailBrushes = New SolidBrush(trailLength - 1) {}
        For i As Integer = 0 To trailLength - 1
            trailBrushes(i) = New SolidBrush(Color.FromArgb(trailAlpha(i), 0, 191, 255))
        Next
    End Sub

    Private Sub InitPaddles()
        paddleLeft = New RectangleF(50,
                                    (ClientSize.Height - paddleHeight) / 2,
                                    paddleWidth,
                                    paddleHeight)

        paddleRight = New RectangleF(ClientSize.Width - 50 - paddleWidth,
                                     (ClientSize.Height - paddleHeight) / 2,
                                     paddleWidth,
                                     paddleHeight)
    End Sub

    Private Sub InitAudio()
        CreateSoundFiles()

        AudioPlayer.AddSound("loop", Path.Combine(Application.StartupPath, "loop.mp3"))
        AudioPlayer.SetVolume("loop", 100)

        AudioPlayer.AddSound("point", Path.Combine(Application.StartupPath, "point.mp3"))
        AudioPlayer.SetVolume("point", 125)

        AudioPlayer.AddOverlapping("bounce", Path.Combine(Application.StartupPath, "bounce.mp3"))
        AudioPlayer.SetVolumeOverlapping("bounce", 70)

        AudioPlayer.AddSound("start", Path.Combine(Application.StartupPath, "start.mp3"))
        AudioPlayer.SetVolume("start", 70)
        AudioPlayer.LoopSound("start")
    End Sub

    ' -------------------------------
    '  Physics Loop
    ' -------------------------------
    Private Sub PhysicsTick(sender As Object, e As EventArgs)
        If Me.WindowState = FormWindowState.Minimized Then Return



        Dim dt As Double = sw.Elapsed.TotalSeconds
        sw.Restart()
        dt = Math.Min(dt, 0.05)

        ' Ball always moves
        ballPos.X += CSng(velX * dt)
        ballPos.Y += CSng(velY * dt)

        HandleWallCollisions()
        UpdateTrail()

        If currentState = GameState.StartScreen Then
            UpdateStartScreenFX()
        ElseIf currentState = GameState.Playing Then
            UpdatePaddles(dt)

            If playerMode = 1 Then
                UpdateAI(dt)
            End If

            HandlePaddleCollisions()
            CheckScore()
        End If

        paddleLeftVelocity = paddleLeft.Y - lastPaddleLeftY
        lastPaddleLeftY = paddleLeft.Y

        paddleRightVelocity = paddleRight.Y - lastPaddleRightY
        lastPaddleRightY = paddleRight.Y

        Invalidate()
    End Sub

    Private Sub UpdateStartScreenFX()
        If titleFadeIn Then
            titleAlpha += 3
            If titleAlpha >= 255 Then
                titleAlpha = 255
                titleFadeIn = False
            End If
        Else
            titleAlpha -= 3
            If titleAlpha <= 80 Then
                titleAlpha = 80
                titleFadeIn = True
            End If
        End If

        If blinkTimer.ElapsedMilliseconds >= 800 Then
            blinkVisible = Not blinkVisible
            blinkTimer.Restart()
        End If
    End Sub

    Private Sub UpdatePaddles(dt As Double)
        If moveUpLeft Then paddleLeft.Y -= CSng(paddleSpeed * dt)
        If moveDownLeft Then paddleLeft.Y += CSng(paddleSpeed * dt)

        If playerMode = 2 Then
            If moveUpRight Then paddleRight.Y -= CSng(paddleSpeed * dt)
            If moveDownRight Then paddleRight.Y += CSng(paddleSpeed * dt)
        End If

        paddleLeft.Y = Math.Max(0, Math.Min(ClientSize.Height - paddleHeight, paddleLeft.Y))
        paddleRight.Y = Math.Max(0, Math.Min(ClientSize.Height - paddleHeight, paddleRight.Y))
    End Sub

    Private Sub UpdateAI(dt As Double)
        Dim targetY As Single = ballPos.Y + ballDiameter / 2

        If targetY < paddleRight.Y + paddleHeight / 2 Then
            ' Move paddle up towards the ball with a slight delay to make it
            ' beatable if you increse the delay factor (0.9) you can adjust the
            ' factor to make the AI easier or harder. A lower factor makes it
            ' easier, a higher factor makes it harder.
            ' example: 0.5 = easier, 1.0 = harder
            paddleRight.Y -= CSng(paddleSpeed * dt * 0.655)
        End If

        If targetY > paddleRight.Y + paddleHeight / 2 Then
            paddleRight.Y += CSng(paddleSpeed * dt * 0.655)

        End If

        ' Clamp the paddle position to stay within the window bounds
        paddleRight.Y = Math.Max(0, Math.Min(ClientSize.Height - paddleHeight, paddleRight.Y))

    End Sub
    Private Sub HandlePaddleCollisions()

        Dim ballRect As New RectangleF(ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
        Dim angle As Double

        ' -------------------------
        ' LEFT PADDLE COLLISION
        ' -------------------------
        If ballRect.IntersectsWith(paddleLeft) AndAlso velX < 0 Then

            If paddleLeftVelocity < -1 Then
                ' Paddle moving UP → upward spin
                angle = 315 * (Math.PI / 180) '315

            ElseIf paddleLeftVelocity > 1 Then
                ' Paddle moving DOWN → downward spin
                angle = 45 * (Math.PI / 180) '45

            Else
                ' Paddle stationary → straight bounce
                angle = 0 * (Math.PI / 180)
            End If

            velX = Math.Cos(angle) * speed
            velY = Math.Sin(angle) * speed

            PlayWithCooldown("bounce", 100)

        End If


        ' -------------------------
        ' RIGHT PADDLE COLLISION
        ' -------------------------
        If ballRect.IntersectsWith(paddleRight) AndAlso velX > 0 Then


            If paddleRightVelocity < -1 Then
                ' Paddle moving UP → upward spin
                angle = 225 * (Math.PI / 180) ' 225

            ElseIf paddleRightVelocity > 1 Then
                ' Paddle moving DOWN → downward spin
                angle = 135 * (Math.PI / 180)

            Else
                ' Paddle stationary
                If playerMode = 1 Then
                    ' Single-player mode → bounce slightly down 
                    angle = 175 * (Math.PI / 180)
                Else
                    ' Two-player mode → straight bounce
                    angle = 180 * (Math.PI / 180)
                End If
            End If

            velX = Math.Cos(angle) * speed
            velY = Math.Sin(angle) * speed

            PlayWithCooldown("bounce", 100)

        End If

    End Sub

    Private Sub HandleWallCollisions()

        ' Vertical bounce (always)
        If ballPos.Y <= 0 Then
            ballPos.Y = 0
            velY = Math.Abs(velY)
            PlayWithCooldown("bounce", 100)

        ElseIf ballPos.Y >= ClientSize.Height - ballDiameter Then
            ballPos.Y = ClientSize.Height - ballDiameter
            velY = -Math.Abs(velY)
            PlayWithCooldown("bounce", 100)
        End If

        ' Horizontal bounce only on Start Screen
        If currentState = GameState.StartScreen OrElse currentState = GameState.EndScreen Then

            If ballPos.X <= 0 Then
                ballPos.X = 0
                velX = Math.Abs(velX)
                PlayWithCooldown("bounce", 100)

            ElseIf ballPos.X >= ClientSize.Width - ballDiameter Then
                ballPos.X = ClientSize.Width - ballDiameter
                velX = -Math.Abs(velX)
                PlayWithCooldown("bounce", 100)

            End If

        End If

    End Sub

    Private Sub CheckScore()
        If scoreLeft >= 10 Then
            winnerText = "Left Player Wins!"
            currentState = GameState.EndScreen
            EndMatch()
            Return
        End If

        If scoreRight >= 10 Then
            winnerText = "Right Player Wins!"
            currentState = GameState.EndScreen
            EndMatch()
            Return
        End If

        If ballPos.X <= 0 Then
            scoreRight += 1
            AudioPlayer.PlaySound("point")
            ResetBall(1) ' Reset ball to the right side
            ResetPaddles() ' Reset paddles to their initial positions
        ElseIf ballPos.X >= ClientSize.Width - ballDiameter Then
            scoreLeft += 1
            AudioPlayer.PlaySound("point")
            ResetBall(-1) ' Reset ball to the left side
            ResetPaddles() ' Reset paddles to their initial positions
        End If
    End Sub

    Private Sub ResetPaddles()

        ' Center the paddles vertically in the middle of the client area
        paddleLeft.Y = (ClientSize.Height - paddleHeight) / 2
        'paddleLeft.Y = (ClientSize.Height - paddleLeft.Height) / 2


        paddleRight.Y = (ClientSize.Height - paddleHeight) / 2
        'paddleRight.Y = (ClientSize.Height - paddleRight.Height) / 2

    End Sub

    Private Sub EndMatch()
        speed = 200


        CenterBall()
        MoveBallRandom()

        AudioPlayer.PauseSound("loop")
        AudioPlayer.LoopSound("start")
    End Sub


    Private Sub CenterBall()

        ' Center the ball in the middle of the client area.
        ballPos = New PointF((ClientSize.Width - ballDiameter) / 2,
                            (ClientSize.Height - ballDiameter) / 2)

    End Sub


    Private Sub MoveBallRandom()

        ' Move the ball in a random direction with a fixed speed.
        Dim rnd As New Random()
        Dim angle As Double = rnd.NextDouble() * Math.PI * 2
        velX = Math.Cos(angle) * speed
        velY = Math.Sin(angle) * speed

    End Sub





    Private Sub ResetBall(direction As Integer)

        '' Center the ball in the middle of the screen
        'ballPos = New PointF((ClientSize.Width - ballDiameter) / 2,
        '                     (ClientSize.Height - ballDiameter) / 2)

        CenterBall()

        'Dim rnd As New Random()

        '' Random angle between -30 and +30 degrees (in radians) 
        'Dim angle As Double = rnd.NextDouble() * (Math.PI / 3) - (Math.PI / 6)
        'velX = Math.Cos(angle) * speed * direction
        'velY = Math.Sin(angle) * speed

        ServeBall(direction)

        trail.Clear()

    End Sub


    Private Sub ServeBall(direction As Integer)

        Dim rnd As New Random()

        ' Random angle between -30 and +30 degrees (in radians) 
        Dim angle As Double = rnd.NextDouble() * (Math.PI / 3) - (Math.PI / 6)
        velX = Math.Cos(angle) * speed * direction
        velY = Math.Sin(angle) * speed

    End Sub


    Public Sub PlayWithCooldown(name As String, ms As Integer)

        Dim now = Environment.TickCount
        If lastPlay.ContainsKey(name) AndAlso now - lastPlay(name) < ms Then Return
        lastPlay(name) = now
        AudioPlayer.PlayOverlapping(name)

    End Sub

    ' -------------------------------
    '  Trail Update
    ' -------------------------------
    Private Sub UpdateTrail()
        trail.Add(New PointF(ballPos.X, ballPos.Y))

        If trail.Count > trailLength Then
            trail.RemoveAt(0)
        End If
    End Sub

    ' -------------------------------
    '  Rendering
    ' -------------------------------
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g = e.Graphics
        g.CompositingMode = CompositingMode.SourceOver
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit

        'DrawTrail(g)
        'DrawBall(g)

        Select Case currentState
            Case GameState.StartScreen
                DrawTrail(g)
                DrawBall(g)

                DrawStartScreen(g)
            Case GameState.Playing
                DrawTrail(g)
                DrawBall(g)
                DrawPaddles(g)
                DrawHUD(g)
            Case GameState.EndScreen
                DrawTrail(g)
                DrawBall(g)

                DrawGameOver(g)

            Case GameState.Pause
                DrawTrail(g)
                DrawBall(g)
                DrawPaddles(g)
                DrawHUD(g)
                DrawPauseScreen(g)

        End Select

    End Sub

    Private Sub DrawPauseScreen(g As Graphics)
        Dim font As New Font("Segoe UI", CSng(ClientSize.Height / 20), FontStyle.Bold)
        Dim text As String = "PAUSED"
        Dim size = g.MeasureString(text, font)

        Using b As New SolidBrush(Color.White)
            g.DrawString(text, font, b,
                     CSng((ClientSize.Width - size.Width) / 2),
                     CSng(ClientSize.Height * 0.4))
        End Using
    End Sub


    Private Sub DrawTrail(g As Graphics)
        Dim count As Integer = Math.Min(trail.Count, trailLength)

        For i As Integer = 0 To count - 1
            Dim p As PointF = trail(i)
            Dim size As Integer = trailSizes(i)
            Dim offset As Single = trailOffsets(i)

            g.FillEllipse(trailBrushes(i),
                          p.X + offset,
                          p.Y + offset,
                          size,
                          size)
        Next
    End Sub

    Private Sub DrawBall(g As Graphics)
        g.FillEllipse(ballBrush,
                      ballPos.X,
                      ballPos.Y,
                      ballDiameter,
                      ballDiameter)
    End Sub

    Private Sub DrawPaddles(g As Graphics)
        g.FillRectangle(paddleBrush, paddleLeft)
        g.FillRectangle(paddleBrush, paddleRight)
    End Sub

    Private Sub DrawHUD(g As Graphics)
        UpdateFPS()

        g.DrawString($"FPS: {fps}", fpsFont, fpsBrush, 10, 10)

        Dim scoreFont As New Font("Segoe UI", CSng(ClientSize.Height / 12), FontStyle.Bold)

        Dim scoreText As String = $"{scoreLeft}   {scoreRight}"
        Dim size = g.MeasureString(scoreText, scoreFont)
        g.DrawString(scoreText, scoreFont, fpsBrush,
                     CSng((ClientSize.Width - size.Width) / 2),
                     CSng(10))
    End Sub

    Private Sub DrawStartScreen(g As Graphics)

        Dim titleFont As New Font("Segoe UI", CSng(ClientSize.Height / 10), FontStyle.Bold)
        Dim menuFont As New Font("Segoe UI", CSng(ClientSize.Height / 30), FontStyle.Regular)
        Dim infoFont As New Font("Segoe UI", CSng(ClientSize.Height / 35), FontStyle.Regular)

        Dim title As String = "PONG"
        Dim titleSize = g.MeasureString(title, titleFont)

        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

        Using tb As New SolidBrush(titleColor)
            g.DrawString(title, titleFont, tb,
                         CSng((ClientSize.Width - titleSize.Width) / 2),
                         CSng(ClientSize.Height * 0.15))
        End Using

        ' Menu options
        Dim option1 As String = "1 Player"
        Dim option2 As String = "2 Players"

        Dim opt1Size = g.MeasureString(option1, menuFont)
        Dim opt2Size = g.MeasureString(option2, menuFont)

        Dim opt1Color As Color = If(selectedOption = 0,
                                    Color.FromArgb(255, 255, 255),
                                    Color.FromArgb(120, 120, 120))

        Dim opt2Color As Color = If(selectedOption = 1,
                                    Color.FromArgb(255, 255, 255),
                                    Color.FromArgb(120, 120, 120))

        Using b1 As New SolidBrush(opt1Color)
            g.DrawString(option1, menuFont, b1,
                         CSng((ClientSize.Width - opt1Size.Width) / 2),
                         CSng(ClientSize.Height * 0.45))
        End Using

        Using b2 As New SolidBrush(opt2Color)
            g.DrawString(option2, menuFont, b2,
                         CSng((ClientSize.Width - opt2Size.Width) / 2),
                         CSng(ClientSize.Height * 0.55))
        End Using

        ' Blink "Press SPACE"
        If blinkVisible Then
            Dim info As String = "Press SPACE to Start"
            Dim infoSize = g.MeasureString(info, infoFont)

            Using ib As New SolidBrush(Color.White)
                g.DrawString(info, infoFont, ib,
                             CSng((ClientSize.Width - infoSize.Width) / 2),
                             CSng(ClientSize.Height * 0.75))
            End Using
        End If
    End Sub

    Private Sub DrawGameOver(g As Graphics)
        Dim font As New Font("Segoe UI", CSng(ClientSize.Height / 20), FontStyle.Bold)

        Dim infoFont As New Font("Segoe UI", CSng(ClientSize.Height / 35), FontStyle.Regular)

        Dim size = g.MeasureString(winnerText, font)
        Dim info As String = "Press SPACE to Restart"
        Dim infoSize = g.MeasureString(info, infoFont)

        Using b As New SolidBrush(Color.White)
            g.DrawString(winnerText, font, b,
                         CSng((ClientSize.Width - size.Width) / 2),
                         CSng(ClientSize.Height * 0.3))

            g.DrawString(info, infoFont, b,
                         CSng((ClientSize.Width - infoSize.Width) / 2),
                         CSng(ClientSize.Height * 0.55))
        End Using
    End Sub


    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' Suppress background flicker
    End Sub

    ' -------------------------------
    '  FPS Counter
    ' -------------------------------
    Private Sub UpdateFPS()
        frameCount += 1

        If fpsTimer.ElapsedMilliseconds >= 1000 Then
            fps = frameCount
            frameCount = 0
            fpsTimer.Restart()
        End If
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)

        If Me.WindowState = FormWindowState.Minimized Then Return
        If trailSizes Is Nothing OrElse trailOffsets Is Nothing Then Return


        ballDiameter = CInt(ClientSize.Height / 20)

        trailLength = CInt(ClientSize.Height / 50)

        If ballPos.Y > ClientSize.Height - ballDiameter Then
            ballPos.Y = ClientSize.Height - ballDiameter
        End If

        'For i As Integer = 0 To trailLength - 1
        '    Dim size As Integer = trailSizes(i)
        '    trailOffsets(i) = CSng((ballDiameter - size) / 2)
        'Next

        paddleHeight = ClientSize.Height / 8
        paddleWidth = ClientSize.Height / 25

        paddleLeft.Height = paddleHeight
        paddleLeft.Width = paddleWidth

        paddleRight.Height = paddleHeight
        paddleRight.Width = paddleWidth



        'paddleLeft.Height = ClientSize.Height / 8
        'paddleLeft.Width = ClientSize.Height / 25


        'paddleRight.Height = ClientSize.Height / 8
        'paddleRight.Width = ClientSize.Height / 25


        paddleLeft.X = ClientSize.Height / 25
        paddleRight.X = ClientSize.Width - ClientSize.Height / 25 - paddleWidth

        'paddleLeft.Y = ClientSize.Height / 2 - paddleLeft.Height / 2


        ResetPaddles()

        CenterBall()

        InitTrails()


        trail.Clear()



        Invalidate()
    End Sub

    ' -------------------------------
    '  Input
    ' -------------------------------
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        ' -------------------------------
        '  Start Screen Input
        ' -------------------------------
        If currentState = GameState.StartScreen Then

            If e.KeyCode = Keys.Up Then
                selectedOption = 0
            ElseIf e.KeyCode = Keys.Down Then
                selectedOption = 1
            ElseIf e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.Enter Then
                playerMode = If(selectedOption = 0, 1, 2)
                StartNewMatch()
            End If

            Return
        End If

        If currentState = GameState.EndScreen Then
            If e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.Enter Then
                currentState = GameState.StartScreen
                winnerText = ""
            End If
            Return
        End If


        ' -------------------------------
        '  Gameplay Input
        ' -------------------------------
        If e.KeyCode = Keys.W Then moveUpLeft = True
        If e.KeyCode = Keys.S Then moveDownLeft = True

        If playerMode = 2 Then
            If e.KeyCode = Keys.Up Then moveUpRight = True
            If e.KeyCode = Keys.Down Then moveDownRight = True
        End If


        If e.KeyCode = Keys.P Then
            If currentState = GameState.Playing Then
                currentState = GameState.Pause
                physicsTimer.Stop()
                Invalidate()
            ElseIf currentState = GameState.Pause Then
                currentState = GameState.Playing
                physicsTimer.Start()
            End If
        End If




    End Sub

    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
        MyBase.OnKeyUp(e)

        If e.KeyCode = Keys.W Then moveUpLeft = False
        If e.KeyCode = Keys.S Then moveDownLeft = False

        If playerMode = 2 Then
            If e.KeyCode = Keys.Up Then moveUpRight = False
            If e.KeyCode = Keys.Down Then moveDownRight = False
        End If
    End Sub

    Private Sub StartNewMatch()

        MovePointerOffScreen()

        speed = 800

        scoreLeft = 0
        scoreRight = 0
        winnerText = ""
        currentState = GameState.Playing

        AudioPlayer.PauseSound("start")
        AudioPlayer.LoopSound("loop")

        ResetBall(If(New Random().Next(0, 2) = 0, -1, 1))
    End Sub

    ' -------------------------------
    '  Cleanup
    ' -------------------------------
    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ballBrush?.Dispose()
        fpsBrush?.Dispose()
        fpsFont?.Dispose()
        paddleBrush?.Dispose()
        physicsTimer?.Dispose()

        If trailBrushes IsNot Nothing Then
            For Each b In trailBrushes
                b?.Dispose()
            Next
        End If

        AudioPlayer.CloseAll()
    End Sub

    Private Sub CreateSoundFiles()
        'Dim FilePath As String = Path.Combine(Application.StartupPath, "loop.mp3")
        CreateFileFromResource(Path.Combine(Application.StartupPath, "loop.mp3"), My.Resources.Resource1.BB_MegaLoop)

        'FilePath = Path.Combine(Application.StartupPath, "bounce.mp3")
        CreateFileFromResource(Path.Combine(Application.StartupPath, "bounce.mp3"), My.Resources.Resource1.Bounce)

        'FilePath = Path.Combine(Application.StartupPath, "start.mp3")
        CreateFileFromResource(Path.Combine(Application.StartupPath, "start.mp3"), My.Resources.Resource1.Start_loop)

        'FilePath = Path.Combine(Application.StartupPath, "point.mp3")
        CreateFileFromResource(Path.Combine(Application.StartupPath, "point.mp3"), My.Resources.Resource1.hit3)

    End Sub

    Private Sub CreateFileFromResource(filepath As String, resource As Byte())
        Try
            If Not IO.File.Exists(filepath) Then
                IO.File.WriteAllBytes(filepath, resource)
            End If
        Catch ex As Exception
            Debug.Print($"Error creating file: {ex.Message}")
        End Try
    End Sub

    Private Sub MovePointerOffScreen()
        ' Move mouse pointer off screen.

        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)

    End Sub

    Private Sub MovePointerCenterScreen()
        ' Move mouse pointer center screen.

        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right \ 2,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)

    End Sub


End Class