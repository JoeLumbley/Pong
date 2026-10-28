Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class AIDifficultyLayout
    Implements IDisposable

#Region "Constants"

    Public Const TitleText As String = "Difficulty"

    Public Const EasyText As String = "Easy"
    Public Const NormalText As String = "Normal"
    Public Const HardText As String = "Hard"

    Public Const InfoText As String = "Press SPACE to Start"


    Public Const KeyboardHintsText As String =
    "E - Easy   N - Normal   H - Hard   Enter - Start Match"

    Public Const FullscreenText As String =
    "F - Fullscreen"

    Public Const ExitFullscreenText As String =
    "F - Exit Fullscreen"

    Public Const HideHintsText As String =
    "CTRL+H - Hide Keyboard Hints"


#End Region

#Region "Title"

    Public Property TitleSize As SizeF

    Public Property TitleX As Single
    Public Property TitleY As Single

#End Region

#Region "Easy"

    Public Property EasySize As SizeF

    Public Property EasyX As Single
    Public Property EasyY As Single

    Public Property EasyRect As Rectangle

    Public Property EasyPath As GraphicsPath

#End Region

#Region "Normal"

    Public Property NormalSize As SizeF

    Public Property NormalX As Single
    Public Property NormalY As Single

    Public Property NormalRect As Rectangle

    Public Property NormalPath As GraphicsPath

#End Region

#Region "Hard"

    Public Property HardSize As SizeF

    Public Property HardX As Single
    Public Property HardY As Single

    Public Property HardRect As Rectangle

    Public Property HardPath As GraphicsPath

#End Region

#Region "Info"

    Public Property InfoSize As SizeF

    Public Property InfoX As Single
    Public Property InfoY As Single

#End Region

#Region "Hints"

    Public Property KeyboardHintsSize As SizeF

    Public Property KeyboardHintsX As Single
    Public Property KeyboardHintsY As Single

    Public Property FullscreenHintSize As SizeF

    Public Property FullscreenHintX As Single
    Public Property FullscreenHintY As Single

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

            ' -----------------------------------
            ' Title
            ' -----------------------------------

            TitleSize =
                g.MeasureString(
                    TitleText,
                    resources.AITitleFont)

            TitleX =
                (form.ClientSize.Width -
                 TitleSize.Width) / 2.0F

            TitleY =
                form.ClientSize.Height * 0.2F

            ' -----------------------------------
            ' Options
            ' -----------------------------------

            Dim baseY As Single =
                form.ClientSize.Height * 0.4F

            Dim spacing As Single =
                form.ClientSize.Height * 0.055F


            'Dim horizontalPadding As Integer = 12

            'Dim verticalPadding As Integer = 6

            Dim horizontalPadding As Integer =
                Math.Max(
                    4,
                    CInt(form.ClientSize.Height / 70.0F))

            Dim verticalPadding As Integer =
                Math.Max(
                    4,
                    CInt(form.ClientSize.Height / 160.0F))





            EasySize =
                g.MeasureString(
                    EasyText,
                    resources.StartMenuFont)

            EasyX =
                (form.ClientSize.Width -
                 EasySize.Width) / 2.0F

            EasyY = baseY

            'EasyRect =
            '    New Rectangle(
            '        CInt(EasyX),
            '        CInt(EasyY),
            '        CInt(EasySize.Width),
            '        CInt(EasySize.Height))

            EasyRect =
                New Rectangle(
                    CInt(EasyX - horizontalPadding),
                    CInt(EasyY - verticalPadding),
                    CInt(EasySize.Width +
                         horizontalPadding * 2),
                    CInt(EasySize.Height +
                         verticalPadding * 2))

            NormalSize =
                g.MeasureString(
                    NormalText,
                    resources.StartMenuFont)

            NormalX =
                (form.ClientSize.Width -
                 NormalSize.Width) / 2.0F

            NormalY =
                EasyY +
                EasySize.Height +
                spacing

            'NormalRect =
            '    New Rectangle(
            '        CInt(NormalX),
            '        CInt(NormalY),
            '        CInt(NormalSize.Width),
            '        CInt(NormalSize.Height))


            NormalRect =
                New Rectangle(
                    CInt(NormalX - horizontalPadding),
                    CInt(NormalY - verticalPadding),
                    CInt(NormalSize.Width +
                         horizontalPadding * 2),
                    CInt(NormalSize.Height +
                         verticalPadding * 2))

            HardSize =
                g.MeasureString(
                    HardText,
                    resources.StartMenuFont)

            HardX =
                (form.ClientSize.Width -
                 HardSize.Width) / 2.0F

            HardY =
                NormalY +
                NormalSize.Height +
                spacing

            'HardRect =
            '    New Rectangle(
            '        CInt(HardX),
            '        CInt(HardY),
            '        CInt(HardSize.Width),
            '        CInt(HardSize.Height))





            HardRect =
                New Rectangle(
                    CInt(HardX - horizontalPadding),
                    CInt(HardY - verticalPadding),
                    CInt(HardSize.Width +
                         horizontalPadding * 2),
                    CInt(HardSize.Height +
                         verticalPadding * 2))

            ' -----------------------------------
            ' Info
            ' -----------------------------------

            InfoSize =
                g.MeasureString(
                    InfoText,
                    resources.StartInfoFont)

            InfoX =
                (form.ClientSize.Width -
                 InfoSize.Width) / 2.0F

            InfoY =
                form.ClientSize.Height * 0.8F




            ' -----------------------------------
            ' Hints
            ' -----------------------------------

            KeyboardHintsSize =
            g.MeasureString(
                KeyboardHintsText,
                resources.KeyboardFont)

            KeyboardHintsX = 10
            KeyboardHintsY = 10






            'Dim fsText As String = ExitFullscreenText

            'If Not model.IsFullscreen Then

            '    fsText = "F - Fullscreen"
            'End If

            Dim fsText As String =
        If(model.IsFullscreen,
           AIDifficultyLayout.ExitFullscreenText,
           AIDifficultyLayout.FullscreenText)


            FullscreenHintSize =
            g.MeasureString(
                fsText,
                resources.KeyboardFont)

            FullscreenHintX =
            form.ClientSize.Width -
            FullscreenHintSize.Width -
            10

            FullscreenHintY = 10
















            HideHintsSize =
            g.MeasureString(
                HideHintsText,
                resources.KeyboardFont)

                HideHintsX = 10

                HideHintsY =
            form.ClientSize.Height -
            HideHintsSize.Height -
            10













        End Using

        CreatePaths(form)




    End Sub

#End Region

#Region "Paths"

    Private Sub CreatePaths(form As Form)

        DisposePaths()

        Dim radius As Integer =
            Math.Max(6, form.ClientSize.Height \ 64)

        EasyPath =
            CreateRoundedRectangle(
                EasyRect,
                radius)

        NormalPath =
            CreateRoundedRectangle(
                NormalRect,
                radius)

        HardPath =
            CreateRoundedRectangle(
                HardRect,
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

        EasyPath?.Dispose()
        NormalPath?.Dispose()
        HardPath?.Dispose()

        EasyPath = Nothing
        NormalPath = Nothing
        HardPath = Nothing

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        DisposePaths()

    End Sub

#End Region

End Class