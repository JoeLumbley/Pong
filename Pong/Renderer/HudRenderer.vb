Imports System.Drawing
Imports Pong.Enums

Public Class HudRenderer

    Private ReadOnly resources As GraphicsResources
    Private ReadOnly layout As HudLayout



#Region "Winner Animation"

    Private winnerScoreAlpha As Integer = 255
    Private winnerFadeIn As Boolean

#End Region

    Public Sub New(
        resources As GraphicsResources,
        layout As HudLayout)

        Me.resources = resources
        Me.layout = layout

    End Sub

    'Public Sub Draw(
    '    g As Graphics,
    '    model As GameModel)

    '    DrawPlayerNames(g, model)

    '    DrawScores(g, model)

    '    'DrawCenterDivider(g)

    'End Sub

    'Public Sub Draw(
    'g As Graphics,
    'model As GameModel)

    '    DrawPlayerNames(g, model)

    '    DrawScores(g, model)

    '    DrawCenterDivider(g)

    '    DrawGameplayHints(g, model)

    'End Sub



    'Public Sub Update()

    '    If winnerFadeIn Then

    '        winnerScoreAlpha += 3

    '        If winnerScoreAlpha >= 255 Then

    '            winnerScoreAlpha = 255
    '            winnerFadeIn = False

    '        End If

    '    Else

    '        winnerScoreAlpha -= 3

    '        If winnerScoreAlpha <= 64 Then

    '            winnerScoreAlpha = 64
    '            winnerFadeIn = True

    '        End If

    '    End If

    'End Sub



    Public Sub Update()

        If winnerFadeIn Then

            winnerScoreAlpha += 4

            If winnerScoreAlpha >= 255 Then

                winnerScoreAlpha = 255
                winnerFadeIn = False

            End If

        Else

            winnerScoreAlpha -= 3

            If winnerScoreAlpha <= 15 Then

                winnerScoreAlpha = 15
                winnerFadeIn = True

            End If

        End If

    End Sub



    Public Sub Draw(
    g As Graphics,
    model As GameModel)

        DrawPlayerNames(g, model)

        DrawScores(g, model)


        If model.State = GameState.Playing Then

            DrawCenterDivider(g)


            DrawGameplayHints(
            g,
            model)

        End If

    End Sub


    Private Sub DrawGameplayHints(
    g As Graphics,
    model As GameModel)

        If Not model.ShowKeyboardHints Then
            Return
        End If



        DrawMovementHints(
        g,
        model)

        DrawPauseHint(g, model)

        DrawFPS(g, model)

    End Sub


    Private Sub DrawMovementHints(
    g As Graphics,
    model As GameModel)

        If model.PlayerMode =
        NumberOfPlayers.OnePlayer Then

            g.DrawString(
            HudLayout.OnePlayerHintText,
            resources.KeyboardFont,
            resources.GrayBrush,
            layout.LeftHintX,
            layout.LeftHintY)

        Else

            g.DrawString(
            HudLayout.TwoPlayerLeftHintText,
            resources.KeyboardFont,
            resources.GrayBrush,
            layout.LeftHintX,
            layout.LeftHintY)

            'g.DrawString(
            'HudLayout.TwoPlayerRightHintText,
            'resources.KeyboardFont,
            'resources.GrayBrush,
            'layout.RightHintX,
            'layout.RightHintY)

        End If

    End Sub



    Private Sub DrawPauseHint(
    g As Graphics,
    model As GameModel)




        If model.PlayerMode =
        NumberOfPlayers.OnePlayer Then

            g.DrawString(
        HudLayout.PauseHintText,
        resources.KeyboardFont,
        resources.GrayBrush,
        layout.PauseHintX,
        layout.PauseHintY)


        Else

            g.DrawString(
        HudLayout.TwoPlayerRightHintText,
        resources.KeyboardFont,
        resources.GrayBrush,
        layout.RightHintX,
        layout.RightHintY)


            g.DrawString(
            HudLayout.PauseHintText,
            resources.KeyboardFont,
            resources.GrayBrush,
            layout.PauseHintX,
            layout.PauseHintY)

        End If





    End Sub



    Private Sub DrawFPS(
    g As Graphics,
    model As GameModel)

        g.DrawString(
        $"FPS: {model.FPS}",
        resources.KeyboardFont,
        resources.GrayBrush,
        layout.FpsX,
        layout.FpsY)

    End Sub























    Private Sub DrawPlayerNames(
        g As Graphics,
        model As GameModel)

        g.DrawString(
            model.LeftPlayerName,
            resources.HudLabelFont,
            resources.LabelBrush,
            layout.LeftLabelX,
            layout.LabelY)

        g.DrawString(
            model.RightPlayerName,
            resources.HudLabelFont,
            resources.LabelBrush,
            layout.RightLabelX,
            layout.LabelY)

    End Sub

    'Private Sub DrawScores(
    '    g As Graphics,
    '    model As GameModel)

    '    g.DrawString(
    '        model.ScoreLeft.ToString(),
    '        resources.HudScoreFont,
    '        resources.ScoreBrush,
    '        layout.LeftScoreX,
    '        layout.ScoreY)

    '    g.DrawString(
    '        model.ScoreRight.ToString(),
    '        resources.HudScoreFont,
    '        resources.ScoreBrush,
    '        layout.RightScoreX,
    '        layout.ScoreY)

    'End Sub












    Private Sub DrawScores(
    g As Graphics,
    model As GameModel)


        Dim leftWins As Boolean = False
        Dim rightWins As Boolean = False

        If model.WinnerIs = 1 Then

            leftWins = True

        End If
        If model.WinnerIs = 2 Then

            rightWins = True

        End If


        DrawScore(
        g,
        model.ScoreLeft.ToString(),
        layout.LeftScoreX,
        layout.ScoreY,
        leftWins,
        model)

        DrawScore(
        g,
        model.ScoreRight.ToString(),
        layout.RightScoreX,
        layout.ScoreY,
        rightWins,
        model)

    End Sub



    Private Sub DrawScore(
    g As Graphics,
    text As String,
    x As Single,
    y As Single,
    isWinner As Boolean,
    model As GameModel)


        If model.State = GameState.EndScreen Then

            If Not isWinner Then

                g.DrawString(
                    text,
                    resources.HudScoreFont,
                    resources.ScoreBrush,
                    x,
                    y)

                Return


            Else


                Using brush As New SolidBrush(
                Color.FromArgb(
                    winnerScoreAlpha,
                    Color.White))

                    g.DrawString(
                    text,
                    resources.HudScoreFont,
                    brush,
                    x,
                    y)

                End Using


            End If



        Else



            g.DrawString(
                    text,
                    resources.HudScoreFont,
                    resources.ScoreBrush,
                    x,
                    y)






        End If


    End Sub

    'Private Sub DrawCenterDivider(
    '    g As Graphics)

    '    Dim centerX As Single =
    '        g.VisibleClipBounds.Width / 2.0F

    '    Dim segmentHeight As Integer = 16
    '    Dim gapHeight As Integer = 10

    '    Dim y As Integer = 0

    '    While y < g.VisibleClipBounds.Height

    '        g.FillRectangle(
    '            resources.GrayBrush,
    '            centerX - 1,
    '            y,
    '            2,
    '            segmentHeight)

    '        y += segmentHeight + gapHeight

    '    End While

    'End Sub


    Private Sub DrawCenterDivider(
    g As Graphics)

        Dim centerX As Single =
            layout.ViewportWidth / 2.0F

        Dim segmentHeight As Integer =
            Math.Max(
                8,
                CInt(layout.ViewportHeight / 45.0F))

        'Dim gapHeight As Integer =
        '    Math.Max(
        '        4,
        '        CInt(layout.ViewportHeight / 72.0F))


        Dim gapHeight As Integer =
            Math.Max(
                4,
                CInt(layout.ViewportHeight / 25.0F))


        'Dim dividerWidth As Integer =
        '    Math.Max(
        '        2,
        '        CInt(layout.ViewportHeight / 540.0F))


        Dim dividerWidth As Integer =
            Math.Max(
                2,
                CInt(layout.ViewportHeight / 73.0F))


        Dim y As Integer = 0

        While y < layout.ViewportHeight

            g.FillRectangle(
                resources.GrayBrush,
                centerX - dividerWidth \ 2,
                y,
                dividerWidth,
                segmentHeight)

            y += segmentHeight + gapHeight

        End While

    End Sub












End Class

