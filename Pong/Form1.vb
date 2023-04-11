'Pong
'In this app we remake the classic ping pong game.
'This version is resizable, pausable and has a computer player.
'Supports keyboard, mouse, Xbox and PlayStation controllers.
'Written in VB.NET this year 2023 actually works on Windows 10 64-Bit.
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

Imports System.Runtime.InteropServices

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
    Private Const BallSpeed As Integer = 10
    Private BallDirection As DirectionEnum
    Private ReadOnly BallMidlineUpPen As New Pen(Color.Green, 7)
    Private ReadOnly BallMidlineDownPen As New Pen(Color.Red, 7)
    Private BallMiddle As Integer = Ball.Y + Ball.Height \ 2
    '***********************************

    'Left Paddle Data *****************
    Private LeftPaddle As Rectangle
    Private Const LeftPaddleSpeed As Integer = 10
    Private LeftPaddleScore As Integer
    Private LPadScoreLocation As Point
    Private ReadOnly LeftPaddleMidlinePen As New Pen(Color.Goldenrod, 7)
    Private LeftPaddleMiddle As Integer = LeftPaddle.Y + LeftPaddle.Height \ 2
    '***********************************

    'Right Paddle Data *****************
    Private RightPaddle As Rectangle
    Private Const RightPaddleSpeed As Integer = 10
    Private RightPaddleScore As Integer
    Private RPadScoreLocation As Point
    '***********************************

    Private ReadOnly ScoreFont As New Font(FontFamily.GenericSansSerif, 75)
    Private ReadOnly AlineCenterMiddle As New StringFormat

    Private InstructStartLocation As Point
    Private ReadOnly InstructStartText As String = vbCrLf &
        "One player:  A  □   Two players:  B  X"

    'One Player Instructions Data *************************
    Private InstructOneLocation As Point
    Private Const InstructOneText As String = vbCrLf &
        "Start:  B  X" & vbCrLf & vbCrLf &
        "Computer plays left paddle." & vbCrLf &
        "Right paddle use ↑  ↓ to move." & vbCrLf &
        "First player to 10 points wins." & vbCrLf & vbCrLf &
        "Pause:  Start  RTrigger  P" & vbCrLf &
        "Resume:  A  □  P"
    '******************************************************

    'Two Player Instructions Data *************************
    Private InstructTwoLocation As Point
    Private Const InstructTwoText As String = vbCrLf &
        "Start:  A  □" & vbCrLf & vbCrLf &
        "Left paddle use  W  S or DPad:  ↑  ↓  to move." & vbCrLf &
        "Right paddle use  ↑  ↓  to move." & vbCrLf &
        "First player to 10 points wins." & vbCrLf & vbCrLf &
        "Pause:  Start  RTrigger  P" & vbCrLf &
        "Resume:  A  □  P"
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
    Private AKeyDown As Boolean = False
    Private BKeyDown As Boolean = False
    Private XKeyDown As Boolean = False
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
    Private EndScreenCounter As Integer = 0
    '******************************************************

    'Back Buffer Data *************************************
    Private Context As BufferedGraphicsContext
    Private Buffer As BufferedGraphics
    '******************************************************

    'Centerline Data *******************
    Private CenterlineTop As Point
    Private CenterlineBottom As Point
    Private ReadOnly CenterlinePen As New Pen(Color.White, 7)
    '***********************************

    'Wall Data ***************************************
    Private Const TopWall As Integer = 0
    Private BottomWall As Integer = ClientSize.Height
    '*************************************************

    Private DrawFlashingText As Boolean = True

    'Joystick Data**************************************************************************************************
    Private Declare Function joyGetPosEx Lib "winmm.dll" (ByVal uJoyID As Integer, ByRef pControllerData As JOYINFOEX) As Integer

    <StructLayout(LayoutKind.Sequential)> Public Structure JOYINFOEX
        Public dwSize As Integer
        Public dwFlags As Integer
        Public dwXpos As Integer
        Public dwYpos As Integer
        Public dwZpos As Integer 'Xbox: Trigger
        Public dwRpos As Integer
        Public dwUpos As Integer
        Public dwVpos As Integer
        Public dwButtons As Integer
        Public dwButtonNumber As Integer
        Public dwPOV As Integer 'D-Pad
        Public dwReserved1 As Integer
        Public dwReserved2 As Integer
    End Structure

    Private AControllerID As Integer = -1
    Private AControllerDown As Boolean = False
    Private AControllerUp As Boolean = False
    Private AControllerLeft As Boolean = False
    Private AControllerRight As Boolean = False
    Private AControllerA As Boolean = False
    Private AControllerB As Boolean = False
    Private AControllerStart As Boolean = False
    Private AControllerX As Boolean = False
    Private AControllerTsUp As Boolean = False
    Private AControllerTsDown As Boolean = False

    Private BControllerID As Integer = -1
    Private BControllerDown As Boolean = False
    Private BControllerUp As Boolean = False
    Private BControllerLeft As Boolean = False
    Private BControllerRight As Boolean = False
    Private BControllerA As Boolean = False
    Private BControllerB As Boolean = False
    Private BControllerStart As Boolean = False
    Private BControllerX As Boolean = False
    Private BControllerTsUp As Boolean = False
    Private BControllerTsDown As Boolean = False

    Private Const NeutralStart = 21845
    Private Const NeutralEnd = 43690

    Private ControllerData As JOYINFOEX
    Private ControllerNumber As Long = 0
    '***************************************************************************************************
    Private ClientCenter As New Point(ClientSize.Width \ 2, ClientSize.Height \ 2)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        InitializeGame()

    End Sub

    Private Sub InitializeGame()

        InitializeControllerData()

        InitializeForm()

        LayoutInstructions()

        InitializeStringAlinement()

        CenterlinePen.DashStyle = Drawing2D.DashStyle.Dash

        InitializePaddles()

        InitializeBall()

        InitializeGraphicsBuffer()

        Timer1.Interval = 16 '16ms = 1000 milliseconds \ 60 frames per second
        Timer1.Start()

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        UpdateGame()

        Refresh() 'Calls OnPaint Event

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

    Private Sub InitializeGraphicsBuffer()

        Context = BufferedGraphicsManager.Current

        Context.MaximumBuffer = Screen.PrimaryScreen.WorkingArea.Size

        Buffer = Context.Allocate(CreateGraphics(), ClientRectangle)

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

    Private Sub UpdatePlaying()

        GetControllerData()

        UpdatePaddles()

        UpdateBall()

        UpdateScore()

        CheckforEndGame()

        CheckForPause()

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

        GetControllerData()

        If AControllerStart = True Or BControllerStart = True Then

            AControllerStart = False
            BControllerStart = False

            GameState = GameStateEnum.Pause

            PlayBounceSound()

        End If

        If PKeyDown = True Then

            PKeyDown = False

            GameState = GameStateEnum.Pause

        End If

    End Sub

    Private Sub UpdateLeftPaddle()


        If NumberOfPlayers = 1 Then

            UpdateLeftPaddleOnePlayer()

        Else

            UpdateLeftPaddleJoystick()

            UpdateLeftPaddleTwoPlayer()

        End If

    End Sub

    Private Sub UpdateRightPaddle()

        UpdateRightPaddleJoystick()

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
            'Yes, the right paddle player is rolling the mouse wheel up.

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
            'Yes, the right paddle player is rolling the mouse wheel down.

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

    Private Sub UpdateRightPaddleJoystick()

        If NumberOfPlayers = 2 Then

            If BControllerDown = True Then

                'Move right paddle down.
                RightPaddle.Y += RightPaddleSpeed

                'Is the right paddle below the playing field?
                If RightPaddle.Y + RightPaddle.Height > BottomWall Then
                    'Yes, the right paddle is below playing field.

                    'Push the right paddle up and back into playing field.
                    RightPaddle.Y = BottomWall - RightPaddle.Height

                End If

            End If

            If BControllerUp = True Then

                'Move right paddle up.
                RightPaddle.Y -= RightPaddleSpeed

                'Is the right paddle above the playing field?
                If RightPaddle.Y < TopWall Then
                    'Yes, the right paddle is above playing field.

                    'Push the right paddle down and back into playing field.
                    RightPaddle.Y = TopWall

                End If

            End If

            If BControllerTsDown = True Then

                'Move right paddle down.
                RightPaddle.Y += RightPaddleSpeed

                'Is the right paddle below the playing field?
                If RightPaddle.Y + RightPaddle.Height > BottomWall Then
                    'Yes, the right paddle is below playing field.

                    'Push the right paddle up and back into playing field.
                    RightPaddle.Y = BottomWall - RightPaddle.Height

                End If

            End If

            If BControllerTsUp = True Then

                'Move right paddle up.
                RightPaddle.Y -= RightPaddleSpeed

                'Is the right paddle above the playing field?
                If RightPaddle.Y < TopWall Then
                    'Yes, the right paddle is above playing field.

                    'Push the right paddle down and back into playing field.
                    RightPaddle.Y = TopWall

                End If

            End If

        Else

            If AControllerDown = True Then

                'Move right paddle down.
                RightPaddle.Y += RightPaddleSpeed

                'Is the right paddle below the playing field?
                If RightPaddle.Y + RightPaddle.Height > BottomWall Then
                    'Yes, the right paddle is below playing field.

                    'Push the right paddle up and back into playing field.
                    RightPaddle.Y = BottomWall - RightPaddle.Height

                End If

            End If

            If AControllerUp = True Then

                'Move right paddle up.
                RightPaddle.Y -= RightPaddleSpeed

                'Is the right paddle above the playing field?
                If RightPaddle.Y < TopWall Then
                    'Yes, the right paddle is above playing field.

                    'Push the right paddle down and back into playing field.
                    RightPaddle.Y = TopWall

                End If

            End If

            If AControllerTsDown = True Then

                'Move right paddle down.
                RightPaddle.Y += RightPaddleSpeed

                'Is the right paddle below the playing field?
                If RightPaddle.Y + RightPaddle.Height > BottomWall Then
                    'Yes, the right paddle is below playing field.

                    'Push the right paddle up and back into playing field.
                    RightPaddle.Y = BottomWall - RightPaddle.Height

                End If

            End If


            If AControllerTsUp = True Then

                'Move right paddle up.
                RightPaddle.Y -= RightPaddleSpeed

                'Is the right paddle above the playing field?
                If RightPaddle.Y < TopWall Then
                    'Yes, the right paddle is above playing field.

                    'Push the right paddle down and back into playing field.
                    RightPaddle.Y = TopWall

                End If

            End If

        End If

    End Sub

    Private Sub UpdateLeftPaddleJoystick()

        If AControllerDown = True Then

            'Move left paddle down.
            LeftPaddle.Y += LeftPaddleSpeed

            'Is the left paddle below the playing field?
            If LeftPaddle.Y + LeftPaddle.Height > BottomWall Then
                'Yes, the left paddle is below playing field.

                'Push the left paddle up and back into playing field.
                LeftPaddle.Y = BottomWall - LeftPaddle.Height

            End If

        End If

        If AControllerUp = True Then

            'Move left paddle up.
            LeftPaddle.Y -= LeftPaddleSpeed

            'Is the left paddle above the playing field? 
            If LeftPaddle.Y < TopWall Then
                'Yes, the left paddle is above playing field.

                'Push the left paddle down and back into playing field.
                LeftPaddle.Y = TopWall

            End If

        End If

        If AControllerTsDown = True Then

            'Move left paddle down.
            LeftPaddle.Y += LeftPaddleSpeed

            'Is the left paddle below the playing field?
            If LeftPaddle.Y + LeftPaddle.Height > BottomWall Then
                'Yes, the left paddle is below playing field.

                'Push the left paddle up and back into playing field.
                LeftPaddle.Y = BottomWall - LeftPaddle.Height

            End If

        End If

        If AControllerTsUp = True Then

            'Move left paddle up.
            LeftPaddle.Y -= LeftPaddleSpeed

            'Is the left paddle above the playing field? 
            If LeftPaddle.Y < TopWall Then
                'Yes, the left paddle is above playing field.

                'Push the left paddle down and back into playing field.
                LeftPaddle.Y = TopWall

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

    Private Sub CheckRightPaddleHit()

        If RightPaddle.IntersectsWith(Ball) Then

            Ball.X = RightPaddle.X - (Ball.Width + 5)

            ApplyRightPaddleEnglishToBall()

            PlayBounceSound()

        Else

            MouseWheelUp = False

            MouseWheelDown = False

        End If

    End Sub

    Private Sub ApplyLeftPaddleEnglishToBall()

        If NumberOfPlayers = 2 Then
            'The human player must manualy apply english to the ball
            'by pressing the controller ↑ ↓ buttons or the W S keys.

            If AControllerUp = True Then

                BallDirection = DirectionEnum.UpRight

            ElseIf AControllerDown = True Then

                BallDirection = DirectionEnum.DownRight

            ElseIf AControllerTsUp = True Then

                BallDirection = DirectionEnum.UpRight

            ElseIf AControllerTsDown = True Then

                BallDirection = DirectionEnum.DownRight

                'Is the left player holding W key down? 
            ElseIf WKeyDown = True Then 'W key moves left paddle up.
                'Yes, the left player is holding W key down.

                'Set the ball direction to up right.
                BallDirection = DirectionEnum.UpRight

                'Is the left player holding S key down?
            ElseIf SKeyDown = True Then 'S key moves left paddle down.
                'Yes, the left player is holding S key down.

                'Set the ball direction to down right.
                BallDirection = DirectionEnum.DownRight

            Else
                'The left player is not holding either W or S key down.
                'The left player is not holding the controller ↑ ↓ buttons down.

                'Set the ball direction to right.
                BallDirection = DirectionEnum.Right

            End If

        Else
            'For the computer player random english.
            'This makes the game more interesting.

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

    Private Sub ApplyRightPaddleEnglishToBall()

        If NumberOfPlayers = 2 Then

            If BControllerUp = True Then

                BallDirection = DirectionEnum.UpLeft

            ElseIf BControllerDown = True Then

                BallDirection = DirectionEnum.DownLeft

            ElseIf BControllerTsUp = True Then

                BallDirection = DirectionEnum.UpLeft

            ElseIf BControllerTsDown = True Then

                BallDirection = DirectionEnum.DownLeft

            ElseIf MouseWheelUp = True Then

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

        Else

            If AControllerUp = True Then

                BallDirection = DirectionEnum.UpLeft

            ElseIf AControllerDown = True Then

                BallDirection = DirectionEnum.DownLeft

            ElseIf AControllerTsUp = True Then

                BallDirection = DirectionEnum.UpLeft

            ElseIf AControllerTsDown = True Then

                BallDirection = DirectionEnum.DownLeft

            ElseIf MouseWheelUp = True Then

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

        End If

    End Sub

    Private Sub UpdatePause()

        GetControllerData()

        If AControllerA = True Or BControllerA = True Then

            GameState = GameStateEnum.Playing

        End If

        If PKeyDown = True Or AKeyDown = True Then

            PKeyDown = False

            GameState = GameStateEnum.Playing

        End If

    End Sub

    Private Sub UpdateInstructions()

        GetControllerData()

        If NumberOfPlayers = 1 Then

            If AControllerB = True Or BControllerB = True Then

                GameState = GameStateEnum.Serve

                PlayBounceSound()

            End If

            If AControllerX = True Or BControllerX = True Then

                GameState = GameStateEnum.Serve

                PlayBounceSound()

            End If

            If SpaceBarDown = True Or BKeyDown = True Or XKeyDown = True Then

                GameState = GameStateEnum.Serve

                PlayBounceSound()

            End If

        Else

            If AControllerA = True Or BControllerA = True Then

                GameState = GameStateEnum.Serve

                PlayBounceSound()

            End If

            If SpaceBarDown = True Or AKeyDown = True Then

                GameState = GameStateEnum.Serve

                PlayBounceSound()

            End If

        End If

    End Sub

    Private Sub UpdateStartScreen()

        GetControllerData()

        InstructStartLocation = New Point(ClientSize.Width \ 2, (ClientSize.Height \ 2) - 15)

        If AControllerA = True Or BControllerA = True Then

            NumberOfPlayers = 1

            GameState = GameStateEnum.Instructions

            PlayBounceSound()

        End If

        If AControllerB = True Or BControllerB = True Then

            NumberOfPlayers = 2

            GameState = GameStateEnum.Instructions

            PlayBounceSound()

        End If

        If AControllerX = True Or BControllerX = True Then

            NumberOfPlayers = 2

            GameState = GameStateEnum.Instructions

            PlayBounceSound()

        End If

        If OneKeyDown = True Or AKeyDown = True Then

            NumberOfPlayers = 1

            GameState = GameStateEnum.Instructions

            PlayBounceSound()

        End If

        If TwoKeyDown = True Or BKeyDown = True Or XKeyDown = True Then

            NumberOfPlayers = 2

            GameState = GameStateEnum.Instructions

            PlayBounceSound()

        End If

    End Sub

    Private Sub UpdateEndScreen()

        UpdateFlashingText()

        EndScreenCounter += 1

        If EndScreenCounter >= 300 Then

            ResetGame()

        End If

    End Sub

    Private Sub ResetGame()

        EndScreenCounter = 0

        LeftPaddleScore = 0

        RightPaddleScore = 0

        LeftPaddle.Y = ClientSize.Height \ 2 - LeftPaddle.Height \ 2 'Center verticaly

        RightPaddle.X = ClientSize.Width - RightPaddle.Width - 20 'Aline right 20 pix padding
        RightPaddle.Y = ClientSize.Height \ 2 - RightPaddle.Height \ 2 'Center verticaly

        PlaceBallCenterCourt()

        GameState = GameStateEnum.StartScreen

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

        Ball.Location = New Point((ClientSize.Width \ 2) - (Ball.Width \ 2), (ClientSize.Height \ 2) - (Ball.Height \ 2))

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

        DrawLeftPaddle()

        DrawRightPaddle()

        DrawBall()

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier()

        End If

        DrawLeftPaddleScore()

        DrawRightPaddleScore()

        DrawPausedText()

    End Sub

    Private Sub DrawEndScreen()

        DrawCenterCourtLine()

        DrawLeftPaddle()

        DrawRightPaddle()

        DrawBall()

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier()

        End If

        DrawEndScores()

    End Sub

    Private Sub DrawServe()

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

    Private Sub DrawStartScreen()

        DrawTitle()

        DrawStartScreenInstructions()

    End Sub

    Private Sub DrawInstructions()

        If NumberOfPlayers = 1 Then

            DrawTitle()

            'Draw one player instructions.
            Buffer.Graphics.DrawString(InstructOneText,
            InstructionsFont, Brushes.White, InstructOneLocation, AlineCenter)

        Else

            DrawTitle()

            'Draw two player instructions.
            Buffer.Graphics.DrawString(InstructTwoText,
            InstructionsFont, Brushes.White, InstructTwoLocation, AlineCenter)

        End If

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

    Private Sub InitializeControllerData()

        ControllerData.dwSize = 64

        ControllerData.dwFlags = &HFF ' All information

    End Sub

    Private Sub GetControllerData()

        For ControllerNumber = 0 To 15 'Up to 16 controllers

            Try

                'Did an error happen when we called joyGetPosEx?
                If joyGetPosEx(ControllerNumber, ControllerData) = 0 Then
                    'No errors.

                    UpdateDPadPosition()

                    UpdateButtonPosition()

                    UpdateLeftThumbstickPosition()

                    AssignController()

                Else
                    'Yes, we have an error.

                    UnassignController()

                End If

            Catch ex As Exception

                MsgBox(ex.ToString)

                Exit Sub

            End Try

        Next

    End Sub

    Private Sub UpdateButtonPosition()
        'The range of buttons is 0 to 255.
        '         XBox / PlayStation
        'What buttons are down?
        Select Case ControllerData.dwButtons
            Case 0 'All the buttons are up.
                If AControllerID = ControllerNumber Then
                    AControllerStart = False
                    AControllerA = False
                    AControllerB = False
                    AControllerX = False
                End If
                If BControllerID = ControllerNumber Then
                    BControllerStart = False
                    BControllerA = False
                    BControllerB = False
                    BControllerX = False
                End If
            Case 1 'A / Square button is down.
                If AControllerID = ControllerNumber Then
                    AControllerA = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerA = True
                End If
            Case 2 'B / X button is down.
                If AControllerID = ControllerNumber Then
                    AControllerB = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerB = True
                End If
            Case 4 'X / Circle button is down.
                If AControllerID = ControllerNumber Then
                    AControllerX = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerX = True
                End If
            Case 8 'Y / Triangle button is down.
            Case 16 'Left Bumper is down.
            Case 32 'Right Bumper is down.
            Case 64 'Back / Left Trigger is down.
            Case 128 'Start / Right Trigger is down.
                If AControllerID = ControllerNumber Then
                    AControllerStart = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerStart = True
                End If
            Case 3 'A+B / Square+X buttons are down.
            Case 5 'A+X / Square+Circle buttons are down.
            Case 9 'A+Y / Square+Triangle buttons are down.
            Case 6 'B+X / X+Circle buttons are down.
            Case 10 'B+Y / X+Triangle buttons are down.
            Case 12 'X+Y / Circle+Triangle buttons are down.
            Case 48 'Left Bumper+Right Bumper buttons are down.
            Case 192 'Back+Start / Left Trigger+Right Trigger are down.
        End Select

    End Sub

    Private Sub UpdateDPadPosition()
        'The range of POV is 0 to 65535.
        '0 through 31500 is used to represent the angle.
        'degrees = POV \ 100  315° = 31500 \ 100

        'What position is the D-Pad in?
        Select Case ControllerData.dwPOV
            Case 0 '0° Up
                If AControllerID = ControllerNumber Then
                    AControllerDown = False
                    AControllerUp = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerDown = False
                    BControllerUp = True
                End If
            Case 4500 '45° Up Right
                If AControllerID = ControllerNumber Then
                    AControllerDown = False
                    AControllerUp = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerDown = False
                    BControllerUp = True
                End If
            Case 9000 '90° Right
            Case 13500 '135° Down Right
                If AControllerID = ControllerNumber Then
                    AControllerUp = False
                    AControllerDown = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerUp = False
                    BControllerDown = True
                End If
            Case 18000 '180° Down
                If AControllerID = ControllerNumber Then
                    AControllerUp = False
                    AControllerDown = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerUp = False
                    BControllerDown = True
                End If
            Case 22500 '225° Down Left
                If AControllerID = ControllerNumber Then
                    AControllerUp = False
                    AControllerDown = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerUp = False
                    BControllerDown = True
                End If
            Case 27000 '270° Left
            Case 31500 '315° Up Left
                If AControllerID = ControllerNumber Then
                    AControllerDown = False
                    AControllerUp = True
                End If
                If BControllerID = ControllerNumber Then
                    BControllerDown = False
                    BControllerUp = True
                End If
            Case 65535 'Neutral
                If AControllerID = ControllerNumber Then
                    AControllerUp = False
                    AControllerDown = False
                End If
                If BControllerID = ControllerNumber Then
                    BControllerUp = False
                    BControllerDown = False
                End If
        End Select

    End Sub

    Private Sub UpdateLeftThumbstickPosition()
        'The range on the Y-axis is 0 to 65535.

        'What position is the left thumbstick in on the Y-axis?
        If ControllerData.dwYpos <= NeutralStart Then
            'The left thumbstick is in the up position.

            If AControllerID = ControllerNumber Then
                AControllerTsDown = False
                AControllerTsUp = True
            End If
            If BControllerID = ControllerNumber Then
                BControllerTsDown = False
                BControllerTsUp = True
            End If

        ElseIf ControllerData.dwYpos >= NeutralEnd Then
            'The left thumbstick is in the down position.

            If AControllerID = ControllerNumber Then
                AControllerTsUp = False
                AControllerTsDown = True
            End If
            If BControllerID = ControllerNumber Then
                BControllerTsUp = False
                BControllerTsDown = True
            End If

        Else
            'The left thumbstick is in the neutral position.

            If AControllerID = ControllerNumber Then
                AControllerTsUp = False
                AControllerTsDown = False
            End If
            If BControllerID = ControllerNumber Then
                BControllerTsUp = False
                BControllerTsDown = False
            End If

        End If

    End Sub

    Private Sub AssignController()
        'Assign controller a letter.

        'Has a controller been assigned to A?
        If AControllerID < 0 Then
            'No controller been assigned to A.

            'Is the controller assigned to B?
            If BControllerID <> ControllerNumber Then
                'No, the controller is not assigned to B.

                'Assign controller to A.
                AControllerID = ControllerNumber

            End If

        End If

        'Has a controller been assigned to B?
        If BControllerID < 0 Then
            'No, a controller has not been assigned to B.

            'Is the controller assigned to A?
            If AControllerID <> ControllerNumber Then
                'No, the controller is not assigned to A.

                'Assign controller to B.
                BControllerID = ControllerNumber

            End If

        End If

    End Sub

    Private Sub UnassignController()

        'Is this the controller to unassign?
        If AControllerID = ControllerNumber Then
            'Yes, this is the one.

            'Unassign
            AControllerID = -1

        End If

        'Is this the controller to unassign?
        If BControllerID = ControllerNumber Then
            'Yes, this is the one.

            'Unassign
            BControllerID = -1

        End If

    End Sub

    Private Sub DrawComputerPlayerIdentifier()

        Buffer.Graphics.DrawString("CPU", InstructionsFont, Brushes.White, ClientSize.Width \ 2 \ 2, 20, AlineCenterMiddle)

    End Sub

    Private Sub DrawRightPaddleScore()

        Buffer.Graphics.DrawString(RightPaddleScore, ScoreFont, Brushes.White, RPadScoreLocation, AlineCenterMiddle)

    End Sub

    Private Sub DrawLeftPaddleScore()

        Buffer.Graphics.DrawString(LeftPaddleScore, ScoreFont, Brushes.White, LPadScoreLocation, AlineCenterMiddle)

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

    Private Sub InitializePaddles()

        LeftPaddle.Width = 25
        LeftPaddle.Height = 100
        LeftPaddle.X = 20
        LeftPaddle.Y = ClientSize.Height \ 2 - LeftPaddle.Height \ 2 'Center vertically
        'LeftPaddleSpeed = 10

        RightPaddle.Width = 25
        RightPaddle.Height = 100
        RightPaddle.X = ClientSize.Width - RightPaddle.Width - 20 'Aline right 20 pix padding
        RightPaddle.Y = ClientSize.Height \ 2 - RightPaddle.Height \ 2 'Center vertically
        'RightPaddleSpeed = 10

    End Sub

    Private Sub InitializeBall()

        Ball.Width = 25
        Ball.Height = 25

        PlaceBallCenterCourt()

        'BallSpeed = 10

    End Sub

    Private Sub LayoutGame()

        'Center the left paddle vertically in the forms client area.
        LeftPaddle.Y = ClientSize.Height \ 2 - LeftPaddle.Height \ 2

        'Center the right paddle vertically in the forms client area.
        RightPaddle.Y = ClientSize.Height \ 2 - RightPaddle.Height \ 2

        'Aline the right paddle along the right side of the form client area allow 20
        'pixels padding.
        RightPaddle.X = ClientSize.Width - RightPaddle.Width - 20

        CenterCourtLine()

        BottomWall = ClientSize.Height

        TitleLocation = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 125)

        LPadScoreLocation = New Point(ClientSize.Width \ 2 \ 2, 100)

        RPadScoreLocation = New Point(ClientSize.Width - (ClientSize.Width \ 4), 100)

        LayoutInstructions()

        ClientCenter = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2)

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

    Private Sub DrawStartScreenInstructions()

        Buffer.Graphics.DrawString(InstructStartText, InstructionsFont, Brushes.White, InstructStartLocation, AlineCenter)

    End Sub

    Private Sub DrawPausedText()

        'Draw paused text.
        Buffer.Graphics.DrawString("Paused", TitleFont, Brushes.White, ClientCenter, AlineCenterMiddle)

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

    Private Sub DrawCenterCourtLine()

        Buffer.Graphics.DrawLine(CenterlinePen, CenterlineTop, CenterlineBottom)

    End Sub

    Private Sub DrawTitle()

        Buffer.Graphics.DrawString(TitleText, TitleFont, Brushes.White, TitleLocation, AlineCenter)

    End Sub

    Private Sub CenterCourtLine()

        'Centers the court line in the client area of our form.
        CenterlineTop = New Point(ClientSize.Width \ 2, 0)

        CenterlineBottom = New Point(ClientSize.Width \ 2, ClientSize.Height)

    End Sub

    Private Sub LayoutInstructions()

        Dim Location As New Point(ClientSize.Width \ 2, (ClientSize.Height \ 2) - 15)

        InstructOneLocation = Location

        InstructTwoLocation = Location

    End Sub

    Private Sub InitializeForm()

        WindowState = FormWindowState.Maximized

        Text = "Pong - Code with Joe"

        SetStyle(ControlStyles.AllPaintingInWmPaint, True) ' True is better
        SetStyle(ControlStyles.OptimizedDoubleBuffer, True) ' True is better

    End Sub

    Private Sub InitializeStringAlinement()

        AlineCenter.Alignment = StringAlignment.Center
        AlineCenterMiddle.Alignment = StringAlignment.Center
        AlineCenterMiddle.LineAlignment = StringAlignment.Center

    End Sub

    Private Shared Sub PlayBounceSound()

        My.Computer.Audio.Play(My.Resources.bounce, AudioPlayMode.Background)

        'Used Audacity to generate tone.
        'Frequency:600Hz  Amplitude:0.1  Duration:0.183s
        'saved as bounce.wav.

    End Sub

    Private Shared Sub PlayScoreSound()

        My.Computer.Audio.Play(My.Resources.score, AudioPlayMode.Background)

    End Sub

    Private Shared Sub PlayWinningSound()

        My.Computer.Audio.Play(My.Resources.winning, AudioPlayMode.Background)

    End Sub

    Private Shared Function RandomNumber() As Integer

        'Initialize random-number generator.
        Randomize()

        'Generate random number between 1 and 3.
        Return CInt(Int((3 * Rnd()) + 1))

    End Function

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

                'Did player push down the p key?
            Case Keys.P
                'Yes, player did push down the p key.

                PKeyDown = True

                'Did player push down the a key?
            Case Keys.A
                'Yes, player did push down the a key.

                AKeyDown = True

                'Did player push down the b key?
            Case Keys.B
                'Yes, player did push down the b key.

                BKeyDown = True

                'Did player push down the x key?
            Case Keys.X
                'Yes, player did push down the x key.

                XKeyDown = True

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

                'Did player let the a key up?
            Case Keys.A
                'Yes, player did let the a key up.

                AKeyDown = False

                'Did player let the b key up?
            Case Keys.B
                'Yes, player did push down the b key.

                BKeyDown = False

                'Did player let the x key up?
            Case Keys.X
                'Yes, player let the x key up.

                XKeyDown = False

        End Select

    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)

        'Intentionally left blank. Do not remove.

    End Sub

End Class

'Learn more:
'
'Consuming Unmanaged DLL Functions
'https://learn.microsoft.com/en-us/dotnet/framework/interop/consuming-unmanaged-dll-functions
'
'DllImportAttribute.EntryPoint Field
'https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.dllimportattribute.entrypoint?view=net-7.0
'
'Passing Structures
'https://learn.microsoft.com/en-us/dotnet/framework/interop/passing-structures
'
'Strings used in Structures
'https://learn.microsoft.com/en-us/dotnet/framework/interop/default-marshalling-for-strings#strings-used-in-structures
'
'joyGetPosEx Function
'https://learn.microsoft.com/en-us/windows/win32/api/joystickapi/nf-joystickapi-joygetposex
'
'JOYINFOEX Structure
'https://learn.microsoft.com/en-us/windows/win32/api/joystickapi/ns-joystickapi-joyinfoex
'
'Multimedia Input
'https://learn.microsoft.com/en-us/windows/win32/Multimedia/multimedia-input
'
'Windows Multimedia
'https://learn.microsoft.com/en-us/windows/win32/multimedia/windows-multimedia-start-page
'
'Reading Input Data From Joystick in Visual Basic
'https://social.msdn.microsoft.com/Forums/en-US/af28b35b-d756-4d87-94c6-ced882ab20a5/reading-input-data-from-joystick-in-visual-basic?forum=vbgeneral
