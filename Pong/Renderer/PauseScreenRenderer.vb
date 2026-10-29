Imports System.Drawing
Imports System.Drawing.Drawing2D

Imports Pong.Enums

Public Class PauseScreenRenderer

    Private ReadOnly resources As GraphicsResources
    Private ReadOnly layout As PauseMenuLayout

    Public Sub New(
        resources As GraphicsResources,
        layout As PauseMenuLayout)

        Me.resources = resources
        Me.layout = layout

    End Sub

    'Public Sub Draw(
    '    g As Graphics,
    '    model As GameModel)

    '    DrawOverlay(g)

    '    DrawTitle(g)

    '    DrawMenuItem(
    '        g,
    '        PauseMenuLayout.ResumeText,
    '        layout.ResumeRect,
    '        layout.ResumePath)

    '    DrawMenuItem(
    '        g,
    '        PauseMenuLayout.NewGameText,
    '        layout.NewGameRect,
    '        layout.NewGamePath)

    '    DrawMenuItem(
    '        g,
    '        PauseMenuLayout.QuitText,
    '        layout.QuitRect,
    '        layout.QuitPath)

    'End Sub


    'Public Sub Draw(
    'g As Graphics,
    'selectedIndex As Integer)

    '    DrawOverlay(g)

    '    DrawTitle(g)

    '    DrawMenuItem(
    '    g,
    '    PauseMenuLayout.ResumeText,
    '    layout.ResumeRect,
    '    layout.ResumePath,
    '    selectedIndex = 0)

    '    DrawMenuItem(
    '    g,
    '    PauseMenuLayout.NewGameText,
    '    layout.NewGameRect,
    '    layout.NewGamePath,
    '    selectedIndex = 1)

    '    DrawMenuItem(
    '    g,
    '    PauseMenuLayout.QuitText,
    '    layout.QuitRect,
    '    layout.QuitPath,
    '    selectedIndex = 2)

    'End Sub


    Public Sub Draw(
    g As Graphics,
    model As GameModel)

        DrawOverlay(g)

        DrawTitle(g)

        DrawMenuItem(
        g,
        PauseMenuLayout.ResumeText,
        layout.ResumeRect,
        layout.ResumePath,
        model.PauseMenuSelection = PauseMenu.ResumeMatch)

        DrawMenuItem(
        g,
        PauseMenuLayout.NewGameText,
        layout.NewGameRect,
        layout.NewGamePath,
        model.PauseMenuSelection = PauseMenu.NewMatch)

        DrawMenuItem(
        g,
        PauseMenuLayout.QuitText,
        layout.QuitRect,
        layout.QuitPath,
        model.PauseMenuSelection = PauseMenu.QuitToMenu)


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

        DrawQuitHint(g)

        DrawHideHintsHint(g)

    End Sub

    Private Sub DrawKeyboardHints(
    g As Graphics)

        g.DrawString(
        PauseMenuLayout.KeyboardHintsText,
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
           PauseMenuLayout.ExitFullscreenText,
           PauseMenuLayout.FullscreenText)

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
        PauseMenuLayout.QuitHintText,
        resources.KeyboardFont,
        resources.GrayBrush,
        layout.QuitHintX,
        layout.QuitHintY)

    End Sub



    Private Sub DrawHideHintsHint(
    g As Graphics)

        g.DrawString(
        PauseMenuLayout.HideHintsText,
        resources.KeyboardFont,
        resources.GrayBrush,
        layout.HideHintsX,
        layout.HideHintsY)

    End Sub


















    Private Sub DrawOverlay(
        g As Graphics)

        g.FillRectangle(
            resources.DimBrush,
            g.VisibleClipBounds)

    End Sub

    Private Sub DrawTitle(
        g As Graphics)

        g.DrawString(
            PauseMenuLayout.PauseTitle,
            resources.PauseTitleFont,
            resources.WhiteBrush,
            layout.TitleX,
            layout.TitleY)

    End Sub

    'Private Sub DrawMenuItem(
    '    g As Graphics,
    '    text As String,
    '    rect As Rectangle,
    '    path As GraphicsPath)

    '    g.FillPath(
    '        resources.DarkBrush,
    '        path)

    '    g.DrawPath(
    '        resources.OutlinePen,
    '        path)

    '    Using sf As New StringFormat()

    '        sf.Alignment = StringAlignment.Center
    '        sf.LineAlignment = StringAlignment.Center

    '        g.DrawString(
    '            text,
    '            resources.PauseMenuFont,
    '            resources.WhiteBrush,
    '            rect,
    '            sf)

    '    End Using

    'End Sub


    'Private Sub DrawMenuItem(
    'g As Graphics,
    'text As String,
    'rect As Rectangle,
    'path As GraphicsPath,
    'isSelected As Boolean)

    '    If isSelected Then

    '        g.FillPath(
    '            resources.LightBrush,
    '            path)

    '    Else

    '        g.FillPath(
    '            resources.DarkBrush,
    '            path)

    '    End If

    '    g.DrawPath(
    '        resources.OutlinePen,
    '        path)

    '    Using sf As New StringFormat()

    '        sf.Alignment = StringAlignment.Center
    '        sf.LineAlignment = StringAlignment.Center

    '        g.DrawString(
    '            text,
    '            resources.PauseMenuFont,
    '            resources.WhiteBrush,
    '            rect,
    '            sf)

    '    End Using

    'End Sub

    Private Sub DrawMenuItem(
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
                resources.PauseMenuFont,
                textBrush,
                rect,
                sf)

        End Using

    End Sub


















End Class


