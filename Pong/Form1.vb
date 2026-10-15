' PONG – Code with Joe 
' A modern, full‑screen remake of the classic Pong arcade game featuring
' smooth physics, glowing motion trails, dynamic paddle spin, AI or two‑player 
' mode, animated menus, and immersive sound effects. Built with VB.NET and 
' GDI+, the game delivers a polished retro experience with responsive controls
' and crisp visuals.  
' 
' GitHub Repo: https://github.com/JoeLumbley/Pong


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


Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Reflection.Emit
Imports System.Reflection.Metadata
Imports System.Runtime.InteropServices
Imports Microsoft.Win32
Imports Pong.Enums

Public Class Form1


    '' -------------------------------
    ''  Ball / Physics
    '' -------------------------------
    'Private ballPos As PointF
    'Private ballDiameter As Integer = 60

    'Private velX As Double
    'Private velY As Double
    'Private speed As Double = 200

    'Private physicsTimer As New Timer()
    'Private physicsStopwatch As New Stopwatch()


    '' -------------------------------
    ''  Pong State
    '' -------------------------------
    'Private paddleLeft As Rectangle
    'Private paddleRight As Rectangle

    'Private paddleWidth As Integer = 32
    'Private paddleHeight As Integer = 128
    'Private paddleSpeed As Integer = 700

    'Public moveLeftPaddleUp As Boolean
    'Public moveLeftPaddleDown As Boolean
    'Public moveRightPaddleUp As Boolean
    'Public moveRightPaddleDown As Boolean

    'Private scoreLeft As Integer = 0
    'Private scoreRight As Integer = 0

    '' -------------------------------
    ''  Paddle Velocity (Spin)
    '' -------------------------------
    'Private lastPaddleLeftY As Single
    'Private paddleLeftVelocity As Single
    'Private lastPaddleRightY As Single
    'Private paddleRightVelocity As Single

    '' -------------------------------
    ''  Player Names
    '' -------------------------------
    'Private leftPlayerName As String = "Left"
    'Private rightPlayerName As String = "Right"

    '' -------------------------------
    ''  AI Difficulty
    '' -------------------------------
    'Private aiDifficulty As Double = 1.0   ' 1.0 = normal
    'Private aiModeFactor As Double = 1.0         ' 1.0 = normal


    ' -------------------------------
    '  Random
    ' -------------------------------
    Private rng As New Random()


    'Private model As New GameModel(Me.ClientSize)



    Private WithEvents AudioRestartTimer As Timer

    'Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

    '<DllImport("dwmapi.dll")>
    'Private Shared Function DwmSetWindowAttribute(
    '    hWnd As IntPtr,
    '    attr As Integer,
    '    ByRef attrValue As Integer,
    '    attrSize As Integer
    ') As Integer
    'End Function


    'Private hdSize As New Size(1280, 720)
    'Private fhdSize As New Size(1920, 1080)

    Private model As New GameModel(ClientSize)

    Private window As New WindowManager()

    Private settings As New SettingsManager()

    Private audio As New AudioController

    Private match As New MatchManager

    Public renderer As New Rendering()

    Private physics As New PhysicsEngine(model, audio, renderer, Me, match, window)

    'Private state As New GameStateManager(model, audio, renderer, Me, physics)

    Private input As New InputManager(model)


    Private resource As New ResourceManager()






    ' ===============================
    '  FORM LIFECYCLE
    ' ===============================

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)


        'model = New GameModel(Me.ClientSize)

        'window = New WindowManager(Me, model)


        'audio = New AudioController()


        'physics = New PhysicsEngine(model, audio, renderer, Me, match, window)


        'match = New MatchManager()

        'renderer = New Rendering(Me, model, audio, window)

        'physics = New PhysicsEngine(model, audio, renderer, Me, match, window)

        'state = New GameStateManager(model, audio, renderer, Me, physics)

        'input = New InputManager(model, state)






        renderer.InitGraphics(Me, model)
        'renderer.InitTrails()
        'renderer.

        InitWindow()
        'InitPhysicsTimer()
        InitGameplay()
        InitAudio()
        InitBall()








        window.MovePointerCenterScreen()

    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        'ScaleBallDiameter()
        Me.WindowState = FormWindowState.Maximized
    End Sub


    ' ===============================
    '  PHYSICS LOOP
    ' ===============================

    'Private Sub PhysicsTick(sender As Object, e As EventArgs)
    '    If Me.WindowState = FormWindowState.Minimized Then Return

    '    Dim dt As Double = physicsStopwatch.Elapsed.TotalSeconds
    '    physicsStopwatch.Restart()
    '    dt = Math.Min(dt, 0.05)

    '    ' Ball movement
    '    ballPos.X += CSng(velX * dt)
    '    ballPos.Y += CSng(velY * dt)
    '    renderer.UpdateBallPosition(ballPos)

    '    HandleWallCollisions()
    '    renderer.UpdateTrail()

    '    Select Case state.GetCurrentState()
    '        Case GameState.StartScreen
    '            renderer.UpdateStartScreenFX()

    '        Case GameState.Playing
    '            UpdatePaddles(dt)

    '            If settings.GetPlayerMode() = 1 Then
    '                UpdateAI(dt)
    '            End If

    '            HandlePaddleCollisions()
    '            CheckScore()

    '            ' Track paddle velocities for spin
    '            paddleLeftVelocity = paddleLeft.Y - lastPaddleLeftY
    '            lastPaddleLeftY = paddleLeft.Y

    '            paddleRightVelocity = paddleRight.Y - lastPaddleRightY
    '            lastPaddleRightY = paddleRight.Y

    '        Case GameState.AIDifficulty
    '            renderer.UpdateStartScreenFX()

    '        Case GameState.Pause
    '            ' No updates needed for Pause screen

    '        Case GameState.EndScreen
    '            renderer.UpdateStartScreenFX()

    '    End Select


    '    Invalidate()

    'End Sub


    ' ===============================
    '  RENDERING
    ' ===============================

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        renderer.UpdateFPS()
        renderer.Render(e.Graphics, model.State, settings, match, Me, model)

    End Sub

    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' Suppress background painting to avoid flicker
    End Sub

    ' ===============================
    '  INPUT HANDLING
    ' ===============================
    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        input.OnKeyDown(e, Me, settings, match, audio, window, renderer, physics, model)

    End Sub

    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
        MyBase.OnKeyUp(e)

        input.OnKeyUp(e, settings)

    End Sub

    Private Sub Form1_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick

        input.OnMouseClick(e, settings, Me, match, audio, renderer, window, physics, model)

    End Sub

    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp

        input.OnMouseUp()

    End Sub

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove

        input.OnMouseMove(e, renderer, settings, Me, audio)

    End Sub

    Private Sub Form1_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel

        input.OnMouseWheel(e, settings, Me, audio)

    End Sub

    ' ===============================
    '  AUDIO RESTART
    ' ===============================

    Private Sub AudioRestartTimer_Tick(sender As Object, e As EventArgs) Handles AudioRestartTimer.Tick
        'RestartAudioEngine()
        audio.RestartAudioEngine(model)
    End Sub

    Private Sub InvalidateForm()
        Invalidate()
    End Sub


    ' ===============================
    '  RESIZE / SCALING
    ' ===============================

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)

        If Me.WindowState = FormWindowState.Minimized Then Return


        renderer.OnResize(Me, model, physics)

        'renderer.UpdateFormState(Me.FormBorderStyle, ClientSize)

        'model.ClientSize = ClientSize
        '' -------------------------------
        '' Scale ball + speed
        '' -------------------------------
        'ScaleBallDiameter()
        ''renderer.UpdateBallDiameter(ballDiameter)
        'ScaleBallSpeed4State()
        'ScalePaddleSpeed()

        ' -------------------------------
        ' Scale paddles
        ' -------------------------------
        'paddleHeight = ClientSize.Height / 8
        'paddleWidth = ClientSize.Height / 25



        'Dim paddleHeight = ClientSize.Height / 8
        'Dim paddleWidth = ClientSize.Height / 25
        'Dim paddleLeft As Rectangle = model.PaddleLeft
        'Dim paddleRight As Rectangle = model.PaddleHeight


        'renderer.RescalePaddles(ClientSize)

        'model.PaddleHeight = model.ClientSize.Height / 8
        'model.PaddleWidth = model.ClientSize.Height / 25



        'paddleRight.Height = paddleHeight
        'paddleRight.Width = paddleWidth

        'model.PaddleLeft.X = model.ClientSize.Height / 25

        'model.LeftPaddleRect = New Rectangle(model.ClientSize.Height / 25,
        '                                (model.ClientSize.Height - model.PaddleHeight) / 2,
        '                                 model.PaddleWidth,
        '                                 model.PaddleHeight)







        'model.RightPaddleRect = New Rectangle(model.ClientSize.Width - model.ClientSize.Height / 25 - model.PaddleWidth,
        '                                 (model.ClientSize.Height - model.PaddleHeight) / 2,
        '                                 model.PaddleWidth,
        '                                 model.PaddleHeight)

        'renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y, ClientSize)

        'physics.ResetPaddles()
        'physics.CenterBall()

        '' -------------------------------
        '' Preserve ball direction
        '' -------------------------------
        'If model.State = GameState.Playing OrElse model.State = GameState.Pause Then
        '    physics.ServeBall(If(rng.Next(0, 2) = 0, -1, 1))
        'Else
        '    physics.MoveBallRandom()
        'End If

        ' -------------------------------
        ' Clear trail safely (no resizing)
        ' -------------------------------
        'trail.Clear()

        'renderer.UpdateBallPosition(ballPos)

        'renderer.UpdateTrail(model)

        'renderer.ClearTrail()

        '' -------------------------------
        '' Rescale fonts
        '' -------------------------------
        'renderer.RescaleFonts(Me)

        '' -------------------------------
        '' AI difficulty scaling
        '' -------------------------------
        'model.AIDifficultyScale = ClientSize.Height / 1080.0





        Invalidate()

    End Sub

    ' ===============================
    '  CLEANUP / FILES
    ' ===============================

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing


        AudioRestartTimer?.Stop()
        physics.StopTimer()

        'physics.Dispose()
        AudioRestartTimer?.Dispose()


        audio?.DisposeAudio()
        audio = Nothing


    End Sub

    Private Sub InitWindow()
        'Me.Text = "PONG - Code with Joe"

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer, True)

        Me.DoubleBuffered = True
        'Me.BackColor = Color.Black

        'Dim dark As Boolean = window.IsDarkMode()

        '' Apply Windows 11 dark title bar
        'window.ApplyDarkTitleBar(dark, Me.Handle)

        'Me.StartPosition = FormStartPosition.CenterScreen
        'Me.MinimumSize = New Size(256, 256)
        'Me.Size = New Size(1280, 720)

        'Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
        'Dim centerX As Integer = (screenBounds.Width - Me.Width) \ 2
        'Dim centerY As Integer = (screenBounds.Height - Me.Height) \ 2
        'Me.Location = New Point(centerX, centerY)

        'Me.WindowState = FormWindowState.Normal

        window.InitWindow(Me)
    End Sub

    'Private Sub InitPhysicsTimer()

    '    physicsTimer.Interval = 15
    '    AddHandler physicsTimer.Tick, AddressOf PhysicsTick

    '    physicsTimer.Start()

    '    physicsStopwatch.Start()


    'End Sub

    Private Sub InitGameplay()
        'InitPaddles()
        'renderer.InitPaddles(ClientSize)
        'InitPhysics()
        physics.Start()
    End Sub

    Private Sub InitPaddles()
        model.LeftPaddleRect = New Rectangle(50,
                                  (ClientSize.Height - model.PaddleHeight) / 2,
                                   model.PaddleWidth,
                                   model.PaddleHeight)

        model.RightPaddleRect = New Rectangle(ClientSize.Width - 50 - model.PaddleWidth,
                                   (ClientSize.Height - model.PaddleHeight) / 2,
                                    model.PaddleWidth,
                                    model.PaddleHeight)
    End Sub

    'Private Sub InitPhysics()
    '    physicsTimer.Start()
    'End Sub

    Private Sub InitBall()
        renderer.ScaleBall(model)
        physics.CenterBall()
        physics.MoveBallRandom()
    End Sub

    Private Sub InitAudio()

        resource.CreateAudioFilesAsNeeded()

        audio.InitAudio()


        InitAudioRestartTimer()

    End Sub

    Private Sub InitAudioRestartTimer()

        'Restart Audio every 3 minutes
        AudioRestartTimer = New Timer With {
            .Interval = 180000,
            .Enabled = True
        }

        ''Restart Audio every 15 seconds for testing
        'AudioRestartTimer = New Timer With {
        '    .Interval = 15000,
        '    .Enabled = True
        '}

    End Sub

    Private Sub UpdatePaddles(dt As Double)

        Dim paddleSpeed As Double = model.PaddleSpeed
        Dim paddle = model.LeftPaddleRect

        If model.LeftPaddleUp Then paddle.Y -= CSng(paddleSpeed * dt)
        If model.LeftPaddleDown Then paddle.Y += CSng(paddleSpeed * dt)

        model.LeftPaddleRect = paddle




        If model.PlayerMode = 2 Then

            paddle = model.RightPaddleRect


            If model.RightPaddleUp Then paddle.Y -= CSng(paddleSpeed * dt)
            If model.RightPaddleDown Then paddle.Y += CSng(paddleSpeed * dt)

            model.RightPaddleRect = paddle


        End If

        paddle = model.LeftPaddleRect
        paddle.Y = Math.Max(0, Math.Min(ClientSize.Height - model.PaddleHeight, model.LeftPaddleRect.Y))
        model.LeftPaddleRect = paddle


        paddle = model.RightPaddleRect
        paddle.Y = Math.Max(0, Math.Min(ClientSize.Height - model.PaddleHeight, model.RightPaddleRect.Y))
        model.RightPaddleRect = paddle

        'renderer.UpdatePaddlePositions(model.PaddleLeft.Y, model.PaddleRight.Y, ClientSize)

    End Sub


    'Private Sub UpdateAI(dt As Double)
    '    Dim targetY As Single = model.BallPos.Y + model.BallDiameter / 2

    '    ' Difficulty = resolution scaling × difficulty mode
    '    Dim difficultyFactor As Double = model.AIModeFactor * model.AIDifficultyScale

    '    Dim paddleCenter As Single = model.PaddleRight.Y + model.PaddleHeight / 2

    '    Dim paddle = model.PaddleRight
    '    Dim paddleSpeed As Double = model.PaddleSpeed



    '    ' AI logic to move the right paddle towards the ball's Y position 
    '    ' If the ball is above the paddle
    '    If targetY < paddleCenter - 20 Then




    '        ' Move paddle up
    '        paddle.Y -= CSng(paddleSpeed * dt * difficultyFactor)

    '        ' If below, move down. The speed is scaled by the difficulty factor.
    '    ElseIf targetY > paddleCenter + 20 Then
    '        ' Move paddle down
    '        paddle.Y += CSng(paddleSpeed * dt * difficultyFactor)
    '    End If
    '    model.PaddleRight = paddle


    '    paddle = model.PaddleRight
    '    paddle.Y = Math.Max(0, Math.Min(ClientSize.Height - model.PaddleHeight, model.PaddleRight.Y))
    '    model.PaddleRight = paddle

    '    'renderer.UpdatePaddlePositions(model.PaddleLeft.Y, model.PaddleRight.Y, ClientSize)

    'End Sub


    'Private Sub HandlePaddleCollisions()
    '    Dim ballRect As New RectangleF(ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
    '    Dim angle As Double

    '    ' LEFT PADDLE
    '    If ballRect.IntersectsWith(paddleLeft) AndAlso velX < 0 Then
    '        If paddleLeftVelocity < -1 Then
    '            angle = 315 * (Math.PI / 180.0)
    '        ElseIf paddleLeftVelocity > 1 Then
    '            angle = 45 * (Math.PI / 180.0)
    '        Else
    '            angle = 0 * (Math.PI / 180.0)
    '        End If

    '        velX = Math.Cos(angle) * speed
    '        velY = Math.Sin(angle) * speed

    '        Audio.PlayWithCooldown("bounce", 100)
    '    End If

    '    ' RIGHT PADDLE
    '    If ballRect.IntersectsWith(paddleRight) AndAlso velX > 0 Then
    '        If paddleRightVelocity < -1 Then
    '            angle = 225 * (Math.PI / 180.0)
    '        ElseIf paddleRightVelocity > 1 Then
    '            angle = 135 * (Math.PI / 180.0)
    '        Else
    '            If settings.GetPlayerMode = 1 Then
    '                angle = 175 * (Math.PI / 180.0)
    '            Else
    '                angle = 180 * (Math.PI / 180.0)
    '            End If
    '        End If

    '        velX = Math.Cos(angle) * speed
    '        velY = Math.Sin(angle) * speed

    '        Audio.PlayWithCooldown("bounce", 100)
    '    End If
    'End Sub


    'Private Sub HandleWallCollisions()
    '    ' Vertical bounce
    '    If ballPos.Y <= 0 Then
    '        ballPos.Y = 0
    '        renderer.UpdateBallPosition(ballPos)

    '        velY = Math.Abs(velY)
    '        audio.PlayWithCooldown("bounce", 100)

    '    ElseIf ballPos.Y >= ClientSize.Height - ballDiameter Then
    '        ballPos.Y = ClientSize.Height - ballDiameter
    '        renderer.UpdateBallPosition(ballPos)

    '        velY = -Math.Abs(velY)
    '        audio.PlayWithCooldown("bounce", 100)

    '    End If

    '    ' Horizontal bounce only on Start / End / AI Difficulty screens
    '    If state.GetCurrentState() = GameState.StartScreen OrElse
    '       state.GetCurrentState() = GameState.EndScreen OrElse
    '       state.GetCurrentState() = GameState.AIDifficulty Then

    '        If ballPos.X <= 0 Then
    '            ballPos.X = 0
    '            renderer.UpdateBallPosition(ballPos)

    '            velX = Math.Abs(velX)
    '            Audio.PlayWithCooldown("bounce", 100)

    '        ElseIf ballPos.X >= ClientSize.Width - ballDiameter Then
    '            ballPos.X = ClientSize.Width - ballDiameter
    '            renderer.UpdateBallPosition(ballPos)

    '            velX = -Math.Abs(velX)
    '            Audio.PlayWithCooldown("bounce", 100)
    '        End If
    '    End If
    'End Sub


    'Private Sub CheckScore()
    '    If state.GetCurrentState() = GameState.Pause Then Return

    '    If scoreLeft >= 10 Then
    '        CenterBall()
    '        MoveBallRandom()

    '        If leftPlayerName = "You" Then
    '            match.SetWinnerText("You Win!")

    '        Else
    '            match.SetWinnerText($"{leftPlayerName} Wins!")

    '        End If

    '        state.SetCurrentState(GameState.EndScreen)
    '        EndMatch()
    '        Return
    '    End If

    '    If scoreRight >= 10 Then
    '        CenterBall()
    '        MoveBallRandom()

    '        match.SetWinnerText($"{rightPlayerName} Wins!")

    '        state.SetCurrentState(GameState.EndScreen)
    '        EndMatch()
    '        Return
    '    End If

    '    If ballPos.X <= 0 Then
    '        scoreRight += 1
    '        renderer.UpdateScore(scoreLeft, scoreRight, Me.CreateGraphics(), ClientSize)
    '        audio.PlayPoint()

    '        ResetBall(1)
    '        ResetPaddles()

    '    ElseIf ballPos.X >= ClientSize.Width - ballDiameter Then
    '        scoreLeft += 1
    '        renderer.UpdateScore(scoreLeft, scoreRight, Me.CreateGraphics(), ClientSize)
    '        audio.PlayPoint()

    '        ResetBall(-1)
    '        ResetPaddles()

    '    End If
    'End Sub


    'Public Sub ResetPaddles()
    '    paddleLeft.Y = (ClientSize.Height - paddleHeight) / 2
    '    paddleRight.Y = (ClientSize.Height - paddleHeight) / 2
    '    renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y, ClientSize)
    'End Sub

    'Public Sub EndMatch()

    '    Audio.FadeOutAndStopGamePlayLoop(600)

    '    window.MovePointerCenterScreen()

    '    speed = 200 * (ClientSize.Height / 1080.0)

    '    CenterBall()
    '    MoveBallRandom()

    '    Audio.PlayStartLoop(600)

    'End Sub

    'Public Sub CenterBall()
    '    ballPos = New PointF((ClientSize.Width - ballDiameter) / 2.0F,
    '                        (ClientSize.Height - ballDiameter) / 2.0F)
    'End Sub

    'Public Sub MoveBallRandom()
    '    Dim angle As Double = rng.NextDouble() * Math.PI * 2.0
    '    velX = Math.Cos(angle) * speed
    '    velY = Math.Sin(angle) * speed
    'End Sub

    'Public Sub ResetBall(direction As Integer)
    '    CenterBall()
    '    ServeBall(direction)
    '    renderer.ClearTrail()

    'End Sub

    'Public Sub ServeBall(direction As Integer)
    '    Dim angle As Double = rng.NextDouble() * (Math.PI / 3.0) - (Math.PI / 6.0)
    '    velX = Math.Cos(angle) * speed * direction
    '    velY = Math.Sin(angle) * speed
    'End Sub

    'Private Sub ScaleBallSpeed4State()

    '    If model.State = GameState.StartScreen OrElse
    '       model.State = GameState.EndScreen OrElse
    '       model.State = GameState.AIDifficulty Then

    '        model.BallSpeed = 200 * (ClientSize.Height / 1080.0)

    '    Else

    '        model.BallSpeed = 800 * (ClientSize.Height / 1080.0)

    '    End If

    'End Sub

    'Private Sub ScalePaddleSpeed()
    '    model.PaddleSpeed = CInt(400 * Math.Sqrt(ClientSize.Height / 1080.0))
    'End Sub

    Private Sub ScaleBallDiameter()
        model.BallDiameter = CInt(ClientSize.Height / 18.0F)
    End Sub

    'Public Sub SetAIModeFactor()
    '    Select Case settings.GetAIDifficultySelection()
    '        Case AIDifficultyLevel.Easy : model.AIModeFactor = 0.5
    '        Case AIDifficultyLevel.Normal : model.AIModeFactor = 0.5599
    '        Case AIDifficultyLevel.Hard : model.AIModeFactor = 0.5999
    '    End Select
    'End Sub

    'Public Sub Quit2StartScreen()

    '    Audio.FadeOutAndStopPausedLoop(600)

    '    model.BallSpeed = 200 * (ClientSize.Height / 1080.0)
    '    model.WinnerText = String.Empty


    '    model.ScoreLeft = 0
    '    model.ScoreRight = 0

    '    renderer.UpdateScore(model.ScoreLeft, model.ScoreRight, Me, model)


    '    physics.CenterBall()
    '    physics.MoveBallRandom()

    '    model.State = GameState.StartScreen

    '    physics.Start()

    '    audio.PlayStartLoop(600)

    'End Sub


    Public Sub QuitGame()

        AudioRestartTimer?.Stop()

        Audio.PlayExitSound()

        Audio.FadeOutAndStopActiveLoops(700)

        physics.StopTimer()

        ' Wait for the sound to finish before closing
        Dim t As New Timer() With {.Interval = 800}
        AddHandler t.Tick,
        Sub()
            t.Stop()
            t.Dispose()

            Me.Close()

        End Sub

        t.Start()

    End Sub

    'Public Sub ToggleFullScreen()

    '    ' Are we in fullscreen mode?
    '    If Me.FormBorderStyle = FormBorderStyle.None Then
    '        ' Yes, we are in fullscreen mode

    '        ' Switch to windowed mode
    '        Me.FormBorderStyle = FormBorderStyle.Sizable

    '        Me.WindowState = FormWindowState.Normal
    '        Me.Size = hdSize

    '        ' Center window
    '        Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
    '        Dim centerX As Integer = (screenBounds.Width - Me.Width) \ 2
    '        Dim centerY As Integer = (screenBounds.Height - Me.Height) \ 2
    '        Me.Location = New Point(centerX, centerY)


    '    Else
    '        ' No, we are NOT in fullscreen

    '        ' Switch to fullscreen mode
    '        Me.FormBorderStyle = FormBorderStyle.None

    '        Me.WindowState = FormWindowState.Normal
    '        Me.Size = hdSize

    '        ' Center window
    '        Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
    '        Dim centerX As Integer = (screenBounds.Width - Me.Width) \ 2
    '        Dim centerY As Integer = (screenBounds.Height - Me.Height) \ 2
    '        Me.Location = New Point(centerX, centerY)

    '        Me.WindowState = FormWindowState.Maximized

    '    End If

    'End Sub

    'Public Sub PauseGame()

    '    physics.StopTimer()

    '    audio.FadeOutAndStopGamePlayLoop(600)

    '    model.LeftPaddleUp = False
    '    model.LeftPaddleDown = False
    '    model.RightPaddleUp = False
    '    model.RightPaddleDown = False

    '    settings.SetPauseMenuSelection(0) ' Resume game

    '    window.MovePointerCenterScreen()

    '    model.State = GameState.Pause

    '    audio.PlayPausedLoop(600)

    'End Sub

    'Public Sub ResumeGame()

    '    audio.FadeOutAndStopPausedLoop(600)

    '    window.MovePointerOffScreen()

    '    model.State = GameState.Playing

    '    physics.Start()

    '    audio.PlayGamePlayLoop(600)

    'End Sub

    'Public Sub StartNewMatch()

    '    audio.FadeOutAndStopStartLoop(600)

    '    audio.FadeOutAndStopPausedLoop(600)

    '    window.MovePointerOffScreen()

    '    model.BallSpeed = 800 * (ClientSize.Height / 1080.0)



    '    model.ScoreLeft = 0
    '    model.ScoreRight = 0

    '    renderer.UpdateScore(model.ScoreLeft, model.ScoreRight, Me, model)

    '    'renderer.UpdateScore(model.ScoreLeft, model.ScoreRight, Me.CreateGraphics(), ClientSize)

    '    If model.PlayerMode = 1 Then

    '        model.LeftPlayerName = "You"
    '        model.RightPlayerName = "CPU"
    '        renderer.UpdatePlayerNames(model.LeftPlayerName, model.RightPlayerName, Me, model)

    '    Else

    '        model.LeftPlayerName = "Left"
    '        model.RightPlayerName = "Right"
    '        renderer.UpdatePlayerNames(model.LeftPlayerName, model.RightPlayerName, Me, model)

    '    End If

    '    physics.ResetPaddles()
    '    physics.CenterBall()
    '    physics.ServeBall(If(rng.Next(0, 2) = 0, -1, 1))

    '    model.State = GameState.Playing

    '    physics.Start()

    '    audio.PlayGamePlayLoop(600)

    'End Sub


    'Public Sub MovePointerOffScreen()
    '    Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right,
    '                                Screen.PrimaryScreen.WorkingArea.Height \ 2)
    'End Sub

    'Public Sub MovePointerCenterScreen()
    '    Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right \ 2,
    '                                Screen.PrimaryScreen.WorkingArea.Height \ 2)
    'End Sub

    'Private Function IsDarkMode() As Boolean
    '    If Environment.OSVersion.Version.Build >= 22000 Then
    '        ' Windows 11+
    '        Return Application.SystemColorMode = SystemColorMode.Dark
    '    Else
    '        ' Windows 10 fallback
    '        Return IsSystemDarkMode_Win10()
    '    End If
    'End Function


    'Private Function IsSystemDarkMode_Win10() As Boolean
    '    Try
    '        Dim key As RegistryKey =
    '        Registry.CurrentUser.OpenSubKey(
    '            "Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")

    '        If key Is Nothing Then Return False

    '        Dim value As Object = key.GetValue("AppsUseLightTheme", 1)
    '        Return CInt(value) = 0
    '    Catch
    '        Return False
    '    End Try
    'End Function


    'Private Sub ApplyDarkTitleBar(isDark As Boolean)
    '    If Environment.OSVersion.Version.Build < 22000 Then Exit Sub ' Only Windows 11+

    '    Dim value As Integer = If(isDark, 1, 0)
    '    DwmSetWindowAttribute(Me.Handle,
    '                          DWMWA_USE_IMMERSIVE_DARK_MODE,
    '                          value,
    '                          Marshal.SizeOf(value))

    'End Sub


End Class
