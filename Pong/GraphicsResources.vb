Imports System.Drawing
Imports System.Drawing.Drawing2D







Public Class GraphicsResources
    Implements IDisposable

#Region "Brushes"

    Public Property BallBrush As SolidBrush
    Public Property PaddleBrush As SolidBrush
    Public Property ScoreBrush As SolidBrush
    Public Property LabelBrush As SolidBrush

    Public Property WhiteBrush As SolidBrush










    Public Property GrayBrush As SolidBrush
    Public Property DimBrush As SolidBrush

    Public Property FullscreenIndicatorBrush As SolidBrush

    Public Property LightBrush As SolidBrush
    Public Property DarkBrush As SolidBrush

#End Region

#Region "Pens"

    Public Property OutlinePen As Pen
    Public Property DarkOutlinePen As Pen

#End Region

#Region "Fonts"

    Public Property HudScoreFont As Font
    Public Property HudLabelFont As Font

    Public Property PauseTitleFont As Font
    Public Property PauseMenuFont As Font

    Public Property StartTitleFont As Font
    Public Property StartMenuFont As Font
    Public Property StartInfoFont As Font

    Public Property AITitleFont As Font

    Public Property GameOverFont As Font
    Public Property GameOverInfoFont As Font

    Public Property KeyboardFont As Font
    Public Property FPSFont As Font



#End Region

    Public Sub Initialize(form As Form)

        CreateBrushes()
        CreatePens()
        ScaleFonts(form.ClientSize.Height)

    End Sub

    Public Sub Rescale(form As Form,
                       model As GameModel, layout As Layout)

        DisposeFonts()
        ScaleFonts(form.ClientSize.Height)


        layout.RescalePaddles(form.ClientSize, model)
        layout.ScaleBall(model)


    End Sub

    Private Sub CreateBrushes()

        BallBrush = New SolidBrush(Color.DeepSkyBlue)
        PaddleBrush = New SolidBrush(Color.White)

        ScoreBrush = New SolidBrush(Color.White)
        LabelBrush = New SolidBrush(Color.Gray)

        WhiteBrush = New SolidBrush(Color.White)



        GrayBrush = New SolidBrush(Color.FromArgb(
            140,
            140,
            140))

        DimBrush = New SolidBrush(Color.FromArgb(
            120,
            0,
            0,
            0))

        FullscreenIndicatorBrush = New SolidBrush(Color.FromArgb(
            120,
            255,
            255,
            255))

        LightBrush = New SolidBrush(Color.FromArgb(
            255,
            42,
            42,
            42))

        DarkBrush = New SolidBrush(Color.FromArgb(
            64,
            0,
            0,
            0))

    End Sub

    Private Sub CreatePens()

        OutlinePen = New Pen(
            Color.FromArgb(255, 64, 64, 64),
            2)

        DarkOutlinePen = New Pen(
            Color.FromArgb(32, 0, 0, 0),
            2)

    End Sub

    Private Sub ScaleFonts(clientHeight As Integer)

        HudScoreFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 15.0F),
            FontStyle.Bold)

        HudLabelFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 55.0F),
            FontStyle.Bold)

        PauseTitleFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 18.0F),
            FontStyle.Bold)

        PauseMenuFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 28.0F))

        StartTitleFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 11.0F),
            FontStyle.Bold)

        StartMenuFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 30.0F))

        StartInfoFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 45.0F))

        AITitleFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 18.0F),
            FontStyle.Bold)

        GameOverFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 14.0F),
            FontStyle.Bold)

        GameOverInfoFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 40.0F))

        KeyboardFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 70.0F),
            FontStyle.Bold)

        FPSFont = New Font(
            "Segoe UI",
            CSng(clientHeight / 75.0F),
            FontStyle.Bold)

    End Sub

    Public Sub UpdateBallBrush(color As Color)

        BallBrush?.Dispose()
        BallBrush = New SolidBrush(color)

    End Sub

    Private Sub DisposeFonts()

        HudScoreFont?.Dispose()
        HudLabelFont?.Dispose()

        PauseTitleFont?.Dispose()
        PauseMenuFont?.Dispose()

        StartTitleFont?.Dispose()
        StartMenuFont?.Dispose()
        StartInfoFont?.Dispose()

        AITitleFont?.Dispose()

        GameOverFont?.Dispose()
        GameOverInfoFont?.Dispose()

        KeyboardFont?.Dispose()
        FPSFont?.Dispose()

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        DisposeFonts()

        BallBrush?.Dispose()
        PaddleBrush?.Dispose()

        ScoreBrush?.Dispose()
        LabelBrush?.Dispose()

        WhiteBrush?.Dispose()
        GrayBrush?.Dispose()
        DimBrush?.Dispose()

        FullscreenIndicatorBrush?.Dispose()

        LightBrush?.Dispose()
        DarkBrush?.Dispose()

        OutlinePen?.Dispose()
        DarkOutlinePen?.Dispose()

    End Sub

End Class



