'Public Class GameOverLayout

'    Public Property TitleX As Single
'    Public Property TitleY As Single

'    Public Property MessageX As Single
'    Public Property MessageY As Single

'    Public Property StartHintX As Single
'    Public Property StartHintY As Single

'    Public Property FullscreenHintX As Single
'    Public Property FullscreenHintY As Single

'    Public Property QuitHintX As Single
'    Public Property QuitHintY As Single

'    Public Sub Update(
'        form As Form,
'        g As Graphics,
'        winnerText As String,
'        titleFont As Font,
'        infoFont As Font,
'        keyboardFont As Font)

'        Dim titleSize As SizeF =
'            g.MeasureString(winnerText, titleFont)

'        TitleX =
'            (form.ClientSize.Width - titleSize.Width) / 2.0F

'        TitleY =
'            CSng(form.ClientSize.Height * 0.35F)

'        Dim infoSize As SizeF =
'            g.MeasureString(
'                "Press ENTER to Start New Match",
'                infoFont)

'        MessageX =
'            (form.ClientSize.Width - infoSize.Width) / 2.0F

'        MessageY =
'            CSng(form.ClientSize.Height * 0.6F)

'        StartHintX = 10
'        StartHintY = 10

'        Dim fullscreenSize As SizeF =
'            g.MeasureString(
'                "F - Exit Fullscreen",
'                keyboardFont)

'        FullscreenHintX =
'            form.ClientSize.Width -
'            fullscreenSize.Width - 10

'        FullscreenHintY = 10

'        Dim quitSize As SizeF =
'            g.MeasureString(
'                "CTRL+Q - Quit Game",
'                keyboardFont)

'        QuitHintX = 10

'        QuitHintY =
'            form.ClientSize.Height -
'            quitSize.Height - 10

'    End Sub

'End Class

Public Class GameOverLayout

    Public Const InfoText As String =
        "Press SPACE to Restart"

    Public Const StartHintText As String =
        "ENTER - Start New Match"

    Public Const QuitHintText As String =
        "CTRL+Q - Quit Game"

    Public Property TitleX As Single
    Public Property TitleY As Single

    Public Property MessageX As Single
    Public Property MessageY As Single

    Public Property StartHintX As Single
    Public Property StartHintY As Single

    Public Property FullscreenHintX As Single
    Public Property FullscreenHintY As Single

    Public Property QuitHintX As Single
    Public Property QuitHintY As Single

    Public Sub Update(
        form As Form,
        g As Graphics,
        winnerText As String,
        gameOverFont As Font,
        gameOverInfoFont As Font,
        keyboardFont As Font,
        model As GameModel)

        Dim titleSize =
            g.MeasureString(
                winnerText,
                gameOverFont)

        TitleX =
            (form.ClientSize.Width - titleSize.Width) / 2.0F

        TitleY =
            CSng(form.ClientSize.Height * 0.3F)

        Dim infoSize =
            g.MeasureString(
                InfoText,
                gameOverInfoFont)

        MessageX =
            (form.ClientSize.Width - infoSize.Width) / 2.0F

        MessageY =
            CSng(form.ClientSize.Height * 0.55F)

        StartHintX = 10
        StartHintY = 10



        If model.IsFullscreen Then


            Dim fullscreenSize =
            g.MeasureString(
                "F - Exit Fullscreen",
                keyboardFont)

            FullscreenHintX =
            form.ClientSize.Width -
            fullscreenSize.Width - 10

            FullscreenHintY = 10


        Else

            Dim fullscreenSize =
            g.MeasureString(
                "F - Fullscreen",
                keyboardFont)

            FullscreenHintX =
            form.ClientSize.Width -
            fullscreenSize.Width - 10

            FullscreenHintY = 10



        End If










        Dim quitSize =
            g.MeasureString(
                QuitHintText,
                keyboardFont)

            QuitHintX = 10

            QuitHintY =
            form.ClientSize.Height -
            quitSize.Height - 10

    End Sub

End Class
