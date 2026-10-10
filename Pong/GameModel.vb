Imports Pong.Enums

Public Class GameModel

    ' ============================================================
    ' GAME STATE
    ' ============================================================
    Public Property State As GameState = GameState.StartScreen

    ' ============================================================
    ' PLAYER MODE / MENU SELECTIONS
    ' ============================================================
    Public Property PlayerMode As Integer = 1   ' 1 = Single Player, 2 = Two Players
    Public Property PlayerSelection As NumberOfPlayers = NumberOfPlayers.OnePlayer

    Public Property AISelection As AIDifficultyLevel = AIDifficultyLevel.Easy
    Public Property PauseSelection As Integer = 0

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
    Public Property PaddleLeft As Rectangle
    Public Property PaddleRight As Rectangle

    Public Property PaddleWidth As Integer = 32
    Public Property PaddleHeight As Integer = 128
    Public Property PaddleSpeed As Integer = 700

    ' Movement flags (set by InputManager, consumed by PhysicsEngine)
    Public Property LeftPaddleUp As Boolean = False
    Public Property LeftPaddleDown As Boolean = False
    Public Property RightPaddleUp As Boolean = False
    Public Property RightPaddleDown As Boolean = False

    ' Paddle velocity (for spin)
    Public Property LastPaddleLeftY As Single
    Public Property PaddleLeftVelocity As Single
    Public Property LastPaddleRightY As Single
    Public Property PaddleRightVelocity As Single

    ' ============================================================
    ' SCORING
    ' ============================================================
    Public Property ScoreLeft As Integer = 0
    Public Property ScoreRight As Integer = 0

    Public Property WinnerText As String = ""

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
    Public Property MouseIsClicking As Boolean = False

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
        PaddleLeft = New Rectangle(50,
                                   (ClientSize.Height - PaddleHeight) \ 2,
                                   PaddleWidth,
                                   PaddleHeight)

        PaddleRight = New Rectangle(ClientSize.Width - 50 - PaddleWidth,
                                    (ClientSize.Height - PaddleHeight) \ 2,
                                    PaddleWidth,
                                    PaddleHeight)

        ' Initialize ball centered
        BallPos = New PointF((ClientSize.Width - BallDiameter) / 2.0F,
                             (ClientSize.Height - BallDiameter) / 2.0F)
    End Sub

End Class
