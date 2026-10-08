Imports System.IO

Public Class ResourceManager

    Public Sub New()
    End Sub


    Public Sub CreateAudioFilesAsNeeded()
        CreateSoundFiles()
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

End Class
