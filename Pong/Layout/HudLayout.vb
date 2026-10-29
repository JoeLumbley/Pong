Imports System.Drawing

Public Class HudLayout

#Region "Player Labels"

    Public Property LeftLabelSize As SizeF
    Public Property RightLabelSize As SizeF

    Public Property LeftLabelX As Single
    Public Property RightLabelX As Single

    Public Property LabelY As Single

#End Region

#Region "Scores"

    Public Property LeftScoreSize As SizeF
    Public Property RightScoreSize As SizeF

    Public Property LeftScoreX As Single
    Public Property RightScoreX As Single

    Public Property ScoreY As Single

#End Region


#Region "Viewport"

    Public Property ViewportWidth As Integer
    Public Property ViewportHeight As Integer

#End Region



#Region "Gameplay Hints"

    Public Property LeftHintSize As SizeF
    Public Property LeftHintX As Single
    Public Property LeftHintY As Single

    Public Property RightHintSize As SizeF
    Public Property RightHintX As Single
    Public Property RightHintY As Single

    Public Property PauseHintSize As SizeF
    Public Property PauseHintX As Single
    Public Property PauseHintY As Single







    Public Property FpsSize As SizeF
    Public Property FpsX As Single
    Public Property FpsY As Single

#End Region



    Public Const OnePlayerHintText As String =
    "W/S - Move"

    Public Const TwoPlayerLeftHintText As String =
    "W/S - Move"

    Public Const TwoPlayerRightHintText As String =
    "UP/DOWN - Move"

    Public Const PauseHintText As String =
    "P - Pause"

    Public Const FpsLabelText As String =
    "FPS:"



















#Region "Cache"

    Public Sub Cache(
        form As Form,
        model As GameModel,
        resources As GraphicsResources)


        ViewportWidth = form.ClientSize.Width
        ViewportHeight = form.ClientSize.Height

        Using g As Graphics = form.CreateGraphics()

            Dim halfWidth As Single =
                form.ClientSize.Width / 2.0F

            ' ------------------------------------
            ' Player Names
            ' ------------------------------------

            LeftLabelSize =
                g.MeasureString(
                    model.LeftPlayerName,
                    resources.HudLabelFont)

            RightLabelSize =
                g.MeasureString(
                    model.RightPlayerName,
                    resources.HudLabelFont)

            LeftLabelX =
                (halfWidth - LeftLabelSize.Width) / 2.0F

            RightLabelX =
                halfWidth +
                ((halfWidth - RightLabelSize.Width) / 2.0F)

            LabelY =
                form.ClientSize.Height / 14.0F

            ' ------------------------------------
            ' Scores
            ' ------------------------------------

            LeftScoreSize =
                g.MeasureString(
                    model.ScoreLeft.ToString(),
                    resources.HudScoreFont)

            RightScoreSize =
                g.MeasureString(
                    model.ScoreRight.ToString(),
                    resources.HudScoreFont)

            LeftScoreX =
                (halfWidth - LeftScoreSize.Width) / 2.0F

            RightScoreX =
                halfWidth +
                ((halfWidth - RightScoreSize.Width) / 2.0F)

            ScoreY =
                form.ClientSize.Height / 12.0F





            ' ------------------------------------
            ' Gameplay Hints
            ' ------------------------------------

            LeftHintSize =
                g.MeasureString(
                    TwoPlayerLeftHintText,
                    resources.KeyboardFont)

            LeftHintX = 10
            LeftHintY = 10











            If model.PlayerMode = Enums.NumberOfPlayers.OnePlayer Then

                PauseHintSize =
                g.MeasureString(
                    PauseHintText,
                    resources.KeyboardFont)

                'PauseHintX =
                '    (ViewportWidth -
                '     PauseHintSize.Width) / 2.0F

                PauseHintX =
                    ViewportWidth -
                    PauseHintSize.Width -
                    10


                PauseHintY =
                    10


            Else

                RightHintSize =
                g.MeasureString(
                    TwoPlayerRightHintText,
                    resources.KeyboardFont)

                RightHintX =
                ViewportWidth -
                RightHintSize.Width -
                10

                RightHintY = 10




                PauseHintSize =
                g.MeasureString(
                    PauseHintText,
                    resources.KeyboardFont)

                PauseHintX =
                    10


                PauseHintY =
                    ViewportHeight -
                    PauseHintSize.Height -
                    10



            End If







            'FullscreenHintX =
            '    ViewportWidth -
            '    FullscreenHintSize.Width -
            '    10

            'FullscreenHintY = 10







            FpsSize =
                g.MeasureString(
                    "FPS: 999",
                    resources.KeyboardFont)

            FpsX =
                ViewportWidth -
                FpsSize.Width -
                10

            FpsY =
                ViewportHeight -
                FpsSize.Height -
                10







        End Using

    End Sub

#End Region

End Class

