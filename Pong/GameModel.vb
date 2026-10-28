'Imports Pong.Enums

'Public Class GameModel
'    Implements IDisposable




'    Public Property Input As New InputState

'    ' ============================================================
'    ' GAME STATE
'    ' ============================================================
'    Public Property State As GameState = GameState.StartScreen

'    ' ============================================================
'    ' PLAYER MODE / MENU SELECTIONS
'    ' ============================================================
'    Public Property PlayerMode As Integer = NumberOfPlayers.OnePlayer
'    Public Property PlayerSelection As NumberOfPlayers = NumberOfPlayers.OnePlayer
'    Public Property AISelection As AIDifficultyLevel = AIDifficultyLevel.Easy

'    Public Property PauseSelection As Integer = PauseMenu.ResumeMatch


'    ' ============================================================
'    ' PLAYER NAMES
'    ' ============================================================
'    Public Property LeftPlayerName As String = "Left"
'    Public Property RightPlayerName As String = "Right"

'    ' ============================================================
'    ' BALL / PHYSICS
'    ' ============================================================
'    Public Property BallPos As PointF
'    Public Property BallDiameter As Integer = 60

'    Public Property VelX As Double
'    Public Property VelY As Double
'    Public Property BallSpeed As Double = 200

'    ' ============================================================
'    ' PADDLES
'    ' ============================================================
'    Public Property LeftPaddleRect As Rectangle
'    Public Property RightPaddleRect As Rectangle

'    Public Property PaddleWidth As Integer = 32
'    Public Property PaddleHeight As Integer = 128
'    Public Property PaddleSpeed As Integer = 700


'    ' Paddle velocity (for spin)
'    Public Property LastPaddleLeftY As Single
'    Public Property PaddleLeftVelocity As Single
'    Public Property LastPaddleRightY As Single
'    Public Property PaddleRightVelocity As Single

'    ' ============================================================
'    ' SCORING
'    ' ============================================================
'    Public Property ScoreLeft As Integer
'    Public Property ScoreRight As Integer

'    Public Property WinnerText As String = ""
'    Public Property WinnerIs As Integer = 0

'    ' ============================================================
'    ' AI DIFFICULTY
'    ' ============================================================
'    Public Property AIDifficultyScale As Double = 1.0
'    Public Property AIModeFactor As Double = 1.0

'    ' ============================================================
'    ' WINDOW / SCALING
'    ' ============================================================
'    Public Property ClientSize As Size
'    Public Property IsFullscreen As Boolean = False

'    ' ============================================================
'    ' UI SETTINGS
'    ' ============================================================
'    Public Property ShowKeyboardHints As Boolean = True

'    ' ============================================================
'    ' MOUSE STATE
'    ' ============================================================
'    'Public Property MouseIsClicking As Boolean = False

'    ' ============================================================
'    ' AUDIO COOLDOWN
'    ' ============================================================
'    Public Property LastPlay As New Dictionary(Of String, Integer)

'    ' ============================================================
'    ' RANDOM
'    ' ============================================================
'    Public Property RNG As Random = New Random()

'    ' ============================================================
'    ' CONSTRUCTOR
'    ' ============================================================
'    Public Sub New(initialClientSize As Size)
'        ClientSize = initialClientSize

'        ' Initialize paddles centered
'        LeftPaddleRect = New Rectangle(
'            50,
'            (ClientSize.Height - PaddleHeight) \ 2,
'            PaddleWidth,
'            PaddleHeight)

'        RightPaddleRect = New Rectangle(
'            ClientSize.Width - 50 - PaddleWidth,
'            (ClientSize.Height - PaddleHeight) \ 2,
'            PaddleWidth,
'            PaddleHeight)

'        ' Initialize ball centered
'        BallPos = New PointF(
'            (ClientSize.Width - BallDiameter) / 2.0F,
'            (ClientSize.Height - BallDiameter) / 2.0F)
'    End Sub

'    ' ============================================================
'    ' DISPOSAL
'    ' ============================================================
'    Private disposed As Boolean = False

'    Protected Overridable Sub Dispose(disposing As Boolean)
'        If Not disposed Then
'            If disposing Then
'                ' Release managed resources

'                RNG = Nothing

'                If LastPlay IsNot Nothing Then
'                    LastPlay.Clear()
'                    LastPlay = Nothing
'                End If
'            End If

'            disposed = True
'        End If
'    End Sub

'    Public Sub Dispose() Implements IDisposable.Dispose
'        Dispose(True)
'        GC.SuppressFinalize(Me)
'    End Sub




'End Class




Imports Pong.Enums

Public Class GameModel
    Implements IDisposable

#Region "Input"

    Public Property Input As New InputState

#End Region

#Region "Game State"

    Public Property State As GameState =
        GameState.StartScreen

#End Region

#Region "Menu Selections"

    Public Property PlayerMode As NumberOfPlayers =
        NumberOfPlayers.OnePlayer

    Public Property AISelection As AIDifficultyLevel =
        AIDifficultyLevel.Easy

    Public Property PauseMenuSelection As PauseMenu =
        PauseMenu.ResumeMatch

#End Region

#Region "Player Names"

    Public Property LeftPlayerName As String = "Left"

    Public Property RightPlayerName As String = "Right"

#End Region

#Region "Ball / Physics"

    Public Property BallPos As PointF

    Public Property BallDiameter As Integer = 60

    Public Property VelX As Double

    Public Property VelY As Double

    Public Property BallSpeed As Double = 75

#End Region

#Region "Paddles"

    Public Property LeftPaddleRect As Rectangle

    Public Property RightPaddleRect As Rectangle

    Public Property PaddleWidth As Integer = 32

    Public Property PaddleHeight As Integer = 128

    Public Property PaddleSpeed As Integer = 700

    Public Property LastPaddleLeftY As Single

    Public Property PaddleLeftVelocity As Single

    Public Property LastPaddleRightY As Single

    Public Property PaddleRightVelocity As Single

#End Region

#Region "Scoring"

    Public Property ScoreLeft As Integer

    Public Property ScoreRight As Integer

    Public Property WinnerText As String = ""

    Public Property WinnerIs As Integer = 0

#End Region

#Region "AI"

    Public Property AIDifficultyScale As Double = 1.0

    Public Property AIModeFactor As Double = 1.0

#End Region

#Region "Window / Scaling"

    Public Property ClientSize As Size

    Public Property IsFullscreen As Boolean = False

#End Region

#Region "UI"

    Public Property ShowKeyboardHints As Boolean = True

#End Region

#Region "Audio"

    Public Property LastPlay As New Dictionary(Of String, Integer)

#End Region

#Region "Random"

    Public Property RNG As New Random()

#End Region



#Region "Performance"

    Public Property FPS As Integer

#End Region







    Public Property dt As Double














#Region "Constructor"

    Public Sub New(initialClientSize As Size)

        ClientSize = initialClientSize

        LeftPaddleRect = New Rectangle(
            50,
            (ClientSize.Height - PaddleHeight) \ 2,
            PaddleWidth,
            PaddleHeight)

        RightPaddleRect = New Rectangle(
            ClientSize.Width - 50 - PaddleWidth,
            (ClientSize.Height - PaddleHeight) \ 2,
            PaddleWidth,
            PaddleHeight)

        BallPos = New PointF(
            (ClientSize.Width - BallDiameter) / 2.0F,
            (ClientSize.Height - BallDiameter) / 2.0F)

    End Sub

#End Region

#Region "Dispose"

    Private disposed As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)

        If disposed Then Return

        If disposing Then

            RNG = Nothing

            If LastPlay IsNot Nothing Then

                LastPlay.Clear()
                LastPlay = Nothing

            End If

        End If

        disposed = True

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        Dispose(True)

        GC.SuppressFinalize(Me)

    End Sub

#End Region

End Class


