
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


<img width="1920" height="1080" alt="036" src="https://github.com/user-attachments/assets/4c1e1c0f-05d6-4cf5-9c77-640033da44d9" />

### Learning Objectives
- Understand the basics of game mechanics and physics, including how to simulate movement and collisions.
- Gain hands-on experience with game development concepts, such as state management and event handling.
- Learn how to implement user input handling, game states, sound effects, and graphical rendering.








[Top](#pong---code-with-joe)  | [Keyboard Controls](#keyboard-controls) | [AudioPlayer Module](#audioplayer-module) | [Cleanuptick Walkthrough](#cleanuptick-walkthrough) | [Fixed Audio Playback Issues](#audio-playback-issues) 


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



[Top](#pong---code-with-joe)  | [Keyboard Controls](#keyboard-controls) | [AudioPlayer Module](#audioplayer-module) | [Cleanuptick Walkthrough](#cleanuptick-walkthrough) | [Fixed Audio Playback Issues](#audio-playback-issues) 


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

### **Too Many Open Aliases**
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
MCI cannot handle this load. Buffers corrupt, handles leak, and playback becomes garbled.

---

### **MCI Doesn’t Actually Stop Playback Reliably**
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

### **MCI Has No Real Mixing**
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

### **Reduce Overlapping Channels**
We lowered the channel count from 24 → 8:

```vb
Private ReadOnly OverlapSuffixes As String() =
    {"A", "B", "C", "D", "E", "F", "G", "H"}
```

This alone dramatically reduced corruption.

---

### **Remove `notify`**
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

### **Safe MCI Usage**
We added:

- `stop` before `seek`  
- alias health checks  
- corruption recovery  
- stable overlapping playback logic  

This prevented buffer reuse issues.

---

### **Automatic Cleanup Every 5 Minutes**
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



[Top](#pong---code-with-joe)  | [Keyboard Controls](#keyboard-controls) | [AudioPlayer Module](#audioplayer-module) | [Cleanuptick Walkthrough](#cleanuptick-walkthrough) | [Fixed: Audio Playback Issues](#audio-playback-issues) 


---
---
---

























---

# AudioPlayer Module

The `AudioPlayer` module is our custom sound engine built on top of the legacy Windows **MCI (Media Control Interface)** subsystem.  
Although MCI is decades old, this module wraps it in a safe, stable, modernized interface suitable for real‑time game audio in our PONG project.

This document explains **what the module does**, **how it works**, and **why certain design decisions were made**, including the fixes that prevent long‑session audio corruption.

---

## Overview

The module provides:

- Loading WAV or MP3 files into named aliases  
- Playing, looping, pausing, and stopping sounds  
- Overlapping playback (multiple rapid sound effects)  
- Volume control  
- Automatic cleanup every 5 minutes  
- Full recovery of alias state (file path + volume)  
- Loop restoration after cleanup  
- Safe MCI usage that avoids corruption

This module is globally accessible and requires no instantiation.

---

## Architecture Breakdown

### 1. **MCI API Binding**

We bind directly to `mciSendStringW`, the Unicode version of the MCI command interface.  
All audio operations are performed by sending text commands like:

```
open
play
pause
stop
status
close
```

This is the foundation of the module.

---

### 2. **Internal State**

We maintain:

- A `HashSet` of active aliases  
- A fixed set of suffixes (`A`–`H`) for overlapping playback  
- A cleanup timer that refreshes MCI state every 5 minutes  

The overlapping suffixes allow rapid sound effects (menu clicks, paddle hits) without corrupting MCI.

---

### 3. **Lazy Initialization**

The cleanup timer is created only when the first audio command is sent.  
This avoids unnecessary initialization and ensures cleanup is always active.

---

### 4. **Helper Methods**

#### `Normalize(name)`
Ensures alias names contain no spaces.

#### `Send(command)`
Sends an MCI command and returns success/failure.

#### `Query(command)`
Sends an MCI command and returns the string result.

These helpers centralize all MCI communication.

---

### 5. **Core Audio API**

#### `AddSound(soundName, filePath)`
Loads a WAV or MP3 file and assigns it an alias.

#### `PlaySound(soundName)`
Stops, rewinds, and plays a sound once.

#### `LoopSound(soundName)`
Same as `PlaySound`, but loops indefinitely.

#### `PauseSound(soundName)`
Pauses playback.

#### `SetVolume(soundName, level)`
Sets volume from 0–1000 (MCI’s native range).

#### `IsPlaying(soundName)`
Checks whether a sound is currently playing.

These functions form the basic building blocks of the audio system.

---

### 6. **Overlapping Playback**

#### `AddOverlapping(baseName, filePath)`
Loads 8 copies of the same sound:

```
selectA
selectB
...
selectH
```

This allows rapid overlapping playback without corrupting MCI.

#### `PlayOverlapping(baseName)`
Finds the first non‑playing alias and plays it.

#### `SetVolumeOverlapping(baseName, level)`
Sets volume for all overlapping channels.

This simulates mixing in an API that does not support mixing.

---

### 7. **Cleanup System**

#### `CloseAll()`
Stops and closes all aliases used when exiting the game.

---

### 8. **Automatic Cleanup (Every 5 Minutes)**

The cleanup routine:

1. Saves alias name, file path, and volume  
2. Stops and closes all aliases  
3. Reopens each alias fresh  
4. Restores volume  
5. Restarts looping sounds  

This prevents:

- buffer corruption  
- alias leakage  
- distorted audio  
- long‑session instability  

This is the fix that made MCI stable enough for real‑time gameplay.

---

## Why Automatic Cleanup Matters

MCI is old.  
It leaks handles, corrupts buffers, and becomes unstable after long sessions. Especially when simulating mixing with overlapping playback.

Our cleanup system resets MCI’s internal state before corruption accumulates.

This is the key reason our audio engine stays stable for hours.

---

## Summary

The `AudioPlayer` module transforms the fragile MCI subsystem into a reliable game audio engine by adding:

- Safe command handling  
- Overlapping playback  
- Looping support  
- Volume control  
- Automatic corruption recovery  
- Long‑session stability  

Despite MCI’s age, this module makes it robust enough for a modern WinForms game.



[Top](#pong---code-with-joe)  | [Keyboard Controls](#keyboard-controls) | [AudioPlayer Module](#audioplayer-module) | [Cleanuptick Walkthrough](#cleanuptick-walkthrough) | [Fixed Audio Playback Issues](#audio-playback-issues) 



---
---
---









































---

# CleanupTick Walkthrough

```vb
Private Sub CleanupTick(sender As Object, e As EventArgs)
```
Defines the cleanup routine that runs every 5 minutes via a timer.

---

```vb
        ' Store alias info before closing
```
Comment: we’re about to gather all data needed to reopen aliases later.

---

```vb
        Dim reopenList As New List(Of (aliasName As String, filePath As String, volume As Integer))
```
Creates a list of **tuples**, each holding:

- `aliasName` - the MCI alias  
- `filePath` - the WAV or MP3 file path  
- `volume` - the current volume  

A **tuple** is a lightweight way to store multiple values together without creating a class or structure.  
Example:

```vb
(aliasName:="selectA", filePath:="sounds/select.wav", volume:=700)
```

This is a single item containing **three fields**.  
We use tuples to temporarily store alias data during cleanup.

This lets us close everything and reopen it cleanly.

---

```vb
        ' Collect file paths + volume levels
```
Comment: next loop gathers alias metadata.

---

```vb
        For Each aliasName In Aliases.ToList()
```
Iterates over a **copy** of the alias list so we can safely modify the original later.

---

```vb
            Dim path = Query($"info {aliasName} file")
```
Asks MCI for the file path associated with this alias.

---

```vb
            If String.IsNullOrWhiteSpace(path) Then Continue For
```
If MCI returns nothing, skip this alias.

---

```vb
            Dim volStr = Query($"status {aliasName} volume")
```
Queries MCI for the alias’s current volume.

---

```vb
            Dim vol As Integer = 500 ' default fallback
```
Sets a default volume in case parsing fails.

---

```vb
            Integer.TryParse(volStr, vol)
```
Attempts to convert the volume string into an integer.

---

```vb
            reopenList.Add((aliasName, path, vol))
```
Adds a **tuple** containing all three values to `reopenList`.

---

```vb
        Next
```
Ends the metadata‑collection loop.

---

```vb
        ' Close all aliases
```
Comment: next loop shuts down all audio devices.

---

```vb
        For Each aliasName In Aliases.ToList()
```
Iterates over a copy of the alias list again.

---

```vb
            Send($"stop {aliasName}")
```
Stops playback for the alias.

---

```vb
            Send($"close {aliasName}")
```
Closes the alias’s audio device.

---

```vb
            Aliases.Remove(aliasName)
```
Removes the alias from the active alias set.

---

```vb
        Next
```
Ends the closing loop.

---

```vb
        ' Reopen aliases
```
Comment: now we reopen everything using the stored tuple data.

---

```vb
        For Each item In reopenList
```
Iterates over each tuple we saved earlier.

---

```vb
            If Send($"open ""{item.filePath}"" alias {item.aliasName}") Then
```
Reopens the WAV or MP3 file using its original alias name.

---

```vb
                Aliases.Add(item.aliasName)
```
Adds the alias back to the active alias set.

---

```vb
            End If
```
Ends the conditional reopen block.

---

```vb
        Next
```
Ends the reopen loop.

---

```vb
        ' Restore volume levels
```
Comment: next loop restores each alias’s original volume.

---

```vb
        For Each item In reopenList
```
Iterates over each tuple again.

---

```vb
            SetVolume(item.aliasName, item.volume)
```
Restores the volume stored in the tuple.

---

```vb
        Next
```
Ends the volume‑restore loop.

---

```vb
        ' Restart any looping sounds (only once)
```
Comment: looping sounds (like background music) need to be restarted manually.

---

```vb
        Form1.RestartLoops()
```
Calls back into the main form to restart any looping audio.

---

```vb
    End Sub
```
Ends the cleanup routine.

---


[Top](#pong---code-with-joe)  | [Keyboard Controls](#keyboard-controls) | [AudioPlayer Module](#audioplayer-module) | [Cleanuptick Walkthrough](#cleanuptick-walkthrough) | [Fixed Audio Playback Issues](#audio-playback-issues) 


---
---
---



















