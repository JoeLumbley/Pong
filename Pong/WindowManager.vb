Imports System.Runtime.InteropServices
Imports Microsoft.Win32

Public Class WindowManager

    Private hdSize As New Size(1280, 720)
    Private fhdSize As New Size(1920, 1080)



    Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmSetWindowAttribute(
        hWnd As IntPtr,
        attr As Integer,
        ByRef attrValue As Integer,
        attrSize As Integer
    ) As Integer
    End Function



    Public Sub ToggleFullScreen(form As Form)

        ' Are we in fullscreen mode?
        If form.FormBorderStyle = FormBorderStyle.None Then
            ' Yes, we are in fullscreen mode

            ' Switch to windowed mode
            form.FormBorderStyle = FormBorderStyle.Sizable

            form.WindowState = FormWindowState.Normal
            form.Size = hdSize

            ' Center window
            Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
            Dim centerX As Integer = (screenBounds.Width - form.Width) \ 2
            Dim centerY As Integer = (screenBounds.Height - form.Height) \ 2
            form.Location = New Point(centerX, centerY)


        Else
            ' No, we are NOT in fullscreen

            ' Switch to fullscreen mode
            form.FormBorderStyle = FormBorderStyle.None

            form.WindowState = FormWindowState.Normal
            form.Size = hdSize

            ' Center window
            Dim screenBounds As Rectangle = Screen.PrimaryScreen.WorkingArea
            Dim centerX As Integer = (screenBounds.Width - form.Width) \ 2
            Dim centerY As Integer = (screenBounds.Height - form.Height) \ 2
            form.Location = New Point(centerX, centerY)

            form.WindowState = FormWindowState.Maximized

        End If

    End Sub





    Public Function IsDarkMode() As Boolean
        If Environment.OSVersion.Version.Build >= 22000 Then
            ' Windows 11+
            Return Application.SystemColorMode = SystemColorMode.Dark
        Else
            ' Windows 10 fallback
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


    Public Sub ApplyDarkTitleBar(isDark As Boolean, hWnd As IntPtr)
        If Environment.OSVersion.Version.Build < 22000 Then Exit Sub ' Only Windows 11+

        Dim value As Integer = If(isDark, 1, 0)
        DwmSetWindowAttribute(hWnd,
                              DWMWA_USE_IMMERSIVE_DARK_MODE,
                              value,
                              Marshal.SizeOf(value))

    End Sub









    Public Sub MovePointerOffScreen()
        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)
    End Sub

    Public Sub MovePointerCenterScreen()
        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right \ 2,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)
    End Sub








End Class
