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




Imports Pong.Enums

Public Class Form1

    Private isInitialized As Boolean

#Region "Systems"

    Private window As WindowManager
    Private resource As ResourceManager

    Private model As GameModel

    Private input As InputManager
    Private audio As AudioController
    Private physics As PhysicsEngine
    Private state As GameStateManager

#End Region

#Region "Rendering"

    Private graphicsResources As GraphicsResources

    Private renderer As RenderingEngine

    Private fpsCounter As FpsCounter

    Private startLayout As StartScreenLayout
    Private pauseLayout As PauseMenuLayout
    Private aiLayout As AIDifficultyLayout
    Private hudLayout As HudLayout

    Private paddleLayout As PaddleLayout



    Private generalLayout As Layout


    Private ballTrail As BallTrailRenderer
    Private paddleTrail As PaddleTrailRenderer
    Private wallImpactEffect As WallImpactEffect
    Private wallImpact As WallImpactRenderer

    Private startRenderer As StartScreenRenderer

    Private hudRenderer As HudRenderer

    Private pauseRenderer As PauseScreenRenderer

    Private aiRenderer As AIDifficultyRenderer



    Private ballRenderer As BallRenderer


    Private paddle As PaddleRenderer


    Private scaler As GameplayScaler

#End Region

#Region "Audio"

    Private WithEvents AudioRestartTimer As Timer

#End Region

#Region "Cached HUD Values"

    Private cachedScoreLeft As Integer = Integer.MinValue
    Private cachedScoreRight As Integer = Integer.MinValue

    Private cachedLeftPlayerName As String = Nothing
    Private cachedRightPlayerName As String = Nothing

#End Region

    ' -------------------------------
    '  Random
    ' -------------------------------
    Private rng As New Random()

    Private gameOverLayout As GameOverLayout
    Private gameOverRenderer As GameOverRenderer

#Region "Form Lifecycle"

    Protected Overrides Sub OnLoad(
        e As EventArgs)

        MyBase.OnLoad(e)

        CreateSystems()

        WireEvents()

        InitAudio()
        InitWindow()
        InitRendering()
        InitPhysics()

        isInitialized = True

    End Sub

    Private Sub Form1_Shown(
        sender As Object,
        e As EventArgs) Handles Me.Shown

        WindowState = FormWindowState.Maximized

    End Sub

    Private Sub Form1_FormClosing(
        sender As Object,
        e As FormClosingEventArgs) Handles Me.FormClosing

        AudioRestartTimer?.Stop()

        physics?.Stop()

        AudioRestartTimer?.Dispose()

        graphicsResources?.Dispose()

        audio?.DisposeAudio()

        model?.Dispose()

    End Sub

#End Region

#Region "Initialization"

    Private Sub CreateSystems()

        window = New WindowManager(Me)

        resource = New ResourceManager()

        model = New GameModel(ClientSize)

        generalLayout = New Layout

        graphicsResources = New GraphicsResources()

        startLayout = New StartScreenLayout()

        pauseLayout = New PauseMenuLayout()

        aiLayout = New AIDifficultyLayout()

        hudLayout = New HudLayout()

        paddleLayout = New PaddleLayout


        input = New InputManager(
        startLayout,
        aiLayout,
        pauseLayout)

        audio = New AudioController()

        physics = New PhysicsEngine(
        model,
        audio)

        state = New GameStateManager(
        model,
        audio,
        physics,
        window)


        scaler =
            New GameplayScaler


    End Sub

    Private Sub InitRendering()

        fpsCounter = New FpsCounter()

        graphicsResources.Initialize(Me)

        startLayout.Cache(Me, graphicsResources, model)

        pauseLayout.Cache(Me, graphicsResources, model)

        aiLayout.Cache(Me, graphicsResources, model)

        hudLayout.Cache(
        Me,
        model,
        graphicsResources)

        ballTrail = New BallTrailRenderer()


        paddleTrail = New PaddleTrailRenderer()


        wallImpact = New WallImpactRenderer


        wallImpactEffect = New WallImpactEffect


        startRenderer =
    New StartScreenRenderer(
        graphicsResources,
        startLayout)

        HudRenderer =
            New HudRenderer(
                graphicsResources,
                hudLayout)

        pauseRenderer =
            New PauseScreenRenderer(
                graphicsResources,
                pauseLayout)

        aiRenderer =
            New AIDifficultyRenderer(
                graphicsResources,
                aiLayout)

        ballRenderer =
            New BallRenderer(
                graphicsResources)

        paddleTrail =
            New PaddleTrailRenderer(
            )

        paddle =
            New PaddleRenderer(
            graphicsResources)



        gameOverLayout =
            New GameOverLayout

        gameOverRenderer =
            New GameOverRenderer(graphicsResources, gameOverLayout)

        renderer =
            New RenderingEngine(
                startRenderer,
                hudRenderer,
                pauseRenderer,
                aiRenderer,
                ballRenderer,
                ballTrail,
                paddleTrail,
                paddle,
                wallImpact,
                wallImpactEffect,
                gameOverLayout,
                gameOverRenderer)

        ballTrail.InitTrails(model)
        paddleTrail.Reset()

    End Sub

    Private Sub InitWindow()

        SetStyle(
            ControlStyles.AllPaintingInWmPaint Or
            ControlStyles.UserPaint Or
            ControlStyles.OptimizedDoubleBuffer,
            True)

        DoubleBuffered = True

        window.Initialize()

        window.CenterCursor()

    End Sub

    Private Sub InitPhysics()

        physics.Start()

    End Sub

    Private Sub InitAudio()

        resource.CreateAudioFilesAsNeeded()

        audio.InitAudio()

        InitAudioRestartTimer()

    End Sub

    Private Sub InitAudioRestartTimer()

        AudioRestartTimer = New Timer With {
            .Interval = 180000,
            .Enabled = True
        }

    End Sub

    Private Sub WireEvents()

        AddHandler input.FullscreenRequested,
            AddressOf HandleFullscreenRequest

        AddHandler input.ExitRequested,
            AddressOf QuitGame

        AddHandler physics.FrameUpdated,
            AddressOf HandleFrameUpdated

        AddHandler physics.WallImpact,
            AddressOf HandleWallImpact

    End Sub

#End Region

#Region "Rendering"

    Protected Overrides Sub OnPaint(
        e As PaintEventArgs)

        MyBase.OnPaint(e)

        If Not isInitialized Then Return

        fpsCounter.Update()
        model.fps = fpsCounter.FPS

        renderer.Render(
            e.Graphics,
            model,
            Me)

    End Sub

    Protected Overrides Sub OnPaintBackground(
        pevent As PaintEventArgs)

        ' Prevent flicker

    End Sub

#End Region

#Region "Resize"

    Protected Overrides Sub OnResize(
        e As EventArgs)

        MyBase.OnResize(e)

        If Not isInitialized Then Return

        If WindowState = FormWindowState.Minimized Then

            If model.State = GameState.Playing Then
                state.PauseGame()
            End If

            Return

        End If

        model.ClientSize = ClientSize

        graphicsResources.Rescale(Me, model, generalLayout)

        scaler.Scale(model)


        UpdateEffectScaling()



        startLayout.Cache(Me, graphicsResources, model)

        pauseLayout.Cache(Me, graphicsResources, model)

        aiLayout.Cache(Me, graphicsResources, model)

        hudLayout.Cache(Me, model, graphicsResources)

        paddleLayout.Resize(ClientSize, model)

        paddleTrail.Reset()
        paddleTrail.Update(model)

        ballTrail.Reset()

        physics.CenterBall()

        If model.State = GameState.Playing OrElse model.State = GameState.Pause Then
            physics.ServeBall(If(rng.Next(0, 2) = 0, -1, 1))
        Else
            physics.MoveBallRandom()
        End If

        physics.ResetPaddles()

        Invalidate()

    End Sub


    Private Sub UpdateEffectScaling()

        Dim scale As Single = CSng(ClientSize.Height / 150.0F)

        wallImpact.OuterRadius = 5.0F * scale
        wallImpact.InnerRadius = 3.0F * scale

    End Sub

#End Region

#Region "Input"

    Protected Overrides Sub OnKeyDown(
        e As KeyEventArgs)

        MyBase.OnKeyDown(e)

        input.OnKeyDown(
            e,
            Me,
            audio,
            state,
            model)

    End Sub

    Protected Overrides Sub OnKeyUp(
        e As KeyEventArgs)

        MyBase.OnKeyUp(e)

        input.OnKeyUp(
            e,
            model)

    End Sub

#End Region

#Region "Mouse"

    Private Sub Form1_MouseClick(
        sender As Object,
        e As MouseEventArgs) Handles Me.MouseClick

        input.OnMouseClick(
            e,
            Me,
            audio,
            state,
            model)

    End Sub

    Private Sub Form1_MouseDown(
        sender As Object,
        e As MouseEventArgs) Handles Me.MouseDown

        input.OnMouseDown(
            e,
            Me,
            audio,
            state)

    End Sub

    Private Sub Form1_MouseUp(
        sender As Object,
        e As MouseEventArgs) Handles Me.MouseUp

        input.OnMouseUp()

    End Sub

    Private Sub Form1_MouseMove(
        sender As Object,
        e As MouseEventArgs) Handles Me.MouseMove

        input.OnMouseMove(
            e,
            Me,
            audio,
            model)

    End Sub

    Private Sub Form1_MouseWheel(
        sender As Object,
        e As MouseEventArgs) Handles Me.MouseWheel

        input.OnMouseWheel(
            e,
            Me,
            audio,
            model)

    End Sub

#End Region

#Region "Frame Updates"

    Private Sub HandleFrameUpdated()

        If IsDisposed OrElse Not IsHandleCreated Then Return

        ballTrail.Update(model)

        paddleTrail.Update(model)

        wallImpactEffect.Update(model.dt)

        startRenderer.Update()

        aiRenderer.Update()

        gameOverRenderer.Update()

        hudRenderer.Update()

        UpdateHudCache()

        Invalidate()

    End Sub

    Private Sub UpdateHudCache()

        If model.ScoreLeft <> cachedScoreLeft OrElse
           model.ScoreRight <> cachedScoreRight OrElse
           model.LeftPlayerName <> cachedLeftPlayerName OrElse
           model.RightPlayerName <> cachedRightPlayerName Then

            hudLayout.Cache(
                Me,
                model,
                graphicsResources)

            cachedScoreLeft = model.ScoreLeft
            cachedScoreRight = model.ScoreRight

            cachedLeftPlayerName = model.LeftPlayerName
            cachedRightPlayerName = model.RightPlayerName

        End If

    End Sub

    Private Sub HandleWallImpact(
        location As PointF)

        wallImpactEffect.Trigger(location)

    End Sub

#End Region

#Region "Window"

    Private Sub HandleFullscreenRequest()

        window.ToggleFullScreen()

        If Me.FormBorderStyle = FormBorderStyle.None Then
            model.IsFullscreen = True
        Else

            model.IsFullscreen = False

        End If

        startLayout.Cache(Me, graphicsResources, model)
        aiLayout.Cache(Me, graphicsResources, model)

        pauseLayout.Cache(Me, graphicsResources, model)

        Invalidate()

    End Sub

#End Region

#Region "Audio"

    Private Sub AudioRestartTimer_Tick(
        sender As Object,
        e As EventArgs) Handles AudioRestartTimer.Tick

        audio.RestartAudioEngine(model)

    End Sub

#End Region

#Region "Quit"

    Public Sub QuitGame()

        AudioRestartTimer?.Stop()

        audio.PlayExitSound()

        audio.FadeOutAndStopActiveLoops(700)

        physics.Stop()

        Dim t As New Timer With {
            .Interval = 800
        }

        AddHandler t.Tick,
            Sub()

                t.Stop()

                t.Dispose()

                Close()

            End Sub

        t.Start()

    End Sub

#End Region


    ' ============================================================
    '  SYSTEM THEME CHANGE DETECTION
    '
    '  Windows sends WM_SETTINGCHANGE whenever a system setting
    '  changes. We use this notification to detect Light/Dark
    '  theme changes and update the title bar appearance without
    '  requiring the application to restart.
    ' ============================================================
    Protected Overrides Sub WndProc(ByRef m As Message)

        Const WM_SETTINGCHANGE As Integer = &H1A

        If m.Msg = WM_SETTINGCHANGE Then

            ' Reapply the current system theme to the window.
            If window IsNot Nothing Then
                window.ApplySystemTheme()
            End If

        End If

        MyBase.WndProc(m)

    End Sub





End Class

