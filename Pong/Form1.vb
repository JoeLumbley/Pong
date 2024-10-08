' Pong

' Pong is a simulation of Table Tennis, a recreational activity and an
' Olympic sport since 1988 is also known by the term "ping-pong" or just "pong".

' This repository is designed to help new game developers learn the fundamentals
' of game programming and design through a classic game.

' Features
'
'   Classic Gameplay: Experience the timeless fun of Pong with modern
'   enhancements.

'   Keyboard and Controller Support: Play using your keyboard ⌨️ or Xbox
'   controllers 🎮 , complete with vibration feedback.
'
'   Resizable and Pausable: Enjoy a flexible gameplay experience that can be
'   paused at any time.
'
'   Single and Multiplayer Modes: Challenge yourself against a computer player
'   or compete with friends.

' Learning Objectives

'   Understand the basics of game mechanics and physics.

'   Gain hands-on experience with VB.NET and game development concepts.

'   Learn how to implement user input handling, game states, and sound effects.

' https://github.com/JoeLumbley/Pong

' MIT License
' Copyright(c) 2023 Joseph W. Lumbley

' Permission Is hereby granted, free Of charge, to any person obtaining a copy
' of this software And associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
' copies of the Software, And to permit persons to whom the Software Is
' furnished to do so, subject to the following conditions:

' The above copyright notice And this permission notice shall be included In all
' copies Or substantial portions of the Software.

' THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
' IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
' LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
' OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
' SOFTWARE.

' Monica is our an AI assistant.
' https://monica.im/

Imports System.Threading
Imports System.Numerics
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO

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

    'State Data *******************************************
    Private GameState As GameStateEnum = GameStateEnum.StartScreen
    Private Serving As ServeStateEnum = ServeStateEnum.LeftPaddle
    Private ServSpeed As Single = 500
    Private Winner As WinStateEnum = WinStateEnum.LeftPaddle
    Private NumberOfPlayers As Integer = 2
    '******************************************************

    Private Structure GameObject
        Public Position As Vector2 'A vector 2 is composed of two floating-point values called X and Y.
        Public Acceleration As Vector2
        Public Velocity As Vector2
        Public MaxVelocity As Vector2
        Public Rect As Rectangle
    End Structure

    Private Ball As GameObject

    Private LeftPaddle As GameObject

    Private RightPaddle As GameObject

    Private Const PingPongEmoji As String = "🏓"
    Private ReadOnly EmojiFont As New Font("Segoe UI Emoji", 88)
    Private EmojiLocation As New Point(ClientSize.Width \ 2 - 110, ClientSize.Height \ 2 - 125)

    Private Const TitleText As String = "P    NG"
    Private TitleLocation As New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 125)
    Private ReadOnly TitleFont As New Font(FontFamily.GenericSansSerif, 100)

    Private InstructStartLocation As Point
    Private ReadOnly InstructStartText As String = vbCrLf &
        "One player  A   Two players  B"

    'One Player Instructions Data *************************
    Private InstructOneLocation As Point
    Private Const InstructOneText As String = vbCrLf &
        "Play  A" & vbCrLf & vbCrLf &
        "You are the left paddle" & vbCrLf &
        "Beat the computer to 10 points to win" & vbCrLf & vbCrLf &
        "Pause / Resume  Start "
    '******************************************************

    'Two Player Instructions Data *************************
    Private InstructTwoLocation As Point
    Private Const InstructTwoText As String = vbCrLf &
        "Play  A  " & vbCrLf & vbCrLf &
        "Left versus Right " & vbCrLf &
        "First player to 10 points wins" & vbCrLf & vbCrLf &
        "Pause / Resume  Start  "
    '******************************************************
    Private ReadOnly InstructionsFont As New Font(FontFamily.GenericSansSerif, 20)

    Private IsBackButtonDown(0 To 3) As Boolean

    Private IsAButtonDown(0 To 3) As Boolean

    Private IsAKeyDown As Boolean = False

    'Centerline Data *******************
    Private CenterlineTop As Point
    Private CenterlineBottom As Point

    Private ReadOnly CenterlinePen As New Pen(Color.White, 16) With {
                    .DashStyle = Drawing2D.DashStyle.Custom,
                    .DashPattern = New Single() {1, 2}
                    }

    Private Const LeftPaddleSpeed As Integer = 8
    Private LeftPaddleScore As Integer
    Private LPadScoreLocation As Point
    Private ReadOnly LeftPaddleMidlinePen As New Pen(Color.Goldenrod, 7)
    Private LeftPaddleMiddle As Integer = LeftPaddle.Rect.Y + LeftPaddle.Rect.Height \ 2

    Private Const RightPaddleSpeed As Integer = 8
    Private RightPaddleScore As Integer
    Private RPadScoreLocation As Point

    Private ReadOnly ScoreFont As New Font(FontFamily.GenericSansSerif, 75)

    Private IsConThumbRYNeutral(0 To 3) As Boolean
    Private IsConThumbLYNeutral(0 To 3) As Boolean

    Private ClientCenter As New Point(ClientSize.Width \ 2, ClientSize.Height \ 2)

    Private ApplyLeftPaddleEnglish As Boolean = False

    Private ApplyRightPaddleEnglish As Boolean = False

    'Counter Data *************************************
    Private FlashCount As Integer = 0
    Private EndScreenCounter As Integer = 0
    '******************************************************

    Private BallMiddle As Single = Ball.Rect.Y + Ball.Rect.Height / 2

    Private RightPaddleMiddle As Single = RightPaddle.Rect.Y + RightPaddle.Rect.Height / 2

    'Keyboard Event Data **********************************
    Private SpaceBarDown As Boolean = False
    Private WKeyDown As Boolean = False
    Private SKeyDown As Boolean = False
    Private UpArrowKeyDown As Boolean = False
    Private DownArrowKeyDown As Boolean = False
    Private OneKeyDown As Boolean = False
    Private TwoKeyDown As Boolean = False
    Private PKeyDown As Boolean = False
    Private IsPKeyDown As Boolean = False

    Private AKeyDown As Boolean = False
    Private BKeyDown As Boolean = False
    Private XKeyDown As Boolean = False
    '******************************************************

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputGetState(dwUserIndex As Integer, ByRef pState As XINPUT_STATE) As Integer
    End Function

    ' XInput1_4.dll seems to be the current version
    ' XInput9_1_0.dll is maintained primarily for backward compatibility. 

    <StructLayout(LayoutKind.Explicit)>
    Public Structure XINPUT_STATE
        <FieldOffset(0)>
        Public dwPacketNumber As UInteger ' Unsigned 32-bit (4-byte) integer range 0 through 4,294,967,295.
        <FieldOffset(4)>
        Public Gamepad As XINPUT_GAMEPAD
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure XINPUT_GAMEPAD
        Public wButtons As UShort ' Unsigned 16-bit (2-byte) integer range 0 through 65,535.
        Public bLeftTrigger As Byte ' Unsigned 8-bit (1-byte) integer range 0 through 255.
        Public bRightTrigger As Byte
        Public sThumbLX As Short ' Signed 16-bit (2-byte) integer range -32,768 through 32,767.
        Public sThumbLY As Short
        Public sThumbRX As Short
        Public sThumbRY As Short

    End Structure

    Private ControllerPosition As XINPUT_STATE

    ' Set the start of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralStart As Short = -16384 '-16,384 = -32,768 / 2
    ' The thumbstick position must be more than 1/2 over the neutral start to register as moved.
    ' A short is a signed 16-bit (2-byte) integer range -32,768 through 32,767. This gives us 65,536 values.

    ' Set the end of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralEnd As Short = 16384 '16,383.5 = 32,767 / 2
    ' The thumbstick position must be more than 1/2 over the neutral end to register as moved.

    ' Set the trigger threshold to 1/4 pull.
    Private Const TriggerThreshold As Byte = 64 '64 = 256 / 4
    ' The trigger position must be greater than the trigger threshold to register as pulled.
    ' A byte is a unsigned 8-bit (1-byte) integer range 0 through 255. This gives us 256 values.

    Private ReadOnly Connected(0 To 3) As Boolean

    Private ConnectionStart As Date = Now

    Private Const DPadUp As Integer = 1
    Private Const DPadDown As Integer = 2

    Private Const DPadLeft As Integer = 4
    Private Const DPadRight As Integer = 8

    Private Const StartButton As Integer = 16
    Private Const BackButton As Integer = 32

    Private Const LeftStickButton As Integer = 64
    Private Const RightStickButton As Integer = 128

    Private Const LeftBumperButton As Integer = 256
    Private Const RightBumperButton As Integer = 512

    Private Const AButton As Integer = 4096
    Private Const BButton As Integer = 8192
    Private Const XButton As Integer = 16384
    Private Const YButton As Integer = 32768

    Private DPadUpPressed As Boolean = False
    Private DPadDownPressed As Boolean = False
    Private DPadLeftPressed As Boolean = False
    Private DPadRightPressed As Boolean = False

    Private StartButtonPressed As Boolean = False
    Private BackButtonPressed As Boolean = False

    Private LeftStickButtonPressed As Boolean = False
    Private RightStickButtonPressed As Boolean = False

    Private LeftBumperButtonPressed As Boolean = False
    Private RightBumperButtonPressed As Boolean = False

    Private AButtonPressed As Boolean = False
    Private BButtonPressed As Boolean = False
    Private XButtonPressed As Boolean = False
    Private YButtonPressed As Boolean = False

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputSetState(playerIndex As Integer, ByRef vibration As XINPUT_VIBRATION) As Integer
    End Function

    Public Structure XINPUT_VIBRATION
        Public wLeftMotorSpeed As UShort
        Public wRightMotorSpeed As UShort
    End Structure

    Private Vibration As XINPUT_VIBRATION

    Private LeftVibrateStart(0 To 3) As Date

    Private RightVibrateStart(0 To 3) As Date

    Private IsLeftVibrating(0 To 3) As Boolean

    Private IsRightVibrating(0 To 3) As Boolean


    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
    Private Shared Function mciSendStringW(<MarshalAs(UnmanagedType.LPTStr)> ByVal lpszCommand As String,
                                           <MarshalAs(UnmanagedType.LPWStr)> ByVal lpszReturnString As StringBuilder,
                                           ByVal cchReturn As UInteger, ByVal hwndCallback As IntPtr) As Integer
    End Function

    Private Sounds() As String

    Private Context As New BufferedGraphicsContext

    Private Buffer As BufferedGraphics

    Private FrameCount As Integer = 0

    Private StartTime As DateTime = Now 'Get current time.

    Private TimeElapsed As TimeSpan

    Private SecondsElapsed As Double = 0

    Private FPS As Integer = 0

    Private ReadOnly FPSFont As New Font(FontFamily.GenericSansSerif, 25)

    Private FPS_Postion As New Point(0, 0)

    Private CurrentFrame As DateTime = Now 'Get current time.

    Private LastFrame As DateTime = CurrentFrame 'Initialize last frame time to current time.

    Private DeltaTime As TimeSpan = CurrentFrame - LastFrame 'Initialize delta time to 0

    Private ReadOnly AlineCenter As New StringFormat With {.Alignment = StringAlignment.Center}

    Private ReadOnly AlineCenterMiddle As New StringFormat With {.Alignment = StringAlignment.Center,
                                                                 .LineAlignment = StringAlignment.Center}

    Private GameLoopCancellationToken As New CancellationTokenSource()

    Private DrawFlashingText As Boolean = True

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        UpdateGame()

        Refresh()

    End Sub

    Private Sub UpdateGame()

        Select Case GameState

            Case GameStateEnum.Playing

                UpdateControllerData()

                UpdateLeftPaddleKeyboard()

                If PKeyDown Then

                    If Not IsPKeyDown Then

                        IsPKeyDown = True

                        GameState = GameStateEnum.Pause

                    End If

                Else

                    IsPKeyDown = False

                End If

                If NumberOfPlayers = 1 Then

                    UpdateComputerPlayer()

                Else

                    UpdateRightPaddleKeyboard()

                End If

                UpdateDeltaTime()

                UpdateBallMovement()

                UpdatePaddleMovement()

                CheckForPaddleHits()

                CheckForWallBounce()

                UpdateScore()

                CheckforEndGame()

                UpdateVibrateTimer()

                'UpdatePlaying()

            Case GameStateEnum.StartScreen

                UpdateControllerData()

                UpdateStartScreenKeyboard()

                UpdateDeltaTime()

                UpdateStartScreen()

                UpdateBallMovement()

                CheckForWallBounce()

                CheckForWallBounceXaxis()

            Case GameStateEnum.Instructions

                UpdateControllerData()

                UpdateInstructionsScreenKeyboard()

                UpdateDeltaTime()

                UpdateBallMovement()

                CheckForWallBounce()

                CheckForWallBounceXaxis()

                'UpdateInstructions()

            Case GameStateEnum.Serve

                UpdateServe()

            Case GameStateEnum.Pause

                If PKeyDown Then

                    If Not IsPKeyDown Then

                        IsPKeyDown = True

                        LastFrame = Now

                        GameState = GameStateEnum.Playing

                    End If

                Else

                    IsPKeyDown = False

                End If

                'UpdatePause()

            Case GameStateEnum.EndScreen

                UpdateEndScreen()

        End Select

    End Sub

    Private Sub UpdateComputerPlayer()
        ' In one player mode the right paddle is played
        ' by the following algorithm.

        BallMiddle = Ball.Rect.Y + Ball.Rect.Height \ 2

        RightPaddleMiddle = RightPaddle.Rect.Y + RightPaddle.Rect.Height \ 2


        ' Is the ball above the paddle?
        If BallMiddle < RightPaddleMiddle Then
            ' Yes, the ball is above the paddle.

            ' Is the paddle moving down?
            If RightPaddle.Velocity.Y > 0 Then
                ' Yes, the paddle is moving down.

                ' Stop move before changing direction.
                RightPaddle.Velocity.Y = 0 'Zero speed.

            End If

            ' Is the paddle at or past the top edge of the client rectangle?
            If RightPaddle.Rect.Y <= ClientRectangle.Top Then
                ' Yes, the paddle is at or past the top edge of the client rectangle.

                ' Stop paddle movement.
                RightPaddle.Velocity.Y = 0

                ' Push paddle above the top edge of the client rectangle.
                RightPaddle.Rect.Y = ClientRectangle.Top

                RightPaddle.Position.Y = RightPaddle.Rect.Y

            Else
                ' No, the paddle is not at or past the top edge of the client rectangle.

                ' Accelerate paddle up.
                RightPaddle.Velocity.Y -= RightPaddle.Acceleration.Y * DeltaTime.TotalSeconds

                ' Limit paddle velocity to the max.
                If RightPaddle.Velocity.Y < -RightPaddle.MaxVelocity.Y Then RightPaddle.Velocity.Y = -RightPaddle.MaxVelocity.Y

            End If

            ' Is the ball below the paddle?
        ElseIf BallMiddle > RightPaddleMiddle Then
            ' Yes, the ball is below the paddle.

            ' Is the paddle moving up?
            If RightPaddle.Velocity.Y < 0 Then
                ' Yes, the paddle is moving up.

                ' Stop move before changing direction.
                RightPaddle.Velocity.Y = 0 'Zero speed.

            End If

            ' Is the paddle at or past the bottom edge of the client rectangle?
            If RightPaddle.Rect.Y + RightPaddle.Rect.Height >= ClientRectangle.Bottom Then
                ' Yes, the paddle is at or past the bottom edge of the client rectangle.

                ' Stop paddle movement.
                RightPaddle.Velocity.Y = 0

                ' Push paddle above the bottom edge of the client rectangle.
                RightPaddle.Rect.Y = ClientRectangle.Bottom - RightPaddle.Rect.Height

                RightPaddle.Position.Y = RightPaddle.Rect.Y

            Else
                ' No, the paddle is not at or past the bottom edge of the client rectangle.

                ' Accelerate paddle down.
                RightPaddle.Velocity.Y += RightPaddle.Acceleration.Y * DeltaTime.TotalSeconds

                ' Limit paddle velocity to the max.
                If RightPaddle.Velocity.Y > RightPaddle.MaxVelocity.Y Then RightPaddle.Velocity.Y = RightPaddle.MaxVelocity.Y

            End If

        Else

            DecelerateRightPaddle()

        End If

    End Sub

    Private Sub CheckforEndGame()

        ' Did left paddle reach winning score?
        If LeftPaddleScore >= 10 Then
            ' Yes, left paddle did reach winning score.

            ' Set winner to left paddle.
            Winner = WinStateEnum.LeftPaddle

            ' Reset the frame counter.
            FlashCount = 0

            ' Change game state to end screen.
            GameState = GameStateEnum.EndScreen

            PlayWinningSound()

        End If

        ' Did right paddle reach winning score?
        If RightPaddleScore >= 10 Then
            ' Yes, right paddle did reach winning score.

            ' Set winner to right paddle.
            Winner = WinStateEnum.RightPaddle

            ' Reset frame counter.
            FlashCount = 0

            ' Change game state to end screen.
            GameState = GameStateEnum.EndScreen

            PlayWinningSound()

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

        LastFrame = Now

        GameState = GameStateEnum.StartScreen

        LoopSound("startscreenmusic")

        PlaceBallCenterCourt()

        Ball.Velocity.X = -50
        Ball.Velocity.Y = -50

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

    Private Sub PlayWinningSound()

        PlaySound("winning")

    End Sub

    Private Sub MovePointerOffScreen()
        'Move mouse pointer off screen.

        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)

    End Sub

    Private Sub UpdateServe()

        LeftPaddle.Position.Y = (ClientSize.Height \ 2) - (LeftPaddle.Rect.Height \ 2)
        LeftPaddle.Rect.Y = LeftPaddle.Position.Y

        RightPaddle.Position.Y = (ClientSize.Height \ 2) - (RightPaddle.Rect.Height \ 2)
        RightPaddle.Rect.Y = RightPaddle.Position.Y

        PlaceBallCenterCourt()

        If Serving = ServeStateEnum.RightPaddle Then

            ServeRightPaddle()

        Else

            ServeLeftPaddle()

        End If

        GameState = GameStateEnum.Playing

    End Sub

    Private Sub ServeLeftPaddle()

        Select Case RandomNumber()

            Case 1

                'Send ball up and to the right.
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = -ServSpeed

            Case 2

                'Send ball to the right.
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = 0

            Case 3

                'Send ball down and to the right.
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = ServSpeed

        End Select

    End Sub

    Private Sub ServeRightPaddle()

        Select Case RandomNumber()

            Case 1

                'Send ball up and to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = -ServSpeed

            Case 2

                'Send ball to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = 0

            Case 3

                'Send ball down and to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = ServSpeed

        End Select

    End Sub
    Private Shared Function RandomNumber() As Integer

        'Initialize random-number generator.
        Randomize()

        'Generate random number between 1 and 3.
        Return CInt(Int((3 * Rnd()) + 1))

    End Function

    Private Sub PlaceBallCenterCourt()

        Ball.Rect.Location = New Point((ClientSize.Width \ 2) - (Ball.Rect.Width \ 2), (ClientSize.Height \ 2) - (Ball.Rect.Height \ 2))

        Ball.Position.X = Ball.Rect.X
        Ball.Position.Y = Ball.Rect.Y

    End Sub

    Private Sub UpdatePaddleMovement()

        UpdateLeftPaddleMovement()

        UpdateRightPaddleMovement()

    End Sub

    Private Sub UpdateScore()

        'Did ball enter left goal zone?
        If Ball.Rect.X < 0 Then
            'Yes, ball entered left goal zone.

            PlayPointSound()

            'Award point to right paddle.
            RightPaddleScore += 1

            'Change possession of ball to right paddle.
            Serving = ServeStateEnum.RightPaddle

            'Change game state to serve.
            GameState = GameStateEnum.Serve

        End If

        'Did ball enter right goal zone?
        If Ball.Rect.X + Ball.Rect.Width > ClientSize.Width Then
            'Yes, ball entered goal zone.

            PlayPointSound()

            'Award a point to left paddle.
            LeftPaddleScore += 1

            'Change possession of ball to left paddle.
            Serving = ServeStateEnum.LeftPaddle

            'Change game state to serve.
            GameState = GameStateEnum.Serve

        End If

    End Sub

    Private Sub CheckForPaddleHits()

        If Ball.Rect.IntersectsWith(LeftPaddle.Rect) Then

            PlaySound("hit")

            Ball.Velocity.X = 0
            Ball.Velocity.Y = 0

            'Push the ball to the paddles right edge.
            Ball.Rect.X = LeftPaddle.Rect.X + LeftPaddle.Rect.Width + 5

            Ball.Position.X = Ball.Rect.X

            ApplyLeftPaddleEnglish = True

            VibrateLeft(0, 42000)

        End If

        If Ball.Rect.IntersectsWith(RightPaddle.Rect) Then

            PlaySound("hit")

            Ball.Velocity.X = 0
            Ball.Velocity.Y = 0

            Ball.Rect.X = RightPaddle.Rect.X - (Ball.Rect.Width + 5)

            Ball.Position.X = Ball.Rect.X

            If NumberOfPlayers = 2 Then

                ApplyRightPaddleEnglish = True

                VibrateLeft(1, 42000)

            Else
                'For the computer player random english.
                'This makes the game more interesting.

                Select Case RandomNumber()

                    Case 1
                        'Send ball up and to the left.

                        Ball.Velocity.X = -ServSpeed
                        Ball.Velocity.Y = -ServSpeed

                    Case 2
                        'Send ball to the left.

                        Ball.Velocity.X = -ServSpeed
                        Ball.Velocity.Y = 0

                    Case 3
                        'Send ball down and to the left.

                        Ball.Velocity.X = -ServSpeed
                        Ball.Velocity.Y = ServSpeed

                End Select

            End If

        End If

    End Sub

    Private Sub VibrateLeft(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The left motor is the low-frequency rumble motor.

        ' Set left motor speed.
        Vibration.wLeftMotorSpeed = Speed

        SendVibrationMotorCommand(CID)

        LeftVibrateStart(CID) = Now

        IsLeftVibrating(CID) = True

    End Sub

    Private Sub SendVibrationMotorCommand(ControllerID As Integer)
        ' Sends vibration motor speed command to the specified controller.

        Try

            ' Send motor speed command to the specified controller.
            If XInputSetState(ControllerID, Vibration) = 0 Then
                ' The motor speed was set. Success.

            Else
                ' The motor speed was not set. Fail.
                ' You can log or handle the failure here if needed.
                ' Example: Console.WriteLine(XInputSetState(ControllerID, Vibration).ToString())

            End If

        Catch ex As Exception

            DisplayError(ex)

            Exit Sub

        End Try

    End Sub

    Private Sub UpdateVibrateTimer()

        UpdateLeftVibrateTimer()

        UpdateRightVibrateTimer()

    End Sub

    Private Sub UpdateLeftVibrateTimer()

        For Each IsConVibrating In IsLeftVibrating

            Dim Index As Integer = Array.IndexOf(IsLeftVibrating, IsConVibrating)

            If Index <> -1 AndAlso IsConVibrating = True Then

                Dim ElapsedTime As TimeSpan = Now - LeftVibrateStart(Index)

                If ElapsedTime.TotalMilliseconds >= 250 Then

                    IsLeftVibrating(Index) = False

                    ' Turn left motor off (set zero speed).
                    Vibration.wLeftMotorSpeed = 0

                    SendVibrationMotorCommand(Index)

                End If

            End If

        Next

    End Sub

    Private Sub UpdateRightVibrateTimer()

        For Each IsConVibrating In IsRightVibrating

            Dim Index As Integer = Array.IndexOf(IsRightVibrating, IsConVibrating)

            If Index <> -1 AndAlso IsConVibrating = True Then

                Dim ElapsedTime As TimeSpan = Now - RightVibrateStart(Index)

                If ElapsedTime.TotalMilliseconds >= 250 Then

                    IsRightVibrating(Index) = False

                    ' Turn right motor off (set zero speed).
                    Vibration.wRightMotorSpeed = 0

                    SendVibrationMotorCommand(Index)

                End If

            End If

        Next

    End Sub

    Private Sub LimitPaddleMovement()

        LimitLeftPaddleMovement()

        LimitRightPaddleMovement()

    End Sub

    Private Sub LimitRightPaddleMovement()

        If RightPaddle.Rect.Y <= ClientRectangle.Top Then

            RightPaddle.Velocity.Y = 100

            RightPaddle.Rect.Y = ClientRectangle.Top

        End If

        If RightPaddle.Rect.Y + RightPaddle.Rect.Height >= ClientRectangle.Bottom Then

            RightPaddle.Velocity.Y = -100

            RightPaddle.Rect.Y = ClientRectangle.Bottom - RightPaddle.Rect.Height

        End If

    End Sub

    Private Sub LimitLeftPaddleMovement()

        If LeftPaddle.Rect.Y <= ClientRectangle.Top Then

            LeftPaddle.Velocity.Y = 100

            LeftPaddle.Rect.Y = ClientRectangle.Top

        End If

        If LeftPaddle.Rect.Y + LeftPaddle.Rect.Height >= ClientRectangle.Bottom Then

            LeftPaddle.Velocity.Y = -100

            LeftPaddle.Rect.Y = ClientRectangle.Bottom - LeftPaddle.Rect.Height

        End If

    End Sub

    Private Sub CheckForWallBounce()

        'Did the ball hit the top wall?
        If Ball.Rect.Y < ClientRectangle.Top Then
            'Yes, the ball hit the top wall.

            Dim TempV As Single = Ball.Velocity.Y

            Ball.Velocity.Y = 0

            'Push the ball to the walls top edge.
            Ball.Rect.Y = ClientRectangle.Top + 5

            Ball.Position.Y = Ball.Rect.Y

            PlayBounceSound()

            'Reverse direction on the y-axis
            Ball.Velocity.Y = TempV * -1

        End If

        'Did the ball hit the bottom wall?
        If Ball.Rect.Y + Ball.Rect.Height > ClientRectangle.Bottom Then
            'Yes, the ball hit the bottom wall.

            Dim TempV As Single = Ball.Velocity.Y

            Ball.Velocity.Y = 0

            'Push the ball to the walls bottom edge.
            Ball.Rect.Y = ClientRectangle.Bottom - Ball.Rect.Height - 5

            Ball.Position.Y = Ball.Rect.Y

            PlayBounceSound()

            'Reverse direction on the y-axis
            Ball.Velocity.Y = TempV * -1

        End If

    End Sub

    Private Sub CheckForWallBounceXaxis()

        'Did the ball hit the left edge of the wall?
        If Ball.Rect.X < ClientRectangle.Left Then
            'Yes, the ball hit the left edge of the wall.

            Dim TempV As Single = Ball.Velocity.X

            Ball.Velocity.X = 0

            'Push the ball to the walls left edge.
            Ball.Rect.X = ClientRectangle.Left + 5

            Ball.Position.X = Ball.Rect.X

            PlayBounceSound()

            'Reverse direction on the y-axis
            Ball.Velocity.X = TempV * -1

        End If

        'Did the ball hit the bottom wall?
        If Ball.Rect.X + Ball.Rect.Width > ClientRectangle.Right Then
            'Yes, the ball hit the bottom wall.

            Dim TempV As Single = Ball.Velocity.X

            Ball.Velocity.X = 0

            'Push the ball to the walls right edge.
            Ball.Rect.X = ClientRectangle.Right - Ball.Rect.Height - 5

            Ball.Position.X = Ball.Rect.X

            PlayBounceSound()

            'Reverse direction on the y-axis
            Ball.Velocity.X = TempV * -1

        End If

    End Sub

    Private Sub DoDPadLogic(controllerNumber As Integer)

        If controllerNumber = 0 Then

            DoDPadLogicControllerZero()

        End If

        If controllerNumber = 1 Then

            If NumberOfPlayers = 2 Then

                DoDPadLogicControllerOne()

            End If

        End If

    End Sub

    Private Sub DoDPadLogicControllerOne()

        If DPadUpPressed Then

            MoveRightPaddleUp()

        ElseIf DPadDownPressed Then

            MoveRightPaddleDown()

        Else

            If IsConThumbRYNeutral(1) = True AndAlso IsConThumbLYNeutral(1) = True AndAlso Not UpArrowKeyDown AndAlso Not DownArrowKeyDown Then

                DecelerateRightPaddle()

            End If

            If ApplyRightPaddleEnglish AndAlso IsConThumbRYNeutral(1) AndAlso IsConThumbLYNeutral(1) AndAlso Not UpArrowKeyDown AndAlso Not DownArrowKeyDown Then

                ApplyRightPaddleEnglish = False

                'Send ball to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = 0

            End If

        End If

    End Sub

    Private Sub DoDPadLogicControllerZero()

        If DPadUpPressed Then

            MoveLeftPaddleUp()

        ElseIf DPadDownPressed Then

            MoveLeftPaddleDown()

        Else
            ' The direction pad is in the neutral position.

            If IsConThumbRYNeutral(0) AndAlso IsConThumbLYNeutral(0) AndAlso Not WKeyDown AndAlso Not SKeyDown Then

                DecelerateLeftPaddle()

            End If

            If ApplyLeftPaddleEnglish AndAlso IsConThumbRYNeutral(0) AndAlso IsConThumbLYNeutral(0) AndAlso Not WKeyDown AndAlso Not SKeyDown Then

                ApplyLeftPaddleEnglish = False

                'Send ball to the right.
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = 0

            End If

        End If

    End Sub

    Private Sub UpdateRightThumbstickPosition(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.
        ' The range on the Y-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.

        If ControllerNumber = 0 Then

            UpdateRightThumbstickPositionControllerZero()

        End If

        If ControllerNumber = 1 Then

            If NumberOfPlayers = 2 Then

                UpdateRightThumbstickPositionControllerOne()

            End If

        End If

    End Sub

    Private Sub UpdateRightThumbstickPositionControllerOne()

        ' What position is the right thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbRY <= NeutralStart Then
            ' The right thumbstick is in the down position.

            IsConThumbRYNeutral(1) = False

            MoveRightPaddleDown()

        ElseIf ControllerPosition.Gamepad.sThumbRY >= NeutralEnd Then
            ' The right thumbstick is in the up position.

            IsConThumbRYNeutral(1) = False

            MoveRightPaddleUp()

        Else
            ' The right thumbstick is in the neutral position.

            IsConThumbRYNeutral(1) = True

        End If

    End Sub

    Private Sub UpdateRightThumbstickPositionControllerZero()

        ' What position is the right thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbRY <= NeutralStart Then
            ' The right thumbstick is in the down position.

            IsConThumbRYNeutral(0) = False

            MoveLeftPaddleDown()

        ElseIf ControllerPosition.Gamepad.sThumbRY >= NeutralEnd Then
            ' The right thumbstick is in the up position.

            IsConThumbRYNeutral(0) = False

            MoveLeftPaddleUp()

        Else
            ' The right thumbstick is in the neutral position.

            IsConThumbRYNeutral(0) = True

        End If

    End Sub

    Private Sub UpdateLeftThumbstickPosition(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.
        ' The range on the Y-axis is -32,768 through 32,767. Signed 16-bit (2-byte) integer.

        If ControllerNumber = 0 Then

            UpdateLeftThumbstickPositionControllerZero()

        End If

        If ControllerNumber = 1 Then

            If NumberOfPlayers = 2 Then

                UpdateLeftThumbstickPositionControllerOne()

            End If

        End If

    End Sub

    Private Sub UpdateLeftThumbstickPositionControllerOne()

        ' What position is the left thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbLY <= NeutralStart Then
            ' The left thumbstick is in the down position.

            IsConThumbLYNeutral(1) = False

            MoveRightPaddleDown()

        ElseIf ControllerPosition.Gamepad.sThumbLY >= NeutralEnd Then
            ' The left thumbstick is in the up position.

            IsConThumbLYNeutral(1) = False

            MoveRightPaddleUp()

        Else
            ' The left thumbstick is in the neutral position.

            IsConThumbLYNeutral(1) = True

        End If

    End Sub

    Private Sub UpdateStartScreenKeyboard()

        If AKeyDown Then

            If Not IsAKeyDown Then

                IsAKeyDown = True

                NumberOfPlayers = 1

                GameState = GameStateEnum.Instructions

                MovePointerOffScreen()

            End If

        Else

            IsAKeyDown = False

        End If

        If BKeyDown Then

            NumberOfPlayers = 2

            GameState = GameStateEnum.Instructions

            MovePointerOffScreen()

        End If

    End Sub

    Private Sub UpdateInstructionsScreenKeyboard()

        If AKeyDown Then

            If Not IsAKeyDown Then

                IsAKeyDown = True

                If IsPlaying("startscreenmusic") = True Then

                    PauseSound("startscreenmusic")

                End If

                GameState = GameStateEnum.Serve

                MovePointerOffScreen()

            End If

        Else

            IsAKeyDown = False

        End If

    End Sub

    Private Sub UpdateLeftPaddleKeyboard()

        If WKeyDown = True Then

            MoveLeftPaddleUp()

        ElseIf SKeyDown = True Then

            MoveLeftPaddleDown()

        Else

            If Not Connected(0) Then

                DecelerateLeftPaddle()

                If ApplyLeftPaddleEnglish Then

                    ApplyLeftPaddleEnglish = False

                    'Send ball to the right.
                    Ball.Velocity.X = ServSpeed
                    Ball.Velocity.Y = 0

                End If

            End If

        End If

    End Sub

    Private Sub UpdateRightPaddleKeyboard()

        If UpArrowKeyDown = True Then

            MoveRightPaddleUp()

        ElseIf DownArrowKeyDown = True Then

            MoveRightPaddleDown()

        Else

            If Not Connected(1) Then

                DecelerateRightPaddle()

                If ApplyRightPaddleEnglish Then

                    ApplyRightPaddleEnglish = False

                    'Send ball to the left.
                    Ball.Velocity.X = -ServSpeed
                    Ball.Velocity.Y = 0

                End If

            End If

        End If

    End Sub

    Private Sub MoveRightPaddleUp()

        ' Is the paddle moving down?
        If RightPaddle.Velocity.Y > 0 Then
            ' Yes, the paddle is moving down.

            ' Stop move before changing direction.
            RightPaddle.Velocity.Y = 0 'Zero speed.

        End If

        ' Is the paddle at or past the top edge of the client rectangle?
        If RightPaddle.Rect.Y <= ClientRectangle.Top Then
            'Yes, the paddle is at or past the top edge of the client rectangle.

            ' Stop paddle movement.
            RightPaddle.Velocity.Y = 0

            ' Push paddle above the top edge of the client rectangle.
            RightPaddle.Rect.Y = ClientRectangle.Top

        Else
            ' No, the paddle is not at or past the top edge of the client rectangle.

            ' Accelerate paddle up.
            RightPaddle.Velocity.Y -= RightPaddle.Acceleration.Y * DeltaTime.TotalSeconds

            ' Limit paddle velocity to the max.
            If RightPaddle.Velocity.Y < -RightPaddle.MaxVelocity.Y Then RightPaddle.Velocity.Y = -RightPaddle.MaxVelocity.Y

        End If

        If ApplyRightPaddleEnglish Then

            ApplyRightPaddleEnglish = False

            ' Send ball up and to the left.
            Ball.Velocity.X = -ServSpeed
            Ball.Velocity.Y = -ServSpeed

        End If

    End Sub

    Private Sub MoveRightPaddleDown()

        ' Is the paddle moving up?
        If RightPaddle.Velocity.Y < 0 Then
            ' Yes, the paddle is moving up.

            ' Stop move before changing direction.
            RightPaddle.Velocity.Y = 0 'Zero speed.

        End If

        ' Is the paddle at or past the bottom edge of the client rectangle?
        If RightPaddle.Rect.Y + RightPaddle.Rect.Height >= ClientRectangle.Bottom Then
            'Yes, the paddle is at or past the bottom edge of the client rectangle.

            ' Stop paddle movement.
            RightPaddle.Velocity.Y = 0

            ' Push paddle above the bottom edge of the client rectangle.
            RightPaddle.Rect.Y = ClientRectangle.Bottom - RightPaddle.Rect.Height

        Else
            ' No, the paddle is not at or past the bottom edge of the client rectangle.

            ' Accelerate paddle down.
            RightPaddle.Velocity.Y += RightPaddle.Acceleration.Y * DeltaTime.TotalSeconds

            ' Limit paddle velocity to the max.
            If RightPaddle.Velocity.Y > RightPaddle.MaxVelocity.Y Then RightPaddle.Velocity.Y = RightPaddle.MaxVelocity.Y

        End If

        If ApplyRightPaddleEnglish Then

            ApplyRightPaddleEnglish = False

            ' Send ball down and to the left.
            Ball.Velocity.X = -ServSpeed
            Ball.Velocity.Y = ServSpeed

        End If

    End Sub

    Private Sub UpdateLeftThumbstickPositionControllerZero()

        ' What position is the left thumbstick in on the Y-axis?
        If ControllerPosition.Gamepad.sThumbLY <= NeutralStart Then
            ' The left thumbstick is in the down position.

            IsConThumbLYNeutral(0) = False

            MoveLeftPaddleDown()

        ElseIf ControllerPosition.Gamepad.sThumbLY >= NeutralEnd Then
            ' The left thumbstick is in the up position.

            IsConThumbLYNeutral(0) = False

            MoveLeftPaddleUp()

        Else
            ' The left thumbstick is in the neutral position.

            IsConThumbLYNeutral(0) = True

        End If

    End Sub

    Private Sub MoveLeftPaddleUp()

        'Is the paddle moving down?
        If LeftPaddle.Velocity.Y > 0 Then
            'Yes, the paddle is moving down.

            'Stop move before changing direction.
            LeftPaddle.Velocity.Y = 0 'Zero speed.

        End If

        If LeftPaddle.Rect.Y <= ClientRectangle.Top Then

            LeftPaddle.Rect.Y = ClientRectangle.Top

            LeftPaddle.Velocity.Y = 0

        Else

            LeftPaddle.Velocity.Y -= LeftPaddle.Acceleration.Y * DeltaTime.TotalSeconds

            'Limit paddle velocity to the max.
            If LeftPaddle.Velocity.Y < -LeftPaddle.MaxVelocity.Y Then LeftPaddle.Velocity.Y = -LeftPaddle.MaxVelocity.Y

        End If

        If ApplyLeftPaddleEnglish Then

            ApplyLeftPaddleEnglish = False

            'Send ball up and to the right.
            Ball.Velocity.X = ServSpeed
            Ball.Velocity.Y = -ServSpeed

        End If

    End Sub

    Private Sub MoveLeftPaddleDown()

        'Is the paddle moving up?
        If LeftPaddle.Velocity.Y < 0 Then
            'Yes, the paddle is moving up.

            'Stop move before changing direction.
            LeftPaddle.Velocity.Y = 0 'Zero speed.

        End If

        If LeftPaddle.Rect.Y + LeftPaddle.Rect.Height >= ClientRectangle.Bottom Then

            LeftPaddle.Rect.Y = ClientRectangle.Bottom - LeftPaddle.Rect.Height

            LeftPaddle.Velocity.Y = 0

        Else

            LeftPaddle.Velocity.Y += LeftPaddle.Acceleration.Y * DeltaTime.TotalSeconds

            'Limit paddle velocity to the max.
            If LeftPaddle.Velocity.Y > LeftPaddle.MaxVelocity.Y Then LeftPaddle.Velocity.Y = LeftPaddle.MaxVelocity.Y

        End If

        If ApplyLeftPaddleEnglish Then

            ApplyLeftPaddleEnglish = False

            'Send ball down and to the right.
            Ball.Velocity.X = ServSpeed
            Ball.Velocity.Y = ServSpeed

        End If

    End Sub

    Private Sub DrawComputerPlayerIdentifier()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

            Buffer.Graphics.DrawString("CPU", InstructionsFont, Brushes.White, ClientSize.Width - (ClientSize.Width \ 4), 20, AlineCenterMiddle)

        End With

    End Sub

    Private Sub DecelerateLeftPaddle()

        'Is the pointer moving up?
        If LeftPaddle.Velocity.Y < 0 Then
            'Yes, the pointer is moving up.

            'Decelerate pointer.
            LeftPaddle.Velocity.Y += LeftPaddle.Acceleration.Y * 2 * DeltaTime.TotalSeconds

            'Limit decelerate to zero speed.
            If LeftPaddle.Velocity.Y > 0 Then LeftPaddle.Velocity.Y = 0 'Zero speed.

        End If

        'Is the pointer moving down?
        If LeftPaddle.Velocity.Y > 0 Then
            'Yes, the pointer is moving down.

            'Decelerate pointer.
            LeftPaddle.Velocity.Y += -LeftPaddle.Acceleration.Y * 2 * DeltaTime.TotalSeconds

            'Limit decelerate to zero speed.
            If LeftPaddle.Velocity.Y < 0 Then LeftPaddle.Velocity.Y = 0 'Zero speed.

        End If

    End Sub

    Private Sub DecelerateRightPaddle()

        'Is the pointer moving up?
        If RightPaddle.Velocity.Y < 0 Then
            'Yes, the pointer is moving up.

            'Decelerate pointer.
            RightPaddle.Velocity.Y += RightPaddle.Acceleration.Y * 2 * DeltaTime.TotalSeconds

            'Limit decelerate to zero speed.
            If RightPaddle.Velocity.Y > 0 Then RightPaddle.Velocity.Y = 0 'Zero speed.

        End If

        'Is the pointer moving down?
        If RightPaddle.Velocity.Y > 0 Then
            'Yes, the pointer is moving down.

            'Decelerate pointer.
            RightPaddle.Velocity.Y += -RightPaddle.Acceleration.Y * 2 * DeltaTime.TotalSeconds

            'Limit decelerate to zero speed.
            If RightPaddle.Velocity.Y < 0 Then RightPaddle.Velocity.Y = 0 'Zero speed.

        End If

    End Sub

    Private Sub UpdateInstructions()

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

        UpdateFrameCounter()

    End Sub

    Private Sub DrawGame()

        DrawBackground()

        Select Case GameState

            Case GameStateEnum.Playing

                DrawPlaying()

            Case GameStateEnum.StartScreen

                DrawBall()

                DrawStartScreen()

            Case GameStateEnum.Instructions

                DrawBall()

                DrawInstructions()

            Case GameStateEnum.Serve

                DrawServe()

            Case GameStateEnum.Pause

                DrawPauseScreen()

            Case GameStateEnum.EndScreen

                DrawEndScreen()

        End Select

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

    Private Sub DrawPausedText()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

            'Draw paused text.
            Buffer.Graphics.DrawString("Paused", TitleFont, Brushes.White, ClientCenter, AlineCenterMiddle)

        End With

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

    Private Sub DrawBackground()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceCopy
            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.None

            .Clear(Color.Black)

        End With

    End Sub

    Private Sub UpdateStartScreen()

    End Sub

    Private Sub DrawStartScreen()

        DrawTitle()

        DrawStartScreenInstructions()

    End Sub

    Private Sub DrawTitle()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality


            .DrawString(TitleText, TitleFont, Brushes.White, TitleLocation, AlineCenter)

            .DrawString(PingPongEmoji, EmojiFont, Brushes.White, EmojiLocation, AlineCenter)


        End With

    End Sub

    Private Sub DrawStartScreenInstructions()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

            .DrawString(InstructStartText, InstructionsFont, Brushes.White, InstructStartLocation, AlineCenter)

        End With

    End Sub

    Private Sub UpdateBallMovement()

        'Move ball horizontally.
        Ball.Position.X += Ball.Velocity.X * DeltaTime.TotalSeconds 'Δs = V * Δt
        'Displacement = Velocity x Delta Time

        Ball.Rect.X = Math.Round(Ball.Position.X)

        'Move our vertically.
        Ball.Position.Y += Ball.Velocity.Y * DeltaTime.TotalSeconds 'Δs = V * Δt
        'Displacement = Velocity x Delta Time

        Ball.Rect.Y = Math.Round(Ball.Position.Y)

    End Sub

    Private Sub Wraparound()

        'When the rectangle exits the right side of the client area.
        If Ball.Rect.X >= ClientRectangle.Right Then

            'The rectangle reappears on the left side the client area.
            Ball.Rect.X = ClientRectangle.Left - Ball.Rect.Width

        End If

        'When the rectangle exits the left side of the client area.
        If Ball.Rect.X <= ClientRectangle.Left Then

            'The rectangle reappears on the right side the client area.
            Ball.Rect.X = ClientRectangle.Right - Ball.Rect.Width

        End If

        Ball.Position.X = Ball.Rect.X

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

        DrawFPSDisplay()

    End Sub

    Private Sub DrawRightPaddleScore()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

            Buffer.Graphics.DrawString(RightPaddleScore, ScoreFont, Brushes.White, RPadScoreLocation, AlineCenterMiddle)

        End With

    End Sub

    Private Sub DrawLeftPaddleScore()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

            Buffer.Graphics.DrawString(LeftPaddleScore, ScoreFont, Brushes.White, LPadScoreLocation, AlineCenterMiddle)

        End With

    End Sub

    Private Sub DrawRightPaddle()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceCopy
            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.None

            .FillRectangle(Brushes.White, RightPaddle.Rect)

        End With

    End Sub

    Private Sub DrawLeftPaddle()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceCopy
            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.None

            .FillRectangle(Brushes.White, LeftPaddle.Rect)

        End With

    End Sub

    Private Sub DrawFPSDisplay()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceOver
            .TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

            'Draw frames per second display.
            Buffer.Graphics.DrawString(FPS.ToString & " FPS", FPSFont, Brushes.MediumOrchid, FPS_Postion)

        End With

    End Sub

    Private Sub DrawCenterCourtLine()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceCopy
            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.None

            .DrawLine(CenterlinePen, CenterlineTop, CenterlineBottom)

        End With

    End Sub

    Private Sub CenterCourtLine()

        'Centers the court line in the client area of our form.
        CenterlineTop = New Point(ClientSize.Width \ 2, 0)

        CenterlineBottom = New Point(ClientSize.Width \ 2, ClientSize.Height)

    End Sub

    Private Sub DrawBall()

        With Buffer.Graphics

            .CompositingMode = Drawing2D.CompositingMode.SourceCopy
            .SmoothingMode = Drawing2D.SmoothingMode.HighSpeed
            .PixelOffsetMode = Drawing2D.PixelOffsetMode.None

            .FillRectangle(Brushes.White, Ball.Rect)

        End With

    End Sub

    Private Sub UpdateControllerData()

        Dim ElapsedTime As TimeSpan = Now - ConnectionStart

        ' Every second check for connected controllers.
        If ElapsedTime.TotalSeconds >= 1 Then

            For controllerNumber As Integer = 0 To 3 ' Up to 4 controllers

                Connected(controllerNumber) = IsControllerConnected(controllerNumber)

            Next

            ConnectionStart = DateTime.Now

        End If

        For controllerNumber As Integer = 0 To 3

            If Connected(controllerNumber) Then

                UpdateControllerState(controllerNumber)

            End If

        Next

    End Sub

    Private Sub UpdateControllerState(controllerNumber As Integer)

        Try

            XInputGetState(controllerNumber, ControllerPosition)

            UpdateButtonPosition(controllerNumber)

            UpdateLeftThumbstickPosition(controllerNumber)

            UpdateRightThumbstickPosition(controllerNumber)

            'UpdateLeftTriggerPosition(controllerNumber)

            'UpdateRightTriggerPosition(controllerNumber)

        Catch ex As Exception
            ' Something went wrong (An exception occurred).

            DisplayError(ex)

        End Try

    End Sub

    Private Sub UpdateButtonPosition(CID As Integer)
        ' The range of buttons is 0 to 65,535. Unsigned 16-bit (2-byte) integer.

        DPadUpPressed = (ControllerPosition.Gamepad.wButtons And DPadUp) <> 0

        DPadDownPressed = (ControllerPosition.Gamepad.wButtons And DPadDown) <> 0

        DPadLeftPressed = (ControllerPosition.Gamepad.wButtons And DPadLeft) <> 0

        DPadRightPressed = (ControllerPosition.Gamepad.wButtons And DPadRight) <> 0

        StartButtonPressed = (ControllerPosition.Gamepad.wButtons And StartButton) <> 0

        BackButtonPressed = (ControllerPosition.Gamepad.wButtons And BackButton) <> 0

        LeftStickButtonPressed = (ControllerPosition.Gamepad.wButtons And LeftStickButton) <> 0

        RightStickButtonPressed = (ControllerPosition.Gamepad.wButtons And RightStickButton) <> 0

        LeftBumperButtonPressed = (ControllerPosition.Gamepad.wButtons And LeftBumperButton) <> 0

        RightBumperButtonPressed = (ControllerPosition.Gamepad.wButtons And RightBumperButton) <> 0

        AButtonPressed = (ControllerPosition.Gamepad.wButtons And AButton) <> 0

        BButtonPressed = (ControllerPosition.Gamepad.wButtons And BButton) <> 0

        XButtonPressed = (ControllerPosition.Gamepad.wButtons And XButton) <> 0

        YButtonPressed = (ControllerPosition.Gamepad.wButtons And YButton) <> 0

        DoButtonLogic(CID)

    End Sub

    Private Sub DoButtonLogic(ControllerNumber As Integer)

        DoDPadLogic(ControllerNumber)

        DoLetterButtonLogic(ControllerNumber)

        DoStartBackLogic(ControllerNumber)

        'DoBumperLogic(ControllerNumber)

        'DoStickLogic(ControllerNumber)

    End Sub

    Private Sub DoLetterButtonLogic(controllerNumber As Integer)

        Select Case GameState

            Case GameStateEnum.StartScreen

                If AButtonPressed Then

                    If Not IsAButtonDown(controllerNumber) Then

                        IsAButtonDown(controllerNumber) = True

                        NumberOfPlayers = 1

                        GameState = GameStateEnum.Instructions

                        MovePointerOffScreen()

                    End If

                Else

                    IsAButtonDown(controllerNumber) = False

                End If

                If BButtonPressed Then

                    NumberOfPlayers = 2

                    GameState = GameStateEnum.Instructions

                    MovePointerOffScreen()

                End If

            Case GameStateEnum.Instructions

                If AButtonPressed Then

                    If Not IsAButtonDown(controllerNumber) Then

                        IsAButtonDown(controllerNumber) = True

                        If IsPlaying("startscreenmusic") = True Then

                            PauseSound("startscreenmusic")

                        End If

                        GameState = GameStateEnum.Serve

                        MovePointerOffScreen()

                    End If

                Else

                    IsAButtonDown(controllerNumber) = False

                End If

            Case GameStateEnum.Playing

            Case GameStateEnum.Serve

            Case GameStateEnum.Pause

            Case GameStateEnum.EndScreen

        End Select

    End Sub

    Private Sub DoStartBackLogic(ControllerNumber As Integer)

        Select Case GameState

            Case GameStateEnum.StartScreen

                If StartButtonPressed Then

                    GameState = GameStateEnum.Instructions

                End If

                If BackButtonPressed Then

                    If Not IsBackButtonDown(ControllerNumber) Then

                        IsBackButtonDown(ControllerNumber) = True

                        Application.Exit()

                    End If

                Else

                    IsBackButtonDown(ControllerNumber) = False

                End If

            Case GameStateEnum.Instructions

                If BackButtonPressed Then

                    If Not IsBackButtonDown(ControllerNumber) Then

                        IsBackButtonDown(ControllerNumber) = True

                        GameState = GameStateEnum.StartScreen

                    End If

                Else

                    IsBackButtonDown(ControllerNumber) = False

                End If

            Case GameStateEnum.Playing

            Case GameStateEnum.Serve

            Case GameStateEnum.Pause

            Case GameStateEnum.EndScreen

        End Select

    End Sub

    Private Function IsControllerConnected(controllerNumber As Integer) As Boolean

        Try

            Return XInputGetState(controllerNumber, ControllerPosition) = 0 ' 0 means the controller is connected.
            ' Anything else (a non-zero value) means the controller is not connected.

        Catch ex As Exception
            ' Something went wrong (An exception occured).

            DisplayError(ex)

            Return False

        End Try

    End Function

    Private Sub DisplayError(ex As Exception)

        MsgBox(ex.ToString()) ' Display the exception message in a message box.

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        InitializeApp()

    End Sub

    Private Sub InitializeApp()

        Ball.Rect.Width = 32
        Ball.Rect.Height = 32
        Ball.Position.X = 960
        Ball.Position.Y = 540
        Ball.Rect.X = Ball.Position.X
        Ball.Rect.Y = Ball.Position.Y
        Ball.Velocity.X = -50
        Ball.Velocity.Y = -50
        Ball.MaxVelocity.X = 500
        Ball.MaxVelocity.Y = 500
        Ball.Acceleration.X = 25
        Ball.Acceleration.Y = 25

        LeftPaddle.Rect.Width = 32
        LeftPaddle.Rect.Height = 128
        LeftPaddle.Position.X = 20
        LeftPaddle.Position.Y = ClientSize.Height \ 2 - LeftPaddle.Rect.Height \ 2 'Center vertically
        LeftPaddle.Rect.X = LeftPaddle.Position.X
        LeftPaddle.Rect.Y = LeftPaddle.Position.Y
        LeftPaddle.Velocity.X = 0
        LeftPaddle.Velocity.Y = 0
        LeftPaddle.MaxVelocity.X = 500
        LeftPaddle.MaxVelocity.Y = 475
        LeftPaddle.Acceleration.X = 0
        LeftPaddle.Acceleration.Y = 2250

        RightPaddle.Rect.Width = 32
        RightPaddle.Rect.Height = 128
        RightPaddle.Position.X = ClientSize.Width - RightPaddle.Rect.Width - 20 'Aline right 20 pix padding
        RightPaddle.Position.Y = ClientSize.Height \ 2 - RightPaddle.Rect.Height \ 2 'Center vertically
        RightPaddle.Rect.X = RightPaddle.Position.X
        RightPaddle.Rect.Y = RightPaddle.Position.Y
        RightPaddle.Velocity.X = 0
        RightPaddle.Velocity.Y = 0
        RightPaddle.MaxVelocity.X = 500
        RightPaddle.MaxVelocity.Y = 475
        RightPaddle.Acceleration.X = 0
        RightPaddle.Acceleration.Y = 2250

        InitializeForm()

        InitializeBuffer()

        CreateSoundFileFromResource()

        AddSound("startscreenmusic", $"{Application.StartupPath}startscreenmusic.mp3")

        SetVolume("startscreenmusic", 300)

        LoopSound("startscreenmusic")

        AddSound("hit", $"{Application.StartupPath}hit.mp3")

        AddOverlapping("bounce", $"{Application.StartupPath}bounce.mp3")

        AddSound("point", $"{Application.StartupPath}point.mp3")

        SetVolume("point", 400)

        AddSound("winning", $"{Application.StartupPath}winning.mp3")

        LayoutTitleAndInstructions()

        Timer1.Interval = 15

        Timer1.Enabled = True

        MovePointerOffScreen()

    End Sub

    Private Sub PlayBounceSound()

        PlayOverlapping("bounce")

    End Sub
    Private Sub PlayPointSound()

        PlaySound("point")

    End Sub

    Private Sub InitializeForm()

        Text = "P🏓NG - Code with Joe"

        SetStyle(ControlStyles.UserPaint, True)

        SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

        SetStyle(ControlStyles.AllPaintingInWmPaint, True)

        Me.WindowState = FormWindowState.Maximized

    End Sub

    Private Sub UpdateDeltaTime()
        'Delta time (Δt) is the elapsed time since the last frame.

        CurrentFrame = Now

        DeltaTime = CurrentFrame - LastFrame 'Calculate delta time

        LastFrame = CurrentFrame 'Update last frame time

    End Sub

    Private Sub UpdateLeftPaddleMovement()

        LeftPaddle.Position.Y += LeftPaddle.Velocity.Y * DeltaTime.TotalSeconds 'Δs = V * Δt
        'Displacement = Velocity x Delta Time

        LeftPaddle.Rect.Y = Math.Round(LeftPaddle.Position.Y)

    End Sub

    Private Sub UpdateRightPaddleMovement()

        RightPaddle.Position.Y += RightPaddle.Velocity.Y * DeltaTime.TotalSeconds 'Δs = V * Δt
        'Displacement = Velocity x Delta Time

        RightPaddle.Rect.Y = Math.Round(RightPaddle.Position.Y)

    End Sub

    Private Sub InitializeBuffer()

        'Set context to the context of this app.
        Context = BufferedGraphicsManager.Current

        'Set buffer size to the primary working area.
        Context.MaximumBuffer = Screen.PrimaryScreen.WorkingArea.Size

        'Create buffer.
        Buffer = Context.Allocate(CreateGraphics(), ClientRectangle)

    End Sub

    Private Sub UpdateFrameCounter()

        TimeElapsed = Now.Subtract(StartTime)

        SecondsElapsed = TimeElapsed.TotalSeconds

        If SecondsElapsed < 1 Then

            FrameCount += 1

        Else

            FPS = FrameCount

            FrameCount = 0

            StartTime = Now

        End If

    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        If WindowState = FormWindowState.Minimized Then

            If GameState = GameStateEnum.Playing Then

                GameState = GameStateEnum.Pause

            End If

        End If

        If GameState = GameStateEnum.StartScreen OrElse GameState = GameStateEnum.Instructions Then

            LayoutTitleAndInstructions()

        End If

        CenterCourtLine()

        LeftPaddle.Position.X = 20
        LeftPaddle.Rect.X = LeftPaddle.Position.X

        RightPaddle.Position.X = ClientSize.Width - RightPaddle.Rect.Width - 20 'Aline right 20 pix padding
        RightPaddle.Rect.X = RightPaddle.Position.X

        'Place the FPS display at the bottom of the client area.
        FPS_Postion.Y = ClientRectangle.Bottom - 75

        LPadScoreLocation = New Point(ClientSize.Width \ 2 \ 2, 100)

        RPadScoreLocation = New Point(ClientSize.Width - (ClientSize.Width \ 4), 100)

        ClientCenter = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2)

    End Sub

    Private Sub LayoutTitleAndInstructions()

        TitleLocation = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 175)

        Ball.Position.Y = ClientSize.Height \ 2 + 40
        Ball.Rect.Y = Ball.Position.Y

        InstructStartLocation = New Point(ClientSize.Width \ 2, (ClientSize.Height \ 2) + 35)

        InstructOneLocation = New Point(ClientSize.Width \ 2, (ClientSize.Height \ 2) + 35)

        InstructTwoLocation = New Point(ClientSize.Width \ 2, (ClientSize.Height \ 2) + 35)

        EmojiLocation = New Point(ClientSize.Width \ 2 - 90, ClientSize.Height \ 2 - 180)

    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing

        GameLoopCancellationToken.Cancel(True)

        CloseSounds()

    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)

        'Intentionally left blank. Do not remove.

    End Sub

    Private Function AddSound(SoundName As String, FilePath As String) As Boolean

        'Do we have a name and does the file exist?
        If Not String.IsNullOrWhiteSpace(SoundName) AndAlso IO.File.Exists(FilePath) Then
            'Yes, we have a name and the file exists.

            Dim CommandOpen As String = $"open ""{FilePath}"" alias {SoundName}"

            Dim ReturnString As New StringBuilder(128)

            'Do we have sounds?
            If Sounds IsNot Nothing Then
                'Yes, we have sounds.

                'Is the sound in the array already?
                If Not Sounds.Contains(SoundName) Then
                    'No, the sound is not in the array.

                    'Did the sound file open?
                    If mciSendStringW(CommandOpen, ReturnString, 0, IntPtr.Zero) = 0 Then
                        'Yes, the sound file did open.

                        'Add the sound to the Sounds array.
                        Array.Resize(Sounds, Sounds.Length + 1)

                        Sounds(Sounds.Length - 1) = SoundName

                        Return True 'The sound was added.

                    End If

                End If

            Else
                'No, we do not have sounds.

                'Did the sound file open?
                If mciSendStringW(CommandOpen, ReturnString, 0, IntPtr.Zero) = 0 Then
                    'Yes, the sound file did open.

                    'Start the Sounds array with the sound.
                    ReDim Sounds(0)

                    Sounds(0) = SoundName

                    Return True 'The sound was added.

                End If

            End If

        End If

        Return False 'The sound was not added.

    End Function

    Private Function SetVolume(SoundName As String, Level As Integer) As Boolean

        'Do we have sounds?
        If Sounds IsNot Nothing Then
            'Yes, we have sounds.

            'Is the sound in the sounds array?
            If Sounds.Contains(SoundName) Then
                'Yes, the sound is the sounds array.

                'Is the level in the valid range?
                If Level >= 0 AndAlso Level <= 1000 Then
                    'Yes, the level is in range.

                    Dim CommandVolume As String = $"setaudio {SoundName} volume to {Level}"

                    Dim ReturnString As New StringBuilder(128)

                    'Was the volume set?
                    If mciSendStringW(CommandVolume, ReturnString, 0, IntPtr.Zero) = 0 Then

                        Return True 'The volume was set.

                    End If

                End If

            End If

        End If

        Return False 'The volume was not set.

    End Function

    Private Function LoopSound(SoundName As String) As Boolean

        ' Do we have sounds?
        If Sounds IsNot Nothing Then
            ' Yes, we have sounds.

            ' Is the sound in the array?
            If Not Sounds.Contains(SoundName) Then
                ' No, the sound is not in the array.

                Return False ' The sound is not playing.

            End If

            Dim CommandSeekToStart As String = $"seek {SoundName} to start"

            Dim ReturnString As New StringBuilder(128)

            mciSendStringW(CommandSeekToStart, ReturnString, 0, IntPtr.Zero)

            Dim CommandPlayRepete As String = $"play {SoundName} repeat"

            If mciSendStringW(CommandPlayRepete, ReturnString, 0, Me.Handle) <> 0 Then

                Return False ' The sound is not playing.

            End If

        End If

        Return True ' The sound is playing.

    End Function

    Private Function PlaySound(SoundName As String) As Boolean

        'Do we have sounds?
        If Sounds IsNot Nothing Then
            'Yes, we have sounds.

            'Is the sound in the array?
            If Sounds.Contains(SoundName) Then
                'Yes, the sound is in the array.

                Dim CommandSeekToStart As String = $"seek {SoundName} to start"

                Dim ReturnString As New StringBuilder(128)

                mciSendStringW(CommandSeekToStart, ReturnString, 0, IntPtr.Zero)

                Dim CommandPlay As String = $"play {SoundName} notify"

                If mciSendStringW(CommandPlay, ReturnString, 0, Me.Handle) = 0 Then

                    Return True 'The sound is playing.

                End If

            End If

        End If

        Return False 'The sound is not playing.

    End Function

    Private Function PauseSound(SoundName As String) As Boolean

        'Do we have sounds?
        If Sounds IsNot Nothing Then
            'Yes, we have sounds.

            'Is the sound in the array?
            If Sounds.Contains(SoundName) Then
                'Yes, the sound is in the array.

                Dim CommandPause As String = $"pause {SoundName} notify"

                Dim ReturnString As New StringBuilder(128)

                If mciSendStringW(CommandPause, ReturnString, 0, Me.Handle) = 0 Then

                    Return True 'The sound is paused.

                End If

            End If

        End If

        Return False 'The sound is not paused.

    End Function

    Private Function IsPlaying(SoundName As String) As Boolean

        Return GetStatus(SoundName, "mode") = "playing"

    End Function

    Private Sub AddOverlapping(SoundName As String, FilePath As String)

        For Each Suffix As String In {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"}

            AddSound(SoundName & Suffix, FilePath)

        Next

    End Sub

    Private Sub PlayOverlapping(SoundName As String)

        For Each Suffix As String In {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"}

            If Not IsPlaying(SoundName & Suffix) Then

                PlaySound(SoundName & Suffix)

                Exit Sub

            End If

        Next

    End Sub

    Private Sub SetVolumeOverlapping(SoundName As String, Level As Integer)

        For Each Suffix As String In {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"}

            SetVolume(SoundName & Suffix, Level)

        Next

    End Sub

    Private Function GetStatus(SoundName As String, StatusType As String) As String

        If Sounds IsNot Nothing Then

            If Sounds.Contains(SoundName) Then

                Dim Command As String = $"status {SoundName} {StatusType}"

                Dim ReturnString As New StringBuilder(128)

                ' Send the command to get the status
                If mciSendStringW(Command, ReturnString, ReturnString.Capacity, IntPtr.Zero) = 0 Then

                    Return ReturnString.ToString() ' Return the status if successful

                End If

            End If

        End If

        Return String.Empty ' Return an empty string if there's an error

    End Function

    Private Sub CloseSounds()

        If Sounds IsNot Nothing Then

            For Each Sound In Sounds

                Dim CommandClose As String = $"close {Sound}"

                Dim ReturnString As New StringBuilder(128)

                mciSendStringW(CommandClose, ReturnString, 0, IntPtr.Zero)

            Next

        End If

    End Sub

    Private Sub CreateSoundFileFromResource()

        Dim FilePath As String = Path.Combine(Application.StartupPath, "startscreenmusic.mp3")

        If Not IO.File.Exists(FilePath) Then

            IO.File.WriteAllBytes(FilePath, My.Resources.Start_Screen_Music_7___Pong)

        End If

        FilePath = Path.Combine(Application.StartupPath, "hit.mp3")

        If Not IO.File.Exists(FilePath) Then

            IO.File.WriteAllBytes(FilePath, My.Resources.Hit2)

        End If

        FilePath = Path.Combine(Application.StartupPath, "bounce.mp3")

        If Not IO.File.Exists(FilePath) Then

            IO.File.WriteAllBytes(FilePath, My.Resources.Bounce2)

        End If

        FilePath = Path.Combine(Application.StartupPath, "point.mp3")

        If Not IO.File.Exists(FilePath) Then

            IO.File.WriteAllBytes(FilePath, My.Resources.Point2)

        End If

        FilePath = Path.Combine(Application.StartupPath, "winning.mp3")

        If Not IO.File.Exists(FilePath) Then

            IO.File.WriteAllBytes(FilePath, My.Resources.Match2)

        End If

    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        DoKeyDown(e)

    End Sub

    Private Sub DoKeyDown(e As KeyEventArgs)

        Select Case e.KeyCode

            Case Keys.Space

                SpaceBarDown = True

            Case Keys.W

                WKeyDown = True

            Case Keys.S

                SKeyDown = True

            Case Keys.Up

                UpArrowKeyDown = True

            Case Keys.Down

                DownArrowKeyDown = True

            Case Keys.D1

                OneKeyDown = True

            Case Keys.NumPad1

                OneKeyDown = True

            Case Keys.D2

                TwoKeyDown = True

            Case Keys.NumPad2

                TwoKeyDown = True

            Case Keys.P

                PKeyDown = True

            Case Keys.A

                AKeyDown = True

            Case Keys.B

                BKeyDown = True

            Case Keys.X

                XKeyDown = True

        End Select

    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp

        DoKeyUp(e)

    End Sub

    Private Sub DoKeyUp(e As KeyEventArgs)

        Select Case e.KeyCode

            Case Keys.Space

                SpaceBarDown = False

            Case Keys.W

                WKeyDown = False

            Case Keys.S

                SKeyDown = False

            Case Keys.Up

                UpArrowKeyDown = False

            Case Keys.Down

                DownArrowKeyDown = False

            Case Keys.D1

                OneKeyDown = False

            Case Keys.NumPad1

                OneKeyDown = False

            Case Keys.D2

                TwoKeyDown = False

            Case Keys.NumPad2

                TwoKeyDown = False

            Case Keys.P
                PKeyDown = False

            Case Keys.A

                AKeyDown = False

            Case Keys.B

                BKeyDown = False

            Case Keys.X

                XKeyDown = False

        End Select

    End Sub

End Class

'Learn more:
'
'Consuming Unmanaged DLL Functions
'https://learn.microsoft.com/en-us/dotnet/framework/interop/consuming-unmanaged-dll-functions
'
'XInputGetState Function
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputgetstate
'
'XINPUT_STATE Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_state
'
'XINPUT_GAMEPAD Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_gamepad
'
'XInputSetState Function
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/nf-xinput-xinputsetstate
'
'XINPUT_VIBRATION Structure
'https://learn.microsoft.com/en-us/windows/win32/api/xinput/ns-xinput-xinput_vibration
'
'Getting Started with XInput in Windows Applications
'https://learn.microsoft.com/en-us/windows/win32/xinput/getting-started-with-xinput
'
'XInput Game Controller APIs
'https://learn.microsoft.com/en-us/windows/win32/api/_xinput/
'
'XInput Versions
'https://learn.microsoft.com/en-us/windows/win32/xinput/xinput-versions
'
'Comparison of XInput and DirectInput Features
'https://learn.microsoft.com/en-us/windows/win32/xinput/xinput-and-directinput
'
'Short Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/short-data-type
'
'Byte Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/byte-data-type
'
'UShort Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/ushort-data-type
'
'UInteger Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/uinteger-data-type
'
'Boolean Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/boolean-data-type
'
'Integer Data Type
'https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/data-types/integer-data-type
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
'