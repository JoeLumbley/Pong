
Imports System.Drawing.Drawing2D
Imports Pong.Enums


Public Class Rendering
    Implements IDisposable

    Private ReadOnly layout As New Layout()

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

    Private Const PaddleTrailLength As Integer = 6
    Private leftPaddleTrail(PaddleTrailLength - 1) As Rectangle
    Private rightPaddleTrail(PaddleTrailLength - 1) As Rectangle
    Private paddleTrailIndex As Integer = -1
    Private paddleTrailCount As Integer = 0

    ' ============================================================
    '   WALL IMPACT EFFECT
    ' ============================================================
    Private Const WallImpactDuration As Double = 0.1
    Private wallImpactPoint As PointF
    Private wallImpactAge As Double = WallImpactDuration


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
    'Private numberOfPlayersSelection As Integer = 0
    Private onePlayerRect As Rectangle
    Private twoPlayersRect As Rectangle

    Private fadeAlpha As Integer = 255
    Private alphaFadeIn As Boolean = True
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
    'Private winnerText As String = ""

    ' ============================================================
    '   UI BRUSHES / PENS
    ' ============================================================
    Private lightBrush As New SolidBrush(Color.FromArgb(255, 42, 42, 42))
    Private darkBrush As New SolidBrush(Color.FromArgb(64, 0, 0, 0))



    Private outlinePen As New Pen(Color.FromArgb(255, 64, 64, 64), 2)
    Private darkOutlinePen As New Pen(Color.FromArgb(32, 0, 0, 0), 2)


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


    ' -------------------------------
    '  Random
    ' -------------------------------
    Private rng As New Random()



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


    Private Sub CacheStartScreen(form As Form)
        Dim g = form.CreateGraphics()
        Dim title As String = "PONG"
        startTitleSize = g.MeasureString(title, startTitleFont)
        startTitleX = (form.ClientSize.Width - startTitleSize.Width) / 2.0F
        startTitleY = form.ClientSize.Height * 0.15F

        Dim option1 As String = "1 Player"
        Dim option2 As String = "2 Players"

        startOption1Size = g.MeasureString(option1, startMenuFont)
        startOption2Size = g.MeasureString(option2, startMenuFont)


        Dim startMenuSpacing = form.ClientSize.Height * 0.12F

        startOption1X = (form.ClientSize.Width - startOption1Size.Width) / 2.0F
        startOption1Y = form.ClientSize.Height * 0.4F
        startOption2X = (form.ClientSize.Width - startOption2Size.Width) / 2.0F
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

        startRadius = form.ClientSize.Height \ 64

        ' Precompute paths
        onePlayerPath?.Dispose()
        twoPlayersPath?.Dispose()

        onePlayerPath = RoundedRect(onePlayerRect, startRadius)
        twoPlayersPath = RoundedRect(twoPlayersRect, startRadius)

        Dim info As String = "Press SPACE to Start"
        startInfoSize = g.MeasureString(info, startInfoFont)
        startInfoX = (form.ClientSize.Width - startInfoSize.Width) / 2.0F
        startInfoY = form.ClientSize.Height * 0.65F
    End Sub

    Public Sub UpdatePaddleTrails(model As GameModel)
        paddleTrailIndex = (paddleTrailIndex + 1) Mod PaddleTrailLength
        leftPaddleTrail(paddleTrailIndex) = model.LeftPaddleRect
        rightPaddleTrail(paddleTrailIndex) = model.RightPaddleRect

        If paddleTrailCount < PaddleTrailLength Then
            paddleTrailCount += 1
        End If
    End Sub

    Public Sub ClearPaddleTrails()
        paddleTrailIndex = -1
        paddleTrailCount = 0
    End Sub




    ' ============================================================
    '   INITIALIZATION
    ' ============================================================
    Public Sub ScaleBall(model As GameModel)
        'model.BallDiameter = CInt(Math.Min(size.Width, size.Height) / 18.0F)
        model.BallDiameter = CInt(model.ClientSize.Height / 18.0F)

    End Sub

    Private Sub DrawPaddleTrails(g As Graphics, model As GameModel)
        If paddleTrailCount <= 0 Then Return

        Using trailBrush As New SolidBrush(Color.White)
            For i As Integer = paddleTrailCount - 1 To 0 Step -1
                Dim index As Integer =
                    (paddleTrailIndex - i + PaddleTrailLength) Mod PaddleTrailLength
                Dim alpha As Integer =
                    CInt(20 * (1.0 - i / CDbl(paddleTrailCount)))

                trailBrush.Color = Color.FromArgb(alpha, Color.White)
                g.FillRectangle(trailBrush, leftPaddleTrail(index))
                g.FillRectangle(trailBrush, rightPaddleTrail(index))
            Next
        End Using
    End Sub

    Public Sub InitGraphics(form As Form, model As GameModel)

        ballBrush = New SolidBrush(Color.DeepSkyBlue)
        paddleBrush = New SolidBrush(Color.White)
        scoreBrush = New SolidBrush(Color.White)
        labelBrush = New SolidBrush(Color.Gray)
        whiteBrush = New SolidBrush(Color.White)
        grayBrush = New SolidBrush(Color.FromArgb(140, 140, 140))
        dimBrush = New SolidBrush(Color.FromArgb(120, 0, 0, 0))
        fsIndicatorBrush = New SolidBrush(Color.FromArgb(120, 255, 255, 255))


        InitTrails(model)
        layout.InitializePaddles(form.ClientSize, model)


        layout.RescalePaddles(form.ClientSize, model)
        layout.ScaleBall(model)

        RescaleFonts(form)

        CacheStartScreen(form)
        CacheAIDifficulty(form)
        CacheHUD(form, model)
        CachePause(form)
        CacheKeyboardHints(form, model)

        fpsStopwatch.Start()
        blinkStopwatch.Start()


        labelY = form.ClientSize.Height / 32
        scoreY = form.ClientSize.Height / 20


    End Sub

    Public Sub InitTrails(model As GameModel)
        For i As Integer = 0 To TrailLength - 1
            Dim size As Integer = model.BallDiameter - (TrailLength - i) * 2
            If size < 10 Then size = 10

            trailSizes(i) = size
            trailOffsets(i) = CSng((model.BallDiameter - size) / 2.0F)

            Dim t As Double = i / CDbl(TrailLength)
            Dim alpha As Integer = CInt(16 * t * t)
            trailAlpha(i) = alpha

            trailBrushes(i) = New SolidBrush(Color.FromArgb(alpha, 0, 191, 255))
        Next
    End Sub

    ' ============================================================
    '   FONT / LAYOUT RESCALING
    ' ============================================================
    Public Sub OnResize(form As Form, model As GameModel, physics As PhysicsEngine)

        If form.WindowState = FormWindowState.Minimized Then Return

        layout.Resize(form.ClientSize, model)
        physics.ResetPaddles()
        physics.CenterBall()


        If model.State = GameState.Playing OrElse model.State = GameState.Pause Then
            physics.ServeBall(If(rng.Next(0, 2) = 0, -1, 1))
        Else
            physics.MoveBallRandom()
        End If

        UpdateTrail(model)
        ClearTrail()




        RescaleFonts(form)



        CacheStartScreen(form)
        CacheAIDifficulty(form)

        CacheHUD(form, model)
        CachePause(form)

        CacheKeyboardHints(form, model)


    End Sub





    Public Sub RescaleFonts(form As Form)
        DisposeFonts()

        hudScoreFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 15.0F), FontStyle.Bold)
        hudLabelFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 55.0F), FontStyle.Bold)
        pauseTitleFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 18.0F), FontStyle.Bold)
        pauseMenuFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 28.0F))
        startTitleFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 12.0F), FontStyle.Bold)
        startMenuFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 30.0F))
        startInfoFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 45.0F))
        aiTitleFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 18.0F), FontStyle.Bold)
        gameOverFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 20.0F), FontStyle.Bold)
        gameOverInfoFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 45.0F))
        keyboardFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 70.0F), FontStyle.Bold)
        fpsFont = New Font("Segoe UI", CSng(form.ClientSize.Height / 75.0F), FontStyle.Bold)

    End Sub

    Public Sub RescalePaddles(size As Size, model As GameModel)
        paddleHeight = size.Height \ 8
        paddleWidth = size.Height \ 30

        model.PaddleHeight = paddleHeight
        model.PaddleWidth = paddleWidth

        paddleLeft.Width = paddleWidth
        paddleLeft.Height = paddleHeight
        paddleRight.Width = paddleWidth
        paddleRight.Height = paddleHeight




        paddleLeft.X = size.Height / 25.0F
        paddleRight.X = size.Width - size.Height / 25.0F - paddleWidth




        paddleLeft.Y = (size.Height - paddleHeight) / 2.0F
        paddleRight.Y = (size.Height - paddleHeight) / 2.0F

        model.LeftPaddleRect = New Rectangle(paddleLeft.X,
                                             paddleLeft.Y,
                                             paddleLeft.Width,
                                             paddleLeft.Height)

        model.RightPaddleRect = New Rectangle(paddleRight.X,
                                             paddleRight.Y,
                                             paddleRight.Width,
                                             paddleRight.Height)


    End Sub

    ' ============================================================
    '   HUD LAYOUT
    ' ============================================================
    Public Sub CacheHUD(form As Form, model As GameModel)
        Dim g = form.CreateGraphics()

        Dim halfWidth As Single = form.ClientSize.Width / 2.0F

        ' Measure
        Dim leftLabelSize = g.MeasureString(model.LeftPlayerName, hudLabelFont)
        Dim rightLabelSize = g.MeasureString(model.RightPlayerName, hudLabelFont)

        Dim leftScoreSize = g.MeasureString(model.ScoreLeft.ToString(), hudScoreFont)
        Dim rightScoreSize = g.MeasureString(model.ScoreRight.ToString(), hudScoreFont)

        ' Center horizontally
        leftLabelX = (halfWidth - leftLabelSize.Width) / 2
        rightLabelX = halfWidth + (halfWidth - rightLabelSize.Width) / 2

        leftScoreX = (halfWidth - leftScoreSize.Width) / 2
        rightScoreX = halfWidth + (halfWidth - rightScoreSize.Width) / 2

        labelY = form.ClientSize.Height / 14


        scoreY = form.ClientSize.Height / 12



    End Sub

    ' ============================================================
    '   PAUSE LAYOUT
    ' ============================================================

    Private Sub CachePause(form As Form)
        Dim g = form.CreateGraphics()
        pauseTitleSize = g.MeasureString(pauseTitle, pauseTitleFont)
        pauseTitleX = (form.ClientSize.Width - pauseTitleSize.Width) / 2.0F
        pauseTitleY = form.ClientSize.Height * 0.25F

        pauseMenuSpacing = form.ClientSize.Height * 0.12F
        pauseMenuStartY = pauseTitleY + pauseTitleSize.Height + (form.ClientSize.Height * 0.01F)

        ReDim pauseMenuItemSizes(pauseMenuItems.Length - 1)
        ReDim pauseMenuItemX(pauseMenuItems.Length - 1)
        ReDim pauseMenuItemY(pauseMenuItems.Length - 1)

        Dim radius As Integer = form.ClientSize.Height \ 64

        ' Dispose old paths
        pauseResumePath?.Dispose()
        pauseNewPath?.Dispose()
        pauseQuitPath?.Dispose()

        For i As Integer = 0 To pauseMenuItems.Length - 1
            pauseMenuItemSizes(i) = g.MeasureString(pauseMenuItems(i), pauseMenuFont)
            pauseMenuItemX(i) = (form.ClientSize.Width - pauseMenuItemSizes(i).Width) / 2.0F
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
    Public Sub CacheKeyboardHints(form As Form, model As GameModel)

        Dim g = form.CreateGraphics()
        hintLeftText = "W S - Move Paddle"
        hintLeftSize = g.MeasureString(hintLeftText, keyboardFont)

        If model.PlayerMode = NumberOfPlayers.TwoPlayers Then
            hintRightText = "Arrows - Move Paddle"
            pauseText = "P - Pause Match"
        Else
            hintRightText = "P - Pause Match"
            pauseText = ""
        End If

        hintRightSize = g.MeasureString(hintRightText, keyboardFont)
        hintRightX = form.ClientSize.Width - hintRightSize.Width - 10

        pauseSize = g.MeasureString(pauseText, keyboardFont)
        pauseY = form.ClientSize.Height - pauseSize.Height - 10

        Dim fpsTwoDigit As String = "FPS: 00"
        fpsSize = g.MeasureString(fpsTwoDigit, keyboardFont)
        fpsX = form.ClientSize.Width - fpsSize.Width - 10
        fpsY = form.ClientSize.Height - fpsSize.Height - 10
    End Sub

    ' ============================================================
    '   AI DIFFICULTY LAYOUT
    ' ============================================================


    Private Sub CacheAIDifficulty(form As Form)
        Dim g = form.CreateGraphics()
        aiTitleSize = g.MeasureString("Difficulty", aiTitleFont)
        aiTitleX = (form.ClientSize.Width - aiTitleSize.Width) / 2.0F
        aiTitleY = form.ClientSize.Height * 0.2F

        Dim baseY As Single = form.ClientSize.Height * 0.4F
        Dim spacing As Single = form.ClientSize.Height * 0.03F

        ReDim aiOptionSizes(aiOptions.Length - 1)
        ReDim aiOptionX(aiOptions.Length - 1)
        ReDim aiOptionY(aiOptions.Length - 1)

        Dim radius As Integer = form.ClientSize.Height \ 64

        aiEasyPath?.Dispose()
        aiNormalPath?.Dispose()
        aiHardPath?.Dispose()

        For i As Integer = 0 To aiOptions.Length - 1
            aiOptionSizes(i) = g.MeasureString(aiOptions(i), startMenuFont)
            aiOptionX(i) = (form.ClientSize.Width - aiOptionSizes(i).Width) / 2.0F
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
        aiInfoX = (form.ClientSize.Width - aiInfoSize.Width) / 2.0F
        aiInfoY = form.ClientSize.Height * 0.75F
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

    Public Sub UpdateBallPosition(deltaTime As Double, model As GameModel)
        model.BallPos = New PointF(model.BallPos.X + CSng(model.VelX * deltaTime),
                                   model.BallPos.Y + CSng(model.VelY * deltaTime))


        UpdateTrail(model)
    End Sub


    Public Sub UpdateBallBrush(newBrush As SolidBrush)
        ballBrush?.Dispose()
        ballBrush = newBrush
    End Sub

    Public Sub UpdateTrail(model As GameModel)
        If trailPoints Is Nothing Then Exit Sub
        If model Is Nothing Then Return

        trailIndex = (trailIndex + 1) Mod TrailLength
        trailPoints(trailIndex) = model.BallPos

        If trailCount < TrailLength Then
            trailCount += 1
        End If

        If trailCount = 1 Then
            trailSizes(0) = model.BallDiameter
            trailOffsets(0) = 0
            trailAlpha(0) = 16
            Exit Sub
        End If

        For i As Integer = 0 To trailCount - 1
            Dim t As Double = 1.0 - (i / (trailCount - 1))
            trailSizes(i) = CInt(model.BallDiameter * (0.5 + 0.5 * t))
            trailOffsets(i) = CSng(model.BallDiameter * (0.25 * (1 - t)))
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

    Public Sub TriggerWallImpact(location As PointF)
        wallImpactPoint = location
        wallImpactAge = 0
    End Sub

    Public Sub UpdateWallImpact()
        If wallImpactAge < WallImpactDuration Then
            wallImpactAge += 0.015
            If wallImpactAge > WallImpactDuration Then
                wallImpactAge = WallImpactDuration
            End If
        End If
    End Sub

    Public Sub UpdateScore(leftScore As Integer, rightScore As Integer, form As Form, model As GameModel)
        model.ScoreLeft = leftScore
        model.ScoreRight = rightScore
        CacheHUD(form, model)
    End Sub

    Public Sub UpdatePlayerNames(leftName As String,
                                 rightName As String,
                                 form As Form,
                                 model As GameModel)

        model.LeftPlayerName = leftName
        model.RightPlayerName = rightName
        CacheHUD(form, model)

    End Sub

    Public Sub UpdatePaddlePositions(leftY As Single, rightY As Single, size As Size)
        paddleLeft.Y = Math.Max(0, Math.Min(size.Height - paddleHeight, leftY))
        paddleRight.Y = Math.Max(0, Math.Min(size.Height - paddleHeight, rightY))
    End Sub

    Public Sub ToggleKeyboardHints()
        showKeyboardHints = Not showKeyboardHints
    End Sub


    Public Sub SetAIDifficultySelection(sel As Integer)
        aiDifficultySelection = sel
    End Sub


    ' ============================================================
    '   RENDERING PIPELINE
    ' ============================================================
    Public Sub Render(g As Graphics,
                      state As GameState,
                      form As Form,
                      model As GameModel)


        g.CompositingMode = CompositingMode.SourceOver
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit

        Select Case state
            Case GameState.StartScreen
                DrawTrail(g)
                DrawBall(g, model)
                DrawStartScreen(g, model)
                If model.ShowKeyboardHints() Then DrawStartHints(g, form)

            Case GameState.Playing
                DrawGamePlayScreen(g, model)
                If model.ShowKeyboardHints() Then DrawGamePlayHints(g)

            Case GameState.Pause
                DrawTrail(g)
                DrawBall(g, model)
                DrawPaddles(g, model)
                DrawHUD(g, model)
                DrawPauseScreen(g, form, model)
                If model.ShowKeyboardHints() Then DrawPauseHints(g, form)

            Case GameState.EndScreen
                DrawTrail(g)
                DrawBall(g, model)
                DrawHUD(g, model)
                DrawGameOver(g, form, model)
                If model.ShowKeyboardHints() Then DrawGameOverHints(g, form)

            Case GameState.AIDifficulty
                DrawTrail(g)
                DrawBall(g, model)
                DrawAIDifficultyScreen(g, model)
                If model.ShowKeyboardHints() Then DrawAIDifficultyHints(g, form)
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

    Private Sub DrawWallImpact(g As Graphics, model As GameModel)
        If wallImpactAge >= WallImpactDuration Then Return

        Dim progress As Double = wallImpactAge / WallImpactDuration
        Dim size As Single = CSng(model.BallDiameter * (0.35 + 1.25 * progress))
        Dim alpha As Integer = CInt(210 * (1.0 - progress))

        If alpha <= 0 Then Return

        Using impactBrush As New SolidBrush(Color.FromArgb(alpha, Color.White))
            g.FillEllipse(impactBrush,
                          wallImpactPoint.X - size / 2.0F,
                          wallImpactPoint.Y - size / 2.0F,
                          size,
                          size)
        End Using
    End Sub

    Private Sub DrawBall(g As Graphics, model As GameModel)
        g.FillEllipse(ballBrush, model.BallPos.X, model.BallPos.Y, model.BallDiameter, model.BallDiameter)

        Dim shadowPath As New GraphicsPath()
        shadowPath.AddEllipse(
            model.BallPos.X + model.BallDiameter * 0.1F,
            model.BallPos.Y + model.BallDiameter * 0.1F,
            model.BallDiameter,
            model.BallDiameter)

        Using shadowBrush As New PathGradientBrush(shadowPath)
            shadowBrush.CenterColor = Color.FromArgb(75, Color.Black)
            shadowBrush.SurroundColors = {Color.FromArgb(0, Color.Black)}
            g.FillPath(shadowBrush, shadowPath)
        End Using

        shadowPath.Dispose()

        Dim highlightSize As Single = model.BallDiameter * 0.24F
        Dim highlightX As Single = model.BallPos.X + model.BallDiameter * 0.2F
        Dim highlightY As Single = model.BallPos.Y + model.BallDiameter * 0.16F

        Using highlightBrush As New SolidBrush(Color.FromArgb(110, Color.White))
            g.FillEllipse(highlightBrush,
                          highlightX,
                          highlightY,
                          highlightSize,
                          highlightSize)
        End Using
    End Sub

    Private Sub DrawPaddles(g As Graphics, model As GameModel)
        g.FillRectangle(paddleBrush, model.LeftPaddleRect)
        g.FillRectangle(paddleBrush, model.RightPaddleRect)
    End Sub


    Private Sub DrawHUD(g As Graphics, model As GameModel)

        ' Is this the end screen?
        If model.State <> GameState.EndScreen Then
            ' This is NOT the end screen so don't fade

            ' Draw right paddle score
            g.DrawString(model.RightPlayerName, hudLabelFont, labelBrush, rightLabelX, labelY)
            g.DrawString(model.ScoreRight.ToString(), hudScoreFont, scoreBrush, rightScoreX, scoreY)

            ' Draw left paddle score
            g.DrawString(model.LeftPlayerName, hudLabelFont, labelBrush, leftLabelX, labelY)
            g.DrawString(model.ScoreLeft.ToString(), hudScoreFont, scoreBrush, leftScoreX, scoreY)

        Else
            ' This is the end so fade in and out

            Dim scoreAlphaColor As Color = Color.FromArgb(fadeAlpha, Color.White)
            Dim labelAlphaColor As Color = Color.FromArgb(fadeAlpha, Color.Gray)

            ' Who won?
            If model.WinnerIs = 1 Then
                ' Left paddle won

                ' Fade left paddle score in and out
                Using scoreAlphaBrush As New SolidBrush(scoreAlphaColor)
                    g.DrawString(model.ScoreLeft.ToString(), hudScoreFont, scoreAlphaBrush, leftScoreX, scoreY)
                End Using

                ' Fade laft paddle label in and out
                Using labelAlphaBrush As New SolidBrush(labelAlphaColor)
                    g.DrawString(model.LeftPlayerName, hudLabelFont, labelAlphaBrush, leftLabelX, labelY)
                End Using

                ' Draw right paddle - No fade
                g.DrawString(model.RightPlayerName, hudLabelFont, labelBrush, rightLabelX, labelY)
                g.DrawString(model.ScoreRight.ToString(), hudScoreFont, scoreBrush, rightScoreX, scoreY)

            Else
                ' Right paddle won

                ' Fade right paddle score in and out
                Using scoreAlphaBrush As New SolidBrush(scoreAlphaColor)
                    g.DrawString(model.ScoreRight.ToString(), hudScoreFont, scoreAlphaBrush, rightScoreX, scoreY)
                End Using

                ' Fade right paddle label in and out
                Using labelAlphaBrush As New SolidBrush(labelAlphaColor)
                    g.DrawString(model.RightPlayerName, hudLabelFont, labelAlphaBrush, rightLabelX, labelY)
                End Using

                ' Draw left paddle - No fade
                g.DrawString(model.LeftPlayerName, hudLabelFont, labelBrush, leftLabelX, labelY)
                g.DrawString(model.ScoreLeft.ToString(), hudScoreFont, scoreBrush, leftScoreX, scoreY)

            End If


        End If


    End Sub


    Private Sub DrawGamePlayScreen(g As Graphics, model As GameModel)
        DrawTrail(g)
        DrawWallImpact(g, model)
        DrawPaddleTrails(g, model)
        DrawBall(g, model)
        DrawPaddles(g, model)
        DrawHUD(g, model)
    End Sub


    Private Sub DrawStartScreen(g As Graphics, model As GameModel)

        ' --- Title Rendering (fade‑in alpha) ---
        Dim titleColor As Color = Color.FromArgb(fadeAlpha, 255, 255, 255)
        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString("PONG", startTitleFont, titleBrush, startTitleX, startTitleY)
        End Using


        ' --- Menu Option Highlighting (shape fill + outline) ---
        Dim opt1Brush As SolidBrush = If(model.PlayerMode = NumberOfPlayers.OnePlayer, whiteBrush, grayBrush)
        Dim opt2Brush As SolidBrush = If(model.PlayerMode = NumberOfPlayers.TwoPlayers, whiteBrush, grayBrush)

        If model.PlayerMode = NumberOfPlayers.OnePlayer Then
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


        ' --- Menu Option Labels (text) ---
        g.DrawString("1 Player", startMenuFont, opt1Brush, startOption1X, startOption1Y)
        g.DrawString("2 Players", startMenuFont, opt2Brush, startOption2X, startOption2Y)


        ' --- Blink Indicator ("Press SPACE to Start") ---
        If blinkVisible Then
            g.DrawString("Press SPACE to Start",
                         startInfoFont,
                         whiteBrush,
                         startInfoX,
                         startInfoY)
        End If

    End Sub


    Public Sub UpdateStartScreenFX()
        If alphaFadeIn Then
            fadeAlpha += 2
            If fadeAlpha >= 255 Then
                fadeAlpha = 255
                alphaFadeIn = False
            End If
        Else
            fadeAlpha -= 2
            If fadeAlpha <= 64 Then
                fadeAlpha = 64
                alphaFadeIn = True
            End If
        End If

        If blinkStopwatch.ElapsedMilliseconds >= 1000 Then
            blinkVisible = Not blinkVisible
            blinkStopwatch.Restart()
        End If
    End Sub


    Private Sub DrawPauseScreen(g As Graphics,
                                form As Form,
                                model As GameModel)

        ' Dim the game
        g.FillRectangle(dimBrush, form.ClientRectangle)

        g.DrawString(pauseTitle, pauseTitleFont, whiteBrush, pauseTitleX, pauseTitleY)

        Dim radius As Integer = form.ClientSize.Height \ 64   ' kept for consistency, but paths already use it

        Dim gPath As GraphicsPath = pauseResumePath


        For i As Integer = 0 To pauseMenuItems.Length - 1
            Dim text = pauseMenuItems(i)
            Dim x = pauseMenuItemX(i)
            Dim y = pauseMenuItemY(i)


            Select Case i
                Case PauseMenu.ResumeMatch : gPath = pauseResumePath
                Case PauseMenu.NewMatch : gPath = pauseNewPath
                Case PauseMenu.QuitToMenu : gPath = pauseQuitPath
            End Select


            If i = model.PauseMenuSelection Then
                g.FillPath(lightBrush, gPath)
                g.DrawPath(outlinePen, gPath)
            Else
                g.FillPath(darkBrush, gPath)
                g.DrawPath(darkOutlinePen, gPath)
            End If

            Dim brush As SolidBrush = If(i = model.PauseMenuSelection, whiteBrush, grayBrush)
            g.DrawString(text, pauseMenuFont, brush, x, y)

        Next

    End Sub


    Private Sub DrawAIDifficultyScreen(g As Graphics, model As GameModel)
        Dim titleColor As Color = Color.FromArgb(fadeAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString("Difficulty", aiTitleFont, titleBrush, aiTitleX, aiTitleY)
        End Using

        Dim gPath As GraphicsPath = aiEasyPath

        For i As Integer = AIDifficultyLevel.Easy To AIDifficultyLevel.Hard

            'Dim gPath As GraphicsPath = aiEasyPath

            Select Case i
                Case AIDifficultyLevel.Easy : gPath = aiEasyPath
                Case AIDifficultyLevel.Normal : gPath = aiNormalPath
                Case AIDifficultyLevel.Hard : gPath = aiHardPath
            End Select

            If i = model.AISelection Then
                g.FillPath(lightBrush, gPath)
                g.DrawPath(outlinePen, gPath)
            Else
                g.FillPath(darkBrush, gPath)
                g.DrawPath(darkOutlinePen, gPath)
            End If

            Dim brush As SolidBrush =
            If(i = model.AISelection, whiteBrush, grayBrush)

            g.DrawString(aiOptions(i), startMenuFont, brush,
                     aiOptionX(i), aiOptionY(i))
        Next

        If blinkVisible Then
            g.DrawString("Press SPACE to Start", startInfoFont, whiteBrush,
                     aiInfoX, aiInfoY)
        End If
    End Sub

    Private Sub DrawGameOver(g As Graphics, form As Form, model As GameModel)
        Dim titleSize = g.MeasureString(model.WinnerText, gameOverFont)
        Dim titleColor As Color = Color.FromArgb(fadeAlpha, 255, 255, 255)

        Using titleBrush As New SolidBrush(titleColor)
            g.DrawString(model.WinnerText, gameOverFont, titleBrush,
                         CSng((form.ClientSize.Width - titleSize.Width) / 2.0F),
                         CSng(form.ClientSize.Height * 0.35F))
        End Using

        If blinkVisible Then
            Dim info As String = "Press SPACE to Start"
            Dim infoSize = g.MeasureString(info, gameOverInfoFont)

            g.DrawString(info, gameOverInfoFont, whiteBrush,
                         CSng((form.ClientSize.Width - infoSize.Width) / 2.0F),
                         CSng(form.ClientSize.Height * 0.6F))
        End If
    End Sub

    ' ============================================================
    '   KEYBOARD HINT RENDERERS
    ' ============================================================
    Private Sub DrawStartHints(g As Graphics, form As Form)

        ' ====================================
        ' Start Screen Hints - Top/Left Corner 
        ' ====================================
        Dim hintText As String = "1 - One Player   2 - Two Players   Enter - Start Match"
        g.DrawString(hintText,
                     keyboardFont,
                     grayBrush,
                     10,
                     10)

        ' ===================================
        ' Fullscreen Hint - Top/Right Corner 
        ' ===================================
        Dim fsText As String =
            If(form.FormBorderStyle = FormBorderStyle.None,
               "F - Exit Fullscreen",
               "F - Fullscreen")

        Dim fsSize = g.MeasureString(fsText, keyboardFont)

        g.DrawString(fsText,
                     keyboardFont,
                     grayBrush,
                     form.ClientSize.Width - fsSize.Width - 10,
                     10)



        ' ====================================
        ' Hide Hints hint - Bottom/Left Corner 
        ' ====================================
        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
        Dim hideSize = g.MeasureString(hideText, keyboardFont)

        g.DrawString(hideText,
                     keyboardFont,
                     grayBrush,
                     10,
                     form.ClientSize.Height - hideSize.Height - 10)

    End Sub

    Private Sub DrawPauseHints(g As Graphics, form As Form)

        ' ====================================
        ' Pause Screen Hints - Top/Left Corner
        ' ====================================
        Dim hintText As String = "R - Resume Match   N - New Match"
        g.DrawString(hintText,
                     keyboardFont,
                     grayBrush,
                     10,
                     10)

        ' ====================================
        ' Fullscreen Hint - Top/Right Corner
        ' ====================================
        Dim fsText As String =
            If(form.FormBorderStyle = FormBorderStyle.None,
               "F - Exit Fullscreen",
               "F - Fullscreen")
        Dim fsSize = g.MeasureString(fsText, keyboardFont)

        g.DrawString(fsText,
                     keyboardFont,
                     grayBrush,
                     form.ClientSize.Width - fsSize.Width - 10,
                     10)


        ' ====================================
        ' Quit Match Hint - Bottom/Left Corner
        ' ====================================
        Dim quitText As String = "Q - Quit Match"
        Dim quitSize = g.MeasureString(quitText, keyboardFont)

        g.DrawString(quitText,
                     keyboardFont,
                     grayBrush,
                     10,
                     form.ClientSize.Height - quitSize.Height - 10)

        ' ====================================
        ' Hide Hints hint - Bottom/Right Corner
        ' ====================================
        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
        Dim hideSize = g.MeasureString(hideText, keyboardFont)

        g.DrawString(hideText,
                     keyboardFont,
                     grayBrush,
                     form.ClientSize.Width - hideSize.Width - 10,
                     form.ClientSize.Height - hideSize.Height - 10)
    End Sub

    Private Sub DrawGamePlayHints(g As Graphics)
        g.DrawString(hintLeftText, keyboardFont, fsIndicatorBrush, hintLeftX, hintLeftY)
        g.DrawString(hintRightText, keyboardFont, fsIndicatorBrush, hintRightX, hintRightY)

        If Not String.IsNullOrEmpty(pauseText) Then
            g.DrawString(pauseText, keyboardFont, fsIndicatorBrush, pauseX, pauseY)
        End If

        g.DrawString(fpsText, keyboardFont, fsIndicatorBrush, fpsX, fpsY)


    End Sub

    Private Sub DrawAIDifficultyHints(g As Graphics, form As Form)
        Dim hintText As String = "E - Easy   N - Normal   H - Hard   Enter - Start Match"
        g.DrawString(hintText, keyboardFont, grayBrush, 10, 10)

        Dim fsText As String =
            If(form.FormBorderStyle = FormBorderStyle.None,
               "F - Exit Fullscreen",
               "F - Fullscreen")

        Dim fsSize = g.MeasureString(fsText, keyboardFont)

        g.DrawString(fsText, keyboardFont, grayBrush,
                     form.ClientSize.Width - fsSize.Width - 10, 10)

        Dim hideText As String = "CTRL H - Hide Keyboard Hints"
        Dim hideSize = g.MeasureString(hideText, keyboardFont)

        g.DrawString(hideText, keyboardFont, grayBrush,
                     10, form.ClientSize.Height - hideSize.Height - 10)
    End Sub


    Private Sub DrawGameOverHints(g As Graphics, form As Form)
        Dim hintText As String = "Enter - Start New Match"
        g.DrawString(hintText, keyboardFont, grayBrush, 10, 10)

        Dim fsText As String =
            If(form.FormBorderStyle = FormBorderStyle.None,
               "F - Exit Fullscreen",
               "F - Fullscreen")

        Dim fsSize = g.MeasureString(fsText, keyboardFont)

        g.DrawString(fsText, keyboardFont, grayBrush,
                     form.ClientSize.Width - fsSize.Width - 10, 10)

        Dim quitText As String = "CTRL Q - Quit Game"
        Dim quitSize = g.MeasureString(quitText, keyboardFont)

        g.DrawString(quitText, keyboardFont, grayBrush,
                     10, form.ClientSize.Height - quitSize.Height - 10)
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

