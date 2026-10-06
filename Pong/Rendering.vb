''Imports System.Runtime.InteropServices

''Public Class Rendering




''    ' -------------------------------
''    '  Game State
''    ' -------------------------------
''    Private Enum GameState
''        StartScreen
''        Playing
''        EndScreen
''        Pause
''        AIDifficulty

''    End Enum

''    Private currentState As GameState = GameState.StartScreen
''    Private winnerText As String = String.Empty

''    ' -------------------------------
''    '  Player Mode
''    ' -------------------------------
''    Private playerMode As Integer = 1       ' 1 = Single Player (AI), 2 = Two Players
''    Private numberOfPlayersSelection As Integer = 0   ' 0 = "1 Player", 1 = "2 Players"

''    Private Enum NumberOfPlayers
''        OnePlayer
''        TwoPlayers
''    End Enum

''    ' -------------------------------
''    '  Ball / Physics
''    ' -------------------------------
''    Private ballPos As PointF
''    Private ballDiameter As Integer = 60

''    Private velX As Double
''    Private velY As Double
''    Private speed As Double = 200

''    Private physicsTimer As New Timer()
''    Private physicsStopwatch As New Stopwatch()

''    ' -------------------------------
''    '  FPS Tracking
''    ' -------------------------------
''    Private frameCount As Integer = 0
''    Private fps As Integer = 0
''    Private fpsStopwatch As New Stopwatch()

''    ' -------------------------------
''    '  GDI Resources
''    ' -------------------------------
''    Private ballBrush As SolidBrush
''    Private fpsBrush As SolidBrush
''    Private fpsFont As Font
''    Private trailBrushes As SolidBrush()
''    Private paddleBrush As SolidBrush
''    Private playerLabelBrush As SolidBrush
''    Private scoreBrush As SolidBrush

''    Private whiteBrush As SolidBrush
''    Private grayBrush As SolidBrush
''    Private dimBrush As SolidBrush

''    ' -------------------------------
''    '  Trail System
''    ' -------------------------------
''    Private trail As New List(Of PointF)
''    Private trailLength As Integer = 10
''    Private trailSizes As Integer()
''    Private trailOffsets As Single()
''    Private trailAlpha As Integer()

''    ' -------------------------------
''    '  Audio Cooldown
''    ' -------------------------------
''    Private lastPlay As New Dictionary(Of String, Integer)

''    ' -------------------------------
''    '  Pong State
''    ' -------------------------------
''    Private paddleLeft As RectangleF
''    Private paddleRight As RectangleF

''    Private paddleWidth As Integer = 32
''    Private paddleHeight As Integer = 128
''    Private paddleSpeed As Integer = 700

''    Private moveLeftPaddleUp As Boolean
''    Private moveLeftPaddleDown As Boolean
''    Private moveRightPaddleUp As Boolean
''    Private moveRightPaddleDown As Boolean

''    Private scoreLeft As Integer = 0
''    Private scoreRight As Integer = 0

''    ' -------------------------------
''    '  Start Screen FX
''    ' -------------------------------
''    Private titleAlpha As Integer = 0
''    Private titleFadeIn As Boolean = True
''    Private blinkVisible As Boolean = True
''    Private blinkStopwatch As New Stopwatch()

''    ' -------------------------------
''    '  Paddle Velocity (Spin)
''    ' -------------------------------
''    Private lastPaddleLeftY As Single
''    Private paddleLeftVelocity As Single
''    Private lastPaddleRightY As Single
''    Private paddleRightVelocity As Single

''    ' -------------------------------
''    '  Pause Menu
''    ' -------------------------------
''    Private pauseMenuSelection As Integer = 0



''    Private aiDifficultySelection As Integer = 0 ' 0 = Easy, 1 = Normal, 2 = Hard
''    Private aiOptions() As String = {"Easy", "Normal", "Hard"}

''    Private Enum AIDifficultyLevel
''        Easy
''        Normal
''        Hard
''    End Enum

''    ' -------------------------------
''    '  Player Names
''    ' -------------------------------
''    Private leftPlayerName As String = "Left"
''    Private rightPlayerName As String = "Right"

''    ' -------------------------------
''    '  Input Repeat Guards
''    ' -------------------------------
''    Private pauseKeyDown As Boolean = False
''    Private pKeyDown As Boolean = False
''    Private mediaPlayPauseKeyDown As Boolean = False
''    Private f11KeyDown As Boolean = False
''    Private fKeyDown As Boolean = False
''    Private escapeKeyDown As Boolean = False
''    Private spaceKeyDown As Boolean = False
''    Private enterKeyDown As Boolean = False
''    Private upKeyDown As Boolean = False
''    Private downKeyDown As Boolean = False

''    Private wKeyDown As Boolean = False
''    Private sKeyDown As Boolean = False
''    Private ctrlQDown As Boolean = False
''    Private ctrlHDown As Boolean = False



''    ' -------------------------------
''    '  AI Difficulty
''    ' -------------------------------
''    Private aiDifficulty As Double = 1.0   ' 1.0 = normal
''    Private aiModeFactor As Double = 1.0         ' 1.0 = normal

''    ' -------------------------------
''    '  Cached Fonts
''    ' -------------------------------
''    Private hudScoreFont As Font
''    Private hudLabelFont As Font
''    Private pauseTitleFont As Font
''    Private pauseMenuFont As Font
''    Private startTitleFont As Font
''    Private aiDifficultyTitleFont As Font
''    Private startMenuFont As Font
''    Private startInfoFont As Font
''    Private gameOverFont As Font
''    Private gameOverInfoFont As Font

''    Private fullscreenIndicatorFont As Font
''    Private fullscreenIndicatorBrush As SolidBrush


''    ' Cached pause layout
''    Private pauseTitle As String = "PAUSED"
''    Private pauseTitleSize As SizeF
''    Private pauseTitleX As Single
''    Private pauseTitleY As Single

''    Private pauseMenuItems() As String = {"Resume", "New", "Quit"}
''    Private pauseMenuItemSizes() As SizeF
''    Private pauseMenuItemX() As Single
''    Private pauseMenuItemY() As Single

''    Private pauseMenuStartY As Single
''    Private pauseMenuSpacing As Single

''    Private pauseMenuItemBrush() As SolidBrush



''    ' -------------------------------
''    '  Random
''    ' -------------------------------
''    Private rng As New Random()

''    Private Audio As AudioPlayer



''    Private WithEvents AudioRestartTimer As Timer


''    Private gameplayLoopVolume As Integer = 50
''    Private startLoopVolume As Integer = 75
''    Private pauseLoopVolume As Integer = 40


''    Private Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

''    <DllImport("dwmapi.dll")>
''    Private Shared Function DwmSetWindowAttribute(
''        hWnd As IntPtr,
''        attr As Integer,
''        ByRef attrValue As Integer,
''        attrSize As Integer
''    ) As Integer
''    End Function

''    Private showKeyboardHints As Boolean = True


''    Private hdSize As New Size(1280, 720)
''    Private fhdSize As New Size(1920, 1080)


''    Private onePlayerOptionRect As Rectangle
''    Private twoPlayersOptionRect As Rectangle


''    Private aiEasyRect As Rectangle
''    Private aiNormalRect As Rectangle
''    Private aiHardRect As Rectangle


''    Private pauseResumeRect As Rectangle
''    Private pauseNewMatchRect As Rectangle
''    Private pauseQuitRect As Rectangle


''    Private lightBrush As New SolidBrush(Color.FromArgb(32, 255, 255, 255))

''    Private darkBrush As New SolidBrush(Color.FromArgb(64, 0, 0, 0))

''    Private outlinePen As New Pen(Color.FromArgb(40, 255, 255, 255), 2)
''    Private darkOutlinePen As New Pen(Color.FromArgb(32, 255, 255, 255), 2)


''    Private mouseIsClicking As Boolean = False

''    ' HUD cached layout
''    Private leftScoreX As Single
''    Private rightScoreX As Single
''    Private leftLabelX As Single
''    Private rightLabelX As Single
''    Private scoreY As Single
''    Private labelY As Single

''    Private leftScoreSize As SizeF
''    Private rightScoreSize As SizeF
''    Private leftLabelSize As SizeF
''    Private rightLabelSize As SizeF




''    ' Cached text
''    Private hintLeftText As String
''    Private hintRightText As String
''    Private pauseText As String
''    Private fpsText As String

''    ' Cached sizes
''    Private hintLeftSize As SizeF
''    Private hintRightSize As SizeF
''    Private pauseSize As SizeF
''    Private fpsSize As SizeF

''    ' Cached positions
''    Private hintLeftX As Single = 10
''    Private hintLeftY As Single = 10

''    Private hintRightX As Single
''    Private hintRightY As Single = 10

''    Private pauseX As Single = 10
''    Private pauseY As Single

''    Private fpsX As Single
''    Private fpsY As Single



''    Public Sub New(g As Graphics, ClientSize As Size)
''        ScaleBallDiameter(ClientSize)
''        InitGraphics(g, ClientSize)
''        InitTrails()
''        InitPaddles(ClientSize)
''        fpsStopwatch.Start()

''    End Sub

''    Private Sub InitPaddles(ClientSize As Size)







''        paddleLeft = New RectangleF(50,
''                                    (ClientSize.Height - paddleHeight) / 2,
''                                    paddleWidth,
''                                    paddleHeight)

''        paddleRight = New RectangleF(ClientSize.Width - 50 - paddleWidth,
''                                     (ClientSize.Height - paddleHeight) / 2,
''                                     paddleWidth,
''                                     paddleHeight)
''    End Sub



''    Private Sub ScaleBallDiameter(ClientSize As Size)
''        ballDiameter = CInt(ClientSize.Height / 18.0F)
''    End Sub




''    Private Sub InitGraphics(g As Graphics, ClientSize As Size)
''        ballBrush = New SolidBrush(Color.DeepSkyBlue)
''        fpsBrush = New SolidBrush(Color.Gray)
''        fpsFont = New Font("Segoe UI", 14, FontStyle.Bold)
''        paddleBrush = New SolidBrush(Color.White)
''        playerLabelBrush = New SolidBrush(Color.Gray)
''        scoreBrush = New SolidBrush(Color.White)

''        whiteBrush = New SolidBrush(Color.White)
''        grayBrush = New SolidBrush(Color.FromArgb(140, 140, 140))
''        dimBrush = New SolidBrush(Color.FromArgb(120, 0, 0, 0))
''        fullscreenIndicatorBrush = New SolidBrush(Color.FromArgb(120, 255, 255, 255))

''        RescaleFonts(g, ClientSize)
''    End Sub
''    'Private Sub InitTrails()
''    '    trailSizes = New Integer(trailLength - 1) {}
''    '    trailOffsets = New Single(trailLength - 1) {}
''    '    trailAlpha = New Integer(trailLength - 1) {}
''    '    trailBrushes = New SolidBrush(trailLength - 1) {}

''    '    For i As Integer = 0 To trailLength - 1
''    '        Dim size As Integer = ballDiameter - (trailLength - i) * 2
''    '        If size < 10 Then size = 10

''    '        trailSizes(i) = size
''    '        trailOffsets(i) = CSng((ballDiameter - size) / 2)

''    '        Dim t As Double = i / CDbl(trailLength)
''    '        Dim alpha As Integer = CInt(32 * t * t)
''    '        trailAlpha(i) = alpha

''    '        trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
''    '    Next
''    'End Sub

''    Private Sub InitTrails()
''        trailSizes = New Integer(trailLength - 1) {}
''        trailOffsets = New Single(trailLength - 1) {}
''        trailAlpha = New Integer(trailLength - 1) {}
''        trailBrushes = New SolidBrush(trailLength - 1) {}

''        For i As Integer = 0 To trailLength - 1

''            ' Precompute size curve
''            Dim size As Integer = ballDiameter - (trailLength - i) * 2
''            If size < 10 Then size = 10
''            trailSizes(i) = size

''            ' Precompute offset curve
''            trailOffsets(i) = CSng((ballDiameter - size) / 2)

''            ' Precompute alpha curve (soft quadratic)
''            Dim t As Double = i / CDbl(trailLength)
''            Dim alpha As Integer = CInt(16 * t * t)
''            trailAlpha(i) = alpha

''            ' Precompute brush
''            trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
''        Next
''    End Sub



''    Public Sub DrawGamePlayScreen(g As Graphics)
''        DrawTrail(g)
''        DrawBall(g)
''        DrawPaddles(g)
''        DrawHUD(g)
''        If showKeyboardHints Then DrawKeyboardHintsGamePlayScreen(g)

''    End Sub

''    Public Sub ToggleKeyboardHints()
''        showKeyboardHints = Not showKeyboardHints
''    End Sub


''    'Public Sub UpdateFPS(framesPerSecond As Integer)
''    '    fps = framesPerSecond
''    '    fpsText = $"FPS: {fps}"

''    'End Sub



''    Public Sub UpdateFPS()
''        frameCount += 1

''        If fpsStopwatch.ElapsedMilliseconds >= 1000 Then
''            fps = frameCount
''            'renderer.UpdateFPS(fps)
''            fpsText = $"FPS: {fps}"

''            frameCount = 0
''            fpsStopwatch.Restart()
''        End If

''    End Sub


''    Public Sub DrawHUD(g As Graphics)


''        ' Draw labels (no allocations)
''        g.DrawString(leftPlayerName, hudLabelFont, playerLabelBrush, leftLabelX, labelY)
''        g.DrawString(rightPlayerName, hudLabelFont, playerLabelBrush, rightLabelX, labelY)

''        ' Draw scores (no allocations)
''        g.DrawString(scoreLeft.ToString(), hudScoreFont, scoreBrush, leftScoreX, scoreY)
''        g.DrawString(scoreRight.ToString(), hudScoreFont, scoreBrush, rightScoreX, scoreY)

''    End Sub

''    Public Sub UpdateScore(leftScore As Integer, rightScore As Integer)
''        Me.scoreLeft = leftScore
''        Me.scoreRight = rightScore
''    End Sub

''    Public Sub UpdatePlayerNames(leftName As String, rightName As String)
''        Me.leftPlayerName = leftName
''        Me.rightPlayerName = rightName
''    End Sub


''    Public Sub UpdateBallPosition(deltaTime As Double)
''        ballPos.X += CInt(velX * deltaTime)
''        ballPos.Y += CInt(velY * deltaTime)
''    End Sub

''    Public Sub UpdateBallVelocity(newVelX As Double, newVelY As Double)
''        velX = newVelX
''        velY = newVelY
''    End Sub

''    Public Sub UpdateBallPosition(newPos As PointF)
''        ballPos = newPos
''    End Sub

''    Public Sub UpdateBallDiameter(newDiameter As Integer)
''        ballDiameter = newDiameter
''    End Sub

''    Public Sub UpdateBallBrush(newBrush As SolidBrush)
''        ballBrush?.Dispose()
''        ballBrush = newBrush
''    End Sub








''    'Public Sub UpdateTrail()
''    '    If trailSizes Is Nothing OrElse trailOffsets Is Nothing Then Return

''    '    ' Add current ball position to the trail
''    '    trail.Add(ballPos)
''    '    ' Limit the trail length
''    '    If trail.Count > trailLength Then
''    '        trail.RemoveAt(0)
''    '    End If
''    '    ' Update sizes, offsets, and alpha values for the trail
''    '    ReDim trailSizes(trail.Count - 1)
''    '    ReDim trailOffsets(trail.Count - 1)
''    '    ReDim trailAlpha(trail.Count - 1)
''    '    For i As Integer = 0 To trail.Count - 1
''    '        Dim t As Double = i / (trail.Count - 1) ' Normalized value from 0 to 1
''    '        trailSizes(i) = CInt(ballDiameter * (0.5 + 0.5 * t)) ' Size grows from half to full size
''    '        trailOffsets(i) = CInt(ballDiameter * (0.25 * (1 - t))) ' Offset decreases as it gets older
''    '        trailAlpha(i) = CInt(255 * (0.2 + 0.8 * t)) ' Alpha increases from 20% to 100%
''    '    Next
''    'End Sub









''    'Public Sub UpdateTrail()

''    '    ' Safety: arrays must be preallocated once during initialization
''    '    If trailSizes Is Nothing OrElse trailOffsets Is Nothing OrElse trailAlpha Is Nothing Then
''    '        Return
''    '    End If

''    '    ' Add current ball position
''    '    trail.Add(ballPos)

''    '    ' Enforce fixed trail length
''    '    If trail.Count > trailLength Then
''    '        trail.RemoveAt(0)
''    '    End If

''    '    Dim count As Integer = trail.Count

''    '    ' Handle the single‑point case safely
''    '    If count = 1 Then
''    '        trailSizes(0) = ballDiameter
''    '        trailOffsets(0) = 0
''    '        trailAlpha(0) = 255
''    '        Return
''    '    End If

''    '    ' Update trail parameters
''    '    For i As Integer = 0 To count - 1

''    '        ' Normalized 0 → 1
''    '        Dim t As Double = i / (count - 1)

''    '        ' Size grows from 50% → 100%
''    '        trailSizes(i) = CInt(ballDiameter * (0.5 + 0.5 * t))

''    '        ' Offset shrinks as trail ages
''    '        trailOffsets(i) = CSng(ballDiameter * (0.25 * (1 - t)))

''    '        ' Alpha grows from 20% → 100%
''    '        trailAlpha(i) = CInt(255 * (0.2 + 0.8 * t))

''    '    Next

''    'End Sub


''    Public Sub UpdateTrail()

''        If trailSizes Is Nothing OrElse trailOffsets Is Nothing OrElse trailAlpha Is Nothing Then
''            Return
''        End If

''        ' Add current ball position
''        trail.Add(ballPos)

''        ' Enforce fixed trail length
''        If trail.Count > trailLength Then
''            trail.RemoveAt(0)
''        End If

''        Dim count As Integer = trail.Count

''        ' Single point case
''        If count = 1 Then
''            trailSizes(0) = ballDiameter
''            trailOffsets(0) = 0
''            trailAlpha(0) = 32
''            Return
''        End If

''        ' Update dynamic size/offset/alpha curves
''        For i As Integer = 0 To count - 1

''            Dim t As Double = i / (count - 1)

''            trailSizes(i) = CInt(ballDiameter * (0.5 + 0.5 * t))
''            trailOffsets(i) = CSng(ballDiameter * (0.25 * (1 - t)))
''            trailAlpha(i) = CInt(32 * (0.2 + 0.8 * t))

''        Next

''    End Sub



''    'Public Sub ClearTrail()
''    '    trail.Clear()
''    '    ReDim trailSizes(0)
''    '    ReDim trailOffsets(0)
''    '    ReDim trailAlpha(0)
''    'End Sub

''    Public Sub ClearTrail()
''        trail.Clear()

''        ' Reset curves but DO NOT resize arrays
''        For i As Integer = 0 To trailLength - 1
''            trailSizes(i) = 0
''            trailOffsets(i) = 0
''            trailAlpha(i) = 0
''        Next
''    End Sub


















''    'Public Sub DrawTrail(g As Graphics)
''    '    For i As Integer = 0 To trail.Count - 1
''    '        Dim pos As PointF = trail(i)
''    '        Dim size As Integer = trailSizes(i)
''    '        Dim offset As Single = trailOffsets(i)
''    '        Dim alpha As Integer = trailAlpha(i)
''    '        Using brush As New SolidBrush(Color.FromArgb(alpha, Color.DeepSkyBlue))
''    '            g.FillEllipse(brush, pos.X + offset, pos.Y + offset, size, size)
''    '        End Using
''    '    Next
''    'End Sub


''    'Public Sub DrawTrail(g As Graphics)

''    '    If trail Is Nothing OrElse
''    '   trailSizes Is Nothing OrElse
''    '   trailOffsets Is Nothing OrElse
''    '   trailBrushes Is Nothing Then
''    '        Exit Sub
''    '    End If

''    '    Dim count As Integer = trail.Count
''    '    If count > trailLength Then count = trailLength

''    '    For i As Integer = 0 To count - 1

''    '        Dim p As PointF = trail(i)
''    '        Dim offset As Single = trailOffsets(i)

''    '        g.FillEllipse(trailBrushes(i),
''    '                  p.X + offset,
''    '                  p.Y + offset,
''    '                  trailSizes(i),
''    '                  trailSizes(i))
''    '    Next

''    'End Sub
''    Public Sub DrawTrail(g As Graphics)

''        If trail Is Nothing OrElse
''       trailSizes Is Nothing OrElse
''       trailOffsets Is Nothing OrElse
''       trailBrushes Is Nothing Then
''            Exit Sub
''        End If

''        Dim count As Integer = trail.Count
''        If count > trailLength Then count = trailLength

''        For i As Integer = 0 To count - 1

''            Dim p As PointF = trail(i)
''            Dim offset As Single = trailOffsets(i)

''            ' Update brush alpha dynamically
''            Dim c As Color = trailBrushes(i).Color
''            trailBrushes(i).Color = Color.FromArgb(trailAlpha(i), c.R, c.G, c.B)

''            g.FillEllipse(trailBrushes(i),
''                      p.X + offset,
''                      p.Y + offset,
''                      trailSizes(i),
''                      trailSizes(i))
''        Next


''        'DrawBall(g)


''    End Sub



''    Public Sub DrawBall(g As Graphics)
''        g.FillEllipse(ballBrush, ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
''    End Sub



''    Public Sub DrawPaddles(g As Graphics)
''        g.FillRectangle(paddleBrush, paddleLeft)
''        g.FillRectangle(paddleBrush, paddleRight)
''    End Sub






''    Public Sub DrawKeyboardHintsGamePlayScreen(g As Graphics)
''        If Not showKeyboardHints Then Return
''        ' Draw left paddle hint
''        g.DrawString(hintLeftText, fullscreenIndicatorFont, fullscreenIndicatorBrush, hintLeftX, hintLeftY)
''        ' Draw right paddle hint
''        g.DrawString(hintRightText, fullscreenIndicatorFont, fullscreenIndicatorBrush, hintRightX, hintRightY)
''        ' Draw pause hint (bottom-left)
''        g.DrawString(pauseText, fullscreenIndicatorFont, fullscreenIndicatorBrush, pauseX, pauseY)
''        ' Draw FPS (bottom-right)
''        g.DrawString(fpsText, fullscreenIndicatorFont, fullscreenIndicatorBrush, fpsX, fpsY)
''    End Sub













''    Public Sub RescaleFonts(g As Graphics, ClientSize As Size)
''        hudScoreFont?.Dispose()
''        hudLabelFont?.Dispose()
''        pauseTitleFont?.Dispose()
''        aiDifficultyTitleFont?.Dispose()
''        pauseMenuFont?.Dispose()
''        startTitleFont?.Dispose()
''        startMenuFont?.Dispose()
''        startInfoFont?.Dispose()
''        gameOverFont?.Dispose()
''        gameOverInfoFont?.Dispose()
''        fullscreenIndicatorFont?.Dispose()
''        fpsFont?.Dispose()

''        hudScoreFont = New Font("Segoe UI", CSng(ClientSize.Height / 12.0F), FontStyle.Bold)
''        hudLabelFont = New Font("Segoe UI", CSng(ClientSize.Height / 50.0F), FontStyle.Regular)

''        pauseTitleFont = New Font("Segoe UI", CSng(ClientSize.Height / 18.0F), FontStyle.Bold)
''        pauseMenuFont = New Font("Segoe UI", CSng(ClientSize.Height / 28.0F), FontStyle.Regular)

''        startTitleFont = New Font("Segoe UI", CSng(ClientSize.Height / 10.0F), FontStyle.Bold)
''        aiDifficultyTitleFont = New Font("Segoe UI", CSng(ClientSize.Height / 18.0F), FontStyle.Bold)

''        startMenuFont = New Font("Segoe UI", CSng(ClientSize.Height / 30.0F), FontStyle.Regular)
''        startInfoFont = New Font("Segoe UI", CSng(ClientSize.Height / 35.0F), FontStyle.Regular)

''        gameOverFont = New Font("Segoe UI", CSng(ClientSize.Height / 20.0F), FontStyle.Bold)
''        gameOverInfoFont = New Font("Segoe UI", CSng(ClientSize.Height / 35.0F), FontStyle.Regular)

''        fullscreenIndicatorFont = New Font("Segoe UI", CSng(ClientSize.Height / 75.0F), FontStyle.Regular)
''        fpsFont = New Font("Segoe UI", CSng(ClientSize.Height / 75.0F), FontStyle.Bold)

''        ' -------------------------------
''        ' Pause Screen Layout Cache
''        ' -------------------------------
''        'Using g As Graphics = Me.CreateGraphics()

''        ' Title
''        pauseTitleSize = g.MeasureString(pauseTitle, pauseTitleFont)
''        pauseTitleX = (ClientSize.Width - pauseTitleSize.Width) / 2.0F
''        pauseTitleY = ClientSize.Height * 0.22F

''        ' Menu spacing
''        pauseMenuSpacing = ClientSize.Height * 0.11F
''        pauseMenuStartY = pauseTitleY + pauseTitleSize.Height + (ClientSize.Height * 0.05F)

''        ' Menu items
''        ReDim pauseMenuItemSizes(pauseMenuItems.Length - 1)
''        ReDim pauseMenuItemX(pauseMenuItems.Length - 1)
''        ReDim pauseMenuItemY(pauseMenuItems.Length - 1)

''        For i As Integer = 0 To pauseMenuItems.Length - 1
''            pauseMenuItemSizes(i) = g.MeasureString(pauseMenuItems(i), pauseMenuFont)
''            pauseMenuItemX(i) = (ClientSize.Width - pauseMenuItemSizes(i).Width) / 2.0F
''            pauseMenuItemY(i) = pauseMenuStartY + i * pauseMenuSpacing
''        Next

''        'End Using






''        'Using g As Graphics = Me.CreateGraphics()

''        Dim halfWidth As Single = ClientSize.Width / 2.0F

''        ' Measure once
''        leftScoreSize = g.MeasureString(scoreLeft.ToString(), hudScoreFont)
''        rightScoreSize = g.MeasureString(scoreRight.ToString(), hudScoreFont)

''        leftLabelSize = g.MeasureString(leftPlayerName, hudLabelFont)
''        rightLabelSize = g.MeasureString(rightPlayerName, hudLabelFont)

''        ' Vertical layout
''        scoreY = 10 + CSng(ClientSize.Height / 25.0F)
''        labelY = scoreY - CSng(ClientSize.Height / 200.0F)

''        ' Horizontal layout
''        leftScoreX = (halfWidth - leftScoreSize.Width) / 2.0F
''        rightScoreX = halfWidth + (halfWidth - rightScoreSize.Width) / 2.0F

''        leftLabelX = (halfWidth - leftLabelSize.Width) / 2.0F
''        rightLabelX = halfWidth + (halfWidth - rightLabelSize.Width) / 2.0F

''        'End Using



''        RecomputeKeyboardHintLayout(g, ClientSize)


''        ' -------------------------------
''        ' Scale paddles
''        ' -------------------------------
''        paddleHeight = ClientSize.Height / 8
''        paddleWidth = ClientSize.Height / 25

''        paddleLeft.Height = paddleHeight
''        paddleLeft.Width = paddleWidth
''        paddleRight.Height = paddleHeight
''        paddleRight.Width = paddleWidth

''        paddleLeft.X = ClientSize.Height / 25
''        paddleRight.X = ClientSize.Width - ClientSize.Height / 25 - paddleWidth

''    End Sub

''    Public Sub UpdatePaddlePositions(leftY As Single, rightY As Single)
''        paddleLeft.Y = leftY
''        paddleRight.Y = rightY
''    End Sub


''    Private Sub RecomputeKeyboardHintLayout(g As Graphics, ClientSize As Size)

''        'Using g As Graphics = Me.CreateGraphics()

''        ' -------------------------------
''        ' Left Paddle Hint
''        ' -------------------------------
''        hintLeftText = "W S - Move Paddle"
''            hintLeftSize = g.MeasureString(hintLeftText, fullscreenIndicatorFont)

''            ' -------------------------------
''            ' Right Paddle Hint
''            ' -------------------------------
''            If playerMode = 2 Then
''                hintRightText = "Arrows - Move Paddle"
''            Else
''                hintRightText = "P - Pause Match"
''            End If

''            hintRightSize = g.MeasureString(hintRightText, fullscreenIndicatorFont)
''            hintRightX = ClientSize.Width - hintRightSize.Width - 10

''            ' -------------------------------
''            ' Pause Hint (Bottom‑Left)
''            ' -------------------------------
''            If playerMode = 2 Then
''                pauseText = "P - Pause Match"
''            Else
''                pauseText = ""
''            End If

''            pauseSize = g.MeasureString(pauseText, fullscreenIndicatorFont)
''            pauseY = ClientSize.Height - pauseSize.Height - 10

''            ' -------------------------------
''            ' FPS (Bottom‑Right)
''            ' -------------------------------
''            fpsText = $"FPS: {fps}"
''            fpsSize = g.MeasureString(fpsText, fullscreenIndicatorFont)
''            fpsX = ClientSize.Width - fpsSize.Width - 10
''            fpsY = ClientSize.Height - fpsSize.Height - 10

''        'End Using

''    End Sub










''End Class


'Imports System.Runtime.InteropServices

'Public Class Rendering

'    ' ============================================================
'    '   GAME STATE
'    ' ============================================================
'    Private Enum GameState
'        StartScreen
'        Playing
'        EndScreen
'        Pause
'        AIDifficulty
'    End Enum

'    Private currentState As GameState = GameState.StartScreen

'    ' ============================================================
'    '   PLAYER MODE
'    ' ============================================================
'    Private playerMode As Integer = 1   ' 1 = Single Player, 2 = Two Players

'    ' ============================================================
'    '   BALL / PHYSICS
'    ' ============================================================
'    Private ballPos As PointF
'    Private ballDiameter As Integer
'    Private velX As Double
'    Private velY As Double

'    ' ============================================================
'    '   FPS TRACKING
'    ' ============================================================
'    Private frameCount As Integer
'    Private fps As Integer
'    Private fpsStopwatch As New Stopwatch()

'    ' ============================================================
'    '   GDI RESOURCES
'    ' ============================================================
'    Private ballBrush As SolidBrush
'    Private paddleBrush As SolidBrush
'    Private scoreBrush As SolidBrush
'    Private labelBrush As SolidBrush
'    Private fpsBrush As SolidBrush
'    Private fullscreenBrush As SolidBrush

'    Private hudScoreFont As Font
'    Private hudLabelFont As Font
'    Private fpsFont As Font
'    Private hintFont As Font

'    ' ============================================================
'    '   TRAIL SYSTEM
'    ' ============================================================
'    Private trail As New List(Of PointF)
'    Private Const TrailLength As Integer = 10
'    Private trailSizes(TrailLength - 1) As Integer
'    Private trailOffsets(TrailLength - 1) As Single
'    Private trailAlpha(TrailLength - 1) As Integer
'    Private trailBrushes(TrailLength - 1) As SolidBrush

'    ' ============================================================
'    '   PADDLES
'    ' ============================================================
'    Private paddleLeft As RectangleF
'    Private paddleRight As RectangleF
'    Private paddleWidth As Integer
'    Private paddleHeight As Integer

'    ' ============================================================
'    '   HUD LAYOUT CACHE
'    ' ============================================================
'    Private leftScoreX As Single
'    Private rightScoreX As Single
'    Private leftLabelX As Single
'    Private rightLabelX As Single
'    Private scoreY As Single
'    Private labelY As Single

'    Private leftPlayerName As String = "Left"
'    Private rightPlayerName As String = "Right"
'    Private scoreLeft As Integer
'    Private scoreRight As Integer

'    ' ============================================================
'    '   KEYBOARD HINTS
'    ' ============================================================
'    Private showHints As Boolean = True
'    Private hintLeftText As String
'    Private hintRightText As String
'    Private pauseText As String
'    Private fpsText As String

'    Private hintLeftX As Single = 10
'    Private hintLeftY As Single = 10
'    Private hintRightX As Single
'    Private hintRightY As Single = 10
'    Private pauseX As Single = 10
'    Private pauseY As Single
'    Private fpsX As Single
'    Private fpsY As Single

'    ' ============================================================
'    '   CONSTRUCTOR
'    ' ============================================================
'    Public Sub New(g As Graphics, clientSize As Size)
'        ScaleBall(clientSize)
'        InitGraphics(g, clientSize)
'        InitTrail()
'        InitPaddles(clientSize)
'        fpsStopwatch.Start()
'    End Sub

'    ' ============================================================
'    '   INITIALIZATION
'    ' ============================================================
'    Private Sub ScaleBall(clientSize As Size)
'        ballDiameter = CInt(clientSize.Height / 18.0F)
'    End Sub

'    Private Sub InitGraphics(g As Graphics, clientSize As Size)
'        ballBrush = New SolidBrush(Color.DeepSkyBlue)
'        paddleBrush = New SolidBrush(Color.White)
'        scoreBrush = New SolidBrush(Color.White)
'        labelBrush = New SolidBrush(Color.Gray)
'        fpsBrush = New SolidBrush(Color.Gray)
'        fullscreenBrush = New SolidBrush(Color.FromArgb(120, 255, 255, 255))

'        RescaleFonts(g, clientSize)
'    End Sub

'    Private Sub InitTrail()
'        For i As Integer = 0 To TrailLength - 1
'            Dim t As Double = i / CDbl(TrailLength)
'            Dim size As Integer = Math.Max(10, ballDiameter - (TrailLength - i) * 2)
'            Dim offset As Single = CSng((ballDiameter - size) / 2)
'            Dim alpha As Integer = CInt(16 * t * t)

'            trailSizes(i) = size
'            trailOffsets(i) = offset
'            trailAlpha(i) = alpha
'            trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
'        Next
'    End Sub

'    Private Sub InitPaddles(clientSize As Size)
'        paddleHeight = clientSize.Height / 8
'        paddleWidth = clientSize.Height / 25

'        paddleLeft = New RectangleF(
'            clientSize.Height / 25,
'            (clientSize.Height - paddleHeight) / 2,
'            paddleWidth,
'            paddleHeight)

'        paddleRight = New RectangleF(
'            clientSize.Width - clientSize.Height / 25 - paddleWidth,
'            (clientSize.Height - paddleHeight) / 2,
'            paddleWidth,
'            paddleHeight)
'    End Sub

'    ' ============================================================
'    '   FONT SCALING + HUD LAYOUT
'    ' ============================================================
'    Public Sub RescaleFonts(g As Graphics, clientSize As Size)
'        hudScoreFont?.Dispose()
'        hudLabelFont?.Dispose()
'        fpsFont?.Dispose()
'        hintFont?.Dispose()

'        hudScoreFont = New Font("Segoe UI", clientSize.Height / 12.0F, FontStyle.Bold)
'        hudLabelFont = New Font("Segoe UI", clientSize.Height / 50.0F)
'        fpsFont = New Font("Segoe UI", clientSize.Height / 75.0F, FontStyle.Bold)
'        hintFont = New Font("Segoe UI", clientSize.Height / 75.0F)

'        CacheHUDLayout(g, clientSize)
'        CacheHintLayout(g, clientSize)

'        RescalePaddles(clientSize)

'        '' -------------------------------
'        '' Scale paddles
'        '' -------------------------------
'        'paddleHeight = clientSize.Height / 8
'        'paddleWidth = clientSize.Height / 25

'        'paddleLeft.Height = paddleHeight
'        'paddleLeft.Width = paddleWidth
'        'paddleRight.Height = paddleHeight
'        'paddleRight.Width = paddleWidth

'        'paddleLeft.X = clientSize.Height / 25
'        'paddleRight.X = clientSize.Width - clientSize.Height / 25 - paddleWidth
'        ''renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y)

'        'ResetPaddles(clientSize)

'    End Sub


'    Public Sub RescalePaddles(clientSize As Size)
'        ' Recompute paddle dimensions
'        paddleHeight = clientSize.Height / 8
'        paddleWidth = clientSize.Height / 25

'        ' Left paddle stays at same relative X
'        paddleLeft.X = clientSize.Height / 25
'        paddleLeft.Width = paddleWidth
'        paddleLeft.Height = paddleHeight

'        ' Right paddle stays at same relative X
'        paddleRight.X = clientSize.Width - clientSize.Height / 25 - paddleWidth
'        paddleRight.Width = paddleWidth
'        paddleRight.Height = paddleHeight

'        ' Recenter paddles vertically
'        paddleLeft.Y = (clientSize.Height - paddleHeight) / 2
'        paddleRight.Y = (clientSize.Height - paddleHeight) / 2
'    End Sub


'    Private Sub ResetPaddles(clientSize As Size)
'        paddleLeft.Y = (clientSize.Height - paddleHeight) / 2
'        paddleRight.Y = (clientSize.Height - paddleHeight) / 2
'        'renderer.UpdatePaddlePositions(paddleLeft.Y, paddleRight.Y)
'    End Sub


'    Private Sub CacheHUDLayout(g As Graphics, clientSize As Size)
'        Dim halfWidth As Single = clientSize.Width / 2.0F

'        scoreY = 10 + clientSize.Height / 25.0F
'        labelY = scoreY - clientSize.Height / 200.0F

'        Dim leftScoreSize = g.MeasureString(scoreLeft.ToString(), hudScoreFont)
'        Dim rightScoreSize = g.MeasureString(scoreRight.ToString(), hudScoreFont)
'        Dim leftLabelSize = g.MeasureString(leftPlayerName, hudLabelFont)
'        Dim rightLabelSize = g.MeasureString(rightPlayerName, hudLabelFont)

'        leftScoreX = (halfWidth - leftScoreSize.Width) / 2.0F
'        rightScoreX = halfWidth + (halfWidth - rightScoreSize.Width) / 2.0F

'        leftLabelX = (halfWidth - leftLabelSize.Width) / 2.0F
'        rightLabelX = halfWidth + (halfWidth - rightLabelSize.Width) / 2.0F
'    End Sub

'    Private Sub CacheHintLayout(g As Graphics, clientSize As Size)
'        hintLeftText = "W S - Move Paddle"
'        hintRightText = If(playerMode = 2, "Arrows - Move Paddle", "P - Pause Match")
'        pauseText = If(playerMode = 2, "P - Pause Match", "")

'        Dim rightSize = g.MeasureString(hintRightText, hintFont)
'        hintRightX = clientSize.Width - rightSize.Width - 10

'        Dim pauseSize = g.MeasureString(pauseText, hintFont)
'        pauseY = clientSize.Height - pauseSize.Height - 10

'        fpsText = $"FPS: {fps}"
'        Dim fpsSize = g.MeasureString(fpsText, hintFont)
'        fpsX = clientSize.Width - fpsSize.Width - 10
'        fpsY = clientSize.Height - fpsSize.Height - 10
'    End Sub




'    ' ============================================================
'    '   UPDATE METHODS
'    ' ============================================================
'    Public Sub UpdateFPS()
'        frameCount += 1

'        If fpsStopwatch.ElapsedMilliseconds >= 1000 Then
'            fps = frameCount
'            fpsText = $"FPS: {fps}"
'            frameCount = 0
'            fpsStopwatch.Restart()
'        End If
'    End Sub

'    Public Sub UpdateBall(deltaTime As Double)
'        ballPos.X += CSng(velX * deltaTime)
'        ballPos.Y += CSng(velY * deltaTime)
'    End Sub

'    Public Sub UpdateTrail()
'        trail.Add(ballPos)
'        If trail.Count > TrailLength Then trail.RemoveAt(0)
'    End Sub

'    Public Sub UpdateScore(left As Integer, right As Integer)
'        scoreLeft = left
'        scoreRight = right
'    End Sub

'    Public Sub UpdatePlayerNames(left As String, right As String)
'        leftPlayerName = left
'        rightPlayerName = right
'    End Sub

'    ' ============================================================
'    '   DRAW METHODS
'    ' ============================================================
'    Public Sub DrawGameplay(g As Graphics)
'        DrawTrail(g)
'        DrawBall(g)
'        DrawPaddles(g)
'        DrawHUD(g)
'        If showHints Then DrawHints(g)
'    End Sub

'    Private Sub DrawTrail(g As Graphics)
'        Dim count As Integer = trail.Count
'        If count = 0 Then Exit Sub

'        For i As Integer = 0 To count - 1
'            Dim p = trail(i)
'            Dim offset = trailOffsets(i)
'            Dim brush = trailBrushes(i)

'            brush.Color = Color.FromArgb(trailAlpha(i), brush.Color.R, brush.Color.G, brush.Color.B)

'            g.FillEllipse(brush,
'                          p.X + offset,
'                          p.Y + offset,
'                          trailSizes(i),
'                          trailSizes(i))
'        Next
'    End Sub

'    Private Sub DrawBall(g As Graphics)
'        g.FillEllipse(ballBrush, ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
'    End Sub

'    Private Sub DrawPaddles(g As Graphics)
'        g.FillRectangle(paddleBrush, paddleLeft)
'        g.FillRectangle(paddleBrush, paddleRight)
'    End Sub

'    Private Sub DrawHUD(g As Graphics)
'        g.DrawString(leftPlayerName, hudLabelFont, labelBrush, leftLabelX, labelY)
'        g.DrawString(rightPlayerName, hudLabelFont, labelBrush, rightLabelX, labelY)
'        g.DrawString(scoreLeft.ToString(), hudScoreFont, scoreBrush, leftScoreX, scoreY)
'        g.DrawString(scoreRight.ToString(), hudScoreFont, scoreBrush, rightScoreX, scoreY)
'    End Sub

'    Private Sub DrawHints(g As Graphics)
'        g.DrawString(hintLeftText, hintFont, fullscreenBrush, hintLeftX, hintLeftY)
'        g.DrawString(hintRightText, hintFont, fullscreenBrush, hintRightX, hintRightY)
'        g.DrawString(pauseText, hintFont, fullscreenBrush, pauseX, pauseY)
'        g.DrawString(fpsText, hintFont, fullscreenBrush, fpsX, fpsY)
'    End Sub

'    ' ============================================================
'    '   COMPATIBILITY METHODS FOR FORM1
'    ' ============================================================

'    ' Old name: UpdateBallPosition(deltaTime As Double)
'    Public Sub UpdateBallPosition(deltaTime As Double)
'        UpdateBall(deltaTime)
'    End Sub

'    ' Old name: UpdateBallPosition(newPos As PointF)
'    Public Sub UpdateBallPosition(newPos As PointF)
'        ballPos = newPos
'    End Sub

'    ' Old name: UpdateBallDiameter
'    Public Sub UpdateBallDiameter(newDiameter As Integer)
'        ballDiameter = newDiameter
'    End Sub

'    ' Old name: DrawGamePlayScreen
'    Public Sub DrawGamePlayScreen(g As Graphics)
'        DrawGameplay(g)
'    End Sub

'    ' Old name: ToggleKeyboardHints
'    Public Sub ToggleKeyboardHints()
'        showHints = Not showHints
'    End Sub

'    ' Old name: UpdatePaddlePositions
'    Public Sub UpdatePaddlePositions(leftY As Single, rightY As Single)
'        paddleLeft.Y = leftY
'        paddleRight.Y = rightY
'    End Sub


































'End Class




'Imports System.Runtime.InteropServices

'Public Class Rendering

'    ' -------------------------------
'    '  Game State
'    ' -------------------------------
'    Private Enum GameState
'        StartScreen
'        Playing
'        EndScreen
'        Pause
'        AIDifficulty
'    End Enum

'    Private currentState As GameState = GameState.StartScreen
'    Private winnerText As String = String.Empty

'    ' -------------------------------
'    '  Player Mode
'    ' -------------------------------
'    Private playerMode As Integer = 1       ' 1 = Single Player (AI), 2 = Two Players
'    Private numberOfPlayersSelection As Integer = 0   ' 0 = "1 Player", 1 = "2 Players"

'    Private Enum NumberOfPlayers
'        OnePlayer
'        TwoPlayers
'    End Enum

'    ' -------------------------------
'    '  Ball / Physics
'    ' -------------------------------
'    Private ballPos As PointF
'    Private ballDiameter As Integer = 60

'    Private velX As Double
'    Private velY As Double

'    ' -------------------------------
'    '  FPS Tracking
'    ' -------------------------------
'    Private frameCount As Integer = 0
'    Private fps As Integer = 0
'    Private fpsStopwatch As New Stopwatch()

'    ' -------------------------------
'    '  GDI Resources
'    ' -------------------------------
'    Private ballBrush As SolidBrush
'    Private fpsBrush As SolidBrush
'    Private fpsFont As Font
'    Private trailBrushes As SolidBrush()
'    Private paddleBrush As SolidBrush
'    Private playerLabelBrush As SolidBrush
'    Private scoreBrush As SolidBrush

'    Private whiteBrush As SolidBrush
'    Private grayBrush As SolidBrush
'    Private dimBrush As SolidBrush

'    Private fullscreenIndicatorFont As Font
'    Private fullscreenIndicatorBrush As SolidBrush

'    ' -------------------------------
'    '  Trail System
'    ' -------------------------------
'    Private trail As New List(Of PointF)
'    Private trailLength As Integer = 10
'    Private trailSizes As Integer()
'    Private trailOffsets As Single()
'    Private trailAlpha As Integer()

'    ' -------------------------------
'    '  Pong State
'    ' -------------------------------
'    Private paddleLeft As RectangleF
'    Private paddleRight As RectangleF

'    Private paddleWidth As Integer = 32
'    Private paddleHeight As Integer = 128

'    ' -------------------------------
'    '  Player Names / Scores
'    ' -------------------------------
'    Private leftPlayerName As String = "Left"
'    Private rightPlayerName As String = "Right"
'    Private scoreLeft As Integer = 0
'    Private scoreRight As Integer = 0

'    ' -------------------------------
'    '  HUD cached layout
'    ' -------------------------------
'    Private leftScoreX As Single
'    Private rightScoreX As Single
'    Private leftLabelX As Single
'    Private rightLabelX As Single
'    Private scoreY As Single
'    Private labelY As Single

'    Private leftScoreSize As SizeF
'    Private rightScoreSize As SizeF
'    Private leftLabelSize As SizeF
'    Private rightLabelSize As SizeF

'    ' -------------------------------
'    '  Keyboard hints
'    ' -------------------------------
'    Private showKeyboardHints As Boolean = True

'    Private hintLeftText As String
'    Private hintRightText As String
'    Private pauseText As String
'    Private fpsText As String

'    Private hintLeftSize As SizeF
'    Private hintRightSize As SizeF
'    Private pauseSize As SizeF
'    Private fpsSize As SizeF

'    Private hintLeftX As Single = 10
'    Private hintLeftY As Single = 10

'    Private hintRightX As Single
'    Private hintRightY As Single = 10

'    Private pauseX As Single = 10
'    Private pauseY As Single

'    Private fpsX As Single
'    Private fpsY As Single

'    ' -------------------------------
'    '  Fonts for HUD / Screens
'    ' -------------------------------
'    Private hudScoreFont As Font
'    Private hudLabelFont As Font
'    Private pauseTitleFont As Font
'    Private pauseMenuFont As Font
'    Private startTitleFont As Font
'    Private aiDifficultyTitleFont As Font
'    Private startMenuFont As Font
'    Private startInfoFont As Font
'    Private gameOverFont As Font
'    Private gameOverInfoFont As Font

'    ' -------------------------------
'    '  Pause layout cache
'    ' -------------------------------
'    Private pauseTitle As String = "PAUSED"
'    Private pauseTitleSize As SizeF
'    Private pauseTitleX As Single
'    Private pauseTitleY As Single

'    Private pauseMenuItems() As String = {"Resume", "New", "Quit"}
'    Private pauseMenuItemSizes() As SizeF
'    Private pauseMenuItemX() As Single
'    Private pauseMenuItemY() As Single

'    Private pauseMenuStartY As Single
'    Private pauseMenuSpacing As Single

'    ' -------------------------------
'    '  Constructor
'    ' -------------------------------
'    Public Sub New(g As Graphics, clientSize As Size)
'        ScaleBallDiameter(clientSize)
'        InitGraphics(g, clientSize)
'        InitTrails()
'        InitPaddles(clientSize)
'        fpsStopwatch.Start()
'    End Sub

'    ' -------------------------------
'    '  Initialization
'    ' -------------------------------
'    Private Sub ScaleBallDiameter(clientSize As Size)
'        ballDiameter = CInt(clientSize.Height / 18.0F)
'    End Sub

'    Private Sub InitGraphics(g As Graphics, clientSize As Size)
'        ballBrush = New SolidBrush(Color.DeepSkyBlue)
'        fpsBrush = New SolidBrush(Color.Gray)
'        fpsFont = New Font("Segoe UI", 14, FontStyle.Bold)
'        paddleBrush = New SolidBrush(Color.White)
'        playerLabelBrush = New SolidBrush(Color.Gray)
'        scoreBrush = New SolidBrush(Color.White)

'        whiteBrush = New SolidBrush(Color.White)
'        grayBrush = New SolidBrush(Color.FromArgb(140, 140, 140))
'        dimBrush = New SolidBrush(Color.FromArgb(120, 0, 0, 0))
'        fullscreenIndicatorBrush = New SolidBrush(Color.FromArgb(120, 255, 255, 255))

'        RescaleFonts(g, clientSize)
'    End Sub

'    Private Sub InitPaddles(clientSize As Size)
'        paddleHeight = clientSize.Height / 8
'        paddleWidth = clientSize.Height / 25

'        paddleLeft = New RectangleF(
'            clientSize.Height / 25,
'            (clientSize.Height - paddleHeight) / 2,
'            paddleWidth,
'            paddleHeight)

'        paddleRight = New RectangleF(
'            clientSize.Width - clientSize.Height / 25 - paddleWidth,
'            (clientSize.Height - paddleHeight) / 2,
'            paddleWidth,
'            paddleHeight)
'    End Sub

'    Private Sub InitTrails()
'        trailSizes = New Integer(trailLength - 1) {}
'        trailOffsets = New Single(trailLength - 1) {}
'        trailAlpha = New Integer(trailLength - 1) {}
'        trailBrushes = New SolidBrush(trailLength - 1) {}

'        For i As Integer = 0 To trailLength - 1
'            Dim size As Integer = ballDiameter - (trailLength - i) * 2
'            If size < 10 Then size = 10
'            trailSizes(i) = size

'            trailOffsets(i) = CSng((ballDiameter - size) / 2)

'            Dim t As Double = i / CDbl(trailLength)
'            Dim alpha As Integer = CInt(16 * t * t)
'            trailAlpha(i) = alpha

'            trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
'        Next
'    End Sub

'    ' -------------------------------
'    '  Rescale fonts + layout
'    ' -------------------------------
'    Public Sub RescaleFonts(g As Graphics, clientSize As Size)
'        hudScoreFont?.Dispose()
'        hudLabelFont?.Dispose()
'        pauseTitleFont?.Dispose()
'        aiDifficultyTitleFont?.Dispose()
'        pauseMenuFont?.Dispose()
'        startTitleFont?.Dispose()
'        startMenuFont?.Dispose()
'        startInfoFont?.Dispose()
'        gameOverFont?.Dispose()
'        gameOverInfoFont?.Dispose()
'        fullscreenIndicatorFont?.Dispose()
'        fpsFont?.Dispose()

'        hudScoreFont = New Font("Segoe UI", CSng(clientSize.Height / 12.0F), FontStyle.Bold)
'        hudLabelFont = New Font("Segoe UI", CSng(clientSize.Height / 50.0F), FontStyle.Regular)

'        pauseTitleFont = New Font("Segoe UI", CSng(clientSize.Height / 18.0F), FontStyle.Bold)
'        pauseMenuFont = New Font("Segoe UI", CSng(clientSize.Height / 28.0F), FontStyle.Regular)

'        startTitleFont = New Font("Segoe UI", CSng(clientSize.Height / 10.0F), FontStyle.Bold)
'        aiDifficultyTitleFont = New Font("Segoe UI", CSng(clientSize.Height / 18.0F), FontStyle.Bold)

'        startMenuFont = New Font("Segoe UI", CSng(clientSize.Height / 30.0F), FontStyle.Regular)
'        startInfoFont = New Font("Segoe UI", CSng(clientSize.Height / 35.0F), FontStyle.Regular)

'        gameOverFont = New Font("Segoe UI", CSng(clientSize.Height / 20.0F), FontStyle.Bold)
'        gameOverInfoFont = New Font("Segoe UI", CSng(clientSize.Height / 35.0F), FontStyle.Regular)

'        fullscreenIndicatorFont = New Font("Segoe UI", CSng(clientSize.Height / 75.0F), FontStyle.Regular)
'        fpsFont = New Font("Segoe UI", CSng(clientSize.Height / 75.0F), FontStyle.Bold)

'        CachePauseLayout(g, clientSize)
'        CacheHUDLayout(g, clientSize)
'        RecomputeKeyboardHintLayout(g, clientSize)
'        RescalePaddles(clientSize)
'    End Sub

'    Private Sub CachePauseLayout(g As Graphics, clientSize As Size)
'        pauseTitleSize = g.MeasureString(pauseTitle, pauseTitleFont)
'        pauseTitleX = (clientSize.Width - pauseTitleSize.Width) / 2.0F
'        pauseTitleY = clientSize.Height * 0.22F

'        pauseMenuSpacing = clientSize.Height * 0.11F
'        pauseMenuStartY = pauseTitleY + pauseTitleSize.Height + (clientSize.Height * 0.05F)

'        ReDim pauseMenuItemSizes(pauseMenuItems.Length - 1)
'        ReDim pauseMenuItemX(pauseMenuItems.Length - 1)
'        ReDim pauseMenuItemY(pauseMenuItems.Length - 1)

'        For i As Integer = 0 To pauseMenuItems.Length - 1
'            pauseMenuItemSizes(i) = g.MeasureString(pauseMenuItems(i), pauseMenuFont)
'            pauseMenuItemX(i) = (clientSize.Width - pauseMenuItemSizes(i).Width) / 2.0F
'            pauseMenuItemY(i) = pauseMenuStartY + i * pauseMenuSpacing
'        Next
'    End Sub

'    Private Sub CacheHUDLayout(g As Graphics, clientSize As Size)
'        Dim halfWidth As Single = clientSize.Width / 2.0F

'        leftScoreSize = g.MeasureString(scoreLeft.ToString(), hudScoreFont)
'        rightScoreSize = g.MeasureString(scoreRight.ToString(), hudScoreFont)

'        leftLabelSize = g.MeasureString(leftPlayerName, hudLabelFont)
'        rightLabelSize = g.MeasureString(rightPlayerName, hudLabelFont)

'        scoreY = 10 + CSng(clientSize.Height / 25.0F)
'        labelY = scoreY - CSng(clientSize.Height / 200.0F)

'        leftScoreX = (halfWidth - leftScoreSize.Width) / 2.0F
'        rightScoreX = halfWidth + (halfWidth - rightScoreSize.Width) / 2.0F

'        leftLabelX = (halfWidth - leftLabelSize.Width) / 2.0F
'        rightLabelX = halfWidth + (halfWidth - rightLabelSize.Width) / 2.0F
'    End Sub

'    Private Sub RescalePaddles(clientSize As Size)
'        paddleHeight = clientSize.Height / 8
'        paddleWidth = clientSize.Height / 25

'        paddleLeft.Width = paddleWidth
'        paddleLeft.Height = paddleHeight
'        paddleRight.Width = paddleWidth
'        paddleRight.Height = paddleHeight

'        paddleLeft.X = clientSize.Height / 25
'        paddleRight.X = clientSize.Width - clientSize.Height / 25 - paddleWidth

'        ' Keep them centered vertically after a resize
'        paddleLeft.Y = (clientSize.Height - paddleHeight) / 2
'        paddleRight.Y = (clientSize.Height - paddleHeight) / 2
'    End Sub

'    Private Sub RecomputeKeyboardHintLayout(g As Graphics, clientSize As Size)
'        hintLeftText = "W S - Move Paddle"
'        hintLeftSize = g.MeasureString(hintLeftText, fullscreenIndicatorFont)

'        If playerMode = 2 Then
'            hintRightText = "Arrows - Move Paddle"
'        Else
'            hintRightText = "P - Pause Match"
'        End If

'        hintRightSize = g.MeasureString(hintRightText, fullscreenIndicatorFont)
'        hintRightX = clientSize.Width - hintRightSize.Width - 10

'        If playerMode = 2 Then
'            pauseText = "P - Pause Match"
'        Else
'            pauseText = ""
'        End If

'        pauseSize = g.MeasureString(pauseText, fullscreenIndicatorFont)
'        pauseY = clientSize.Height - pauseSize.Height - 10

'        fpsText = $"FPS: {fps}"
'        fpsSize = g.MeasureString(fpsText, fullscreenIndicatorFont)
'        fpsX = clientSize.Width - fpsSize.Width - 10
'        fpsY = clientSize.Height - fpsSize.Height - 10
'    End Sub

'    ' -------------------------------
'    '  Update methods
'    ' -------------------------------
'    Public Sub UpdateFPS()
'        frameCount += 1

'        If fpsStopwatch.ElapsedMilliseconds >= 1000 Then
'            fps = frameCount
'            fpsText = $"FPS: {fps}"
'            frameCount = 0
'            fpsStopwatch.Restart()
'        End If
'    End Sub

'    Public Sub UpdateBallPosition(deltaTime As Double)
'        ballPos.X += CSng(velX * deltaTime)
'        ballPos.Y += CSng(velY * deltaTime)
'    End Sub

'    Public Sub UpdateBallPosition(newPos As PointF)
'        ballPos = newPos
'    End Sub

'    Public Sub UpdateBallVelocity(newVelX As Double, newVelY As Double)
'        velX = newVelX
'        velY = newVelY
'    End Sub

'    Public Sub UpdateBallDiameter(newDiameter As Integer)
'        ballDiameter = newDiameter
'    End Sub

'    Public Sub UpdateBallBrush(newBrush As SolidBrush)
'        ballBrush?.Dispose()
'        ballBrush = newBrush
'    End Sub

'    Public Sub UpdateTrail()
'        If trailSizes Is Nothing OrElse trailOffsets Is Nothing OrElse trailAlpha Is Nothing Then
'            Return
'        End If

'        trail.Add(ballPos)

'        If trail.Count > trailLength Then
'            trail.RemoveAt(0)
'        End If

'        Dim count As Integer = trail.Count

'        If count = 1 Then
'            trailSizes(0) = ballDiameter
'            trailOffsets(0) = 0
'            trailAlpha(0) = 32
'            Return
'        End If

'        For i As Integer = 0 To count - 1
'            Dim t As Double = i / (count - 1)
'            trailSizes(i) = CInt(ballDiameter * (0.5 + 0.5 * t))
'            trailOffsets(i) = CSng(ballDiameter * (0.25 * (1 - t)))
'            trailAlpha(i) = CInt(32 * (0.2 + 0.8 * t))
'        Next
'    End Sub

'    Public Sub ClearTrail()
'        trail.Clear()
'        For i As Integer = 0 To trailLength - 1
'            trailSizes(i) = 0
'            trailOffsets(i) = 0
'            trailAlpha(i) = 0
'        Next
'    End Sub

'    Public Sub UpdateScore(leftScore As Integer, rightScore As Integer)
'        Me.scoreLeft = leftScore
'        Me.scoreRight = rightScore
'    End Sub

'    Public Sub UpdatePlayerNames(leftName As String, rightName As String)
'        Me.leftPlayerName = leftName
'        Me.rightPlayerName = rightName
'    End Sub

'    Public Sub UpdatePaddlePositions(leftY As Single, rightY As Single)
'        paddleLeft.Y = leftY
'        paddleRight.Y = rightY
'    End Sub

'    Public Sub ToggleKeyboardHints()
'        showKeyboardHints = Not showKeyboardHints
'    End Sub

'    ' -------------------------------
'    '  Draw methods
'    ' -------------------------------
'    Public Sub DrawGamePlayScreen(g As Graphics)
'        DrawTrail(g)
'        DrawBall(g)
'        DrawPaddles(g)
'        DrawHUD(g)
'        If showKeyboardHints Then DrawKeyboardHintsGamePlayScreen(g)
'    End Sub

'    Public Sub DrawTrail(g As Graphics)
'        If trail Is Nothing OrElse
'           trailSizes Is Nothing OrElse
'           trailOffsets Is Nothing OrElse
'           trailBrushes Is Nothing Then
'            Exit Sub
'        End If

'        Dim count As Integer = trail.Count
'        If count > trailLength Then count = trailLength

'        For i As Integer = 0 To count - 1
'            Dim p As PointF = trail(i)
'            Dim offset As Single = trailOffsets(i)

'            Dim c As Color = trailBrushes(i).Color
'            trailBrushes(i).Color = Color.FromArgb(trailAlpha(i), c.R, c.G, c.B)

'            g.FillEllipse(trailBrushes(i),
'                          p.X + offset,
'                          p.Y + offset,
'                          trailSizes(i),
'                          trailSizes(i))
'        Next
'    End Sub

'    Public Sub DrawBall(g As Graphics)
'        g.FillEllipse(ballBrush, ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
'    End Sub

'    Public Sub DrawPaddles(g As Graphics)
'        g.FillRectangle(paddleBrush, paddleLeft)
'        g.FillRectangle(paddleBrush, paddleRight)
'    End Sub

'    Public Sub DrawHUD(g As Graphics)
'        g.DrawString(leftPlayerName, hudLabelFont, playerLabelBrush, leftLabelX, labelY)
'        g.DrawString(rightPlayerName, hudLabelFont, playerLabelBrush, rightLabelX, labelY)
'        g.DrawString(scoreLeft.ToString(), hudScoreFont, scoreBrush, leftScoreX, scoreY)
'        g.DrawString(scoreRight.ToString(), hudScoreFont, scoreBrush, rightScoreX, scoreY)
'    End Sub

'    Public Sub DrawKeyboardHintsGamePlayScreen(g As Graphics)
'        If Not showKeyboardHints Then Return

'        g.DrawString(hintLeftText, fullscreenIndicatorFont, fullscreenIndicatorBrush, hintLeftX, hintLeftY)
'        g.DrawString(hintRightText, fullscreenIndicatorFont, fullscreenIndicatorBrush, hintRightX, hintRightY)
'        g.DrawString(pauseText, fullscreenIndicatorFont, fullscreenIndicatorBrush, pauseX, pauseY)
'        g.DrawString(fpsText, fullscreenIndicatorFont, fullscreenIndicatorBrush, fpsX, fpsY)
'    End Sub

'End Class




'Imports System.Runtime.InteropServices
'Imports System.Diagnostics
'Imports System.Drawing.Drawing2D

'Public Class Rendering
'    Implements IDisposable

'    ' -------------------------------
'    '  Ball / Physics (visual only)
'    ' -------------------------------
'    Private ballPos As PointF
'    Private ballDiameter As Integer = 60
'    Private velX As Double
'    Private velY As Double

'    ' -------------------------------
'    '  FPS Tracking
'    ' -------------------------------
'    Private frameCount As Integer = 0
'    Private fps As Integer = 0
'    Private fpsStopwatch As New Stopwatch()

'    ' -------------------------------
'    '  GDI Resources
'    ' -------------------------------
'    Private ballBrush As SolidBrush
'    Private fpsBrush As SolidBrush
'    Private fpsFont As Font
'    Private trailBrushes As SolidBrush()
'    Private paddleBrush As SolidBrush
'    Private playerLabelBrush As SolidBrush
'    Private scoreBrush As SolidBrush

'    Private whiteBrush As SolidBrush
'    Private grayBrush As SolidBrush
'    Private dimBrush As SolidBrush

'    Private fullscreenIndicatorFont As Font
'    Private fullscreenIndicatorBrush As SolidBrush

'    ' -------------------------------
'    '  Trail System (circular buffer)
'    ' -------------------------------
'    Private trailLength As Integer = 10
'    Private trailPoints() As PointF
'    Private trailSizes() As Integer
'    Private trailOffsets() As Single
'    Private trailAlpha() As Integer
'    Private trailIndex As Integer = -1
'    Private trailCount As Integer = 0

'    ' -------------------------------
'    '  Pong State (visual only)
'    ' -------------------------------
'    Private paddleLeft As RectangleF
'    Private paddleRight As RectangleF

'    Private paddleWidth As Integer = 32
'    Private paddleHeight As Integer = 128

'    ' -------------------------------
'    '  Player Names / Scores
'    ' -------------------------------
'    Private leftPlayerName As String = "Left"
'    Private rightPlayerName As String = "Right"
'    Private scoreLeft As Integer = 0
'    Private scoreRight As Integer = 0

'    ' -------------------------------
'    '  HUD cached layout
'    ' -------------------------------
'    Private leftScoreX As Single
'    Private rightScoreX As Single
'    Private leftLabelX As Single
'    Private rightLabelX As Single
'    Private scoreY As Single
'    Private labelY As Single

'    Private leftScoreSize As SizeF
'    Private rightScoreSize As SizeF
'    Private leftLabelSize As SizeF
'    Private rightLabelSize As SizeF

'    ' -------------------------------
'    '  Keyboard hints
'    ' -------------------------------
'    Private showKeyboardHints As Boolean = True

'    Private hintLeftText As String
'    Private hintRightText As String
'    Private pauseText As String
'    Private fpsText As String

'    Private hintLeftSize As SizeF
'    Private hintRightSize As SizeF
'    Private pauseSize As SizeF
'    Private fpsSize As SizeF

'    Private hintLeftX As Single = 10
'    Private hintLeftY As Single = 10

'    Private hintRightX As Single
'    Private hintRightY As Single = 10

'    Private pauseX As Single = 10
'    Private pauseY As Single

'    Private fpsX As Single
'    Private fpsY As Single

'    ' -------------------------------
'    '  Fonts for HUD / Screens
'    ' -------------------------------
'    Private hudScoreFont As Font
'    Private hudLabelFont As Font
'    Private pauseTitleFont As Font
'    Private pauseMenuFont As Font
'    Private startTitleFont As Font
'    Private aiDifficultyTitleFont As Font
'    Private startMenuFont As Font
'    Private startInfoFont As Font
'    Private gameOverFont As Font
'    Private gameOverInfoFont As Font

'    ' -------------------------------
'    '  Pause layout cache
'    ' -------------------------------
'    Private pauseTitle As String = "PAUSED"
'    Private pauseTitleSize As SizeF
'    Private pauseTitleX As Single
'    Private pauseTitleY As Single

'    Private pauseMenuItems() As String = {"Resume", "New", "Quit"}
'    Private pauseMenuItemSizes() As SizeF
'    Private pauseMenuItemX() As Single
'    Private pauseMenuItemY() As Single

'    Private pauseMenuStartY As Single
'    Private pauseMenuSpacing As Single

'    ' -------------------------------
'    '  Mode (needed for hints only)
'    ' -------------------------------
'    Private playerMode As Integer = 1   ' 1 = Single Player (AI), 2 = Two Players




'    ' -------------------------------
'    '  Game State
'    ' -------------------------------
'    Public Enum GameState
'        StartScreen
'        Playing
'        EndScreen
'        Pause
'        AIDifficulty
'    End Enum

'    Private currentState As GameState = GameState.StartScreen





'    Private formBorderStyle As FormBorderStyle
'    Private clientSize As Size





'    ' -------------------------------
'    '  Start Screen FX
'    ' -------------------------------
'    Private titleAlpha As Integer = 0
'    Private titleFadeIn As Boolean = True
'    Private blinkVisible As Boolean = True
'    Private blinkStopwatch As New Stopwatch()


'    Public Sub UpdateFormState(style As FormBorderStyle, size As Size)
'        formBorderStyle = style
'        clientSize = size
'    End Sub







'    ' -------------------------------
'    '  Constructor
'    ' -------------------------------
'    Public Sub New(g As Graphics, clientSize As Size)
'        g.SmoothingMode = SmoothingMode.AntiAlias
'        g.PixelOffsetMode = PixelOffsetMode.HighQuality

'        ScaleBallDiameter(clientSize)
'        InitGraphics(g, clientSize)
'        InitTrails()
'        InitPaddles(clientSize)
'        CacheHUDLayout(g, clientSize)
'        RecomputeKeyboardHintLayout(g, clientSize)

'        fpsStopwatch.Start()
'    End Sub

'    ' -------------------------------
'    '  Initialization
'    ' -------------------------------
'    Private Sub ScaleBallDiameter(clientSize As Size)
'        Dim minSide As Integer = Math.Min(clientSize.Width, clientSize.Height)
'        ballDiameter = CInt(minSide / 18.0F)
'    End Sub

'    Private Sub InitGraphics(g As Graphics, clientSize As Size)
'        ballBrush = New SolidBrush(Color.DeepSkyBlue)
'        fpsBrush = New SolidBrush(Color.Gray)
'        fpsFont = New Font("Segoe UI", CSng(clientSize.Height / 75.0F), FontStyle.Bold)

'        paddleBrush = New SolidBrush(Color.White)
'        playerLabelBrush = New SolidBrush(Color.Gray)
'        scoreBrush = New SolidBrush(Color.White)

'        whiteBrush = New SolidBrush(Color.White)
'        grayBrush = New SolidBrush(Color.FromArgb(140, 140, 140))
'        dimBrush = New SolidBrush(Color.FromArgb(120, 0, 0, 0))
'        fullscreenIndicatorBrush = New SolidBrush(Color.FromArgb(120, 255, 255, 255))

'        RescaleFonts(g, clientSize)
'    End Sub

'    Private Sub InitPaddles(clientSize As Size)
'        paddleHeight = clientSize.Height / 8
'        paddleWidth = clientSize.Width / 50

'        paddleLeft = New RectangleF(
'            clientSize.Width / 50.0F,
'            (clientSize.Height - paddleHeight) / 2.0F,
'            paddleWidth,
'            paddleHeight)

'        paddleRight = New RectangleF(
'            clientSize.Width - clientSize.Width / 50.0F - paddleWidth,
'            (clientSize.Height - paddleHeight) / 2.0F,
'            paddleWidth,
'            paddleHeight)
'    End Sub

'    Private Sub InitTrails()
'        trailPoints = New PointF(trailLength - 1) {}
'        trailSizes = New Integer(trailLength - 1) {}
'        trailOffsets = New Single(trailLength - 1) {}
'        trailAlpha = New Integer(trailLength - 1) {}
'        trailBrushes = New SolidBrush(trailLength - 1) {}

'        For i As Integer = 0 To trailLength - 1
'            Dim size As Integer = ballDiameter - (trailLength - i) * 2
'            If size < 10 Then size = 10
'            trailSizes(i) = size

'            trailOffsets(i) = CSng((ballDiameter - size) / 2.0F)

'            Dim t As Double = i / CDbl(trailLength)
'            Dim alpha As Integer = CInt(16 * t * t)
'            trailAlpha(i) = alpha

'            trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
'        Next
'    End Sub

'    ' -------------------------------
'    '  Rescale fonts + layout
'    ' -------------------------------
'    Public Sub RescaleFonts(g As Graphics, clientSize As Size)
'        DisposeFonts()

'        hudScoreFont = New Font("Segoe UI", CSng(clientSize.Height / 12.0F), FontStyle.Bold)
'        hudLabelFont = New Font("Segoe UI", CSng(clientSize.Height / 50.0F), FontStyle.Regular)

'        pauseTitleFont = New Font("Segoe UI", CSng(clientSize.Height / 18.0F), FontStyle.Bold)
'        pauseMenuFont = New Font("Segoe UI", CSng(clientSize.Height / 28.0F), FontStyle.Regular)

'        startTitleFont = New Font("Segoe UI", CSng(clientSize.Height / 10.0F), FontStyle.Bold)
'        aiDifficultyTitleFont = New Font("Segoe UI", CSng(clientSize.Height / 18.0F), FontStyle.Bold)

'        startMenuFont = New Font("Segoe UI", CSng(clientSize.Height / 30.0F), FontStyle.Regular)
'        startInfoFont = New Font("Segoe UI", CSng(clientSize.Height / 35.0F), FontStyle.Regular)

'        gameOverFont = New Font("Segoe UI", CSng(clientSize.Height / 20.0F), FontStyle.Bold)
'        gameOverInfoFont = New Font("Segoe UI", CSng(clientSize.Height / 35.0F), FontStyle.Regular)

'        fullscreenIndicatorFont = New Font("Segoe UI", CSng(clientSize.Height / 75.0F), FontStyle.Regular)
'        fpsFont = New Font("Segoe UI", CSng(clientSize.Height / 75.0F), FontStyle.Bold)

'        CachePauseLayout(g, clientSize)
'        CacheHUDLayout(g, clientSize)
'        RecomputeKeyboardHintLayout(g, clientSize)
'        RescalePaddles(clientSize)
'    End Sub

'    Private Sub CachePauseLayout(g As Graphics, clientSize As Size)
'        pauseTitleSize = g.MeasureString(pauseTitle, pauseTitleFont)
'        pauseTitleX = (clientSize.Width - pauseTitleSize.Width) / 2.0F
'        pauseTitleY = clientSize.Height * 0.22F

'        pauseMenuSpacing = clientSize.Height * 0.11F
'        pauseMenuStartY = pauseTitleY + pauseTitleSize.Height + (clientSize.Height * 0.05F)

'        ReDim pauseMenuItemSizes(pauseMenuItems.Length - 1)
'        ReDim pauseMenuItemX(pauseMenuItems.Length - 1)
'        ReDim pauseMenuItemY(pauseMenuItems.Length - 1)

'        For i As Integer = 0 To pauseMenuItems.Length - 1
'            pauseMenuItemSizes(i) = g.MeasureString(pauseMenuItems(i), pauseMenuFont)
'            pauseMenuItemX(i) = (clientSize.Width - pauseMenuItemSizes(i).Width) / 2.0F
'            pauseMenuItemY(i) = pauseMenuStartY + i * pauseMenuSpacing
'        Next
'    End Sub

'    Private Sub CacheHUDLayout(g As Graphics, clientSize As Size)
'        Dim halfWidth As Single = clientSize.Width / 2.0F

'        leftScoreSize = g.MeasureString(scoreLeft.ToString(), hudScoreFont)
'        rightScoreSize = g.MeasureString(scoreRight.ToString(), hudScoreFont)

'        leftLabelSize = g.MeasureString(leftPlayerName, hudLabelFont)
'        rightLabelSize = g.MeasureString(rightPlayerName, hudLabelFont)

'        scoreY = 10 + CSng(clientSize.Height / 25.0F)
'        labelY = scoreY - CSng(clientSize.Height / 200.0F)

'        leftScoreX = (halfWidth - leftScoreSize.Width) / 2.0F
'        rightScoreX = halfWidth + (halfWidth - rightScoreSize.Width) / 2.0F

'        leftLabelX = (halfWidth - leftLabelSize.Width) / 2.0F
'        rightLabelX = halfWidth + (halfWidth - rightLabelSize.Width) / 2.0F
'    End Sub

'    Private Sub RescalePaddles(clientSize As Size)
'        paddleHeight = clientSize.Height / 8
'        paddleWidth = clientSize.Width / 50

'        paddleLeft.Width = paddleWidth
'        paddleLeft.Height = paddleHeight
'        paddleRight.Width = paddleWidth
'        paddleRight.Height = paddleHeight

'        paddleLeft.X = clientSize.Width / 50.0F
'        paddleRight.X = clientSize.Width - clientSize.Width / 50.0F - paddleWidth

'        paddleLeft.Y = (clientSize.Height - paddleHeight) / 2.0F
'        paddleRight.Y = (clientSize.Height - paddleHeight) / 2.0F
'    End Sub

'    Private Sub RecomputeKeyboardHintLayout(g As Graphics, clientSize As Size)
'        hintLeftText = "W S - Move Paddle"
'        hintLeftSize = g.MeasureString(hintLeftText, fullscreenIndicatorFont)

'        If playerMode = 2 Then
'            hintRightText = "Arrows - Move Paddle"
'            pauseText = "P - Pause Match"
'        Else
'            hintRightText = "P - Pause Match"
'            pauseText = ""
'        End If

'        hintRightSize = g.MeasureString(hintRightText, fullscreenIndicatorFont)
'        hintRightX = clientSize.Width - hintRightSize.Width - 10

'        pauseSize = g.MeasureString(pauseText, fullscreenIndicatorFont)
'        pauseY = clientSize.Height - pauseSize.Height - 10

'        fpsText = $"FPS: {fps}"
'        fpsSize = g.MeasureString(fpsText, fullscreenIndicatorFont)
'        fpsX = clientSize.Width - fpsSize.Width - 10
'        fpsY = clientSize.Height - fpsSize.Height - 10
'    End Sub

'    ' -------------------------------
'    '  Update methods
'    ' -------------------------------
'    Public Sub UpdateFPS()
'        frameCount += 1

'        If fpsStopwatch.ElapsedMilliseconds >= 1000 Then
'            fps = frameCount
'            fpsText = $"FPS: {fps}"
'            frameCount = 0
'            fpsStopwatch.Restart()
'        End If
'    End Sub

'    Public Sub UpdateBallPosition(deltaTime As Double)
'        ballPos.X += CSng(velX * deltaTime)
'        ballPos.Y += CSng(velY * deltaTime)
'        UpdateTrail()
'    End Sub

'    Public Sub UpdateBallPosition(newPos As PointF)
'        ballPos = newPos
'        UpdateTrail()
'    End Sub

'    Public Sub UpdateBallVelocity(newVelX As Double, newVelY As Double)
'        velX = newVelX
'        velY = newVelY
'    End Sub

'    Public Sub UpdateBallDiameter(newDiameter As Integer)
'        ballDiameter = newDiameter
'        InitTrails()
'    End Sub

'    Public Sub UpdateBallBrush(newBrush As SolidBrush)
'        ballBrush?.Dispose()
'        ballBrush = newBrush
'    End Sub

'    Public Sub UpdateTrail()
'        If trailPoints Is Nothing Then Return

'        trailIndex = (trailIndex + 1) Mod trailLength
'        trailPoints(trailIndex) = ballPos

'        If trailCount < trailLength Then
'            trailCount += 1
'        End If

'        If trailCount = 1 Then
'            trailSizes(0) = ballDiameter
'            trailOffsets(0) = 0
'            trailAlpha(0) = 32
'            Return
'        End If





'        'For i As Integer = 0 To trailCount - 1
'        '    Dim t As Double = i / (trailCount - 1)
'        '    trailSizes(i) = CInt(ballDiameter * (0.5 + 0.5 * t))
'        '    trailOffsets(i) = CSng(ballDiameter * (0.25 * (1 - t)))
'        '    trailAlpha(i) = CInt(32 * (0.2 + 0.8 * t))
'        'Next




'        For i As Integer = 0 To trailCount - 1
'            Dim t As Double = 1.0 - (i / (trailCount - 1))   ' reversed taper

'            trailSizes(i) = CInt(ballDiameter * (0.5 + 0.5 * t))
'            trailOffsets(i) = CSng(ballDiameter * (0.25 * (1 - t)))
'            trailAlpha(i) = CInt(32 * (0.2 + 0.8 * t))
'        Next

'    End Sub

'    Public Sub ClearTrail()
'        trailCount = 0
'        trailIndex = -1
'        For i As Integer = 0 To trailLength - 1
'            trailSizes(i) = 0
'            trailOffsets(i) = 0
'            trailAlpha(i) = 0
'        Next
'    End Sub

'    Public Sub UpdateScore(leftScore As Integer, rightScore As Integer, g As Graphics, clientSize As Size)
'        Me.scoreLeft = leftScore
'        Me.scoreRight = rightScore
'        CacheHUDLayout(g, clientSize)
'    End Sub

'    Public Sub UpdatePlayerNames(leftName As String, rightName As String, g As Graphics, clientSize As Size)
'        Me.leftPlayerName = leftName
'        Me.rightPlayerName = rightName
'        CacheHUDLayout(g, clientSize)
'    End Sub

'    Public Sub UpdatePaddlePositions(leftY As Single, rightY As Single, clientSize As Size)
'        paddleLeft.Y = Math.Max(0, Math.Min(clientSize.Height - paddleHeight, leftY))
'        paddleRight.Y = Math.Max(0, Math.Min(clientSize.Height - paddleHeight, rightY))
'    End Sub

'    Public Sub ToggleKeyboardHints()
'        showKeyboardHints = Not showKeyboardHints
'    End Sub

'    Public Sub SetPlayerMode(mode As Integer, g As Graphics, clientSize As Size)
'        playerMode = mode
'        RecomputeKeyboardHintLayout(g, clientSize)
'    End Sub

'    ' -------------------------------
'    '  Draw methods
'    ' -------------------------------
'    Public Sub DrawGamePlayScreen(g As Graphics)
'        g.SmoothingMode = SmoothingMode.AntiAlias
'        DrawTrail(g)
'        DrawBall(g)
'        DrawPaddles(g)
'        DrawHUD(g)
'        If showKeyboardHints Then DrawKeyboardHintsGamePlayScreen(g)
'    End Sub

'    Private Sub DrawTrail(g As Graphics)
'        If trailCount <= 0 Then Return

'        For i As Integer = 0 To trailCount - 1
'            Dim index As Integer = (trailIndex - i + trailLength) Mod trailLength
'            Dim p As PointF = trailPoints(index)
'            Dim offset As Single = trailOffsets(i)

'            Dim alpha As Integer = trailAlpha(i)
'            If alpha <= 0 Then Continue For

'            Dim baseColor As Color = Color.FromArgb(alpha, 0, 191, 255)
'            trailBrushes(i).Color = baseColor

'            g.FillEllipse(trailBrushes(i),
'                          p.X + offset,
'                          p.Y + offset,
'                          trailSizes(i),
'                          trailSizes(i))
'        Next
'    End Sub

'    Private Sub DrawBall(g As Graphics)
'        g.FillEllipse(ballBrush, ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
'    End Sub

'    Private Sub DrawPaddles(g As Graphics)
'        g.FillRectangle(paddleBrush, paddleLeft)
'        g.FillRectangle(paddleBrush, paddleRight)
'    End Sub

'    Private Sub DrawHUD(g As Graphics)
'        g.DrawString(leftPlayerName, hudLabelFont, playerLabelBrush, leftLabelX, labelY)
'        g.DrawString(rightPlayerName, hudLabelFont, playerLabelBrush, rightLabelX, labelY)
'        g.DrawString(scoreLeft.ToString(), hudScoreFont, scoreBrush, leftScoreX, scoreY)
'        g.DrawString(scoreRight.ToString(), hudScoreFont, scoreBrush, rightScoreX, scoreY)
'    End Sub

'    Private Sub DrawKeyboardHintsGamePlayScreen(g As Graphics)
'        If Not showKeyboardHints Then Return

'        g.DrawString(hintLeftText, fullscreenIndicatorFont, fullscreenIndicatorBrush, hintLeftX, hintLeftY)
'        g.DrawString(hintRightText, fullscreenIndicatorFont, fullscreenIndicatorBrush, hintRightX, hintRightY)
'        If Not String.IsNullOrEmpty(pauseText) Then
'            g.DrawString(pauseText, fullscreenIndicatorFont, fullscreenIndicatorBrush, pauseX, pauseY)
'        End If
'        g.DrawString(fpsText, fullscreenIndicatorFont, fullscreenIndicatorBrush, fpsX, fpsY)
'    End Sub



'    'Public Sub Render(g As Graphics, state As GameState, showHints As Boolean)

'    '    Select Case state
'    '        Case GameState.StartScreen
'    '            DrawTrail(g)
'    '            DrawBall(g)
'    '            DrawStartScreen(g)
'    '            If showHints Then DrawKeyboardHintsStartScreen(g)

'    '        Case GameState.Playing
'    '            DrawGamePlayScreen(g)

'    '        Case GameState.EndScreen
'    '            DrawTrail(g)
'    '            DrawBall(g)
'    '            DrawHUD(g)
'    '            DrawGameOver(g)
'    '            If showHints Then DrawKeyboardHintsGameOverScreen(g)

'    '        Case GameState.Pause
'    '            DrawTrail(g)
'    '            DrawBall(g)
'    '            DrawPaddles(g)
'    '            DrawHUD(g)
'    '            DrawPauseScreen(g)
'    '            If showHints Then DrawKeyboardHintsPauseScreen(g)

'    '        Case GameState.AIDifficulty
'    '            DrawTrail(g)
'    '            DrawBall(g)
'    '            DrawAIDifficultyScreen(g)
'    '            If showHints Then DrawKeyboardHintsAIDifficultyScreen(g)
'    '    End Select

'    'End Sub



'    Public Sub Render(g As Graphics, state As GameState, showHints As Boolean)

'        Select Case state

'            Case GameState.StartScreen
'                DrawTrail(g)
'                DrawBall(g)
'                DrawStartScreen(g)
'                If showHints Then DrawKeyboardHintsStartScreen(g)

'            Case GameState.Playing
'                DrawGamePlayScreen(g)
'                If showHints Then DrawKeyboardHintsGamePlayScreen(g)

'            Case GameState.Pause
'                DrawTrail(g)
'                DrawBall(g)
'                DrawPaddles(g)
'                DrawHUD(g)
'                DrawPauseScreen(g)
'                If showHints Then DrawKeyboardHintsPauseScreen(g)

'            Case GameState.EndScreen
'                DrawTrail(g)
'                DrawBall(g)
'                DrawHUD(g)
'                DrawGameOver(g)
'                If showHints Then DrawKeyboardHintsGameOverScreen(g)

'            Case GameState.AIDifficulty
'                DrawTrail(g)
'                DrawBall(g)
'                DrawAIDifficultyScreen(g)
'                If showHints Then DrawKeyboardHintsAIDifficultyScreen(g)

'        End Select

'    End Sub


'    Private Sub DrawKeyboardHintsStartScreen(g As Graphics)

'        ' -------------------------------
'        '  Keyboard Hints (Top‑Left)
'        ' -------------------------------
'        Dim hintText As String =
'        "1 - One Player   2 - Two Players   Enter - Start Match"

'        Dim hintSize = g.MeasureString(hintText, fullscreenIndicatorFont)

'        g.DrawString(hintText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 10,
'                 10)


'        ' -------------------------------
'        '  Fullscreen Indicator (Top‑Right)
'        ' -------------------------------
'        Dim fsText As String =
'        If(Me.formBorderStyle = FormBorderStyle.None,
'           "F - Exit Fullscreen",
'           "F - Fullscreen")

'        Dim fsSize = g.MeasureString(fsText, fullscreenIndicatorFont)

'        g.DrawString(fsText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 clientSize.Width - fsSize.Width - 10,
'                 10)


'        ' -------------------------------
'        '  Hide Keyboard Hints (Bottom‑Left)
'        ' -------------------------------
'        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
'        Dim hideSize = g.MeasureString(hideText, fullscreenIndicatorFont)

'        g.DrawString(hideText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 10,
'                 clientSize.Height - hideSize.Height - 10)

'    End Sub


'    Private Sub DrawStartScreen(g As Graphics)

'        ' -------------------------------
'        '  Title
'        ' -------------------------------
'        Dim title As String = "PONG"
'        Dim titleSize = g.MeasureString(title, startTitleFont)
'        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

'        Using titleBrush As New SolidBrush(titleColor)
'            g.DrawString(title, startTitleFont, titleBrush,
'                     CSng((clientSize.Width - titleSize.Width) / 2.0F),
'                     CSng(clientSize.Height * 0.15F))
'        End Using


'        ' -------------------------------
'        '  Menu Options
'        ' -------------------------------
'        Dim option1 As String = "1 Player"
'        Dim option2 As String = "2 Players"

'        Dim opt1Size = g.MeasureString(option1, startMenuFont)
'        Dim opt2Size = g.MeasureString(option2, startMenuFont)

'        Dim opt1X As Single = CSng((clientSize.Width - opt1Size.Width) / 2.0F)
'        Dim opt1Y As Single = CSng(clientSize.Height * 0.41F)

'        Dim opt2X As Single = CSng((clientSize.Width - opt2Size.Width) / 2.0F)
'        Dim opt2Y As Single = CSng(clientSize.Height * 0.52F)

'        ' Store clickable rectangles
'        onePlayerOptionRect = New Rectangle(CInt(opt1X), CInt(opt1Y),
'                                        CInt(opt1Size.Width), CInt(opt1Size.Height))

'        twoPlayersOptionRect = New Rectangle(CInt(opt2X), CInt(opt2Y),
'                                         CInt(opt2Size.Width), CInt(opt2Size.Height))


'        ' -------------------------------
'        '  Highlight + Outline (Pause Menu Style)
'        ' -------------------------------

'        ' Option 1
'        If numberOfPlayersSelection = 0 Then
'            FillRoundedRectangle(g, lightBrush, onePlayerOptionRect, clientSize.Height / 64)
'            DrawRoundedRectangle(g, outlinePen, onePlayerOptionRect, clientSize.Height / 64)

'        Else
'            FillRoundedRectangle(g, darkBrush, onePlayerOptionRect, clientSize.Height / 64)
'            DrawRoundedRectangle(g, darkOutlinePen, onePlayerOptionRect, clientSize.Height / 64)

'        End If

'        ' Option 2
'        If numberOfPlayersSelection = 1 Then
'            FillRoundedRectangle(g, lightBrush, twoPlayersOptionRect, clientSize.Height / 64)
'            DrawRoundedRectangle(g, outlinePen, twoPlayersOptionRect, clientSize.Height / 64)

'        Else
'            FillRoundedRectangle(g, darkBrush, twoPlayersOptionRect, clientSize.Height / 64)
'            DrawRoundedRectangle(g, darkOutlinePen, twoPlayersOptionRect, clientSize.Height / 64)

'        End If


'        ' -------------------------------
'        '  Draw Text
'        ' -------------------------------
'        Dim opt1Brush As SolidBrush = If(numberOfPlayersSelection = 0, whiteBrush, grayBrush)
'        Dim opt2Brush As SolidBrush = If(numberOfPlayersSelection = 1, whiteBrush, grayBrush)

'        g.DrawString(option1, startMenuFont, opt1Brush, opt1X, opt1Y)
'        g.DrawString(option2, startMenuFont, opt2Brush, opt2X, opt2Y)


'        ' -------------------------------
'        '  Blink "Press SPACE"
'        ' -------------------------------
'        If blinkVisible Then
'            Dim info As String = "Press SPACE to Start"
'            Dim infoSize = g.MeasureString(info, startInfoFont)

'            g.DrawString(info, startInfoFont, whiteBrush,
'                     CSng((clientSize.Width - infoSize.Width) / 2.0F),
'                     CSng(clientSize.Height * 0.75F))
'        End If

'    End Sub




'    Private Sub DrawKeyboardHintsPauseScreen(g As Graphics)

'        ' -------------------------------
'        '  Keyboard Hints (Top‑Left)
'        ' -------------------------------
'        Dim hintText As String
'        hintText = $"R - Resume Match   N - New Match"
'        Dim hintSize = g.MeasureString(hintText, fullscreenIndicatorFont)

'        g.DrawString(hintText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 10,
'                 10)


'        ' -------------------------------
'        '  Fullscreen Indicator (Top-Right)
'        ' -------------------------------
'        Dim fsText As String =
'        If(Me.formBorderStyle = FormBorderStyle.None,
'           "F - Exit Fullscreen",
'           "F - Fullscreen")

'        Dim fsSize = g.MeasureString(fsText, fullscreenIndicatorFont)

'        g.DrawString(fsText,
'             fullscreenIndicatorFont,
'             grayBrush,
'             clientSize.Width - fsSize.Width - 10,
'             10)

'        ' -------------------------------
'        '  Quit Match (Bottom‑Left)
'        ' -------------------------------
'        Dim quitText As String = "Q - Quit Match"

'        Dim quitSize = g.MeasureString(quitText, fullscreenIndicatorFont)

'        g.DrawString(quitText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 10,
'                 clientSize.Height - quitSize.Height - 10)

'        ' -------------------------------
'        '  Hide Keyboard Hints (Bottom‑Right)
'        ' -------------------------------
'        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
'        Dim hideSize = g.MeasureString(hideText, fullscreenIndicatorFont)

'        g.DrawString(hideText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 clientSize.Width - hideSize.Width - 10,
'                 clientSize.Height - hideSize.Height - 10)


'    End Sub


'    'Private Sub DrawKeyboardHintsGamePlayScreen(g As Graphics)

'    '    ' Left paddle hint
'    '    g.DrawString(hintLeftText,
'    '             fullscreenIndicatorFont,
'    '             grayBrush,
'    '             hintLeftX,
'    '             hintLeftY)

'    '    ' Right paddle hint
'    '    g.DrawString(hintRightText,
'    '             fullscreenIndicatorFont,
'    '             grayBrush,
'    '             hintRightX,
'    '             hintRightY)

'    '    ' Pause hint (bottom-left)
'    '    g.DrawString(pauseText,
'    '             fullscreenIndicatorFont,
'    '             grayBrush,
'    '             pauseX,
'    '             pauseY)

'    '    ' FPS (bottom-right)
'    '    g.DrawString(fpsText,
'    '             fullscreenIndicatorFont,
'    '             grayBrush,
'    '             fpsX,
'    '             fpsY)

'    'End Sub


'    Private Sub DrawKeyboardHintsAIDifficultyScreen(g As Graphics)

'        ' -------------------------------
'        '  Keyboard Hints (Top‑Left)
'        ' -------------------------------
'        Dim hintText As String = "E - Easy   N - Normal   H - Hard   Enter - Start Match"
'        Dim hintSize = g.MeasureString(hintText, fullscreenIndicatorFont)

'        g.DrawString(hintText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 10,
'                 10)


'        ' -------------------------------
'        '  Fullscreen Indicator (Top‑Right)
'        ' -------------------------------
'        Dim fsText As String =
'        If(Me.formBorderStyle = FormBorderStyle.None,
'           "F - Exit Fullscreen",
'           "F - Fullscreen")

'        Dim fsSize = g.MeasureString(fsText, fullscreenIndicatorFont)

'        g.DrawString(fsText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 clientSize.Width - fsSize.Width - 10,
'                 10)


'        ' -------------------------------
'        '  Hide Keyboard Hints (Bottom‑Left)
'        ' -------------------------------
'        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
'        Dim hideSize = g.MeasureString(hideText, fullscreenIndicatorFont)

'        g.DrawString(hideText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 10,
'                 clientSize.Height - hideSize.Height - 10)

'    End Sub



'    Private Sub DrawAIDifficultyScreen(g As Graphics)

'        ' -------------------------------
'        '  Title
'        ' -------------------------------
'        Dim title As String = "Difficulty"
'        Dim titleSize = g.MeasureString(title, aiDifficultyTitleFont)
'        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

'        Using titleBrush As New SolidBrush(titleColor)
'            g.DrawString(title, aiDifficultyTitleFont, titleBrush,
'                     CSng((clientSize.Width - titleSize.Width) / 2.0F),
'                     CSng(clientSize.Height * 0.2F))
'        End Using


'        ' -------------------------------
'        '  Menu Options + Rectangles
'        ' -------------------------------
'        Dim baseY As Single = CSng(clientSize.Height * 0.41F)

'        For i As Integer = 0 To aiOptions.Length - 1

'            Dim text = aiOptions(i)
'            Dim size = g.MeasureString(text, startMenuFont)

'            Dim x = CSng((clientSize.Width - size.Width) / 2.0F)
'            Dim y = baseY + i * (size.Height + clientSize.Height * 0.03F)

'            Dim rect As New Rectangle(CInt(x), CInt(y),
'                                  CInt(size.Width), CInt(size.Height))

'            ' Store clickable rectangles
'            Select Case i
'                Case AIDifficultyLevel.Easy
'                    aiEasyRect = rect
'                Case AIDifficultyLevel.Normal
'                    aiNormalRect = rect
'                Case AIDifficultyLevel.Hard
'                    aiHardRect = rect
'            End Select


'            ' -------------------------------
'            '  Highlight (Pause Menu Style)
'            ' -------------------------------
'            If i = aiDifficultySelection Then
'                'g.FillRectangle(lightBrush, rect)
'                FillRoundedRectangle(g, lightBrush, rect, clientSize.Height / 64)

'                'g.DrawRectangle(outlinePen, rect)
'                DrawRoundedRectangle(g, outlinePen, rect, clientSize.Height / 64)

'            Else
'                'g.FillRectangle(darkBrush, rect)
'                FillRoundedRectangle(g, darkBrush, rect, clientSize.Height / 64)

'                'g.DrawRectangle(darkOutlinePen, rect)
'                DrawRoundedRectangle(g, darkOutlinePen, rect, clientSize.Height / 64)

'            End If


'            ' -------------------------------
'            '  Draw Text
'            ' -------------------------------
'            Dim brush As SolidBrush = If(i = aiDifficultySelection, whiteBrush, grayBrush)
'            g.DrawString(text, startMenuFont, brush, x, y)

'        Next


'        ' -------------------------------
'        '  Blink "Press SPACE"
'        ' -------------------------------
'        If blinkVisible Then
'            Dim info As String = "Press SPACE to Start"
'            Dim infoSize = g.MeasureString(info, startInfoFont)

'            g.DrawString(info, startInfoFont, whiteBrush,
'                     CSng((clientSize.Width - infoSize.Width) / 2.0F),
'                     CSng(clientSize.Height * 0.75F))
'        End If

'    End Sub



'    Private Sub DrawKeyboardHintsGameOverScreen(g As Graphics)


'        ' -------------------------------
'        '  Keyboard Hints (Top‑Left)
'        ' -------------------------------
'        Dim hintText As String = "Enter - Start New Match"
'        Dim hintSize = g.MeasureString(hintText, fullscreenIndicatorFont)

'        g.DrawString(hintText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 10,
'                 10)


'        ' -------------------------------
'        '  Fullscreen Indicator (Top-Right)
'        ' -------------------------------
'        Dim fsText As String =
'        If(Me.formBorderStyle = FormBorderStyle.None,
'           "F - Exit Fullscreen",
'           "F - Fullscreen")

'        Dim fsSize = g.MeasureString(fsText, fullscreenIndicatorFont)

'        g.DrawString(fsText,
'             fullscreenIndicatorFont,
'             grayBrush,
'             clientSize.Width - fsSize.Width - 10,
'             10)



'        ' -------------------------------
'        '  Quit Game (Bottom‑Left)
'        ' -------------------------------
'        Dim quitText As String = "CTRL Q - Quit Game"
'        Dim quitSize = g.MeasureString(quitText, fullscreenIndicatorFont)

'        g.DrawString(quitText,
'                 fullscreenIndicatorFont,
'                 grayBrush,
'                 10,
'                 clientSize.Height - quitSize.Height - 10)



'    End Sub

'    Private Sub DrawGameOver(g As Graphics)


'        ' -------------------------------
'        '  Title
'        ' -------------------------------
'        Dim title As String = winnerText
'        Dim titleSize = g.MeasureString(title, gameOverFont)
'        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

'        Using titleBrush As New SolidBrush(titleColor)
'            g.DrawString(title, gameOverFont, titleBrush,
'                     CSng((clientSize.Width - titleSize.Width) / 2.0F),
'                     CSng(clientSize.Height * 0.3F))
'        End Using


'        ' -------------------------------
'        '  Blink "Press SPACE"
'        ' -------------------------------
'        If blinkVisible Then
'            Dim info As String = "Press SPACE to Start"
'            Dim infoSize = g.MeasureString(info, startInfoFont)

'            g.DrawString(info, startInfoFont, whiteBrush,
'                     CSng((clientSize.Width - infoSize.Width) / 2.0F),
'                     CSng(clientSize.Height * 0.55F))
'        End If

'    End Sub






'    Private Sub DrawPauseScreen(g As Graphics)

'        ' Dim the game
'        g.FillRectangle(dimBrush, ClientRectangle)

'        ' Title
'        g.DrawString(pauseTitle, pauseTitleFont, whiteBrush,
'                 pauseTitleX, pauseTitleY)

'        ' Pre-created brushes/pens (created once in Form_Load)
'        ' darkBrush
'        ' outlinePen
'        ' darkOutlinePen

'        For i As Integer = 0 To pauseMenuItems.Length - 1

'            Dim text = pauseMenuItems(i)
'            Dim x = pauseMenuItemX(i)
'            Dim y = pauseMenuItemY(i)

'            ' Measure once per item
'            Dim size = g.MeasureString(text, pauseMenuFont)

'            ' Compute clickable rectangles
'            Dim rect As Rectangle = New Rectangle(CInt(x), CInt(y),
'                                              CInt(size.Width), CInt(size.Height))

'            Select Case i
'                Case 0 : pauseResumeRect = rect
'                Case 1 : pauseNewMatchRect = rect
'                Case 2 : pauseQuitRect = rect
'            End Select

'            ' Highlight selection
'            If i = pauseMenuSelection Then
'                'g.FillRectangle(lightBrush, rect)
'                FillRoundedRectangle(g, lightBrush, rect, clientSize.Height / 64)
'                'g.DrawRectangle(outlinePen, rect)
'                DrawRoundedRectangle(g, outlinePen, rect, clientSize.Height / 64)
'            Else
'                'g.FillRectangle(darkBrush, rect)
'                FillRoundedRectangle(g, darkBrush, rect, clientSize.Height / 64)

'                'g.DrawRectangle(darkOutlinePen, rect)
'                DrawRoundedRectangle(g, darkOutlinePen, rect, clientSize.Height / 64)

'            End If

'            ' Draw text
'            Dim brush As SolidBrush = If(i = pauseMenuSelection, whiteBrush, grayBrush)
'            g.DrawString(text, pauseMenuFont, brush, x, y)

'        Next

'    End Sub









'    ' -------------------------------
'    '  Disposal
'    ' -------------------------------
'    Private Sub DisposeFonts()
'        hudScoreFont?.Dispose()
'        hudLabelFont?.Dispose()
'        pauseTitleFont?.Dispose()
'        pauseMenuFont?.Dispose()
'        startTitleFont?.Dispose()
'        aiDifficultyTitleFont?.Dispose()
'        startMenuFont?.Dispose()
'        startInfoFont?.Dispose()
'        gameOverFont?.Dispose()
'        gameOverInfoFont?.Dispose()
'        fullscreenIndicatorFont?.Dispose()
'        fpsFont?.Dispose()
'    End Sub

'    Public Sub Dispose() Implements IDisposable.Dispose
'        ballBrush?.Dispose()
'        fpsBrush?.Dispose()
'        paddleBrush?.Dispose()
'        playerLabelBrush?.Dispose()
'        scoreBrush?.Dispose()
'        whiteBrush?.Dispose()
'        grayBrush?.Dispose()
'        dimBrush?.Dispose()
'        fullscreenIndicatorBrush?.Dispose()

'        If trailBrushes IsNot Nothing Then
'            For Each b In trailBrushes
'                b?.Dispose()
'            Next
'        End If

'        DisposeFonts()
'        fpsStopwatch?.Stop()
'    End Sub

'End Class





'Imports System.Runtime.InteropServices
'Imports System.Diagnostics
'Imports System.Drawing.Drawing2D

'Public Class Rendering
'    Implements IDisposable

'    ' ============================================================
'    '   CORE STATE
'    ' ============================================================
'    Public Enum GameState
'        StartScreen
'        Playing
'        Pause
'        EndScreen
'        AIDifficulty
'    End Enum

'    Private currentState As GameState = GameState.StartScreen

'    Private formBorderStyle As FormBorderStyle
'    Private clientSize As Size

'    ' ============================================================
'    '   BALL / PHYSICS (visual only)
'    ' ============================================================
'    Private ballPos As PointF
'    Private ballDiameter As Integer = 60
'    Private velX As Double
'    Private velY As Double

'    ' ============================================================
'    '   FPS TRACKING
'    ' ============================================================
'    Private frameCount As Integer
'    Private fps As Integer
'    Private fpsStopwatch As New Stopwatch()

'    ' ============================================================
'    '   GDI RESOURCES
'    ' ============================================================
'    Private ballBrush As SolidBrush
'    Private paddleBrush As SolidBrush
'    Private scoreBrush As SolidBrush
'    Private playerLabelBrush As SolidBrush
'    Private whiteBrush As SolidBrush
'    Private grayBrush As SolidBrush
'    Private dimBrush As SolidBrush
'    Private fullscreenIndicatorBrush As SolidBrush

'    Private hudScoreFont As Font
'    Private hudLabelFont As Font
'    Private pauseTitleFont As Font
'    Private pauseMenuFont As Font
'    Private startTitleFont As Font
'    Private startMenuFont As Font
'    Private startInfoFont As Font
'    Private aiDifficultyTitleFont As Font
'    Private gameOverFont As Font
'    Private gameOverInfoFont As Font
'    Private keyboardHintsFont As Font
'    Private fpsFont As Font

'    ' ============================================================
'    '   TRAIL SYSTEM (circular buffer)
'    ' ============================================================
'    Private trailLength As Integer = 20
'    Private trailPoints() As PointF
'    Private trailSizes() As Integer
'    Private trailOffsets() As Single
'    Private trailAlpha() As Integer
'    Private trailBrushes() As SolidBrush
'    Private trailIndex As Integer = -1
'    Private trailCount As Integer = 0

'    ' ============================================================
'    '   PADDLES
'    ' ============================================================
'    Private paddleLeft As RectangleF
'    Private paddleRight As RectangleF
'    Private paddleWidth As Integer = 32
'    Private paddleHeight As Integer = 128

'    ' ============================================================
'    '   PLAYER NAMES / SCORES
'    ' ============================================================
'    Private leftPlayerName As String = "Left"
'    Private rightPlayerName As String = "Right"
'    Private scoreLeft As Integer = 0
'    Private scoreRight As Integer = 0

'    ' ============================================================
'    '   HUD LAYOUT CACHE
'    ' ============================================================
'    Private leftScoreX As Single
'    Private rightScoreX As Single
'    Private leftLabelX As Single
'    Private rightLabelX As Single
'    Private scoreY As Single
'    Private labelY As Single

'    Private leftScoreSize As SizeF
'    Private rightScoreSize As SizeF
'    Private leftLabelSize As SizeF
'    Private rightLabelSize As SizeF

'    ' ============================================================
'    '   KEYBOARD HINTS
'    ' ============================================================
'    Private showKeyboardHints As Boolean = True

'    Private hintLeftText As String
'    Private hintRightText As String
'    Private pauseText As String
'    Private fpsText As String

'    Private hintLeftSize As SizeF
'    Private hintRightSize As SizeF
'    Private pauseSize As SizeF
'    Private fpsSize As SizeF

'    Private hintLeftX As Single = 10
'    Private hintLeftY As Single = 10
'    Private hintRightX As Single
'    Private hintRightY As Single = 10
'    Private pauseX As Single = 10
'    Private pauseY As Single
'    Private fpsX As Single
'    Private fpsY As Single

'    ' ============================================================
'    '   START SCREEN
'    ' ============================================================
'    Private numberOfPlayersSelection As Integer = 0
'    Private onePlayerOptionRect As Rectangle
'    Private twoPlayersOptionRect As Rectangle

'    Private titleAlpha As Integer = 255
'    Private titleFadeIn As Boolean = True

'    Private blinkVisible As Boolean = True
'    Private blinkStopwatch As New Stopwatch()

'    ' ============================================================
'    '   AI DIFFICULTY SCREEN
'    ' ============================================================
'    Private aiDifficultySelection As Integer = 0
'    Private aiOptions() As String = {"Easy", "Normal", "Hard"}

'    Private Enum AIDifficultyLevel
'        Easy
'        Normal
'        Hard
'    End Enum

'    Private aiEasyRect As Rectangle
'    Private aiNormalRect As Rectangle
'    Private aiHardRect As Rectangle

'    ' ============================================================
'    '   PAUSE SCREEN
'    ' ============================================================
'    Private pauseTitle As String = "PAUSED"
'    Private pauseTitleSize As SizeF
'    Private pauseTitleX As Single
'    Private pauseTitleY As Single

'    Private pauseMenuItems() As String = {"Resume", "New", "Quit"}
'    Private pauseMenuItemSizes() As SizeF
'    Private pauseMenuItemX() As Single
'    Private pauseMenuItemY() As Single
'    Private pauseMenuStartY As Single
'    Private pauseMenuSpacing As Single

'    Private pauseMenuSelection As Integer = 0
'    Private pauseResumeRect As Rectangle
'    Private pauseNewMatchRect As Rectangle
'    Private pauseQuitRect As Rectangle

'    ' ============================================================
'    '   GAME OVER SCREEN
'    ' ============================================================
'    Private winnerText As String = ""

'    ' ============================================================
'    '   UI BRUSHES / PENS
'    ' ============================================================
'    Private lightBrush As New SolidBrush(Color.FromArgb(32, 255, 255, 255))
'    Private darkBrush As New SolidBrush(Color.FromArgb(64, 0, 0, 0))
'    Private outlinePen As New Pen(Color.FromArgb(40, 255, 255, 255), 2)
'    Private darkOutlinePen As New Pen(Color.FromArgb(32, 255, 255, 255), 2)

'    ' A.I. Difficulty Layout Cache
'    Private aiDifficultyTitleSize As SizeF
'    Private aiDifficultyTitleX As Single
'    Private aiDifficultyTitleY As Single

'    Private aiOptionSizes() As SizeF
'    Private aiOptionX() As Single
'    Private aiOptionY() As Single

'    Private aiInfoSize As SizeF
'    Private aiInfoX As Single
'    Private aiInfoY As Single







'    Public Function OnePlayerRect() As Rectangle

'        Return onePlayerOptionRect

'    End Function


'    Public Function TwoPlayersRect() As Rectangle

'        Return twoPlayersOptionRect

'    End Function

'    Public Function EasyRect() As Rectangle
'        Return aiEasyRect
'    End Function

'    Public Function NormalRect() As Rectangle
'        Return aiNormalRect
'    End Function

'    Public Function HardRect() As Rectangle
'        Return aiHardRect
'    End Function



'    Public Function ResumeRect() As Rectangle
'        Return pauseResumeRect
'    End Function

'    Public Function NewMatchRect() As Rectangle
'        Return pauseNewMatchRect
'    End Function

'    Public Function QuitRect() As Rectangle
'        Return pauseQuitRect
'    End Function






'    ' ============================================================
'    '   CONSTRUCTOR
'    ' ============================================================
'    Public Sub New(g As Graphics, size As Size)
'        clientSize = size
'        g.SmoothingMode = SmoothingMode.AntiAlias
'        g.PixelOffsetMode = PixelOffsetMode.HighQuality

'        ScaleBallDiameter(size)
'        InitGraphics(g, size)
'        InitTrails()
'        InitPaddles(size)
'        CacheHUDLayout(g, size)
'        CachePauseLayout(g, size)

'        RecomputeKeyboardHintLayout(g, size)
'        CacheAIDifficultyScreenLayout(g, size)

'        fpsStopwatch.Start()
'        blinkStopwatch.Start()
'    End Sub

'    ' ============================================================
'    '   FORM STATE UPDATE
'    ' ============================================================
'    Public Sub UpdateFormState(style As FormBorderStyle, size As Size)
'        formBorderStyle = style
'        clientSize = size
'    End Sub

'    ' ============================================================
'    '   INITIALIZATION
'    ' ============================================================
'    Private Sub ScaleBallDiameter(size As Size)
'        ballDiameter = CInt(Math.Min(size.Width, size.Height) / 18.0F)
'    End Sub

'    Private Sub InitGraphics(g As Graphics, size As Size)
'        ballBrush = New SolidBrush(Color.DeepSkyBlue)
'        paddleBrush = New SolidBrush(Color.White)
'        playerLabelBrush = New SolidBrush(Color.Gray)
'        scoreBrush = New SolidBrush(Color.White)
'        whiteBrush = New SolidBrush(Color.White)
'        grayBrush = New SolidBrush(Color.FromArgb(140, 140, 140))
'        dimBrush = New SolidBrush(Color.FromArgb(120, 0, 0, 0))
'        fullscreenIndicatorBrush = New SolidBrush(Color.FromArgb(120, 255, 255, 255))

'        RescaleFonts(g, size)
'    End Sub

'    Private Sub InitPaddles(size As Size)
'        paddleHeight = size.Height / 8
'        paddleWidth = size.Width / 50

'        paddleLeft = New RectangleF(size.Width / 50.0F,
'                                    (size.Height - paddleHeight) / 2.0F,
'                                    paddleWidth,
'                                    paddleHeight)

'        paddleRight = New RectangleF(size.Width - size.Width / 50.0F - paddleWidth,
'                                     (size.Height - paddleHeight) / 2.0F,
'                                     paddleWidth,
'                                     paddleHeight)
'    End Sub

'    Private Sub InitTrails()
'        trailPoints = New PointF(trailLength - 1) {}
'        trailSizes = New Integer(trailLength - 1) {}
'        trailOffsets = New Single(trailLength - 1) {}
'        trailAlpha = New Integer(trailLength - 1) {}
'        trailBrushes = New SolidBrush(trailLength - 1) {}

'        For i As Integer = 0 To trailLength - 1
'            Dim size As Integer = ballDiameter - (trailLength - i) * 2
'            If size < 10 Then size = 10

'            trailSizes(i) = size
'            trailOffsets(i) = CSng((ballDiameter - size) / 2.0F)

'            Dim t As Double = i / CDbl(trailLength)
'            Dim alpha As Integer = CInt(16 * t * t)
'            trailAlpha(i) = alpha

'            trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
'        Next
'    End Sub

'    ' ============================================================
'    '   FONT / LAYOUT RESCALING
'    ' ============================================================
'    Public Sub RescaleFonts(g As Graphics, size As Size)
'        DisposeFonts()

'        hudScoreFont = New Font("Segoe UI", CSng(size.Height / 15.0F), FontStyle.Bold)
'        hudLabelFont = New Font("Segoe UI", CSng(size.Height / 60.0F), FontStyle.Regular)
'        pauseTitleFont = New Font("Segoe UI", CSng(size.Height / 18.0F), FontStyle.Bold)
'        pauseMenuFont = New Font("Segoe UI", CSng(size.Height / 28.0F), FontStyle.Regular)
'        startTitleFont = New Font("Segoe UI", CSng(size.Height / 12.0F), FontStyle.Bold)
'        startMenuFont = New Font("Segoe UI", CSng(size.Height / 30.0F), FontStyle.Regular)
'        startInfoFont = New Font("Segoe UI", CSng(size.Height / 45.0F), FontStyle.Regular)
'        aiDifficultyTitleFont = New Font("Segoe UI", CSng(size.Height / 18.0F), FontStyle.Bold)
'        gameOverFont = New Font("Segoe UI", CSng(size.Height / 20.0F), FontStyle.Bold)
'        gameOverInfoFont = New Font("Segoe UI", CSng(size.Height / 45.0F), FontStyle.Regular)



'        keyboardHintsFont = New Font("Segoe UI", CSng(size.Height / 80.0F), FontStyle.Regular)
'        fpsFont = New Font("Segoe UI", CSng(size.Height / 75.0F), FontStyle.Bold)

'        CachePauseLayout(g, size)
'        CacheHUDLayout(g, size)

'        RecomputeKeyboardHintLayout(g, size)
'        CacheAIDifficultyScreenLayout(g, size)

'        RescalePaddles(size)
'    End Sub

'    Private Sub CachePauseLayout(g As Graphics, size As Size)
'        pauseTitleSize = g.MeasureString(pauseTitle, pauseTitleFont)
'        pauseTitleX = (size.Width - pauseTitleSize.Width) / 2.0F
'        pauseTitleY = size.Height * 0.2F

'        pauseMenuSpacing = size.Height * 0.13F
'        pauseMenuStartY = pauseTitleY + pauseTitleSize.Height + (size.Height * 0.01F)

'        ReDim pauseMenuItemSizes(pauseMenuItems.Length - 1)
'        ReDim pauseMenuItemX(pauseMenuItems.Length - 1)
'        ReDim pauseMenuItemY(pauseMenuItems.Length - 1)

'        For i As Integer = 0 To pauseMenuItems.Length - 1
'            pauseMenuItemSizes(i) = g.MeasureString(pauseMenuItems(i), pauseMenuFont)
'            pauseMenuItemX(i) = (size.Width - pauseMenuItemSizes(i).Width) / 2.0F
'            pauseMenuItemY(i) = pauseMenuStartY + i * pauseMenuSpacing
'        Next
'    End Sub

'    Private Sub CacheHUDLayout(g As Graphics, size As Size)
'        Dim halfWidth As Single = size.Width / 2.0F

'        leftScoreSize = g.MeasureString(scoreLeft.ToString(), hudScoreFont)
'        rightScoreSize = g.MeasureString(scoreRight.ToString(), hudScoreFont)
'        leftLabelSize = g.MeasureString(leftPlayerName, hudLabelFont)
'        rightLabelSize = g.MeasureString(rightPlayerName, hudLabelFont)

'        scoreY = 10 + CSng(size.Height / 25.0F)
'        labelY = scoreY - CSng(size.Height / 200.0F)

'        leftScoreX = (halfWidth - leftScoreSize.Width) / 2.0F
'        rightScoreX = halfWidth + (halfWidth - rightScoreSize.Width) / 2.0F
'        leftLabelX = (halfWidth - leftLabelSize.Width) / 2.0F
'        rightLabelX = halfWidth + (halfWidth - rightLabelSize.Width) / 2.0F
'    End Sub


'    Private Sub CacheAIDifficultyScreenLayout(g As Graphics, size As Size)
'        ' ============================
'        ' Title
'        ' ============================
'        aiDifficultyTitleSize = g.MeasureString("Difficulty", aiDifficultyTitleFont)
'        aiDifficultyTitleX = (size.Width - aiDifficultyTitleSize.Width) / 2.0F
'        aiDifficultyTitleY = size.Height * 0.2F

'        ' ============================
'        ' Options
'        ' ============================
'        Dim baseY As Single = size.Height * 0.4F
'        Dim spacing As Single = size.Height * 0.03F

'        ReDim aiOptionSizes(aiOptions.Length - 1)
'        ReDim aiOptionX(aiOptions.Length - 1)
'        ReDim aiOptionY(aiOptions.Length - 1)

'        For i As Integer = 0 To aiOptions.Length - 1
'            aiOptionSizes(i) = g.MeasureString(aiOptions(i), startMenuFont)

'            aiOptionX(i) = (size.Width - aiOptionSizes(i).Width) / 2.0F
'            aiOptionY(i) = baseY + i * (aiOptionSizes(i).Height + spacing)

'            Dim rect As New Rectangle(
'            CInt(aiOptionX(i)),
'            CInt(aiOptionY(i)),
'            CInt(aiOptionSizes(i).Width),
'            CInt(aiOptionSizes(i).Height)
'        )

'            Select Case i
'                Case AIDifficultyLevel.Easy : aiEasyRect = rect
'                Case AIDifficultyLevel.Normal : aiNormalRect = rect
'                Case AIDifficultyLevel.Hard : aiHardRect = rect
'            End Select
'        Next

'        ' ============================
'        ' Blink Info ("Press SPACE to Start")
'        ' ============================
'        Dim info As String = "Press SPACE to Start"
'        aiInfoSize = g.MeasureString(info, startInfoFont)
'        aiInfoX = (size.Width - aiInfoSize.Width) / 2.0F
'        aiInfoY = size.Height * 0.8F
'    End Sub























'    Private Sub RescalePaddles(size As Size)
'        paddleHeight = size.Height / 8
'        paddleWidth = size.Height / 30

'        paddleLeft.Width = paddleWidth
'        paddleLeft.Height = paddleHeight
'        paddleRight.Width = paddleWidth
'        paddleRight.Height = paddleHeight

'        paddleLeft.X = size.Width / 50.0F
'        paddleRight.X = size.Width - size.Width / 50.0F - paddleWidth

'        paddleLeft.Y = (size.Height - paddleHeight) / 2.0F
'        paddleRight.Y = (size.Height - paddleHeight) / 2.0F
'    End Sub

'    Private Sub RecomputeKeyboardHintLayout(g As Graphics, size As Size)
'        hintLeftText = "W S - Move Paddle"
'        hintLeftSize = g.MeasureString(hintLeftText, keyboardHintsFont)

'        If numberOfPlayersSelection = 1 Then
'            hintRightText = "Arrows - Move Paddle"
'            pauseText = "P - Pause Match"
'        Else
'            hintRightText = "P - Pause Match"
'            pauseText = ""
'        End If

'        hintRightSize = g.MeasureString(hintRightText, keyboardHintsFont)
'        hintRightX = size.Width - hintRightSize.Width - 10

'        pauseSize = g.MeasureString(pauseText, keyboardHintsFont)
'        pauseY = size.Height - pauseSize.Height - 10

'        'fpsText = $"FPS: {fps}"

'        ' fps may be in the single digits use two digit size for layout
'        Dim fpsTwoDigitMeasureText As String = "FPS: 00"

'        fpsSize = g.MeasureString(fpsTwoDigitMeasureText, keyboardHintsFont)
'        fpsX = size.Width - fpsSize.Width - 10
'        fpsY = size.Height - fpsSize.Height - 10
'    End Sub

'    ' ============================================================
'    '   UPDATE METHODS
'    ' ============================================================
'    Public Sub UpdateFPS()
'        frameCount += 1

'        If fpsStopwatch.ElapsedMilliseconds >= 1000 Then
'            fps = frameCount
'            fpsText = $"FPS: {fps}"
'            frameCount = 0
'            fpsStopwatch.Restart()
'        End If
'    End Sub

'    Public Sub UpdateBallPosition(deltaTime As Double)
'        ballPos.X += CSng(velX * deltaTime)
'        ballPos.Y += CSng(velY * deltaTime)
'        UpdateTrail()
'    End Sub

'    Public Sub UpdateBallPosition(newPos As PointF)
'        ballPos = newPos
'        UpdateTrail()
'    End Sub

'    Public Sub UpdateBallVelocity(newVelX As Double, newVelY As Double)
'        velX = newVelX
'        velY = newVelY
'    End Sub

'    Public Sub UpdateBallDiameter(newDiameter As Integer)
'        ballDiameter = newDiameter
'        InitTrails()
'    End Sub

'    Public Sub UpdateBallBrush(newBrush As SolidBrush)
'        ballBrush?.Dispose()
'        ballBrush = newBrush
'    End Sub

'    Public Sub UpdateTrail()
'        If trailPoints Is Nothing Then Return

'        trailIndex = (trailIndex + 1) Mod trailLength
'        trailPoints(trailIndex) = ballPos

'        If trailCount < trailLength Then
'            trailCount += 1
'        End If

'        If trailCount = 1 Then
'            trailSizes(0) = ballDiameter
'            trailOffsets(0) = 0
'            trailAlpha(0) = 16
'            Return
'        End If

'        For i As Integer = 0 To trailCount - 1
'            Dim t As Double = 1.0 - (i / (trailCount - 1))
'            trailSizes(i) = CInt(ballDiameter * (0.5 + 0.5 * t))
'            trailOffsets(i) = CSng(ballDiameter * (0.25 * (1 - t)))
'            trailAlpha(i) = CInt(16 * (0.2 + 0.8 * t))
'        Next
'    End Sub

'    Public Sub ClearTrail()
'        trailCount = 0
'        trailIndex = -1
'        For i As Integer = 0 To trailLength - 1
'            trailSizes(i) = 0
'            trailOffsets(i) = 0
'            trailAlpha(i) = 0
'        Next
'    End Sub

'    Public Sub UpdateScore(leftScore As Integer, rightScore As Integer, g As Graphics, size As Size)
'        Me.scoreLeft = leftScore
'        Me.scoreRight = rightScore
'        CacheHUDLayout(g, size)
'    End Sub

'    Public Sub UpdatePlayerNames(leftName As String, rightName As String, g As Graphics, size As Size)
'        Me.leftPlayerName = leftName
'        Me.rightPlayerName = rightName
'        CacheHUDLayout(g, size)
'    End Sub

'    Public Sub UpdatePaddlePositions(leftY As Single, rightY As Single, size As Size)
'        paddleLeft.Y = Math.Max(0, Math.Min(size.Height - paddleHeight, leftY))
'        paddleRight.Y = Math.Max(0, Math.Min(size.Height - paddleHeight, rightY))
'    End Sub

'    Public Sub ToggleKeyboardHints()
'        showKeyboardHints = Not showKeyboardHints
'    End Sub

'    Public Sub SetPlayerMode(mode As Integer, g As Graphics, size As Size)
'        numberOfPlayersSelection = If(mode = 2, 1, 0)
'        RecomputeKeyboardHintLayout(g, size)
'    End Sub

'    Public Sub SetWinnerText(text As String)
'        winnerText = text
'    End Sub

'    Public Sub SetGameState(state As GameState)
'        currentState = state
'    End Sub

'    Public Sub SetAIDifficultySelection(sel As Integer)
'        aiDifficultySelection = sel
'    End Sub

'    Public Sub SetStartMenuSelection(sel As Integer)
'        numberOfPlayersSelection = sel
'    End Sub

'    Public Sub SetPauseMenuSelection(sel As Integer)
'        pauseMenuSelection = sel
'    End Sub

'    ' ============================================================
'    '   RENDERING PIPELINE
'    ' ============================================================
'    Public Sub Render(g As Graphics, state As GameState, showHints As Boolean)
'        Select Case state

'            Case GameState.StartScreen
'                DrawTrail(g)
'                DrawBall(g)
'                DrawStartScreen(g)
'                If showHints Then DrawKeyboardHintsStartScreen(g)

'            Case GameState.Playing
'                DrawGamePlayScreen(g)
'                If showHints Then DrawKeyboardHintsGamePlayScreen(g)

'            Case GameState.Pause
'                DrawTrail(g)
'                DrawBall(g)
'                DrawPaddles(g)
'                DrawHUD(g)
'                DrawPauseScreen(g)
'                If showHints Then DrawKeyboardHintsPauseScreen(g)

'            Case GameState.EndScreen
'                DrawTrail(g)
'                DrawBall(g)
'                DrawHUD(g)
'                DrawGameOver(g)
'                If showHints Then DrawKeyboardHintsGameOverScreen(g)

'            Case GameState.AIDifficulty
'                DrawTrail(g)
'                DrawBall(g)
'                DrawAIDifficultyScreen(g)
'                If showHints Then DrawKeyboardHintsAIDifficultyScreen(g)

'        End Select
'    End Sub

'    ' ============================================================
'    '   CORE DRAW METHODS (missing from your file)
'    ' ============================================================

'    Private Sub DrawTrail(g As Graphics)
'        If trailCount <= 0 Then Return

'        For i As Integer = 0 To trailCount - 1
'            Dim index As Integer = (trailIndex - i + trailLength) Mod trailLength
'            Dim p As PointF = trailPoints(index)
'            Dim offset As Single = trailOffsets(i)

'            Dim alpha As Integer = trailAlpha(i)
'            If alpha <= 0 Then Continue For

'            Dim baseColor As Color = Color.FromArgb(alpha, 0, 191, 255)
'            trailBrushes(i).Color = baseColor

'            g.FillEllipse(trailBrushes(i),
'                      p.X + offset,
'                      p.Y + offset,
'                      trailSizes(i),
'                      trailSizes(i))
'        Next
'    End Sub

'    Private Sub DrawBall(g As Graphics)
'        g.FillEllipse(ballBrush, ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
'    End Sub

'    Private Sub DrawPaddles(g As Graphics)
'        g.FillRectangle(paddleBrush, paddleLeft)
'        g.FillRectangle(paddleBrush, paddleRight)
'    End Sub

'    Private Sub DrawHUD(g As Graphics)
'        g.DrawString(leftPlayerName, hudLabelFont, playerLabelBrush, leftLabelX, labelY)
'        g.DrawString(rightPlayerName, hudLabelFont, playerLabelBrush, rightLabelX, labelY)
'        g.DrawString(scoreLeft.ToString(), hudScoreFont, scoreBrush, leftScoreX, scoreY)
'        g.DrawString(scoreRight.ToString(), hudScoreFont, scoreBrush, rightScoreX, scoreY)
'    End Sub


'    ' ============================================================
'    '   SCREEN RENDERERS
'    ' ============================================================
'    Private Sub DrawGamePlayScreen(g As Graphics)
'        DrawTrail(g)
'        DrawBall(g)
'        DrawPaddles(g)
'        DrawHUD(g)
'    End Sub

'    Private Sub DrawStartScreen(g As Graphics)
'        Dim title As String = "PONG"
'        Dim titleSize = g.MeasureString(title, startTitleFont)
'        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

'        Using titleBrush As New SolidBrush(titleColor)
'            g.DrawString(title, startTitleFont, titleBrush,
'                         CSng((clientSize.Width - titleSize.Width) / 2.0F),
'                         CSng(clientSize.Height * 0.15F))
'        End Using

'        Dim option1 As String = "1 Player"
'        Dim option2 As String = "2 Players"

'        Dim opt1Size = g.MeasureString(option1, startMenuFont)
'        Dim opt2Size = g.MeasureString(option2, startMenuFont)

'        Dim opt1X As Single = CSng((clientSize.Width - opt1Size.Width) / 2.0F)
'        Dim opt1Y As Single = CSng(clientSize.Height * 0.4F)
'        Dim opt2X As Single = CSng((clientSize.Width - opt2Size.Width) / 2.0F)
'        Dim opt2Y As Single = CSng(clientSize.Height * 0.55F)

'        onePlayerOptionRect = New Rectangle(CInt(opt1X), CInt(opt1Y),
'                                            CInt(opt1Size.Width), CInt(opt1Size.Height))
'        twoPlayersOptionRect = New Rectangle(CInt(opt2X), CInt(opt2Y),
'                                             CInt(opt2Size.Width), CInt(opt2Size.Height))

'        If numberOfPlayersSelection = 0 Then
'            FillRoundedRectangle(g, lightBrush, onePlayerOptionRect, clientSize.Height / 64)
'            DrawRoundedRectangle(g, outlinePen, onePlayerOptionRect, clientSize.Height / 64)
'        Else
'            FillRoundedRectangle(g, darkBrush, onePlayerOptionRect, clientSize.Height / 64)
'            DrawRoundedRectangle(g, darkOutlinePen, onePlayerOptionRect, clientSize.Height / 64)
'        End If

'        If numberOfPlayersSelection = 1 Then
'            FillRoundedRectangle(g, lightBrush, twoPlayersOptionRect, clientSize.Height / 64)
'            DrawRoundedRectangle(g, outlinePen, twoPlayersOptionRect, clientSize.Height / 64)
'        Else
'            FillRoundedRectangle(g, darkBrush, twoPlayersOptionRect, clientSize.Height / 64)
'            DrawRoundedRectangle(g, darkOutlinePen, twoPlayersOptionRect, clientSize.Height / 64)
'        End If

'        Dim opt1Brush As SolidBrush = If(numberOfPlayersSelection = 0, whiteBrush, grayBrush)
'        Dim opt2Brush As SolidBrush = If(numberOfPlayersSelection = 1, whiteBrush, grayBrush)

'        g.DrawString(option1, startMenuFont, opt1Brush, opt1X, opt1Y)
'        g.DrawString(option2, startMenuFont, opt2Brush, opt2X, opt2Y)

'        If blinkVisible Then
'            Dim info As String = "Press SPACE to Start"
'            Dim infoSize = g.MeasureString(info, startInfoFont)

'            g.DrawString(info, startInfoFont, whiteBrush,
'                         CSng((clientSize.Width - infoSize.Width) / 2.0F),
'                         CSng(clientSize.Height * 0.75F))
'        End If
'    End Sub



'    Public Sub UpdateStartScreenFX()
'        If titleFadeIn Then
'            titleAlpha += 3
'            If titleAlpha >= 255 Then
'                titleAlpha = 255
'                titleFadeIn = False
'            End If
'        Else
'            titleAlpha -= 3
'            If titleAlpha <= 80 Then
'                titleAlpha = 80
'                titleFadeIn = True
'            End If
'        End If

'        If blinkStopwatch.ElapsedMilliseconds >= 800 Then
'            blinkVisible = Not blinkVisible
'            blinkStopwatch.Restart()
'        End If
'    End Sub


'    Private Sub DrawPauseScreen(g As Graphics)
'        g.FillRectangle(dimBrush, ClientRectangle)

'        g.DrawString(pauseTitle, pauseTitleFont, whiteBrush, pauseTitleX, pauseTitleY)

'        For i As Integer = 0 To pauseMenuItems.Length - 1
'            Dim text = pauseMenuItems(i)
'            Dim x = pauseMenuItemX(i)
'            Dim y = pauseMenuItemY(i)
'            Dim size = pauseMenuItemSizes(i)

'            Dim rect As New Rectangle(CInt(x), CInt(y), CInt(size.Width), CInt(size.Height))

'            Select Case i
'                Case 0 : pauseResumeRect = rect
'                Case 1 : pauseNewMatchRect = rect
'                Case 2 : pauseQuitRect = rect
'            End Select

'            If i = pauseMenuSelection Then
'                FillRoundedRectangle(g, lightBrush, rect, clientSize.Height / 64)
'                DrawRoundedRectangle(g, outlinePen, rect, clientSize.Height / 64)
'            Else
'                FillRoundedRectangle(g, darkBrush, rect, clientSize.Height / 64)
'                DrawRoundedRectangle(g, darkOutlinePen, rect, clientSize.Height / 64)
'            End If

'            Dim brush As SolidBrush = If(i = pauseMenuSelection, whiteBrush, grayBrush)
'            g.DrawString(text, pauseMenuFont, brush, x, y)
'        Next
'    End Sub

'    'Private Sub DrawAIDifficultyScreen(g As Graphics)
'    '    Dim title As String = "Difficulty"
'    '    Dim titleSize = g.MeasureString(title, aiDifficultyTitleFont)
'    '    Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

'    '    Using titleBrush As New SolidBrush(titleColor)
'    '        g.DrawString(title, aiDifficultyTitleFont, titleBrush,
'    '                     CSng((clientSize.Width - titleSize.Width) / 2.0F),
'    '                     CSng(clientSize.Height * 0.2F))
'    '    End Using

'    '    Dim baseY As Single = CSng(clientSize.Height * 0.41F)

'    '    For i As Integer = 0 To aiOptions.Length - 1
'    '        Dim text = aiOptions(i)
'    '        Dim size = g.MeasureString(text, startMenuFont)
'    '        Dim x = CSng((clientSize.Width - size.Width) / 2.0F)
'    '        Dim y = baseY + i * (size.Height + clientSize.Height * 0.03F)

'    '        Dim rect As New Rectangle(CInt(x), CInt(y), CInt(size.Width), CInt(size.Height))

'    '        Select Case i
'    '            Case AIDifficultyLevel.Easy : aiEasyRect = rect
'    '            Case AIDifficultyLevel.Normal : aiNormalRect = rect
'    '            Case AIDifficultyLevel.Hard : aiHardRect = rect
'    '        End Select

'    '        If i = aiDifficultySelection Then
'    '            FillRoundedRectangle(g, lightBrush, rect, clientSize.Height / 64)
'    '            DrawRoundedRectangle(g, outlinePen, rect, clientSize.Height / 64)
'    '        Else
'    '            FillRoundedRectangle(g, darkBrush, rect, clientSize.Height / 64)
'    '            DrawRoundedRectangle(g, darkOutlinePen, rect, clientSize.Height / 64)
'    '        End If

'    '        Dim brush As SolidBrush = If(i = aiDifficultySelection, whiteBrush, grayBrush)
'    '        g.DrawString(text, startMenuFont, brush, x, y)
'    '    Next

'    '    If blinkVisible Then
'    '        Dim info As String = "Press SPACE to Start"
'    '        Dim infoSize = g.MeasureString(info, startInfoFont)

'    '        g.DrawString(info, startInfoFont, whiteBrush,
'    '                     CSng((clientSize.Width - infoSize.Width) / 2.0F),
'    '                     CSng(clientSize.Height * 0.75F))
'    '    End If
'    'End Sub


'    Private Sub DrawAIDifficultyScreen(g As Graphics)

'        'If aiOptionSizes Is Nothing OrElse aiOptionX Is Nothing Then Exit Sub


'        If aiOptionSizes Is Nothing _
'        OrElse aiOptionX Is Nothing _
'        OrElse aiOptionY Is Nothing _
'        OrElse aiDifficultyTitleFont Is Nothing _
'        OrElse startMenuFont Is Nothing _
'        OrElse startInfoFont Is Nothing Then

'            Exit Sub
'        End If




'        ' ============================
'        ' Title
'        ' ============================
'        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)
'        Using titleBrush As New SolidBrush(titleColor)
'            g.DrawString("Difficulty", aiDifficultyTitleFont, titleBrush,
'                     aiDifficultyTitleX, aiDifficultyTitleY)
'        End Using

'        ' ============================
'        ' Options
'        ' ============================
'        For i As Integer = 0 To aiOptions.Length - 1
'            Dim rect As Rectangle

'            Select Case i
'                Case AIDifficultyLevel.Easy : rect = aiEasyRect
'                Case AIDifficultyLevel.Normal : rect = aiNormalRect
'                Case AIDifficultyLevel.Hard : rect = aiHardRect
'            End Select

'            Dim radius As Integer = clientSize.Height \ 64

'            If i = aiDifficultySelection Then
'                FillRoundedRectangle(g, lightBrush, rect, radius)
'                DrawRoundedRectangle(g, outlinePen, rect, radius)
'            Else
'                FillRoundedRectangle(g, darkBrush, rect, radius)
'                DrawRoundedRectangle(g, darkOutlinePen, rect, radius)
'            End If

'            Dim brush As SolidBrush =
'            If(i = aiDifficultySelection, whiteBrush, grayBrush)

'            g.DrawString(aiOptions(i), startMenuFont, brush,
'                     aiOptionX(i), aiOptionY(i))
'        Next

'        ' ============================
'        ' Blink Info
'        ' ============================
'        If blinkVisible Then
'            g.DrawString("Press SPACE to Start", startInfoFont, whiteBrush,
'                     aiInfoX, aiInfoY)
'        End If
'    End Sub





























'    Private Sub DrawGameOver(g As Graphics)
'        Dim titleSize = g.MeasureString(winnerText, gameOverFont)
'        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

'        Using titleBrush As New SolidBrush(titleColor)
'            g.DrawString(winnerText, gameOverFont, titleBrush,
'                         CSng((clientSize.Width - titleSize.Width) / 2.0F),
'                         CSng(clientSize.Height * 0.35F))
'        End Using

'        If blinkVisible Then
'            Dim info As String = "Press SPACE to Start"
'            Dim infoSize = g.MeasureString(info, gameOverInfoFont)

'            g.DrawString(info, gameOverInfoFont, whiteBrush,
'                         CSng((clientSize.Width - infoSize.Width) / 2.0F),
'                         CSng(clientSize.Height * 0.6F))
'        End If
'    End Sub

'    ' ============================================================
'    '   KEYBOARD HINT RENDERERS
'    ' ============================================================
'    Private Sub DrawKeyboardHintsStartScreen(g As Graphics)
'        Dim hintText As String = "1 - One Player   2 - Two Players   Enter - Start Match"
'        g.DrawString(hintText, keyboardHintsFont, grayBrush, 10, 10)

'        Dim fsText As String =
'            If(formBorderStyle = FormBorderStyle.None,
'               "F - Exit Fullscreen",
'               "F - Fullscreen")

'        Dim fsSize = g.MeasureString(fsText, keyboardHintsFont)

'        g.DrawString(fsText, keyboardHintsFont, grayBrush,
'                     clientSize.Width - fsSize.Width - 10, 10)

'        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
'        Dim hideSize = g.MeasureString(hideText, keyboardHintsFont)

'        g.DrawString(hideText, keyboardHintsFont, grayBrush,
'                     10, clientSize.Height - hideSize.Height - 10)
'    End Sub

'    Private Sub DrawKeyboardHintsPauseScreen(g As Graphics)
'        Dim hintText As String = "R - Resume Match   N - New Match"
'        g.DrawString(hintText, keyboardHintsFont, grayBrush, 10, 10)

'        Dim fsText As String =
'            If(formBorderStyle = FormBorderStyle.None,
'               "F - Exit Fullscreen",
'               "F - Fullscreen")

'        Dim fsSize = g.MeasureString(fsText, keyboardHintsFont)

'        g.DrawString(fsText, keyboardHintsFont, grayBrush,
'                     clientSize.Width - fsSize.Width - 10, 10)

'        Dim quitText As String = "Q - Quit Match"
'        Dim quitSize = g.MeasureString(quitText, keyboardHintsFont)

'        g.DrawString(quitText, keyboardHintsFont, grayBrush,
'                     10, clientSize.Height - quitSize.Height - 10)

'        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
'        Dim hideSize = g.MeasureString(hideText, keyboardHintsFont)

'        g.DrawString(hideText, keyboardHintsFont, grayBrush,
'                     clientSize.Width - hideSize.Width - 10,
'                     clientSize.Height - hideSize.Height - 10)
'    End Sub

'    Private Sub DrawKeyboardHintsGamePlayScreen(g As Graphics)
'        g.DrawString(hintLeftText, keyboardHintsFont, fullscreenIndicatorBrush, hintLeftX, hintLeftY)
'        g.DrawString(hintRightText, keyboardHintsFont, fullscreenIndicatorBrush, hintRightX, hintRightY)

'        If Not String.IsNullOrEmpty(pauseText) Then
'            g.DrawString(pauseText, keyboardHintsFont, fullscreenIndicatorBrush, pauseX, pauseY)
'        End If

'        g.DrawString(fpsText, keyboardHintsFont, fullscreenIndicatorBrush, fpsX, fpsY)
'    End Sub

'    Private Sub DrawKeyboardHintsAIDifficultyScreen(g As Graphics)
'        Dim hintText As String = "E - Easy   N - Normal   H - Hard   Enter - Start Match"
'        g.DrawString(hintText, keyboardHintsFont, grayBrush, 10, 10)

'        Dim fsText As String =
'            If(formBorderStyle = FormBorderStyle.None,
'               "F - Exit Fullscreen",
'               "F - Fullscreen")

'        Dim fsSize = g.MeasureString(fsText, keyboardHintsFont)

'        g.DrawString(fsText, keyboardHintsFont, grayBrush,
'                     clientSize.Width - fsSize.Width - 10, 10)

'        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
'        Dim hideSize = g.MeasureString(hideText, keyboardHintsFont)

'        g.DrawString(hideText, keyboardHintsFont, grayBrush,
'                     10, clientSize.Height - hideSize.Height - 10)
'    End Sub

'    Private Sub DrawKeyboardHintsGameOverScreen(g As Graphics)
'        Dim hintText As String = "Enter - Start New Match"
'        g.DrawString(hintText, keyboardHintsFont, grayBrush, 10, 10)

'        Dim fsText As String =
'            If(formBorderStyle = FormBorderStyle.None,
'               "F - Exit Fullscreen",
'               "F - Fullscreen")

'        Dim fsSize = g.MeasureString(fsText, keyboardHintsFont)

'        g.DrawString(fsText, keyboardHintsFont, grayBrush,
'                     clientSize.Width - fsSize.Width - 10, 10)

'        Dim quitText As String = "CTRL Q - Quit Game"
'        Dim quitSize = g.MeasureString(quitText, keyboardHintsFont)

'        g.DrawString(quitText, keyboardHintsFont, grayBrush,
'                     10, clientSize.Height - quitSize.Height - 10)
'    End Sub

'    ' ============================================================
'    '   ROUNDED RECTANGLE HELPERS
'    ' ============================================================
'    Private Function RoundedRect(rect As Rectangle, radius As Integer) As GraphicsPath
'        Dim path As New GraphicsPath()
'        Dim d As Integer = radius * 2

'        path.AddArc(rect.X, rect.Y, d, d, 180, 90)
'        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
'        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
'        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
'        path.CloseFigure()

'        Return path
'    End Function

'    Private Sub FillRoundedRectangle(g As Graphics, brush As Brush, rect As Rectangle, radius As Integer)
'        Using path As GraphicsPath = RoundedRect(rect, radius)
'            g.FillPath(brush, path)
'        End Using
'    End Sub

'    Private Sub DrawRoundedRectangle(g As Graphics, pen As Pen, rect As Rectangle, radius As Integer)
'        Using path As GraphicsPath = RoundedRect(rect, radius)
'            g.DrawPath(pen, path)
'        End Using
'    End Sub

'    ' ============================================================
'    '   CLIENT RECTANGLE
'    ' ============================================================
'    Private ReadOnly Property ClientRectangle As Rectangle
'        Get
'            Return New Rectangle(0, 0, clientSize.Width, clientSize.Height)
'        End Get
'    End Property

'    ' ============================================================
'    '   DISPOSAL
'    ' ============================================================
'    Public Sub Dispose() Implements IDisposable.Dispose
'        DisposeFonts()

'        ballBrush?.Dispose()
'        paddleBrush?.Dispose()
'        scoreBrush?.Dispose()
'        playerLabelBrush?.Dispose()
'        whiteBrush?.Dispose()
'        grayBrush?.Dispose()
'        dimBrush?.Dispose()
'        fullscreenIndicatorBrush?.Dispose()

'        For Each b In trailBrushes
'            b?.Dispose()
'        Next

'        outlinePen?.Dispose()
'        darkOutlinePen?.Dispose()
'    End Sub

'    Private Sub DisposeFonts()
'        hudScoreFont?.Dispose()
'        hudLabelFont?.Dispose()
'        pauseTitleFont?.Dispose()
'        pauseMenuFont?.Dispose()
'        startTitleFont?.Dispose()
'        startMenuFont?.Dispose()
'        startInfoFont?.Dispose()
'        aiDifficultyTitleFont?.Dispose()
'        gameOverFont?.Dispose()
'        gameOverInfoFont?.Dispose()
'        keyboardHintsFont?.Dispose()
'        fpsFont?.Dispose()
'    End Sub

'End Class







Imports System.Drawing
    Imports System.Drawing.Drawing2D
    Imports System.Diagnostics

    Public Class Rendering
        Implements IDisposable

        ' ============================================================
        '   PUBLIC ENUMS
        ' ============================================================
        Public Enum GameState
            StartScreen
            Playing
            Pause
            EndScreen
            AIDifficulty
        End Enum

        ' ============================================================
        '   CORE STATE
        ' ============================================================
        Private currentState As GameState = GameState.StartScreen
        Private formBorderStyle As FormBorderStyle
        Private clientSize As Size

        ' ============================================================
        '   BALL / PHYSICS
        ' ============================================================
        Private ballPos As PointF
        Private ballDiameter As Integer
        Private velX As Double
        Private velY As Double

        ' ============================================================
        '   FPS
        ' ============================================================
        Private frameCount As Integer
        Private fps As Integer
        Private fpsStopwatch As New Stopwatch()

        ' ============================================================
        '   GDI RESOURCES
        ' ============================================================
        Private ballBrush As SolidBrush
        Private paddleBrush As SolidBrush
        Private scoreBrush As SolidBrush
        Private labelBrush As SolidBrush
        Private whiteBrush As SolidBrush
        Private grayBrush As SolidBrush
        Private dimBrush As SolidBrush
        Private fsIndicatorBrush As SolidBrush

        Private hudScoreFont As Font
        Private hudLabelFont As Font
        Private pauseTitleFont As Font
        Private pauseMenuFont As Font
        Private startTitleFont As Font
        Private startMenuFont As Font
        Private startInfoFont As Font
        Private aiTitleFont As Font
        Private gameOverFont As Font
        Private gameOverInfoFont As Font
        Private keyboardFont As Font
        Private fpsFont As Font

        ' ============================================================
        '   TRAIL SYSTEM
        ' ============================================================
        Private Const TrailLength As Integer = 20
        Private trailPoints(TrailLength - 1) As PointF
        Private trailSizes(TrailLength - 1) As Integer
        Private trailOffsets(TrailLength - 1) As Single
        Private trailAlpha(TrailLength - 1) As Integer
        Private trailBrushes(TrailLength - 1) As SolidBrush
        Private trailIndex As Integer = -1
        Private trailCount As Integer = 0

        ' ============================================================
        '   PADDLES
        ' ============================================================
        Private paddleLeft As RectangleF
        Private paddleRight As RectangleF
        Private paddleWidth As Integer
        Private paddleHeight As Integer

        ' ============================================================
        '   PLAYER NAMES / SCORES
        ' ============================================================
        Private leftPlayerName As String = "Left"
        Private rightPlayerName As String = "Right"
        Private scoreLeft As Integer = 0
        Private scoreRight As Integer = 0

        ' ============================================================
        '   HUD LAYOUT CACHE
        ' ============================================================
        Private leftScoreX As Single
        Private rightScoreX As Single
        Private leftLabelX As Single
        Private rightLabelX As Single
        Private scoreY As Single
        Private labelY As Single

        Private leftScoreSize As SizeF
        Private rightScoreSize As SizeF
        Private leftLabelSize As SizeF
        Private rightLabelSize As SizeF

        ' ============================================================
        '   KEYBOARD HINTS
        ' ============================================================
        Private showKeyboardHints As Boolean = True
        Private hintLeftText As String
        Private hintRightText As String
        Private pauseText As String
        Private fpsText As String

        Private hintLeftSize As SizeF
        Private hintRightSize As SizeF
        Private pauseSize As SizeF
        Private fpsSize As SizeF

        Private hintLeftX As Single = 10
        Private hintLeftY As Single = 10
        Private hintRightX As Single
        Private hintRightY As Single = 10
        Private pauseX As Single = 10
        Private pauseY As Single
        Private fpsX As Single
        Private fpsY As Single

        ' ============================================================
        '   START SCREEN
        ' ============================================================
        Private numberOfPlayersSelection As Integer = 0
        Private onePlayerRect As Rectangle
        Private twoPlayersRect As Rectangle

        Private titleAlpha As Integer = 255
        Private titleFadeIn As Boolean = True
        Private blinkVisible As Boolean = True
        Private blinkStopwatch As New Stopwatch()

        ' ============================================================
        '   AI DIFFICULTY SCREEN
        ' ============================================================
        Private aiDifficultySelection As Integer = 0
        Private aiOptions() As String = {"Easy", "Normal", "Hard"}

        Private aiEasyRect As Rectangle
        Private aiNormalRect As Rectangle
        Private aiHardRect As Rectangle

        Private aiTitleSize As SizeF
        Private aiTitleX As Single
        Private aiTitleY As Single

        Private aiOptionSizes() As SizeF
        Private aiOptionX() As Single
        Private aiOptionY() As Single

        Private aiInfoSize As SizeF
        Private aiInfoX As Single
        Private aiInfoY As Single

        ' ============================================================
        '   PAUSE SCREEN
        ' ============================================================
        Private pauseTitle As String = "PAUSED"
        Private pauseTitleSize As SizeF
        Private pauseTitleX As Single
        Private pauseTitleY As Single

        Private pauseMenuItems() As String = {"Resume", "New", "Quit"}
        Private pauseMenuItemSizes() As SizeF
        Private pauseMenuItemX() As Single
        Private pauseMenuItemY() As Single
        Private pauseMenuStartY As Single
        Private pauseMenuSpacing As Single

        Private pauseMenuSelection As Integer = 0
        Private pauseResumeRect As Rectangle
        Private pauseNewRect As Rectangle
        Private pauseQuitRect As Rectangle

        ' ============================================================
        '   GAME OVER SCREEN
        ' ============================================================
        Private winnerText As String = ""

        ' ============================================================
        '   UI BRUSHES / PENS
        ' ============================================================
        Private lightBrush As New SolidBrush(Color.FromArgb(32, 255, 255, 255))
        Private darkBrush As New SolidBrush(Color.FromArgb(64, 0, 0, 0))
        Private outlinePen As New Pen(Color.FromArgb(40, 255, 255, 255), 2)
        Private darkOutlinePen As New Pen(Color.FromArgb(32, 255, 255, 255), 2)

        ' ============================================================
        '   PUBLIC RECT ACCESSORS
        ' ============================================================
        Public Function OnePlayerOptionRect() As Rectangle
            Return onePlayerRect
        End Function

        Public Function TwoPlayersOptionRect() As Rectangle
            Return twoPlayersRect
        End Function

        Public Function EasyRect() As Rectangle
            Return aiEasyRect
        End Function

        Public Function NormalRect() As Rectangle
            Return aiNormalRect
        End Function

        Public Function HardRect() As Rectangle
            Return aiHardRect
        End Function

        Public Function ResumeRect() As Rectangle
            Return pauseResumeRect
        End Function

        Public Function NewMatchRect() As Rectangle
            Return pauseNewRect
        End Function

        Public Function QuitRect() As Rectangle
            Return pauseQuitRect
        End Function

        ' ============================================================
        '   CONSTRUCTOR
        ' ============================================================
        Public Sub New(g As Graphics, size As Size)
            clientSize = size
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            ScaleBall(size)
            InitGraphics(g, size)
            InitTrails()
            InitPaddles(size)
            CacheHUD(g, size)
            CachePause(g, size)
            CacheKeyboardHints(g, size)
            CacheAIDifficulty(g, size)

            fpsStopwatch.Start()
            blinkStopwatch.Start()
        End Sub

        ' ============================================================
        '   FORM STATE UPDATE
        ' ============================================================
        Public Sub UpdateFormState(style As FormBorderStyle, size As Size)
            formBorderStyle = style
            clientSize = size
        End Sub

        ' ============================================================
        '   INITIALIZATION
        ' ============================================================
        Private Sub ScaleBall(size As Size)
            ballDiameter = CInt(Math.Min(size.Width, size.Height) / 18.0F)
        End Sub

    Public Sub InitGraphics(g As Graphics, size As Size)
        ballBrush = New SolidBrush(Color.DeepSkyBlue)
        paddleBrush = New SolidBrush(Color.White)
        scoreBrush = New SolidBrush(Color.White)
        labelBrush = New SolidBrush(Color.Gray)
        whiteBrush = New SolidBrush(Color.White)
        grayBrush = New SolidBrush(Color.FromArgb(140, 140, 140))
        dimBrush = New SolidBrush(Color.FromArgb(120, 0, 0, 0))
        fsIndicatorBrush = New SolidBrush(Color.FromArgb(120, 255, 255, 255))

        RescaleFonts(g, size)
    End Sub

    Private Sub InitPaddles(size As Size)
            paddleHeight = size.Height \ 8
            paddleWidth = size.Width \ 50

            paddleLeft = New RectangleF(size.Width / 50.0F,
                                    (size.Height - paddleHeight) / 2.0F,
                                    paddleWidth,
                                    paddleHeight)

            paddleRight = New RectangleF(size.Width - size.Width / 50.0F - paddleWidth,
                                     (size.Height - paddleHeight) / 2.0F,
                                     paddleWidth,
                                     paddleHeight)
        End Sub

        Private Sub InitTrails()
            For i As Integer = 0 To TrailLength - 1
                Dim size As Integer = ballDiameter - (TrailLength - i) * 2
                If size < 10 Then size = 10

                trailSizes(i) = size
                trailOffsets(i) = CSng((ballDiameter - size) / 2.0F)

                Dim t As Double = i / CDbl(TrailLength)
                Dim alpha As Integer = CInt(16 * t * t)
                trailAlpha(i) = alpha

                trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
            Next
        End Sub

        ' ============================================================
        '   FONT / LAYOUT RESCALING
        ' ============================================================
        Public Sub RescaleFonts(g As Graphics, size As Size)
            DisposeFonts()

            hudScoreFont = New Font("Segoe UI", CSng(size.Height / 15.0F), FontStyle.Bold)
            hudLabelFont = New Font("Segoe UI", CSng(size.Height / 60.0F))
            pauseTitleFont = New Font("Segoe UI", CSng(size.Height / 18.0F), FontStyle.Bold)
            pauseMenuFont = New Font("Segoe UI", CSng(size.Height / 28.0F))
            startTitleFont = New Font("Segoe UI", CSng(size.Height / 12.0F), FontStyle.Bold)
            startMenuFont = New Font("Segoe UI", CSng(size.Height / 30.0F))
            startInfoFont = New Font("Segoe UI", CSng(size.Height / 45.0F))
            aiTitleFont = New Font("Segoe UI", CSng(size.Height / 18.0F), FontStyle.Bold)
            gameOverFont = New Font("Segoe UI", CSng(size.Height / 20.0F), FontStyle.Bold)
            gameOverInfoFont = New Font("Segoe UI", CSng(size.Height / 45.0F))
            keyboardFont = New Font("Segoe UI", CSng(size.Height / 80.0F))
            fpsFont = New Font("Segoe UI", CSng(size.Height / 75.0F), FontStyle.Bold)

            CachePause(g, size)
            CacheHUD(g, size)
            CacheKeyboardHints(g, size)
            CacheAIDifficulty(g, size)
            RescalePaddles(size)
        End Sub

        Private Sub RescalePaddles(size As Size)
            paddleHeight = size.Height \ 8
            paddleWidth = size.Height \ 30

            paddleLeft.Width = paddleWidth
            paddleLeft.Height = paddleHeight
            paddleRight.Width = paddleWidth
            paddleRight.Height = paddleHeight

            paddleLeft.X = size.Width / 50.0F
            paddleRight.X = size.Width - size.Width / 50.0F - paddleWidth

            paddleLeft.Y = (size.Height - paddleHeight) / 2.0F
            paddleRight.Y = (size.Height - paddleHeight) / 2.0F
        End Sub

        ' ============================================================
        '   HUD LAYOUT
        ' ============================================================
        Private Sub CacheHUD(g As Graphics, size As Size)
            Dim halfWidth As Single = size.Width / 2.0F

            leftScoreSize = g.MeasureString(scoreLeft.ToString(), hudScoreFont)
            rightScoreSize = g.MeasureString(scoreRight.ToString(), hudScoreFont)
            leftLabelSize = g.MeasureString(leftPlayerName, hudLabelFont)
            rightLabelSize = g.MeasureString(rightPlayerName, hudLabelFont)

            scoreY = 10 + CSng(size.Height / 25.0F)
            labelY = scoreY - CSng(size.Height / 200.0F)

            leftScoreX = (halfWidth - leftScoreSize.Width) / 2.0F
            rightScoreX = halfWidth + (halfWidth - rightScoreSize.Width) / 2.0F
            leftLabelX = (halfWidth - leftLabelSize.Width) / 2.0F
            rightLabelX = halfWidth + (halfWidth - rightLabelSize.Width) / 2.0F
        End Sub

        ' ============================================================
        '   PAUSE LAYOUT
        ' ============================================================
        Private Sub CachePause(g As Graphics, size As Size)
            pauseTitleSize = g.MeasureString(pauseTitle, pauseTitleFont)
            pauseTitleX = (size.Width - pauseTitleSize.Width) / 2.0F
            pauseTitleY = size.Height * 0.2F

            pauseMenuSpacing = size.Height * 0.13F
            pauseMenuStartY = pauseTitleY + pauseTitleSize.Height + (size.Height * 0.01F)

            ReDim pauseMenuItemSizes(pauseMenuItems.Length - 1)
            ReDim pauseMenuItemX(pauseMenuItems.Length - 1)
            ReDim pauseMenuItemY(pauseMenuItems.Length - 1)

            For i As Integer = 0 To pauseMenuItems.Length - 1
                pauseMenuItemSizes(i) = g.MeasureString(pauseMenuItems(i), pauseMenuFont)
                pauseMenuItemX(i) = (size.Width - pauseMenuItemSizes(i).Width) / 2.0F
                pauseMenuItemY(i) = pauseMenuStartY + i * pauseMenuSpacing
            Next
        End Sub

        ' ============================================================
        '   KEYBOARD HINT LAYOUT
        ' ============================================================
        Private Sub CacheKeyboardHints(g As Graphics, size As Size)
            hintLeftText = "W S - Move Paddle"
            hintLeftSize = g.MeasureString(hintLeftText, keyboardFont)

            If numberOfPlayersSelection = 1 Then
                hintRightText = "Arrows - Move Paddle"
                pauseText = "P - Pause Match"
            Else
                hintRightText = "P - Pause Match"
                pauseText = ""
            End If

            hintRightSize = g.MeasureString(hintRightText, keyboardFont)
            hintRightX = size.Width - hintRightSize.Width - 10

            pauseSize = g.MeasureString(pauseText, keyboardFont)
            pauseY = size.Height - pauseSize.Height - 10

            Dim fpsTwoDigit As String = "FPS: 00"
            fpsSize = g.MeasureString(fpsTwoDigit, keyboardFont)
            fpsX = size.Width - fpsSize.Width - 10
            fpsY = size.Height - fpsSize.Height - 10
        End Sub

        ' ============================================================
        '   AI DIFFICULTY LAYOUT
        ' ============================================================
        Private Sub CacheAIDifficulty(g As Graphics, size As Size)
            aiTitleSize = g.MeasureString("Difficulty", aiTitleFont)
            aiTitleX = (size.Width - aiTitleSize.Width) / 2.0F
            aiTitleY = size.Height * 0.2F

            Dim baseY As Single = size.Height * 0.4F
            Dim spacing As Single = size.Height * 0.03F

            ReDim aiOptionSizes(aiOptions.Length - 1)
            ReDim aiOptionX(aiOptions.Length - 1)
            ReDim aiOptionY(aiOptions.Length - 1)

            For i As Integer = 0 To aiOptions.Length - 1
                aiOptionSizes(i) = g.MeasureString(aiOptions(i), startMenuFont)
                aiOptionX(i) = (size.Width - aiOptionSizes(i).Width) / 2.0F
                aiOptionY(i) = baseY + i * (aiOptionSizes(i).Height + spacing)

                Dim rect As New Rectangle(
                CInt(aiOptionX(i)),
                CInt(aiOptionY(i)),
                CInt(aiOptionSizes(i).Width),
                CInt(aiOptionSizes(i).Height)
            )

                Select Case i
                    Case 0 : aiEasyRect = rect
                    Case 1 : aiNormalRect = rect
                    Case 2 : aiHardRect = rect
                End Select
            Next









                    ' ============================
        ' Blink Info ("Press SPACE to Start")
        ' ============================
        Dim info As String = "Press SPACE to Start"
        aiInfoSize = g.MeasureString(info, startInfoFont)
        aiInfoX = (size.Width - aiInfoSize.Width) / 2.0F
        aiInfoY = size.Height * 0.8F
    End Sub

    ' ============================================================
    '   UPDATE METHODS
    ' ============================================================
    Public Sub UpdateFPS()
        frameCount += 1

        If fpsStopwatch.ElapsedMilliseconds >= 1000 Then
            fps = frameCount
            fpsText = $"FPS: {fps}"
            frameCount = 0
            fpsStopwatch.Restart()
        End If
    End Sub

    Public Sub UpdateBallPosition(deltaTime As Double)
        ballPos.X += CSng(velX * deltaTime)
        ballPos.Y += CSng(velY * deltaTime)
        UpdateTrail()
    End Sub

    Public Sub UpdateBallPosition(newPos As PointF)
        ballPos = newPos
        UpdateTrail()
    End Sub

    Public Sub UpdateBallVelocity(newVelX As Double, newVelY As Double)
        velX = newVelX
        velY = newVelY
    End Sub

    Public Sub UpdateBallDiameter(newDiameter As Integer)
        ballDiameter = newDiameter
        InitTrails()
    End Sub

    Public Sub UpdateBallBrush(newBrush As SolidBrush)
        ballBrush?.Dispose()
        ballBrush = newBrush
    End Sub

    Public Sub UpdateTrail()
        If trailPoints Is Nothing Then Exit Sub

        trailIndex = (trailIndex + 1) Mod TrailLength
        trailPoints(trailIndex) = ballPos

        If trailCount < TrailLength Then
            trailCount += 1
        End If

        If trailCount = 1 Then
            trailSizes(0) = ballDiameter
            trailOffsets(0) = 0
            trailAlpha(0) = 16
            Exit Sub
        End If

        For i As Integer = 0 To trailCount - 1
            Dim t As Double = 1.0 - (i / (trailCount - 1))
            trailSizes(i) = CInt(ballDiameter * (0.5 + 0.5 * t))
            trailOffsets(i) = CSng(ballDiameter * (0.25 * (1 - t)))
            trailAlpha(i) = CInt(16 * (0.2 + 0.8 * t))
        Next
    End Sub

    Public Sub ClearTrail()
        trailCount = 0
        trailIndex = -1
        For i As Integer = 0 To TrailLength - 1
            trailSizes(i) = 0
            trailOffsets(i) = 0
            trailAlpha(i) = 0
        Next
    End Sub

    Public Sub UpdateScore(leftScore As Integer, rightScore As Integer, g As Graphics, size As Size)
        scoreLeft = leftScore
        scoreRight = rightScore
        CacheHUD(g, size)
    End Sub

    Public Sub UpdatePlayerNames(leftName As String, rightName As String, g As Graphics, size As Size)
        leftPlayerName = leftName
        rightPlayerName = rightName
        CacheHUD(g, size)
    End Sub

    Public Sub UpdatePaddlePositions(leftY As Single, rightY As Single, size As Size)
        paddleLeft.Y = Math.Max(0, Math.Min(size.Height - paddleHeight, leftY))
        paddleRight.Y = Math.Max(0, Math.Min(size.Height - paddleHeight, rightY))
    End Sub

    Public Sub ToggleKeyboardHints()
        showKeyboardHints = Not showKeyboardHints
    End Sub

    Public Sub SetPlayerMode(mode As Integer, g As Graphics, size As Size)
        numberOfPlayersSelection = If(mode = 2, 1, 0)
        CacheKeyboardHints(g, size)
    End Sub

    Public Sub SetWinnerText(text As String)
        winnerText = text
    End Sub

    Public Sub SetGameState(state As GameState)
        currentState = state
    End Sub

    Public Sub SetAIDifficultySelection(sel As Integer)
        aiDifficultySelection = sel
    End Sub

    Public Sub SetStartMenuSelection(sel As Integer)
        numberOfPlayersSelection = sel
    End Sub

    Public Sub SetPauseMenuSelection(sel As Integer)
        pauseMenuSelection = sel
    End Sub

    ' ============================================================
    '   RENDERING PIPELINE
    ' ============================================================
    Public Sub Render(g As Graphics, state As GameState, showHints As Boolean)


        g.CompositingMode = CompositingMode.SourceOver
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

        Select Case state
            Case GameState.StartScreen
                DrawTrail(g)
                DrawBall(g)
                DrawStartScreen(g)
                If showHints Then DrawKeyboardHintsStartScreen(g)

            Case GameState.Playing
                DrawGamePlayScreen(g)
                If showHints Then DrawKeyboardHintsGamePlayScreen(g)

            Case GameState.Pause
                DrawTrail(g)
                DrawBall(g)
                DrawPaddles(g)
                DrawHUD(g)
                DrawPauseScreen(g)
                If showHints Then DrawKeyboardHintsPauseScreen(g)

            Case GameState.EndScreen
                DrawTrail(g)
                DrawBall(g)
                DrawHUD(g)
                DrawGameOver(g)
                If showHints Then DrawKeyboardHintsGameOverScreen(g)

            Case GameState.AIDifficulty
                DrawTrail(g)
                DrawBall(g)
                DrawAIDifficultyScreen(g)
                If showHints Then DrawKeyboardHintsAIDifficultyScreen(g)
        End Select
    End Sub

    ' ============================================================
    '   DRAW METHODS
    ' ============================================================
    Private Sub DrawTrail(g As Graphics)
        If trailCount <= 0 Then Exit Sub

        For i As Integer = 0 To trailCount - 1
            Dim index As Integer = (trailIndex - i + TrailLength) Mod TrailLength
            Dim p As PointF = trailPoints(index)
            Dim offset As Single = trailOffsets(i)
            Dim alpha As Integer = trailAlpha(i)

            If alpha <= 0 Then Continue For

            Dim baseColor As Color = Color.FromArgb(alpha, 0, 191, 255)
            trailBrushes(i).Color = baseColor

            g.FillEllipse(trailBrushes(i),
                          p.X + offset,
                          p.Y + offset,
                          trailSizes(i),
                          trailSizes(i))
        Next
    End Sub

    Private Sub DrawBall(g As Graphics)
        g.FillEllipse(ballBrush, ballPos.X, ballPos.Y, ballDiameter, ballDiameter)
    End Sub

    Private Sub DrawPaddles(g As Graphics)
        g.FillRectangle(paddleBrush, paddleLeft)
        g.FillRectangle(paddleBrush, paddleRight)
    End Sub

    Private Sub DrawHUD(g As Graphics)
        g.DrawString(leftPlayerName, hudLabelFont, labelBrush, leftLabelX, labelY)
        g.DrawString(rightPlayerName, hudLabelFont, labelBrush, rightLabelX, labelY)
        g.DrawString(scoreLeft.ToString(), hudScoreFont, scoreBrush, leftScoreX, scoreY)
        g.DrawString(scoreRight.ToString(), hudScoreFont, scoreBrush, rightScoreX, scoreY)
    End Sub

    Private Sub DrawGamePlayScreen(g As Graphics)
        DrawTrail(g)
        DrawBall(g)
        DrawPaddles(g)
        DrawHUD(g)
    End Sub

    Private Sub DrawStartScreen(g As Graphics)
        Dim title As String = "PONG"
        Dim titleSize = g.MeasureString(title, startTitleFont)
        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString(title, startTitleFont, titleBrush,
                         CSng((clientSize.Width - titleSize.Width) / 2.0F),
                         CSng(clientSize.Height * 0.15F))
        End Using

        Dim option1 As String = "1 Player"
        Dim option2 As String = "2 Players"

        Dim opt1Size = g.MeasureString(option1, startMenuFont)
        Dim opt2Size = g.MeasureString(option2, startMenuFont)

        Dim opt1X As Single = CSng((clientSize.Width - opt1Size.Width) / 2.0F)
        Dim opt1Y As Single = CSng(clientSize.Height * 0.4F)
        Dim opt2X As Single = CSng((clientSize.Width - opt2Size.Width) / 2.0F)
        Dim opt2Y As Single = CSng(clientSize.Height * 0.55F)

        onePlayerRect = New Rectangle(CInt(opt1X), CInt(opt1Y),
                                      CInt(opt1Size.Width), CInt(opt1Size.Height))
        twoPlayersRect = New Rectangle(CInt(opt2X), CInt(opt2Y),
                                       CInt(opt2Size.Width), CInt(opt2Size.Height))

        Dim radius As Integer = clientSize.Height \ 64

        If numberOfPlayersSelection = 0 Then
            FillRoundedRectangle(g, lightBrush, onePlayerRect, radius)
            DrawRoundedRectangle(g, outlinePen, onePlayerRect, radius)
        Else
            FillRoundedRectangle(g, darkBrush, onePlayerRect, radius)
            DrawRoundedRectangle(g, darkOutlinePen, onePlayerRect, radius)
        End If

        If numberOfPlayersSelection = 1 Then
            FillRoundedRectangle(g, lightBrush, twoPlayersRect, radius)
            DrawRoundedRectangle(g, outlinePen, twoPlayersRect, radius)
        Else
            FillRoundedRectangle(g, darkBrush, twoPlayersRect, radius)
            DrawRoundedRectangle(g, darkOutlinePen, twoPlayersRect, radius)
        End If

        Dim opt1Brush As SolidBrush = If(numberOfPlayersSelection = 0, whiteBrush, grayBrush)
        Dim opt2Brush As SolidBrush = If(numberOfPlayersSelection = 1, whiteBrush, grayBrush)

        g.DrawString(option1, startMenuFont, opt1Brush, opt1X, opt1Y)
        g.DrawString(option2, startMenuFont, opt2Brush, opt2X, opt2Y)

        If blinkVisible Then
            Dim info As String = "Press SPACE to Start"
            Dim infoSize = g.MeasureString(info, startInfoFont)

            g.DrawString(info, startInfoFont, whiteBrush,
                         CSng((clientSize.Width - infoSize.Width) / 2.0F),
                         CSng(clientSize.Height * 0.75F))
        End If
    End Sub

    Public Sub UpdateStartScreenFX()
        If titleFadeIn Then
            titleAlpha += 2
            If titleAlpha >= 255 Then
                titleAlpha = 255
                titleFadeIn = False
            End If
        Else
            titleAlpha -= 2
            If titleAlpha <= 64 Then
                titleAlpha = 64
                titleFadeIn = True
            End If
        End If

        If blinkStopwatch.ElapsedMilliseconds >= 1000 Then
            blinkVisible = Not blinkVisible
            blinkStopwatch.Restart()
        End If
    End Sub

    Private Sub DrawPauseScreen(g As Graphics)
        g.FillRectangle(dimBrush, ClientRectangle)

        'g.DrawString(pauseTitle, pauseTitleFont, whiteBrush, pauseTitleX, pauseTitleY)

        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString(pauseTitle, pauseTitleFont, whiteBrush,
                         pauseTitleX, pauseTitleY)
        End Using







        Dim radius As Integer = clientSize.Height \ 64

        For i As Integer = 0 To pauseMenuItems.Length - 1
            Dim text = pauseMenuItems(i)
            Dim x = pauseMenuItemX(i)
            Dim y = pauseMenuItemY(i)
            Dim size = pauseMenuItemSizes(i)

            Dim rect As New Rectangle(CInt(x), CInt(y), CInt(size.Width), CInt(size.Height))

            Select Case i
                Case 0 : pauseResumeRect = rect
                Case 1 : pauseNewRect = rect
                Case 2 : pauseQuitRect = rect
            End Select

            If i = pauseMenuSelection Then
                FillRoundedRectangle(g, lightBrush, rect, radius)
                DrawRoundedRectangle(g, outlinePen, rect, radius)
            Else
                FillRoundedRectangle(g, darkBrush, rect, radius)
                DrawRoundedRectangle(g, darkOutlinePen, rect, radius)
            End If

            Dim brush As SolidBrush = If(i = pauseMenuSelection, whiteBrush, grayBrush)
            g.DrawString(text, pauseMenuFont, brush, x, y)
        Next
    End Sub

    Private Sub DrawAIDifficultyScreen(g As Graphics)
        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString("Difficulty", aiTitleFont, titleBrush,
                         aiTitleX, aiTitleY)
        End Using

        Dim radius As Integer = clientSize.Height \ 64

        For i As Integer = 0 To aiOptions.Length - 1
            Dim rect As Rectangle =
                If(i = 0, aiEasyRect,
                If(i = 1, aiNormalRect, aiHardRect))

            If i = aiDifficultySelection Then
                FillRoundedRectangle(g, lightBrush, rect, radius)
                DrawRoundedRectangle(g, outlinePen, rect, radius)
            Else
                FillRoundedRectangle(g, darkBrush, rect, radius)
                DrawRoundedRectangle(g, darkOutlinePen, rect, radius)
            End If

            Dim brush As SolidBrush =
                If(i = aiDifficultySelection, whiteBrush, grayBrush)

            g.DrawString(aiOptions(i), startMenuFont, brush,
                         aiOptionX(i), aiOptionY(i))
        Next

        If blinkVisible Then
            g.DrawString("Press SPACE to Start", startInfoFont, whiteBrush,
                         aiInfoX, aiInfoY)
        End If
    End Sub

    Private Sub DrawGameOver(g As Graphics)
        Dim titleSize = g.MeasureString(winnerText, gameOverFont)
        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString(winnerText, gameOverFont, titleBrush,
                         CSng((clientSize.Width - titleSize.Width) / 2.0F),
                         CSng(clientSize.Height * 0.35F))
        End Using

        If blinkVisible Then
            Dim info As String = "Press SPACE to Start"
            Dim infoSize = g.MeasureString(info, gameOverInfoFont)

            g.DrawString(info, gameOverInfoFont, whiteBrush,
                         CSng((clientSize.Width - infoSize.Width) / 2.0F),
                         CSng(clientSize.Height * 0.6F))
        End If
    End Sub

    ' ============================================================
    '   KEYBOARD HINT RENDERERS
    ' ============================================================
    Private Sub DrawKeyboardHintsStartScreen(g As Graphics)
        Dim hintText As String = "1 - One Player   2 - Two Players   Enter - Start Match"
        g.DrawString(hintText, keyboardFont, grayBrush, 10, 10)

        Dim fsText As String =
            If(formBorderStyle = FormBorderStyle.None,
               "F - Exit Fullscreen",
               "F - Fullscreen")

        Dim fsSize = g.MeasureString(fsText, keyboardFont)

        g.DrawString(fsText, keyboardFont, grayBrush,
                     clientSize.Width - fsSize.Width - 10, 10)

        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
        Dim hideSize = g.MeasureString(hideText, keyboardFont)

        g.DrawString(hideText, keyboardFont, grayBrush,
                     10, clientSize.Height - hideSize.Height - 10)
    End Sub

    Private Sub DrawKeyboardHintsPauseScreen(g As Graphics)
        Dim hintText As String = "R - Resume Match   N - New Match"
        g.DrawString(hintText, keyboardFont, grayBrush, 10, 10)

        Dim fsText As String =
            If(formBorderStyle = FormBorderStyle.None,
               "F - Exit Fullscreen",
               "F - Fullscreen")

        Dim fsSize = g.MeasureString(fsText, keyboardFont)

        g.DrawString(fsText, keyboardFont, grayBrush,
                     clientSize.Width - fsSize.Width - 10, 10)

        Dim quitText As String = "Q - Quit Match"
        Dim quitSize = g.MeasureString(quitText, keyboardFont)

        g.DrawString(quitText, keyboardFont, grayBrush,
                     10, clientSize.Height - quitSize.Height - 10)

        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
        Dim hideSize = g.MeasureString(hideText, keyboardFont)

        g.DrawString(hideText, keyboardFont, grayBrush,
                     clientSize.Width - hideSize.Width - 10,
                     clientSize.Height - hideSize.Height - 10)
    End Sub

    Private Sub DrawKeyboardHintsGamePlayScreen(g As Graphics)
        g.DrawString(hintLeftText, keyboardFont, fsIndicatorBrush, hintLeftX, hintLeftY)
        g.DrawString(hintRightText, keyboardFont, fsIndicatorBrush, hintRightX, hintRightY)

        If Not String.IsNullOrEmpty(pauseText) Then
            g.DrawString(pauseText, keyboardFont, fsIndicatorBrush, pauseX, pauseY)
        End If

        g.DrawString(fpsText, keyboardFont, fsIndicatorBrush, fpsX, fpsY)
    End Sub

    Private Sub DrawKeyboardHintsAIDifficultyScreen(g As Graphics)
        Dim hintText As String = "E - Easy   N - Normal   H - Hard   Enter - Start Match"
        g.DrawString(hintText, keyboardFont, grayBrush, 10, 10)

        Dim fsText As String =
            If(formBorderStyle = FormBorderStyle.None,
               "F - Exit Fullscreen",
               "F - Fullscreen")

        Dim fsSize = g.MeasureString(fsText, keyboardFont)

        g.DrawString(fsText, keyboardFont, grayBrush,
                     clientSize.Width - fsSize.Width - 10, 10)

        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
        Dim hideSize = g.MeasureString(hideText, keyboardFont)

        g.DrawString(hideText, keyboardFont, grayBrush,
                     10, clientSize.Height - hideSize.Height - 10)
    End Sub

        'Private Sub DrawKeyboardHintsGameOverScreen(g As Graphics)
        '    Dim hintText As String = "Enter - Start








        Private Sub DrawKeyboardHintsGameOverScreen(g As Graphics)
        Dim hintText As String = "Enter - Start New Match"
        g.DrawString(hintText, keyboardFont, grayBrush, 10, 10)

        Dim fsText As String =
            If(formBorderStyle = FormBorderStyle.None,
               "F - Exit Fullscreen",
               "F - Fullscreen")

        Dim fsSize = g.MeasureString(fsText, keyboardFont)

        g.DrawString(fsText, keyboardFont, grayBrush,
                     clientSize.Width - fsSize.Width - 10, 10)

        Dim quitText As String = "CTRL Q - Quit Game"
        Dim quitSize = g.MeasureString(quitText, keyboardFont)

        g.DrawString(quitText, keyboardFont, grayBrush,
                     10, clientSize.Height - quitSize.Height - 10)
    End Sub

    ' ============================================================
    '   ROUNDED RECTANGLE HELPERS
    ' ============================================================
    Private Function RoundedRect(rect As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim d As Integer = radius * 2

        path.AddArc(rect.X, rect.Y, d, d, 180, 90)
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90)
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90)
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90)
        path.CloseFigure()

        Return path
    End Function

    Private Sub FillRoundedRectangle(g As Graphics, brush As Brush, rect As Rectangle, radius As Integer)
        Using path As GraphicsPath = RoundedRect(rect, radius)
            g.FillPath(brush, path)
        End Using
    End Sub

    Private Sub DrawRoundedRectangle(g As Graphics, pen As Pen, rect As Rectangle, radius As Integer)
        Using path As GraphicsPath = RoundedRect(rect, radius)
            g.DrawPath(pen, path)
        End Using
    End Sub

    ' ============================================================
    '   CLIENT RECTANGLE
    ' ============================================================
    Private ReadOnly Property ClientRectangle As Rectangle
        Get
            Return New Rectangle(0, 0, clientSize.Width, clientSize.Height)
        End Get
    End Property

    ' ============================================================
    '   DISPOSAL
    ' ============================================================
    Public Sub Dispose() Implements IDisposable.Dispose
        DisposeFonts()

        ballBrush?.Dispose()
        paddleBrush?.Dispose()
        scoreBrush?.Dispose()
        labelBrush?.Dispose()
        whiteBrush?.Dispose()
        grayBrush?.Dispose()
        dimBrush?.Dispose()
        fsIndicatorBrush?.Dispose()

        For Each b In trailBrushes
            b?.Dispose()
        Next

        outlinePen?.Dispose()
        darkOutlinePen?.Dispose()
    End Sub

    Private Sub DisposeFonts()
        hudScoreFont?.Dispose()
        hudLabelFont?.Dispose()
        pauseTitleFont?.Dispose()
        pauseMenuFont?.Dispose()
        startTitleFont?.Dispose()
        startMenuFont?.Dispose()
        startInfoFont?.Dispose()
        aiTitleFont?.Dispose()
        gameOverFont?.Dispose()
        gameOverInfoFont?.Dispose()
        keyboardFont?.Dispose()
        fpsFont?.Dispose()
    End Sub

End Class
