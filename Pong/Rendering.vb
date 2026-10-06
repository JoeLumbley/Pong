
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
    Private outlinePen As New Pen(Color.FromArgb(32, 255, 255, 255), 2)
    Private darkOutlinePen As New Pen(Color.FromArgb(32, 255, 255, 255), 2)


    ' ============================================================
    '   GRAPHICS PATHS
    ' ============================================================
    Private onePlayerPath As GraphicsPath
    Private twoPlayersPath As GraphicsPath

    Private pauseResumePath As GraphicsPath
    Private pauseNewPath As GraphicsPath
    Private pauseQuitPath As GraphicsPath

    Private aiEasyPath As GraphicsPath
    Private aiNormalPath As GraphicsPath
    Private aiHardPath As GraphicsPath


    ' ============================================================
    '   START SCREEN LAYOUT / PATHS
    ' ============================================================
    Private startTitleSize As SizeF
    Private startTitleX As Single
    Private startTitleY As Single

    Private startOption1Size As SizeF
    Private startOption2Size As SizeF
    Private startOption1X As Single
    Private startOption1Y As Single
    Private startOption2X As Single
    Private startOption2Y As Single

    Private startInfoSize As SizeF
    Private startInfoX As Single
    Private startInfoY As Single

    Private startRadius As Integer









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
    'Public Sub New(g As Graphics, size As Size)
    '    clientSize = size
    '    g.SmoothingMode = SmoothingMode.AntiAlias
    '    g.PixelOffsetMode = PixelOffsetMode.HighQuality

    '    ScaleBall(size)
    '    InitGraphics(g, size)
    '    InitTrails()
    '    InitPaddles(size)
    '    CacheHUD(g, size)
    '    CachePause(g, size)
    '    CacheKeyboardHints(g, size)
    '    CacheAIDifficulty(g, size)

    '    fpsStopwatch.Start()
    '    blinkStopwatch.Start()
    'End Sub


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
        CacheStartScreen(g, size)   ' <<< NEW

        fpsStopwatch.Start()
        blinkStopwatch.Start()
    End Sub


    Private Sub CacheStartScreen(g As Graphics, size As Size)
        Dim title As String = "PONG"
        startTitleSize = g.MeasureString(title, startTitleFont)
        startTitleX = (size.Width - startTitleSize.Width) / 2.0F
        startTitleY = size.Height * 0.15F

        Dim option1 As String = "1 Player"
        Dim option2 As String = "2 Players"

        startOption1Size = g.MeasureString(option1, startMenuFont)
        startOption2Size = g.MeasureString(option2, startMenuFont)


        Dim startMenuSpacing = size.Height * 0.12F

        startOption1X = (size.Width - startOption1Size.Width) / 2.0F
        startOption1Y = size.Height * 0.4F
        startOption2X = (size.Width - startOption2Size.Width) / 2.0F
        startOption2Y = startOption1Y + startMenuSpacing

        onePlayerRect = New Rectangle(
        CInt(startOption1X),
        CInt(startOption1Y),
        CInt(startOption1Size.Width),
        CInt(startOption1Size.Height))

        twoPlayersRect = New Rectangle(
        CInt(startOption2X),
        CInt(startOption2Y),
        CInt(startOption2Size.Width),
        CInt(startOption2Size.Height))

        startRadius = size.Height \ 64

        ' Precompute paths
        onePlayerPath?.Dispose()
        twoPlayersPath?.Dispose()

        onePlayerPath = RoundedRect(onePlayerRect, startRadius)
        twoPlayersPath = RoundedRect(twoPlayersRect, startRadius)

        Dim info As String = "Press SPACE to Start"
        startInfoSize = g.MeasureString(info, startInfoFont)
        startInfoX = (size.Width - startInfoSize.Width) / 2.0F
        startInfoY = size.Height * 0.65F
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
        CacheStartScreen(g, size)
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
    'Private Sub CachePause(g As Graphics, size As Size)
    '    pauseTitleSize = g.MeasureString(pauseTitle, pauseTitleFont)
    '    pauseTitleX = (size.Width - pauseTitleSize.Width) / 2.0F
    '    pauseTitleY = size.Height * 0.2F

    '    pauseMenuSpacing = size.Height * 0.13F
    '    pauseMenuStartY = pauseTitleY + pauseTitleSize.Height + (size.Height * 0.01F)

    '    ReDim pauseMenuItemSizes(pauseMenuItems.Length - 1)
    '    ReDim pauseMenuItemX(pauseMenuItems.Length - 1)
    '    ReDim pauseMenuItemY(pauseMenuItems.Length - 1)

    '    For i As Integer = 0 To pauseMenuItems.Length - 1
    '        pauseMenuItemSizes(i) = g.MeasureString(pauseMenuItems(i), pauseMenuFont)
    '        pauseMenuItemX(i) = (size.Width - pauseMenuItemSizes(i).Width) / 2.0F
    '        pauseMenuItemY(i) = pauseMenuStartY + i * pauseMenuSpacing
    '    Next
    'End Sub


    Private Sub CachePause(g As Graphics, size As Size)
        pauseTitleSize = g.MeasureString(pauseTitle, pauseTitleFont)
        pauseTitleX = (size.Width - pauseTitleSize.Width) / 2.0F
        pauseTitleY = size.Height * 0.25F

        pauseMenuSpacing = size.Height * 0.12F
        pauseMenuStartY = pauseTitleY + pauseTitleSize.Height + (size.Height * 0.01F)

        ReDim pauseMenuItemSizes(pauseMenuItems.Length - 1)
        ReDim pauseMenuItemX(pauseMenuItems.Length - 1)
        ReDim pauseMenuItemY(pauseMenuItems.Length - 1)

        Dim radius As Integer = size.Height \ 64

        ' Dispose old paths
        pauseResumePath?.Dispose()
        pauseNewPath?.Dispose()
        pauseQuitPath?.Dispose()

        For i As Integer = 0 To pauseMenuItems.Length - 1
            pauseMenuItemSizes(i) = g.MeasureString(pauseMenuItems(i), pauseMenuFont)
            pauseMenuItemX(i) = (size.Width - pauseMenuItemSizes(i).Width) / 2.0F
            pauseMenuItemY(i) = pauseMenuStartY + i * pauseMenuSpacing

            Dim rect As New Rectangle(
            CInt(pauseMenuItemX(i)),
            CInt(pauseMenuItemY(i)),
            CInt(pauseMenuItemSizes(i).Width),
            CInt(pauseMenuItemSizes(i).Height))

            Select Case i
                Case 0
                    pauseResumeRect = rect
                    pauseResumePath = RoundedRect(rect, radius)
                Case 1
                    pauseNewRect = rect
                    pauseNewPath = RoundedRect(rect, radius)
                Case 2
                    pauseQuitRect = rect
                    pauseQuitPath = RoundedRect(rect, radius)
            End Select
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
    '' ============================================================
    'Private Sub CacheAIDifficulty(g As Graphics, size As Size)
    '    aiTitleSize = g.MeasureString("Difficulty", aiTitleFont)
    '    aiTitleX = (size.Width - aiTitleSize.Width) / 2.0F
    '    aiTitleY = size.Height * 0.2F

    '    Dim baseY As Single = size.Height * 0.4F
    '    Dim spacing As Single = size.Height * 0.03F

    '    ReDim aiOptionSizes(aiOptions.Length - 1)
    '    ReDim aiOptionX(aiOptions.Length - 1)
    '    ReDim aiOptionY(aiOptions.Length - 1)

    '    For i As Integer = 0 To aiOptions.Length - 1
    '        aiOptionSizes(i) = g.MeasureString(aiOptions(i), startMenuFont)
    '        aiOptionX(i) = (size.Width - aiOptionSizes(i).Width) / 2.0F
    '        aiOptionY(i) = baseY + i * (aiOptionSizes(i).Height + spacing)

    '        Dim rect As New Rectangle(
    '            CInt(aiOptionX(i)),
    '            CInt(aiOptionY(i)),
    '            CInt(aiOptionSizes(i).Width),
    '            CInt(aiOptionSizes(i).Height)
    '        )

    '        Select Case i
    '            Case 0 : aiEasyRect = rect
    '            Case 1 : aiNormalRect = rect
    '            Case 2 : aiHardRect = rect
    '        End Select
    '    Next


    '    ' ============================
    '    ' Blink Info ("Press SPACE to Start")
    '    ' ============================
    '    Dim info As String = "Press SPACE to Start"
    '    aiInfoSize = g.MeasureString(info, startInfoFont)
    '    aiInfoX = (size.Width - aiInfoSize.Width) / 2.0F
    '    aiInfoY = size.Height * 0.8F
    'End Sub


    Private Sub CacheAIDifficulty(g As Graphics, size As Size)
        aiTitleSize = g.MeasureString("Difficulty", aiTitleFont)
        aiTitleX = (size.Width - aiTitleSize.Width) / 2.0F
        aiTitleY = size.Height * 0.2F

        Dim baseY As Single = size.Height * 0.4F
        Dim spacing As Single = size.Height * 0.03F

        ReDim aiOptionSizes(aiOptions.Length - 1)
        ReDim aiOptionX(aiOptions.Length - 1)
        ReDim aiOptionY(aiOptions.Length - 1)

        Dim radius As Integer = size.Height \ 64

        aiEasyPath?.Dispose()
        aiNormalPath?.Dispose()
        aiHardPath?.Dispose()

        For i As Integer = 0 To aiOptions.Length - 1
            aiOptionSizes(i) = g.MeasureString(aiOptions(i), startMenuFont)
            aiOptionX(i) = (size.Width - aiOptionSizes(i).Width) / 2.0F
            aiOptionY(i) = baseY + i * (aiOptionSizes(i).Height + spacing)

            Dim rect As New Rectangle(
            CInt(aiOptionX(i)),
            CInt(aiOptionY(i)),
            CInt(aiOptionSizes(i).Width),
            CInt(aiOptionSizes(i).Height))

            Select Case i
                Case 0
                    aiEasyRect = rect
                    aiEasyPath = RoundedRect(rect, radius)
                Case 1
                    aiNormalRect = rect
                    aiNormalPath = RoundedRect(rect, radius)
                Case 2
                    aiHardRect = rect
                    aiHardPath = RoundedRect(rect, radius)
            End Select
        Next

        Dim info As String = "Press SPACE to Start"
        aiInfoSize = g.MeasureString(info, startInfoFont)
        aiInfoX = (size.Width - aiInfoSize.Width) / 2.0F
        aiInfoY = size.Height * 0.75F
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

    'Private Sub DrawStartScreen(g As Graphics)
    '    Dim title As String = "PONG"
    '    Dim titleSize = g.MeasureString(title, startTitleFont)
    '    Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

    '    Using titleBrush As New SolidBrush(titleColor)
    '        g.DrawString(title, startTitleFont, titleBrush,
    '                     CSng((clientSize.Width - titleSize.Width) / 2.0F),
    '                     CSng(clientSize.Height * 0.15F))
    '    End Using

    '    Dim option1 As String = "1 Player"
    '    Dim option2 As String = "2 Players"

    '    Dim opt1Size = g.MeasureString(option1, startMenuFont)
    '    Dim opt2Size = g.MeasureString(option2, startMenuFont)

    '    Dim opt1X As Single = CSng((clientSize.Width - opt1Size.Width) / 2.0F)
    '    Dim opt1Y As Single = CSng(clientSize.Height * 0.4F)
    '    Dim opt2X As Single = CSng((clientSize.Width - opt2Size.Width) / 2.0F)
    '    Dim opt2Y As Single = CSng(clientSize.Height * 0.55F)

    '    onePlayerRect = New Rectangle(CInt(opt1X), CInt(opt1Y),
    '                                  CInt(opt1Size.Width), CInt(opt1Size.Height))
    '    twoPlayersRect = New Rectangle(CInt(opt2X), CInt(opt2Y),
    '                                   CInt(opt2Size.Width), CInt(opt2Size.Height))

    '    Dim radius As Integer = clientSize.Height \ 64

    '    If numberOfPlayersSelection = 0 Then
    '        FillRoundedRectangle(g, lightBrush, onePlayerRect, radius)
    '        DrawRoundedRectangle(g, outlinePen, onePlayerRect, radius)
    '    Else
    '        FillRoundedRectangle(g, darkBrush, onePlayerRect, radius)
    '        DrawRoundedRectangle(g, darkOutlinePen, onePlayerRect, radius)
    '    End If

    '    If numberOfPlayersSelection = 1 Then
    '        FillRoundedRectangle(g, lightBrush, twoPlayersRect, radius)
    '        DrawRoundedRectangle(g, outlinePen, twoPlayersRect, radius)
    '    Else
    '        FillRoundedRectangle(g, darkBrush, twoPlayersRect, radius)
    '        DrawRoundedRectangle(g, darkOutlinePen, twoPlayersRect, radius)
    '    End If

    '    Dim opt1Brush As SolidBrush = If(numberOfPlayersSelection = 0, whiteBrush, grayBrush)
    '    Dim opt2Brush As SolidBrush = If(numberOfPlayersSelection = 1, whiteBrush, grayBrush)

    '    g.DrawString(option1, startMenuFont, opt1Brush, opt1X, opt1Y)
    '    g.DrawString(option2, startMenuFont, opt2Brush, opt2X, opt2Y)

    '    If blinkVisible Then
    '        Dim info As String = "Press SPACE to Start"
    '        Dim infoSize = g.MeasureString(info, startInfoFont)

    '        g.DrawString(info, startInfoFont, whiteBrush,
    '                     CSng((clientSize.Width - infoSize.Width) / 2.0F),
    '                     CSng(clientSize.Height * 0.75F))
    '    End If
    'End Sub

    Private Sub DrawStartScreen(g As Graphics)
        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString("PONG", startTitleFont, titleBrush, startTitleX, startTitleY)
        End Using

        Dim opt1Brush As SolidBrush = If(numberOfPlayersSelection = 0, whiteBrush, grayBrush)
        Dim opt2Brush As SolidBrush = If(numberOfPlayersSelection = 1, whiteBrush, grayBrush)

        ' Fill/outline using precomputed paths
        If numberOfPlayersSelection = 0 Then
            g.FillPath(lightBrush, onePlayerPath)
            g.DrawPath(outlinePen, onePlayerPath)
            g.FillPath(darkBrush, twoPlayersPath)
            g.DrawPath(darkOutlinePen, twoPlayersPath)
        Else
            g.FillPath(darkBrush, onePlayerPath)
            g.DrawPath(darkOutlinePen, onePlayerPath)
            g.FillPath(lightBrush, twoPlayersPath)
            g.DrawPath(outlinePen, twoPlayersPath)
        End If

        g.DrawString("1 Player", startMenuFont, opt1Brush, startOption1X, startOption1Y)
        g.DrawString("2 Players", startMenuFont, opt2Brush, startOption2X, startOption2Y)

        If blinkVisible Then
            g.DrawString("Press SPACE to Start", startInfoFont, whiteBrush,
                     startInfoX, startInfoY)
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

    'Private Sub DrawPauseScreen(g As Graphics)
    '    g.FillRectangle(dimBrush, ClientRectangle)

    '    g.DrawString(pauseTitle, pauseTitleFont, whiteBrush, pauseTitleX, pauseTitleY)


    '    Dim radius As Integer = clientSize.Height \ 64

    '    For i As Integer = 0 To pauseMenuItems.Length - 1
    '        Dim text = pauseMenuItems(i)
    '        Dim x = pauseMenuItemX(i)
    '        Dim y = pauseMenuItemY(i)
    '        Dim size = pauseMenuItemSizes(i)

    '        Dim rect As New Rectangle(CInt(x), CInt(y), CInt(size.Width), CInt(size.Height))

    '        Select Case i
    '            Case 0 : pauseResumeRect = rect
    '            Case 1 : pauseNewRect = rect
    '            Case 2 : pauseQuitRect = rect
    '        End Select

    '        If i = pauseMenuSelection Then
    '            FillRoundedRectangle(g, lightBrush, rect, radius)
    '            DrawRoundedRectangle(g, outlinePen, rect, radius)
    '        Else
    '            FillRoundedRectangle(g, darkBrush, rect, radius)
    '            DrawRoundedRectangle(g, darkOutlinePen, rect, radius)
    '        End If

    '        Dim brush As SolidBrush = If(i = pauseMenuSelection, whiteBrush, grayBrush)
    '        g.DrawString(text, pauseMenuFont, brush, x, y)
    '    Next
    'End Sub

    Private Sub DrawPauseScreen(g As Graphics)
        g.FillRectangle(dimBrush, ClientRectangle)
        g.DrawString(pauseTitle, pauseTitleFont, whiteBrush, pauseTitleX, pauseTitleY)

        Dim radius As Integer = clientSize.Height \ 64   ' kept for consistency, but paths already use it

        Dim gPath As GraphicsPath = pauseResumePath


        For i As Integer = 0 To pauseMenuItems.Length - 1
            Dim text = pauseMenuItems(i)
            Dim x = pauseMenuItemX(i)
            Dim y = pauseMenuItemY(i)

            'Dim gPath As GraphicsPath = pauseResumePath

            Select Case i
                Case 0 : gPath = pauseResumePath
                Case 1 : gPath = pauseNewPath
                Case 2 : gPath = pauseQuitPath
            End Select

            If i = pauseMenuSelection Then
                g.FillPath(lightBrush, gPath)
                g.DrawPath(outlinePen, gPath)
            Else
                g.FillPath(darkBrush, gPath)
                g.DrawPath(darkOutlinePen, gPath)
            End If

            Dim brush As SolidBrush = If(i = pauseMenuSelection, whiteBrush, grayBrush)
            g.DrawString(text, pauseMenuFont, brush, x, y)
        Next
    End Sub














































    'Private Sub DrawAIDifficultyScreen(g As Graphics)
    '    Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

    '    Using titleBrush As New SolidBrush(titleColor)
    '        g.DrawString("Difficulty", aiTitleFont, titleBrush,
    '                     aiTitleX, aiTitleY)
    '    End Using

    '    Dim radius As Integer = clientSize.Height \ 64

    '    For i As Integer = 0 To aiOptions.Length - 1
    '        Dim rect As Rectangle =
    '            If(i = 0, aiEasyRect,
    '            If(i = 1, aiNormalRect, aiHardRect))

    '        If i = aiDifficultySelection Then
    '            FillRoundedRectangle(g, lightBrush, rect, radius)
    '            DrawRoundedRectangle(g, outlinePen, rect, radius)
    '        Else
    '            FillRoundedRectangle(g, darkBrush, rect, radius)
    '            DrawRoundedRectangle(g, darkOutlinePen, rect, radius)
    '        End If

    '        Dim brush As SolidBrush =
    '            If(i = aiDifficultySelection, whiteBrush, grayBrush)

    '        g.DrawString(aiOptions(i), startMenuFont, brush,
    '                     aiOptionX(i), aiOptionY(i))
    '    Next

    '    If blinkVisible Then
    '        g.DrawString("Press SPACE to Start", startInfoFont, whiteBrush,
    '                     aiInfoX, aiInfoY)
    '    End If
    'End Sub



    Private Sub DrawAIDifficultyScreen(g As Graphics)
        Dim titleColor As Color = Color.FromArgb(titleAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString("Difficulty", aiTitleFont, titleBrush, aiTitleX, aiTitleY)
        End Using

        Dim gPath As GraphicsPath = aiEasyPath

        For i As Integer = 0 To aiOptions.Length - 1

            'Dim gPath As GraphicsPath = aiEasyPath

            Select Case i
                Case 0 : gPath = aiEasyPath
                Case 1 : gPath = aiNormalPath
                Case 2 : gPath = aiHardPath
            End Select

            If i = aiDifficultySelection Then
                g.FillPath(lightBrush, gPath)
                g.DrawPath(outlinePen, gPath)
            Else
                g.FillPath(darkBrush, gPath)
                g.DrawPath(darkOutlinePen, gPath)
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

