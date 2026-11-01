Imports Microsoft.Win32
Imports System.Runtime.InteropServices

Public Class WindowManager

    Private ReadOnly hostForm As Form

    Private ReadOnly minimumSize As New Size(255, 255)
    Private ReadOnly hdSize As New Size(1280, 720)


    Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

    Public Sub New(form As Form)

        hostForm = form

    End Sub

    Public Sub Initialize()

        hostForm.Text = "Pong - Code with Joe"
        hostForm.BackColor = Color.Black

        hostForm.StartPosition = FormStartPosition.CenterScreen
        hostForm.MinimumSize = minimumSize
        hostForm.Size = hdSize

        ApplySystemTheme()

        CenterWindow()

    End Sub

    Public Sub ApplySystemTheme()

        ApplyDarkTitleBar(IsDarkMode())

    End Sub

    Private Function IsDarkMode() As Boolean

        Try

            Using key As RegistryKey =
                Registry.CurrentUser.OpenSubKey(
                    "Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")

                If key Is Nothing Then Return False

                Dim value As Integer =
                    CInt(key.GetValue("AppsUseLightTheme", 1))

                Return value = 0

            End Using

        Catch

            Return False

        End Try

    End Function

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmSetWindowAttribute(
        hwnd As IntPtr,
        attribute As Integer,
        ByRef value As Integer,
        valueSize As Integer) As Integer
    End Function

    Private Sub ApplyDarkTitleBar(isDark As Boolean)

        If Environment.OSVersion.Version.Major < 10 Then
            Return
        End If

        Dim value As Integer = If(isDark, 1, 0)

        DwmSetWindowAttribute(
            hostForm.Handle,
            DWMWA_USE_IMMERSIVE_DARK_MODE,
            value,
            Marshal.SizeOf(Of Integer)())

    End Sub

    Public Sub ToggleFullscreen()

        If hostForm.FormBorderStyle = FormBorderStyle.None Then

            hostForm.FormBorderStyle = FormBorderStyle.Sizable
            hostForm.WindowState = FormWindowState.Normal
            hostForm.Size = hdSize

            CenterWindow()

        Else

            hostForm.FormBorderStyle = FormBorderStyle.None
            hostForm.WindowState = FormWindowState.Maximized

        End If

    End Sub

    Private Sub CenterWindow()

        Dim bounds =
            Screen.PrimaryScreen.WorkingArea

        hostForm.Location =
            New Point(
                (bounds.Width - hostForm.Width) \ 2,
                (bounds.Height - hostForm.Height) \ 2)

    End Sub



    Public Sub MoveCursorOffScreen()

        Dim bounds = Screen.PrimaryScreen.Bounds

        Cursor.Position =
            New Point(bounds.Right,
                      bounds.Height \ 2)

    End Sub

    Public Sub CenterCursor()

        Dim bounds = Screen.PrimaryScreen.Bounds

        Cursor.Position =
            New Point(bounds.Width \ 2,
                      bounds.Height \ 2)

    End Sub

End Class



