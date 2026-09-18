'Imports System.Runtime.InteropServices
'Imports System.Text
'Imports System.Windows.Forms
'Imports System.Threading
'Imports System.Diagnostics
'Imports System.Threading.Tasks
'Imports System.IO

'Public Module AudioPlayer

'    ' ============================================================
'    ' MCI API
'    ' ============================================================
'    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
'    Private Function mciSendStringW(
'        <MarshalAs(UnmanagedType.LPWStr)> command As String,
'        <MarshalAs(UnmanagedType.LPWStr)> returnString As StringBuilder,
'        returnLength As UInteger,
'        callback As IntPtr) As Integer
'    End Function

'    ' ============================================================
'    ' Internal State
'    ' ============================================================
'    Private ReadOnly Aliases As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
'    Private ReadOnly SoundInfo As New Dictionary(Of String, (filePath As String, volume As Integer))(StringComparer.OrdinalIgnoreCase)
'    Private ReadOnly Looping As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
'    Private ReadOnly Cooldowns As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

'    Private ReadOnly OverlapSuffixes As String() =
'        {"a", "b", "c", "d", "e", "f", "g", "h"}

'    Private cleanupTimer As System.Threading.Timer
'    Private cleanupInitialized As Boolean

'    Private ReadOnly syncRoot As New Object()

'    ' ============================================================
'    ' Initialization
'    ' ============================================================
'    Private Sub EnsureCleanupTimer()
'        If cleanupInitialized Then Return

'        SyncLock syncRoot
'            If cleanupInitialized Then Return

'            ' Fire cleanup every 6 minutes (360000 ms)
'            cleanupTimer = New System.Threading.Timer(AddressOf CleanupTick, Nothing, 360000, 360000)
'            'cleanupTimer = New System.Threading.Timer(AddressOf CleanupTick, Nothing, 15000, 15000)


'            cleanupInitialized = True
'        End SyncLock

'    End Sub

'    ' ============================================================
'    ' Helpers
'    ' ============================================================
'    Private Function Normalize(name As String) As String
'        If name Is Nothing Then Return ""
'        Return name.Trim().Replace(" ", "_")
'    End Function

'    Private Function ShouldLogError(command As String, code As Integer) As Boolean
'        ' Suppress benign 263 errors for status/stop/close
'        If code = 263 Then
'            Dim c = command.Trim().ToLowerInvariant()
'            If c.StartsWith("status ") OrElse c.StartsWith("stop ") OrElse c.StartsWith("close ") Then
'                Return False
'            End If
'        End If
'        Return True
'    End Function

'    Private Function Send(command As String) As Boolean
'        EnsureCleanupTimer()

'        Dim sb As New StringBuilder(256)
'        Dim result = mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)

'        If result <> 0 AndAlso ShouldLogError(command, result) Then
'            Debug.Print($"MCI Error {result}: {command}")
'            Return False
'        End If

'        Return result = 0
'    End Function

'    Private Function Query(command As String) As String
'        EnsureCleanupTimer()

'        Dim sb As New StringBuilder(256)
'        Dim result = mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)

'        If result <> 0 AndAlso ShouldLogError(command, result) Then
'            Debug.Print($"MCI Error {result}: {command}")
'            Return ""
'        End If

'        Return sb.ToString().Trim()
'    End Function

'    Private Function AnyPlaying() As Boolean
'        Dim snapshot As List(Of String)

'        SyncLock syncRoot
'            snapshot = Aliases.ToList()
'        End SyncLock

'        For Each aliasName In snapshot
'            If Query($"status {aliasName} mode").Equals("playing", StringComparison.OrdinalIgnoreCase) Then
'                Return True
'            End If
'        Next

'        Return False
'    End Function

'    Private Function CooldownReady(soundName As String, ms As Integer) As Boolean
'        Dim now = Environment.TickCount

'        SyncLock syncRoot
'            Dim last As Integer
'            If Cooldowns.TryGetValue(soundName, last) Then
'                If now - last < ms Then Return False
'            End If
'            Cooldowns(soundName) = now
'        End SyncLock

'        Return True
'    End Function

'    Private Function GetDeviceType(filePath As String) As String
'        Dim ext = Path.GetExtension(filePath).ToLowerInvariant()
'        Select Case ext
'            Case ".wav"
'                Return "waveaudio"
'            Case ".mp3"
'                Return "mpegvideo"
'            Case Else
'                Return "" ' let MCI auto-select
'        End Select
'    End Function

'    Private Function OpenSoundInternal(soundName As String, filePath As String, volume As Integer) As Boolean
'        Dim deviceType = GetDeviceType(filePath)
'        Dim ok As Boolean

'        If String.IsNullOrEmpty(deviceType) Then
'            ok = Send($"open ""{filePath}"" alias {soundName}")
'        Else
'            ok = Send($"open ""{filePath}"" type {deviceType} alias {soundName}")
'        End If

'        If ok Then
'            SyncLock syncRoot
'                Aliases.Add(soundName)
'                SoundInfo(soundName) = (filePath, volume)
'            End SyncLock
'        End If

'        Return ok
'    End Function

'    ' ============================================================
'    ' Volume Fade (Async)
'    ' ============================================================
'    Private Async Function FadeVolumeAsync(soundName As String, startVol As Integer, endVol As Integer, durationMs As Integer) As Task
'        Dim steps As Integer = Math.Max(1, durationMs \ 10)
'        Dim delta As Double = (endVol - startVol) / steps
'        Dim current As Double = startVol

'        For i = 1 To steps
'            current += delta
'            SetVolume(soundName, CInt(current))
'            Await Task.Delay(10)
'        Next

'        SetVolume(soundName, endVol)
'    End Function

'    Public Sub FadeVolume(soundName As String, startVol As Integer, endVol As Integer, durationMs As Integer)
'        Dim fadeTask As Task = FadeVolumeAsync(soundName, startVol, endVol, durationMs)

'    End Sub


'    ' ============================================================
'    ' Fade Out (Async Wrapper)
'    ' ============================================================
'    Public Sub FadeOut(soundName As String, durationMs As Integer)
'        soundName = Normalize(soundName)

'        SyncLock syncRoot
'            If Not Aliases.Contains(soundName) Then Exit Sub
'        End SyncLock

'        Dim info = SoundInfo(soundName)
'        Dim currentVol = info.volume

'        ' Fade from current volume to 0
'        FadeVolume(soundName, currentVol, 0, durationMs)
'    End Sub

'    Public Sub FadeOutAndStop(soundName As String, durationMs As Integer)
'        FadeOut(soundName, durationMs)

'        ' Stop AFTER fade completes
'        Task.Run(Async Function()
'                     Await Task.Delay(durationMs)
'                     Send($"stop {Normalize(soundName)}")
'                 End Function)
'    End Sub

'    ' ============================================================
'    ' Core API
'    ' ============================================================
'    Public Function AddSound(soundName As String, filePath As String) As Boolean
'        EnsureCleanupTimer()

'        If String.IsNullOrWhiteSpace(soundName) OrElse Not IO.File.Exists(filePath) Then
'            Debug.Print($"{soundName} not added.")
'            Return False
'        End If

'        soundName = Normalize(soundName)

'        SyncLock syncRoot
'            If Aliases.Contains(soundName) Then Return True
'        End SyncLock

'        If OpenSoundInternal(soundName, filePath, 500) Then
'            Return True
'        End If

'        Debug.Print($"{soundName} failed to open.")
'        Return False
'    End Function

'    Public Function PlaySound(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)
'        If Not CooldownReady(soundName, 40) Then Return False

'        SyncLock syncRoot
'            If Not Aliases.Contains(soundName) Then Return False
'        End SyncLock

'        Send($"stop {soundName}")
'        Send($"seek {soundName} to start")

'        Dim info = SoundInfo(soundName)

'        SetVolume(soundName, 0)
'        FadeVolume(soundName, 0, info.volume, 80)

'        Return Send($"play {soundName}")
'    End Function

'    Public Function LoopSound(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)

'        SyncLock syncRoot
'            If Not Aliases.Contains(soundName) Then Return False
'        End SyncLock

'        Send($"stop {soundName}")
'        Send($"seek {soundName} to start")

'        Dim ok = Send($"play {soundName} repeat")
'        If ok Then
'            SyncLock syncRoot
'                Looping.Add(soundName)
'            End SyncLock
'        End If

'        Return ok
'    End Function

'    Public Function StopSound(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)

'        SyncLock syncRoot
'            If Not Aliases.Contains(soundName) Then Return False
'        End SyncLock

'        Dim info = SoundInfo(soundName)
'        FadeVolume(soundName, info.volume, 0, 2000)

'        Return Send($"stop {soundName}")
'    End Function

'    Public Function PauseSound(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)

'        SyncLock syncRoot
'            If Not Aliases.Contains(soundName) Then Return False
'        End SyncLock

'        Return Send($"pause {soundName}")
'    End Function

'    Public Function SetVolume(soundName As String, level As Integer) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)
'        level = Math.Max(0, Math.Min(1000, level))

'        SyncLock syncRoot
'            If Not Aliases.Contains(soundName) Then Return False
'        End SyncLock

'        Dim ok = Send($"setaudio {soundName} volume to {level}")

'        If ok Then
'            SyncLock syncRoot
'                Dim info = SoundInfo(soundName)
'                SoundInfo(soundName) = (info.filePath, level)
'            End SyncLock
'        End If

'        Return ok
'    End Function

'    Public Function IsPlaying(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)

'        SyncLock syncRoot
'            If Not Aliases.Contains(soundName) Then Return False
'        End SyncLock

'        Return Query($"status {soundName} mode").Equals("playing", StringComparison.OrdinalIgnoreCase)
'    End Function

'    ' ============================================================
'    ' Overlapping Playback
'    ' ============================================================
'    Public Sub AddOverlapping(baseName As String, filePath As String)
'        EnsureCleanupTimer()

'        For Each suffix In OverlapSuffixes
'            AddSound(baseName & suffix, filePath)
'        Next
'    End Sub

'    Public Sub PlayOverlapping(baseName As String)
'        EnsureCleanupTimer()

'        Dim list As List(Of String)

'        SyncLock syncRoot
'            list = OverlapSuffixes.Select(Function(s) Normalize(baseName & s)).
'                                   Where(Function(a) Aliases.Contains(a)).
'                                   ToList()
'        End SyncLock

'        For Each aliasName In list
'            If Not Query($"status {aliasName} mode").Equals("playing", StringComparison.OrdinalIgnoreCase) Then

'                If Not CooldownReady(aliasName, 40) Then Exit Sub

'                Send($"stop {aliasName}")
'                Send($"seek {aliasName} to start")
'                Send($"play {aliasName}")
'                Exit Sub
'            End If
'        Next
'    End Sub

'    Public Sub SetVolumeOverlapping(baseName As String, level As Integer)
'        For Each suffix In OverlapSuffixes
'            SetVolume(baseName & suffix, level)
'        Next
'    End Sub

'    ' ============================================================
'    ' Cleanup
'    ' ============================================================
'    Public Function CloseByAlias(aliasName As String) As Boolean
'        EnsureCleanupTimer()

'        aliasName = Normalize(aliasName)

'        SyncLock syncRoot
'            If Not Aliases.Contains(aliasName) Then Return False
'        End SyncLock

'        Send($"stop {aliasName}")

'        Dim ok = Send($"close {aliasName}")

'        If ok Then
'            SyncLock syncRoot
'                Aliases.Remove(aliasName)
'                SoundInfo.Remove(aliasName)
'                Looping.Remove(aliasName)
'            End SyncLock
'        End If

'        Return ok
'    End Function

'    Public Sub CloseAll()
'        EnsureCleanupTimer()

'        Dim list As List(Of String)

'        SyncLock syncRoot
'            list = Aliases.ToList()
'            Aliases.Clear()
'            SoundInfo.Clear()
'            Looping.Clear()
'        End SyncLock

'        For Each aliasName In list
'            Send($"stop {aliasName}")
'        Next

'        For Each aliasName In list
'            Send($"close {aliasName}")
'        Next
'    End Sub

'    Private Sub CleanupTick(state As Object)
'        If AnyPlaying() Then Return

'        Dim reopenList As New List(Of (aliasName As String, filePath As String, volume As Integer))
'        Dim loopingList As New List(Of String)
'        Dim closeList As New List(Of String)

'        Try
'            SyncLock syncRoot
'                For Each aliasName In Aliases
'                    Dim info = SoundInfo(aliasName)
'                    reopenList.Add((aliasName, info.filePath, info.volume))
'                Next

'                loopingList.AddRange(Looping)
'                closeList.AddRange(Aliases)
'            End SyncLock
'        Catch ex As Exception
'            Debug.WriteLine($"CleanupTick snapshot failed: {ex.Message}")
'            Return
'        End Try

'        For Each aliasName In closeList
'            Send($"stop {aliasName}")
'        Next

'        For Each aliasName In closeList
'            Send($"close {aliasName}")
'        Next

'        Dim successfulReopens As New List(Of String)

'        For Each item In reopenList
'            If OpenSoundInternal(item.aliasName, item.filePath, item.volume) Then
'                successfulReopens.Add(item.aliasName)
'            Else
'                Debug.WriteLine($"Failed to reopen alias: {item.aliasName}")
'            End If
'        Next

'        If successfulReopens.Count > 0 Then
'            SyncLock syncRoot
'                Aliases.Clear()
'                SoundInfo.Clear()
'                For Each aliasName In successfulReopens
'                    Dim info = reopenList.First(Function(r) r.aliasName = aliasName)
'                    Aliases.Add(aliasName)
'                    SoundInfo(aliasName) = (info.filePath, info.volume)
'                Next
'            End SyncLock
'        End If

'        If loopingList.Count > 0 Then
'            RestartLoopsOnUIThread(loopingList)
'        End If
'    End Sub

'    Private Sub RestartLoopsOnUIThread(loopingList As List(Of String))
'        If Application.OpenForms.Count = 0 Then Return

'        Dim mainForm = TryCast(Application.OpenForms(0), Form1)
'        If mainForm Is Nothing OrElse mainForm.IsDisposed Then Return

'        Try
'            mainForm.BeginInvoke(New Action(Sub()
'                                                If Not mainForm.IsDisposed Then
'                                                    mainForm.RestartLoops()
'                                                End If
'                                            End Sub))
'        Catch ex As Exception
'            Debug.WriteLine($"RestartLoops failed: {ex.Message}")
'        End Try
'    End Sub

'End Module





Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading
Imports System.Diagnostics
Imports System.Threading.Tasks
Imports System.IO

Public Class AudioPlayer
    Implements IDisposable

    ' ============================================================
    ' MCI API
    ' ============================================================
    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
    Private Shared Function mciSendStringW(
        <MarshalAs(UnmanagedType.LPWStr)> command As String,
        <MarshalAs(UnmanagedType.LPWStr)> returnString As StringBuilder,
        returnLength As UInteger,
        callback As IntPtr) As Integer
    End Function

    ' ============================================================
    ' Instance State
    ' ============================================================
    Private ReadOnly Aliases As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly SoundInfo As New Dictionary(Of String, (filePath As String, volume As Integer))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly Looping As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly Cooldowns As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

    Private ReadOnly OverlapSuffixes As String() =
        {"a", "b", "c", "d", "e", "f", "g", "h"}

    'Private cleanupTimer As System.Threading.Timer
    Private cleanupInitialized As Boolean

    Private ReadOnly syncRoot As New Object()

    ' ============================================================
    ' Constructor / Destructor
    ' ============================================================
    Public Sub New()
        'EnsureCleanupTimer()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Try
            'cleanupTimer?.Dispose()
        Catch
        End Try

        CloseAll()
    End Sub

    ' ============================================================
    ' Initialization
    ' ============================================================
    'Private Sub EnsureCleanupTimer()
    '    If cleanupInitialized Then Return

    '    SyncLock syncRoot
    '        If cleanupInitialized Then Return

    '        cleanupTimer = New System.Threading.Timer(AddressOf CleanupTick, Nothing, 360000, 360000)
    '        cleanupInitialized = True
    '    End SyncLock
    'End Sub

    ' ============================================================
    ' Helpers
    ' ============================================================
    Private Function Normalize(name As String) As String
        If name Is Nothing Then Return ""
        Return name.Trim().Replace(" ", "_")
    End Function

    Private Function ShouldLogError(command As String, code As Integer) As Boolean
        If code = 263 Then
            Dim c = command.Trim().ToLowerInvariant()
            If c.StartsWith("status ") OrElse c.StartsWith("stop ") OrElse c.StartsWith("close ") Then
                Return False
            End If
        End If
        Return True
    End Function

    Private Function Send(command As String) As Boolean
        Dim sb As New StringBuilder(256)
        Dim result = mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)

        If result <> 0 AndAlso ShouldLogError(command, result) Then
            Debug.Print($"MCI Error {result}: {command}")
            Return False
        End If

        Return result = 0
    End Function

    Private Function Query(command As String) As String
        Dim sb As New StringBuilder(256)
        Dim result = mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)

        If result <> 0 AndAlso ShouldLogError(command, result) Then
            Debug.Print($"MCI Error {result}: {command}")
            Return ""
        End If

        Return sb.ToString().Trim()
    End Function

    Private Function AnyPlaying() As Boolean
        Dim snapshot As List(Of String)

        SyncLock syncRoot
            snapshot = Aliases.ToList()
        End SyncLock

        For Each aliasName In snapshot
            If Query($"status {aliasName} mode").Equals("playing", StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function CooldownReady(soundName As String, ms As Integer) As Boolean
        Dim now = Environment.TickCount

        SyncLock syncRoot
            Dim last As Integer
            If Cooldowns.TryGetValue(soundName, last) Then
                If now - last < ms Then Return False
            End If
            Cooldowns(soundName) = now
        End SyncLock

        Return True
    End Function

    Private Function GetDeviceType(filePath As String) As String
        Dim ext = Path.GetExtension(filePath).ToLowerInvariant()
        Select Case ext
            Case ".wav" : Return "waveaudio"
            Case ".mp3" : Return "mpegvideo"
            Case Else : Return ""
        End Select
    End Function

    Private Function OpenSoundInternal(soundName As String, filePath As String, volume As Integer) As Boolean
        Dim deviceType = GetDeviceType(filePath)
        Dim ok As Boolean

        If String.IsNullOrEmpty(deviceType) Then
            ok = Send($"open ""{filePath}"" alias {soundName}")
        Else
            ok = Send($"open ""{filePath}"" type {deviceType} alias {soundName}")
        End If

        If ok Then
            SyncLock syncRoot
                Aliases.Add(soundName)
                SoundInfo(soundName) = (filePath, volume)
            End SyncLock
        End If

        Return ok
    End Function

    ' ============================================================
    ' Volume Fade (Async)
    ' ============================================================
    Private Async Function FadeVolumeAsync(soundName As String, startVol As Integer, endVol As Integer, durationMs As Integer) As Task
        Dim steps As Integer = Math.Max(1, durationMs \ 10)
        Dim delta As Double = (endVol - startVol) / steps
        Dim current As Double = startVol

        For i = 1 To steps
            current += delta
            SetVolume(soundName, CInt(current))
            Await Task.Delay(10)
        Next

        SetVolume(soundName, endVol)
    End Function

    Public Sub FadeVolume(soundName As String, startVol As Integer, endVol As Integer, durationMs As Integer)
        Dim fadeTask As Task = FadeVolumeAsync(soundName, startVol, endVol, durationMs)
    End Sub

    ' ============================================================
    ' Fade Out
    ' ============================================================
    Public Sub FadeOut(soundName As String, durationMs As Integer)
        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Exit Sub
        End SyncLock

        Dim info = SoundInfo(soundName)
        Dim currentVol = info.volume

        FadeVolume(soundName, currentVol, 0, durationMs)
    End Sub

    Public Sub FadeOutAndStop(soundName As String, durationMs As Integer)
        FadeOut(soundName, durationMs)

        Task.Run(Async Function()
                     Await Task.Delay(durationMs)
                     Send($"stop {Normalize(soundName)}")
                 End Function)
    End Sub

    ' ============================================================
    ' Core API
    ' ============================================================
    Public Function AddSound(soundName As String, filePath As String) As Boolean
        soundName = Normalize(soundName)

        If String.IsNullOrWhiteSpace(soundName) OrElse Not IO.File.Exists(filePath) Then
            Debug.Print($"{soundName} not added.")
            Return False
        End If

        SyncLock syncRoot
            If Aliases.Contains(soundName) Then Return True
        End SyncLock

        If OpenSoundInternal(soundName, filePath, 500) Then
            Return True
        End If

        Debug.Print($"{soundName} failed to open.")
        Return False
    End Function

    Public Function PlaySound(soundName As String) As Boolean
        soundName = Normalize(soundName)
        If Not CooldownReady(soundName, 40) Then Return False

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False
        End SyncLock

        Send($"stop {soundName}")
        Send($"seek {soundName} to start")

        Dim info = SoundInfo(soundName)

        SetVolume(soundName, 0)
        FadeVolume(soundName, 0, info.volume, 80)

        Return Send($"play {soundName}")
    End Function

    Public Function LoopSound(soundName As String) As Boolean
        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False
        End SyncLock

        Send($"stop {soundName}")
        Send($"seek {soundName} to start")

        Dim ok = Send($"play {soundName} repeat")
        If ok Then
            SyncLock syncRoot
                Looping.Add(soundName)
            End SyncLock
        End If

        Return ok
    End Function

    Public Function StopSound(soundName As String) As Boolean
        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False
        End SyncLock

        Dim info = SoundInfo(soundName)
        FadeVolume(soundName, info.volume, 0, 2000)

        Return Send($"stop {soundName}")
    End Function

    Public Function PauseSound(soundName As String) As Boolean
        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False
        End SyncLock

        Return Send($"pause {soundName}")
    End Function

    Public Function SetVolume(soundName As String, level As Integer) As Boolean
        soundName = Normalize(soundName)
        level = Math.Max(0, Math.Min(1000, level))

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False
        End SyncLock

        Dim ok = Send($"setaudio {soundName} volume to {level}")

        If ok Then
            SyncLock syncRoot
                Dim info = SoundInfo(soundName)
                SoundInfo(soundName) = (info.filePath, level)
            End SyncLock
        End If

        Return ok
    End Function

    Public Function IsPlaying(soundName As String) As Boolean
        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False
        End SyncLock

        Return Query($"status {soundName} mode").Equals("playing", StringComparison.OrdinalIgnoreCase)
    End Function

    ' ============================================================
    ' Overlapping Playback
    ' ============================================================
    Public Sub AddOverlapping(baseName As String, filePath As String)
        For Each suffix In OverlapSuffixes
            AddSound(baseName & suffix, filePath)
        Next
    End Sub

    Public Sub PlayOverlapping(baseName As String)
        Dim list As List(Of String)

        SyncLock syncRoot
            list = OverlapSuffixes.Select(Function(s) Normalize(baseName & s)).
                                   Where(Function(a) Aliases.Contains(a)).
                                   ToList()
        End SyncLock

        For Each aliasName In list
            If Not Query($"status {aliasName} mode").Equals("playing", StringComparison.OrdinalIgnoreCase) Then

                If Not CooldownReady(aliasName, 40) Then Exit Sub

                Send($"stop {aliasName}")
                Send($"seek {aliasName} to start")
                Send($"play {aliasName}")
                Exit Sub
            End If
        Next
    End Sub

    Public Sub SetVolumeOverlapping(baseName As String, level As Integer)
        For Each suffix In OverlapSuffixes
            SetVolume(baseName & suffix, level)
        Next
    End Sub

    ' ============================================================
    ' Cleanup
    ' ============================================================
    Public Function CloseByAlias(aliasName As String) As Boolean
        aliasName = Normalize(aliasName)

        SyncLock syncRoot
            If Not Aliases.Contains(aliasName) Then Return False
        End SyncLock

        Send($"stop {aliasName}")

        Dim ok = Send($"close {aliasName}")

        If ok Then
            SyncLock syncRoot
                Aliases.Remove(aliasName)
                SoundInfo.Remove(aliasName)
                Looping.Remove(aliasName)
            End SyncLock
        End If

        Return ok
    End Function

    Public Sub CloseAll()
        Dim list As List(Of String)

        SyncLock syncRoot
            list = Aliases.ToList()
            Aliases.Clear()
            SoundInfo.Clear()
            Looping.Clear()
        End SyncLock

        For Each aliasName In list
            Send($"stop {aliasName}")
        Next

        For Each aliasName In list
            Send($"close {aliasName}")
        Next
    End Sub

    Private Sub CleanupTick(state As Object)
        If AnyPlaying() Then Return

        Dim reopenList As New List(Of (aliasName As String, filePath As String, volume As Integer))
        Dim loopingList As New List(Of String)
        Dim closeList As New List(Of String)

        Try
            SyncLock syncRoot
                For Each aliasName In Aliases
                    Dim info = SoundInfo(aliasName)
                    reopenList.Add((aliasName, info.filePath, info.volume))
                Next

                loopingList.AddRange(Looping)
                closeList.AddRange(Aliases)
            End SyncLock
        Catch ex As Exception
            Debug.WriteLine($"CleanupTick snapshot failed: {ex.Message}")
            Return
        End Try

        For Each aliasName In closeList
            Send($"stop {aliasName}")
        Next

        For Each aliasName In closeList
            Send($"close {aliasName}")
        Next

        Dim successfulReopens As New List(Of String)

        For Each item In reopenList
            If OpenSoundInternal(item.aliasName, item.filePath, item.volume) Then
                successfulReopens.Add(item.aliasName)
            Else
                Debug.WriteLine($"Failed to reopen alias: {item.aliasName}")
            End If
        Next

        If successfulReopens.Count > 0 Then
            SyncLock syncRoot
                Aliases.Clear()
                SoundInfo.Clear()
                For Each aliasName In successfulReopens
                    Dim info = reopenList.First(Function(r) r.aliasName = aliasName)
                    Aliases.Add(aliasName)
                    SoundInfo(aliasName) = (info.filePath, info.volume)
                Next
            End SyncLock
        End If

        If loopingList.Count > 0 Then
            RestartLoopsOnUIThread(loopingList)
        End If
    End Sub

    Private Sub RestartLoopsOnUIThread(loopingList As List(Of String))
        If Application.OpenForms.Count = 0 Then Return

        Dim mainForm = TryCast(Application.OpenForms(0), Form1)
        If mainForm Is Nothing OrElse mainForm.IsDisposed Then Return

        Try
            mainForm.BeginInvoke(New Action(Sub()
                                                If Not mainForm.IsDisposed Then
                                                    mainForm.RestartLoops()
                                                End If
                                            End Sub))
        Catch ex As Exception
            Debug.WriteLine($"RestartLoops failed: {ex.Message}")
        End Try
    End Sub

End Class
