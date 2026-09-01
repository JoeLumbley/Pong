' PONG – Code with Joe 
' A modern, full‑screen remake of the classic Pong arcade game featuring
' smooth physics, glowing motion trails, dynamic paddle spin, AI or two‑player 
' mode, animated menus, and immersive sound effects. Built with VB.NET and 
' GDI+, the game delivers a polished retro experience with responsive controls
' and crisp visuals.  
' 


' MIT License
' Copyright(c) 2023 Joseph W. Lumbley

' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:

' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.

' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.


Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Diagnostics

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
    '  Ball / Physics
    ' -------------------------------
    Private ballPos As PointF
    Private ballDiameter As Integer = 60

    Private velX As Double
    Private velY As Double
    Private speed As Double = 200

    Private physicsTimer As New Timer()
    Private physicsStopwatch As New Stopwatch()

    ' -------------------------------
    '  FPS Tracking
    ' -------------------------------
    Private frameCount As Integer = 0
    Private fps As Integer = 0
    Private fpsStopwatch As New Stopwatch()

    ' -------------------------------
    '  GDI Resources
    ' -------------------------------
    Private ballBrush As SolidBrush
    Private fpsBrush As SolidBrush
    Private fpsFont As Font
    Private trailBrushes As SolidBrush()
    Private paddleBrush As SolidBrush
    Private playerLabelBrush As SolidBrush
    Private scoreBrush As SolidBrush

    Private whiteBrush As SolidBrush
    Private grayBrush As SolidBrush
    Private dimBrush As SolidBrush

    ' -------------------------------
    '  Trail System
    ' -------------------------------
    Private trail As New List(Of PointF)
    Private trailLength As Integer = 25
    Private trailSizes As Integer()
    Private trailOffsets As Single()
    Private trailAlpha As Integer()

    ' -------------------------------
    '  Audio Cooldown
    ' -------------------------------
    Private lastPlay As New Dictionary(Of String, Integer)

    ' -------------------------------
    '  Pong State
    ' -------------------------------
    Private paddleLeft As RectangleF
    Private paddleRight As RectangleF

    Private paddleWidth As Integer = 32
    Private paddleHeight As Integer = 128
    Private paddleSpeed As Integer = 700

    Private moveLeftPaddleUp As Boolean
    Private moveLeftPaddleDown As Boolean
    Private moveRightPaddleUp As Boolean
    Private moveRightPaddleDown As Boolean

    Private scoreLeft As Integer = 0
    Private scoreRight As Integer = 0

    ' -------------------------------
    '  Start Screen FX
    ' -------------------------------
    Private titleAlpha As Integer = 0
    Private titleFadeIn As Boolean = True
    Private blinkVisible As Boolean = True
    Private blinkStopwatch As New Stopwatch()

    ' -------------------------------
    '  Paddle Velocity (Spin)
    ' -------------------------------
    Private lastPaddleLeftY As Single
    Private paddleLeftVelocity As Single
    Private lastPaddleRightY As Single
    Private paddleRightVelocity As Single

    ' -------------------------------
    '  Pause Menu
    ' -------------------------------
    Private pauseMenuIndex As Integer = 0

    ' -------------------------------
    '  Player Names
    ' -------------------------------
    Private leftPlayerName As String = "Left"
    Private rightPlayerName As String = "Right"

    ' -------------------------------
    '  Input Repeat Guards
    ' -------------------------------
    Private pauseKeyDown As Boolean = False
    Private pKeyDown As Boolean = False
    Private mediaPlayPauseKeyDown As Boolean = False
    Private f11KeyDown As Boolean = False

    ' -------------------------------
    '  AI Difficulty
    ' -------------------------------
    Private aiDifficulty As Double = 1.0   ' 1.0 = normal

    ' -------------------------------
    '  Cached Fonts
    ' -------------------------------
    Private hudScoreFont As Font
    Private hudLabelFont As Font
    Private pauseTitleFont As Font
    Private pauseMenuFont As Font
    Private startTitleFont As Font
    Private startMenuFont As Font
    Private startInfoFont As Font
    Private gameOverFont As Font
    Private gameOverInfoFont As Font

    Private fullscreenIndicatorFont As Font
    Private fullscreenIndicatorBrush As SolidBrush

    'Private fpsFont As Font

    ' -------------------------------
    '  Random
    ' -------------------------------
    Private rng As New Random()

    ' ===============================
    '  FORM LIFECYCLE
    ' ===============================

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        InitWindow()
        InitTimers()
        InitGraphics()
        InitTrails()
        InitGameplay()
        InitAudio()
        InitBall()

        blinkStopwatch.Start()
    End Sub

    Private Sub InitWindow()
        Me.Text = "PONG - Code with Joe"

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer, True)

        Me.DoubleBuffered = True
        Me.BackColor = Color.Black

        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MinimumSize = New Size(256, 256)
        Me.Size = New Size(1280, 720)

        Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
        Dim centerX As Integer = (screenBounds.Width - Me.Width) \ 2
        Dim centerY As Integer = (screenBounds.Height - Me.Height) \ 2
        Me.Location = New Point(centerX, centerY)

        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub InitTimers()
        physicsTimer.Interval = 15
        AddHandler physicsTimer.Tick, AddressOf PhysicsTick

        physicsStopwatch.Start()
        fpsStopwatch.Start()
    End Sub

    Private Sub InitGraphics()
        ballBrush = New SolidBrush(Color.DeepSkyBlue)
        fpsBrush = New SolidBrush(Color.Gray)
        fpsFont = New Font("Segoe UI", 14, FontStyle.Bold)
        paddleBrush = New SolidBrush(Color.White)
        playerLabelBrush = New SolidBrush(Color.Gray)
        scoreBrush = New SolidBrush(Color.White)

        whiteBrush = New SolidBrush(Color.White)
        grayBrush = New SolidBrush(Color.FromArgb(140, 140, 140))
        dimBrush = New SolidBrush(Color.FromArgb(120, 0, 0, 0))
        fullscreenIndicatorBrush = New SolidBrush(Color.FromArgb(120, 255, 255, 255))

        RescaleFonts()
    End Sub

    Private Sub InitTrails()
        trailSizes = New Integer(trailLength - 1) {}
        trailOffsets = New Single(trailLength - 1) {}
        trailAlpha = New Integer(trailLength - 1) {}
        trailBrushes = New SolidBrush(trailLength - 1) {}

        For i As Integer = 0 To trailLength - 1
            Dim size As Integer = ballDiameter - (trailLength - i) * 2
            If size < 10 Then size = 10

            trailSizes(i) = size
            trailOffsets(i) = CSng((ballDiameter - size) / 2)

            Dim t As Double = i / CDbl(trailLength)
            Dim alpha As Integer = CInt(32 * t * t)
            trailAlpha(i) = alpha

            trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
        Next
    End Sub

    Private Sub InitGameplay()
        InitPaddles()
        InitPhysics()
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

    Private Sub InitPhysics()
        physicsTimer.Start()
    End Sub

    Private Sub InitBall()
        ScaleBallDiameter()
        CenterBall()
        MoveBallRandom()
    End Sub

    Private Sub InitAudio()
        CreateSoundFiles()

        AudioPlayer.AddSound("loop", Path.Combine(Application.StartupPath, "loop.mp3"))
        AudioPlayer.SetVolume("loop", 100)

        AudioPlayer.AddSound("point", Path.Combine(Application.StartupPath, "point.mp3"))
        AudioPlayer.SetVolume("point", 500)

        AudioPlayer.AddOverlapping("bounce", Path.Combine(Application.StartupPath, "bounce.mp3"))
        AudioPlayer.SetVolumeOverlapping("bounce", 150)

        AudioPlayer.AddSound("start", Path.Combine(Application.StartupPath, "start.mp3"))
        AudioPlayer.SetVolume("start", 100)
        AudioPlayer.LoopSound("start")

        AudioPlayer.AddOverlapping("arrow_up", Path.Combine(Application.StartupPath, "arrow_up.mp3"))
        AudioPlayer.SetVolumeOverlapping("arrow_up", 200)

        AudioPlayer.AddOverlapping("arrow_down", Path.Combine(Application.StartupPath, "arrow_down.mp3"))
        AudioPlayer.SetVolumeOverlapping("arrow_down", 200)

        AudioPlayer.AddOverlapping("select", Path.Combine(Application.StartupPath, "select.mp3"))
        AudioPlayer.SetVolumeOverlapping("select", 100)

        AudioPlayer.AddSound("pause", Path.Combine(Application.StartupPath, "pause.mp3"))
        AudioPlayer.SetVolume("pause", 800)
    End Sub

    ' ===============================
    '  PHYSICS LOOP
    ' ===============================

    Private Sub PhysicsTick(sender As Object, e As EventArgs)
        If Me.WindowState = FormWindowState.Minimized Then Return

        Dim dt As Double = physicsStopwatch.Elapsed.TotalSeconds
        physicsStopwatch.Restart()
        dt = Math.Min(dt, 0.05)

        ' Ball movement
        ballPos.X += CSng(velX * dt)
        ballPos.Y += CSng(velY * dt)

        HandleWallCollisions()
        UpdateTrail()

        Select Case currentState
            Case GameState.StartScreen
                UpdateStartScreenFX()

            Case GameState.Playing
                UpdatePaddles(dt)

                If playerMode = 1 Then
                    UpdateAI(dt)
                End If

                HandlePaddleCollisions()
                CheckScore()
        End Select

        ' Track paddle velocities for spin
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

        If blinkStopwatch.ElapsedMilliseconds >= 800 Then
            blinkVisible = Not blinkVisible
            blinkStopwatch.Restart()
        End If
    End Sub

    Private Sub UpdatePaddles(dt As Double)
        If moveLeftPaddleUp Then paddleLeft.Y -= CSng(paddleSpeed * dt)
        If moveLeftPaddleDown Then paddleLeft.Y += CSng(paddleSpeed * dt)

        If playerMode = 2 Then
            If moveRightPaddleUp Then paddleRight.Y -= CSng(paddleSpeed * dt)
            If moveRightPaddleDown Then paddleRight.Y += CSng(paddleSpeed * dt)
        End If

        paddleLeft.Y = Math.Max(0, Math.Min(ClientSize.Height - paddleHeight, paddleLeft.Y))
        paddleRight.Y = Math.Max(0, Math.Min(ClientSize.Height - paddleHeight, paddleRight.Y))
    End Sub

    Private Sub UpdateAI(dt As Double)
        Dim targetY As Single = ballPos.Y + ballDiameter / 2
        Dim difficultyFactor As Double = 0.7 * aiDifficulty

        Dim paddleCenter As Single = paddleRight.Y + paddleHeight / 2

        If targetY < paddleCenter Then
            paddleRight.Y -= CSng(paddleSpeed * dt * difficultyFactor)
        ElseIf targetY > paddleCenter Then
            paddleRight.Y += CSng(paddleSpeed * dt * difficultyFactor)
        End If

        paddleRight.Y = Math.Max(0, Math.Min(ClientSize.Height - paddleHeight, paddleRight.Y))
    End Sub

    Private Sub HandlePaddleCollisions()
        Dim ballRect As New RectangleF(ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
        Dim angle As Double

        ' LEFT PADDLE
        If ballRect.IntersectsWith(paddleLeft) AndAlso velX < 0 Then
            If paddleLeftVelocity < -1 Then
                angle = 315 * (Math.PI / 180.0)
            ElseIf paddleLeftVelocity > 1 Then
                angle = 45 * (Math.PI / 180.0)
            Else
                angle = 0 * (Math.PI / 180.0)
            End If

            velX = Math.Cos(angle) * speed
            velY = Math.Sin(angle) * speed

            PlayWithCooldown("bounce", 100)
        End If

        ' RIGHT PADDLE
        If ballRect.IntersectsWith(paddleRight) AndAlso velX > 0 Then
            If paddleRightVelocity < -1 Then
                angle = 225 * (Math.PI / 180.0)
            ElseIf paddleRightVelocity > 1 Then
                angle = 135 * (Math.PI / 180.0)
            Else
                If playerMode = 1 Then
                    angle = 175 * (Math.PI / 180.0)
                Else
                    angle = 180 * (Math.PI / 180.0)
                End If
            End If

            velX = Math.Cos(angle) * speed
            velY = Math.Sin(angle) * speed

            PlayWithCooldown("bounce", 100)
        End If
    End Sub

    Private Sub HandleWallCollisions()
        ' Vertical bounce
        If ballPos.Y <= 0 Then
            ballPos.Y = 0
            velY = Math.Abs(velY)
            PlayWithCooldown("bounce", 100)
        ElseIf ballPos.Y >= ClientSize.Height - ballDiameter Then
            ballPos.Y = ClientSize.Height - ballDiameter
            velY = -Math.Abs(velY)
            PlayWithCooldown("bounce", 100)
        End If

        ' Horizontal bounce only on Start / End screens
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
        If currentState = GameState.Pause Then Return

        If scoreLeft >= 10 Then
            CenterBall()
            MoveBallRandom()

            If leftPlayerName = "You" Then
                winnerText = "You Win!"
            Else
                winnerText = $"{leftPlayerName} Wins!"
            End If

            currentState = GameState.EndScreen
            EndMatch()
            Return
        End If

        If scoreRight >= 10 Then
            CenterBall()
            MoveBallRandom()

            winnerText = $"{rightPlayerName} Wins!"
            currentState = GameState.EndScreen
            EndMatch()
            Return
        End If

        If ballPos.X <= 0 Then
            scoreRight += 1
            AudioPlayer.PlaySound("point")
            ResetBall(1)
            ResetPaddles()
        ElseIf ballPos.X >= ClientSize.Width - ballDiameter Then
            scoreLeft += 1
            AudioPlayer.PlaySound("point")
            ResetBall(-1)
            ResetPaddles()
        End If
    End Sub

    Private Sub ResetPaddles()
        paddleLeft.Y = (ClientSize.Height - paddleHeight) / 2
        paddleRight.Y = (ClientSize.Height - paddleHeight) / 2
    End Sub

    Private Sub EndMatch()
        AudioPlayer.PauseSound("loop")

        speed = 200 * (ClientSize.Height / 1080.0)

        CenterBall()
        MoveBallRandom()

        AudioPlayer.LoopSound("start")
    End Sub

    Private Sub CenterBall()
        ballPos = New PointF((ClientSize.Width - ballDiameter) / 2.0F,
                             (ClientSize.Height - ballDiameter) / 2.0F)
    End Sub

    Private Sub MoveBallRandom()
        Dim angle As Double = rng.NextDouble() * Math.PI * 2.0
        velX = Math.Cos(angle) * speed
        velY = Math.Sin(angle) * speed
    End Sub

    Private Sub ResetBall(direction As Integer)
        CenterBall()
        ServeBall(direction)
        trail.Clear()
    End Sub

    Private Sub ServeBall(direction As Integer)
        Dim angle As Double = rng.NextDouble() * (Math.PI / 3.0) - (Math.PI / 6.0)
        velX = Math.Cos(angle) * speed * direction
        velY = Math.Sin(angle) * speed
    End Sub

    Private Sub PlayWithCooldown(name As String, ms As Integer)
        Dim now As Integer = Environment.TickCount

        If lastPlay.ContainsKey(name) AndAlso now - lastPlay(name) < ms Then
            Return
        End If

        lastPlay(name) = now
        AudioPlayer.PlayOverlapping(name)
    End Sub

    ' ===============================
    '  TRAIL
    ' ===============================

    Private Sub UpdateTrail()
        trail.Add(New PointF(ballPos.X, ballPos.Y))

        If trail.Count > trailLength Then
            trail.RemoveAt(0)
        End If
    End Sub

    ' ===============================
    '  RENDERING
    ' ===============================

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g As Graphics = e.Graphics
        g.CompositingMode = CompositingMode.SourceOver
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

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
        g.FillRectangle(dimBrush, ClientRectangle)

        Dim title As String = "PAUSED"
        Dim titleSize = g.MeasureString(title, pauseTitleFont)

        g.DrawString(title, pauseTitleFont, whiteBrush,
                     CSng((ClientSize.Width - titleSize.Width) / 2.0F),
                     CSng(ClientSize.Height * 0.25F))

        Dim items() As String = {"Resume", "Restart Match", "Quit to Start Screen"}

        For i As Integer = 0 To items.Length - 1
            Dim text = items(i)
            Dim size = g.MeasureString(text, pauseMenuFont)

            Dim brush As SolidBrush = If(i = pauseMenuIndex, whiteBrush, grayBrush)

            g.DrawString(text, pauseMenuFont, brush,
                         CSng((ClientSize.Width - size.Width) / 2.0F),
                         CSng(ClientSize.Height * 0.4F + i * (size.Height + 10)))
        Next
    End Sub

    Private Sub DrawTrail(g As Graphics)
        If trail Is Nothing OrElse
           trailSizes Is Nothing OrElse
           trailOffsets Is Nothing OrElse
           trailBrushes Is Nothing Then
            Exit Sub
        End If

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




        ' -------------------------------
        '  Fullscreen Indicator
        ' -------------------------------
        Dim fsText As String =
        If(Me.FormBorderStyle = FormBorderStyle.None,
           "F11 Exit Fullscreen",
           "F11 Fullscreen")



        Dim fsSize = g.MeasureString(fsText, fullscreenIndicatorFont)

        g.DrawString(fsText,
             fullscreenIndicatorFont,
             grayBrush,
             ClientSize.Width - fsSize.Width - 10,
             10)





        Dim halfWidth As Single = ClientSize.Width / 2.0F

        Dim leftScoreText As String = scoreLeft.ToString()
        Dim rightScoreText As String = scoreRight.ToString()

        Dim leftScoreSize = g.MeasureString(leftScoreText, hudScoreFont)
        Dim rightScoreSize = g.MeasureString(rightScoreText, hudScoreFont)

        Dim leftLabelSize = g.MeasureString(leftPlayerName, hudLabelFont)
        Dim rightLabelSize = g.MeasureString(rightPlayerName, hudLabelFont)

        Dim scoreY As Single = 10
        Dim labelY As Single = scoreY - CSng(ClientSize.Height / 200.0)

        Dim leftScoreX As Single = (halfWidth - leftScoreSize.Width) / 2.0F
        Dim rightScoreX As Single = halfWidth + (halfWidth - rightScoreSize.Width) / 2.0F

        Dim leftLabelX As Single = (halfWidth - leftLabelSize.Width) / 2.0F
        Dim rightLabelX As Single = halfWidth + (halfWidth - rightLabelSize.Width) / 2.0F

        g.DrawString(leftPlayerName, hudLabelFont, playerLabelBrush, leftLabelX, labelY)
        g.DrawString(rightPlayerName, hudLabelFont, playerLabelBrush, rightLabelX, labelY)

        g.DrawString(leftScoreText, hudScoreFont, scoreBrush, leftScoreX, scoreY)
        g.DrawString(rightScoreText, hudScoreFont, scoreBrush, rightScoreX, scoreY)
    End Sub

    Private Sub DrawStartScreen(g As Graphics)
        Dim title As String = "PONG"
        Dim titleSize = g.MeasureString(title, startTitleFont)
        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString(title, startTitleFont, titleBrush,
                         CSng((ClientSize.Width - titleSize.Width) / 2.0F),
                         CSng(ClientSize.Height * 0.15F))
        End Using

        Dim option1 As String = "1 Player"
        Dim option2 As String = "2 Players"

        Dim opt1Size = g.MeasureString(option1, startMenuFont)
        Dim opt2Size = g.MeasureString(option2, startMenuFont)

        Dim opt1Brush As SolidBrush = If(selectedOption = 0, whiteBrush, grayBrush)
        Dim opt2Brush As SolidBrush = If(selectedOption = 1, whiteBrush, grayBrush)

        g.DrawString(option1, startMenuFont, opt1Brush,
                     CSng((ClientSize.Width - opt1Size.Width) / 2.0F),
                     CSng(ClientSize.Height * 0.45F))

        g.DrawString(option2, startMenuFont, opt2Brush,
                     CSng((ClientSize.Width - opt2Size.Width) / 2.0F),
                     CSng(ClientSize.Height * 0.55F))

        If blinkVisible Then
            Dim info As String = "Press SPACE to Start"
            Dim infoSize = g.MeasureString(info, startInfoFont)

            g.DrawString(info, startInfoFont, whiteBrush,
                         CSng((ClientSize.Width - infoSize.Width) / 2.0F),
                         CSng(ClientSize.Height * 0.75F))
        End If
    End Sub

    Private Sub DrawGameOver(g As Graphics)
        Dim size = g.MeasureString(winnerText, gameOverFont)
        Dim info As String = "Press SPACE to Restart"
        Dim infoSize = g.MeasureString(info, gameOverInfoFont)

        g.DrawString(winnerText, gameOverFont, whiteBrush,
                     CSng((ClientSize.Width - size.Width) / 2.0F),
                     CSng(ClientSize.Height * 0.3F))

        g.DrawString(info, gameOverInfoFont, whiteBrush,
                     CSng((ClientSize.Width - infoSize.Width) / 2.0F),
                     CSng(ClientSize.Height * 0.55F))
    End Sub

    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' Suppress background painting to avoid flicker
    End Sub

    ' ===============================
    '  FPS COUNTER
    ' ===============================

    Private Sub UpdateFPS()
        frameCount += 1

        If fpsStopwatch.ElapsedMilliseconds >= 1000 Then
            fps = frameCount
            frameCount = 0
            fpsStopwatch.Restart()
        End If
    End Sub

    ' ===============================
    '  RESIZE / SCALING
    ' ===============================

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)

        If Me.WindowState = FormWindowState.Minimized Then Return
        If trailSizes Is Nothing OrElse trailOffsets Is Nothing Then Return

        ScaleBallDiameter()
        ScaleBallSpeed()
        ScalePaddleSpeed()
        aiDifficulty = ClientSize.Height / 1080.0

        paddleHeight = ClientSize.Height / 8
        paddleWidth = ClientSize.Height / 25

        paddleLeft.Height = paddleHeight
        paddleLeft.Width = paddleWidth
        paddleRight.Height = paddleHeight
        paddleRight.Width = paddleWidth

        paddleLeft.X = ClientSize.Height / 25
        paddleRight.X = ClientSize.Width - ClientSize.Height / 25 - paddleWidth

        ResetPaddles()
        CenterBall()

        If currentState = GameState.Playing Then
            ServeBall(If(velX < 0, -1, 1))
        Else
            MoveBallRandom()
        End If

        Dim newLength As Integer = CInt(ClientSize.Height / 30)
        If newLength < 5 Then newLength = 5

        If newLength <> trailLength Then
            trailLength = newLength

            ReDim Preserve trailSizes(trailLength - 1)
            ReDim Preserve trailOffsets(trailLength - 1)
            ReDim Preserve trailAlpha(trailLength - 1)
            ReDim Preserve trailBrushes(trailLength - 1)

            For i As Integer = 0 To trailLength - 1
                Dim size As Integer = ballDiameter - (trailLength - i) * 2
                If size < 10 Then size = 10

                trailSizes(i) = size
                trailOffsets(i) = CSng((ballDiameter - size) / 2.0F)

                Dim t As Double = i / CDbl(trailLength)
                Dim alpha As Integer = CInt(32 * t * t)
                trailAlpha(i) = alpha

                If trailBrushes(i) Is Nothing Then
                    trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
                Else
                    trailBrushes(i).Color = Color.FromArgb(alpha, 0, 191, 255)
                End If
            Next
        End If

        trail.Clear()
        RescaleFonts()

        Invalidate()
    End Sub

    Private Sub RescaleFonts()
        hudScoreFont = New Font("Segoe UI", CSng(ClientSize.Height / 12.0F), FontStyle.Bold)
        hudLabelFont = New Font("Segoe UI", CSng(ClientSize.Height / 50.0F), FontStyle.Regular)

        pauseTitleFont = New Font("Segoe UI", CSng(ClientSize.Height / 18.0F), FontStyle.Bold)
        pauseMenuFont = New Font("Segoe UI", CSng(ClientSize.Height / 28.0F), FontStyle.Regular)

        startTitleFont = New Font("Segoe UI", CSng(ClientSize.Height / 10.0F), FontStyle.Bold)
        startMenuFont = New Font("Segoe UI", CSng(ClientSize.Height / 30.0F), FontStyle.Regular)
        startInfoFont = New Font("Segoe UI", CSng(ClientSize.Height / 35.0F), FontStyle.Regular)

        gameOverFont = New Font("Segoe UI", CSng(ClientSize.Height / 20.0F), FontStyle.Bold)
        gameOverInfoFont = New Font("Segoe UI", CSng(ClientSize.Height / 35.0F), FontStyle.Regular)

        fullscreenIndicatorFont = New Font("Segoe UI", CSng(ClientSize.Height / 75.0F), FontStyle.Regular)
        fpsFont = New Font("Segoe UI", CSng(ClientSize.Height / 75.0F), FontStyle.Bold)


    End Sub

    Private Sub ScaleBallSpeed()
        If currentState = GameState.StartScreen OrElse currentState = GameState.EndScreen Then
            speed = 200 * (ClientSize.Height / 1080.0)
        Else
            speed = 800 * (ClientSize.Height / 1080.0)
        End If
    End Sub

    Private Sub ScalePaddleSpeed()
        paddleSpeed = CInt(700 * Math.Sqrt(ClientSize.Height / 1080.0))
    End Sub

    Private Sub ScaleBallDiameter()
        ballDiameter = CInt(ClientSize.Height / 18.0F)
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ScaleBallDiameter()
        Me.WindowState = FormWindowState.Maximized
    End Sub

    ' ===============================
    '  INPUT
    ' ===============================

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        If e.KeyCode = Keys.F11 Then
            If f11KeyDown Then Return
            f11KeyDown = True

            ToggleFullScreen()
            Return
        End If

        ' START SCREEN INPUT
        If currentState = GameState.StartScreen Then
            HandleStartScreenInput(e)
            Return
        End If

        ' END SCREEN INPUT
        If currentState = GameState.EndScreen Then
            If e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.Enter Then
                currentState = GameState.StartScreen
                winnerText = ""
            End If
            Return
        End If

        ' GAMEPLAY INPUT
        If currentState = GameState.Playing Then
            HandleGameplayInput(e)
            Return
        End If

        ' PAUSE MENU INPUT
        If currentState = GameState.Pause Then
            HandlePauseInput(e)
            Return
        End If
    End Sub

    Private Sub HandleStartScreenInput(e As KeyEventArgs)
        Select Case e.KeyCode
            Case Keys.Up
                If selectedOption <> 0 Then
                    AudioPlayer.PlayOverlapping("arrow_up")
                    selectedOption = 0
                End If

            Case Keys.Down
                If selectedOption <> 1 Then
                    AudioPlayer.PlayOverlapping("arrow_down")
                    selectedOption = 1
                End If

            Case Keys.D1, Keys.NumPad1
                If selectedOption <> 0 Then
                    AudioPlayer.PlayOverlapping("arrow_up")
                    selectedOption = 0
                End If

            Case Keys.D2, Keys.NumPad2
                If selectedOption <> 1 Then
                    AudioPlayer.PlayOverlapping("arrow_down")
                    selectedOption = 1
                End If

            Case Keys.Space, Keys.Enter
                playerMode = If(selectedOption = 0, 1, 2)
                StartNewMatch()

            Case Keys.Escape
                Me.Close()
        End Select
    End Sub

    Private Sub HandleGameplayInput(e As KeyEventArgs)
        If e.KeyCode = Keys.W Then moveLeftPaddleUp = True
        If e.KeyCode = Keys.S Then moveLeftPaddleDown = True

        If playerMode = 2 Then
            If e.KeyCode = Keys.Up Then moveRightPaddleUp = True
            If e.KeyCode = Keys.Down Then moveRightPaddleDown = True
        End If

        If e.KeyCode = Keys.P Then
            If pKeyDown Then Return
            pKeyDown = True
            PauseGame()
            Return
        End If

        If e.KeyCode = Keys.Pause Then
            If pauseKeyDown Then Return
            pauseKeyDown = True
            PauseGame()
            Return
        End If

        If e.KeyCode = Keys.MediaPlayPause Then
            If mediaPlayPauseKeyDown Then Return
            mediaPlayPauseKeyDown = True
            PauseGame()
            Return
        End If
    End Sub

    Private Sub HandlePauseInput(e As KeyEventArgs)
        If e.KeyCode = Keys.P Then
            If pKeyDown Then Return
            pKeyDown = True
            UnpauseGame()
            Return
        End If

        If e.KeyCode = Keys.Pause Then
            If pauseKeyDown Then Return
            pauseKeyDown = True
            UnpauseGame()
            Return
        End If

        If e.KeyCode = Keys.MediaPlayPause Then
            If mediaPlayPauseKeyDown Then Return
            mediaPlayPauseKeyDown = True
            UnpauseGame()
            Return
        End If

        If e.KeyCode = Keys.Up Then
            If pauseMenuIndex > 0 Then
                pauseMenuIndex = Math.Max(0, pauseMenuIndex - 1)
                AudioPlayer.PlayOverlapping("arrow_up")
                Invalidate()
            End If
            Return
        End If

        If e.KeyCode = Keys.Down Then
            If pauseMenuIndex < 2 Then
                pauseMenuIndex = Math.Min(2, pauseMenuIndex + 1)
                AudioPlayer.PlayOverlapping("arrow_down")
                Invalidate()
            End If
            Return
        End If

        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Space Then
            AudioPlayer.PlayOverlapping("select")

            Select Case pauseMenuIndex
                Case 0
                    UnpauseGame()
                    Return

                Case 1
                    StartNewMatch()
                    Return

                Case 2

                    '' Quit to Start Screen
                    'AudioPlayer.PauseSound("pause")
                    'MovePointerOffScreen()

                    'speed = 200 * (ClientSize.Height / 1080.0)

                    'winnerText = ""
                    'scoreLeft = 0
                    'scoreRight = 0

                    'CenterBall()
                    'MoveBallRandom()

                    'currentState = GameState.StartScreen
                    'physicsTimer.Start()
                    'Invalidate()

                    'AudioPlayer.LoopSound("start")

                    Quit2StartScreen()
                    Return

            End Select
        End If
    End Sub

    Private Sub Quit2StartScreen()
        ' Quit to Start Screen

        AudioPlayer.PauseSound("pause")

        MovePointerOffScreen()

        speed = 200 * (ClientSize.Height / 1080.0)
        winnerText = ""
        scoreLeft = 0
        scoreRight = 0

        CenterBall()
        MoveBallRandom()

        currentState = GameState.StartScreen
        physicsTimer.Start()
        Invalidate()

        AudioPlayer.LoopSound("start")

    End Sub

    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
        MyBase.OnKeyUp(e)

        If e.KeyCode = Keys.W Then moveLeftPaddleUp = False
        If e.KeyCode = Keys.S Then moveLeftPaddleDown = False

        If playerMode = 2 Then
            If e.KeyCode = Keys.Up Then moveRightPaddleUp = False
            If e.KeyCode = Keys.Down Then moveRightPaddleDown = False
        End If

        If e.KeyCode = Keys.P Then
            pKeyDown = False
        End If

        If e.KeyCode = Keys.Pause Then
            pauseKeyDown = False
        End If

        If e.KeyCode = Keys.MediaPlayPause Then
            mediaPlayPauseKeyDown = False
        End If

        If e.KeyCode = Keys.F11 Then
            f11KeyDown = False
        End If

    End Sub

    Private Sub ToggleFullScreen()
        If Me.FormBorderStyle = FormBorderStyle.None Then
            Me.FormBorderStyle = FormBorderStyle.Sizable
            Me.WindowState = FormWindowState.Normal

            Me.Size = New Size(1280, 720)

            Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
            Dim centerX As Integer = (screenBounds.Width - Me.Width) \ 2
            Dim centerY As Integer = (screenBounds.Height - Me.Height) \ 2
            Me.Location = New Point(centerX, centerY)

            Invalidate()
        Else
            Me.FormBorderStyle = FormBorderStyle.None
            Me.WindowState = FormWindowState.Maximized
            Invalidate()
        End If
    End Sub

    Private Sub PauseGame()
        AudioPlayer.PauseSound("loop")
        currentState = GameState.Pause
        physicsTimer.Stop()

        moveLeftPaddleUp = False
        moveLeftPaddleDown = False
        moveRightPaddleUp = False
        moveRightPaddleDown = False

        pauseMenuIndex = 0
        Invalidate()

        AudioPlayer.LoopSound("pause")
    End Sub

    Private Sub UnpauseGame()
        AudioPlayer.PauseSound("pause")
        currentState = GameState.Playing
        physicsTimer.Start()
        Invalidate()

        AudioPlayer.LoopSound("loop")
    End Sub

    Private Sub StartNewMatch()
        AudioPlayer.PauseSound("start")
        AudioPlayer.PauseSound("pause")

        MovePointerOffScreen()

        speed = 800 * (ClientSize.Height / 1080.0)

        scoreLeft = 0
        scoreRight = 0

        If playerMode = 1 Then
            leftPlayerName = "You"
            rightPlayerName = "CPU"
        Else
            leftPlayerName = "Left"
            rightPlayerName = "Right"
        End If

        CenterBall()
        ServeBall(If(rng.Next(0, 2) = 0, -1, 1))

        currentState = GameState.Playing
        physicsTimer.Start()
        Invalidate()

        AudioPlayer.LoopSound("loop")
    End Sub

    ' ===============================
    '  CLEANUP / FILES
    ' ===============================

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        ballBrush?.Dispose()
        fpsBrush?.Dispose()
        fpsFont?.Dispose()
        paddleBrush?.Dispose()
        playerLabelBrush?.Dispose()
        scoreBrush?.Dispose()
        whiteBrush?.Dispose()
        grayBrush?.Dispose()
        dimBrush?.Dispose()

        hudScoreFont?.Dispose()
        hudLabelFont?.Dispose()
        pauseTitleFont?.Dispose()
        pauseMenuFont?.Dispose()
        startTitleFont?.Dispose()
        startMenuFont?.Dispose()
        startInfoFont?.Dispose()
        gameOverFont?.Dispose()
        gameOverInfoFont?.Dispose()
        fullscreenIndicatorFont?.Dispose()

        physicsTimer?.Dispose()

        If trailBrushes IsNot Nothing Then
            For Each b In trailBrushes
                b?.Dispose()
            Next
        End If

        AudioPlayer.CloseAll()
    End Sub

    Private Sub CreateSoundFiles()
        CreateFileFromResource(Path.Combine(Application.StartupPath, "loop.mp3"), My.Resources.Resource1.PlayingLoop)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "bounce.mp3"), My.Resources.Resource1.bounce3)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "start.mp3"), My.Resources.Resource1.Start_loop)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "point.mp3"), My.Resources.Resource1.hit4)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "arrow_up.mp3"), My.Resources.Resource1.ArrowUp2)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "arrow_down.mp3"), My.Resources.Resource1.ArrowDown2)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "select.mp3"), My.Resources.Resource1.Select2)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "pause.mp3"), My.Resources.Resource1.PauseMusic2)
    End Sub

    Private Sub CreateFileFromResource(filepath As String, resource As Byte())
        Try
            If Not File.Exists(filepath) Then
                File.WriteAllBytes(filepath, resource)
            End If
        Catch ex As Exception
            Debug.Print($"Error creating file: {ex.Message}")
        End Try
    End Sub

    Private Sub MovePointerOffScreen()
        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)
    End Sub

    Private Sub MovePointerCenterScreen()
        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right \ 2,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)
    End Sub

End Class
