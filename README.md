#  P🏓NG

![011](https://github.com/user-attachments/assets/f129ad9d-270e-49a0-b833-56bad8938062)

P🏓NG is a  simulation of **Table Tennis**, a recreational activity and an Olympic sport since 1988, is also known by the term "ping-pong" or just "pong".

This repository is designed to help new game developers learn the fundamentals of game programming and design through a classic game.

## Features
- **Classic Gameplay**: Experience the timeless fun of Pong with modern enhancements.
- **Keyboard and Controller Support**: Play using your keyboard ⌨️ or Xbox controllers 🎮 , complete with vibration feedback.
- **Resizable and Pausable**: Enjoy a flexible gameplay experience that can be paused at any time.
- **Single and Multiplayer Modes**: Challenge yourself against a computer player or compete with friends.

## Learning Objectives
- Understand the basics of game mechanics and physics.
- Gain hands-on experience with VB.NET and game development concepts.
- Learn how to implement user input handling, game states, and sound effects.






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


# ⌨️ Keyboard Controls

To play using your keyboard, use the following controls:

- **Player 1 ( 🏓 Left Paddle)**
  - **W** : Move Up
  - **S** : Move Down

- **Player 2 (Right Paddle 🏓 )**
  - **Up Arrow** ⬆️ : Move Up
  - **Down Arrow** ⬇️ : Move Down

- **Pause/Resume Game**
  - **P** : Pause or Resume the game
 



# 🎮 Xbox Controllers

To play using your Xbox controllers, use the following controls:

  - **Thumbstick or D Pad Up** : Move Up
  - **Thumbstick or D Pad Down** : Move Down

- **Pause/Resume Game**
  - **Start** : Pause or Resume the game
 





# 📄 Code Walk Through




### 1. **Imports and Class Declaration**
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

### 2. **Enumerations**
```vb
Private Enum GameStateEnum
    StartScreen = 0
    Instructions = 1
    Serve = 2
    Playing = 3
    EndScreen = 4
    Pause = 5
End Enum
```
- `GameStateEnum` defines different states of the game (e.g., Start Screen, Playing, End Screen).

### 3. **Game Object Structure**
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

### 4. **Game State Variables**
```vb
Private GameState As GameStateEnum = GameStateEnum.StartScreen
Private Serving As ServeStateEnum = ServeStateEnum.LeftPaddle
Private ServSpeed As Single = 500
Private Winner As WinStateEnum = WinStateEnum.LeftPaddle
Private NumberOfPlayers As Integer = 2
```
- These variables track the current game state, which paddle is serving, the speed of the serve, the winner, and the number of players.

### 5. **Game Loop**
```vb
Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
    UpdateGame()
    Refresh()
End Sub
```
- This subroutine is called at regular intervals (set by a timer). It updates the game state and refreshes the display.

### 6. **Update Game Logic**
```vb
Private Sub UpdateGame()
    Select Case GameState
        Case GameStateEnum.Playing
            UpdateControllerData()
            UpdateLeftPaddleKeyboard()
            ...
        Case GameStateEnum.StartScreen
            UpdateStartScreenKeyboard()
            ...
        Case GameStateEnum.EndScreen
            UpdateEndScreen()
    End Select
End Sub
```
- The `UpdateGame` method uses a `Select Case` statement to determine what actions to take based on the current game state. It handles input, updates game objects, and checks for game conditions.

### 7. **Input Handling**
```vb
Private Sub UpdateLeftPaddleKeyboard()
    If WKeyDown = True Then MoveLeftPaddleUp()
    ElseIf SKeyDown = True Then MoveLeftPaddleDown()
    ...
End Sub
```
- This method checks if specific keys are pressed (W/S for the left paddle) and moves the paddle accordingly.

### 8. **Collision Detection**
```vb
Private Sub CheckForPaddleHits()
    If Ball.Rect.IntersectsWith(LeftPaddle.Rect) Then
        ...
    End If
    If Ball.Rect.IntersectsWith(RightPaddle.Rect) Then
        ...
    End If
End Sub
```
- This method checks if the ball intersects with either paddle, handling the logic for bouncing the ball back and updating its velocity.

### 9. **Game State Transitions**
```vb
Private Sub CheckforEndGame()
    If LeftPaddleScore >= 10 Then
        Winner = WinStateEnum.LeftPaddle
        GameState = GameStateEnum.EndScreen
    End If
    ...
End Sub
```
- This method checks if a player has reached the winning score and transitions the game state to the end screen if so.

### 10. **Rendering Graphics**
```vb
Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
    DrawGame()
    Buffer.Render(e.Graphics)
End Sub
```
- This method is responsible for rendering the game graphics. It calls the `DrawGame` method to draw all game elements and then renders the buffer to the screen.

### 11. **Sound Management**
```vb
Private Function AddSound(SoundName As String, FilePath As String) As Boolean
    ...
End Function
```
- This function manages sound effects, allowing sounds to be added and played during the game.

### Conclusion
The code is structured to handle game logic, user input, rendering, and sound management in a clear and organized manner. Each section focuses on a specific aspect of the game, making it easier to understand and modify. If you have specific sections you'd like to dive deeper into or have questions about, let me know!















