#  P🏓NG

![011](https://github.com/user-attachments/assets/f129ad9d-270e-49a0-b833-56bad8938062)

P🏓NG is a  simulation of **Table Tennis**, a recreational activity and an Olympic sport since 1988, is also known by the term "ping-pong" or just "pong".
This project is special to me as it was developed with my son, Joey, making it a true father-and-son collaboration.

This repository is designed to help new game developers learn the fundamentals of game programming and design through a classic game.

## Features
- **Classic Gameplay**: Experience the timeless fun of pong with modern enhancements.
- **Keyboard and Controller Support**: Play using your keyboard ⌨️ or Xbox controllers 🎮 , complete with vibration feedback.
- **Resizable and Pausable**: Enjoy a flexible gameplay experience that can be paused at any time.
- **Single and Multiplayer Modes**: Challenge yourself against a computer player or compete with friends.

## Learning Objectives
- Understand the basics of game mechanics and physics.
- Gain hands-on experience with VB.NET and game development concepts.
- Learn how to implement user input handling, game states, and sound effects.


---

[Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---


# Getting Started
To begin, you'll need to set up your development environment and clone the repository to get the game files. 

## 🛠️ **Set Up Your Environment**


![014](https://github.com/user-attachments/assets/918c5027-4d82-41aa-8f64-f4f87c2d235a)




Visual Studio is an Integrated Development Environment (IDE) created by Microsoft. 

It is a powerful software development tool that provides a comprehensive set of features and tools for building a wide range of applications.

This is the IDE I use to make P🏓NG and I recommend that you use.




![005](https://github.com/user-attachments/assets/2c8d863d-df92-4989-b5af-1f70e503d4f9)

Grab Visual Studio Community—it's free and perfect for indie devs.

Install Visual Studio from here:  https://visualstudio.microsoft.com/downloads/ and include the .NET Desktop Development workload.


## 🧬📦 **Clone the Repository** 

Click the "Code" button.

Copy the repository's URL.

![010](https://github.com/user-attachments/assets/562269e4-ec11-4702-addb-795769eeac3c)

Open Visual Studio.

Click "Clone a repository".



![006](https://github.com/user-attachments/assets/e7e542f5-a0f6-4258-bc58-491db0a0a78d)

Paste the repository URL into the location field.

Click the "Clone" button.

![008](https://github.com/user-attachments/assets/fd1627da-83ab-48ed-9c3b-ffc6049add27)

Once the cloning process is complete, you will have your own local copy of the game that you can run and modify on your computer.

Dive into the code, experiment, and enhance the game while learning valuable programming skills!

---

[Getting Started](#getting-started) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---


# ⌨️ Keyboard Controls

To play using your keyboard, use the following controls:

- **Player 1 ( 🏓 Left Paddle )**
  - **W** : Move Up
  - **S** : Move Down

- **Player 2 ( Right Paddle 🏓 )**
  - **Up Arrow** ⬆️ : Move Up
  - **Down Arrow** ⬇️ : Move Down

- **Pause/Resume Game**
  - **Pause** : **Pause** or **Resume** the game
  - **Escape** : From pause screen **Resets** the game
 

---

[Getting Started](#getting-started)  |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

# 🎮 Xbox Controllers

To play using your Xbox controllers, use the following controls:
- **🏓 Paddle Movement 🏓**
  - **Thumbstick Up** or **DPad Up** : Move Up
  - **Thumbstick Down** or **DPad Down** : Move Down

- **Pause/Resume Game**
  - **Start** : **Pause** or **Resume** the game
  - **Back** : From pause screen **Resets** the game
 

![016](https://github.com/user-attachments/assets/5eea8eb5-35f0-4e9c-a54c-539550b5c06d)

---

[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Top](#png)

---


# 📄 Code Walk Through


Here, we'll take a deep dive into the game's code and architecture, guiding you through the essential components that make up this classic table tennis simulation. Whether you're a beginner looking to understand the fundamentals of game programming or an experienced developer seeking to enhance your skills, this walkthrough will provide you with valuable insights.

We'll explore key areas such as game mechanics, user input handling, collision detection, and rendering graphics. Each section will break down the code into manageable parts, explaining the purpose and functionality behind them. By the end of this walkthrough, you'll have a solid understanding of how to create and modify a game like P🏓NG, empowering you to implement your own ideas and features. Let’s get started!

---

[Imports and Class Declaration](#imports-and-class-declaration)

[Enumerations](#enumerations)

[Game Object Structure](#game-object-structure) 

[Game State Variables](#game-state-variables)

[Game Loop](#game-loop)

[Update Game Logic](#update-game-logic) 

[Input Handling](#input-handling)

[Collision Detection](#collision-detection)

[Game State Transitions](#game-state-transitions) 

[Rendering Graphics](#rendering-graphics)

[Sound Management](#sound-management)

---

##  **Imports and Class Declaration**
```vb
Imports System.Threading
Imports System.Numerics
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO

Public Class Form1
```
- The code begins by importing necessary namespaces for threading, mathematical operations (like vectors), interop services for using unmanaged code, and file handling.
- The class `Form1` is declared, which will contain all the game logic.
  
---

[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

##  **Enumerations**
```vb
Private Enum GameStateEnum
    StartScreen
    Instructions
    Serve
    Playing
    EndScreen
    Pause
End Enum
```
- `GameStateEnum` defines different states of the game (e.g., Start Screen, Playing, End Screen).


---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---


##  **Game Object Structure**
```vb
Private Structure GameObject
    Public Position As Vector2
    Public Acceleration As Vector2
    Public Velocity As Vector2
    Public MaxVelocity As Vector2
    Public Rect As Rectangle
End Structure
```
- `GameObject` structure is defined to represent game entities like the ball and paddles. It includes properties for position, acceleration, velocity, maximum velocity, and the rectangle that represents its dimensions.


---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

##  **Game State Variables**
```vb
Private GameState As GameStateEnum = GameStateEnum.StartScreen
Private Serving As ServeStateEnum = ServeStateEnum.LeftPaddle
Private ServSpeed As Single = 500
Private Winner As WinStateEnum = WinStateEnum.LeftPaddle
Private NumberOfPlayers As Integer = 2
```
- These variables track the current game state, which paddle is serving, the speed of the serve, the winner, and the number of players.



---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

##  **Game Loop**
```vb
Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

    UpdateGame()

    Refresh()

End Sub
```
- This subroutine is called at regular intervals (set by a timer). It updates the game state and refreshes the display.

### Code Breakdown

`Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick`: This subroutine is executed at regular intervals, controlled by a timer named `Timer1`. The `Tick` event is triggered by the timer at specified intervals. `sender` is the source of the event, and `e` is the event data.

`UpdateGame()`: This line calls the `UpdateGame` method, which contains the logic for updating the game's state. This might include moving paddles, updating scores, checking for collisions, etc.

`Refresh()`: This line refreshes the display, causing the form to redraw its contents. It ensures that any changes made by `UpdateGame` are visually updated on the screen.


This subroutine keeps the game running smoothly by continuously updating the game state and refreshing the display at regular intervals. Essentially, it's the heartbeat of your game loop.

---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

##  **Update Game Logic**
```vb
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

```
- The `UpdateGame` method uses a `Select Case` statement to determine what actions to take based on the current game state. It handles input, updates game objects, and checks for game conditions.


---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

##  **Input Handling**
```vb
    Private Sub UpdateLeftPaddleKeyboard()

        If WKeyDown Then

            MoveLeftPaddleUp()

        ElseIf SKeyDown Then

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
```
- This method checks if specific keys are pressed (W/S for the left paddle) and moves the paddle accordingly.
  
### Code Breakdown


```Private Sub UpdateLeftPaddleKeyboard()```: This line defines a private subroutine named ```UpdateLeftPaddleKeyboard```. It's responsible for updating the movement of the left paddle based on keyboard input.

```If WKeyDown Then```: Checks if the ```W``` key is pressed. If true, it calls ```MoveLeftPaddleUp()``` to move the left paddle up.

```ElseIf SKeyDown Then```: If the ```W``` key is not pressed, it checks if the ```S``` key is pressed. If true, it calls ```MoveLeftPaddleDown()``` to move the left paddle down.

```Else```: If neither the ```W``` key nor the ```S``` key is pressed, it goes into this block.

```If Not Connected(0) Then```: Checks if the first controller is not connected. If true, it proceeds with the following steps.

```DecelerateLeftPaddle()```: Calls a function to slow down the left paddle.

```If ApplyLeftPaddleEnglish Then```: Checks if the flag to apply spin (English) to the ball is set. If true, it proceeds with the following steps.

```ApplyLeftPaddleEnglish = False```: Resets the flag for applying spin.

```Ball.Velocity.X = ServSpeed```: Sets the ball's horizontal velocity to a serving speed.

```Ball.Velocity.Y = 0```: Sets the ball's vertical velocity to 0, ensuring it moves straight.

This subroutine effectively manages the left paddle’s movement based on keyboard input, including handling cases where the controller isn't connected. 




---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

##  **Collision Detection**
```vb
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
         VibrateLeft(0, 42000)

     End If

 End Sub
```
- This method checks if the ball intersects with the left paddle, handling the logic for bouncing the ball back and updating its velocity.

### Code Breakdown

```Private Sub CheckForLeftPaddleHits()```: This line starts a subroutine called CheckForLeftPaddleHits. It's a private subroutine, so it's only accessible within the current module or class.

```If Ball.Rect.IntersectsWith(LeftPaddle.Rect) Then```: This line checks if the ball's rectangle intersects with the left paddle's rectangle. Essentially, it checks if the ball has hit the left paddle.

```PlaySound("hit")```: If the ball hits the left paddle, this line plays a sound named "hit" to provide audio feedback.

```Ball.Velocity.X = 0``` and ```Ball.Velocity.Y = 0```: These lines stop the ball's movement by setting its horizontal and vertical velocity to 0.

```Ball.Rect.X = LeftPaddle.Rect.X + LeftPaddle.Rect.Width + 5```: This line sets the ball's position to the right edge of the left paddle, plus an additional 5 pixels, to ensure the ball doesn't overlap the paddle.

```Ball.Position.X = Ball.Rect.X```: This line updates the ball's position to match the new rectangle position, keeping everything in sync.

```ApplyLeftPaddleEnglish = True```: This line sets a flag indicating that the ball should have some spin (English) applied, possibly affecting its movement after hitting the paddle.

```VibrateLeft(0, 42000)```: This line triggers a vibration effect on the left side of the left paddle controller, adding a tactile response to the hit.

All these steps together create a comprehensive check for collisions between the ball and the left paddle and apply appropriate responses like stopping the ball, playing sounds, and updating positions. 

 
---

```vb
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
                VibrateLeft(1, 42000)

            Else
                ' No, the number of players is not two.

                DoComputerPlayerEnglish()

            End If

        End If

    End Sub
```
- This method checks if the ball intersects with the right paddle, handling the logic for bouncing the ball back and updating its velocity.

### Code Breakdown

```Private Sub CheckForRightPaddleHits()```: Starts a subroutine to check if the ball hits the right paddle.

```If Ball.Rect.IntersectsWith(RightPaddle.Rect) Then```: Checks if the ball’s rectangle intersects with the right paddle’s rectangle.

```PlaySound("hit")```: Plays the sound named "hit" when the ball hits the paddle.

```Ball.Velocity.X = 0``` and ```Ball.Velocity.Y = 0```: Stops the ball's movement by setting its horizontal and vertical velocity to 0.

```Ball.Rect.X = RightPaddle.Rect.X - (Ball.Rect.Width + 5)```: Moves the ball to the left of the right paddle, leaving a 5-pixel gap.

```Ball.Position.X = Ball.Rect.X```: Updates the ball’s position to match the new rectangle position.

```If NumberOfPlayers = 2 Then```: Checks if there are two players.

```ApplyRightPaddleEnglish = True```: Sets a flag to apply spin (English) to the ball.

```VibrateLeft(1, 42000)```: Triggers a vibration effect on the right paddle’s controller.

```DoComputerPlayerEnglish()```: Calls a method for the computer player’s spin if there isn’t a second player.

---

[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)


---

##  **Game State Transitions**
```vb
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

```
- This method checks if a player has reached the winning score and transitions the game state to the end screen if so.

### Code Breakdown


`Private Sub CheckforEndGame()`: This defines a private subroutine named `CheckforEndGame`. It’s responsible for checking if any player has reached the winning score.

`If LeftPaddleScore >= 10 Then`: Checks if the left paddle’s score is 10 or more.

`Winner = WinStateEnum.LeftPaddle`: If the left paddle has reached the winning score, it sets the winner to the left paddle.

`FlashCount = 0`: Resets the frame counter, for the end game animation.

`GameState = GameStateEnum.EndScreen`: Changes the game state to the end screen, where the end game events are handled.

`PlayWinningSound()`: Plays a sound to celebrate the win.

`If RightPaddleScore >= 10 Then`: Similarly checks if the right paddle has reached the winning score.

`Winner = WinStateEnum.RightPaddle`: Sets the winner to the right paddle if it has reached the winning score.

`FlashCount = 0`: Resets the frame counter again.

`GameState = GameStateEnum.EndScreen`: Changes the game state to the end screen.

`PlayWinningSound()`: Plays the winning sound.

This method ensures the game transitions smoothly to the end screen when either player reaches the winning score. 


---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

##  **Rendering Graphics**
```vb
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

```
- This method is responsible for rendering the game graphics. It calls the `DrawGame` method to draw all game elements and then renders the buffer to the screen.

### Code Breakdown


`Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)`: This line starts the `OnPaint` method, which is responsible for painting the game's graphics. `Overrides` means it replaces the base class's `OnPaint` method. `e` is a PaintEventArgs object that gives you access to the graphics object.

`DrawGame()`: This line calls a method named `DrawGame`. This method contains the logic for drawing all the game elements like paddles, balls, and scores.

`Buffer.Render(e.Graphics)`: This line tells the game to display the image from a buffer (a temporary storage) onto the screen using `e.Graphics`.

`Buffer.Dispose()`: This line frees up memory used by the buffer. It's a cleanup step to ensure your program doesn’t use more memory than it needs.

`Buffer = Nothing`: This line sets the buffer to `Nothing` (null), ensuring it’s completely reset and no longer in use.

`Buffer = Context.Allocate(CreateGraphics(), ClientRectangle)`: This line creates a new buffer. `Context.Allocate` sets up the buffer to match the size of the game's window (`ClientRectangle`).

`UpdateFrameCounter()`: This line updates a counter that keeps track of how many frames have been displayed. It's useful for performance monitoring or displaying the frame rate.




---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---


##  **Sound Management**

```vb
Private Function AddSound(SoundName As String, FilePath As String) As Boolean

    If Not String.IsNullOrWhiteSpace(SoundName) AndAlso IO.File.Exists(FilePath) Then

        Dim CommandOpen As String = $"open ""{FilePath}"" alias {SoundName}"

        Dim ReturnString As New StringBuilder(128)

        If mciSendStringW(CommandOpen, ReturnString, 0, IntPtr.Zero) = 0 Then

            Array.Resize(Sounds, Sounds.Length + 1)

            Sounds(Sounds.Length - 1) = SoundName

            Return True

        End If

    End If

    Return False

End Function

```

- **Loading Sounds**: The game loads sound files from specified paths and associates them with names for easy reference.

### Code Breakdown


`Private Function AddSound(SoundName As String, FilePath As String) As Boolean`: This line defines a function named `AddSound`. It takes two parameters: `SoundName` and `FilePath`. The function returns a Boolean value (`True` or `False`).

`If Not String.IsNullOrWhiteSpace(SoundName) AndAlso IO.File.Exists(FilePath) Then`: This line checks if `SoundName` is not empty and if the file at `FilePath` exists.

`Dim CommandOpen As String = $"open ""{FilePath}"" alias {SoundName}"`: This line creates a command string to open the sound file with a specific alias name.

`Dim ReturnString As New StringBuilder(128)`: This line creates a new StringBuilder object with a capacity of 128 characters to store the response from the command.

`If mciSendStringW(CommandOpen, ReturnString, 0, IntPtr.Zero) = 0 Then`: This line sends the command to open the sound file. If the command is successful (returns 0), it proceeds to the next steps.

`Array.Resize(Sounds, Sounds.Length + 1)`: This line resizes the `Sounds` array to accommodate one more element.

`Sounds(Sounds.Length - 1) = SoundName`: This line adds the `SoundName` to the last position of the `Sounds` array.

`Return True`: This line returns `True`, indicating that the sound was successfully added.

`End If`: This line marks the end of the `If` block.

`Return False`: This line returns `False` if the sound could not be added.

`End Function`: This line marks the end of the function.

This function checks if the sound file exists, opens it, and adds it to an array of sounds for your game. If any step fails, it returns `False`. 


---




```vb
Private Function PlaySound(SoundName As String) As Boolean

    If Sounds IsNot Nothing AndAlso Sounds.Contains(SoundName) Then

        Dim CommandSeekToStart As String = $"seek {SoundName} to start"

        Dim ReturnString As New StringBuilder(128)

        mciSendStringW(CommandSeekToStart, ReturnString, 0, IntPtr.Zero)

        Dim CommandPlay As String = $"play {SoundName} notify"

        If mciSendStringW(CommandPlay, ReturnString, 0, Me.Handle) = 0 Then

            Return True

        End If

    End If

    Return False

End Function
```
- **Playing Sounds**: This function is provided to play specific sounds at appropriate game events (e.g., ball hits, scoring).


- **Volume Control**: The volume of sounds can be adjusted, allowing for a customizable audio experience.
- **Looping Sounds**: Background music can be looped to create an immersive atmosphere.






---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)


---

## A Special Thanks

A heartfelt thank you to my son, Joey, for his invaluable help in developing P🏓NG. This project is a testament to our teamwork and shared passion for coding!
![017](https://github.com/user-attachments/assets/60c0154a-93ad-43fa-a268-c1e0f0839411)

---

## Join the Conversation

I invite all players and developers to share their experiences, thoughts, and feedback on P🏓NG. Your insights are essential for making this project even better. Let’s collaborate and create something amazing together!

You can explore my work and projects on my GitHub: [github.com/JoeLumbley](https://github.com/JoeLumbley) and check out my tutorials on my YouTube channel: [youtube.com/@codewithjoe6074](https://youtube.com/@codewithjoe6074).

---

Happy coding!
