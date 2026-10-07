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
Imports System.Runtime.InteropServices
Imports Microsoft.Win32

Public Class Form1

    ' -------------------------------
    '  Game State
    ' -------------------------------

    Private Enum GameState
        StartScreen
        Playing
        Pause
        EndScreen
        AIDifficulty
    End Enum


    Private currentState As GameState = GameState.StartScreen
    Private winnerText As String = String.Empty

    ' -------------------------------
    '  Player Mode
    ' -------------------------------
    Private playerMode As Integer = 1       ' 1 = Single Player (AI), 2 = Two Players
    Private numberOfPlayersSelection As Integer = 0   ' 0 = "1 Player", 1 = "2 Players"

    Private Enum NumberOfPlayers
        OnePlayer
        TwoPlayers
    End Enum

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
    '  Audio Cooldown
    ' -------------------------------
    Private lastPlay As New Dictionary(Of String, Integer)

    ' -------------------------------
    '  Pong State
    ' -------------------------------
    Private paddleLeft As Rectangle
    Private paddleRight As Rectangle

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
    '  Paddle Velocity (Spin)
    ' -------------------------------
    Private lastPaddleLeftY As Single
    Private paddleLeftVelocity As Single
    Private lastPaddleRightY As Single
    Private paddleRightVelocity As Single

    ' -------------------------------
    '  Pause Menu
    ' -------------------------------
    Private pauseMenuSelection As Integer = 0



    Private aiDifficultySelection As Integer = 0 ' 0 = Easy, 1 = Normal, 2 = Hard
    Private aiOptions() As String = {"Easy", "Normal", "Hard"}

    Private Enum AIDifficultyLevel
        Easy
        Normal
        Hard
    End Enum

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
    Private fKeyDown As Boolean = False
    Private escapeKeyDown As Boolean = False
    Private spaceKeyDown As Boolean = False
    Private enterKeyDown As Boolean = False
    Private upKeyDown As Boolean = False
    Private downKeyDown As Boolean = False

    Private wKeyDown As Boolean = False
    Private sKeyDown As Boolean = False
    Private ctrlQDown As Boolean = False
    Private ctrlHDown As Boolean = False



    ' -------------------------------
    '  AI Difficulty
    ' -------------------------------
    Private aiDifficulty As Double = 1.0   ' 1.0 = normal
    Private aiModeFactor As Double = 1.0         ' 1.0 = normal


    ' -------------------------------
    '  Random
    ' -------------------------------
    Private rng As New Random()

    Private Audio As AudioPlayer



    Private WithEvents AudioRestartTimer As Timer


    Private gameplayLoopVolume As Integer = 50
    Private startLoopVolume As Integer = 75
    Private pauseLoopVolume As Integer = 40


    Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmSetWindowAttribute(
        hWnd As IntPtr,
        attr As Integer,
        ByRef attrValue As Integer,
        attrSize As Integer
    ) As Integer
    End Function

    Private showKeyboardHints As Boolean = True


    Private hdSize As New Size(1280, 720)
    Private fhdSize As New Size(1920, 1080)


    Private mouseIsClicking As Boolean = False


    Private renderer As New Rendering(Me.CreateGraphics(), Me.ClientSize)



    Private startScreenScrollAccum As Integer = 0
    Private Const ScrollThreshold As Integer = 400

    Private aiDifficultyScrollAccum As Integer = 0


    Private pauseScrollAccum As Integer = 0

    ' ===============================
    '  FORM LIFECYCLE
    ' ===============================

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        InitWindow()
        InitTimers()
        InitGameplay()
        InitAudio()
        InitBall()

        MovePointerCenterScreen()

    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ScaleBallDiameter()
        Me.WindowState = FormWindowState.Maximized
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
        renderer.UpdateBallPosition(ballPos)

        HandleWallCollisions()
        'UpdateTrail()
        renderer.UpdateTrail()
        Select Case currentState
            Case GameState.StartScreen
                'UpdateStartScreenFX()
                renderer.UpdateStartScreenFX()


            Case GameState.Playing
                UpdatePaddles(dt)

                If playerMode = 1 Then
                    UpdateAI(dt)
                End If

                HandlePaddleCollisions()
                CheckScore()

            Case GameState.AIDifficulty
                'UpdateStartScreenFX()
                renderer.UpdateStartScreenFX()


            Case GameState.Pause
                ' No updates needed for Pause screen


            Case GameState.EndScreen
                'UpdateStartScreenFX()
                renderer.UpdateStartScreenFX()


        End Select

        ' Track paddle velocities for spin
        paddleLeftVelocity = paddleLeft.Y - lastPaddleLeftY
        lastPaddleLeftY = paddleLeft.Y

        paddleRightVelocity = paddleRight.Y - lastPaddleRightY
        lastPaddleRightY = paddleRight.Y

        Invalidate()
    End Sub


    ' ===============================
    '  RENDERING
    ' ===============================


    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)


        renderer.UpdateFormState(Me.FormBorderStyle, Me.ClientSize)
        renderer.UpdateFPS()
        renderer.Render(e.Graphics, currentState, showKeyboardHints)
    End Sub


    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' Suppress background painting to avoid flicker
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        ' ============================================================
        ' 1. Fullscreen Toggle (F11 / F)
        ' ============================================================
        If e.KeyCode = Keys.F11 OrElse e.KeyCode = Keys.F Then

            ' Repeat‑guard
            If (e.KeyCode = Keys.F11 AndAlso f11KeyDown) OrElse
           (e.KeyCode = Keys.F AndAlso fKeyDown) Then Return

            ' Mark correct key as down
            If e.KeyCode = Keys.F11 Then
                f11KeyDown = True
            Else
                fKeyDown = True
            End If

            PlayFullScreenSound()
            ToggleFullScreen()
            Invalidate()
            Return
        End If


        ' ============================================================
        ' 2. Escape pressed while fullscreen (exit fullscreen)
        ' ============================================================
        If Me.FormBorderStyle = FormBorderStyle.None AndAlso
       e.KeyCode = Keys.Escape Then

            If escapeKeyDown Then Return
            escapeKeyDown = True

            PlayFullScreenSound()
            ToggleFullScreen()
            Invalidate()
            Return
        End If


        ' ============================================================
        ' 3. Quit Game (Ctrl + Q)
        ' ============================================================
        If e.Control AndAlso e.KeyCode = Keys.Q Then

            If ctrlQDown Then Return
            ctrlQDown = True

            QuitGame()
            Return
        End If


        ' ============================================================
        ' 4. Keyboard Hints Toggle (Ctrl + H)
        ' ============================================================
        If e.Control AndAlso e.KeyCode = Keys.H Then

            If ctrlHDown Then Return
            ctrlHDown = True

            showKeyboardHints = Not showKeyboardHints
            renderer.ToggleKeyboardHints()

            Invalidate()

            Return
        End If


        ' ============================================================
        ' 5. State‑based Input Dispatch
        ' ============================================================
        Select Case currentState

            Case GameState.StartScreen
                HandleStartScreenInput(e)
                Return

            Case GameState.EndScreen
                HandleEndScreenInput(e)
                Return

            Case GameState.Playing
                HandleGameplayInput(e)
                Return

            Case GameState.Pause
                HandlePauseInput(e)
                Return

            Case GameState.AIDifficulty
                HandleAIDifficultyInput(e)
                Return

        End Select

    End Sub



    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
        MyBase.OnKeyUp(e)

        ' ============================================================
        ' 1. Release Paddle Movement Keys
        ' ============================================================
        If e.KeyCode = Keys.W Then
            moveLeftPaddleUp = False
            wKeyDown = False
        End If

        If e.KeyCode = Keys.S Then
            moveLeftPaddleDown = False
            sKeyDown = False
        End If

        If playerMode = 2 Then
            If e.KeyCode = Keys.Up Then
                moveRightPaddleUp = False
                upKeyDown = False
            End If

            If e.KeyCode = Keys.Down Then
                moveRightPaddleDown = False
                downKeyDown = False
            End If
        End If


        ' ============================================================
        ' 2. Release Pause / Resume Keys
        ' ============================================================
        If e.KeyCode = Keys.P Then pKeyDown = False
        If e.KeyCode = Keys.Pause Then pauseKeyDown = False
        If e.KeyCode = Keys.MediaPlayPause Then mediaPlayPauseKeyDown = False


        ' ============================================================
        ' 3. Release Fullscreen Toggle Keys
        ' ============================================================
        If e.KeyCode = Keys.F11 Then f11KeyDown = False
        If e.KeyCode = Keys.F Then fKeyDown = False


        ' ============================================================
        ' 4. Release Escape Key
        ' ============================================================
        If e.KeyCode = Keys.Escape Then escapeKeyDown = False


        ' ============================================================
        ' 5. Release Confirm Keys (Enter / Space)
        ' ============================================================
        If e.KeyCode = Keys.Enter Then enterKeyDown = False
        If e.KeyCode = Keys.Space Then spaceKeyDown = False


        ' ============================================================
        ' 6. Release Menu Navigation Keys (Up / Down / W / S)
        ' ============================================================
        If e.KeyCode = Keys.Up Then upKeyDown = False
        If e.KeyCode = Keys.Down Then downKeyDown = False

        If e.KeyCode = Keys.W Then wKeyDown = False
        If e.KeyCode = Keys.S Then sKeyDown = False


        ' ============================================================
        ' 7. Release Quit Game Key (Ctrl + Q)
        ' ============================================================
        If e.KeyCode = Keys.Q Then ctrlQDown = False
        If e.KeyCode = Keys.ControlKey Then ctrlQDown = False


        ' ============================================================
        ' 8. Release Keyboard Hints Key (Ctrl + H)
        ' ============================================================
        If e.KeyCode = Keys.H Then ctrlHDown = False
        If e.KeyCode = Keys.ControlKey Then ctrlHDown = False

    End Sub



    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        StartScreen_MouseMove(e)
        AIDifficulty_MouseMove(e)
        PauseMenu_MouseMove(e)
    End Sub

    Private Sub Form1_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick

        StartScreen_MouseClick(e)
        AIDifficulty_MouseClick(e)
        PauseMenu_MouseClick(e)
        EndScreen_MouseClick(e)


    End Sub



    Private Sub Form1_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel

        If currentState = GameState.Pause Then
            HandlePauseMouseWheel(e.Delta)
            Return
        End If

        If currentState = GameState.StartScreen Then
            HandleStartScreenMouseWheel(e.Delta)
            Return
        End If

        If currentState = GameState.AIDifficulty Then
            HandleAIDifficultyMouseWheel(e.Delta)
            Return
        End If

    End Sub


    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp

        mouseIsClicking = False

    End Sub


    ' ===============================
    '  AUDIO RESTART
    ' ===============================

    Private Sub AudioRestartTimer_Tick(sender As Object, e As EventArgs) Handles AudioRestartTimer.Tick
        RestartAudioEngine()
    End Sub


    ' ===============================
    '  RESIZE / SCALING
    ' ===============================


    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)

        If Me.WindowState = FormWindowState.Minimized Then Return

        renderer.UpdateFormState(Me.FormBorderStyle, ClientSize)


        ' -------------------------------
        ' Scale ball + speed
        ' -------------------------------
        ScaleBallDiameter()
        renderer.UpdateBallDiameter(ballDiameter)
        ScaleBallSpeed4State()
        ScalePaddleSpeed()

        ' -------------------------------
        ' Scale paddles
        ' -------------------------------
        paddleHeight = ClientSize.Height / 8
        paddleWidth = ClientSize.Height / 25

        paddleLeft.Height = paddleHeight
        paddleLeft.Width = paddleWidth
        paddleRight.Height = paddleHeight
        paddleRight.Width = paddleWidth

        paddleLeft.X = ClientSize.Height / 25
        paddleRight.X = ClientSize.Width - ClientSize.Height / 25 - paddleWidth

        renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y, ClientSize)

        ResetPaddles()
        CenterBall()

        ' -------------------------------
        ' Preserve ball direction
        ' -------------------------------
        If currentState = GameState.Playing OrElse currentState = GameState.Pause Then
            ServeBall(If(rng.Next(0, 2) = 0, -1, 1))
        Else
            MoveBallRandom()
        End If

        ' -------------------------------
        ' Clear trail safely (no resizing)
        ' -------------------------------
        'trail.Clear()

        renderer.UpdateBallPosition(ballPos)

        renderer.UpdateTrail()

        renderer.ClearTrail()

        ' -------------------------------
        ' Rescale fonts
        ' -------------------------------
        renderer.RescaleFonts(Me.CreateGraphics(), ClientSize)

        ' -------------------------------
        ' AI difficulty scaling
        ' -------------------------------
        aiDifficulty = ClientSize.Height / 1080.0

        Invalidate()
    End Sub

    ' ===============================
    '  CLEANUP / FILES
    ' ===============================

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        'Audio.CloseAll()

        AudioRestartTimer?.Stop()
        physicsTimer?.Stop()


        'ballBrush?.Dispose()
        'fpsBrush?.Dispose()
        'fpsFont?.Dispose()
        'paddleBrush?.Dispose()
        'playerLabelBrush?.Dispose()
        'scoreBrush?.Dispose()
        'whiteBrush?.Dispose()
        'grayBrush?.Dispose()
        'dimBrush?.Dispose()

        'If trailBrushes IsNot Nothing Then
        '    For Each b In trailBrushes
        '        b?.Dispose()
        '    Next
        'End If

        'hudScoreFont?.Dispose()
        'hudLabelFont?.Dispose()
        'pauseTitleFont?.Dispose()
        'pauseMenuFont?.Dispose()
        'startTitleFont?.Dispose()
        'aiDifficultyTitleFont?.Dispose()
        'startMenuFont?.Dispose()
        'startInfoFont?.Dispose()
        'gameOverFont?.Dispose()
        'gameOverInfoFont?.Dispose()
        'fullscreenIndicatorFont?.Dispose()

        physicsTimer?.Dispose()
        AudioRestartTimer?.Dispose()


        Audio?.Dispose()
        Audio = Nothing


    End Sub

    Private Sub InitWindow()
        Me.Text = "PONG - Code with Joe"

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.UserPaint Or
                    ControlStyles.OptimizedDoubleBuffer, True)

        Me.DoubleBuffered = True
        Me.BackColor = Color.Black

        Dim dark As Boolean = IsDarkMode()

        ' Apply Windows 11 dark title bar
        ApplyDarkTitleBar(dark)

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


    End Sub


    Private Sub InitGameplay()
        InitPaddles()
        InitPhysics()
    End Sub

    Private Sub InitPaddles()
        paddleLeft = New Rectangle(50,
                                    (ClientSize.Height - paddleHeight) / 2,
                                    paddleWidth,
                                    paddleHeight)

        paddleRight = New Rectangle(ClientSize.Width - 50 - paddleWidth,
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

        Audio = New AudioPlayer()

        CreateSoundFiles()

        LoadAndRegisterSounds()

        PlayStartLoop(600)

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

    Private Sub RestartAudioEngine()

        FadeOutAndStopActiveLoops(600)

        If Audio.IsPlaying("bounce") Then
            Audio.FadeOutAndStop("bounce", 600)
        End If

        ' Wait for fade-out to complete before restarting engine
        Dim t As New Timer() With {.Interval = 700}

        AddHandler t.Tick, Sub()
                               t.Stop()
                               t.Dispose()

                               ' Dispose old engine
                               Audio?.Dispose()

                               ' Create new engine
                               Audio = New AudioPlayer()

                               ' Reload all sounds
                               LoadAndRegisterSounds()

                               ' Restore loops based on game state
                               RestartLoops()
                           End Sub

        t.Start()

    End Sub

    Private Sub LoadAndRegisterSounds()

        ' ---------------------------------------------------------
        ' Overlapping SFX
        ' ---------------------------------------------------------
        Audio.AddOverlapping("bounce", Path.Combine(Application.StartupPath, "bounce.mp3"))
        Audio.SetVolumeOverlapping("bounce", 200)

        Audio.AddOverlapping("arrow_up", Path.Combine(Application.StartupPath, "arrow_up.mp3"))
        Audio.SetVolumeOverlapping("arrow_up", 250)

        Audio.AddOverlapping("arrow_down", Path.Combine(Application.StartupPath, "arrow_down.mp3"))
        Audio.SetVolumeOverlapping("arrow_down", 200)

        ' ---------------------------------------------------------
        ' Single‑instance SFX
        ' ---------------------------------------------------------
        Audio.AddSound("fullscreen", Path.Combine(Application.StartupPath, "fullscreen.mp3"))
        Audio.SetVolume("fullscreen", 150)

        Audio.AddSound("select", Path.Combine(Application.StartupPath, "select.mp3"))
        Audio.SetVolume("select", 300)

        Audio.AddSound("point", Path.Combine(Application.StartupPath, "point.mp3"))
        Audio.SetVolume("point", 500)

        Audio.AddSound("exit", Path.Combine(Application.StartupPath, "exit.mp3"))
        Audio.SetVolume("exit", 300)

        ' ---------------------------------------------------------
        ' Loops (Start, Gameplay, Pause)
        ' ---------------------------------------------------------
        Audio.AddSound("startloop", Path.Combine(Application.StartupPath, "startloop.mp3"))
        Audio.SetVolume("startloop", startLoopVolume)

        Audio.AddSound("gameplayloop", Path.Combine(Application.StartupPath, "gameplayloop.mp3"))
        Audio.SetVolume("gameplayloop", gameplayLoopVolume)

        Audio.AddSound("pause", Path.Combine(Application.StartupPath, "pause.mp3"))
        Audio.SetVolume("pause", pauseLoopVolume)


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

        renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y, ClientSize)

    End Sub


    Private Sub UpdateAI(dt As Double)
        Dim targetY As Single = ballPos.Y + ballDiameter / 2

        ' Difficulty = resolution scaling × difficulty mode
        Dim difficultyFactor As Double = aiModeFactor * aiDifficulty

        Dim paddleCenter As Single = paddleRight.Y + paddleHeight / 2

        ' AI logic to move the right paddle towards the ball's Y position 
        ' If the ball is above the paddle
        If targetY < paddleCenter - 20 Then
            ' Move paddle up
            paddleRight.Y -= CSng(paddleSpeed * dt * difficultyFactor)

            ' If below, move down. The speed is scaled by the difficulty factor.
        ElseIf targetY > paddleCenter + 20 Then
            ' Move paddle down
            paddleRight.Y += CSng(paddleSpeed * dt * difficultyFactor)
        End If

        paddleRight.Y = Math.Max(0, Math.Min(ClientSize.Height - paddleHeight, paddleRight.Y))

        renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y, ClientSize)

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
            renderer.UpdateBallPosition(ballPos)

            velY = Math.Abs(velY)
            PlayWithCooldown("bounce", 100)

        ElseIf ballPos.Y >= ClientSize.Height - ballDiameter Then
            ballPos.Y = ClientSize.Height - ballDiameter
            renderer.UpdateBallPosition(ballPos)

            velY = -Math.Abs(velY)
            PlayWithCooldown("bounce", 100)
        End If

        ' Horizontal bounce only on Start / End / AI Difficulty screens
        If currentState = GameState.StartScreen OrElse
           currentState = GameState.EndScreen OrElse
           currentState = GameState.AIDifficulty Then

            If ballPos.X <= 0 Then
                ballPos.X = 0
                renderer.UpdateBallPosition(ballPos)

                velX = Math.Abs(velX)
                PlayWithCooldown("bounce", 100)

            ElseIf ballPos.X >= ClientSize.Width - ballDiameter Then
                ballPos.X = ClientSize.Width - ballDiameter
                renderer.UpdateBallPosition(ballPos)

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
                renderer.SetWinnerText(winnerText)

            Else
                winnerText = $"{leftPlayerName} Wins!"
                renderer.SetWinnerText(winnerText)

            End If

            currentState = GameState.EndScreen
            EndMatch()
            Return
        End If

        If scoreRight >= 10 Then
            CenterBall()
            MoveBallRandom()

            winnerText = $"{rightPlayerName} Wins!"
            renderer.SetWinnerText(winnerText)

            currentState = GameState.EndScreen
            EndMatch()
            Return
        End If

        If ballPos.X <= 0 Then
            scoreRight += 1
            renderer.UpdateScore(scoreLeft, scoreRight, Me.CreateGraphics(), ClientSize)
            PlayPoint()
            ResetBall(1)
            ResetPaddles()
            renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y, ClientSize)

        ElseIf ballPos.X >= ClientSize.Width - ballDiameter Then
            scoreLeft += 1
            renderer.UpdateScore(scoreLeft, scoreRight, Me.CreateGraphics(), ClientSize)
            PlayPoint()
            ResetBall(-1)
            ResetPaddles()
            renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y, ClientSize)

        End If
    End Sub

    Private Sub ResetPaddles()
        paddleLeft.Y = (ClientSize.Height - paddleHeight) / 2
        paddleRight.Y = (ClientSize.Height - paddleHeight) / 2
        renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y, ClientSize)
    End Sub

    Private Sub EndMatch()

        FadeOutAndStopGamePlayLoop(600)

        MovePointerCenterScreen()

        speed = 200 * (ClientSize.Height / 1080.0)

        CenterBall()
        MoveBallRandom()

        PlayStartLoop(600)

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
        'trail.Clear()
        renderer.ClearTrail()

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
        Audio.PlayOverlapping(name)
    End Sub


    Private Sub ScaleBallSpeed4State()

        If currentState = GameState.StartScreen OrElse
           currentState = GameState.EndScreen OrElse
           currentState = GameState.AIDifficulty Then

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

    Private Sub HandleAIDifficultyInput(e As KeyEventArgs)

        ' -------------------------------
        '  AI Difficulty Menu Navigation
        ' -------------------------------

        ' Menu Up (Arrow Up or W)
        If (e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.W) Then

            If (e.KeyCode = Keys.Up AndAlso upKeyDown) OrElse
               (e.KeyCode = Keys.W AndAlso wKeyDown) Then Return

            If e.KeyCode = Keys.Up Then upKeyDown = True Else wKeyDown = True

            If aiDifficultySelection > 0 Then
                aiDifficultySelection -= 1
                renderer.SetAIDifficultySelection(aiDifficultySelection)

                PlayMenuUpSound()
                Invalidate()
            End If

            Return

        End If

        ' Menu Down (Arrow Down or S)
        If (e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.S) Then

            If (e.KeyCode = Keys.Down AndAlso downKeyDown) OrElse
               (e.KeyCode = Keys.S AndAlso sKeyDown) Then Return

            If e.KeyCode = Keys.Down Then downKeyDown = True Else sKeyDown = True

            If aiDifficultySelection < aiOptions.Length - 1 Then
                aiDifficultySelection += 1
                renderer.SetAIDifficultySelection(aiDifficultySelection)

                PlayMenuDownSound()
                Invalidate()
            End If

            Return

        End If


        ' ============================
        '   DIRECT SELECT: E / N / H / 1 / 2 / 3
        ' ============================

        Dim directSelectDifficulty As Nullable(Of AIDifficultyLevel) = Nothing

        Select Case e.KeyCode
            Case Keys.E, Keys.D1, Keys.NumPad1
                directSelectDifficulty = AIDifficultyLevel.Easy

            Case Keys.N, Keys.D2, Keys.NumPad2
                directSelectDifficulty = AIDifficultyLevel.Normal

            Case Keys.H, Keys.D3, Keys.NumPad3
                directSelectDifficulty = AIDifficultyLevel.Hard
        End Select

        If directSelectDifficulty.HasValue Then

            aiDifficultySelection = directSelectDifficulty.Value
            renderer.SetAIDifficultySelection(aiDifficultySelection)

            ' Update AI mode factor
            SetAIModeFactor()

            ' Start match immediately
            currentState = GameState.Playing
            StartNewMatch()

            PlaySelectSound()
            Invalidate()

            Return
        End If


        ' ============================
        '   SELECT: SPACE / ENTER
        ' ============================
        If (e.KeyCode = Keys.Space AndAlso Not spaceKeyDown) OrElse
           (e.KeyCode = Keys.Enter AndAlso Not enterKeyDown) Then
            If e.KeyCode = Keys.Space Then
                spaceKeyDown = True
            Else
                enterKeyDown = True
            End If

            SetAIModeFactor()

            currentState = GameState.Playing
            StartNewMatch()
            PlaySelectSound()
            Invalidate()

            Return

        End If

        ' ============================
        '   ESCAPE → RETURN TO START
        ' ============================

        If e.KeyCode = Keys.Escape AndAlso Not escapeKeyDown Then
            escapeKeyDown = True

            currentState = GameState.StartScreen
            PlaySelectSound()
            Invalidate()

            Return

        End If

    End Sub

    Private Sub SetAIModeFactor()
        Select Case aiDifficultySelection
            Case AIDifficultyLevel.Easy : aiModeFactor = 0.6
            Case AIDifficultyLevel.Normal : aiModeFactor = 0.63
            Case AIDifficultyLevel.Hard : aiModeFactor = 0.7
        End Select
    End Sub


    Private Sub HandleEndScreenInput(e As KeyEventArgs)

        ' ============================================================
        ' 1. Return to Start Screen (Space)
        ' ============================================================
        If e.KeyCode = Keys.Space Then
            If spaceKeyDown Then Return
            spaceKeyDown = True

            PlaySelectSound()
            currentState = GameState.StartScreen
            winnerText = ""
            renderer.SetWinnerText(winnerText)

            Invalidate()
            Return
        End If


        ' ============================================================
        ' 2. Return to Start Screen (Enter)
        ' ============================================================
        If e.KeyCode = Keys.Enter Then
            If enterKeyDown Then Return
            enterKeyDown = True

            PlaySelectSound()
            currentState = GameState.StartScreen
            winnerText = ""
            renderer.SetWinnerText(winnerText)

            Invalidate()
            Return
        End If


        ' ============================================================
        ' 3. Return to Start Screen (Escape)
        ' ============================================================
        If e.KeyCode = Keys.Escape Then
            If escapeKeyDown Then Return
            escapeKeyDown = True

            PlaySelectSound()
            currentState = GameState.StartScreen
            winnerText = ""
            renderer.SetWinnerText(winnerText)

            Invalidate()
            Return
        End If

    End Sub


    Private Sub HandleStartScreenInput(e As KeyEventArgs)

        ' ============================================================
        ' 1. Menu Navigation (Up/W and Down/S)
        ' ============================================================
        Select Case e.KeyCode

        ' ------------------------------
        '  Up / W   --   ↑ Menu Up ↑
        ' ------------------------------
            Case Keys.Up
                If upKeyDown Then Return
                upKeyDown = True

                If numberOfPlayersSelection <> NumberOfPlayers.OnePlayer Then
                    numberOfPlayersSelection = NumberOfPlayers.OnePlayer
                    renderer.SetStartMenuSelection(numberOfPlayersSelection)

                    PlayMenuUpSound()
                    Invalidate()
                End If
                Return

            Case Keys.W
                If wKeyDown Then Return
                wKeyDown = True

                If numberOfPlayersSelection <> NumberOfPlayers.OnePlayer Then
                    numberOfPlayersSelection = NumberOfPlayers.OnePlayer
                    renderer.SetStartMenuSelection(numberOfPlayersSelection)

                    PlayMenuUpSound()
                    Invalidate()
                End If
                Return


        ' ------------------------------
        '  Down / S   --   ↓ Menu Down ↓
        ' ------------------------------
            Case Keys.Down
                If downKeyDown Then Return
                downKeyDown = True

                If numberOfPlayersSelection <> NumberOfPlayers.TwoPlayers Then
                    numberOfPlayersSelection = NumberOfPlayers.TwoPlayers
                    renderer.SetStartMenuSelection(numberOfPlayersSelection)

                    PlayMenuDownSound()
                    Invalidate()
                End If
                Return

            Case Keys.S
                If sKeyDown Then Return
                sKeyDown = True

                If numberOfPlayersSelection <> NumberOfPlayers.TwoPlayers Then
                    numberOfPlayersSelection = NumberOfPlayers.TwoPlayers
                    renderer.SetStartMenuSelection(numberOfPlayersSelection)

                    PlayMenuDownSound()
                    Invalidate()
                End If
                Return


        ' ============================================================
        ' 2. Direct Selection via Number Keys (1 or 2)
        ' ============================================================
            Case Keys.D1, Keys.NumPad1
                numberOfPlayersSelection = NumberOfPlayers.OnePlayer
                renderer.SetStartMenuSelection(numberOfPlayersSelection)

                playerMode = 1
                currentState = GameState.AIDifficulty

                PlaySelectSound()
                Invalidate()
                Return

            Case Keys.D2, Keys.NumPad2
                numberOfPlayersSelection = NumberOfPlayers.TwoPlayers
                renderer.SetStartMenuSelection(numberOfPlayersSelection)

                playerMode = 2
                StartNewMatch()

                PlaySelectSound()
                Invalidate()
                Return


        ' ============================================================
        ' 3. Confirm Selection (Space / Enter)
        ' ============================================================
            Case Keys.Space
                If spaceKeyDown Then Return
                spaceKeyDown = True

                If numberOfPlayersSelection = NumberOfPlayers.OnePlayer Then
                    playerMode = 1
                    currentState = GameState.AIDifficulty
                Else
                    playerMode = 2
                    StartNewMatch()
                End If

                PlaySelectSound()
                Invalidate()
                Return

            Case Keys.Enter
                If enterKeyDown Then Return
                enterKeyDown = True

                If numberOfPlayersSelection = NumberOfPlayers.OnePlayer Then
                    playerMode = 1
                    currentState = GameState.AIDifficulty
                Else
                    playerMode = 2
                    StartNewMatch()
                End If

                PlaySelectSound()
                Invalidate()
                Return


        ' ============================================================
        ' 4. Escape (Exit Game)
        ' ============================================================
            Case Keys.Escape
                If escapeKeyDown Then Return
                escapeKeyDown = True

                QuitGame()
                Return

        End Select

    End Sub


    Private Sub HandleGameplayInput(e As KeyEventArgs)

        ' ============================================================
        ' 1. Paddle Movement (Left Player: W/S)
        ' ============================================================
        If e.KeyCode = Keys.W Then
            moveLeftPaddleUp = True
        End If

        If e.KeyCode = Keys.S Then
            moveLeftPaddleDown = True
        End If


        ' ============================================================
        ' 2. Paddle Movement (Right Player: Up/Down in 2‑Player mode)
        ' ============================================================
        If playerMode = 2 Then
            If e.KeyCode = Keys.Up Then
                moveRightPaddleUp = True
            End If

            If e.KeyCode = Keys.Down Then
                moveRightPaddleDown = True
            End If
        End If


        ' ============================================================
        ' 3. Pause Game (P, Pause/Break, MediaPlayPause)
        ' ============================================================
        If e.KeyCode = Keys.P Then
            If pKeyDown Then Return
            pKeyDown = True

            PlaySelectSound()
            PauseGame()
            Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Pause Then
            If pauseKeyDown Then Return
            pauseKeyDown = True

            PlaySelectSound()
            PauseGame()
            Invalidate()
            Return
        End If

        If e.KeyCode = Keys.MediaPlayPause Then
            If mediaPlayPauseKeyDown Then Return
            mediaPlayPauseKeyDown = True

            PlaySelectSound()
            PauseGame()
            Invalidate()
            Return
        End If


        ' ============================================================
        ' 4. Escape (Pause only when windowed)
        ' ============================================================
        If Me.FormBorderStyle <> FormBorderStyle.None AndAlso
           e.KeyCode = Keys.Escape Then
            If escapeKeyDown Then Return
            escapeKeyDown = True

            PlaySelectSound()
            PauseGame()
            Invalidate()
            Return
        End If


    End Sub

    Private Sub HandlePauseInput(e As KeyEventArgs)

        ' ============================================================
        ' 1. Resume Game (P, Pause/Break, MediaPlayPause)
        ' ============================================================
        If e.KeyCode = Keys.P Then
            If pKeyDown Then Return
            pKeyDown = True

            ResumeGame()
            PlaySelectSound()

            Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Pause Then
            If pauseKeyDown Then Return
            pauseKeyDown = True

            ResumeGame()
            PlaySelectSound()

            Invalidate()
            Return
        End If

        If e.KeyCode = Keys.MediaPlayPause Then
            If mediaPlayPauseKeyDown Then Return
            mediaPlayPauseKeyDown = True

            ResumeGame()
            PlaySelectSound()

            Invalidate()
            Return
        End If


        ' ============================================================
        ' 2. Pause Menu Navigation (Up/W and Down/S)
        ' ============================================================
        If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.W Then
            If pauseMenuSelection > 0 Then
                pauseMenuSelection = Math.Max(0, pauseMenuSelection - 1)
                renderer.SetPauseMenuSelection(pauseMenuSelection)

                PlayMenuUpSound()
                Invalidate()
            End If
            Return
        End If

        If e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.S Then
            If pauseMenuSelection < 2 Then
                pauseMenuSelection = Math.Min(2, pauseMenuSelection + 1)
                renderer.SetPauseMenuSelection(pauseMenuSelection)

                PlayMenuDownSound()
                Invalidate()
            End If
            Return
        End If


        ' ============================================================
        ' 3. Direct Hotkeys (R = Resume, N = New Match, Q = Quit)
        ' ============================================================
        If e.KeyCode = Keys.R Then
            ResumeGame()
            PlaySelectSound()

            Invalidate()
            Return
        End If

        If e.KeyCode = Keys.N Then
            StartNewMatch()
            PlaySelectSound()

            Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Q Then
            Quit2StartScreen()
            PlaySelectSound()
            Invalidate()
            Return
        End If


        ' ============================================================
        ' 4. Escape Key (Quit to Start Screen)
        ' ============================================================
        If e.KeyCode = Keys.Escape Then
            If escapeKeyDown Then Return
            escapeKeyDown = True

            Quit2StartScreen()
            PlaySelectSound()
            Invalidate()
            Return
        End If


        ' ============================================================
        ' 5. Confirm Selection (Enter / Space)
        ' ============================================================
        If e.KeyCode = Keys.Enter Then
            If enterKeyDown Then Return
            enterKeyDown = True

            Select Case pauseMenuSelection
                Case 0 : ResumeGame()
                Case 1 : StartNewMatch()
                Case 2 : Quit2StartScreen()
            End Select
            PlaySelectSound()
            Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Space Then
            If spaceKeyDown Then Return
            spaceKeyDown = True

            Select Case pauseMenuSelection
                Case 0 : ResumeGame()
                Case 1 : StartNewMatch()
                Case 2 : Quit2StartScreen()
            End Select
            PlaySelectSound()
            Invalidate()
            Return
        End If

    End Sub

    Private Sub Quit2StartScreen()

        FadeOutAndStopPausedLoop(600)

        'MovePointerCenterScreen()

        speed = 200 * (ClientSize.Height / 1080.0)
        winnerText = ""
        renderer.SetWinnerText(winnerText)

        scoreLeft = 0
        scoreRight = 0

        CenterBall()
        MoveBallRandom()

        currentState = GameState.StartScreen
        physicsTimer.Start()

        PlayStartLoop(600)

    End Sub


    Private Sub QuitGame()

        AudioRestartTimer?.Stop()

        PlayExitSound()

        FadeOutAndStopActiveLoops(700)

        physicsTimer?.Stop()

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

    Private Sub ToggleFullScreen()

        ' Are we in fullscreen mode?
        If Me.FormBorderStyle = FormBorderStyle.None Then
            ' Yes, we are in fullscreen mode

            ' Switch to windowed mode
            Me.FormBorderStyle = FormBorderStyle.Sizable

            Me.WindowState = FormWindowState.Normal
            Me.Size = hdSize

            ' Center window
            Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
            Dim centerX As Integer = (screenBounds.Width - Me.Width) \ 2
            Dim centerY As Integer = (screenBounds.Height - Me.Height) \ 2
            Me.Location = New Point(centerX, centerY)


        Else
            ' No, we are NOT in fullscreen

            ' Switch to fullscreen mode
            Me.FormBorderStyle = FormBorderStyle.None

            Me.WindowState = FormWindowState.Normal
            Me.Size = hdSize

            ' Center window
            Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
            Dim centerX As Integer = (screenBounds.Width - Me.Width) \ 2
            Dim centerY As Integer = (screenBounds.Height - Me.Height) \ 2
            Me.Location = New Point(centerX, centerY)

            Me.WindowState = FormWindowState.Maximized

        End If

    End Sub

    Private Sub PauseGame()

        FadeOutAndStopGamePlayLoop(600)

        MovePointerCenterScreen()


        pauseMenuSelection = 0 ' Resume game
        renderer.SetPauseMenuSelection(pauseMenuSelection)

        currentState = GameState.Pause
        physicsTimer.Stop()

        moveLeftPaddleUp = False
        moveLeftPaddleDown = False
        moveRightPaddleUp = False
        moveRightPaddleDown = False

        PlayPausedLoop(600)

    End Sub

    Private Sub ResumeGame()

        FadeOutAndStopPausedLoop(600)

        MovePointerOffScreen()


        currentState = GameState.Playing
        physicsTimer.Start()

        PlayGamePlayLoop(600)
    End Sub

    Private Sub StartNewMatch()
        FadeOutAndStopStartLoop(600)
        FadeOutAndStopPausedLoop(600)

        MovePointerOffScreen()

        speed = 800 * (ClientSize.Height / 1080.0)

        scoreLeft = 0
        scoreRight = 0
        renderer.UpdateScore(scoreLeft, scoreRight, Me.CreateGraphics(), ClientSize)

        If playerMode = 1 Then
            leftPlayerName = "You"
            rightPlayerName = "CPU"
            renderer.UpdatePlayerNames(leftPlayerName, rightPlayerName, Me.CreateGraphics(), ClientSize)
        Else
            leftPlayerName = "Left"
            rightPlayerName = "Right"
            renderer.UpdatePlayerNames(leftPlayerName, rightPlayerName, Me.CreateGraphics(), ClientSize)
        End If

        CenterBall()
        ServeBall(If(rng.Next(0, 2) = 0, -1, 1))

        currentState = GameState.Playing
        physicsTimer.Start()

        PlayGamePlayLoop(600)

    End Sub


    Private Sub CreateSoundFiles()
        CreateFileFromResource(Path.Combine(Application.StartupPath, "gameplayloop.mp3"), My.Resources.Resource1.TechNoir)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "bounce.mp3"), My.Resources.Resource1.bounce3)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "startloop.mp3"), My.Resources.Resource1.StartLoop)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "point.mp3"), My.Resources.Resource1.Score)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "arrow_up.mp3"), My.Resources.Resource1.ArrowUp2)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "arrow_down.mp3"), My.Resources.Resource1.ArrowDown2)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "select.mp3"), My.Resources.Resource1.Select2)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "pause.mp3"), My.Resources.Resource1.PauseLoop)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "fullscreen.mp3"), My.Resources.Resource1.FullScreen)
        CreateFileFromResource(Path.Combine(Application.StartupPath, "exit.mp3"), My.Resources.Resource1.ExitSound2)

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
        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)
    End Sub

    Private Sub MovePointerCenterScreen()
        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right \ 2,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)
    End Sub

    Public Sub RestartLoops()


        Select Case currentState

            Case GameState.StartScreen, GameState.EndScreen, GameState.AIDifficulty
                PlayStartLoop(600) ' includes fade-in

            Case GameState.Playing
                PlayGamePlayLoop(600) ' includes fade-in

            Case GameState.Pause
                PlayPausedLoop(600)  ' includes fade-in

        End Select

    End Sub

    Private Sub FadeOutAndStopActiveLoops(durationMs As Integer)
        ' Fade-out and stop only loops that are actually playing
        If Audio.IsPlaying("startloop") Then
            Audio.FadeOutAndStop("startloop", durationMs)
        End If
        If Audio.IsPlaying("gameplayloop") Then
            Audio.FadeOutAndStop("gameplayloop", durationMs)
        End If
        If Audio.IsPlaying("pause") Then
            Audio.FadeOutAndStop("pause", durationMs)
        End If
    End Sub

    Private Sub PlayPoint()
        Audio.PlaySound("point")
    End Sub

    Private Sub PlaySelectSound()
        Audio.PlaySound("select")
    End Sub


    Private Sub PlayExitSound()
        Audio.PlaySound("exit")
    End Sub

    Private Sub PlayPausedLoop(durationMs As Integer)

        ' Fade‑in loop
        Audio.SetVolume("pause", 0)
        Audio.LoopSound("pause")
        Audio.FadeVolume("pause", 0, pauseLoopVolume, durationMs)

    End Sub

    Private Sub PlayStartLoop(durationMs As Integer)

        ' Fade‑in loop
        Audio.SetVolume("startloop", 0)
        Audio.LoopSound("startloop")
        Audio.FadeVolume("startloop", 0, startLoopVolume, durationMs)

    End Sub


    Private Sub PlayGamePlayLoop(durationMs As Integer)

        ' Fade‑in loop
        Audio.SetVolume("gameplayloop", 0)
        Audio.LoopSound("gameplayloop")
        Audio.FadeVolume("gameplayloop", 0, gameplayLoopVolume, durationMs)

    End Sub


    Private Sub PlayFullScreenSound()
        Audio.PlaySound("fullscreen")
    End Sub


    Private Sub PlayBounce()
        Audio.PlayOverlapping("bounce")
    End Sub

    Private Sub PlayMenuUpSound()
        Audio.PlayOverlapping("arrow_up")
    End Sub


    Private Sub PlayMenuMoveSound()
        Audio.PlayOverlapping("arrow_up")
    End Sub

    Private Sub PlayMenuDownSound()
        Audio.PlayOverlapping("arrow_down")
    End Sub


    Private Sub FadeOutAndStopGamePlayLoop(durationMs As Integer)
        If Audio.IsPlaying("gameplayloop") Then Audio.FadeOutAndStop("gameplayloop", durationMs)
    End Sub


    Private Sub FadeOutAndStopStartLoop(durationMs As Integer)
        If Audio.IsPlaying("startloop") Then Audio.FadeOutAndStop("startloop", durationMs)
    End Sub

    Private Sub FadeOutAndStopPausedLoop(durationMs As Integer)
        If Audio.IsPlaying("pause") Then Audio.FadeOutAndStop("pause", durationMs)
    End Sub

    Private Function IsDarkMode() As Boolean
        If Environment.OSVersion.Version.Build >= 22000 Then
            ' Windows 11+
            Return Application.SystemColorMode = SystemColorMode.Dark
        Else
            ' Windows 10 fallback
            Return IsSystemDarkMode_Win10()
        End If
    End Function


    Private Function IsSystemDarkMode_Win10() As Boolean
        Try
            Dim key As RegistryKey =
            Registry.CurrentUser.OpenSubKey(
                "Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")

            If key Is Nothing Then Return False

            Dim value As Object = key.GetValue("AppsUseLightTheme", 1)
            Return CInt(value) = 0
        Catch
            Return False
        End Try
    End Function


    Private Sub ApplyDarkTitleBar(isDark As Boolean)
        If Environment.OSVersion.Version.Build < 22000 Then Exit Sub ' Only Windows 11+

        Dim value As Integer = If(isDark, 1, 0)
        DwmSetWindowAttribute(Me.Handle,
                              DWMWA_USE_IMMERSIVE_DARK_MODE,
                              value,
                              Marshal.SizeOf(value))

    End Sub


    Private Sub StartScreen_MouseClick(e As MouseEventArgs)
        If currentState <> GameState.StartScreen Then Return
        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' Ignore middle-click logic unless needed
        If e.Button = MouseButtons.Middle Then
            If numberOfPlayersSelection = NumberOfPlayers.OnePlayer Then
                playerMode = 1
                currentState = GameState.AIDifficulty
            Else
                playerMode = 2
                StartNewMatch()
            End If

            PlaySelectSound()
            Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' One Player
        If renderer.OnePlayerOptionRect.Contains(e.Location) Then
            numberOfPlayersSelection = NumberOfPlayers.OnePlayer   ' ← update selection
            renderer.SetStartMenuSelection(numberOfPlayersSelection)

            playerMode = 1
            currentState = GameState.AIDifficulty

            PlaySelectSound()
            Invalidate()
            Return
        End If

        ' Two Players
        If renderer.TwoPlayersOptionRect.Contains(e.Location) Then
            numberOfPlayersSelection = NumberOfPlayers.TwoPlayers   ' ← update selection
            renderer.SetStartMenuSelection(numberOfPlayersSelection)

            playerMode = 2
            StartNewMatch()

            PlaySelectSound()
            Invalidate()
            Return
        End If


    End Sub

    Private Sub StartScreen_MouseMove(e As MouseEventArgs)
        If currentState <> GameState.StartScreen Then Return

        Dim oldSelection = numberOfPlayersSelection




        If renderer.OnePlayerOptionRect.Contains(e.Location) Then
            numberOfPlayersSelection = NumberOfPlayers.OnePlayer
            renderer.SetStartMenuSelection(numberOfPlayersSelection)

        ElseIf renderer.TwoPlayersOptionRect.Contains(e.Location) Then
            numberOfPlayersSelection = NumberOfPlayers.TwoPlayers
            renderer.SetStartMenuSelection(numberOfPlayersSelection)

        End If


        If oldSelection <> numberOfPlayersSelection Then
            PlayMenuMoveSound()
            'PlayMenuUpSound()

            Invalidate()
        End If
    End Sub

    Private Sub AIDifficulty_MouseMove(e As MouseEventArgs)
        If currentState <> GameState.AIDifficulty Then Return

        Dim oldSelection = aiDifficultySelection


        If renderer.EasyRect.Contains(e.Location) Then
            aiDifficultySelection = AIDifficultyLevel.Easy
            renderer.SetAIDifficultySelection(aiDifficultySelection)
        ElseIf renderer.NormalRect.Contains(e.Location) Then
            aiDifficultySelection = AIDifficultyLevel.Normal
            renderer.SetAIDifficultySelection(aiDifficultySelection)

        ElseIf renderer.HardRect.Contains(e.Location) Then
            aiDifficultySelection = AIDifficultyLevel.Hard
            renderer.SetAIDifficultySelection(aiDifficultySelection)

        End If


        If oldSelection <> aiDifficultySelection Then
            'PlayMenuUpSound()
            PlayMenuMoveSound()

            Invalidate()
        End If
    End Sub


    Private Sub AIDifficulty_MouseClick(e As MouseEventArgs)
        If currentState <> GameState.AIDifficulty Then Return
        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' ------------------------------
        ' Middle-click = activate current selection
        ' ------------------------------
        If e.Button = MouseButtons.Middle Then
            ActivateAIDifficulty(aiDifficultySelection)
            PlaySelectSound()
            Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' Easy
        If renderer.EasyRect.Contains(e.Location) Then
            aiDifficultySelection = AIDifficultyLevel.Easy
            renderer.SetAIDifficultySelection(aiDifficultySelection)

            ActivateAIDifficulty(aiDifficultySelection)
            PlaySelectSound()
            Invalidate()
            Return
        End If

        ' Normal
        If renderer.NormalRect.Contains(e.Location) Then
            aiDifficultySelection = AIDifficultyLevel.Normal
            renderer.SetAIDifficultySelection(aiDifficultySelection)

            ActivateAIDifficulty(aiDifficultySelection)
            PlaySelectSound()
            Invalidate()
            Return
        End If

        ' Hard
        If renderer.HardRect.Contains(e.Location) Then
            aiDifficultySelection = AIDifficultyLevel.Hard
            renderer.SetAIDifficultySelection(aiDifficultySelection)

            ActivateAIDifficulty(aiDifficultySelection)
            PlaySelectSound()
            Invalidate()
            Return
        End If

    End Sub

    Private Sub ActivateAIDifficulty(level As AIDifficultyLevel)
        aiDifficultySelection = level
        renderer.SetAIDifficultySelection(aiDifficultySelection)

        SetAIModeFactor()
        currentState = GameState.Playing
        StartNewMatch()
    End Sub


    Private Sub PauseMenu_MouseMove(e As MouseEventArgs)
        If currentState <> GameState.Pause Then Return

        Dim oldSelection = pauseMenuSelection


        If renderer.ResumeRect.Contains(e.Location) Then
            pauseMenuSelection = 0
            renderer.SetPauseMenuSelection(pauseMenuSelection)

        ElseIf renderer.NewMatchRect.Contains(e.Location) Then
            pauseMenuSelection = 1
            renderer.SetPauseMenuSelection(pauseMenuSelection)

        ElseIf renderer.QuitRect.Contains(e.Location) Then
            pauseMenuSelection = 2
            renderer.SetPauseMenuSelection(pauseMenuSelection)
        End If


        If oldSelection <> pauseMenuSelection Then
            'PlayMenuUpSound()
            PlayMenuMoveSound()

            Invalidate()
        End If
    End Sub


    Private Sub PauseMenu_MouseClick(e As MouseEventArgs)
        If currentState <> GameState.Pause Then Return
        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' ------------------------------
        ' Middle-click = activate current selection
        ' ------------------------------
        If e.Button = MouseButtons.Middle Then
            Select Case pauseMenuSelection
                Case 0 : ResumeGame()
                Case 1 : StartNewMatch()
                Case 2 : Quit2StartScreen()
            End Select
            renderer.SetPauseMenuSelection(pauseMenuSelection)

            PlaySelectSound()
            Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' Resume
        If renderer.ResumeRect.Contains(e.Location) Then
            pauseMenuSelection = 0   ' ← keep selection visually correct
            renderer.SetPauseMenuSelection(pauseMenuSelection)

            ResumeGame()
            PlaySelectSound()
            Invalidate()
            Return
        End If

        ' New Match
        If renderer.NewMatchRect.Contains(e.Location) Then
            pauseMenuSelection = 1   ' ← keep selection visually correct
            renderer.SetPauseMenuSelection(pauseMenuSelection)

            StartNewMatch()
            PlaySelectSound()
            Invalidate()
            Return
        End If

        ' Quit
        If renderer.QuitRect.Contains(e.Location) Then
            pauseMenuSelection = 2   ' ← keep selection visually correct
            renderer.SetPauseMenuSelection(pauseMenuSelection)

            Quit2StartScreen()
            PlaySelectSound()
            Invalidate()
            Return
        End If
    End Sub


    Private Sub EndScreen_MouseClick(e As MouseEventArgs)
        If currentState <> GameState.EndScreen Then Return

        PlaySelectSound()
        currentState = GameState.StartScreen
        winnerText = ""
        renderer.SetWinnerText(winnerText)

        Invalidate()
    End Sub

    Private Sub HandlePauseMouseWheel(delta As Integer)

        ' Accumulate wheel movement
        pauseScrollAccum += delta

        ' ============================
        ' Scroll Up → Move Selection Up
        ' ============================
        If pauseScrollAccum >= ScrollThreshold Then

            If pauseMenuSelection > 0 Then
                pauseMenuSelection -= 1
                renderer.SetPauseMenuSelection(pauseMenuSelection)

                PlayMenuUpSound()
                Invalidate()
            End If

            pauseScrollAccum = 0
            Return
        End If

        ' ============================
        ' Scroll Down → Move Selection Down
        ' ============================
        If pauseScrollAccum <= -ScrollThreshold Then

            If pauseMenuSelection < 2 Then
                pauseMenuSelection += 1
                renderer.SetPauseMenuSelection(pauseMenuSelection)

                PlayMenuDownSound()
                Invalidate()
            End If

            pauseScrollAccum = 0
            Return
        End If

    End Sub


    Private Sub HandleStartScreenMouseWheel(delta As Integer)

        ' Accumulate wheel movement
        startScreenScrollAccum += delta

        ' Scroll up enough → select One Player
        If startScreenScrollAccum >= ScrollThreshold Then
            If numberOfPlayersSelection <> NumberOfPlayers.OnePlayer Then
                numberOfPlayersSelection = NumberOfPlayers.OnePlayer
                renderer.SetStartMenuSelection(numberOfPlayersSelection)

                PlayMenuUpSound()
                Invalidate()
            End If

            startScreenScrollAccum = 0
            Return
        End If

        ' Scroll down enough → select Two Players
        If startScreenScrollAccum <= -ScrollThreshold Then
            If numberOfPlayersSelection <> NumberOfPlayers.TwoPlayers Then
                numberOfPlayersSelection = NumberOfPlayers.TwoPlayers
                renderer.SetStartMenuSelection(numberOfPlayersSelection)

                PlayMenuDownSound()
                Invalidate()
            End If

            startScreenScrollAccum = 0
            Return
        End If

    End Sub


    Private Sub HandleAIDifficultyMouseWheel(delta As Integer)

        ' Accumulate wheel movement
        aiDifficultyScrollAccum += delta

        ' ============================
        ' Scroll Up → Move Selection Up
        ' ============================
        If aiDifficultyScrollAccum >= ScrollThreshold Then

            If aiDifficultySelection > AIDifficultyLevel.Easy Then
                aiDifficultySelection -= 1
                renderer.SetAIDifficultySelection(aiDifficultySelection)

                PlayMenuUpSound()
                Invalidate()
            End If

            aiDifficultyScrollAccum = 0
            Return
        End If

        ' ============================
        ' Scroll Down → Move Selection Down
        ' ============================
        If aiDifficultyScrollAccum <= -ScrollThreshold Then

            If aiDifficultySelection < AIDifficultyLevel.Hard Then
                aiDifficultySelection += 1
                renderer.SetAIDifficultySelection(aiDifficultySelection)

                PlayMenuDownSound()
                Invalidate()
            End If

            aiDifficultyScrollAccum = 0
            Return
        End If

    End Sub

End Class
