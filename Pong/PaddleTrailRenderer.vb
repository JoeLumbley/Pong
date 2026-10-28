
Imports System.Drawing

Public Class PaddleTrailRenderer

    Private Const PaddleTrailLength As Integer = 4

    Private leftPaddleTrail(PaddleTrailLength - 1) As Rectangle
    Private rightPaddleTrail(PaddleTrailLength - 1) As Rectangle

    Private paddleTrailIndex As Integer = -1
    Private paddleTrailCount As Integer = 0

    Public Sub Update(model As GameModel)

        paddleTrailIndex =
            (paddleTrailIndex + 1) Mod PaddleTrailLength

        leftPaddleTrail(paddleTrailIndex) =
            model.LeftPaddleRect

        rightPaddleTrail(paddleTrailIndex) =
            model.RightPaddleRect

        If paddleTrailCount < PaddleTrailLength Then
            paddleTrailCount += 1
        End If

    End Sub

    Public Sub Reset()

        paddleTrailIndex = -1
        paddleTrailCount = 0

    End Sub

    Public Sub Draw(g As Graphics)

        If paddleTrailCount <= 0 Then Return

        Using trailBrush As New SolidBrush(Color.White)

            For i As Integer = paddleTrailCount - 1 To 0 Step -1

                Dim index As Integer =
                    (paddleTrailIndex - i + PaddleTrailLength) Mod PaddleTrailLength

                Dim alpha As Integer =
                    CInt(8 * (1.0 - i / CDbl(paddleTrailCount)))

                trailBrush.Color =
                    Color.FromArgb(alpha, Color.White)

                g.FillRectangle(
                    trailBrush,
                    leftPaddleTrail(index))

                g.FillRectangle(
                    trailBrush,
                    rightPaddleTrail(index))

            Next

        End Using

    End Sub

End Class


