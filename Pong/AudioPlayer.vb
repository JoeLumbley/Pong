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

'    ' MCI becomes unstable above ~8 overlapping channels
'    Private ReadOnly OverlapSuffixes As String() =
'        {"A", "B", "C", "D", "E", "F", "G", "H"}

'    Private cleanupTimer As Timer = Nothing
'    Private cleanupInitialized As Boolean = False

'    ' --- Lazy Initialization -------------------------------------------------

'    Private Sub EnsureCleanupTimer()
'        If cleanupInitialized Then Return

'        cleanupTimer = New Timer() With {
'            .Interval = 2 * 60 * 1000 ' 2 minutes
'        }

'        AddHandler cleanupTimer.Tick, AddressOf CleanupTick
'        cleanupTimer.Start()

'        cleanupInitialized = True
'    End Sub

'    ' --- Helpers -------------------------------------------------------------

'    Private Function Normalize(name As String) As String
'        Return name.Trim().Replace(" ", "_")
'    End Function

'    Private Function Send(command As String) As Boolean
'        EnsureCleanupTimer()

'        Dim sb As New StringBuilder(256)
'        Dim result = mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)

'        If result <> 0 Then
'            Debug.Print($"MCI Error {result}: {command}")
'            Return False
'        End If

'        Return True
'    End Function

'    Private Function Query(command As String) As String
'        EnsureCleanupTimer()

'        Dim sb As New StringBuilder(256)
'        mciSendStringW(command, sb, CUInt(sb.Capacity), IntPtr.Zero)
'        Return sb.ToString().Trim()
'    End Function

'    ' --- Core API ------------------------------------------------------------

'    Public Function AddSound(soundName As String, filePath As String) As Boolean
'        EnsureCleanupTimer()

'        If String.IsNullOrWhiteSpace(soundName) OrElse Not IO.File.Exists(filePath) Then
'            Debug.Print($"{soundName} not added.")
'            Return False
'        End If

'        soundName = Normalize(soundName)

'        If Aliases.Contains(soundName) Then Return True

'        If Send($"open ""{filePath}"" alias {soundName}") Then
'            Aliases.Add(soundName)
'            Return True
'        End If

'        Debug.Print($"{soundName} failed to open.")
'        Return False
'    End Function

'    Public Function PlaySound(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)
'        If Not Aliases.Contains(soundName) Then Return False

'        Send($"stop {soundName}")
'        Send($"seek {soundName} to start")

'        Return Send($"play {soundName}")
'    End Function

'    Public Function LoopSound(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)
'        If Not Aliases.Contains(soundName) Then Return False

'        Send($"stop {soundName}")
'        Send($"seek {soundName} to start")

'        Return Send($"play {soundName} repeat")
'    End Function

'    Public Function PauseSound(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)
'        If Not Aliases.Contains(soundName) Then Return False

'        Return Send($"pause {soundName}")
'    End Function

'    Public Function SetVolume(soundName As String, level As Integer) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)
'        If Not Aliases.Contains(soundName) Then Return False

'        level = Math.Max(0, Math.Min(1000, level))
'        Return Send($"setaudio {soundName} volume to {level}")
'    End Function

'    Public Function IsPlaying(soundName As String) As Boolean
'        EnsureCleanupTimer()

'        soundName = Normalize(soundName)
'        If Not Aliases.Contains(soundName) Then Return False

'        Dim mode = Query($"status {soundName} mode")
'        Return mode.Equals("playing", StringComparison.OrdinalIgnoreCase)
'    End Function

'    ' --- Overlapping Playback ------------------------------------------------

'    Public Sub AddOverlapping(baseName As String, filePath As String)
'        EnsureCleanupTimer()

'        For Each suffix In OverlapSuffixes
'            AddSound(baseName & suffix, filePath)
'        Next
'    End Sub

'    Public Sub PlayOverlapping(baseName As String)
'        EnsureCleanupTimer()

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
'        EnsureCleanupTimer()

'        For Each suffix In OverlapSuffixes
'            SetVolume(baseName & suffix, level)
'        Next
'    End Sub

'    ' --- Cleanup -------------------------------------------------------------

'    Public Sub CloseAll()
'        EnsureCleanupTimer()

'        For Each aliasName In Aliases.ToList()
'            Send($"stop {aliasName}")
'            Send($"close {aliasName}")
'            Aliases.Remove(aliasName)
'        Next
'    End Sub


'    Private Sub CleanupTick(sender As Object, e As EventArgs)
'        ' Store alias info before closing
'        Dim reopenList As New List(Of (aliasName As String, filePath As String, volume As Integer))

'        ' Collect file paths + volume levels
'        For Each aliasName In Aliases.ToList()
'            Dim path = Query($"info {aliasName} file")
'            If String.IsNullOrWhiteSpace(path) Then Continue For

'            Dim volStr = Query($"status {aliasName} volume")
'            Dim vol As Integer = 500 ' default fallback

'            Integer.TryParse(volStr, vol)

'            reopenList.Add((aliasName, path, vol))
'        Next

'        ' Close all aliases
'        For Each aliasName In Aliases.ToList()
'            Send($"stop {aliasName}")
'            Send($"close {aliasName}")
'            Aliases.Remove(aliasName)
'        Next

'        ' Reopen aliases
'        For Each item In reopenList
'            If Send($"open ""{item.filePath}"" alias {item.aliasName}") Then
'                Aliases.Add(item.aliasName)
'            End If
'        Next

'        ' Restore volume levels
'        For Each item In reopenList
'            SetVolume(item.aliasName, item.volume)
'        Next

'        ' Restart any looping sounds (only once)
'        Form1.RestartLoops()
'    End Sub


'End Module




Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms
Imports System.Threading

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

    ' Metadata cache: file path + volume
    Private ReadOnly SoundInfo As New Dictionary(Of String, (filePath As String, volume As Integer))(StringComparer.OrdinalIgnoreCase)

    ' Track which aliases are looping
    Private ReadOnly Looping As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

    ' Cleanup timer (background thread)
    Private cleanupTimer As System.Threading.Timer = Nothing
    Private cleanupInitialized As Boolean = False

    Private ReadOnly syncRoot As New Object()

    ' --- Lazy Initialization -------------------------------------------------

    Private Sub EnsureCleanupTimer()
        If cleanupInitialized Then Return

        SyncLock syncRoot
            If cleanupInitialized Then Return

            ' Run every 2 minutes, off the UI thread
            cleanupTimer = New System.Threading.Timer(
                AddressOf CleanupTick,
                Nothing,
                dueTime:=2 * 60 * 1000,
                period:=2 * 60 * 1000)

            cleanupInitialized = True
        End SyncLock
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

    Private Function AnyPlaying() As Boolean
        ' No SyncLock here to avoid nested locks with Query; we only read
        For Each aliasName In Aliases.ToList()
            Dim mode = Query($"status {aliasName} mode")
            If mode.Equals("playing", StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next
        Return False
    End Function

    ' --- Core API ------------------------------------------------------------

    Public Function AddSound(soundName As String, filePath As String) As Boolean
        EnsureCleanupTimer()

        If String.IsNullOrWhiteSpace(soundName) OrElse Not IO.File.Exists(filePath) Then
            Debug.Print($"{soundName} not added.")
            Return False
        End If

        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Aliases.Contains(soundName) Then Return True

            If Send($"open ""{filePath}"" alias {soundName}") Then
                Aliases.Add(soundName)
                ' Default volume cache (neutral)
                SoundInfo(soundName) = (filePath, 500)
                Return True
            End If
        End SyncLock

        Debug.Print($"{soundName} failed to open.")
        Return False
    End Function

    Public Function PlaySound(soundName As String) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False

            Send($"stop {soundName}")
            Send($"seek {soundName} to start")

            Return Send($"play {soundName}")
        End SyncLock
    End Function

    Public Function LoopSound(soundName As String) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False

            Send($"stop {soundName}")
            Send($"seek {soundName} to start")

            Dim ok = Send($"play {soundName} repeat")
            If ok Then
                Looping.Add(soundName)
            End If
            Return ok
        End SyncLock
    End Function

    Public Function PauseSound(soundName As String) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False

            Return Send($"pause {soundName}")
        End SyncLock
    End Function

    Public Function SetVolume(soundName As String, level As Integer) As Boolean
        EnsureCleanupTimer()

        soundName = Normalize(soundName)

        SyncLock syncRoot
            If Not Aliases.Contains(soundName) Then Return False

            level = Math.Max(0, Math.Min(1000, level))

            Dim ok = Send($"setaudio {soundName} volume to {level}")
            If ok Then
                If SoundInfo.ContainsKey(soundName) Then
                    Dim info = SoundInfo(soundName)
                    SoundInfo(soundName) = (info.filePath, level)
                Else
                    SoundInfo(soundName) = (String.Empty, level)
                End If
            End If

            Return ok
        End SyncLock
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

        SyncLock syncRoot
            For Each suffix In OverlapSuffixes
                Dim aliasName = Normalize(baseName & suffix)
                If Not Aliases.Contains(aliasName) Then Continue For

                Dim mode = Query($"status {aliasName} mode")
                If Not mode.Equals("playing", StringComparison.OrdinalIgnoreCase) Then
                    Send($"stop {aliasName}")
                    Send($"seek {aliasName} to start")
                    Send($"play {aliasName}")
                    Exit Sub
                End If
            Next
        End SyncLock
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

        SyncLock syncRoot
            For Each aliasName In Aliases.ToList()
                Send($"stop {aliasName}")
                Send($"close {aliasName}")
                Aliases.Remove(aliasName)
                SoundInfo.Remove(aliasName)
                Looping.Remove(aliasName)
            Next
        End SyncLock
    End Sub

    Private Sub CleanupTick(state As Object)
        ' Avoid cleanup while anything is actively playing
        If AnyPlaying() Then Return

        Dim reopenList As New List(Of (aliasName As String, filePath As String, volume As Integer))
        Dim loopingList As New List(Of String)

        SyncLock syncRoot
            ' Collect metadata from cache
            For Each aliasName In Aliases.ToList()
                If SoundInfo.ContainsKey(aliasName) Then
                    Dim info = SoundInfo(aliasName)
                    If Not String.IsNullOrWhiteSpace(info.filePath) Then
                        reopenList.Add((aliasName, info.filePath, info.volume))
                    End If
                End If
            Next

            ' Track which were looping
            loopingList.AddRange(Looping)

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
                    SoundInfo(item.aliasName) = (item.filePath, item.volume)
                End If
            Next

            ' Restore volume levels
            For Each item In reopenList
                Send($"setaudio {item.aliasName} volume to {item.volume}")
            Next
        End SyncLock

        ' Restart looping sounds via Form1 (UI thread)
        If loopingList.Count > 0 AndAlso Application.OpenForms.Count > 0 Then
            Dim f = TryCast(Application.OpenForms(0), Form1)
            If f IsNot Nothing Then
                f.BeginInvoke(New Action(Sub()
                                             f.RestartLoops()
                                         End Sub))
            End If
        End If
    End Sub

End Module
