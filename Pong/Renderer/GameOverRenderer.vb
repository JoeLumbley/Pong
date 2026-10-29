'Public Class GameOverRenderer

'    Private ReadOnly layout As GameOverLayout

'    Public Sub New(layout As GameOverLayout)

'        Me.layout = layout

'    End Sub

'    Public Sub Render(
'        g As Graphics,
'        form As Form,
'        model As GameModel,
'        resources As GraphicsResources,
'        fadeAlpha As Integer,
'        blinkVisible As Boolean)

'        layout.Update(
'            form,
'            g,
'            model.WinnerText,
'            resources.GameOverFont,
'            resources.GameOverInfoFont,
'            resources.KeyboardFont)

'        DrawTitle(
'            g,
'            model,
'            resources.GameOverFont,
'            fadeAlpha)

'        DrawRestartMessage(
'            g,
'            resources.GameOverInfoFont,
'            resources.WhiteBrush,
'            blinkVisible)

'        DrawHints(
'            g,
'            form,
'            resources.KeyboardFont,
'            resources.GrayBrush)


'    End Sub

'    Private Sub DrawTitle(
'        g As Graphics,
'        model As GameModel,
'        titleFont As Font,
'        fadeAlpha As Integer)

'        Using titleBrush As New SolidBrush(
'            Color.FromArgb(
'                fadeAlpha,
'                Color.White))

'            g.DrawString(
'                model.WinnerText,
'                titleFont,
'                titleBrush,
'                layout.TitleX,
'                layout.TitleY)

'        End Using

'    End Sub

'    Private Sub DrawRestartMessage(
'        g As Graphics,
'        infoFont As Font,
'        whiteBrush As Brush,
'        blinkVisible As Boolean)

'        If Not blinkVisible Then Return

'        g.DrawString(
'            "Press ENTER to Start New Match",
'            infoFont,
'            whiteBrush,
'            layout.MessageX,
'            layout.MessageY)

'    End Sub

'    Private Sub DrawHints(
'        g As Graphics,
'        form As Form,
'        keyboardFont As Font,
'        grayBrush As Brush)

'        g.DrawString(
'            "ENTER - Start New Match",
'            keyboardFont,
'            grayBrush,
'            layout.StartHintX,
'            layout.StartHintY)

'        Dim fullscreenText As String =
'            If(form.FormBorderStyle = FormBorderStyle.None,
'               "F - Exit Fullscreen",
'               "F - Fullscreen")

'        g.DrawString(
'            fullscreenText,
'            keyboardFont,
'            grayBrush,
'            layout.FullscreenHintX,
'            layout.FullscreenHintY)

'        g.DrawString(
'            "CTRL+Q - Quit Game",
'            keyboardFont,
'            grayBrush,
'            layout.QuitHintX,
'            layout.QuitHintY)

'    End Sub

'End Class



Imports System.Diagnostics

Public Class GameOverRenderer

    Private ReadOnly resources As GraphicsResources
    Private ReadOnly layout As GameOverLayout

#Region "Animation"

    Private fadeAlpha As Integer = 255

    Private alphaFadeIn As Boolean = False

    Private blinkVisible As Boolean = True

    Private ReadOnly blinkStopwatch As Stopwatch =
        Stopwatch.StartNew()

#End Region

    Public Sub New(
        resources As GraphicsResources,
        layout As GameOverLayout)

        Me.resources = resources
        Me.layout = layout

    End Sub

    Public Sub Update()

        If alphaFadeIn Then

            fadeAlpha += 4

            If fadeAlpha >= 255 Then

                fadeAlpha = 255
                alphaFadeIn = False

            End If

        Else

            fadeAlpha -= 3

            If fadeAlpha <= 20 Then

                fadeAlpha = 20
                alphaFadeIn = True

            End If

        End If

        If blinkStopwatch.ElapsedMilliseconds >= 1500 Then

            blinkVisible = Not blinkVisible

            blinkStopwatch.Restart()

        End If

    End Sub

    Public Sub Draw(
        g As Graphics,
        form As Form,
        model As GameModel, gameOverLayout As GameOverLayout)

        layout.Update(
            form,
            g,
            model.WinnerText,
            resources.GameOverFont,
            resources.GameOverInfoFont,
            resources.KeyboardFont,
            model)

        DrawTitle(
            g,
            model)

        DrawRestartMessage(g, gameOverLayout)

        DrawHints(
            g,
            model)

    End Sub

    Private Sub DrawTitle(
        g As Graphics,
        model As GameModel)

        Using titleBrush As New SolidBrush(
            Color.FromArgb(
                fadeAlpha,
                Color.White))

            g.DrawString(
                model.WinnerText,
                resources.GameOverFont,
                titleBrush,
                layout.TitleX,
                layout.TitleY)

        End Using

    End Sub

    Private Sub DrawRestartMessage(
        g As Graphics, GameOverLayout As GameOverLayout)

        If Not blinkVisible Then Return

        g.DrawString(
            GameOverLayout.InfoText,
            resources.GameOverInfoFont,
            resources.WhiteBrush,
            layout.MessageX,
            layout.MessageY)

    End Sub

    Private Sub DrawHints(
        g As Graphics,
        model As GameModel)

        If Not model.ShowKeyboardHints Then
            Return
        End If

        DrawStartHint(g)

        DrawFullscreenHint(
            g,
            model)

        DrawQuitHint(g)

    End Sub

    Private Sub DrawStartHint(
        g As Graphics)

        g.DrawString(
            GameOverLayout.StartHintText,
            resources.KeyboardFont,
            resources.GrayBrush,
            layout.StartHintX,
            layout.StartHintY)

    End Sub

    Private Sub DrawFullscreenHint(
        g As Graphics,
        model As GameModel)

        Dim text As String =
            If(model.IsFullscreen,
               "F - Exit Fullscreen",
               "F - Fullscreen")

        g.DrawString(
            text,
            resources.KeyboardFont,
            resources.GrayBrush,
            layout.FullscreenHintX,
            layout.FullscreenHintY)

    End Sub

    Private Sub DrawQuitHint(
        g As Graphics)

        g.DrawString(
            GameOverLayout.QuitHintText,
            resources.KeyboardFont,
            resources.GrayBrush,
            layout.QuitHintX,
            layout.QuitHintY)

    End Sub

End Class