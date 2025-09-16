#  P🏓NG

**P🏓NG** is designed to help new game developers grasp fundamental programming concepts and design principles through an engaging and interactive experience. The game features classic gameplay mechanics, supports keyboard and Xbox controller inputs, and allows for both single-player and multiplayer modes. 

<img width="1920" height="1080" alt="020" src="https://github.com/user-attachments/assets/6d3a28f0-9260-4f62-9cd5-68a33bc49f23" />



### Key Features
- **Classic Gameplay**: Experience the timeless fun of ping-pong with modern enhancements, including smooth animations and responsive controls.
- **Keyboard and Controller Support**: Play using your keyboard or Xbox controllers, complete with vibration feedback for an immersive experience.
- **Resizable and Pausable**: Enjoy a flexible gameplay experience that can be paused at any time, allowing players to take breaks without losing progress.
- **Single and Multiplayer Modes**: Challenge yourself against a computer player or compete with friends, making the game versatile for different play styles.

### Learning Objectives
- Understand the basics of game mechanics and physics, including how to simulate movement and collisions.
- Gain hands-on experience with VB.NET and game development concepts, such as state management and event handling.
- Learn how to implement user input handling, game states, sound effects, and graphical rendering.

---



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

In this comprehensive walkthrough, we will explore the code structure and functionality of the **P🏓NG** game, a classic table tennis simulation. This guide aims to provide a detailed understanding of game programming concepts, from architecture to implementation. Whether you're a novice or an experienced developer, this walkthrough will deepen your knowledge and inspire you to create your own games.

---

## Imports and Class Declaration

The game begins by importing necessary namespaces and declaring the main class.

```vb
Imports System.Numerics
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO

Public Class Form1
```

### Explanation of Imports
- **System.Numerics**: This namespace provides mathematical types such as `Vector2`, which are essential for representing 2D vectors used in game physics and positioning.
- **System.ComponentModel**: Contains classes that are used to implement the run-time and design-time behavior of components and controls, helping manage the game’s UI.
- **System.Runtime.InteropServices**: Allows interaction with unmanaged code, crucial for handling Xbox controller inputs through XInput.
- **System.Text**: Provides classes for manipulating strings, including `StringBuilder`, which is used for constructing commands sent to the multimedia API.
- **System.IO**: Facilitates file handling operations, allowing the game to read and write sound files and other resources.

---

## Enumerations

### Game State Enumeration

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

### Purpose of GameStateEnum
This enumeration defines the various states of the game, allowing the program to manage transitions and logic based on the current state:
- **StartScreen**: The initial screen where players can choose to start the game.
- **Instructions**: Displays the rules and controls of the game.
- **Serve**: The state when players are preparing to serve the ball.
- **Playing**: The main gameplay state where the action occurs.
- **EndScreen**: Displays the result of the game and the winner.
- **Pause**: The game is temporarily halted.

---

## Game Object Structure

### GameObject Structure

```vb
Private Structure GameObject
    Public Position As Vector2
    Public Acceleration As Vector2
    Public Velocity As Vector2
    Public MaxVelocity As Vector2
    Public Rect As Rectangle
End Structure
```

### Explanation of GameObject
The `GameObject` structure represents entities in the game, such as the ball and paddles. Each object has the following properties:
- **Position**: A `Vector2` representing the current location of the object in 2D space.
- **Acceleration**: A `Vector2` representing the change in velocity over time, allowing for smooth movement.
- **Velocity**: A `Vector2` representing the current speed and direction of the object, essential for updating its position each frame.
- **MaxVelocity**: A `Vector2` representing the maximum speed the object can achieve, ensuring that objects do not move too fast.
- **Rect**: A `Rectangle` that defines the boundaries of the object for collision detection.

### Example of GameObject Usage
```vb
Private Ball As GameObject
Private LeftPaddle As GameObject
Private RightPaddle As GameObject
```
In the example above, instances of `GameObject` are created for the ball and both paddles, allowing the game to track their positions and movements.

---

## Game State Variables

### State Variables

```vb
Private GameState As GameStateEnum = GameStateEnum.StartScreen
Private Serving As ServeStateEnum = ServeStateEnum.LeftPaddle
Private ServSpeed As Single = 500
Private Winner As WinStateEnum = WinStateEnum.LeftPaddle
Private NumberOfPlayers As Integer = 1
```

### Purpose of Each Variable
- **GameState**: Tracks the current state of the game, determining which logic to execute.
- **Serving**: Indicates which paddle is currently serving (left or right), affecting ball movement.
- **ServSpeed**: Controls the speed at which the ball is served, influencing gameplay dynamics.
- **Winner**: Keeps track of the current winner of the game, used for displaying results.
- **NumberOfPlayers**: Indicates whether the game is single-player or multiplayer, affecting gameplay logic.

### Additional Variables for Game Management
```vb
Private FlashCount As Integer = 0
Private EndScreenCounter As Integer = 0
```
These additional variables are used to manage game transitions and visual effects during the end screen.

---

## Game Loop

The game loop is a critical component of the P🏓NG game, ensuring that the game runs smoothly and updates in real-time. It consists of two main subroutines: **UpdateGame** and **DrawGame**.

### UpdateGame

This subroutine is called at regular intervals to update the game's state. It processes input, updates game objects, checks for collisions, and transitions between game states.

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

### Explanation of UpdateGame
- **Select Case**: This structure evaluates the current game state and calls the corresponding update method.
- Each case corresponds to a different game state, allowing for specific logic to be executed based on the game's current condition.

### DrawGame

This subroutine handles rendering the game's graphics. It draws all the game elements, such as paddles, the ball, and scores, onto the screen.

```vb
Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    DrawGame(e.Graphics) ' Draw all game elements
    UpdateFrameCounter() ' Update frame counter
End Sub
```

### Explanation of DrawGame
- **DrawGame**: Responsible for rendering all visual elements of the game based on the current game state.
- **UpdateFrameCounter**: Keeps track of the frames per second (FPS) for performance monitoring and display.

---

## Update Game Logic

The `UpdateGame` method uses a `Select Case` statement to determine what actions to take based on the current game state. It handles input, updates game objects, and checks for game conditions.

### Example of State Handling

```vb
Private Sub UpdateGame()
    Select Case GameState
        Case GameStateEnum.Playing
            UpdatePlaying()
            ' Additional cases for other states...
    End Select
End Sub
```

### State-Specific Logic
Each state has a corresponding method that encapsulates its specific logic:
- **UpdatePlaying**: Handles game logic while playing, including movement, collision detection, and scoring.
- **UpdateStartScreen**: Manages the start screen logic, including user input for starting the game.
- **UpdateInstructions**: Displays and manages the instructions screen, guiding players on how to play.
- **UpdateServe**: Prepares the game for serving, positioning the ball and paddles appropriately.
- **UpdatePause**: Manages the pause state, allowing players to resume the game.
- **UpdateEndScreen**: Displays the end screen logic, showing the winner and allowing for a reset.

---

## Input Handling

### Keyboard Input for Left Paddle

The game allows players to control the paddles using keyboard input. Here's how the left paddle's movement is handled:

```vb
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
                Ball.Velocity.X = ServSpeed
                Ball.Velocity.Y = 0
            End If
        End If
    End If
End Sub
```

### Explanation of Keyboard Input
- **WKeyDown**: Checks if the W key is pressed to move the left paddle up.
- **SKeyDown**: Checks if the S key is pressed to move the left paddle down.
- **DecelerateLeftPaddle**: If neither key is pressed and the controller is not connected, the paddle decelerates to a stop.

### Controller Input Handling

The game also supports Xbox controllers. The input handling function uses the `XboxControllers` structure to manage the state of the controllers.

### Example of Controller Input Logic

```vb
Private Sub HandleControllerInput()
    For ControllerNumber As Integer = 0 To 3
        If Controllers.Connected(ControllerNumber) Then
            DoButtonLogic(ControllerNumber)
            UpdateLeftThumbstickPosition(ControllerNumber)
            UpdateRightThumbstickPosition(ControllerNumber)
        End If
    Next
End Sub
```

- **DoButtonLogic**: Processes button presses for the specified controller.
- **UpdateLeftThumbstickPosition**: Updates the position of the left thumbstick based on input.
- **UpdateRightThumbstickPosition**: Updates the position of the right thumbstick based on input.

---

## Collision Detection

### Check for Paddle Hits

Collision detection is essential for determining interactions between game objects. Here's how the game checks for collisions with the paddles:

```vb
Private Sub CheckForLeftPaddleHits()
    If Ball.Rect.IntersectsWith(LeftPaddle.Rect) Then
        PlaySound("hit")
        Ball.Velocity.X = 0
        Ball.Velocity.Y = 0
        Ball.Rect.X = LeftPaddle.Rect.X + LeftPaddle.Rect.Width + 5
        Ball.Position.X = Ball.Rect.X
        ApplyLeftPaddleEnglish = True
        Controllers.VibrateLeft(0, 42000)
    End If
End Sub
```

### Explanation of Collision Detection
- **IntersectsWith**: Checks if the ball's rectangle intersects with the left paddle's rectangle, indicating a collision.
- **PlaySound**: Plays a sound effect when the ball hits the paddle.
- **Velocity**: Stops the ball's movement and sets its position to avoid overlapping with the paddle.
- **ApplyLeftPaddleEnglish**: A flag indicating that spin should be applied to the ball after hitting the paddle.
- **Vibration Feedback**: Triggers a vibration effect on the controller for tactile feedback.

### Check for Right Paddle Hits

```vb
Private Sub CheckForRightPaddleHits()
    If Ball.Rect.IntersectsWith(RightPaddle.Rect) Then
        PlaySound("hit")
        Ball.Velocity.X = 0
        Ball.Velocity.Y = 0
        Ball.Rect.X = RightPaddle.Rect.X - (Ball.Rect.Width + 5)
        Ball.Position.X = Ball.Rect.X
        If NumberOfPlayers = 2 Then
            ApplyRightPaddleEnglish = True
            Controllers.VibrateLeft(1, 42000)
        Else
            DoComputerPlayerEnglish()
        End If
    End If
End Sub
```

This method checks if the ball intersects with the right paddle and handles the logic for bouncing the ball back and updating its velocity.

---

## Game State Transitions

### Check for End Game

```vb
Private Sub CheckforEndGame()
    If LeftPaddleScore >= 10 Then
        Winner = WinStateEnum.LeftPaddle
        GameState = GameStateEnum.EndScreen
        PlayWinningSound()
    End If

    If RightPaddleScore >= 10 Then
        Winner = WinStateEnum.RightPaddle
        GameState = GameStateEnum.EndScreen
        PlayWinningSound()
    End If
End Sub
```

This method checks if a player has reached the winning score and transitions the game state to the end screen if so:
- **Winner**: Sets the winner based on the score.
- **PlayWinningSound**: Plays a sound to indicate the end of the game.

---

## Rendering Graphics

### OnPaint Method

```vb
Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    MyBase.OnPaint(e)
    DrawGame(e.Graphics)
    UpdateFrameCounter()
End Sub
```

This method is responsible for rendering the game graphics. It calls the `DrawGame` method to draw all game elements and then updates the frame counter.

### DrawGame Method

```vb
Private Sub DrawGame(g As Graphics)
    g.Clear(Color.Black) ' Clear the background
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
```

- **g.Clear(Color.Black)**: Clears the previous frame by filling the background with black.
- **Select Case**: Determines which drawing method to call based on the current game state.

### Drawing Methods

Each state has its own drawing logic:
- **DrawPlaying**: Renders the game elements during active gameplay, including paddles, ball, and scores.
- **DrawStartScreen**: Displays the start screen with options for players to choose.
- **DrawInstructions**: Shows the instructions for playing the game, guiding players on controls and objectives.
- **DrawServe**: Prepares the visual elements for the serving state, indicating which player is serving.
- **DrawPauseScreen**: Displays the pause screen with relevant information and options to resume or exit.
- **DrawEndScreen**: Shows the end screen with the winner and final scores, along with options to restart the game.

### Example of DrawPlaying Method

```vb
Private Sub DrawPlaying(g As Graphics)
    g.Clear(Color.Black)
    DrawLeftPaddle(g)
    DrawRightPaddle(g)
    DrawBall(g)
    DrawScore(g)
    DrawFPSDisplay(g)
End Sub
```

In the `DrawPlaying` method, we clear the background, draw the paddles, ball, and scores, and display the current frames per second (FPS). This method is called repeatedly during the game loop to ensure a smooth visual experience.

---

## Sound Management

### Adding and Playing Sounds

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

### Explanation of Sound Management
This function loads sound files from specified paths and associates them with names for easy reference:
- **CommandOpen**: Constructs a command to open the sound file.
- **mciSendStringW**: Sends the command to the multimedia API to open the sound file.

### Playing Sounds

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

This function plays specific sounds at appropriate game events (e.g., ball hits, scoring). It ensures that audio feedback is provided to enhance the gaming experience.

### Sound Management Functions
- **AddSound**: Loads a sound file and associates it with a name.
- **PlaySound**: Plays a specific sound, seeking to the start of the sound file before playing.
- **PauseSound**: Pauses a currently playing sound.
- **LoopSound**: Plays a sound on repeat, useful for background music.

---

## Conclusion

This detailed walkthrough provides a comprehensive overview of the P🏓NG game code structure and functionality, enabling you to understand and modify the game effectively. Each section delves into the specific components that make the game work, from handling user input to managing game states and rendering graphics.

### Next Steps
- **Experiment with Modifications**: Try adjusting paddle speeds, changing ball physics, or implementing new gameplay mechanics.
- **Enhance Features**: Add power-ups, special abilities, or new game modes to increase replayability.
- **Explore Networking**: Implement online multiplayer functionality to allow players to compete over the internet.
- **Optimize Performance**: Analyze the game's performance and optimize rendering and input handling for smoother gameplay.

By understanding these components, you can enhance your coding skills and apply this knowledge to create your own games or modify existing ones. 

### Additional Resources
- **Game Development Tutorials**: Explore online tutorials and courses focused on game development using VB.NET and other languages.
- **Open Source Projects**: Study other open-source game projects to learn different approaches and techniques.
- **Game Design Principles**: Read about game design principles to improve your understanding of creating engaging and fun gameplay experiences.

Happy coding!
---


[Getting Started](#getting-started) | [Keyboard Controls](#%EF%B8%8F-keyboard-controls) | [XBox Controllers](#-xbox-controllers) |  [Code Walk Through](#-code-walk-through) |  [Top](#png)

---

<img width="1920" height="1080" alt="021" src="https://github.com/user-attachments/assets/c739c9b4-c91b-45f7-91b1-221e240e9227" />


https://youtu.be/ppUMmJLlsDg?si=HlilqN7ddzhGwYtN

---

![019](https://github.com/user-attachments/assets/4ec46822-d239-4e7b-91be-209baa982017)

https://youtube.com/shorts/_W-7EHnru2w?si=H6w0GIQnZGHYtB_t

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
