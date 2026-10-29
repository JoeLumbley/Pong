Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Pong.Enums
Imports System.Diagnostics

Public Class AIDifficultyRenderer

    Private ReadOnly resources As GraphicsResources
    Private ReadOnly layout As AIDifficultyLayout

#Region "Animation"

    Private fadeAlpha As Integer = 255

    Private alphaFadeIn As Boolean = False

    Private blinkVisible As Boolean = True

    Private ReadOnly blinkStopwatch As Stopwatch =
    Stopwatch.StartNew()

#End Region

    Public Sub New(
        resources As GraphicsResources,
        layout As AIDifficultyLayout)

        Me.resources = resources
        Me.layout = layout

    End Sub

    'Public Sub Draw(
    '    g As Graphics,
    '    model As GameModel)

    '    DrawTitle(g)

    '    DrawDifficultyOption(
    '        g,
    '        AIDifficultyLayout.EasyText,
    '        layout.EasyRect,
    '        layout.EasyPath,
    '        model.AISelection = AIDifficultyLevel.Easy)

    '    DrawDifficultyOption(
    '        g,
    '        AIDifficultyLayout.NormalText,
    '        layout.NormalRect,
    '        layout.NormalPath,
    '        model.AISelection = AIDifficultyLevel.Normal)

    '    DrawDifficultyOption(
    '        g,
    '        AIDifficultyLayout.HardText,
    '        layout.HardRect,
    '        layout.HardPath,
    '        model.AISelection = AIDifficultyLevel.Hard)

    '    DrawInfo(g)

    'End Sub




    Public Sub Draw(
    g As Graphics,
    model As GameModel)

        DrawTitle(g)

        DrawDifficultyOption(
        g,
        AIDifficultyLayout.EasyText,
        layout.EasyRect,
        layout.EasyPath,
        model.AISelection = AIDifficultyLevel.Easy)

        DrawDifficultyOption(
        g,
        AIDifficultyLayout.NormalText,
        layout.NormalRect,
        layout.NormalPath,
        model.AISelection = AIDifficultyLevel.Normal)

        DrawDifficultyOption(
        g,
        AIDifficultyLayout.HardText,
        layout.HardRect,
        layout.HardPath,
        model.AISelection = AIDifficultyLevel.Hard)

        DrawInfo(g)

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
        AIDifficultyLayout.KeyboardHintsText,
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
           AIDifficultyLayout.ExitFullscreenText,
           AIDifficultyLayout.FullscreenText)

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
        AIDifficultyLayout.HideHintsText,
        resources.KeyboardFont,
        resources.GrayBrush,
        layout.HideHintsX,
        layout.HideHintsY)

    End Sub

    'Private Sub DrawTitle(
    '    g As Graphics)

    '    g.DrawString(
    '        AIDifficultyLayout.TitleText,
    '        resources.AITitleFont,
    '        resources.WhiteBrush,
    '        layout.TitleX,
    '        layout.TitleY)

    'End Sub

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
            AIDifficultyLayout.TitleText,
            resources.AITitleFont,
            brush,
            layout.TitleX,
            layout.TitleY)

        End Using

    End Sub
















    'Private Sub DrawInfo(
    '    g As Graphics)

    '    g.DrawString(
    '        AIDifficultyLayout.InfoText,
    '        resources.StartInfoFont,
    '        resources.GrayBrush,
    '        layout.InfoX,
    '        layout.InfoY)

    'End Sub

    'Private Sub DrawInfo(
    'g As Graphics)

    '    g.DrawString(
    '    AIDifficultyLayout.InfoText,
    '    resources.StartInfoFont,
    '    resources.GrayBrush,
    '    layout.InfoX,
    '    layout.InfoY)

    'End Sub

    Private Sub DrawInfo(
    g As Graphics)

        If Not blinkVisible Then
            Return
        End If

        g.DrawString(
        AIDifficultyLayout.InfoText,
        resources.StartInfoFont,
        resources.WhiteBrush,
        layout.InfoX,
        layout.InfoY)

    End Sub
















    Private Sub DrawDifficultyOption(
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

            sf.Alignment = StringAlignment.Center
            sf.LineAlignment = StringAlignment.Center

            g.DrawString(
                text,
                resources.StartMenuFont,
                textBrush,
                rect,
                sf)

        End Using

    End Sub


    Public Sub Update()

        If alphaFadeIn Then

            fadeAlpha += 2

            If fadeAlpha >= 255 Then

                fadeAlpha = 255

                alphaFadeIn = False

            End If

        Else

            fadeAlpha -= 2

            If fadeAlpha <= 64 Then

                fadeAlpha = 64

                alphaFadeIn = True

            End If

        End If

        If blinkStopwatch.ElapsedMilliseconds >= 1000 Then

            blinkVisible = Not blinkVisible

            blinkStopwatch.Restart()

        End If

    End Sub






End Class