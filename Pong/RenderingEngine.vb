

Imports System.Drawing.Drawing2D
Imports Pong.Enums

Public Class RenderingEngine
    Implements IDisposable

    Private ReadOnly resources As GraphicsResources

    Private ReadOnly startRenderer As StartScreenRenderer

    Private ReadOnly hudRenderer As HudRenderer

    Private ReadOnly pauseRenderer As PauseScreenRenderer

    Private ReadOnly aiRenderer As AIDifficultyRenderer


    Private ReadOnly ballRenderer As BallRenderer

    Private ReadOnly ballTrail As BallTrailRenderer

    Private ReadOnly paddleRenderer As PaddleRenderer

    Private ReadOnly paddleTrailRenderer As PaddleTrailRenderer

    Private ReadOnly impact As WallImpactRenderer


    Private ReadOnly wallImpactEffect As WallImpactEffect
    Private ReadOnly wallImpactRenderer As WallImpactRenderer





    Private ReadOnly gameOverLayout As GameOverLayout
    Private ReadOnly gameOverRenderer As GameOverRenderer

    'Public Sub New(
    'resources As GraphicsResources,
    'startRenderer As StartScreenRenderer,
    'hudRenderer As HudRenderer,
    'pauseRenderer As PauseScreenRenderer,
    'aiRenderer As AIDifficultyRenderer,
    'ballRenderer As BallRenderer,
    'ballTrail As BallTrailRenderer,
    'paddleTrailRenderer As PaddleTrailRenderer)

    '    Me.resources = resources

    '    Me.startRenderer = startRenderer
    '    Me.hudRenderer = hudRenderer
    '    Me.pauseRenderer = pauseRenderer
    '    Me.aiRenderer = aiRenderer

    '    Me.ballRenderer = ballRenderer
    '    Me.ballTrail = ballTrail

    '    Me.paddleTrailRenderer = paddleTrailRenderer

    'End Sub




    Public Sub New(
    resources As GraphicsResources,
    startRenderer As StartScreenRenderer,
    hudRenderer As HudRenderer,
    pauseRenderer As PauseScreenRenderer,
    aiRenderer As AIDifficultyRenderer,
    ballRenderer As BallRenderer,
    ballTrail As BallTrailRenderer,
    paddleTrailRenderer As PaddleTrailRenderer,
    paddleRenderer As PaddleRenderer,
    impact As WallImpactRenderer,
    impactEffect As WallImpactEffect,
    gameOverLayout As GameOverLayout,
    gameOverRenderer As GameOverRenderer)


        Me.resources = resources

        Me.startRenderer = startRenderer
        Me.hudRenderer = hudRenderer
        Me.pauseRenderer = pauseRenderer
        Me.aiRenderer = aiRenderer

        Me.ballRenderer = ballRenderer
        Me.ballTrail = ballTrail

        Me.paddleTrailRenderer = paddleTrailRenderer



        Me.paddleRenderer = paddleRenderer


        Me.impact = impact


        Me.wallImpactEffect = impactEffect


        Me.gameOverLayout = gameOverLayout

        Me.gameOverRenderer = gameOverRenderer

    End Sub




    Public Sub Render(
        g As Graphics,
        model As GameModel,
        form As Form)

        g.CompositingMode = Drawing2D.CompositingMode.SourceOver
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
        g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic

        g.CompositingQuality = Drawing2D.CompositingQuality.GammaCorrected


        g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality











        DrawBackground(
                    g)


        Select Case model.State

            Case GameState.StartScreen


                ballTrail.Draw(g)


                ballRenderer.Draw(
                    g,
                    model)

                startRenderer.Draw(
                    g,
                    model)

            'Case GameState.Playing


            '    ballTrail.Draw(g)

            '    ballRenderer.Draw(
            '        g,
            '        model)

            '    paddleTrailRenderer.Draw(
            '        g)


            '    hudRenderer.Draw(
            '        g,
            '        model)


            Case GameState.Playing

                impact.Draw(g, wallImpactEffect)


                ballTrail.Draw(g)



                paddleTrailRenderer.Draw(g)

                paddleRenderer.Draw(g, model)

                ballRenderer.Draw(g, model)

                hudRenderer.Draw(g, model)



            Case GameState.Pause

                ballTrail.Draw(g)

                ballRenderer.Draw(
                    g,
                    model)

                paddleTrailRenderer.Draw(
                    g)

                paddleRenderer.Draw(g, model)



                hudRenderer.Draw(
                    g,
                    model)

                pauseRenderer.Draw(
                    g,
                    model)


            Case GameState.AIDifficulty

                ballTrail.Draw(g)

                ballRenderer.Draw(
                    g,
                    model)

                aiRenderer.Draw(
                    g,
                    model)

            Case GameState.EndScreen

                ballTrail.Draw(g)

                ballRenderer.Draw(
                    g,
                    model)


                'paddleTrailRenderer.Draw(
                '    g)


                hudRenderer.Draw(
                    g,
                    model)

                ' GameOverRenderer goes here


                gameOverRenderer.Draw(g, form, model, gameOverLayout)

        End Select

    End Sub



    Private Sub DrawBackground(
        g As Graphics)

        Using brush As New LinearGradientBrush(
            New Point(0, 0),
            New Point(0, CInt(g.VisibleClipBounds.Height)),
            Color.FromArgb(32, 32, 32),
            Color.Black)

            g.FillRectangle(
                brush,
                g.VisibleClipBounds)

        End Using

    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose

        ' GraphicsResources are owned and disposed by Form1

    End Sub

End Class



