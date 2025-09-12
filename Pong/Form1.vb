' P🏓NG is a simulation of Table Tennis, a recreational activity and an
' Olympic sport since 1988 is also known by the term "ping-pong" or just "pong".

' This repository is designed to help new game developers learn the fundamentals
' of game programming and design through a classic game.

' Features
'
'   Classic Gameplay: Experience the timeless fun of ping-pong with modern
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

' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:

' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.

' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.


Imports System.Numerics
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO

Public Structure XboxControllers

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputGetState(dwUserIndex As Integer,
                                     ByRef pState As XINPUT_STATE) As Integer
    End Function

    <StructLayout(LayoutKind.Explicit)>
    Private Structure XINPUT_STATE

        <FieldOffset(0)>
        Public dwPacketNumber As UInteger ' Unsigned integer range 0 through 4,294,967,295.
        <FieldOffset(4)>
        Public Gamepad As XINPUT_GAMEPAD
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure XINPUT_GAMEPAD

        Public wButtons As UShort ' Unsigned integer range 0 through 65,535.
        Public bLeftTrigger As Byte ' Unsigned integer range 0 through 255.
        Public bRightTrigger As Byte
        Public sThumbLX As Short ' Signed integer range -32,768 through 32,767.
        Public sThumbLY As Short
        Public sThumbRX As Short
        Public sThumbRY As Short
    End Structure

    Private State As XINPUT_STATE

    Private Enum Button

        DPadUp = 1
        DPadDown = 2
        DPadLeft = 4
        DPadRight = 8
        Start = 16
        Back = 32
        LeftStick = 64
        RightStick = 128
        LeftBumper = 256
        RightBumper = 512
        A = 4096
        B = 8192
        X = 16384
        Y = 32768
    End Enum

    ' Set the start of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralStart As Short = -16384 ' -16,384 = -32,768 / 2
    ' The thumbstick position must be more than 1/2 over the neutral start to
    ' register as moved.
    ' A short is a signed 16-bit (2-byte) integer range -32,768 through 32,767.
    ' This gives us 65,536 values.

    ' Set the end of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralEnd As Short = 16384 ' 16,383.5 = 32,767 / 2
    ' The thumbstick position must be more than 1/2 over the neutral end to
    ' register as moved.

    ' Set the trigger threshold to 1/4 pull.
    Private Const TriggerThreshold As Byte = 64 ' 64 = 256 / 4
    ' The trigger position must be greater than the trigger threshold to
    ' register as pulled.
    ' A byte is a unsigned 8-bit (1-byte) integer range 0 through 255.
    ' This gives us 256 values.

    Public Connected() As Boolean

    Private ConnectionStart As Date

    Public Buttons() As UShort

    Public LeftThumbstickXaxisNeutral() As Boolean
    Public LeftThumbstickYaxisNeutral() As Boolean

    Public RightThumbstickXaxisNeutral() As Boolean
    Public RightThumbstickYaxisNeutral() As Boolean

    Public DPadNeutral() As Boolean

    Public LetterButtonsNeutral() As Boolean

    Public DPadUp() As Boolean
    Public DPadDown() As Boolean
    Public DPadLeft() As Boolean
    Public DPadRight() As Boolean

    Public Start() As Boolean
    Public Back() As Boolean

    Public LeftStick() As Boolean
    Public RightStick() As Boolean

    Public LeftBumper() As Boolean
    Public RightBumper() As Boolean

    Public A() As Boolean
    Public B() As Boolean
    Public X() As Boolean
    Public Y() As Boolean

    Public RightThumbstickUp() As Boolean
    Public RightThumbstickDown() As Boolean
    Public RightThumbstickLeft() As Boolean
    Public RightThumbstickRight() As Boolean

    Public LeftThumbstickUp() As Boolean
    Public LeftThumbstickDown() As Boolean
    Public LeftThumbstickLeft() As Boolean
    Public LeftThumbstickRight() As Boolean

    Public LeftTrigger() As Boolean
    Public RightTrigger() As Boolean

    Public TimeToVibe As Integer

    Private LeftVibrateStart() As Date

    Private RightVibrateStart() As Date

    Private IsLeftVibrating() As Boolean

    Private IsRightVibrating() As Boolean

    Public Sub Initialize()

        ' Initialize the Connected array to indicate whether controllers are connected.
        Connected = New Boolean(0 To 3) {}

        ' Record the current date and time when initialization starts.
        ConnectionStart = DateTime.Now

        ' Initialize the Buttons array to store the state of controller buttons.
        Buttons = New UShort(0 To 3) {}

        ' Initialize arrays to check if thumbstick axes are in the neutral position.
        LeftThumbstickXaxisNeutral = New Boolean(0 To 3) {}
        LeftThumbstickYaxisNeutral = New Boolean(0 To 3) {}
        RightThumbstickXaxisNeutral = New Boolean(0 To 3) {}
        RightThumbstickYaxisNeutral = New Boolean(0 To 3) {}

        ' Initialize array to check if the D-Pad is in the neutral position.
        DPadNeutral = New Boolean(0 To 3) {}

        ' Initialize array to check if letter buttons are in the neutral position.
        LetterButtonsNeutral = New Boolean(0 To 3) {}

        ' Set all thumbstick axes, triggers, D-Pad, letter buttons, start/back buttons,
        ' bumpers,and stick buttons to neutral for all controllers (indices 0 to 3).
        For i As Integer = 0 To 3

            LeftThumbstickXaxisNeutral(i) = True
            LeftThumbstickYaxisNeutral(i) = True
            RightThumbstickXaxisNeutral(i) = True
            RightThumbstickYaxisNeutral(i) = True

            DPadNeutral(i) = True

            LetterButtonsNeutral(i) = True

        Next

        ' Initialize arrays for thumbstick directional states.
        RightThumbstickLeft = New Boolean(0 To 3) {}
        RightThumbstickRight = New Boolean(0 To 3) {}
        RightThumbstickDown = New Boolean(0 To 3) {}
        RightThumbstickUp = New Boolean(0 To 3) {}
        LeftThumbstickLeft = New Boolean(0 To 3) {}
        LeftThumbstickRight = New Boolean(0 To 3) {}
        LeftThumbstickDown = New Boolean(0 To 3) {}
        LeftThumbstickUp = New Boolean(0 To 3) {}

        ' Initialize arrays for trigger states.
        LeftTrigger = New Boolean(0 To 3) {}
        RightTrigger = New Boolean(0 To 3) {}

        ' Initialize arrays for letter button states (A, B, X, Y).
        A = New Boolean(0 To 3) {}
        B = New Boolean(0 To 3) {}
        X = New Boolean(0 To 3) {}
        Y = New Boolean(0 To 3) {}

        ' Initialize arrays for bumper button states.
        LeftBumper = New Boolean(0 To 3) {}
        RightBumper = New Boolean(0 To 3) {}

        ' Initialize arrays for D-Pad directional states.
        DPadUp = New Boolean(0 To 3) {}
        DPadDown = New Boolean(0 To 3) {}
        DPadLeft = New Boolean(0 To 3) {}
        DPadRight = New Boolean(0 To 3) {}

        ' Initialize arrays for start and back button states.
        Start = New Boolean(0 To 3) {}
        Back = New Boolean(0 To 3) {}

        ' Initialize arrays for stick button states.
        LeftStick = New Boolean(0 To 3) {}
        RightStick = New Boolean(0 To 3) {}

        TimeToVibe = 400 'ms

        LeftVibrateStart = New Date(0 To 3) {}
        RightVibrateStart = New Date(0 To 3) {}

        For ControllerNumber As Integer = 0 To 3

            LeftVibrateStart(ControllerNumber) = Now

            RightVibrateStart(ControllerNumber) = Now

        Next

        IsLeftVibrating = New Boolean(0 To 3) {}
        IsRightVibrating = New Boolean(0 To 3) {}

        ' Call the TestInitialization method to verify the initial state of the controllers.
        TestInitialization()

    End Sub

    Public Sub Update()

        Dim ElapsedTime As TimeSpan = Now - ConnectionStart

        ' Every second check for connected controllers.
        If ElapsedTime.TotalSeconds >= 1 Then

            For ControllerNumber As Integer = 0 To 3 ' Up to 4 controllers

                Connected(ControllerNumber) = IsConnected(ControllerNumber)

            Next

            ConnectionStart = DateTime.Now

        End If

        For ControllerNumber As Integer = 0 To 3

            If Connected(ControllerNumber) Then

                UpdateState(ControllerNumber)

            End If

        Next

        UpdateVibrateTimers()

    End Sub

    Private Sub UpdateState(controllerNumber As Integer)

        Try

            XInputGetState(controllerNumber, State)

            UpdateButtons(controllerNumber)

            UpdateThumbsticks(controllerNumber)

            UpdateTriggers(controllerNumber)

        Catch ex As Exception
            ' Something went wrong (An exception occurred).

            Debug.Print($"Error getting XInput state: {controllerNumber} | {ex.Message}")

        End Try

    End Sub

    Private Sub UpdateButtons(CID As Integer)

        UpdateDPadButtons(CID)

        UpdateLetterButtons(CID)

        UpdateBumperButtons(CID)

        UpdateStickButtons(CID)

        UpdateStartBackButtons(CID)

        UpdateDPadNeutral(CID)

        UpdateLetterButtonsNeutral(CID)

        Buttons(CID) = State.Gamepad.wButtons

    End Sub

    Private Sub UpdateThumbsticks(controllerNumber As Integer)

        UpdateLeftThumbstick(controllerNumber)

        UpdateRightThumbstick(controllerNumber)

    End Sub

    Private Sub UpdateTriggers(controllerNumber As Integer)

        UpdateLeftTrigger(controllerNumber)

        UpdateRightTrigger(controllerNumber)

    End Sub

    Private Sub UpdateDPadButtons(CID As Integer)

        DPadUp(CID) = (State.Gamepad.wButtons And Button.DPadUp) <> 0
        DPadDown(CID) = (State.Gamepad.wButtons And Button.DPadDown) <> 0
        DPadLeft(CID) = (State.Gamepad.wButtons And Button.DPadLeft) <> 0
        DPadRight(CID) = (State.Gamepad.wButtons And Button.DPadRight) <> 0

    End Sub

    Private Sub UpdateLetterButtons(CID As Integer)

        A(CID) = (State.Gamepad.wButtons And Button.A) <> 0
        B(CID) = (State.Gamepad.wButtons And Button.B) <> 0
        X(CID) = (State.Gamepad.wButtons And Button.X) <> 0
        Y(CID) = (State.Gamepad.wButtons And Button.Y) <> 0

    End Sub

    Private Sub UpdateBumperButtons(CID As Integer)

        LeftBumper(CID) = (State.Gamepad.wButtons And Button.LeftBumper) <> 0
        RightBumper(CID) = (State.Gamepad.wButtons And Button.RightBumper) <> 0

    End Sub

    Private Sub UpdateStickButtons(CID As Integer)

        LeftStick(CID) = (State.Gamepad.wButtons And Button.LeftStick) <> 0
        RightStick(CID) = (State.Gamepad.wButtons And Button.RightStick) <> 0

    End Sub

    Private Sub UpdateStartBackButtons(CID As Integer)

        Start(CID) = (State.Gamepad.wButtons And Button.Start) <> 0
        Back(CID) = (State.Gamepad.wButtons And Button.Back) <> 0

    End Sub

    Private Sub UpdateLeftThumbstick(ControllerNumber As Integer)

        UpdateLeftThumbstickXaxis(ControllerNumber)

        UpdateLeftThumbstickYaxis(ControllerNumber)

    End Sub

    Private Sub UpdateLeftThumbstickYaxis(ControllerNumber As Integer)
        ' The range on the Y-axis is -32,768 through 32,767.
        ' Signed 16-bit (2-byte) integer.

        ' What position is the left thumbstick in on the Y-axis?
        If State.Gamepad.sThumbLY <= NeutralStart Then
            ' The left thumbstick is in the down position.

            LeftThumbstickUp(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = False

            LeftThumbstickDown(ControllerNumber) = True

        ElseIf State.Gamepad.sThumbLY >= NeutralEnd Then
            ' The left thumbstick is in the up position.

            LeftThumbstickDown(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = False

            LeftThumbstickUp(ControllerNumber) = True

        Else
            ' The left thumbstick is in the neutral position.

            LeftThumbstickUp(ControllerNumber) = False

            LeftThumbstickDown(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateLeftThumbstickXaxis(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767.
        ' sThumbLX is a signed 16-bit (2-byte) integer.

        ' What position is the left thumbstick in on the X-axis?
        If State.Gamepad.sThumbLX <= NeutralStart Then
            ' The left thumbstick is in the left position.

            LeftThumbstickRight(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = False

            LeftThumbstickLeft(ControllerNumber) = True

        ElseIf State.Gamepad.sThumbLX >= NeutralEnd Then
            ' The left thumbstick is in the right position.

            LeftThumbstickLeft(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = False

            LeftThumbstickRight(ControllerNumber) = True

        Else
            ' The left thumbstick is in the neutral position.

            LeftThumbstickLeft(ControllerNumber) = False

            LeftThumbstickRight(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateRightThumbstick(ControllerNumber As Integer)

        UpdateRightThumbstickXaxis(ControllerNumber)

        UpdateRightThumbstickYaxis(ControllerNumber)

    End Sub

    Private Sub UpdateRightThumbstickYaxis(ControllerNumber As Integer)
        ' The range on the Y-axis is -32,768 through 32,767.
        ' sThumbRY is a signed 16-bit (2-byte) integer.

        ' What position is the right thumbstick in on the Y-axis?
        If State.Gamepad.sThumbRY <= NeutralStart Then
            ' The right thumbstick is in the down position.

            RightThumbstickUp(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = False

            RightThumbstickDown(ControllerNumber) = True

        ElseIf State.Gamepad.sThumbRY >= NeutralEnd Then
            ' The right thumbstick is in the up position.

            RightThumbstickDown(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = False

            RightThumbstickUp(ControllerNumber) = True

        Else
            ' The right thumbstick is in the neutral position.

            RightThumbstickUp(ControllerNumber) = False

            RightThumbstickDown(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateRightThumbstickXaxis(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767.
        ' Signed 16-bit (2-byte) integer.

        ' What position is the right thumbstick in on the X-axis?
        If State.Gamepad.sThumbRX <= NeutralStart Then
            ' The right thumbstick is in the left position.

            RightThumbstickRight(ControllerNumber) = False

            RightThumbstickXaxisNeutral(ControllerNumber) = False

            RightThumbstickLeft(ControllerNumber) = True

        ElseIf State.Gamepad.sThumbRX >= NeutralEnd Then
            ' The right thumbstick is in the right position.

            RightThumbstickLeft(ControllerNumber) = False

            RightThumbstickXaxisNeutral(ControllerNumber) = False

            RightThumbstickRight(ControllerNumber) = True

        Else
            ' The right thumbstick is in the neutral position.

            RightThumbstickLeft(ControllerNumber) = False

            RightThumbstickRight(ControllerNumber) = False

            RightThumbstickXaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateRightTrigger(ControllerNumber As Integer)
        ' The range of right trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        ' The trigger position must be greater than the trigger threshold to
        ' register as pressed.

        ' What position is the right trigger in?
        If State.Gamepad.bRightTrigger > TriggerThreshold Then
            ' The right trigger is in the down position. Trigger Break. Bang!

            RightTrigger(ControllerNumber) = True

        Else
            ' The right trigger is in the neutral position. Pre-Travel.

            RightTrigger(ControllerNumber) = False

        End If

    End Sub

    Private Sub UpdateLeftTrigger(ControllerNumber As Integer)
        ' The range of left trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        ' The trigger position must be greater than the trigger threshold to
        ' register as pressed.

        ' What position is the left trigger in?
        If State.Gamepad.bLeftTrigger > TriggerThreshold Then
            ' The left trigger is in the fire position. Trigger Break. Bang!

            LeftTrigger(ControllerNumber) = True

        Else
            ' The left trigger is in the neutral position. Pre-Travel.

            LeftTrigger(ControllerNumber) = False

        End If

    End Sub

    Private Sub UpdateDPadNeutral(controllerNumber As Integer)

        If DPadDown(controllerNumber) Or
           DPadLeft(controllerNumber) Or
           DPadRight(controllerNumber) Or
           DPadUp(controllerNumber) Then

            DPadNeutral(controllerNumber) = False

        Else

            DPadNeutral(controllerNumber) = True

        End If

    End Sub

    Private Sub UpdateLetterButtonsNeutral(controllerNumber As Integer)

        If A(controllerNumber) Or
           B(controllerNumber) Or
           X(controllerNumber) Or
           Y(controllerNumber) Then

            LetterButtonsNeutral(controllerNumber) = False

        Else

            LetterButtonsNeutral(controllerNumber) = True

        End If

    End Sub

    Private Function IsConnected(controllerNumber As Integer) As Boolean

        Try

            Return XInputGetState(controllerNumber, State) = 0
            ' 0 means the controller is connected.
            ' Anything else (a non-zero value) means the controller is not
            ' connected.

        Catch ex As Exception
            ' Something went wrong (An exception occured).

            Debug.Print($"Error getting XInput state: {controllerNumber} | {ex.Message}")

            Return False

        End Try

    End Function

    Private Sub TestInitialization()

        ' Check that ConnectionStart is not Nothing (initialization was successful)
        Debug.Assert(Not ConnectionStart = Nothing,
                     $"Connection Start should not be Nothing.")

        ' Check that Buttons array is initialized
        Debug.Assert(Buttons IsNot Nothing,
                     $"Buttons should not be Nothing.")

        Debug.Assert(Not TimeToVibe = Nothing,
                     $"TimeToVibe should not be Nothing.")

        For i As Integer = 0 To 3

            ' Check that all controllers are initialized as not connected
            Debug.Assert(Not Connected(i),
                         $"Controller {i} should not be connected after initialization.")

            ' Check that all axes of the Left Thumbsticks are initialized as neutral. 
            Debug.Assert(LeftThumbstickXaxisNeutral(i),
                         $"Left Thumbstick X-axis for Controller {i} should be neutral.")
            Debug.Assert(LeftThumbstickYaxisNeutral(i),
                         $"Left Thumbstick Y-axis for Controller {i} should be neutral.")

            ' Check that all axes of the Right Thumbsticks are initialized as neutral. 
            Debug.Assert(RightThumbstickXaxisNeutral(i),
                         $"Right Thumbstick X-axis for Controller {i} should be neutral.")
            Debug.Assert(RightThumbstickYaxisNeutral(i),
                         $"Right Thumbstick Y-axis for Controller {i} should be neutral.")

            ' Check that all DPads are initialized as neutral. 
            Debug.Assert(DPadNeutral(i),
                         $"DPad for Controller {i} should be neutral.")

            ' Check that all Letter Buttons are initialized as neutral. 
            Debug.Assert(LetterButtonsNeutral(i),
                         $"Letter Buttons for Controller {i} should be neutral.")

            ' Check that additional Right Thumbstick states are not active.
            Debug.Assert(Not RightThumbstickLeft(i),
                         $"Right Thumbstick Left for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickRight(i),
                         $"Right Thumbstick Right for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickDown(i),
                         $"Right Thumbstick Down for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickUp(i),
                         $"Right Thumbstick Up for Controller {i} should not be true.")

            ' Check that additional Left Thumbstick states are not active.
            Debug.Assert(Not LeftThumbstickLeft(i),
                         $"Left Thumbstick Left for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickRight(i),
                         $"Left Thumbstick Right for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickDown(i),
                         $"Left Thumbstick Down for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickUp(i),
                         $"Left Thumbstick Up for Controller {i} should not be true.")

            ' Check that trigger states are not active.
            Debug.Assert(Not LeftTrigger(i),
                         $"Left Trigger for Controller {i} should not be true.")
            Debug.Assert(Not RightTrigger(i),
                         $"Right Trigger for Controller {i} should not be true.")

            ' Check that letter button states (A, B, X, Y) are not active.
            Debug.Assert(Not A(i),
                         $"A for Controller {i} should not be true.")
            Debug.Assert(Not B(i),
                         $"B for Controller {i} should not be true.")
            Debug.Assert(Not X(i),
                         $"X for Controller {i} should not be true.")
            Debug.Assert(Not Y(i),
                         $"Y for Controller {i} should not be true.")

            ' Check that bumper button states are not active.
            Debug.Assert(Not LeftBumper(i),
                         $"Left Bumper for Controller {i} should not be true.")
            Debug.Assert(Not RightBumper(i),
                         $"Right Bumper for Controller {i} should not be true.")

            ' Check that D-Pad directional states are not active.
            Debug.Assert(Not DPadUp(i),
                         $"D-Pad Up for Controller {i} should not be true.")
            Debug.Assert(Not DPadDown(i),
                         $"D-Pad Down for Controller {i} should not be true.")
            Debug.Assert(Not DPadLeft(i),
                         $"D-Pad Left for Controller {i} should not be true.")
            Debug.Assert(Not DPadRight(i),
                         $"D-Pad Right for Controller {i} should not be true.")

            ' Check that start and back button states are not active.
            Debug.Assert(Not Start(i),
                         $"Start Button for Controller {i} should not be true.")
            Debug.Assert(Not Back(i),
                         $"Back Button for Controller {i} should not be true.")

            ' Check that stick button states are not active.
            Debug.Assert(Not LeftStick(i),
                         $"Left Stick for Controller {i} should not be true.")
            Debug.Assert(Not RightStick(i),
                         $"Right Stick for Controller {i} should not be true.")

            Debug.Assert(Not LeftVibrateStart(i) = Nothing,
                         $"Left Vibrate Start for Controller {i} should not be Nothing.")
            Debug.Assert(Not RightVibrateStart(i) = Nothing,
                         $"Right Vibrate Start for Controller {i} should not be Nothing.")

            Debug.Assert(Not IsLeftVibrating(i),
                         $"Is Left Vibrating for Controller {i} should not be true.")
            Debug.Assert(Not IsRightVibrating(i),
                         $"Is Right Vibrating for Controller {i} should not be true.")

        Next

        ' For Lex

    End Sub

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputSetState(playerIndex As Integer,
                                     ByRef vibration As XINPUT_VIBRATION) As Integer
    End Function

    Private Structure XINPUT_VIBRATION

        Public wLeftMotorSpeed As UShort
        Public wRightMotorSpeed As UShort
    End Structure

    Private Vibration As XINPUT_VIBRATION

    Public Sub VibrateLeft(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The left motor is the low-frequency rumble motor.

        ' Set left motor speed.
        Vibration.wLeftMotorSpeed = Speed

        LeftVibrateStart(CID) = Now

        IsLeftVibrating(CID) = True

    End Sub

    Public Sub VibrateRight(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The right motor is the high-frequency rumble motor.

        ' Set right motor speed.
        Vibration.wRightMotorSpeed = Speed

        RightVibrateStart(CID) = Now

        IsRightVibrating(CID) = True

    End Sub

    Private Sub SendVibrationMotorCommand(ControllerID As Integer)
        ' Sends vibration motor speed command to the specified controller.

        Try

            ' Send motor speed command to the specified controller.
            If XInputSetState(ControllerID, Vibration) = 0 Then
                ' The motor speed was set. Success.

                Debug.Print($"{ControllerID} did vibrate.  {Vibration.wLeftMotorSpeed} |  {Vibration.wRightMotorSpeed} ")

            Else
                ' The motor speed was not set. Fail.

                Debug.Print($"{ControllerID} did not vibrate.  {Vibration.wLeftMotorSpeed} |  {Vibration.wRightMotorSpeed} ")

            End If

        Catch ex As Exception

            Debug.Print($"Error sending vibration motor command: {ControllerID} | {Vibration.wLeftMotorSpeed} |  {Vibration.wRightMotorSpeed} | {ex.Message}")

            Exit Sub

        End Try

    End Sub

    Private Sub UpdateVibrateTimers()

        UpdateLeftVibrateTimer()

        UpdateRightVibrateTimer()

    End Sub

    Private Sub UpdateLeftVibrateTimer()

        For ControllerNumber As Integer = 0 To 3

            If IsLeftVibrating(ControllerNumber) Then

                Dim ElapsedTime As TimeSpan = Now - LeftVibrateStart(ControllerNumber)

                If ElapsedTime.TotalMilliseconds >= TimeToVibe Then

                    IsLeftVibrating(ControllerNumber) = False

                    ' Turn left motor off (set zero speed).
                    Vibration.wLeftMotorSpeed = 0

                End If

                SendVibrationMotorCommand(ControllerNumber)

            End If

        Next

    End Sub

    Private Sub UpdateRightVibrateTimer()

        For ControllerNumber As Integer = 0 To 3

            If IsRightVibrating(ControllerNumber) Then

                Dim ElapsedTime As TimeSpan = Now - RightVibrateStart(ControllerNumber)

                If ElapsedTime.TotalMilliseconds >= TimeToVibe Then

                    IsRightVibrating(ControllerNumber) = False

                    ' Turn left motor off (set zero speed).
                    Vibration.wRightMotorSpeed = 0

                End If

                SendVibrationMotorCommand(ControllerNumber)

            End If

        Next

    End Sub

End Structure

Public Class Form1

    Private Controllers As XboxControllers

    'Game State Data *************************************
    Private Enum GameStateEnum
        StartScreen
        Instructions
        Serve
        Playing
        EndScreen
        Pause
    End Enum

    Private Enum ServeStateEnum
        LeftPaddle
        RightPaddle
    End Enum

    Private Enum WinStateEnum
        LeftPaddle
        RightPaddle
    End Enum

    'State Data *******************************************
    Private GameState As GameStateEnum = GameStateEnum.StartScreen
    Private Serving As ServeStateEnum = ServeStateEnum.LeftPaddle
    Private ServSpeed As Single = 500
    Private Winner As WinStateEnum = WinStateEnum.LeftPaddle
    Private NumberOfPlayers As Integer = 1
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

    Private Const TitleText As String = "P🏓NG"
    Private TitleLocation As New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 125)
    Private ReadOnly TitleFont As New Font("Segoe UI Emoji", 100)

    Private InstructStartLocation As Point
    Private ReadOnly InstructStartText As String = Environment.NewLine &
        "One player  A   Two players  B"

    'One Player Instructions Data *************************
    Private InstructOneLocation As Point
    Private Shared ReadOnly InstructOneText As String = Environment.NewLine &
        "Play  A" & Environment.NewLine & Environment.NewLine &
        "You are the 🏓 left paddle" & Environment.NewLine &
        "Beat the computer to 10 points to win 🏆" & Environment.NewLine & Environment.NewLine &
        "Pause / Resume  Start "
    '******************************************************

    'Two Player Instructions Data *************************
    Private InstructTwoLocation As Point
    Private Shared ReadOnly InstructTwoText As String = Environment.NewLine &
        "Play  A  " & Environment.NewLine & Environment.NewLine &
        "🏓 Left Paddle vs Right Paddle 🏓" & Environment.NewLine &
        "First player to 10 points wins 🏆" & Environment.NewLine & Environment.NewLine &
        "Pause / Resume  Start  "
    '******************************************************
    Private ReadOnly InstructionsFont As New Font("Segoe UI Emoji", 20)

    Private IsBackButtonDown(0 To 3) As Boolean

    Private IsStartButtonDown(0 To 3) As Boolean

    Private IsAButtonDown(0 To 3) As Boolean

    Private IsAKeyDown As Boolean = False

    Private IsXButtonDown(0 To 3) As Boolean

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

    Private RPadTrophyLocation As Point
    Private LPadTrophyLocation As Point

    Private ReadOnly ScoreFont As New Font(FontFamily.GenericSansSerif, 75)

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

    Private BackspaceKeyDown As Boolean = False
    Private IsBackspaceKeyDown As Boolean = False

    Private AKeyDown As Boolean = False
    Private BKeyDown As Boolean = False
    Private XKeyDown As Boolean = False
    Private IsXKeyDown As Boolean = False

    Private EscKeyDown As Boolean = False
    Private IsEscKeyDown As Boolean = False

    Private PauseKeyDown As Boolean = False
    Private IsPauseKeyDown As Boolean = False

    Private LastKeyDown As Date = Now


    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
    Private Shared Function mciSendStringW(<MarshalAs(UnmanagedType.LPTStr)> ByVal lpszCommand As String,
                                           <MarshalAs(UnmanagedType.LPWStr)> ByVal lpszReturnString As StringBuilder,
                                           ByVal cchReturn As UInteger, ByVal hwndCallback As IntPtr) As Integer
    End Function

    Private Sounds() As String

    Private Context As New BufferedGraphicsContext

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

    Private DrawFlashingText As Boolean = True

    Private gameTimer As Timer

    Private RightPaddleGoalIndicatorTimer As Integer = 0
    Private LeftPaddleGoalIndicatorTimer As Integer = 0

    Private RightPaddleGoalIndicatorBrush As SolidBrush = Brushes.Transparent
    Private LeftPaddleGoalIndicatorBrush As SolidBrush = Brushes.Transparent

    Private RightPaddleGoalIndicatorFade As Integer = 0
    Private LeftPaddleGoalIndicatorFade As Integer = 0


    Private RightPaddleGoalIndicatorRect As Rectangle
    Private LeftPaddleGoalIndicatorRect As Rectangle

    Private RightPaddleGoalIndicatorExpand As Integer = 0
    Private LeftPaddleGoalIndicatorExpand As Integer = 0



    Public Sub New()
        InitializeComponent()

        InitializeApp()

    End Sub

    Private Sub OnGameTick(sender As Object, e As EventArgs)

        UpdateGame()

        Invalidate() 'Calls OnPaint Sub

    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

        MyBase.OnPaint(e)

        DrawGame(e.Graphics)

        UpdateFrameCounter()

    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        Dim ElapsedTime As TimeSpan = Now - LastKeyDown

        ' Every 50 milliseconds check for keydown.
        If ElapsedTime.TotalMilliseconds >= 50 Then

            DoKeyDown(e)

            LastKeyDown = DateTime.Now

        End If

        e.Handled = True

    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp

        DoKeyUp(e)

        e.Handled = True

    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing

        CloseSounds()

    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal e As PaintEventArgs)

        'Intentionally left blank. Do not remove.

    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        ' Pause the game if the window is minimized.
        If WindowState = FormWindowState.Minimized Then

            If GameState = GameStateEnum.Playing Then

                GameState = GameStateEnum.Pause

            End If

        End If

        LayoutTitleAndInstructions()

        CenterCourtLine()

        LeftPaddle.Position.X = 20
        LeftPaddle.Rect.X = LeftPaddle.Position.X

        RightPaddle.Position.X = ClientSize.Width - RightPaddle.Rect.Width - 20 'Aline right 20 pix padding
        RightPaddle.Rect.X = RightPaddle.Position.X

        ' Place the FPS display at the bottom of the client area.
        FPS_Postion.Y = ClientRectangle.Bottom - 75

        LPadScoreLocation = New Point(ClientSize.Width \ 2 \ 2, 100)

        LPadTrophyLocation = New Point(ClientSize.Width \ 2 \ 2, ClientSize.Height \ 2 - 0)

        RPadScoreLocation = New Point(ClientSize.Width - (ClientSize.Width \ 4), 100)

        RPadTrophyLocation = New Point(ClientSize.Width - (ClientSize.Width \ 4), ClientSize.Height \ 2 - 0)

        ClientCenter = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2)

    End Sub

    Private Sub UpdateGame()

        Select Case GameState

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

            Case GameStateEnum.EndScreen

                UpdateEndScreen()

        End Select

    End Sub

    Private Sub DrawGame(g As Graphics)

        g.CompositingMode = Drawing2D.CompositingMode.SourceOver
        g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
        g.SmoothingMode = Drawing2D.SmoothingMode.None

        DrawBackground(g)

        Select Case GameState

            Case GameStateEnum.Playing

                DrawPlaying(g)

            Case GameStateEnum.StartScreen

                DrawStartScreen(g)

            Case GameStateEnum.Instructions

                DrawInstructions(g)

            Case GameStateEnum.Serve

                DrawServe(g)

            Case GameStateEnum.Pause

                DrawPauseScreen(g)

            Case GameStateEnum.EndScreen

                DrawEndScreen(g)

        End Select

    End Sub

    Private Sub UpdatePlaying()

        Controllers.Update()

        HandleControllerInput()

        UpdateLeftPaddleKeyboard()

        If PKeyDown Then

            If Not IsPKeyDown Then

                IsPKeyDown = True

                GameState = GameStateEnum.Pause

                MovePointerOffScreen()

                PlayPauseSound()

            End If

        Else

            IsPKeyDown = False

        End If

        If PauseKeyDown Then

            If Not IsPauseKeyDown Then

                IsPauseKeyDown = True

                GameState = GameStateEnum.Pause

                MovePointerOffScreen()

                PlayPauseSound()

            End If

        Else

            IsPauseKeyDown = False

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

        UpdateGoalIndicators()

    End Sub

    Private Sub UpdateGoalIndicators()

        UpdateRightPaddleGoalIndicator()

        UpdateLeftPaddleGoalIndicator()

    End Sub

    Private Sub UpdateLeftPaddleGoalIndicator()

        If LeftPaddleGoalIndicatorTimer > 0 Then

            If LeftPaddleGoalIndicatorFade > 0 Then
                LeftPaddleGoalIndicatorFade -= CInt(0.4 * DeltaTime.TotalMilliseconds)
            End If

            If LeftPaddleGoalIndicatorFade < 0 Then
                LeftPaddleGoalIndicatorFade = 0
            End If

            LeftPaddleGoalIndicatorBrush = New SolidBrush(Color.FromArgb(LeftPaddleGoalIndicatorFade, 0, 255, 0))

            LeftPaddleGoalIndicatorTimer -= DeltaTime.TotalMilliseconds

            LeftPaddleGoalIndicatorExpand += CInt(0.1 * DeltaTime.TotalMilliseconds)

            LeftPaddleGoalIndicatorRect = New Rectangle(ClientRectangle.Right - LeftPaddleGoalIndicatorExpand, ClientRectangle.Top, LeftPaddleGoalIndicatorExpand, ClientSize.Height)

        Else

            LeftPaddleGoalIndicatorBrush = Brushes.Transparent

            LeftPaddleGoalIndicatorTimer = 0

            LeftPaddleGoalIndicatorExpand = 32

            LeftPaddleGoalIndicatorRect = New Rectangle(ClientRectangle.Right - 32, ClientRectangle.Top, 32, ClientSize.Height)

        End If

    End Sub

    Private Sub UpdateRightPaddleGoalIndicator()
        If RightPaddleGoalIndicatorTimer > 0 Then

            If RightPaddleGoalIndicatorFade > 0 Then
                RightPaddleGoalIndicatorFade -= CInt(0.4 * DeltaTime.TotalMilliseconds)
            End If

            If RightPaddleGoalIndicatorFade < 0 Then
                RightPaddleGoalIndicatorFade = 0
            End If

            RightPaddleGoalIndicatorBrush = New SolidBrush(Color.FromArgb(RightPaddleGoalIndicatorFade, 0, 255, 0))

            RightPaddleGoalIndicatorTimer -= DeltaTime.TotalMilliseconds

            RightPaddleGoalIndicatorExpand += CInt(0.1 * DeltaTime.TotalMilliseconds)

            RightPaddleGoalIndicatorRect = New Rectangle(ClientRectangle.Left, ClientRectangle.Top, RightPaddleGoalIndicatorExpand, ClientSize.Height)

        Else

            RightPaddleGoalIndicatorBrush = Brushes.Transparent

            RightPaddleGoalIndicatorTimer = 0

            RightPaddleGoalIndicatorExpand = 32

            RightPaddleGoalIndicatorRect = New Rectangle(ClientRectangle.Left, ClientRectangle.Top, 32, ClientSize.Height)

        End If


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

        If EndScreenCounter >= 500 Then

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
        ' This algorithm controls the rate of flash for text.

        ' Advance the frame counter.
        FlashCount += 1

        ' Draw text for 60 frames.
        If FlashCount <= 60 Then

            DrawFlashingText = True

        Else

            DrawFlashingText = False

        End If

        ' Dont draw text for the next 60 frames.
        If FlashCount >= 120 Then

            ' Repete
            FlashCount = 0

        End If

    End Sub

    Private Sub PlayWinningSound()

        PlaySound("winning")

    End Sub

    Private Sub PlayPauseSound()

        LoopSound("pause")

    End Sub

    Private Sub MovePointerOffScreen()
        ' Move mouse pointer off screen.

        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right,
                                    Screen.PrimaryScreen.WorkingArea.Height \ 2)

    End Sub

    Private Sub MovePointerCenterScreen()
        ' Move mouse pointer center screen.

        Cursor.Position = New Point(Screen.PrimaryScreen.WorkingArea.Right \ 2,
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

                ' Send ball up and to the right.
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = -ServSpeed

            Case 2

                ' Send ball to the right.
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = 0

            Case 3

                ' Send ball down and to the right.
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = ServSpeed

        End Select

    End Sub

    Private Sub ServeRightPaddle()

        Select Case RandomNumber()

            Case 1

                ' Send ball up and to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = -ServSpeed

            Case 2

                ' Send ball to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = 0

            Case 3

                ' Send ball down and to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = ServSpeed

        End Select

    End Sub
    Private Shared Function RandomNumber() As Integer

        ' Initialize random-number generator.
        Randomize()

        ' Generate random number between 1 and 3.
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

        ' Did ball enter left goal zone?
        If Ball.Rect.X < 0 Then
            ' Yes, ball entered left goal zone.

            PlayPointSound()
            'TODO
            RightPaddleGoalIndicatorTimer = 1000
            RightPaddleGoalIndicatorFade = 255

            ' Award point to right paddle.
            RightPaddleScore += 1

            ' Change possession of ball to right paddle.
            Serving = ServeStateEnum.RightPaddle

            ' Change game state to serve.
            GameState = GameStateEnum.Serve

        End If

        ' Did ball enter right goal zone?
        If Ball.Rect.X + Ball.Rect.Width > ClientSize.Width Then
            ' Yes, ball entered goal zone.

            PlayPointSound()
            ' TODO

            LeftPaddleGoalIndicatorTimer = 1000
            LeftPaddleGoalIndicatorFade = 255


            ' Award a point to left paddle.
            LeftPaddleScore += 1

            ' Change possession of ball to left paddle.
            Serving = ServeStateEnum.LeftPaddle

            ' Change game state to serve.
            GameState = GameStateEnum.Serve

        End If

    End Sub

    Private Sub CheckForPaddleHits()

        CheckForLeftPaddleHits()

        CheckForRightPaddleHits()

    End Sub

    Private Sub CheckForRightPaddleHits()

        ' Did the ball hit the right paddle?
        If Ball.Rect.IntersectsWith(RightPaddle.Rect) Then
            ' Yes, the ball did hit the right paddle.

            PlaySound("hit")

            ' Stop the ball's movement.
            Ball.Velocity.X = 0
            Ball.Velocity.Y = 0

            ' Moves the ball to the left of the right paddle, ensuring there’s a 5-pixel gap.
            Ball.Rect.X = RightPaddle.Rect.X - (Ball.Rect.Width + 5)

            ' Update the ball's position to match the new rectangle position.
            Ball.Position.X = Ball.Rect.X

            ' Is the number of players two?
            If NumberOfPlayers = 2 Then
                ' Yes, the number of players is two.

                ' Set a flag to indicate that the ball should have some spin (English) applied.
                ApplyRightPaddleEnglish = True

                ' Trigger a vibration effect on the left side of the right paddle controller "1".
                Controllers.VibrateLeft(1, 42000)

            Else
                ' No, the number of players is not two.

                DoComputerPlayerEnglish()

            End If

        End If

    End Sub

    Private Sub DoComputerPlayerEnglish()
        ' For the computer player we use random english.
        ' This makes the game more interesting.

        Select Case RandomNumber()

            Case 1

                ' Send ball up and to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = -ServSpeed

            Case 2

                ' Send ball to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = 0

            Case 3

                ' Send ball down and to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = ServSpeed

        End Select

    End Sub

    Private Sub CheckForLeftPaddleHits()

        ' Did the ball hit the left paddle?
        If Ball.Rect.IntersectsWith(LeftPaddle.Rect) Then
            ' Yes, the ball did hit the left paddle.

            PlaySound("hit")

            ' Stop the ball's movement.
            Ball.Velocity.X = 0
            Ball.Velocity.Y = 0

            ' Set the ball's position to the right edge of the left paddle, plus an extra 5 pixels.
            Ball.Rect.X = LeftPaddle.Rect.X + LeftPaddle.Rect.Width + 5

            ' Update the ball's position to match the new rectangle position.
            Ball.Position.X = Ball.Rect.X

            ' Set a flag to indicate that the ball should have some spin (English) applied.
            ApplyLeftPaddleEnglish = True

            ' Trigger a vibration effect on the left side of the left paddle controller "0".
            Controllers.VibrateLeft(0, 42000)

        End If

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

        If Controllers.DPadUp(1) Then

            MoveRightPaddleUp()

        ElseIf Controllers.DPadDown(1) Then

            MoveRightPaddleDown()

        Else

            If Controllers.RightThumbstickYaxisNeutral(1) AndAlso Controllers.LeftThumbstickYaxisNeutral(1) AndAlso Not UpArrowKeyDown AndAlso Not DownArrowKeyDown Then

                DecelerateRightPaddle()

            End If

            If ApplyRightPaddleEnglish AndAlso Controllers.RightThumbstickYaxisNeutral(1) AndAlso Controllers.LeftThumbstickYaxisNeutral(1) AndAlso Not UpArrowKeyDown AndAlso Not DownArrowKeyDown Then

                ApplyRightPaddleEnglish = False

                'Send ball to the left.
                Ball.Velocity.X = -ServSpeed
                Ball.Velocity.Y = 0

            End If

        End If

    End Sub

    Private Sub DoDPadLogicControllerZero()

        If Controllers.DPadUp(0) Then

            MoveLeftPaddleUp()

        ElseIf Controllers.DPadDown(0) Then

            MoveLeftPaddleDown()

        Else
            ' The direction pad is in the neutral position.

            If Controllers.RightThumbstickYaxisNeutral(0) AndAlso Controllers.LeftThumbstickYaxisNeutral(0) AndAlso Not WKeyDown AndAlso Not SKeyDown Then

                DecelerateLeftPaddle()

            End If

            If ApplyLeftPaddleEnglish AndAlso Controllers.RightThumbstickYaxisNeutral(0) AndAlso Controllers.LeftThumbstickYaxisNeutral(0) AndAlso Not WKeyDown AndAlso Not SKeyDown Then

                ApplyLeftPaddleEnglish = False

                'Send ball to the right.
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = 0

                Debug.Print("Left Paddle Send Ball Right")

            End If

        End If

    End Sub

    Private Sub UpdateRightThumbstickPosition(ControllerNumber As Integer)

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

        If Controllers.RightThumbstickDown(1) Then
            MoveRightPaddleDown()
        ElseIf Controllers.RightThumbstickUp(1) Then
            MoveRightPaddleUp()
        End If

    End Sub

    Private Sub UpdateRightThumbstickPositionControllerZero()

        If Controllers.RightThumbstickDown(0) Then
            MoveLeftPaddleDown()
        ElseIf Controllers.RightThumbstickUp(0) Then
            MoveLeftPaddleUp()
        End If

    End Sub

    Private Sub UpdateLeftThumbstickPosition(ControllerNumber As Integer)

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

        If Controllers.LeftThumbstickDown(1) Then
            MoveRightPaddleDown()
        ElseIf Controllers.LeftThumbstickUp(1) Then
            MoveRightPaddleUp()
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

        If EscKeyDown Then

            If Not IsEscKeyDown Then

                IsEscKeyDown = True

                MovePointerCenterScreen()

                Application.Exit()

            End If

        Else

            IsEscKeyDown = False

        End If

        If XKeyDown Then

            If Not IsXKeyDown Then

                IsXKeyDown = True

                MovePointerCenterScreen()

                Application.Exit()

            End If

        Else

            IsXKeyDown = False

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

        If EscKeyDown Then

            If Not IsEscKeyDown Then

                IsEscKeyDown = True

                GameState = GameStateEnum.StartScreen

                MovePointerOffScreen()

            End If

        Else

            IsEscKeyDown = False

        End If

        If XKeyDown Then

            If Not IsXKeyDown Then

                IsXKeyDown = True

                GameState = GameStateEnum.StartScreen

                MovePointerOffScreen()

            End If

        Else

            IsXKeyDown = False

        End If

    End Sub

    Private Sub UpdateLeftPaddleKeyboard()

        If WKeyDown Then

            MoveLeftPaddleUp()


        ElseIf SKeyDown Then

            MoveLeftPaddleDown()

        Else

            If Not Controllers.Connected(0) Then

                DecelerateLeftPaddle()

                If ApplyLeftPaddleEnglish Then

                    ApplyLeftPaddleEnglish = False

                    'Send ball to the right.
                    Ball.Velocity.X = ServSpeed
                    Ball.Velocity.Y = 0

                    Debug.Print("Left Paddle Send Ball Right")

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

            If Not Controllers.Connected(1) Then

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

        If Controllers.LeftThumbstickDown(0) Then
            MoveLeftPaddleDown()
        ElseIf Controllers.LeftThumbstickUp(0) Then
            MoveLeftPaddleUp()
        End If

    End Sub

    Private Sub MoveLeftPaddleUp()

        ' Is the paddle moving down?
        If LeftPaddle.Velocity.Y > 0 Then
            ' Yes, the paddle is moving down.

            ' Stop move before changing direction.
            LeftPaddle.Velocity.Y = 0 ' Zero speed.

            Debug.Print($"Left Paddle Stop Velocity {LeftPaddle.Velocity.Y}")

        End If

        MoveLeftPaddleUpCheckTopBoundaryMaxVelocity()

        If ApplyLeftPaddleEnglish Then

            ApplyLeftPaddleEnglish = False

            ' Send ball up and to the right.
            Ball.Velocity.X = ServSpeed
            Ball.Velocity.Y = -ServSpeed

            Debug.Print("Left Paddle Send Ball Up Right")

        End If

    End Sub

    Private Sub MoveLeftPaddleUpCheckTopBoundaryMaxVelocity()

        ' Has the paddle reached or exceeded the top of the client area?
        If LeftPaddle.Rect.Top <= ClientRectangle.Top Then
            ' Yes, the paddle has reached the top of the client area.

            ' Is the paddle moving up?
            If LeftPaddle.Velocity.Y < 0 Then
                'Yes, the paddle is moving up.

                ' Stop the paddle.
                LeftPaddle.Velocity.Y = 0

                Debug.Print($"Left Paddle Stop Velocity {LeftPaddle.Velocity.Y}")

            End If

            ' Is the paddle passed the top of the client area.
            If LeftPaddle.Rect.Top < ClientRectangle.Top Then
                ' Yes, the paddle is passed the top of the client area.

                ' Aline paddle to the top of the client area.
                LeftPaddle.Rect.Y = ClientRectangle.Top

                ' Snyc paddle position.
                LeftPaddle.Position.Y = LeftPaddle.Rect.Y

                Debug.Print("Left Paddle Aline Top")

            End If

            ' Has the paddle reached or exceeded max velocity?
        ElseIf LeftPaddle.Velocity.Y > -LeftPaddle.MaxVelocity.Y Then
            ' No, the paddle has not reached or exceeded max velocity.
            ' No, the paddle has not reached or exceeded the top of the client area.

            ' Calculate potential new velocity
            Dim newVelocityY As Double = LeftPaddle.Velocity.Y - (LeftPaddle.Acceleration.Y * DeltaTime.TotalSeconds)

            ' Does the potential new velocity exceed the max velocity?
            If newVelocityY < -LeftPaddle.MaxVelocity.Y Then
                ' Yes, the potential new velocity does exceed the max velocity.

                ' Limit paddle velocity to the max.
                LeftPaddle.Velocity.Y = -LeftPaddle.MaxVelocity.Y

                Debug.Print($"Left Paddle Up-- Velocity {LeftPaddle.Velocity.Y} -Max-")

            Else
                ' No, the potential new velocity does not exceed the max velocity.

                ' Send paddle up.
                LeftPaddle.Velocity.Y = newVelocityY

                Debug.Print($"Left Paddle Up-- Velocity {LeftPaddle.Velocity.Y}")

            End If

        End If

    End Sub

    Private Sub MoveLeftPaddleDown()

        ' Is the paddle moving up?
        If LeftPaddle.Velocity.Y < 0 Then
            ' Yes, the paddle is moving up.

            ' Stop move before changing direction.
            LeftPaddle.Velocity.Y = 0 ' Zero speed.

            Debug.Print($"Left Paddle Stop Velocity {LeftPaddle.Velocity.Y}")

        End If

        MoveLeftPaddleDownCheckBottomBoundary()

        If ApplyLeftPaddleEnglish Then

            ApplyLeftPaddleEnglish = False

            ' Send ball down and to the right.
            Ball.Velocity.X = ServSpeed
            Ball.Velocity.Y = ServSpeed

            Debug.Print("Left Paddle Send Ball Down Right")

        End If

    End Sub

    Private Sub MoveLeftPaddleDownCheckBottomBoundary()

        ' Has the paddle reached or exceeded the bottom of the client area?
        If LeftPaddle.Rect.Bottom >= ClientRectangle.Bottom Then
            ' Yes, the paddle has reached or exceeded the bottom of the client area.

            ' Is the paddle moving down?
            If LeftPaddle.Velocity.Y > 0 Then
                ' Yes, the paddle is moving down.

                ' Stop the paddle.
                LeftPaddle.Velocity.Y = 0

                Debug.Print($"Left Paddle Stop Velocity {LeftPaddle.Velocity.Y}")

            End If

            ' Is the paddle passed the bottom of the client area.
            If LeftPaddle.Rect.Bottom > ClientRectangle.Bottom Then
                ' Yes, the paddle is passed the bottom of the client area.

                ' Aline paddle to bottom of the client area.
                LeftPaddle.Rect.Y = ClientRectangle.Bottom - LeftPaddle.Rect.Height

                ' Sync paddle position.
                LeftPaddle.Position.Y = LeftPaddle.Rect.Y

                Debug.Print("Left Paddle Aline Bottom")

            End If

            ' Has the paddle reached or exceeded max velocity?
        ElseIf LeftPaddle.Velocity.Y < LeftPaddle.MaxVelocity.Y Then
            ' No, the paddle has not reached or exceeded max velocity.
            ' No, the paddle has not reached or exceeded the bottom of the client area.

            ' Calculate potential new velocity
            Dim newVelocityY As Double = LeftPaddle.Velocity.Y + (LeftPaddle.Acceleration.Y * DeltaTime.TotalSeconds)

            ' Does the potential new velocity exceed the max velocity?
            If newVelocityY > LeftPaddle.MaxVelocity.Y Then
                ' Yes, the potential new velocity does exceed the max velocity.

                ' Limit paddle velocity to the max.
                LeftPaddle.Velocity.Y = LeftPaddle.MaxVelocity.Y

                Debug.Print($"Left Paddle Down Velocity {LeftPaddle.Velocity.Y} -Max-")

            Else
                ' No, the potential new velocity does not exceed the max velocity.

                ' Send paddle down.
                LeftPaddle.Velocity.Y = newVelocityY

                Debug.Print($"Left Paddle Down Velocity {LeftPaddle.Velocity.Y}")

            End If

        End If

    End Sub

    Private Sub DrawComputerPlayerIdentifier(g As Graphics)

        g.DrawString("CPU", InstructionsFont, Brushes.White, ClientSize.Width - (ClientSize.Width \ 4), 20, AlineCenterMiddle)

    End Sub

    Private Sub DecelerateLeftPaddle()

        ' Is the paddle moving up?
        If LeftPaddle.Velocity.Y < 0 Then
            ' Yes, the paddle is moving up.

            ' Calculate potential new velocity
            Dim newVelocityY As Double = LeftPaddle.Velocity.Y + (LeftPaddle.Acceleration.Y * 2 * DeltaTime.TotalSeconds)

            ' Does the potential new velocity exceed zero speed?
            If newVelocityY > 0 Then
                ' Yes, the potential new velocity does exceed zero speed.

                ' Limit paddle decelerate to zero speed.
                ' This prevents the paddle from reversing direction.
                LeftPaddle.Velocity.Y = 0

                Debug.Print($"Left Paddle Stop Velocity {LeftPaddle.Velocity.Y}")

            Else
                ' No, the potential new velocity does not exceed zero speed.

                ' Decelerate paddle.
                LeftPaddle.Velocity.Y = newVelocityY

                Debug.Print($"Left Paddle DCel Velocity {LeftPaddle.Velocity.Y}")

            End If

        End If

        ' Is the paddle moving down?
        If LeftPaddle.Velocity.Y > 0 Then
            ' Yes, the paddle is moving down.

            ' Calculate potential new velocity
            Dim newVelocityY As Double = LeftPaddle.Velocity.Y + (-LeftPaddle.Acceleration.Y * 2 * DeltaTime.TotalSeconds)

            ' Does the potential new velocity exceed zero speed?
            If newVelocityY < 0 Then
                ' Yes, the potential new velocity does exceed zero speed.

                ' Limit paddle decelerate to zero speed.
                ' This prevents the paddle from reversing direction.
                LeftPaddle.Velocity.Y = 0

                Debug.Print($"Left Paddle Stop Velocity {LeftPaddle.Velocity.Y}")

            Else
                ' No, the potential new velocity does not exceed zero speed.

                ' Decelerate paddle.
                LeftPaddle.Velocity.Y = newVelocityY

                Debug.Print($"Left Paddle DCel Velocity {LeftPaddle.Velocity.Y}")

            End If

        End If

    End Sub

    Private Sub DecelerateRightPaddle()

        ' Is the paddle moving up?
        If RightPaddle.Velocity.Y < 0 Then
            ' Yes, the paddle is moving up.

            ' Decelerate paddle.
            RightPaddle.Velocity.Y += RightPaddle.Acceleration.Y * 2 * DeltaTime.TotalSeconds

            ' Limit decelerate to zero speed.
            If RightPaddle.Velocity.Y > 0 Then RightPaddle.Velocity.Y = 0 'Zero speed.

        End If

        ' Is the paddle moving down?
        If RightPaddle.Velocity.Y > 0 Then
            ' Yes, the paddle is moving down.

            ' Decelerate paddle.
            RightPaddle.Velocity.Y += -RightPaddle.Acceleration.Y * 2 * DeltaTime.TotalSeconds

            ' Limit decelerate to zero speed.
            If RightPaddle.Velocity.Y < 0 Then RightPaddle.Velocity.Y = 0 'Zero speed.

        End If

    End Sub

    Private Sub UpdatePause()

        Controllers.Update()

        HandleControllerInput()

        If PKeyDown Then

            If Not IsPKeyDown Then

                IsPKeyDown = True

                LastFrame = Now

                GameState = GameStateEnum.Playing

                MovePointerOffScreen()

                PauseSound("pause")

            End If

        Else

            IsPKeyDown = False

        End If

        If PauseKeyDown Then

            If Not IsPauseKeyDown Then

                IsPauseKeyDown = True

                LastFrame = Now

                GameState = GameStateEnum.Playing

                MovePointerOffScreen()

                PauseSound("pause")

            End If

        Else

            IsPauseKeyDown = False

        End If

        If BackspaceKeyDown Then

            If Not IsBackspaceKeyDown Then

                IsBackspaceKeyDown = True

                ResetGame()

                PauseSound("pause")

                MovePointerOffScreen()

            End If

        Else

            IsBackspaceKeyDown = False

        End If

        If EscKeyDown Then

            If Not IsEscKeyDown Then

                IsEscKeyDown = True

                ResetGame()

                PauseSound("pause")

                MovePointerOffScreen()

            End If

        Else

            IsEscKeyDown = False

        End If

        If XKeyDown Then

            If Not IsXKeyDown Then

                IsXKeyDown = True

                ResetGame()

                PauseSound("pause")

                MovePointerOffScreen()

            End If

        Else

            IsXKeyDown = False

        End If

    End Sub

    Private Sub UpdateInstructions()

        Controllers.Update()

        HandleControllerInput()

        UpdateInstructionsScreenKeyboard()

        UpdateDeltaTime()

        UpdateBallMovement()

        CheckForWallBounce()

        CheckForWallBounceXaxis()

    End Sub

    Private Sub DrawEndScreen(g As Graphics)

        DrawGoalIndicators(g)

        DrawCenterCourtLine(g)

        DrawLeftPaddle(g)

        DrawRightPaddle(g)

        DrawBall(g)

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier(g)

        End If

        DrawEndScores(g)

        UpdateGoalIndicators()

    End Sub

    Private Sub DrawGoalIndicators(g As Graphics)
        'Draw goal zone indicators.

        'g.FillRectangle(RightPaddleGoalIndicatorBrush, New RectangleF(ClientRectangle.Left, ClientRectangle.Top, 32, ClientSize.Height))

        'g.FillRectangle(LeftPaddleGoalIndicatorBrush, New RectangleF(ClientRectangle.Right - 32, ClientRectangle.Top, 32, ClientSize.Height))

        g.FillRectangle(RightPaddleGoalIndicatorBrush, RightPaddleGoalIndicatorRect)

        g.FillRectangle(LeftPaddleGoalIndicatorBrush, LeftPaddleGoalIndicatorRect)


    End Sub

    Private Sub DrawEndScores(g As Graphics)

        'Did the left paddle win?
        If Winner = WinStateEnum.LeftPaddle Then
            'Yes, the left paddle won.

            'Flash the winning score.
            If DrawFlashingText = True Then

                DrawLeftPaddleScore(g)

                DrawLeftPaddleTrophy(g)

            End If

        Else
            'No, the left paddle didn't win.

            DrawLeftPaddleScore(g)

        End If

        'Did the right paddle win?
        If Winner = WinStateEnum.RightPaddle Then
            'Yes, the right paddle won.

            'Flash the winning score.
            If DrawFlashingText = True Then

                DrawRightPaddleScore(g)

                DrawRightPaddleTrophy(g)

            End If

        Else
            'No, the right paddle didn't win.

            DrawRightPaddleScore(g)

        End If

    End Sub

    Private Sub DrawPauseScreen(g As Graphics)

        DrawCenterCourtLine(g)

        DrawLeftPaddle(g)

        DrawRightPaddle(g)

        DrawBall(g)

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier(g)

        End If

        DrawLeftPaddleScore(g)

        DrawRightPaddleScore(g)

        DrawPausedText(g)

    End Sub

    Private Sub DrawPausedText(g As Graphics)

        g.DrawString("Paused", TitleFont, Brushes.White, ClientCenter, AlineCenterMiddle)

    End Sub

    Private Sub DrawServe(g As Graphics)

        DrawCenterCourtLine(g)

        DrawLeftPaddle(g)

        DrawRightPaddle(g)

        DrawBall(g)

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier(g)

        End If

        DrawLeftPaddleScore(g)

        DrawRightPaddleScore(g)

    End Sub

    Private Sub DrawBackground(g As Graphics)

        g.Clear(Color.Black)

    End Sub

    Private Sub UpdateStartScreen()

        Controllers.Update()

        HandleControllerInput()

        UpdateStartScreenKeyboard()

        UpdateDeltaTime()

        UpdateBallMovement()

        CheckForWallBounce()

        CheckForWallBounceXaxis()

    End Sub

    Private Sub HandleControllerInput()
        ' Respond to input from each connected controller.

        For ControllerNumber As Integer = 0 To 3

            If Controllers.Connected(ControllerNumber) Then

                DoButtonLogic(ControllerNumber)
                UpdateLeftThumbstickPosition(ControllerNumber)
                UpdateRightThumbstickPosition(ControllerNumber)

            End If

        Next

    End Sub

    Private Sub DrawStartScreen(g As Graphics)

        Dim statusText As String = If(Controllers.Connected(0), "Controller 0 Connected", "Controller 0 Not Connected")
        Dim statusBrush As Brush = If(Controllers.Connected(0), Brushes.Lime, Brushes.Tomato)

        g.DrawString(statusText, New Font(FontFamily.GenericSansSerif, 13), statusBrush, New PointF(0, 0),
             New StringFormat() With {.Alignment = StringAlignment.Near})

        Dim statusText1 As String = If(Controllers.Connected(1), "Controller 1 Connected", "Controller 1 Not Connected")
        Dim statusBrush1 As Brush = If(Controllers.Connected(1), Brushes.Lime, Brushes.Tomato)

        g.DrawString(statusText1, New Font(FontFamily.GenericSansSerif, 13), statusBrush1, New PointF(0, 25),
             New StringFormat() With {.Alignment = StringAlignment.Near})

        DrawBall(g)

        DrawTitle(g)

        DrawStartScreenInstructions(g)

    End Sub

    Private Sub DrawTitle(g As Graphics)

        g.DrawString(TitleText, TitleFont, Brushes.White, TitleLocation, AlineCenter)

    End Sub

    Private Sub DrawStartScreenInstructions(g As Graphics)

        g.DrawString(InstructStartText, InstructionsFont, Brushes.White, InstructStartLocation, AlineCenter)

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

    Private Sub DrawInstructions(g As Graphics)

        DrawBall(g)

        DrawTitle(g)

        If NumberOfPlayers = 1 Then

            'Draw one player instructions.
            g.DrawString(InstructOneText,
            InstructionsFont, Brushes.White, InstructOneLocation, AlineCenter)

        Else

            'Draw two player instructions.
            g.DrawString(InstructTwoText,
            InstructionsFont, Brushes.White, InstructTwoLocation, AlineCenter)

        End If

    End Sub

    Private Sub DrawPlaying(g As Graphics)

        DrawGoalIndicators(g)

        DrawCenterCourtLine(g)

        DrawLeftPaddle(g)

        DrawRightPaddle(g)

        DrawBall(g)

        If NumberOfPlayers = 1 Then

            DrawComputerPlayerIdentifier(g)

        End If

        DrawLeftPaddleScore(g)

        DrawRightPaddleScore(g)

        DrawFPSDisplay(g)

    End Sub

    Private Sub DrawRightPaddleScore(g As Graphics)

        g.DrawString(RightPaddleScore, ScoreFont, Brushes.White, RPadScoreLocation, AlineCenterMiddle)

    End Sub

    Private Sub DrawRightPaddleTrophy(g As Graphics)

        g.DrawString("🏆", TitleFont, Brushes.White, RPadTrophyLocation, AlineCenterMiddle)

    End Sub

    Private Sub DrawLeftPaddleTrophy(g As Graphics)

        g.DrawString("🏆", TitleFont, Brushes.White, LPadTrophyLocation, AlineCenterMiddle)

    End Sub

    Private Sub DrawLeftPaddleScore(g As Graphics)

        g.DrawString(LeftPaddleScore, ScoreFont, Brushes.White, LPadScoreLocation, AlineCenterMiddle)

    End Sub

    Private Sub DrawRightPaddle(g As Graphics)

        g.FillRectangle(Brushes.White, RightPaddle.Rect)

    End Sub

    Private Sub DrawLeftPaddle(g As Graphics)

        g.FillRectangle(Brushes.White, LeftPaddle.Rect)

    End Sub

    Private Sub DrawFPSDisplay(g As Graphics)

        g.DrawString(FPS.ToString & " FPS", FPSFont, Brushes.MediumOrchid, FPS_Postion)

    End Sub

    Private Sub DrawCenterCourtLine(g As Graphics)

        g.DrawLine(CenterlinePen, CenterlineTop, CenterlineBottom)

    End Sub

    Private Sub CenterCourtLine()

        'Centers the court line in the client area of our form.
        CenterlineTop = New Point(ClientSize.Width \ 2, 0)

        CenterlineBottom = New Point(ClientSize.Width \ 2, ClientSize.Height)

    End Sub

    Private Sub DrawBall(g As Graphics)

        g.FillRectangle(Brushes.White, Ball.Rect)

    End Sub

    Private Sub DoButtonLogic(ControllerNumber As Integer)

        DoDPadLogic(ControllerNumber)

        DoLetterButtonLogic(ControllerNumber)

        DoStartBackLogic(ControllerNumber)

        'DoBumperLogic(ControllerNumber)

        'DoStickLogic(ControllerNumber)

    End Sub

    Private Sub DoLetterButtonLogic(ControllerNumber As Integer)

        Select Case GameState

            Case GameStateEnum.StartScreen

                If Controllers.A(ControllerNumber) Then

                    If Not IsAButtonDown(ControllerNumber) Then

                        IsAButtonDown(ControllerNumber) = True

                        NumberOfPlayers = 1

                        GameState = GameStateEnum.Instructions

                        MovePointerOffScreen()

                    End If

                Else

                    IsAButtonDown(ControllerNumber) = False

                End If

                If Controllers.B(ControllerNumber) Then

                    NumberOfPlayers = 2

                    GameState = GameStateEnum.Instructions

                    MovePointerOffScreen()

                End If

                If Controllers.X(ControllerNumber) Then

                    If Not IsXButtonDown(ControllerNumber) Then

                        IsXButtonDown(ControllerNumber) = True

                        MovePointerCenterScreen()

                        Application.Exit()

                    End If

                Else

                    IsXButtonDown(ControllerNumber) = False

                End If

            Case GameStateEnum.Instructions

                If Controllers.A(ControllerNumber) Then

                    If Not IsAButtonDown(ControllerNumber) Then

                        IsAButtonDown(ControllerNumber) = True

                        If IsPlaying("startscreenmusic") = True Then

                            PauseSound("startscreenmusic")

                        End If

                        GameState = GameStateEnum.Serve

                        MovePointerOffScreen()

                    End If

                Else

                    IsAButtonDown(ControllerNumber) = False

                End If

                If Controllers.X(ControllerNumber) Then

                    If Not IsXButtonDown(ControllerNumber) Then

                        IsXButtonDown(ControllerNumber) = True

                        GameState = GameStateEnum.StartScreen

                        MovePointerOffScreen()

                    End If

                Else

                    IsXButtonDown(ControllerNumber) = False

                End If

            Case GameStateEnum.Playing

            Case GameStateEnum.Serve

            Case GameStateEnum.Pause

                If Controllers.X(ControllerNumber) Then

                    If Not IsXButtonDown(ControllerNumber) Then

                        IsXButtonDown(ControllerNumber) = True

                        ResetGame()

                        PauseSound("pause")

                        MovePointerOffScreen()

                    End If

                Else

                    IsXButtonDown(ControllerNumber) = False

                End If


            Case GameStateEnum.EndScreen

        End Select

    End Sub

    Private Sub DoStartBackLogic(ControllerNumber As Integer)

        Select Case GameState

            Case GameStateEnum.StartScreen

                If Controllers.Start(ControllerNumber) Then

                    If Not IsStartButtonDown(ControllerNumber) Then

                        IsStartButtonDown(ControllerNumber) = True

                        GameState = GameStateEnum.Instructions

                        MovePointerOffScreen()

                    End If

                Else

                    IsStartButtonDown(ControllerNumber) = False

                End If

                If Controllers.Back(ControllerNumber) Then

                    If Not IsBackButtonDown(ControllerNumber) Then

                        IsBackButtonDown(ControllerNumber) = True

                        MovePointerCenterScreen()

                        Application.Exit()

                    End If

                Else

                    IsBackButtonDown(ControllerNumber) = False

                End If

            Case GameStateEnum.Instructions

                If Controllers.Back(ControllerNumber) Then

                    If Not IsBackButtonDown(ControllerNumber) Then

                        IsBackButtonDown(ControllerNumber) = True

                        GameState = GameStateEnum.StartScreen

                        MovePointerOffScreen()

                    End If

                Else

                    IsBackButtonDown(ControllerNumber) = False

                End If

                If Controllers.Start(ControllerNumber) Then

                    If Not IsStartButtonDown(ControllerNumber) Then

                        IsStartButtonDown(ControllerNumber) = True

                        If IsPlaying("startscreenmusic") = True Then

                            PauseSound("startscreenmusic")

                        End If

                        GameState = GameStateEnum.Serve

                        MovePointerOffScreen()

                    End If

                Else

                    IsStartButtonDown(ControllerNumber) = False

                End If

            Case GameStateEnum.Playing

                If Controllers.Start(ControllerNumber) Then

                    If Not IsStartButtonDown(ControllerNumber) Then

                        IsStartButtonDown(ControllerNumber) = True

                        GameState = GameStateEnum.Pause

                        MovePointerOffScreen()

                        PlayPauseSound()

                    End If

                Else

                    IsStartButtonDown(ControllerNumber) = False

                End If

            Case GameStateEnum.Serve

            Case GameStateEnum.Pause

                If Controllers.Start(ControllerNumber) Then

                    If Not IsStartButtonDown(ControllerNumber) Then

                        IsStartButtonDown(ControllerNumber) = True

                        LastFrame = Now

                        GameState = GameStateEnum.Playing

                        MovePointerOffScreen()

                        PauseSound("pause")

                    End If

                Else

                    IsStartButtonDown(ControllerNumber) = False

                End If

                If Controllers.Back(ControllerNumber) Then

                    If Not IsBackButtonDown(ControllerNumber) Then

                        IsBackButtonDown(ControllerNumber) = True

                        ResetGame()

                        PauseSound("pause")

                        MovePointerOffScreen()

                    End If

                Else

                    IsBackButtonDown(ControllerNumber) = False

                End If

            Case GameStateEnum.EndScreen

        End Select

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

        CreateSoundFileFromResource()

        AddSound("startscreenmusic", $"{Application.StartupPath}startscreenmusic.mp3")

        SetVolume("startscreenmusic", 300)

        LoopSound("startscreenmusic")

        AddSound("hit", $"{Application.StartupPath}hit.mp3")

        AddOverlapping("bounce", $"{Application.StartupPath}bounce.mp3")

        AddSound("point", $"{Application.StartupPath}point.mp3")

        SetVolume("point", 400)

        AddSound("winning", $"{Application.StartupPath}winning.mp3")

        AddSound("pause", $"{Application.StartupPath}pause.mp3")

        LayoutTitleAndInstructions()

        MovePointerOffScreen()

        Controllers.Initialize()

        SetupGameTimer()

        Debug.Print($"Initialization Complete")

    End Sub

    Private Sub SetupGameTimer()

        gameTimer = New Timer() With {.Interval = 15}

        AddHandler gameTimer.Tick, AddressOf OnGameTick

        gameTimer.Start()

    End Sub

    Private Sub PlayBounceSound()

        PlayOverlapping("bounce")

    End Sub

    Private Sub PlayPointSound()

        PlaySound("point")

    End Sub

    Private Sub InitializeForm()

        CenterToScreen()

        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)

        SetStyle(ControlStyles.UserPaint, True)

        Text = "P🏓NG - Code with Joe"

        WindowState = FormWindowState.Maximized

    End Sub

    Private Sub UpdateDeltaTime()
        ' Delta time (Δt) is the elapsed time since the last frame.

        CurrentFrame = Now

        DeltaTime = CurrentFrame - LastFrame ' Calculate delta time

        LastFrame = CurrentFrame ' Update last frame time

    End Sub

    Private Sub UpdateLeftPaddleMovement()

        LeftPaddle.Position.Y += LeftPaddle.Velocity.Y * DeltaTime.TotalSeconds ' Δs = V * Δt
        ' Displacement = Velocity x Delta Time

        LeftPaddle.Rect.Y = Math.Round(LeftPaddle.Position.Y)

    End Sub

    Private Sub UpdateRightPaddleMovement()

        RightPaddle.Position.Y += RightPaddle.Velocity.Y * DeltaTime.TotalSeconds ' Δs = V * Δt
        ' Displacement = Velocity x Delta Time

        RightPaddle.Rect.Y = Math.Round(RightPaddle.Position.Y)

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

    Private Sub LayoutTitleAndInstructions()

        TitleLocation = New Point(ClientSize.Width \ 2, ClientSize.Height \ 2 - 175)

        Ball.Position.Y = ClientSize.Height \ 2 + 40
        Ball.Rect.Y = Ball.Position.Y

        InstructStartLocation = New Point(ClientSize.Width \ 2, (ClientSize.Height \ 2) + 35)

        InstructOneLocation = New Point(ClientSize.Width \ 2, (ClientSize.Height \ 2) + 35)

        InstructTwoLocation = New Point(ClientSize.Width \ 2, (ClientSize.Height \ 2) + 35)

        EmojiLocation = New Point(ClientSize.Width \ 2 - 90, ClientSize.Height \ 2 - 190)

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

        FilePath = Path.Combine(Application.StartupPath, "pause.mp3")

        If Not IO.File.Exists(FilePath) Then

            IO.File.WriteAllBytes(FilePath, My.Resources.PauseMusic2)

        End If

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

            Case Keys.Back

                BackspaceKeyDown = True

            Case Keys.Escape

                EscKeyDown = True

            Case Keys.Pause

                PauseKeyDown = True

        End Select

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

            Case Keys.Back

                BackspaceKeyDown = False

            Case Keys.Escape

                EscKeyDown = False

            Case Keys.Pause

                PauseKeyDown = False

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

' Monica is our an AI assistant.
' https://monica.im/
' She is helping me write better code and learn new things.

' Copilot is another AI assistant.
' https://copilot.github.com/
' It is also helping me write better code and learn new things.