'Pong
'This version of the classic game is an example I made for beginning coders.
'I'm making a video to explain the code on my YouTube channel.
'https://www.youtube.com/@codewithjoe6074
'
'MIT License
'Copyright(c) 2023 Joseph Lumbley

'Permission Is hereby granted, free Of charge, to any person obtaining a copy
'of this software And associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
'copies of the Software, And to permit persons to whom the Software Is
'furnished to do so, subject to the following conditions:

'The above copyright notice And this permission notice shall be included In all
'copies Or substantial portions of the Software.

'THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
'IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
'LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
'OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
'SOFTWARE.

Public Class Form1

    Private Enum GameStateEnum
        StartScreen = 0
        Instructions = 1
        Serve = 2
        Playing = 3
        EndScreen = 4
        Pause = 5
    End Enum

    Private Enum ServeStateEnum
        LeftPaddle = 1
        RightPaddle = 2
    End Enum

    Private Enum WinStateEnum
        LeftPaddle = 1
        RightPaddle = 2
    End Enum

    Private Enum DirectionEnum
        UpLeft = 1
        Left = 2
        DownLeft = 3
        UpRight = 4
        Right = 5
        DownRight = 6
    End Enum

    'Ball Data *************************
    Private Ball As Rectangle
    Private BallSpeed As Integer
    Private BallDirection As DirectionEnum
    Private ReadOnly BallMidlineUpPen As New Pen(Color.Green, 7)
    Private ReadOnly BallMidlineDownPen As New Pen(Color.Red, 7)
    '***********************************

    'Left Paddle Data *****************
    Private LeftPaddle As Rectangle
    Private LeftPaddleSpeed As Integer
    Private LeftPaddleScore As Integer
    Private LeftPaddleScoreLocation As Point
    Private ReadOnly LeftPaddleMidlinePen As New Pen(Color.Goldenrod, 7)
    '***********************************

    'Right Paddle Data *****************
    Private RightPaddle As Rectangle
    Private RightPaddleSpeed As Integer
    Private RightPaddleScore As Integer
    Private RightPaddleScoreLocation As Point
    '***********************************

    Private ReadOnly ScoreFont As New Font(FontFamily.GenericSansSerif, 75)
    Private ReadOnly AlineCenterMiddle As New StringFormat

    'One Player Instructions Data *************************
    Private InstructOneLocation As Point
    Private Const InstructOneText As String =
        "Right paddle uses ↑ and ↓ to move." & vbCrLf &
        "Computer plays left paddle." & vbCrLf &
        "First player to 10 points wins." & vbCrLf &
        "Press space bar to start."
    '******************************************************

    'Two Player Instructions Data *************************
    Private InstructTwoLocation As Point
    Private Const InstructTwoText As String =
        "Left paddle uses W and S to move." & vbCrLf &
        "Right paddle uses ↑ and ↓ to move." & vbCrLf &
        "First player to 10 points wins." & vbCrLf &
        "Press space bar to start."
    '******************************************************
    Private ReadOnly InstructionsFont As New Font(FontFamily.GenericSansSerif, 13)
    Private ReadOnly AlineCenter As New StringFormat

    'Title Data *******************************************
    Private Const TitleText As String = "PONG"
    Private TitleLocation As New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 125)
    Private ReadOnly TitleFont As New Font(FontFamily.GenericSansSerif, 50)
    '******************************************************

    'Keyboard Event Data **********************************
    Private SpaceBarDown As Boolean = False
    Private WKeyDown As Boolean = False
    Private SKeyDown As Boolean = False
    Private UpArrowKeyDown As Boolean = False
    Private DownArrowKeyDown As Boolean = False
    Private OneKeyDown As Boolean = False
    Private TwoKeyDown As Boolean = False
    Private PKeyDown As Boolean = False
    '******************************************************

    'Mouse Event Data *************************************
    Private MouseWheelUp As Boolean = False
    Private MouseWheelDown As Boolean = False
    '******************************************************

    'State Data *******************************************
    Private GameState As GameStateEnum = GameStateEnum.StartScreen
    Private Serving As ServeStateEnum = ServeStateEnum.RightPaddle
    Private Winner As WinStateEnum
    Private NumberOfPlayers As Integer = 1
    '******************************************************

    'Counter Data *************************************
    Private FlashCount As Integer = 0
    Private StateCounter As Integer = 0
    '******************************************************

    'Back Buffer Data *************************************
    Private Context As BufferedGraphicsContext
    Private Buffer As BufferedGraphics
    '******************************************************

    'Centerline Data *******************
    Private CenterlineTop As Point
    Private CenterlineBottom As Point
    Private CenterlinePen As New Pen(Color.White, 7)
    '***********************************

    Private LeftPaddleMiddle As Integer = LeftPaddle.Y + LeftPaddle.Height \ 2
    Private BallMiddle As Integer = Ball.Y + Ball.Height \ 2

    Const TopWall As Integer = 0
    Dim BottomWall As Integer = ClientSize.Height

    Dim DrawFlashingText As Boolean = True

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        InitializeGame()

        Timer1.Interval = 16
        Timer1.Start()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        UpdateGame()

        Refresh()

    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

        DrawGame()

        'Show buffer on form.
        Buffer.Render(e.Graphics)

        'Release memory used by buffer.
        Buffer.Dispose()
        Buffer = Nothing

        'Create new buffer.
        Buffer = Context.Allocate(CreateGraphics(), ClientRectangle)

        'Use these settings when drawing to the backbuffer.
        With Buffer.Graphics
            'Bug fix don't change.
            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            'To fix draw string error: "Parameters not valid."
            'I set the compositing mode to source over.
            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
            .CompositingQuality = Drawing2D.CompositingQuality.HighSpeed
            .InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighSpeed
        End With

    End Sub

    Private Sub UpdateGame()

        Select Case GameState

            Case GameStateEnum.EndScreen

                UpdateEndScreen()

            Case GameStateEnum.Playing

                UpdatePlaying()

            Case GameStateEnum.StartScreen

                UpdateStartScreen()

            Case GameStateEnum.Instructions

                UpdateInstructions()

            Case GameStateEnum.Serve

                UpdateServe()

            Case GameStateEnum.Pause

                UpdatePause()

        End Select

    End Sub

    Private Sub DrawGame()

        Buffer.Graphics.Clear(Color.Black)

        Select Case GameState
            Case GameStateEnum.EndScreen

                DrawEndScreen()

            Case GameStateEnum.StartScreen

                DrawStartScreen()

            Case GameStateEnum.Instructions

                DrawInstructions()

            Case GameStateEnum.Playing

                DrawPlaying()

            Case GameStateEnum.Serve

                DrawServe()

            Case GameStateEnum.Pause

                DrawPauseScreen()

        End Select

    End Sub

    Private Sub UpdatePlaying()

        UpdatePaddles()

        UpdateBall()

        UpdateScore()

        CheckforEndGame()

        CheckForPause()

    End Sub

    Private Sub DrawPlaying()

        DrawCenterCourtLine()

        DrawLeftPaddle()

        DrawRightPaddle()

        DrawBall()

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier()

        End If

        DrawLeftPaddleScore()

        DrawRightPaddleScore()

    End Sub

    Private Sub DrawComputerPlayerIdentifier()

        Buffer.Graphics.DrawString("CPU", InstructionsFont, Brushes.White, ClientSize.Width \ 2 \ 2, 20, AlineCenterMiddle)

    End Sub

    Private Sub DrawRightPaddleScore()

        Buffer.Graphics.DrawString(RightPaddleScore, ScoreFont, Brushes.White, RightPaddleScoreLocation, AlineCenterMiddle)

    End Sub

    Private Sub DrawLeftPaddleScore()

        Buffer.Graphics.DrawString(LeftPaddleScore, ScoreFont, Brushes.White, LeftPaddleScoreLocation, AlineCenterMiddle)

    End Sub

    Private Sub UpdatePaddles()

        UpdateLeftPaddle()

        UpdateRightPaddle()

    End Sub
    Private Sub UpdateBall()

        MoveBall()

        CheckPaddleHits()

    End Sub
    Private Sub UpdateScore()

        'Did ball enter left goal zone?
        If Ball.X < 0 Then
            'Yes, ball entered left goal zone.

            'Award point to right paddle.
            RightPaddleScore += 1

            'Change possession of ball to right paddle.
            Serving = ServeStateEnum.RightPaddle

            'Change game state to serve.
            GameState = GameStateEnum.Serve

            PlayScoreSound()

        End If

        'Did ball enter right goal zone?
        If Ball.X + Ball.Width > ClientSize.Width Then
            'Yes, ball entered goal zone.

            'Award a point to left paddle.
            LeftPaddleScore += 1

            'Change possession of ball to left paddle.
            Serving = ServeStateEnum.LeftPaddle

            'Change game state to serve.
            GameState = GameStateEnum.Serve

            PlayScoreSound()

        End If

    End Sub

    Private Sub CheckforEndGame()

        'Did left paddle reach winning score?
        If LeftPaddleScore >= 10 Then
            'Yes, left paddle did reach winning score.

            'Set winner to left paddle.
            Winner = WinStateEnum.LeftPaddle

            'Reset the frame counter.
            FlashCount = 0

            'Change game state to end screen.
            GameState = GameStateEnum.EndScreen

            PlayWinningSound()

        End If

        'Did right paddle reach winning score?
        If RightPaddleScore >= 10 Then
            'Yes, right paddle did reach winning score.

            'Set winner to right paddle.
            Winner = WinStateEnum.RightPaddle

            'Reset frame counter.
            FlashCount = 0

            'Change game state to end screen.
            GameState = GameStateEnum.EndScreen

            PlayWinningSound()

        End If

    End Sub

    Private Sub CheckForPause()

        If PKeyDown = True Then

            PKeyDown = False

            GameState = GameStateEnum.Pause

        End If

    End Sub

    Private Sub UpdateLeftPaddle()

        If NumberOfPlayers = 1 Then

            UpdateLeftPaddleOnePlayer()

        Else

            UpdateLeftPaddleTwoPlayer()

        End If

    End Sub

    Private Sub UpdateRightPaddle()

        UpdateRightPaddleKeyboard()

        UpdateRightPaddleMouse()

    End Sub
    Private Sub MoveBall()

        Select Case BallDirection

            Case DirectionEnum.UpLeft

                MoveBallUpLeft()

            Case DirectionEnum.Left

                MoveBallLeft()

            Case DirectionEnum.DownLeft

                MoveBallDownLeft()

            Case DirectionEnum.UpRight

                MoveBallUpRight()

            Case DirectionEnum.Right

                MoveBallRight()

            Case DirectionEnum.DownRight

                MoveBallDownRight()

        End Select

        BallMiddle = Ball.Y + Ball.Height \ 2

    End Sub

    Private Sub CheckPaddleHits()

        CheckLeftPaddleHit()

        CheckRightPaddleHit()

    End Sub

    Private Sub MoveBallRight()

        Ball.X += BallSpeed

    End Sub

    Private Sub MoveBallLeft()

        Ball.X -= BallSpeed

    End Sub

    Private Sub MoveBallDown()

        Ball.Y += BallSpeed

    End Sub

    Private Sub MoveBallUp()

        Ball.Y -= BallSpeed

    End Sub

    Private Sub MoveBallDownRight()

        MoveBallRight()

        MoveBallDown()

        'Did the ball hit the bottom wall?
        If Ball.Y + Ball.Height > BottomWall Then
            'Yes, the ball hit the bottom wall.

            BallDirection = DirectionEnum.UpRight

            PlayBounceSound()

        End If

    End Sub

    Private Sub MoveBallUpRight()

        MoveBallRight()

        MoveBallUp()

        'Did the ball hit the top wall?
        If Ball.Y < TopWall Then
            'Yes, the ball hit the top wall.

            BallDirection = DirectionEnum.DownRight

            PlayBounceSound()

        End If

    End Sub

    Private Sub MoveBallDownLeft()

        MoveBallLeft()

        MoveBallDown()

        'Did the ball hit the bottom wall?
        If Ball.Y + Ball.Height > BottomWall Then
            'Yes, the ball hit the bottom wall.

            BallDirection = DirectionEnum.UpLeft

            PlayBounceSound()

        End If

    End Sub

    Private Sub MoveBallUpLeft()

        MoveBallLeft()

        MoveBallUp()

        'Did the ball hit the top wall?
        If Ball.Y < TopWall Then
            'Yes, the ball hit the top wall.

            BallDirection = DirectionEnum.DownLeft

            PlayBounceSound()

        End If

    End Sub

    Private Sub UpdateLeftPaddleOnePlayer()
        'In one player mode the left paddle is played
        'by the following algorithm.

        'Is the ball above the paddle?
        If BallMiddle < LeftPaddleMiddle Then
            'Yes, the ball is above the paddle.

            'Move the paddle up.
            LeftPaddle.Y -= LeftPaddleSpeed - 1

            'Is the paddle above the playing field? 
            If LeftPaddle.Y < TopWall Then
                'Yes, the paddle is above the playing field.

                'Push the paddle back on to the playing field.
                LeftPaddle.Y = TopWall

            End If

            LeftPaddleMiddle = LeftPaddle.Y + LeftPaddle.Height \ 2

        End If

        'Is the ball below the paddle?
        If BallMiddle > LeftPaddleMiddle Then
            'Yes, the ball is below the paddle.

            'Move the paddle down.
            LeftPaddle.Y += LeftPaddleSpeed - 1

            'Is the paddle below the playing field?
            If LeftPaddle.Y + LeftPaddle.Height > BottomWall Then
                'Yes, the paddle is below the playing field.

                'Push the paddle back on to the playing field.
                LeftPaddle.Y = BottomWall - LeftPaddle.Height

            End If

            LeftPaddleMiddle = LeftPaddle.Y + LeftPaddle.Height \ 2

        End If

    End Sub

    Private Sub UpdateLeftPaddleTwoPlayer()

        'Is the left player pressing the W key down?
        If WKeyDown = True Then
            'Yes, the left player is pressing the W key down.

            'Move left paddle up.
            LeftPaddle.Y -= LeftPaddleSpeed

            'Is the left paddle above the playing field? 
            If LeftPaddle.Y < TopWall Then
                'Yes, the left paddle is above playing field.

                'Push the left paddle down and back into playing field.
                LeftPaddle.Y = TopWall

            End If
        End If

        'Is the left player pressing the S key down?
        If SKeyDown = True Then
            'Yes, the left player is pressing the S key down.

            'Move left paddle down.
            LeftPaddle.Y += LeftPaddleSpeed

            'Is the left paddle below the playing field?
            If LeftPaddle.Y + LeftPaddle.Height > BottomWall Then
                'Yes, the left paddle is below playing field.

                'Push the left paddle up and back into playing field.
                LeftPaddle.Y = BottomWall - LeftPaddle.Height

            End If

        End If

    End Sub

    Private Sub UpdateRightPaddleKeyboard()

        'Is the right player pressing the up arrow key down?
        If UpArrowKeyDown = True Then
            'Yes, the right player is pressing the up arrow key down.

            'Move right paddle up.
            RightPaddle.Y -= RightPaddleSpeed

            'Is the right paddle above the playing field?
            If RightPaddle.Y < TopWall Then
                'Yes, the right paddle is above playing field.

                'Push the right paddle down and back into playing field.
                RightPaddle.Y = TopWall

            End If

        End If

        'Is the right paddle player pressing the down arrow key down?
        If DownArrowKeyDown = True Then
            'Yes, the right paddle player is pressing the down arrow key down.

            'Move right paddle down.
            RightPaddle.Y += RightPaddleSpeed

            'Is the right paddle below the playing field?
            If RightPaddle.Y + RightPaddle.Height > BottomWall Then
                'Yes, the right paddle is below playing field.

                'Push the right paddle up and back into playing field.
                RightPaddle.Y = BottomWall - RightPaddle.Height

            End If

        End If

    End Sub

    Private Sub UpdateRightPaddleMouse()

        'Is the right paddle player rolling the mouse wheel up?
        If MouseWheelUp = True Then
            'Yes, the right paddle player is rolling the mouse wheel up?

            'Move right paddle up.
            RightPaddle.Y -= RightPaddleSpeed * 4

            'Is the right paddle above the playing field?
            If RightPaddle.Y < TopWall Then
                'Yes, the right paddle is above playing field.

                'Push the right paddle down and back into playing field.
                RightPaddle.Y = TopWall

            End If

        End If

        'Is the right paddle player rolling the mouse wheel down?
        If MouseWheelDown = True Then
            'Yes, the right paddle player is rolling the mouse wheel down?

            'Move right paddle down.
            RightPaddle.Y += RightPaddleSpeed * 4

            'Is the right paddle below the playing field?
            If RightPaddle.Y + RightPaddle.Height > BottomWall Then
                'Yes, the right paddle is below playing field.

                'Push the right paddle up and back into playing field.
                RightPaddle.Y = BottomWall - RightPaddle.Height

            End If

        End If

    End Sub

    Private Sub CheckLeftPaddleHit()

        'Did the ball hit the left paddle?
        If LeftPaddle.IntersectsWith(Ball) Then
            'Yes, the ball hit the left paddle.

            'Push the ball to the paddles right edge.
            Ball.X = LeftPaddle.X + LeftPaddle.Width + 1

            ApplyLeftPaddleEnglishToBall()

            PlayBounceSound()

        End If

    End Sub

    Private Sub ApplyLeftPaddleEnglishToBall()

        If NumberOfPlayers = 2 Then

            'Is the left player holding W key down? 
            If WKeyDown = True Then 'W key moves left paddle up.
                'Yes, the left player is holding W key down.

                'Set the ball direction to up right.
                BallDirection = DirectionEnum.UpRight

                'Is the left player holding S key down?
            ElseIf SKeyDown = True Then 'S key moves left paddle down.
                'Yes, the left player is holding S key down.

                'Set the ball direction to down right.
                BallDirection = DirectionEnum.DownRight

            Else
                'Left the player is not holding either W or S key down.

                'Set the ball direction to right.
                BallDirection = DirectionEnum.Right

            End If

        Else

            Select Case RandomNumber()

                Case 1

                    BallDirection = DirectionEnum.UpRight

                Case 2

                    BallDirection = DirectionEnum.Right

                Case 3

                    BallDirection = DirectionEnum.DownRight

            End Select

        End If

    End Sub

    Private Sub CheckRightPaddleHit()

        If RightPaddle.IntersectsWith(Ball) Then

            Ball.X = RightPaddle.X - Ball.Width + 1

            ApplyRightPaddleEnglishToBall()

            PlayBounceSound()

        Else

            MouseWheelUp = False

            MouseWheelDown = False

        End If

    End Sub

    Private Sub ApplyRightPaddleEnglishToBall()

        If MouseWheelUp = True Then

            BallDirection = DirectionEnum.UpLeft

            MouseWheelUp = False

        ElseIf MouseWheelDown = True Then

            BallDirection = DirectionEnum.DownLeft

            MouseWheelDown = False

        ElseIf UpArrowKeyDown = True Then

            BallDirection = DirectionEnum.UpLeft

        ElseIf DownArrowKeyDown = True Then

            BallDirection = DirectionEnum.DownLeft

        Else

            BallDirection = DirectionEnum.Left

        End If

    End Sub

    Private Sub InitializeGame()

        'Width = 640
        'Height = 480

        WindowState = FormWindowState.Maximized

        Text = "Code with Joe - Pong"

        SetStyle(ControlStyles.AllPaintingInWmPaint, True) ' True is better
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True) ' True is better

        LayoutInstructions()

        AlineCenter.Alignment = StringAlignment.Center
        AlineCenterMiddle.Alignment = StringAlignment.Center
        AlineCenterMiddle.LineAlignment = StringAlignment.Center

        CenterlinePen.DashStyle = Drawing2D.DashStyle.Dash


        InitializePaddles()

        InitializeBall()

        Context = BufferedGraphicsManager.Current
        Context.MaximumBuffer = New Size(Width + 1, Height + 1)
        Buffer = Context.Allocate(CreateGraphics(), New Rectangle(0, 0, Width, Height))

    End Sub

    Private Sub InitializePaddles()

        LeftPaddle.Width = 25
        LeftPaddle.Height = 100
        LeftPaddle.X = 20
        LeftPaddle.Y = ClientSize.Height \ 2 - LeftPaddle.Height \ 2 'Center vertically
        LeftPaddleSpeed = 10

        RightPaddle.Width = 25
        RightPaddle.Height = 100
        RightPaddle.X = ClientSize.Width - RightPaddle.Width - 20 'Aline right 20 pix padding
        RightPaddle.Y = ClientSize.Height \ 2 - RightPaddle.Height \ 2 'Center vertically
        RightPaddleSpeed = 10

    End Sub

    Private Sub InitializeBall()

        Ball.Width = 25
        Ball.Height = 25

        PlaceBallCenterCourt()

        BallSpeed = 10

    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        DoKeyDown(e)

    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp

        DoKeyUp(e)

    End Sub

    Private Sub Form1_MouseWheel(ByVal sender As Object, ByVal e As MouseEventArgs) Handles MyBase.MouseWheel

        'Is the player rolling the mouse wheel up?
        If e.Delta > 0 Then
            'Yes, the player is rolling the mouse wheel up.

            MouseWheelDown = False

            MouseWheelUp = True

        Else
            'No, the player is not rolling the mouse wheel up.

            MouseWheelUp = False

            MouseWheelDown = True

        End If

    End Sub

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove

        'Are the players playing?
        If GameState = GameStateEnum.Playing Then
            'Yes, the players are playing.

            'Move mouse pointer off the play area.
            Cursor.Position = PointToScreen(New Point(ClientRectangle.Width - 3, ClientRectangle.Height \ 2))

        End If

    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        If WindowState = FormWindowState.Minimized Then

            If GameState = GameStateEnum.Playing Then

                GameState = GameStateEnum.Pause

            End If

        End If

        LayoutGame()

    End Sub

    Private Sub DoKeyDown(e As KeyEventArgs)
        'When the player pushes a key down do the following...

        Select Case e.KeyCode

                'Did player push space bar?
            Case Keys.Space
                'Yes, player did push space bar.

                SpaceBarDown = True

                'Did player push W key?
            Case Keys.W
                'Yes, player did push W key.

                WKeyDown = True

                'Did player push S key?
            Case Keys.S
                'Yes, player did push S key.

                SKeyDown = True

                'Did player push up arrow key?
            Case Keys.Up
                'Yes, player did push up arrow key.

                UpArrowKeyDown = True

                'Did player push down arrow key?
            Case Keys.Down
                'Yes, player did push down arrow key.

                DownArrowKeyDown = True

                'Did player push down the 1 key?
            Case Keys.D1
                'Yes, player did push down the 1 key.

                OneKeyDown = True

                'Did player push down the 1 key on number pad?
            Case Keys.NumPad1
                'Yes, player did push down the 1 key on number pad.

                OneKeyDown = True

                'Did player push down the 2 key?
            Case Keys.D2
                'Yes, player did push down the 2 key.

                TwoKeyDown = True

                'Did player push down the 2 key on number pad?
            Case Keys.NumPad2
                'Yes, player did push down the 2 key on number pad.

                TwoKeyDown = True

                'Did player push down the p key on number pad?
            Case Keys.P
                'Yes, player did push down the p key on number pad.
                PKeyDown = True

        End Select

    End Sub

    Private Sub DoKeyUp(e As KeyEventArgs)
        'When the player lets a key up do the following...

        Select Case e.KeyCode

                'Did the player let the space bar up?
            Case Keys.Space
                'Yes, the player let the space bar up?

                SpaceBarDown = False

            Case Keys.W

                WKeyDown = False

            Case Keys.S

                SKeyDown = False

            Case Keys.Up

                UpArrowKeyDown = False

            Case Keys.Down

                DownArrowKeyDown = False

            Case Keys.D1 'The 1 key.

                OneKeyDown = False

            Case Keys.NumPad1 'The 1 key on number pad.

                OneKeyDown = False

            Case Keys.D2  'The 2 key.

                TwoKeyDown = False

            Case Keys.NumPad2 'The 2 key on number pad.

                TwoKeyDown = False

                'Did player let the p key up?
            Case Keys.P
                'Yes, player did let the p key up.
                PKeyDown = False

        End Select

    End Sub

    Private Sub UpdatePause()

        If PKeyDown = True Then

            PKeyDown = False

            GameState = GameStateEnum.Playing

        End If

    End Sub

    Private Sub UpdateInstructions()

        If SpaceBarDown = True Then

            PlayBounceSound()

            GameState = GameStateEnum.Serve

        End If

    End Sub

    Private Sub UpdateStartScreen()

        If OneKeyDown = True Then

            PlayBounceSound()

            NumberOfPlayers = 1

            GameState = GameStateEnum.Instructions

        End If

        If TwoKeyDown = True Then

            NumberOfPlayers = 2

            GameState = GameStateEnum.Instructions

        End If

    End Sub

    Private Sub UpdateEndScreen()

        UpdateFlashingText()

        StateCounter += 1

        If StateCounter >= 300 Then

            StateCounter = 0

            LeftPaddleScore = 0

            RightPaddleScore = 0

            LeftPaddle.Y = ClientSize.Height \ 2 - LeftPaddle.Height \ 2 'Center verticaly

            RightPaddle.X = ClientSize.Width - RightPaddle.Width - 20 'Aline right 20 pix padding
            RightPaddle.Y = ClientSize.Height \ 2 - RightPaddle.Height \ 2 'Center verticaly

            PlaceBallCenterCourt()

            'Ball.X = ClientSize.Width \ 2 - Ball.Width \ 2 'Center horizontally
            'Ball.Y = ClientSize.Height \ 2 - Ball.Height \ 2 'Center vertically

            GameState = GameStateEnum.StartScreen

        End If

    End Sub

    Private Sub UpdateFlashingText()
        'This algorithm controls the rate of flash for text.

        'Advance the frame counter.
        FlashCount += 1

        'Draw text for 20 frames.
        If FlashCount <= 20 Then

            DrawFlashingText = True

        Else

            DrawFlashingText = False

        End If

        'Dont draw text for the next 20 frames.
        If FlashCount >= 40 Then

            'Repete
            FlashCount = 0

        End If

    End Sub

    Private Sub UpdateServe()

        PlaceBallCenterCourt()

        If Serving = ServeStateEnum.RightPaddle Then

            ServeRightPaddle()

        Else

            ServeLeftPaddle()

        End If

        GameState = GameStateEnum.Playing

    End Sub

    Private Sub PlaceBallCenterCourt()

        'Place ball center court.
        Ball.X = ClientSize.Width \ 2 - Ball.Width \ 2 'Center horizontally
        Ball.Y = ClientSize.Height \ 2 - Ball.Height \ 2 'Center vertically

    End Sub

    Private Sub ServeLeftPaddle()

        Select Case RandomNumber()

            Case 1

                BallDirection = DirectionEnum.UpRight

            Case 2

                BallDirection = DirectionEnum.Right

            Case 3

                BallDirection = DirectionEnum.DownRight

        End Select

    End Sub

    Private Sub ServeRightPaddle()

        Select Case RandomNumber()

            Case 1

                BallDirection = DirectionEnum.UpLeft

            Case 2

                BallDirection = DirectionEnum.Left

            Case 3

                BallDirection = DirectionEnum.DownLeft

        End Select

    End Sub

    Private Sub DrawPauseScreen()

        DrawCenterCourtLine()

        DrawBall()

        DrawLeftPaddle()

        DrawRightPaddle()

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier()

        End If

        DrawLeftPaddleScore()

        DrawRightPaddleScore()

        'Draw paused text.
        Buffer.Graphics.DrawString("Paused", TitleFont, Brushes.White, ClientSize.Width \ 2, ClientSize.Height \ 2, AlineCenterMiddle)

    End Sub

    Private Sub DrawEndScreen()

        DrawCenterCourtLine()

        DrawEndScores()

        DrawLeftPaddle()

        DrawRightPaddle()

        DrawBall()

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier()

        End If

    End Sub

    Private Sub DrawBall()

        Buffer.Graphics.FillRectangle(Brushes.White, Ball)

    End Sub

    Private Sub DrawRightPaddle()

        Buffer.Graphics.FillRectangle(Brushes.White, RightPaddle)

    End Sub

    Private Sub DrawLeftPaddle()

        Buffer.Graphics.FillRectangle(Brushes.White, LeftPaddle)

    End Sub

    Private Sub DrawEndScores()

        'Did the left paddle win?
        If Winner = WinStateEnum.LeftPaddle Then
            'Yes, the left paddle won.

            'Flash the winning score.
            If DrawFlashingText = True Then

                DrawLeftPaddleScore()

            End If

        Else
            'No, the left paddle didn't win.

            DrawLeftPaddleScore()

        End If

        'Did the right paddle win?
        If Winner = WinStateEnum.RightPaddle Then
            'Yes, the right paddle won.


            'Flash the winning score.
            If DrawFlashingText = True Then

                DrawRightPaddleScore()

            End If

        Else
            'No, the right paddle didn't win.

            DrawRightPaddleScore()

        End If

    End Sub

    Private Sub DrawCenterCourtLine()

        Buffer.Graphics.DrawLine(CenterlinePen, CenterlineTop, CenterlineBottom)

    End Sub

    Private Sub DrawServe()

        DrawCenterCourtLine()

        DrawLeftPaddleScore()

        DrawRightPaddleScore()

        DrawLeftPaddle()

        DrawRightPaddle()

        DrawBall()

    End Sub

    Private Sub DrawStartScreen()

        DrawTitle()

        'Draw instructions.
        Buffer.Graphics.DrawString("Press 1 or 2 for the number of players.", InstructionsFont, Brushes.White, ClientSize.Width \ 2, ClientSize.Height \ 2 - 15, AlineCenter)

    End Sub

    Private Sub DrawInstructions()

        If NumberOfPlayers = 1 Then

            DrawTitle()

            'Draw one player instructions.
            Buffer.Graphics.DrawString(InstructOneText, InstructionsFont, Brushes.White, InstructOneLocation, AlineCenter)

        Else

            DrawTitle()

            'Draw two player instructions.
            Buffer.Graphics.DrawString(InstructTwoText, InstructionsFont, Brushes.White, InstructTwoLocation, AlineCenter)

        End If

    End Sub

    Private Sub DrawTitle()

        Buffer.Graphics.DrawString(TitleText, TitleFont, Brushes.White, TitleLocation, AlineCenter)

    End Sub

    Private Sub LayoutGame()

        'Center the left paddle vertically in the forms client area.
        LeftPaddle.Y = ClientSize.Height \ 2 - LeftPaddle.Height \ 2

        'Center the right paddle vertically in the forms client area.
        RightPaddle.Y = ClientSize.Height \ 2 - RightPaddle.Height \ 2 'Center verticaly

        'Aline the right paddle along the right side of the form client area allow 20 pixels padding.
        RightPaddle.X = ClientSize.Width - RightPaddle.Width - 20

        CenterCourtLine()

        BottomWall = ClientSize.Height

        TitleLocation = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 125)

        LeftPaddleScoreLocation = New Point(ClientSize.Width \ 2 \ 2, 100)

        RightPaddleScoreLocation = New Point(ClientSize.Width - (ClientSize.Width \ 4), 100)

        LayoutInstructions()

    End Sub

    Private Sub CenterCourtLine()

        'Centers the court line in the client area of our form.
        CenterlineTop = New Point(ClientSize.Width \ 2, 0)
        CenterlineBottom = New Point(ClientSize.Width \ 2, ClientSize.Height)

    End Sub

    Private Sub LayoutInstructions()

        InstructOneLocation = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 15)

        InstructTwoLocation = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 15)

    End Sub

    Private Shared Sub PlayBounceSound()

        My.Computer.Audio.Play(Application.StartupPath & "bounce.wav")

    End Sub

    Private Shared Sub PlayScoreSound()

        My.Computer.Audio.Play(Application.StartupPath & "score.wav")

    End Sub

    Private Shared Sub PlayWinningSound()

        My.Computer.Audio.Play(Application.StartupPath & "winning.wav")

    End Sub

    Private Shared Function RandomNumber() As Integer

        'Initialize random-number generator.
        Randomize()

        'Generate random number between 1 and 3.
        Return CInt(Int((3 * Rnd()) + 1))

    End Function

    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)

        'Intentionally left blank. Do not remove.

    End Sub

End Class

