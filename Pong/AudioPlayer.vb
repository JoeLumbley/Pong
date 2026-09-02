'Imports System.Runtime.InteropServices
'Imports System.Text

'Public Module AudioPlayer

'    ' --- MCI API -------------------------------------------------------------

'    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
'    Private Function mciSendStringW(
'        <MarshalAs(UnmanagedType.LPWStr)> command As String,
'        <MarshalAs(UnmanagedType.LPWStr)> returnString As StringBuilder,
'        returnLength As UInteger,
'        callback As IntPtr) As Integer
'    End Function

'    ' --- Internal State ------------------------------------------------------

'    Private ReadOnly Aliases As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

'    Private ReadOnly OverlapSuffixes As String() =
'        Enumerable.Range(0, 24).Select(Function(i) Chr(Asc("A") + i).ToString()).ToArray()

'    ' --- Helpers -------------------------------------------------------------

'    Private Function Normalize(name As String) As String
'        Return name.Trim().Replace(" ", "_")
'    End Function

'    Private Function Send(command As String) As Boolean
'        Dim sb As New StringBuilder(256)
'        Dim result = mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)

'        If result <> 0 Then
'            Debug.Print($"MCI Error {result}: {command}")
'            Return False
'        End If

'        Return True
'    End Function

'    Private Function Query(command As String) As String
'        Dim sb As New StringBuilder(256)
'        mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)
'        Return sb.ToString().Trim()
'    End Function

'    ' --- Core API ------------------------------------------------------------

'    Public Function AddSound(soundName As String, filePath As String) As Boolean
'        If String.IsNullOrWhiteSpace(soundName) OrElse Not IO.File.Exists(filePath) Then
'            Debug.Print($"{soundName} not added.")
'            Return False
'        End If

'        soundName = Normalize(soundName)

'        If Aliases.Contains(soundName) Then Return True

'        Dim cmd = $"open ""{filePath}"" alias {soundName}"

'        If Send(cmd) Then
'            Aliases.Add(soundName)
'            Return True
'        End If

'        Debug.Print($"{soundName} failed to open.")
'        Return False
'    End Function

'    Public Function PlaySound(soundName As String) As Boolean
'        soundName = Normalize(soundName)

'        If Not Aliases.Contains(soundName) Then
'            Debug.Print($"{soundName} not playing.")
'            Return False
'        End If

'        Return Send($"seek {soundName} to start") AndAlso
'               Send($"play {soundName} notify")
'    End Function

'    Public Function LoopSound(soundName As String) As Boolean
'        soundName = Normalize(soundName)

'        If Not Aliases.Contains(soundName) Then
'            Debug.Print($"{soundName} not looping.")
'            Return False
'        End If

'        Return Send($"seek {soundName} to start") AndAlso
'               Send($"play {soundName} repeat")
'    End Function

'    Public Function PauseSound(soundName As String) As Boolean
'        soundName = Normalize(soundName)

'        If Not Aliases.Contains(soundName) Then
'            Debug.Print($"{soundName} not paused.")
'            Return False
'        End If

'        Return Send($"pause {soundName} notify")
'    End Function

'    Public Function SetVolume(soundName As String, level As Integer) As Boolean
'        soundName = Normalize(soundName)

'        If Not Aliases.Contains(soundName) Then
'            Debug.Print($"{soundName} volume not set.")
'            Return False
'        End If

'        level = Math.Max(0, Math.Min(1000, level))

'        Return Send($"setaudio {soundName} volume to {level}")
'    End Function

'    Public Function IsPlaying(soundName As String) As Boolean
'        soundName = Normalize(soundName)
'        If Not Aliases.Contains(soundName) Then Return False
'        Return Query($"status {soundName} mode").Equals("playing", StringComparison.OrdinalIgnoreCase)
'    End Function

'    ' --- Overlapping Playback ------------------------------------------------

'    Public Sub AddOverlapping(baseName As String, filePath As String)
'        For Each suffix In OverlapSuffixes
'            AddSound(baseName & suffix, filePath)
'        Next
'    End Sub

'    Public Sub PlayOverlapping(baseName As String)
'        For Each suffix In OverlapSuffixes
'            Dim aliasName = Normalize(baseName & suffix)
'            If Not IsPlaying(aliasName) Then
'                PlaySound(aliasName)
'                Exit Sub
'            End If
'        Next
'    End Sub

'    Public Sub SetVolumeOverlapping(baseName As String, level As Integer)
'        For Each suffix In OverlapSuffixes
'            SetVolume(baseName & suffix, level)
'        Next
'    End Sub

'    ' --- Cleanup -------------------------------------------------------------

'    Public Sub CloseAll()
'        For Each aliasName In Aliases
'            Send($"close {aliasName}")
'        Next

'        Aliases.Clear()
'    End Sub

'End Module




'Imports System.Runtime.InteropServices
'Imports System.Text
'Imports System.Windows.Forms

'Public Module AudioPlayer

'    ' --- MCI API -------------------------------------------------------------

'    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
'    Private Function mciSendStringW(
'        <MarshalAs(UnmanagedType.LPWStr)> command As String,
'        <MarshalAs(UnmanagedType.LPWStr)> returnString As StringBuilder,
'        returnLength As UInteger,
'        callback As IntPtr) As Integer
'    End Function

'    ' --- Internal State ------------------------------------------------------

'    Private ReadOnly Aliases As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

'    ' Keep overlapping count modest for MCI stability
'    Private ReadOnly OverlapSuffixes As String() =
'        {"A", "B", "C", "D", "E", "F", "G", "H"}

'    Private ReadOnly cleanupTimer As New Timer() With {
'        .Interval = 5 * 60 * 1000 ' 5 minutes
'    }

'    ' Static constructor: start cleanup timer
'    Shared Sub New()
'        AddHandler cleanupTimer.Tick, AddressOf CleanupTick
'        cleanupTimer.Start()
'    End Sub

'    ' --- Helpers -------------------------------------------------------------

'    Private Function Normalize(name As String) As String
'        Return name.Trim().Replace(" ", "_")
'    End Function

'    Private Function Send(command As String) As Boolean
'        Dim sb As New StringBuilder(256)
'        Dim result = mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)

'        If result <> 0 Then
'            Debug.Print($"MCI Error {result}: {command}")
'            Return False
'        End If

'        Return True
'    End Function

'    Private Function Query(command As String) As String
'        Dim sb As New StringBuilder(256)
'        mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)
'        Return sb.ToString().Trim()
'    End Function

'    ' --- Core API ------------------------------------------------------------

'    Public Function AddSound(soundName As String, filePath As String) As Boolean
'        If String.IsNullOrWhiteSpace(soundName) OrElse Not IO.File.Exists(filePath) Then
'            Debug.Print($"{soundName} not added.")
'            Return False
'        End If

'        soundName = Normalize(soundName)

'        If Aliases.Contains(soundName) Then Return True

'        Dim cmd = $"open ""{filePath}"" alias {soundName}"

'        If Send(cmd) Then
'            Aliases.Add(soundName)
'            Return True
'        End If

'        Debug.Print($"{soundName} failed to open.")
'        Return False
'    End Function

'    Public Function PlaySound(soundName As String) As Boolean
'        soundName = Normalize(soundName)

'        If Not Aliases.Contains(soundName) Then
'            Debug.Print($"{soundName} not playing.")
'            Return False
'        End If

'        ' Stop first to avoid buffer reuse issues
'        Send($"stop {soundName}")
'        Send($"seek {soundName} to start")

'        Return Send($"play {soundName}")
'    End Function

'    Public Function LoopSound(soundName As String) As Boolean
'        soundName = Normalize(soundName)

'        If Not Aliases.Contains(soundName) Then
'            Debug.Print($"{soundName} not looping.")
'            Return False
'        End If

'        Send($"stop {soundName}")
'        Send($"seek {soundName} to start")

'        Return Send($"play {soundName} repeat")
'    End Function

'    Public Function PauseSound(soundName As String) As Boolean
'        soundName = Normalize(soundName)

'        If Not Aliases.Contains(soundName) Then
'            Debug.Print($"{soundName} not paused.")
'            Return False
'        End If

'        Return Send($"pause {soundName}")
'    End Function

'    Public Function SetVolume(soundName As String, level As Integer) As Boolean
'        soundName = Normalize(soundName)

'        If Not Aliases.Contains(soundName) Then
'            Debug.Print($"{soundName} volume not set.")
'            Return False
'        End If

'        level = Math.Max(0, Math.Min(1000, level))

'        Return Send($"setaudio {soundName} volume to {level}")
'    End Function

'    Public Function IsPlaying(soundName As String) As Boolean
'        soundName = Normalize(soundName)
'        If Not Aliases.Contains(soundName) Then Return False

'        Dim mode = Query($"status {soundName} mode")
'        Return mode.Equals("playing", StringComparison.OrdinalIgnoreCase)
'    End Function

'    ' --- Overlapping Playback ------------------------------------------------

'    Public Sub AddOverlapping(baseName As String, filePath As String)
'        For Each suffix In OverlapSuffixes
'            AddSound(baseName & suffix, filePath)
'        Next
'    End Sub

'    Public Sub PlayOverlapping(baseName As String)
'        For Each suffix In OverlapSuffixes
'            Dim aliasName = Normalize(baseName & suffix)
'            If Not Aliases.Contains(aliasName) Then Continue For

'            If Not IsPlaying(aliasName) Then
'                PlaySound(aliasName)
'                Exit Sub
'            End If
'        Next
'    End Sub

'    Public Sub SetVolumeOverlapping(baseName As String, level As Integer)
'        For Each suffix In OverlapSuffixes
'            SetVolume(baseName & suffix, level)
'        Next
'    End Sub

'    ' --- Cleanup -------------------------------------------------------------

'    Public Sub CloseAll()
'        For Each aliasName In Aliases.ToList()
'            Send($"stop {aliasName}")
'            Send($"close {aliasName}")
'            Aliases.Remove(aliasName)
'        Next
'    End Sub

'    ' Automatic cleanup every 5 minutes:
'    ' stop + close + reopen all aliases to keep MCI stable
'    Private Sub CleanupTick(sender As Object, e As EventArgs)
'        Dim reopenList As New List(Of (aliasName As String, filePath As String))

'        ' Collect file paths for all aliases
'        For Each aliasName In Aliases.ToList()
'            Dim path = Query($"info {aliasName} file")
'            If String.IsNullOrWhiteSpace(path) Then Continue For
'            reopenList.Add((aliasName, path))
'        Next

'        ' Close all current aliases
'        For Each aliasName In Aliases.ToList()
'            Send($"stop {aliasName}")
'            Send($"close {aliasName}")
'            Aliases.Remove(aliasName)
'        Next

'        ' Reopen aliases with original file paths
'        For Each item In reopenList
'            Dim cmd = $"open ""{item.filePath}"" alias {item.aliasName}"
'            If Send(cmd) Then
'                Aliases.Add(item.aliasName)
'            End If
'        Next
'    End Sub

'End Module



Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Public Module AudioPlayer

    ' --- MCI API -------------------------------------------------------------

    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
    Private Function mciSendStringW(
        <MarshalAs(UnmanagedType.LPWStr)> command As String,
        <MarshalAs(UnmanagedType.LPWStr)> returnString As StringBuilder,
        returnLength As UInteger,
        callback As IntPtr) As Integer
    End Function

    ' --- Internal State ------------------------------------------------------

    Private ReadOnly Aliases As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

    ' MCI becomes unstable above ~8 overlapping channels
    Private ReadOnly OverlapSuffixes As String() =
        {"A", "B", "C", "D", "E", "F", "G", "H"}

    Private cleanupTimer As Timer = Nothing
    Private cleanupInitialized As Boolean = False

    ' --- Lazy Initialization -------------------------------------------------

    Private Sub EnsureCleanupTimer()
        If cleanupInitialized Then Return

        cleanupTimer = New Timer() With {
            .Interval = 5 * 60 * 1000 ' 5 minutes
        }


        AddHandler cleanupTimer.Tick, AddressOf CleanupTick
        cleanupTimer.Start()

        cleanupInitialized = True
    End Sub

    ' --- Helpers -------------------------------------------------------------

    Private Function Normalize(name As String) As String
        Return name.Trim().Replace(" ", "_")
    End Function

    Private Function Send(command As String) As Boolean
        EnsureCleanupTimer()

        Dim sb As New StringBuilder(256)
        Dim result = mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)

        If result <> 0 Then
            Debug.Print($"MCI Error {result}: {command}")
            Return False
        End If

        Return True
    End Function

    Private Function Query(command As String) As String
        EnsureCleanupTimer()

        Dim sb As New StringBuilder(256)
        mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)
        Return sb.ToString().Trim()
    End Function

    ' --- Core API ------------------------------------------------------------

    Public Function AddSound(soundName As String, filePath As String) As Boolean
        EnsureCleanupTimer()

        If String.IsNullOrWhiteSpace(soundName) OrElse Not IO.File.Exists(filePath) Then
            Debug.Print($"{soundName} not added.")
            Return False
        End If

        soundName = Normalize(soundName)

        If Aliases.Contains(soundName) Then Return True

        If Send($"open ""{filePath}"" alias {soundName}") Then
            Aliases.Add(soundName)
            Return True
        End If

        Debug.Print($"{soundName} failed to open.")
        Return False
    End Function

    Public Function PlaySound(soundName As String) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)
        If Not Aliases.Contains(soundName) Then Return False

        Send($"stop {soundName}")
        Send($"seek {soundName} to start")

        Return Send($"play {soundName}")
    End Function

    Public Function LoopSound(soundName As String) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)
        If Not Aliases.Contains(soundName) Then Return False

        Send($"stop {soundName}")
        Send($"seek {soundName} to start")

        Return Send($"play {soundName} repeat")
    End Function

    Public Function PauseSound(soundName As String) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)
        If Not Aliases.Contains(soundName) Then Return False

        Return Send($"pause {soundName}")
    End Function

    Public Function SetVolume(soundName As String, level As Integer) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)
        If Not Aliases.Contains(soundName) Then Return False

        level = Math.Max(0, Math.Min(1000, level))
        Return Send($"setaudio {soundName} volume to {level}")
    End Function

    Public Function IsPlaying(soundName As String) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)
        If Not Aliases.Contains(soundName) Then Return False

        Dim mode = Query($"status {soundName} mode")
        Return mode.Equals("playing", StringComparison.OrdinalIgnoreCase)
    End Function

    ' --- Overlapping Playback ------------------------------------------------

    Public Sub AddOverlapping(baseName As String, filePath As String)
        EnsureCleanupTimer()

        For Each suffix In OverlapSuffixes
            AddSound(baseName & suffix, filePath)
        Next
    End Sub

    Public Sub PlayOverlapping(baseName As String)
        EnsureCleanupTimer()

        For Each suffix In OverlapSuffixes
            Dim aliasName = Normalize(baseName & suffix)
            If Not Aliases.Contains(aliasName) Then Continue For

            If Not IsPlaying(aliasName) Then
                PlaySound(aliasName)
                Exit Sub
            End If
        Next
    End Sub

    Public Sub SetVolumeOverlapping(baseName As String, level As Integer)
        EnsureCleanupTimer()

        For Each suffix In OverlapSuffixes
            SetVolume(baseName & suffix, level)
        Next
    End Sub

    ' --- Cleanup -------------------------------------------------------------

    Public Sub CloseAll()
        EnsureCleanupTimer()

        For Each aliasName In Aliases.ToList()
            Send($"stop {aliasName}")
            Send($"close {aliasName}")
            Aliases.Remove(aliasName)
        Next
    End Sub

    'Private Sub CleanupTick(sender As Object, e As EventArgs)
    '    Dim reopenList As New List(Of (aliasName As String, filePath As String))

    '    ' Collect file paths
    '    For Each aliasName In Aliases.ToList()
    '        Dim path = Query($"info {aliasName} file")
    '        If String.IsNullOrWhiteSpace(path) Then Continue For
    '        reopenList.Add((aliasName, path))
    '    Next

    '    ' Close all aliases
    '    For Each aliasName In Aliases.ToList()
    '        Send($"stop {aliasName}")
    '        Send($"close {aliasName}")
    '        Aliases.Remove(aliasName)
    '    Next

    '    ' Reopen aliases
    '    For Each item In reopenList
    '        If Send($"open ""{item.filePath}"" alias {item.aliasName}") Then
    '            Aliases.Add(item.aliasName)
    '        End If
    '    Next

    '    ' Restore the volume levels for all aliases
    '    For Each item In reopenList
    '        Dim volume = Query($"status {item.aliasName} volume")
    '        If Integer.TryParse(volume, Nothing) Then
    '            SetVolume(item.aliasName, CInt(volume))
    '        End If



    '        ' Restart any loops that were active before cleanup

    '        Form1.RestartLoops()

    'End Sub

    Private Sub CleanupTick(sender As Object, e As EventArgs)
        ' Store alias info before closing
        Dim reopenList As New List(Of (aliasName As String, filePath As String, volume As Integer))

        ' Collect file paths + volume levels
        For Each aliasName In Aliases.ToList()
            Dim path = Query($"info {aliasName} file")
            If String.IsNullOrWhiteSpace(path) Then Continue For

            Dim volStr = Query($"status {aliasName} volume")
            Dim vol As Integer = 500 ' default fallback

            Integer.TryParse(volStr, vol)

            reopenList.Add((aliasName, path, vol))
        Next

        ' Close all aliases
        For Each aliasName In Aliases.ToList()
            Send($"stop {aliasName}")
            Send($"close {aliasName}")
            Aliases.Remove(aliasName)
        Next

        ' Reopen aliases
        For Each item In reopenList
            If Send($"open ""{item.filePath}"" alias {item.aliasName}") Then
                Aliases.Add(item.aliasName)
            End If
        Next

        ' Restore volume levels
        For Each item In reopenList
            SetVolume(item.aliasName, item.volume)
        Next

        ' Restart any looping sounds (only once)
        Form1.RestartLoops()
    End Sub


End Module
