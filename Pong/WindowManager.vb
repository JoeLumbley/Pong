Imports System.Runtime.InteropServices
Imports Microsoft.Win32

Public Class WindowManager

    Private ReadOnly hostForm As Form

    Private hdSize As New Size(1280, 720)
    Private fhdSize As New Size(1920, 1080)

    Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

    Public Sub New(form As Form)
        hostForm = form
    End Sub

    ' ==========================
    '  Initialize Window
    ' ==========================
    Public Sub InitWindow()
        hostForm.Text = "PONG - Code with Joe"
        hostForm.BackColor = Color.Black

        Dim dark As Boolean = IsDarkMode()
        ApplyDarkTitleBar(dark, hostForm.Handle)

        hostForm.StartPosition = FormStartPosition.CenterScreen
        hostForm.MinimumSize = New Size(256, 256)
        hostForm.Size = hdSize

        CenterWindow()
        hostForm.WindowState = FormWindowState.Normal
    End Sub

    ' ==========================
    '  Toggle Fullscreen
    ' ==========================
    Public Sub ToggleFullScreen()

        If hostForm.FormBorderStyle = FormBorderStyle.None Then
            hostForm.FormBorderStyle = FormBorderStyle.Sizable
            hostForm.WindowState = FormWindowState.Normal
            hostForm.Size = hdSize
            CenterWindow()
        Else
            hostForm.FormBorderStyle = FormBorderStyle.None
            hostForm.WindowState = FormWindowState.Normal
            hostForm.Size = hdSize
            CenterWindow()
            hostForm.WindowState = FormWindowState.Maximized
        End If

    End Sub

    ' ============================================================
    ' WINDOW CENTERING
    ' ============================================================
    Private Sub CenterWindow()
        Dim bounds As Rectangle = Screen.PrimaryScreen.WorkingArea
        Dim x As Integer = (bounds.Width - hostForm.Width) \ 2
        Dim y As Integer = (bounds.Height - hostForm.Height) \ 2
        hostForm.Location = New Point(x, y)
    End Sub

    ' ============================================================
    ' DARK TITLE BAR (WINDOWS 11)
    ' ============================================================
    Public Function IsDarkMode() As Boolean
        If Environment.OSVersion.Version.Build >= 22000 Then
            Return Application.SystemColorMode = SystemColorMode.Dark
        Else
            Return IsSystemDarkMode_Win10()
        End If
    End Function

    Public Function IsSystemDarkMode_Win10() As Boolean
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

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmSetWindowAttribute(
        hWnd As IntPtr,
        attr As Integer,
        ByRef attrValue As Integer,
        attrSize As Integer
    ) As Integer
    End Function

    Public Sub ApplyDarkTitleBar(isDark As Boolean, hWnd As IntPtr)
        If Environment.OSVersion.Version.Build < 22000 Then Exit Sub

        Dim value As Integer = If(isDark, 1, 0)
        DwmSetWindowAttribute(hWnd,
                              DWMWA_USE_IMMERSIVE_DARK_MODE,
                              value,
                              Marshal.SizeOf(value))
    End Sub

    ' ============================================================
    '  POINTER MOVEMENT
    ' ============================================================
    Public Sub MovePointerOffScreen()
        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)
    End Sub

    Public Sub MovePointerCenterScreen()
        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right \ 2,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)
    End Sub

End Class
