Imports System.Drawing
Imports System.Drawing.Drawing2D

Imports Pong.Enums

Public Class PauseMenuLayout
    Implements IDisposable

#Region "Constants"

    Public Const PauseTitle As String = "PAUSED"

    Public Const ResumeText As String = "Resume"
    Public Const NewGameText As String = "New"
    Public Const QuitText As String = "Quit"

#End Region

#Region "Title"

    Public Property TitleSize As SizeF

    Public Property TitleX As Single
    Public Property TitleY As Single

#End Region

#Region "Resume"

    Public Property ResumeSize As SizeF

    Public Property ResumeX As Single
    Public Property ResumeY As Single

    Public Property ResumeRect As Rectangle

    Public Property ResumePath As GraphicsPath

#End Region

#Region "New Game"

    Public Property NewGameSize As SizeF

    Public Property NewGameX As Single
    Public Property NewGameY As Single

    Public Property NewGameRect As Rectangle

    Public Property NewGamePath As GraphicsPath

#End Region

#Region "Quit"

    Public Property QuitSize As SizeF

    Public Property QuitX As Single
    Public Property QuitY As Single

    Public Property QuitRect As Rectangle

    Public Property QuitPath As GraphicsPath

#End Region

#Region "Layout"

    Public Property MenuSpacing As Single

#End Region




#Region "Hint Constants"

    Public Const KeyboardHintsText As String =
    "R - Resume Match   N - New Match"

    Public Const FullscreenText As String =
    "F - Fullscreen"

    Public Const ExitFullscreenText As String =
    "F - Exit Fullscreen"

    Public Const QuitHintText As String =
    "Q - Quit Match"

    Public Const HideHintsText As String =
    "CTRL+H - Hide Keyboard Hints"

#End Region


#Region "Hints"

    Public Property KeyboardHintsSize As SizeF
    Public Property KeyboardHintsX As Single
    Public Property KeyboardHintsY As Single

    Public Property FullscreenHintSize As SizeF
    Public Property FullscreenHintX As Single
    Public Property FullscreenHintY As Single

    Public Property QuitHintSize As SizeF
    Public Property QuitHintX As Single
    Public Property QuitHintY As Single

    Public Property HideHintsSize As SizeF
    Public Property HideHintsX As Single
    Public Property HideHintsY As Single

#End Region









#Region "Cache"

    Public Sub Cache(
        form As Form,
        resources As GraphicsResources,
        model As GameModel)

        Using g As Graphics = form.CreateGraphics()

            TitleSize =
                g.MeasureString(
                    PauseTitle,
                    resources.PauseTitleFont)

            TitleX =
                (form.ClientSize.Width -
                 TitleSize.Width) / 2.0F

            TitleY =
                form.ClientSize.Height * 0.21F







            MenuSpacing =
                form.ClientSize.Height * 0.15F


            Dim horizontalPadding As Integer =
                Math.Max(
                    4,
                    CInt(form.ClientSize.Height / 70.0F))

            Dim verticalPadding As Integer =
                Math.Max(
                    4,
                    CInt(form.ClientSize.Height / 160.0F))










            ' ---------------------------------
            ' Resume
            ' ---------------------------------

            ResumeSize =
                g.MeasureString(
                    ResumeText,
                    resources.PauseMenuFont)

            ResumeX =
                (form.ClientSize.Width -
                 ResumeSize.Width) / 2.0F

            ResumeY =
                TitleY +
                TitleSize.Height +
                (form.ClientSize.Height * 0.05F)

            'ResumeRect =
            '    New Rectangle(
            '        CInt(ResumeX),
            '        CInt(ResumeY),
            '        CInt(ResumeSize.Width),
            '        CInt(ResumeSize.Height))

            ResumeRect =
                New Rectangle(
                    CInt(ResumeX - horizontalPadding),
                    CInt(ResumeY - verticalPadding),
                    CInt(ResumeSize.Width +
                         horizontalPadding * 2),
                    CInt(ResumeSize.Height +
                         verticalPadding * 2))

            ' ---------------------------------
            ' New
            ' ---------------------------------

            NewGameSize =
                g.MeasureString(
                    NewGameText,
                    resources.PauseMenuFont)

            NewGameX =
                (form.ClientSize.Width -
                 NewGameSize.Width) / 2.0F

            NewGameY =
                ResumeY + MenuSpacing

            'NewGameRect =
            '    New Rectangle(
            '        CInt(NewGameX),
            '        CInt(NewGameY),
            '        CInt(NewGameSize.Width),
            '        CInt(NewGameSize.Height))


            NewGameRect =
                New Rectangle(
                    CInt(NewGameX - horizontalPadding),
                    CInt(NewGameY - verticalPadding),
                    CInt(NewGameSize.Width +
                         horizontalPadding * 2),
                    CInt(NewGameSize.Height +
                         verticalPadding * 2))

            ' ---------------------------------
            ' Quit
            ' ---------------------------------

            QuitSize =
                g.MeasureString(
                    QuitText,
                    resources.PauseMenuFont)

            QuitX =
                (form.ClientSize.Width -
                 QuitSize.Width) / 2.0F

            QuitY =
                NewGameY + MenuSpacing

            'QuitRect =
            '    New Rectangle(
            '        CInt(QuitX),
            '        CInt(QuitY),
            '        CInt(QuitSize.Width),
            '        CInt(QuitSize.Height))

            QuitRect =
                New Rectangle(
                    CInt(QuitX - horizontalPadding),
                    CInt(QuitY - verticalPadding),
                    CInt(QuitSize.Width +
                         horizontalPadding * 2),
                    CInt(QuitSize.Height +
                         verticalPadding * 2))




            ' ---------------------------------
            ' Hints
            ' ---------------------------------

            KeyboardHintsSize =
                g.MeasureString(
                    KeyboardHintsText,
                    resources.KeyboardFont)

            KeyboardHintsX = 10
            KeyboardHintsY = 10










            If model.IsFullscreen Then


                FullscreenHintSize =
                g.MeasureString(
                    ExitFullscreenText,
                    resources.KeyboardFont)

                FullscreenHintX =
                form.ClientSize.Width -
                FullscreenHintSize.Width -
                10

                FullscreenHintY = 10


            Else
                FullscreenHintSize =
                g.MeasureString(
                    FullscreenText,
                    resources.KeyboardFont)

                FullscreenHintX =
                form.ClientSize.Width -
                FullscreenHintSize.Width -
                10

                FullscreenHintY = 10




            End If


            'FullscreenHintSize =
            '    g.MeasureString(
            '        ExitFullscreenText,
            '        resources.KeyboardFont)

            '    FullscreenHintX =
            '    form.ClientSize.Width -
            '    FullscreenHintSize.Width -
            '    10

            '    FullscreenHintY = 10
















            QuitHintSize =
                g.MeasureString(
                    QuitHintText,
                    resources.KeyboardFont)

                QuitHintX = 10

                QuitHintY =
                form.ClientSize.Height -
                QuitHintSize.Height -
                10

                HideHintsSize =
                g.MeasureString(
                    HideHintsText,
                    resources.KeyboardFont)

                HideHintsX =
                form.ClientSize.Width -
                HideHintsSize.Width -
                10

                HideHintsY =
                form.ClientSize.Height -
                HideHintsSize.Height -
                10




        End Using

        CreatePaths(form)

    End Sub

#End Region

#Region "Path Creation"

    Private Sub CreatePaths(form As Form)

        DisposePaths()

        Dim radius As Integer =
            Math.Max(6, form.ClientSize.Height \ 64)

        ResumePath =
            CreateRoundedRectangle(
                ResumeRect,
                radius)

        NewGamePath =
            CreateRoundedRectangle(
                NewGameRect,
                radius)

        QuitPath =
            CreateRoundedRectangle(
                QuitRect,
                radius)

    End Sub

    Private Function CreateRoundedRectangle(
        rect As Rectangle,
        radius As Integer) As GraphicsPath

        Dim diameter As Integer = radius * 2

        Dim path As New GraphicsPath()

        path.StartFigure()

        path.AddArc(
            rect.Left,
            rect.Top,
            diameter,
            diameter,
            180,
            90)

        path.AddArc(
            rect.Right - diameter,
            rect.Top,
            diameter,
            diameter,
            270,
            90)

        path.AddArc(
            rect.Right - diameter,
            rect.Bottom - diameter,
            diameter,
            diameter,
            0,
            90)

        path.AddArc(
            rect.Left,
            rect.Bottom - diameter,
            diameter,
            diameter,
            90,
            90)

        path.CloseFigure()

        Return path

    End Function

#End Region

#Region "Dispose"

    Private Sub DisposePaths()

        ResumePath?.Dispose()
        NewGamePath?.Dispose()
        QuitPath?.Dispose()

        ResumePath = Nothing
        NewGamePath = Nothing
        QuitPath = Nothing

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        DisposePaths()

    End Sub

#End Region

End Class