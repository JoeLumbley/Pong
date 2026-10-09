Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Pong.Enums

Public Class GameStateManager




    Private currentState As GameState = GameState.StartScreen


    Public Function GetCurrentState() As GameState
        Return currentState
    End Function

    Public Sub SetCurrentState(state As GameState)
        currentState = state

    End Sub

End Class
