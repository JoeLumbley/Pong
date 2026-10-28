
'Imports System.Drawing
'Imports System.Drawing.Drawing2D

'Public Class StartScreenLayout
'    Implements IDisposable

'    '#Region "Text"

'    '    Public Const TitleText As String = "PONG"

'    '    Public Const OnePlayerText As String = "1 Player"

'    '    Public Const TwoPlayersText As String = "2 Players"

'    '    Public Const InfoText As String = "Press SPACE to Start"

'    '#End Region

'#Region "Text"

'    Public Const TitleText As String = "PONG"

'    Public Const OnePlayerText As String = "1 Player"

'    Public Const TwoPlayersText As String = "2 Players"

'    Public Const InfoText As String = "Press SPACE to Start"

'    Public Const KeyboardHintsText As String =
'    "1 - One Player   2 - Two Players   Enter - Start Match"

'    Public Const HideHintsText As String =
'    "CTRL+H - Hide Keyboard Hints"

'#End Region














'#Region "Viewport"

'    Public Property ViewportWidth As Integer

'    Public Property ViewportHeight As Integer

'#End Region

'#Region "Title"

'    Public Property TitleSize As SizeF

'    Public Property TitleX As Single

'    Public Property TitleY As Single

'#End Region

'#Region "One Player"

'    Public Property OnePlayerSize As SizeF

'    Public Property OnePlayerX As Single

'    Public Property OnePlayerY As Single

'    Public Property OnePlayerRect As Rectangle

'    Public Property OnePlayerPath As GraphicsPath

'#End Region

'#Region "Two Players"

'    Public Property TwoPlayersSize As SizeF

'    Public Property TwoPlayersX As Single

'    Public Property TwoPlayersY As Single

'    Public Property TwoPlayersRect As Rectangle

'    Public Property TwoPlayersPath As GraphicsPath

'#End Region

'#Region "Info"

'    Public Property InfoSize As SizeF

'    Public Property InfoX As Single

'    Public Property InfoY As Single

'#End Region

'#Region "Style"

'    Public Property Radius As Integer

'#End Region


'#Region "Hints"

'    Public Property KeyboardHintsX As Single
'    Public Property KeyboardHintsY As Single

'    Public Property FullscreenHintX As Single
'    Public Property FullscreenHintY As Single

'    Public Property HideHintsX As Single
'    Public Property HideHintsY As Single

'#End Region

'#Region "Cache"

'    Public Sub Cache(
'        form As Form,
'        resources As GraphicsResources)

'        ViewportWidth = form.ClientSize.Width
'        ViewportHeight = form.ClientSize.Height

'        Using g As Graphics = form.CreateGraphics()

'            ' ---------------------------------
'            ' Title
'            ' ---------------------------------

'            TitleSize =
'                g.MeasureString(
'                    TitleText,
'                    resources.StartTitleFont)

'            TitleX =
'                (ViewportWidth - TitleSize.Width) / 2.0F

'            TitleY =
'                ViewportHeight * 0.15F

'            ' ---------------------------------
'            ' Menu Measurements
'            ' ---------------------------------

'            OnePlayerSize =
'                g.MeasureString(
'                    OnePlayerText,
'                    resources.StartMenuFont)

'            TwoPlayersSize =
'                g.MeasureString(
'                    TwoPlayersText,
'                    resources.StartMenuFont)

'            Dim spacing As Integer =
'                CInt(ViewportHeight * 0.14F)


'            Dim horizontalPadding As Integer =
'                Math.Max(
'                    4,
'                    CInt(ViewportHeight / 70.0F))



'            Dim verticalPadding As Integer =
'                Math.Max(
'                    4,
'                    CInt(ViewportHeight / 160.0F))


'            ' ---------------------------------
'            ' One Player
'            ' ---------------------------------

'            OnePlayerX =
'                (ViewportWidth -
'                 OnePlayerSize.Width) / 2.0F

'            OnePlayerY =
'                ViewportHeight * 0.4F

'            OnePlayerRect =
'                New Rectangle(
'                    CInt(OnePlayerX - horizontalPadding),
'                    CInt(OnePlayerY - verticalPadding),
'                    CInt(OnePlayerSize.Width +
'                         horizontalPadding * 2),
'                    CInt(OnePlayerSize.Height +
'                         verticalPadding * 2))

'            ' ---------------------------------
'            ' Two Players
'            ' ---------------------------------

'            TwoPlayersX =
'                (ViewportWidth -
'                 TwoPlayersSize.Width) / 2.0F

'            TwoPlayersY =
'                OnePlayerY + spacing

'            TwoPlayersRect =
'                New Rectangle(
'                    CInt(TwoPlayersX - horizontalPadding),
'                    CInt(TwoPlayersY - verticalPadding),
'                    CInt(TwoPlayersSize.Width +
'                         horizontalPadding * 2),
'                    CInt(TwoPlayersSize.Height +
'                         verticalPadding * 2))

'            ' ---------------------------------
'            ' Info
'            ' ---------------------------------

'            InfoSize =
'                g.MeasureString(
'                    InfoText,
'                    resources.StartInfoFont)

'            InfoX =
'                (ViewportWidth -
'                 InfoSize.Width) / 2.0F

'            InfoY =
'                ViewportHeight * 0.68F

'            Dim fullscreenSize =
'    g.MeasureString(
'        "F - Exit Fullscreen",
'        resources.KeyboardFont)

'            FullscreenHintX =
'                ViewportWidth -
'                fullscreenSize.Width -
'                10







'        End Using

'        Radius =
'            Math.Max(
'                8,
'                ViewportHeight \ 60)

'        CreatePaths()


'        ' ---------------------------------
'        ' Hints
'        ' ---------------------------------

'        KeyboardHintsX = 10
'        KeyboardHintsY = 10

'        FullscreenHintY = 10

'        HideHintsX = 10



'    End Sub

'#End Region

'#Region "Path Creation"

'    Private Sub CreatePaths()

'        DisposePaths()

'        OnePlayerPath =
'            CreateRoundedRectangle(
'                OnePlayerRect,
'                Radius)

'        TwoPlayersPath =
'            CreateRoundedRectangle(
'                TwoPlayersRect,
'                Radius)

'    End Sub

'    Private Function CreateRoundedRectangle(
'        rect As Rectangle,
'        radius As Integer) As GraphicsPath

'        Dim diameter As Integer =
'            radius * 2

'        Dim path As New GraphicsPath()

'        path.StartFigure()

'        path.AddArc(
'            rect.Left,
'            rect.Top,
'            diameter,
'            diameter,
'            180,
'            90)

'        path.AddArc(
'            rect.Right - diameter,
'            rect.Top,
'            diameter,
'            diameter,
'            270,
'            90)

'        path.AddArc(
'            rect.Right - diameter,
'            rect.Bottom - diameter,
'            diameter,
'            diameter,
'            0,
'            90)

'        path.AddArc(
'            rect.Left,
'            rect.Bottom - diameter,
'            diameter,
'            diameter,
'            90,
'            90)

'        path.CloseFigure()

'        Return path

'    End Function

'#End Region

'#Region "Dispose"

'    Private Sub DisposePaths()

'        OnePlayerPath?.Dispose()
'        TwoPlayersPath?.Dispose()

'        OnePlayerPath = Nothing
'        TwoPlayersPath = Nothing

'    End Sub

'    Public Sub Dispose() Implements IDisposable.Dispose

'        DisposePaths()

'    End Sub

'#End Region

'End Class



Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class StartScreenLayout
    Implements IDisposable

#Region "Text"

    Public Const TitleText As String = "PONG"

    Public Const OnePlayerText As String = "1 Player"

    Public Const TwoPlayersText As String = "2 Players"

    Public Const InfoText As String = "Press SPACE to Start"

    Public Const KeyboardHintsText As String =
        "1 - One Player   2 - Two Players   Enter - Start Match"

    Public Const HideHintsText As String =
        "CTRL+H - Hide Keyboard Hints"

#End Region

#Region "Viewport"

    Public Property ViewportWidth As Integer

    Public Property ViewportHeight As Integer

#End Region

#Region "Title"

    Public Property TitleSize As SizeF

    Public Property TitleX As Single

    Public Property TitleY As Single

#End Region

#Region "One Player"

    Public Property OnePlayerSize As SizeF

    Public Property OnePlayerX As Single

    Public Property OnePlayerY As Single

    Public Property OnePlayerRect As Rectangle

    Public Property OnePlayerPath As GraphicsPath

#End Region

#Region "Two Players"

    Public Property TwoPlayersSize As SizeF

    Public Property TwoPlayersX As Single

    Public Property TwoPlayersY As Single

    Public Property TwoPlayersRect As Rectangle

    Public Property TwoPlayersPath As GraphicsPath

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

#Region "Style"

    Public Property Radius As Integer

#End Region

#Region "Cache"

    Public Sub Cache(
        form As Form,
        resources As GraphicsResources,
        model As GameModel)

        ViewportWidth = form.ClientSize.Width
        ViewportHeight = form.ClientSize.Height

        Using g As Graphics = form.CreateGraphics()

            ' ---------------------------------
            ' Title
            ' ---------------------------------

            TitleSize =
                g.MeasureString(
                    TitleText,
                    resources.StartTitleFont)

            TitleX =
                (ViewportWidth - TitleSize.Width) / 2.0F

            TitleY =
                ViewportHeight * 0.15F

            ' ---------------------------------
            ' Menu Measurements
            ' ---------------------------------

            OnePlayerSize =
                g.MeasureString(
                    OnePlayerText,
                    resources.StartMenuFont)

            TwoPlayersSize =
                g.MeasureString(
                    TwoPlayersText,
                    resources.StartMenuFont)

            Dim spacing As Integer =
                CInt(ViewportHeight * 0.14F)

            Dim horizontalPadding As Integer =
                Math.Max(
                    4,
                    CInt(ViewportHeight / 70.0F))

            Dim verticalPadding As Integer =
                Math.Max(
                    4,
                    CInt(ViewportHeight / 160.0F))

            ' ---------------------------------
            ' One Player
            ' ---------------------------------

            OnePlayerX =
                (ViewportWidth -
                 OnePlayerSize.Width) / 2.0F

            OnePlayerY =
                ViewportHeight * 0.4F

            OnePlayerRect =
                New Rectangle(
                    CInt(OnePlayerX - horizontalPadding),
                    CInt(OnePlayerY - verticalPadding),
                    CInt(OnePlayerSize.Width +
                         horizontalPadding * 2),
                    CInt(OnePlayerSize.Height +
                         verticalPadding * 2))

            ' ---------------------------------
            ' Two Players
            ' ---------------------------------

            TwoPlayersX =
                (ViewportWidth -
                 TwoPlayersSize.Width) / 2.0F

            TwoPlayersY =
                OnePlayerY + spacing

            TwoPlayersRect =
                New Rectangle(
                    CInt(TwoPlayersX - horizontalPadding),
                    CInt(TwoPlayersY - verticalPadding),
                    CInt(TwoPlayersSize.Width +
                         horizontalPadding * 2),
                    CInt(TwoPlayersSize.Height +
                         verticalPadding * 2))

            ' ---------------------------------
            ' Info
            ' ---------------------------------

            InfoSize =
                g.MeasureString(
                    InfoText,
                    resources.StartInfoFont)

            InfoX =
                (ViewportWidth -
                 InfoSize.Width) / 2.0F

            InfoY =
                ViewportHeight * 0.68F

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
                    "F - Exit Fullscreen",
                    resources.KeyboardFont)

            Else

                FullscreenHintSize =
                g.MeasureString(
                    "F - Fullscreen",
                    resources.KeyboardFont)


            End If





            'full


            FullscreenHintX =
                ViewportWidth -
                FullscreenHintSize.Width -
                10

            FullscreenHintY = 10














            HideHintsSize =
                g.MeasureString(
                    HideHintsText,
                    resources.KeyboardFont)

                HideHintsX = 10

                HideHintsY =
                ViewportHeight -
                HideHintsSize.Height -
                10

        End Using

        Radius =
            Math.Max(
                8,
                ViewportHeight \ 60)

        CreatePaths()

    End Sub

#End Region

#Region "Path Creation"

    Private Sub CreatePaths()

        DisposePaths()

        OnePlayerPath =
            CreateRoundedRectangle(
                OnePlayerRect,
                Radius)

        TwoPlayersPath =
            CreateRoundedRectangle(
                TwoPlayersRect,
                Radius)

    End Sub

    Private Function CreateRoundedRectangle(
        rect As Rectangle,
        radius As Integer) As GraphicsPath

        Dim diameter As Integer =
            radius * 2

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

        OnePlayerPath?.Dispose()
        TwoPlayersPath?.Dispose()

        OnePlayerPath = Nothing
        TwoPlayersPath = Nothing

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        DisposePaths()

    End Sub

#End Region

End Class


