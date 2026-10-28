'Public Class FpsCounter

'    Private frameCount As Integer
'    Private FramesPreSecond As Integer

'    Private ReadOnly stopwatch As Stopwatch

'    Public ReadOnly Property FPS As Integer
'        Get
'            Return FramesPreSecond
'        End Get
'    End Property

'    Public Sub Update()

'        frameCount += 1

'        If stopwatch.ElapsedMilliseconds >= 1000 Then
'            FramesPreSecond = frameCount
'            frameCount = 0
'            stopwatch.Restart()
'        End If

'    End Sub

'End Class




Imports System.Diagnostics

Public Class FpsCounter

    Private frameCount As Integer
    Private framesPerSecond As Integer

    Private ReadOnly stopwatch As Stopwatch

    Public Sub New()

        stopwatch = Stopwatch.StartNew()

    End Sub

    Public ReadOnly Property FPS As Integer
        Get
            Return framesPerSecond
        End Get
    End Property

    Public Sub Update()

        frameCount += 1

        If stopwatch.ElapsedMilliseconds >= 1000 Then

            framesPerSecond = frameCount

            frameCount = 0

            stopwatch.Restart()

        End If

    End Sub

End Class