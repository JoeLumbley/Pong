Imports Pong.Enums

Public Class GameModel
    Implements IDisposable




    Public Property Input As New InputState

    ' ============================================================
    ' GAME STATE
    ' ============================================================
    Public Property State As GameState = GameState.StartScreen

    ' ============================================================
    ' PLAYER MODE / MENU SELECTIONS
    ' ============================================================
    Public Property PlayerMode As Integer = NumberOfPlayers.OnePlayer
    Public Property PlayerSelection As NumberOfPlayers = NumberOfPlayers.OnePlayer
    Public Property AISelection As AIDifficultyLevel = AIDifficultyLevel.Easy

    Public Property PauseSelection As Integer = PauseMenu.ResumeMatch


    ' ============================================================
    ' PLAYER NAMES
    ' ============================================================
    Public Property LeftPlayerName As String = "Left"
    Public Property RightPlayerName As String = "Right"

    ' ============================================================
    ' BALL / PHYSICS
    ' ============================================================
    Public Property BallPos As PointF
    Public Property BallDiameter As Integer = 60

    Public Property VelX As Double
    Public Property VelY As Double
    Public Property BallSpeed As Double = 200

    ' ============================================================
    ' PADDLES
    ' ============================================================
    Public Property LeftPaddleRect As Rectangle
    Public Property RightPaddleRect As Rectangle

    Public Property PaddleWidth As Integer = 32
    Public Property PaddleHeight As Integer = 128
    Public Property PaddleSpeed As Integer = 700

    ' Movement flags (set by InputManager, consumed by PhysicsEngine)
    'Public Property LeftPaddleUp As Boolean
    'Public Property LeftPaddleDown As Boolean
    'Public Property RightPaddleUp As Boolean
    'Public Property RightPaddleDown As Boolean

    ' Paddle velocity (for spin)
    Public Property LastPaddleLeftY As Single
    Public Property PaddleLeftVelocity As Single
    Public Property LastPaddleRightY As Single
    Public Property PaddleRightVelocity As Single

    ' ============================================================
    ' SCORING
    ' ============================================================
    Public Property ScoreLeft As Integer
    Public Property ScoreRight As Integer

    Public Property WinnerText As String = ""
    Public Property WinnerIs As Integer = 0

    ' ============================================================
    ' AI DIFFICULTY
    ' ============================================================
    Public Property AIDifficultyScale As Double = 1.0
    Public Property AIModeFactor As Double = 1.0

    ' ============================================================
    ' WINDOW / SCALING
    ' ============================================================
    Public Property ClientSize As Size
    Public Property IsFullscreen As Boolean = False

    ' ============================================================
    ' UI SETTINGS
    ' ============================================================
    Public Property ShowKeyboardHints As Boolean = True

    ' ============================================================
    ' MOUSE STATE
    ' ============================================================
    'Public Property MouseIsClicking As Boolean = False

    ' ============================================================
    ' AUDIO COOLDOWN
    ' ============================================================
    Public Property LastPlay As New Dictionary(Of String, Integer)

    ' ============================================================
    ' RANDOM
    ' ============================================================
    Public Property RNG As Random = New Random()

    ' ============================================================
    ' CONSTRUCTOR
    ' ============================================================
    Public Sub New(initialClientSize As Size)
        ClientSize = initialClientSize

        ' Initialize paddles centered
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

        ' Initialize ball centered
        BallPos = New PointF(
            (ClientSize.Width - BallDiameter) / 2.0F,
            (ClientSize.Height - BallDiameter) / 2.0F)
    End Sub

    ' ============================================================
    ' DISPOSAL
    ' ============================================================
    Private disposed As Boolean = False

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposed Then
            If disposing Then
                ' Release managed resources

                RNG = Nothing

                If LastPlay IsNot Nothing Then
                    LastPlay.Clear()
                    LastPlay = Nothing
                End If
            End If

            disposed = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub




End Class
