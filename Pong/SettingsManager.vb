Imports Pong.Enums

Public Class SettingsManager

    Private aiDifficultySelection As Integer = 0 ' 0 = Easy, 1 = Normal, 2 = Hard
    Private aiOptions() As String = {"Easy", "Normal", "Hard"}

    Private pauseMenuSelection As Integer = 0



    Private playerMode As Integer = 1       ' 1 = Single Player (AI), 2 = Two Players
    Private numberOfPlayersSelection As Integer = 0   ' 0 = "1 Player", 1 = "2 Players"

    Private showKeyboardHints As Boolean = True


    Public Function toggleShowKeyboardHints() As Boolean
        showKeyboardHints = Not showKeyboardHints
        Return showKeyboardHints
    End Function

    Public Function GetShowKeyboardHints() As Boolean
        Return showKeyboardHints
    End Function











    Public Function GetPlayerMode() As Integer
        Return playerMode
    End Function

    Public Sub SetPlayerMode(mode As Integer)
        If mode = 1 OrElse mode = 2 Then
            playerMode = mode
        End If
    End Sub

    Public Function GetNumberOfPlayersSelection() As Integer
        Return numberOfPlayersSelection
    End Function

    Public Sub SetNumberOfPlayersSelection(selection As Integer)
        If selection = 0 OrElse selection = 1 Then
            numberOfPlayersSelection = selection
        End If
    End Sub




    Public Function GetPauseMenuSelection() As Integer
        Return pauseMenuSelection
    End Function

    Public Sub SetPauseMenuSelection(selection As Integer)
        pauseMenuSelection = selection
    End Sub


    Public Function GetAIDifficultySelection() As Integer
        Return aiDifficultySelection
    End Function

    Public Sub SetAIDifficultySelection(selection As Integer)
        If selection >= 0 AndAlso selection < aiOptions.Length Then
            aiDifficultySelection = selection
        End If
    End Sub

    Public Function GetAIOptionsLength() As Integer
        Return aiOptions.Length
    End Function





    Public Function GetAIDifficultyLevel() As AIDifficultyLevel
        Select Case aiDifficultySelection
            Case 0
                Return AIDifficultyLevel.Easy
            Case 1
                Return AIDifficultyLevel.Normal
            Case 2
                Return AIDifficultyLevel.Hard
            Case Else
                Return AIDifficultyLevel.Normal ' Default to Normal if out of bounds
        End Select
    End Function

    Public Sub SetAIDifficultyLevel(level As AIDifficultyLevel)
        Select Case level
            Case AIDifficultyLevel.Easy
                aiDifficultySelection = 0
            Case AIDifficultyLevel.Normal
                aiDifficultySelection = 1
            Case AIDifficultyLevel.Hard
                aiDifficultySelection = 2
        End Select
    End Sub

    Public Function GetAIDifficultyString() As String
        Return aiOptions(aiDifficultySelection)
    End Function

    Public Sub SetAIDifficultyString(difficulty As String)
        For i As Integer = 0 To aiOptions.Length - 1
            If aiOptions(i).Equals(difficulty, StringComparison.OrdinalIgnoreCase) Then
                aiDifficultySelection = i
                Exit For
            End If
        Next
    End Sub



End Class
