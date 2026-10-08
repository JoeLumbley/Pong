Imports System.IO
Imports Pong.Enums

Public Class AudioController

    Private Audio As AudioPlayer

    Private gameplayLoopVolume As Integer = 50
    Private startLoopVolume As Integer = 75
    Private pauseLoopVolume As Integer = 40

    ' -------------------------------
    '  Audio Cooldown
    ' -------------------------------
    Private lastPlay As New Dictionary(Of String, Integer)


















    Public Sub New()
        InitAudio()
    End Sub


    Private Sub InitAudio()

        Audio = New AudioPlayer()

        'CreateSoundFiles()

        LoadAndRegisterSounds()

        PlayStartLoop(600)

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

    Public Sub RestartAudioEngine(state As GameStateManager)

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
                               RestartLoops(state)
                           End Sub

        t.Start()

    End Sub

    Public Sub FadeOutAndStopActiveLoops(durationMs As Integer)
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

    Private Sub RestartLoops(state As GameStateManager)

        Select Case state.GetCurrentState()

            Case GameState.StartScreen, GameState.EndScreen, GameState.AIDifficulty
                PlayStartLoop(600) ' includes fade-in

            Case GameState.Playing
                PlayGamePlayLoop(600) ' includes fade-in

            Case GameState.Pause
                PlayPausedLoop(600)  ' includes fade-in

        End Select

    End Sub


    Public Sub PlayPoint()
        Audio.PlaySound("point")
    End Sub

    Public Sub PlaySelectSound()
        Audio.PlaySound("select")
    End Sub


    Public Sub PlayExitSound()
        Audio.PlaySound("exit")
    End Sub

    Public Sub PlayPausedLoop(durationMs As Integer)

        ' Fade‑in loop
        Audio.SetVolume("pause", 0)
        Audio.LoopSound("pause")
        Audio.FadeVolume("pause", 0, pauseLoopVolume, durationMs)

    End Sub

    Public Sub PlayStartLoop(durationMs As Integer)

        ' Fade‑in loop
        Audio.SetVolume("startloop", 0)
        Audio.LoopSound("startloop")
        Audio.FadeVolume("startloop", 0, startLoopVolume, durationMs)

    End Sub


    Public Sub PlayGamePlayLoop(durationMs As Integer)

        ' Fade‑in loop
        Audio.SetVolume("gameplayloop", 0)
        Audio.LoopSound("gameplayloop")
        Audio.FadeVolume("gameplayloop", 0, gameplayLoopVolume, durationMs)

    End Sub


    Public Sub PlayFullScreenSound()
        Audio.PlaySound("fullscreen")
    End Sub


    Public Sub PlayBounce()
        Audio.PlayOverlapping("bounce")
    End Sub

    Public Sub PlayMenuUpSound()
        Audio.PlayOverlapping("arrow_up")
    End Sub


    Public Sub PlayMenuMoveSound()
        Audio.PlayOverlapping("arrow_up")
    End Sub

    Public Sub PlayMenuDownSound()
        Audio.PlayOverlapping("arrow_down")
    End Sub


    Public Sub FadeOutAndStopGamePlayLoop(durationMs As Integer)
        If Audio.IsPlaying("gameplayloop") Then Audio.FadeOutAndStop("gameplayloop", durationMs)
    End Sub


    Public Sub FadeOutAndStopStartLoop(durationMs As Integer)
        If Audio.IsPlaying("startloop") Then Audio.FadeOutAndStop("startloop", durationMs)
    End Sub

    Public Sub FadeOutAndStopPausedLoop(durationMs As Integer)
        If Audio.IsPlaying("pause") Then Audio.FadeOutAndStop("pause", durationMs)
    End Sub

    Public Sub DisposeAudio()
        Audio?.Dispose()
    End Sub




    Public Sub PlayWithCooldown(name As String, ms As Integer)
        Dim now As Integer = Environment.TickCount

        If lastPlay.ContainsKey(name) AndAlso now - lastPlay(name) < ms Then
            Return
        End If

        lastPlay(name) = now
        Audio.PlayOverlapping(name)
    End Sub













End Class
