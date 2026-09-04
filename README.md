
# PONG - Code with Joe
A modern, full‑screen remake of the classic Pong arcade game. Rebuilt from scratch with smooth physics, glowing motion trails, dynamic paddle spin, an AI opponent, animated menus, and a fully‑featured audio system.




This project isn’t just Pong.
It’s a compact 2D game engine demonstrating real‑time rendering, delta‑time physics, state‑driven UI, and responsive design.


<img width="1920" height="1080" alt="034" src="https://github.com/user-attachments/assets/40523aea-56e8-495d-9957-e2d3b06f856a" />


### Key Features
- **Classic Gameplay**: Experience the timeless fun of ping-pong with modern enhancements, including smooth animations and responsive controls.
- **Keyboard Support**: Play using your keyboard.
- **Resizable and Pausable**: Enjoy a flexible gameplay experience that can be paused at any time, allowing players to take breaks without losing progress.
- **Single and Multiplayer Modes**: Challenge yourself against a computer player or compete with friends, making the game versatile for different play styles.

### Learning Objectives
- Understand the basics of game mechanics and physics, including how to simulate movement and collisions.
- Gain hands-on experience with game development concepts, such as state management and event handling.
- Learn how to implement user input handling, game states, sound effects, and graphical rendering.


[Keyboard Controls](#keyboard-ontrols) 

[Fixed: Audio Playback Issues](#audio-playback-issues)


<img width="1920" height="1080" alt="036" src="https://github.com/user-attachments/assets/4c1e1c0f-05d6-4cf5-9c77-640033da44d9" />


---
---
---









# Keyboard Controls

<img width="1920" height="1080" alt="035" src="https://github.com/user-attachments/assets/5059bff0-d55b-4009-9088-d71d735b5a8d" />

---


## Global Controls
| Key | Action |
|-----|--------|
| **F11** | Toggle fullscreen mode |
| **F** | Toggle fullscreen mode (secondary shortcut) |
| **Escape** | Context‑sensitive: fullscreen exit, pause, or return to start screen |

---

## Start Screen
| Key | Action |
|-----|--------|
| **Up / W** | Move selection up |
| **Down / S** | Move selection down |
| **1 / NumPad1** | Select “1‑Player” |
| **2 / NumPad2** | Select “2‑Player” |
| **Space / Enter / S** | Confirm selection and start match |
| **Escape** | Exit the game |

---

## Gameplay
| Key | Action |
|-----|--------|
| **W** | Move left paddle up |
| **S** | Move left paddle down |
| **Up Arrow** | Move right paddle up (2‑Player mode only) |
| **Down Arrow** | Move right paddle down (2‑Player mode only) |
| **P** | Pause the game |
| **Pause / Break** | Pause the game |
| **MediaPlayPause** | Pause the game |
| **Escape** | Pause the game (windowed mode) or exit fullscreen |

---

## Pause Menu
| Key | Action |
|-----|--------|
| **Up Arrow** | Move menu selection up |
| **Down Arrow** | Move menu selection down |
| **Enter / Space** | Activate selected menu option |
| **P** | Resume game |
| **Pause / Break** | Resume game |
| **MediaPlayPause** | Resume game |
| **R** | Resume game |
| **N** | Start a new match |
| **Q** | Quit to Start Screen |
| **Escape** | Quit to Start Screen |

---

## End Screen
| Key | Action |
|-----|--------|
| **Space / Enter** | Return to Start Screen |


[Top](#pong---code-with-joe) | [Fixed: Audio Playback Issues](#audio-playback-issues)

---
---
---















# Audio Playback Issues 
Why Our PONG Game’s Sound Became Garbled Over Time

During development, we discovered that our PONG game’s audio would slowly degrade during long play sessions. After 10–20 minutes, sound effects became distorted, crackly, or randomly cut off. This wasn’t a bug in our game logic. It was a limitation of the legacy Windows **MCI (winmm.dll)** audio subsystem.

Together, we investigated the root causes and implemented a full set of fixes that stabilized audio playback without replacing the entire audio engine.


---

## Why MCI Audio Breaks Down Over Time

MCI is a **1990s-era multimedia API**, and while it still works, it has several well‑documented failure modes. Our game happened to hit *all* of them.

### **1. Too Many Open Aliases**
Our overlapping playback system opened **24 copies** of each sound:

```vb
Private ReadOnly OverlapSuffixes As String() =
    Enumerable.Range(0, 24).Select(Function(i) Chr(Asc("A") + i).ToString()).ToArray()
```

Calling:

```vb
AddOverlapping("select", "select.wav")
```

created:

```
selectA
selectB
selectC
...
selectX
selectY
selectZ
```

With ~10 sound effects, this meant **240 active audio devices**.  
MCI cannot handle this load — buffers corrupt, handles leak, and playback becomes garbled.

---

### **2. MCI Doesn’t Actually Stop Playback Reliably**
We relied on:

```vb
status alias mode
```

But MCI often reports:

```
not ready
stopped
```

even when the device is still internally active.

This caused our overlapping logic:

```vb
If Not IsPlaying(aliasName) Then PlaySound(aliasName)
```

to reuse aliases that were still busy → corrupted audio buffers.

---

### **3. MCI Has No Real Mixing**
We simulated mixing by opening many copies of the same WAV.  
MCI was never designed for this, and rapid overlapping playback (menu navigation, paddle hits, ball bounces) overwhelmed the mixer.

---

### **4. MCI Leaks Handles**
Even after calling:

```vb
Send("close alias")
```

Windows often keeps the underlying device open until the process exits.  
Over time, this leads to distortion and playback failure.

---

## What in Our Code Triggered the Corruption

### **Overlapping Playback**
Our biggest culprit:

```vb
Public Sub PlayOverlapping(baseName As String)
    For Each suffix In OverlapSuffixes
        Dim aliasName = Normalize(baseName & suffix)
        If Not IsPlaying(aliasName) Then
            PlaySound(aliasName)
            Exit Sub
        End If
    Next
End Sub
```

Rapid sound effects burned through aliases faster than MCI could release them.

### **No Periodic Cleanup**
We only closed aliases when the game exited.  
After 10–20 minutes, MCI was already corrupted.

### **Use of `notify`**
Commands like:

```vb
play alias notify
pause alias notify
```

are known to cause buffer corruption unless you handle MCI notifications. We didn’t.

---

## How We Fixed It

### **Fix 1 Reduce Overlapping Channels**
We lowered the channel count from 24 → 8:

```vb
Private ReadOnly OverlapSuffixes As String() =
    {"A", "B", "C", "D", "E", "F", "G", "H"}
```

This alone dramatically reduced corruption.

---

### **Fix 2 Remove `notify`**
We replaced:

```vb
play soundName notify
```

with:

```vb
play soundName
```

This eliminated a major source of buffer corruption.

---

### **Fix 3 Safe MCI Usage**
We added:

- `stop` before `seek`  
- alias health checks  
- corruption recovery  
- stable overlapping playback logic  

This prevented buffer reuse issues.

---

### **Fix 4 Automatic Cleanup Every 5 Minutes**
We implemented a timer that:

1. Saves alias file paths and volume  
2. Stops all aliases  
3. Closes all aliases  
4. Reopens them fresh  
5. Restores volume  
6. Restarts looping sounds  

This prevents long‑term corruption and keeps MCI stable indefinitely.

---

## Result: Clean, Stable Audio Playback

After applying all fixes:

- No more garbled audio  
- No more crackling after long sessions  
- No more stuck aliases  
- No more corrupted overlapping playback  
- No need to replace the audio engine  

Our PONG game now runs for hours with **stable sound**.


[Top](#pong---code-with-joe)  | [Keyboard Controls](#keyboard-ontrols)


---
---
---











