Imports Pong.Enums

'' PONG – Code with Joe 
'' A modern, full‑screen remake of the classic Pong arcade game featuring
'' smooth physics, glowing motion trails, dynamic paddle spin, AI or two‑player 
'' mode, animated menus, and immersive sound effects. Built with VB.NET and 
'' GDI+, the game delivers a polished retro experience with responsive controls
'' and crisp visuals.  
'' 
'' GitHub Repo: https://github.com/JoeLumbley/Pong


'' MIT License
'' Copyright(c) 2023 Joseph W. Lumbley

'' Permission is hereby granted, free of charge, to any person obtaining a copy
'' of this software and associated documentation files (the "Software"), to deal
'' in the Software without restriction, including without limitation the rights
'' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'' copies of the Software, and to permit persons to whom the Software is
'' furnished to do so, subject to the following conditions:

'' The above copyright notice and this permission notice shall be included in all
'' copies or substantial portions of the Software.

'' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
'' SOFTWARE.



'Public Class Form1

'    'Private window As New WindowManager
'    Private window As New WindowManager(Me)



'    Private resource As New ResourceManager

'    Private model As New GameModel(ClientSize)

'    'Private match As New MatchManager

'    Private input As New InputManager

'    Private audio As New AudioController



'    Public renderer As New Rendering

'    Private physics As New PhysicsEngine(model, audio)


'    Private state As New GameStateManager(model, audio, physics, window)

'    ' -------------------------------
'    '  Random
'    ' -------------------------------
'    Private rng As New Random

'    Private WithEvents AudioRestartTimer As Timer


'    ' ===============================
'    '  FORM LIFECYCLE
'    ' ===============================

'    Protected Overrides Sub OnLoad(e As EventArgs)
'        MyBase.OnLoad(e)

'        InitAudio()
'        InitWindow()
'        InitPhysics()
'        renderer.InitGraphics(Me, model)
'        state = New GameStateManager(model, audio, physics, window)


'    End Sub

'    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
'        Me.WindowState = FormWindowState.Maximized
'    End Sub


'    ' ===============================
'    '  CLEANUP / FILES
'    ' ===============================

'    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

'        AudioRestartTimer?.Stop()
'        physics.Stop()

'        AudioRestartTimer?.Dispose()

'        audio?.DisposeAudio()
'        audio = Nothing

'        model?.Dispose()

'    End Sub

'    ' ===============================
'    '  RENDERING
'    ' ===============================

'    Protected Overrides Sub OnPaint(e As PaintEventArgs)
'        MyBase.OnPaint(e)

'        renderer.UpdateFPS()
'        renderer.Render(e.Graphics, model.State, Me, model)

'    End Sub

'    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
'        ' Suppress background painting to avoid flicker
'    End Sub

'    ' ===============================
'    '  INPUT HANDLING
'    ' ===============================

'    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
'        MyBase.OnKeyDown(e)

'        input.OnKeyDown(e, Me, audio, state, renderer, model)

'    End Sub

'    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
'        MyBase.OnKeyUp(e)

'        input.OnKeyUp(e, model)

'    End Sub

'    Private Sub Form1_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick

'        'input.OnMouseClick(e, Me, audio, renderer, window, physics, model)

'        input.OnMouseClick(e,
'                           Me,
'                           audio,
'                           renderer,
'                           state,
'                           model)

'    End Sub

'    Private Sub Form1_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp

'        input.OnMouseUp()

'    End Sub

'    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove

'        input.OnMouseMove(e, renderer, Me, audio, model)

'    End Sub

'    Private Sub Form1_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel

'        input.OnMouseWheel(e, Me, audio, model)

'    End Sub

'    ' ===============================
'    '  AUDIO RESTART
'    ' ===============================

'    Private Sub AudioRestartTimer_Tick(sender As Object, e As EventArgs) Handles AudioRestartTimer.Tick
'        audio.RestartAudioEngine(model)
'    End Sub


'    ' ===============================
'    '  RESIZE / SCALING
'    ' ===============================

'    Protected Overrides Sub OnResize(e As EventArgs)
'        MyBase.OnResize(e)

'        If Me.WindowState = FormWindowState.Minimized Then Return

'        renderer.OnResize(Me, model, physics)

'        Invalidate()

'    End Sub


'    Private Sub InitWindow()

'        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or
'                    ControlStyles.UserPaint Or
'                    ControlStyles.OptimizedDoubleBuffer, True)

'        Me.DoubleBuffered = True

'        window.InitWindow()

'        window.MovePointerCenterScreen()

'    End Sub


'    Private Sub InitPhysics()
'        physics.Start()
'    End Sub

'    Private Sub InitAudio()
'        resource.CreateAudioFilesAsNeeded()
'        audio.InitAudio()
'        InitAudioRestartTimer()
'    End Sub

'    Private Sub InitAudioRestartTimer()

'        'Restart Audio every 3 minutes
'        AudioRestartTimer = New Timer With {
'            .Interval = 180000,
'            .Enabled = True
'        }

'        ''Restart Audio every 15 seconds for testing
'        'AudioRestartTimer = New Timer With {
'        '    .Interval = 15000,
'        '    .Enabled = True
'        '}

'    End Sub

'    Public Sub QuitGame()

'        AudioRestartTimer?.Stop()

'        audio.PlayExitSound()

'        audio.FadeOutAndStopActiveLoops(700)

'        physics.Stop()

'        ' Wait for the sound to finish before closing
'        Dim t As New Timer() With {.Interval = 800}
'        AddHandler t.Tick,
'        Sub()
'            t.Stop()
'            t.Dispose()

'            Me.Close()

'        End Sub

'        t.Start()

'    End Sub


'End Class





Public Class Form1


    Private isInitialized As Boolean

#Region "Systems"

    Private window As WindowManager
    Private resource As ResourceManager
    Private model As GameModel

    Private input As InputManager
    Private audio As AudioController
    Private renderer As Rendering

    Private physics As PhysicsEngine
    Private state As GameStateManager

    Private cachedScoreLeft As Integer = Integer.MinValue
    Private cachedScoreRight As Integer = Integer.MinValue
    Private cachedLeftPlayerName As String = Nothing
    Private cachedRightPlayerName As String = Nothing

#End Region

#Region "Timers"

    Private WithEvents AudioRestartTimer As Timer

#End Region

#Region "Form Lifecycle"

    'Protected Overrides Sub OnLoad(e As EventArgs)


    '    CreateSystems()

    '    MyBase.OnLoad(e)


    '    WireEvents()

    '    InitAudio()
    '    InitWindow()

    '    renderer.InitGraphics(Me, model)

    '    InitPhysics()

    'End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)

        CreateSystems()

        MyBase.OnLoad(e)

        WireEvents()

        InitAudio()
        InitWindow()

        renderer.InitGraphics(Me, model)

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

        audio?.DisposeAudio()

        model?.Dispose()

    End Sub

#End Region

#Region "Initialization"

    Private Sub CreateSystems()

        window = New WindowManager(Me)

        resource = New ResourceManager()

        model = New GameModel(ClientSize)

        input = New InputManager()

        audio = New AudioController()

        renderer = New Rendering()

        physics = New PhysicsEngine(
            model,
            audio)

        state = New GameStateManager(
            model,
            audio,
            physics,
            window)

    End Sub

    Private Sub WireEvents()

        AddHandler input.FullscreenRequested,
            AddressOf HandleFullscreenRequest

        AddHandler input.ExitRequested,
            AddressOf QuitGame

        AddHandler physics.FrameUpdated,
            AddressOf HandleFrameUpdated

    End Sub

    Private Sub InitWindow()

        SetStyle(
            ControlStyles.AllPaintingInWmPaint Or
            ControlStyles.UserPaint Or
            ControlStyles.OptimizedDoubleBuffer,
            True)

        DoubleBuffered = True

        window.InitWindow()

        window.MovePointerCenterScreen()

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

#End Region

#Region "Input Events"

    Protected Overrides Sub OnKeyDown(
        e As KeyEventArgs)

        MyBase.OnKeyDown(e)

        input.OnKeyDown(
            e,
            Me,
            audio,
            state,
            renderer,
            model)

    End Sub

    Protected Overrides Sub OnKeyUp(
        e As KeyEventArgs)

        MyBase.OnKeyUp(e)

        input.OnKeyUp(
            e,
            model)

    End Sub

    Private Sub Form1_MouseClick(
        sender As Object,
        e As MouseEventArgs) Handles Me.MouseClick

        input.OnMouseClick(
            e,
            Me,
            audio,
            renderer,
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
            renderer,
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

#Region "Rendering"

    'Protected Overrides Sub OnPaint(
    '    e As PaintEventArgs)

    '    MyBase.OnPaint(e)

    '    renderer.UpdateFPS()

    '    renderer.Render(
    '        e.Graphics,
    '        model.State,
    '        Me,
    '        model)

    'End Sub


    Protected Overrides Sub OnPaint(
    e As PaintEventArgs)

        MyBase.OnPaint(e)

        If Not isInitialized Then Return

        renderer.UpdateFPS()

        renderer.Render(
        e.Graphics,
        model.State,
        Me,
        model)

    End Sub

    Protected Overrides Sub OnPaintBackground(
        pevent As PaintEventArgs)

        ' Prevent flicker

    End Sub

#End Region

#Region "Resize"

    'Protected Overrides Sub OnResize(
    '    e As EventArgs)

    '    MyBase.OnResize(e)

    '    If WindowState = FormWindowState.Minimized Then
    '        Return
    '    End If

    '    renderer.OnResize(
    '        Me,
    '        model,
    '        physics)

    '    Invalidate()

    'End Sub

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

        renderer.OnResize(
        Me,
        model,
        physics)

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

#Region "InputManager Event Handlers"

    Private Sub HandleFullscreenRequest()

        window.ToggleFullScreen()

        model.IsFullscreen =
            Not model.IsFullscreen

        Invalidate()

    End Sub

#End Region

#Region "Physics Event Handlers"

    Private Sub HandleFrameUpdated()

        If IsDisposed OrElse Not IsHandleCreated Then Return

        renderer.UpdateTrail(model)
        renderer.UpdateStartScreenFX()

        If model.ScoreLeft <> cachedScoreLeft OrElse
           model.ScoreRight <> cachedScoreRight OrElse
           model.LeftPlayerName <> cachedLeftPlayerName OrElse
           model.RightPlayerName <> cachedRightPlayerName Then

            renderer.CacheHUD(Me, model)

            cachedScoreLeft = model.ScoreLeft
            cachedScoreRight = model.ScoreRight
            cachedLeftPlayerName = model.LeftPlayerName
            cachedRightPlayerName = model.RightPlayerName

        End If

        Invalidate()

    End Sub

#End Region

#Region "Application Commands"

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

End Class