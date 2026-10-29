Imports System.Drawing.Drawing2D
Imports Pong.Enums

Public Class RenderingEngine
    Implements IDisposable

    Private ReadOnly startRenderer As StartScreenRenderer
    Private ReadOnly hudRenderer As HudRenderer
    Private ReadOnly pauseRenderer As PauseScreenRenderer
    Private ReadOnly aiRenderer As AIDifficultyRenderer

    Private ReadOnly ballRenderer As BallRenderer
    Private ReadOnly ballTrailRenderer As BallTrailRenderer

    Private ReadOnly paddleRenderer As PaddleRenderer
    Private ReadOnly paddleTrailRenderer As PaddleTrailRenderer

    Private ReadOnly wallImpactRenderer As WallImpactRenderer
    Private ReadOnly wallImpactEffect As WallImpactEffect

    Private ReadOnly gameOverLayout As GameOverLayout
    Private ReadOnly gameOverRenderer As GameOverRenderer

    Public Sub New(
        startRenderer As StartScreenRenderer,
        hudRenderer As HudRenderer,
        pauseRenderer As PauseScreenRenderer,
        aiRenderer As AIDifficultyRenderer,
        ballRenderer As BallRenderer,
        ballTrailRenderer As BallTrailRenderer,
        paddleTrailRenderer As PaddleTrailRenderer,
        paddleRenderer As PaddleRenderer,
        wallImpactRenderer As WallImpactRenderer,
        wallImpactEffect As WallImpactEffect,
        gameOverLayout As GameOverLayout,
        gameOverRenderer As GameOverRenderer)

        Me.startRenderer = startRenderer
        Me.hudRenderer = hudRenderer
        Me.pauseRenderer = pauseRenderer
        Me.aiRenderer = aiRenderer

        Me.ballRenderer = ballRenderer
        Me.ballTrailRenderer = ballTrailRenderer

        Me.paddleTrailRenderer = paddleTrailRenderer
        Me.paddleRenderer = paddleRenderer

        Me.wallImpactRenderer = wallImpactRenderer
        Me.wallImpactEffect = wallImpactEffect

        Me.gameOverLayout = gameOverLayout
        Me.gameOverRenderer = gameOverRenderer

    End Sub

    Public Sub Render(
        g As Graphics,
        model As GameModel,
        form As Form)

        ConfigureGraphics(g)

        DrawBackground(g)

        RenderBall(g, model)

        Select Case model.State

            Case GameState.StartScreen

                startRenderer.Draw(g, model)

            Case GameState.Playing

                wallImpactRenderer.Draw(
                    g,
                    wallImpactEffect)

                RenderPaddles(g, model)

                hudRenderer.Draw(
                    g,
                    model)

            Case GameState.Pause

                RenderPaddles(g, model)

                hudRenderer.Draw(
                    g,
                    model)

                pauseRenderer.Draw(
                    g,
                    model)

            Case GameState.AIDifficulty

                aiRenderer.Draw(
                    g,
                    model)

            Case GameState.EndScreen

                'RenderPaddles(g, model)

                hudRenderer.Draw(
                    g,
                    model)

                gameOverRenderer.Draw(
                    g,
                    form,
                    model,
                    gameOverLayout)

        End Select

    End Sub

    Private Shared Sub ConfigureGraphics(
        g As Graphics)

        g.CompositingMode = CompositingMode.SourceOver
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.CompositingQuality = CompositingQuality.GammaCorrected
        g.PixelOffsetMode = PixelOffsetMode.HighQuality

    End Sub

    Private Sub RenderBall(
        g As Graphics,
        model As GameModel)

        ballTrailRenderer.Draw(g)
        ballRenderer.Draw(g, model)

    End Sub

    Private Sub RenderPaddles(
        g As Graphics,
        model As GameModel)

        paddleTrailRenderer.Draw(g)
        paddleRenderer.Draw(g, model)

    End Sub

    Private Shared Sub DrawBackground(
        g As Graphics)

        Using backgroundBrush As New LinearGradientBrush(
            New RectangleF(g.VisibleClipBounds),
            Color.FromArgb(25, 25, 25),
            Color.Black,
            LinearGradientMode.Vertical)

            g.FillRectangle(
                backgroundBrush,
                g.VisibleClipBounds)

        End Using

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        ' GraphicsResources are owned by Form1

    End Sub

End Class


