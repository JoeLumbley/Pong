
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Diagnostics
Imports Pong.Enums

Public Class StartScreenRenderer

    Private ReadOnly resources As GraphicsResources
    Private ReadOnly layout As StartScreenLayout

#Region "Animation"

    Private fadeAlpha As Integer = 255

    Private alphaFadeIn As Boolean = False

    Private blinkVisible As Boolean = True

    Private ReadOnly blinkStopwatch As Stopwatch =
        Stopwatch.StartNew()

#End Region

    Public Sub New(
        resources As GraphicsResources,
        layout As StartScreenLayout)

        Me.resources = resources
        Me.layout = layout

    End Sub

    Public Sub Update()

        If alphaFadeIn Then

            fadeAlpha += 3

            If fadeAlpha >= 255 Then

                fadeAlpha = 255

                alphaFadeIn = False

            End If

        Else

            fadeAlpha -= 1

            If fadeAlpha <= 16 Then

                fadeAlpha = 16

                alphaFadeIn = True

            End If

        End If

        If blinkStopwatch.ElapsedMilliseconds >= 1000 Then

            blinkVisible = Not blinkVisible

            blinkStopwatch.Restart()

        End If

    End Sub

    'Public Sub Draw(
    '    g As Graphics,
    '    model As GameModel)

    '    'DrawBackground(g)

    '    DrawTitle(g)

    '    DrawMenuButton(
    '        g,
    '        StartScreenLayout.OnePlayerText,
    '        layout.OnePlayerRect,
    '        layout.OnePlayerPath,
    '        model.PlayerMode =
    '        NumberOfPlayers.OnePlayer)

    '    DrawMenuButton(
    '        g,
    '        StartScreenLayout.TwoPlayersText,
    '        layout.TwoPlayersRect,
    '        layout.TwoPlayersPath,
    '        model.PlayerMode =
    '        NumberOfPlayers.TwoPlayers)

    '    DrawStartInfo(g)

    'End Sub



    Public Sub Draw(
    g As Graphics,
    model As GameModel)

        DrawTitle(g)

        DrawMenuButton(
        g,
        StartScreenLayout.OnePlayerText,
        layout.OnePlayerRect,
        layout.OnePlayerPath,
        model.PlayerMode =
        NumberOfPlayers.OnePlayer)

        DrawMenuButton(
        g,
        StartScreenLayout.TwoPlayersText,
        layout.TwoPlayersRect,
        layout.TwoPlayersPath,
        model.PlayerMode =
        NumberOfPlayers.TwoPlayers)

        DrawStartInfo(g)

        DrawHints(
        g,
        model)

    End Sub

    Private Sub DrawHints(
    g As Graphics,
    model As GameModel)

        If Not model.ShowKeyboardHints Then
            Return
        End If

        DrawKeyboardHints(g)

        DrawFullscreenHint(
        g,
        model)

        DrawHideHintsHint(g)

    End Sub


    Private Sub DrawKeyboardHints(
    g As Graphics)

        g.DrawString(
        StartScreenLayout.KeyboardHintsText,
        resources.KeyboardFont,
        resources.GrayBrush,
        layout.KeyboardHintsX,
        layout.KeyboardHintsY)

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



    Private Sub DrawHideHintsHint(
    g As Graphics)

        g.DrawString(
        StartScreenLayout.HideHintsText,
        resources.KeyboardFont,
        resources.GrayBrush,
        layout.HideHintsX,
        layout.HideHintsY)

    End Sub








    Private Sub DrawBackground(
        g As Graphics)

        Using brush As New LinearGradientBrush(
            New Point(0, 0),
            New Point(0, CInt(g.VisibleClipBounds.Height)),
            Color.FromArgb(30, 30, 30),
            Color.Black)

            g.FillRectangle(
                brush,
                g.VisibleClipBounds)

        End Using

    End Sub

    Private Sub DrawTitle(
        g As Graphics)

        Dim color As Color =
            Color.FromArgb(
                fadeAlpha,
                255,
                255,
                255)

        Using brush As New SolidBrush(color)

            g.DrawString(
                StartScreenLayout.TitleText,
                resources.StartTitleFont,
                brush,
                layout.TitleX,
                layout.TitleY)

        End Using

    End Sub

    Private Sub DrawMenuButton(
        g As Graphics,
        text As String,
        rect As Rectangle,
        path As GraphicsPath,
        isSelected As Boolean)

        Dim textBrush As Brush

        If isSelected Then

            g.FillPath(
                resources.LightBrush,
                path)

            g.DrawPath(
                resources.OutlinePen,
                path)

            textBrush = resources.WhiteBrush

        Else

            g.FillPath(
                resources.DarkBrush,
                path)

            g.DrawPath(
                resources.DarkOutlinePen,
                path)

            textBrush = resources.GrayBrush

        End If

        Using sf As New StringFormat()

            sf.Alignment =
                StringAlignment.Center

            sf.LineAlignment =
                StringAlignment.Center

            g.DrawString(
                text,
                resources.StartMenuFont,
                textBrush,
                rect,
                sf)

        End Using

    End Sub

    Private Sub DrawStartInfo(
        g As Graphics)

        If Not blinkVisible Then Return

        g.DrawString(
            StartScreenLayout.InfoText,
            resources.StartInfoFont,
            resources.WhiteBrush,
            layout.InfoX,
            layout.InfoY)

    End Sub

End Class









