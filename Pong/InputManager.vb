Imports Pong.Enums

Public Class InputManager

    ' -------------------------------
    '  Input Repeat Guards
    ' -------------------------------
    Private pauseKeyDown As Boolean = False
    Private pKeyDown As Boolean = False
    Private mediaPlayPauseKeyDown As Boolean = False
    Private f11KeyDown As Boolean = False
    Private fKeyDown As Boolean = False
    Private escapeKeyDown As Boolean = False
    Private spaceKeyDown As Boolean = False
    Private enterKeyDown As Boolean = False
    Private upKeyDown As Boolean = False
    Private downKeyDown As Boolean = False

    Private wKeyDown As Boolean = False
    Private sKeyDown As Boolean = False
    Private ctrlQDown As Boolean = False
    Private ctrlHDown As Boolean = False

    Private mouseIsClicking As Boolean = False

    Private startScreenScrollAccum As Integer = 0
    Private Const ScrollThreshold As Integer = 400

    Private aiDifficultyScrollAccum As Integer = 0

    Private pauseScrollAccum As Integer = 0

    ' ===========================
    '  PUBLIC INTERFACE
    ' ===========================

    Public Sub OnKeyDown(e As KeyEventArgs,
                         form As Form,
                         match As MatchManager,
                         audio As AudioController,
                         window As WindowManager,
                         renderer As Rendering,
                         physics As PhysicsEngine,
                         model As GameModel)

        ' ============================================================
        ' 1. Fullscreen Toggle (F11 / F)
        ' ============================================================
        If e.KeyCode = Keys.F11 OrElse e.KeyCode = Keys.F Then

            ' Repeat‑guard
            If (e.KeyCode = Keys.F11 AndAlso f11KeyDown) OrElse
            (e.KeyCode = Keys.F AndAlso fKeyDown) Then Return

            ' Mark correct key as down
            If e.KeyCode = Keys.F11 Then
                f11KeyDown = True
            Else
                fKeyDown = True
            End If

            audio.PlayFullScreenSound()
            window.ToggleFullScreen()
            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 2. Escape pressed while fullscreen (exit fullscreen)
        ' ============================================================
        If form.FormBorderStyle = FormBorderStyle.None AndAlso
       e.KeyCode = Keys.Escape Then

            If escapeKeyDown Then Return
            escapeKeyDown = True

            audio.PlayFullScreenSound()
            window.ToggleFullScreen()
            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 3. Quit Game (Ctrl + Q)
        ' ============================================================
        If e.Control AndAlso e.KeyCode = Keys.Q Then

            If ctrlQDown Then Return
            ctrlQDown = True

            Form1.QuitGame()
            Return
        End If


        ' ============================================================
        ' 4. Keyboard Hints Toggle (Ctrl + H)
        ' ============================================================
        If e.Control AndAlso e.KeyCode = Keys.H Then

            If ctrlHDown Then Return
            ctrlHDown = True

            ToggleShowKeyboardHints(model)

            form.Invalidate()

            Return
        End If


        ' ============================================================
        ' 5. State‑based Input Dispatch
        ' ============================================================
        Select Case model.State

            Case GameState.StartScreen
                HandleStartScreen_KeyDown(e, form, audio, renderer, match, window, physics, model)
                Return

            Case GameState.EndScreen
                HandleEndScreen_KeyDown(e, form, audio, model)
                Return

            Case GameState.Playing
                HandleGameplay_KeyDown(e, form, audio, match, window, physics, model)
                Return

            Case GameState.Pause
                HandlePause_KeyDown(e, form, audio, window, physics, renderer, match, model)
                Return

            Case GameState.AIDifficulty
                HandleAIDifficulty_KeyDown(e, form, audio, match, window, model, physics, renderer)
                Return

        End Select

    End Sub

    Public Sub ToggleShowKeyboardHints(model As GameModel)

        model.ShowKeyboardHints = Not model.ShowKeyboardHints

    End Sub

    Public Sub OnKeyUp(e As KeyEventArgs,
                       model As GameModel)

        ' ============================================================
        ' 1. Release Paddle Movement Keys
        ' ============================================================
        If e.KeyCode = Keys.W Then
            model.LeftPaddleUp = False
            wKeyDown = False
        End If

        If e.KeyCode = Keys.S Then
            model.LeftPaddleDown = False
            sKeyDown = False
        End If

        If model.PlayerMode = NumberOfPlayers.TwoPlayers Then
            If e.KeyCode = Keys.Up Then
                model.RightPaddleUp = False
                upKeyDown = False
            End If

            If e.KeyCode = Keys.Down Then
                model.RightPaddleDown = False
                downKeyDown = False
            End If
        End If


        ' ============================================================
        ' 2. Release Pause / Resume Keys
        ' ============================================================
        If e.KeyCode = Keys.P Then pKeyDown = False
        If e.KeyCode = Keys.Pause Then pauseKeyDown = False
        If e.KeyCode = Keys.MediaPlayPause Then mediaPlayPauseKeyDown = False


        ' ============================================================
        ' 3. Release Fullscreen Toggle Keys
        ' ============================================================
        If e.KeyCode = Keys.F11 Then f11KeyDown = False
        If e.KeyCode = Keys.F Then fKeyDown = False


        ' ============================================================
        ' 4. Release Escape Key
        ' ============================================================
        If e.KeyCode = Keys.Escape Then escapeKeyDown = False


        ' ============================================================
        ' 5. Release Confirm Keys (Enter / Space)
        ' ============================================================
        If e.KeyCode = Keys.Enter Then enterKeyDown = False
        If e.KeyCode = Keys.Space Then spaceKeyDown = False


        ' ============================================================
        ' 6. Release Menu Navigation Keys (Up / Down / W / S)
        ' ============================================================
        If e.KeyCode = Keys.Up Then upKeyDown = False
        If e.KeyCode = Keys.Down Then downKeyDown = False

        If e.KeyCode = Keys.W Then wKeyDown = False
        If e.KeyCode = Keys.S Then sKeyDown = False


        ' ============================================================
        ' 7. Release Quit Game Key (Ctrl + Q)
        ' ============================================================
        If e.KeyCode = Keys.Q Then ctrlQDown = False
        If e.KeyCode = Keys.ControlKey Then ctrlQDown = False


        ' ============================================================
        ' 8. Release Keyboard Hints Key (Ctrl + H)
        ' ============================================================
        If e.KeyCode = Keys.H Then ctrlHDown = False
        If e.KeyCode = Keys.ControlKey Then ctrlHDown = False


    End Sub


    Public Sub OnMouseClick(e As MouseEventArgs,
                            form As Form,
                            match As MatchManager,
                            audio As AudioController,
                            renderer As Rendering,
                            window As WindowManager,
                            physics As PhysicsEngine,
                            model As GameModel)

        HandleAIDifficulty_MouseClick(e, form, audio, match, window, model, physics, renderer)
        HandleStartScreen_MouseClick(e, form, audio, renderer, match, window, physics, model)
        HandlePauseMenu_MouseClick(e, form, audio, window, physics, renderer, match, model)
        HandleEndScreen_MouseClick(e, form, audio, model)

    End Sub

    Public Sub OnMouseUp()

        mouseIsClicking = False

    End Sub

    Public Sub OnMouseMove(e As MouseEventArgs,
                           renderer As Rendering,
                           form As Form,
                           audio As AudioController,
                           model As GameModel)

        HandleStartScreen_MouseMove(e, renderer, form, audio, model)
        HandleAIDifficulty_MouseMove(e, renderer, form, audio, model)
        HandlePauseMenu_MouseMove(e, renderer, form, audio, model)

    End Sub

    Public Sub OnMouseWheel(e As MouseEventArgs,
                            form As Form,
                            audio As AudioController,
                            model As GameModel)

        If model.State = GameState.Pause Then
            HandlePauseMouseWheel(e.Delta, form, audio, model)
            Return
        End If

        If model.State = GameState.StartScreen Then
            HandleStartScreenMouseWheel(e.Delta, form, audio, model)
            Return
        End If

        If model.State = GameState.AIDifficulty Then
            HandleAIDifficultyMouseWheel(e.Delta, form, audio, model)
            Return
        End If

    End Sub

    ' ===========================
    '  KEYBOARD HANDLERS
    ' ===========================

    Private Sub HandleStartScreen_KeyDown(e As KeyEventArgs,
                                          form As Form,
                                          audio As AudioController,
                                          renderer As Rendering,
                                          match As MatchManager,
                                          window As WindowManager,
                                          physics As PhysicsEngine,
                                          model As GameModel)

        ' ============================================================
        ' 1. Menu Navigation (Up/W and Down/S)
        ' ============================================================
        Select Case e.KeyCode

        ' ------------------------------
        '  Up / W   --   ↑ Menu Up ↑
        ' ------------------------------
            Case Keys.Up
                If upKeyDown Then Return
                upKeyDown = True

                If model.PlayerSelection <> NumberOfPlayers.OnePlayer Then

                    model.PlayerSelection = NumberOfPlayers.OnePlayer

                    audio.PlayMenuUpSound()
                    form.Invalidate()
                End If
                Return

            Case Keys.W
                If wKeyDown Then Return
                wKeyDown = True

                If model.PlayerSelection <> NumberOfPlayers.OnePlayer Then

                    model.PlayerSelection = NumberOfPlayers.OnePlayer

                    audio.PlayMenuUpSound()
                    form.Invalidate()
                End If
                Return


        ' ------------------------------
        '  Down / S   --   ↓ Menu Down ↓
        ' ------------------------------
            Case Keys.Down
                If downKeyDown Then Return
                downKeyDown = True

                If model.PlayerSelection <> NumberOfPlayers.TwoPlayers Then

                    model.PlayerSelection = NumberOfPlayers.TwoPlayers

                    audio.PlayMenuDownSound()
                    form.Invalidate()
                End If
                Return

            Case Keys.S
                If sKeyDown Then Return
                sKeyDown = True

                If model.PlayerSelection <> NumberOfPlayers.TwoPlayers Then

                    model.PlayerSelection = NumberOfPlayers.TwoPlayers

                    audio.PlayMenuDownSound()
                    form.Invalidate()
                End If
                Return


        ' ============================================================
        ' 2. Direct Selection via Number Keys (1 or 2)
        ' ============================================================
            Case Keys.D1, Keys.NumPad1

                model.PlayerSelection = NumberOfPlayers.OnePlayer

                HandleOnePlayerSelection(model, renderer, form)

                audio.PlaySelectSound()
                form.Invalidate()
                Return

            Case Keys.D2, Keys.NumPad2

                model.PlayerSelection = NumberOfPlayers.TwoPlayers

                HandleTwoPlayerSelection(model, renderer, form, match, audio, window, physics)

                audio.PlaySelectSound()
                form.Invalidate()
                Return


        ' ============================================================
        ' 3. Confirm Selection (Space / Enter)
        ' ============================================================
            Case Keys.Space
                If spaceKeyDown Then Return
                spaceKeyDown = True

                If model.PlayerSelection = NumberOfPlayers.OnePlayer Then

                    HandleOnePlayerSelection(model, renderer, form)

                Else

                    HandleTwoPlayerSelection(model, renderer, form, match, audio, window, physics)

                End If

                audio.PlaySelectSound()
                form.Invalidate()

                Return

            Case Keys.Enter

                If enterKeyDown Then Return
                enterKeyDown = True

                If model.PlayerSelection = NumberOfPlayers.OnePlayer Then

                    HandleOnePlayerSelection(model, renderer, form)

                Else

                    HandleTwoPlayerSelection(model, renderer, form, match, audio, window, physics)

                End If

                audio.PlaySelectSound()
                form.Invalidate()

                Return

        ' ============================================================
        ' 4. Escape (Exit Game)
        ' ============================================================
            Case Keys.Escape
                If escapeKeyDown Then Return
                escapeKeyDown = True

                Form1.QuitGame()
                Return

        End Select

    End Sub

    Private Sub HandleOnePlayerSelection(model As GameModel,
                                         renderer As Rendering,
                                         form As Form)

        model.PlayerMode = NumberOfPlayers.OnePlayer
        renderer.CacheKeyboardHints(form, model)
        model.State = GameState.AIDifficulty

    End Sub

    Private Sub HandleTwoPlayerSelection(model As GameModel,
                                         renderer As Rendering,
                                         form As Form,
                                         match As MatchManager,
                                         audio As AudioController,
                                         window As WindowManager,
                                         physics As PhysicsEngine)

        model.PlayerMode = NumberOfPlayers.TwoPlayers
        renderer.CacheKeyboardHints(form, model)
        match.StartNewMatch(audio, window, model, physics, renderer, form)

    End Sub

    Private Sub HandleAIDifficulty_KeyDown(e As KeyEventArgs,
                                           form As Form,
                                           audio As AudioController,
                                           match As MatchManager,
                                           window As WindowManager,
                                           model As GameModel,
                                           physics As PhysicsEngine,
                                           renderer As Rendering)

        ' -------------------------------
        '  AI Difficulty Menu Navigation
        ' -------------------------------

        ' Menu Up (Arrow Up or W)
        If (e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.W) Then

            If (e.KeyCode = Keys.Up AndAlso upKeyDown) OrElse
               (e.KeyCode = Keys.W AndAlso wKeyDown) Then Return

            If e.KeyCode = Keys.Up Then upKeyDown = True Else wKeyDown = True

            If model.AISelection > AIDifficultyLevel.Easy Then
                model.AISelection -= 1
                renderer.SetAIDifficultySelection(model.AISelection)

                audio.PlayMenuUpSound()
                form.Invalidate()
            End If

            Return

        End If

        ' Menu Down (Arrow Down or S)
        If (e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.S) Then

            If (e.KeyCode = Keys.Down AndAlso downKeyDown) OrElse
               (e.KeyCode = Keys.S AndAlso sKeyDown) Then Return

            If e.KeyCode = Keys.Down Then downKeyDown = True Else sKeyDown = True

            If model.AISelection < AIDifficultyLevel.Hard Then
                model.AISelection += 1
                renderer.SetAIDifficultySelection(model.AISelection)

                audio.PlayMenuDownSound()
                form.Invalidate()
            End If

            Return

        End If


        ' ============================
        '   DIRECT SELECT: E / N / H / 1 / 2 / 3
        ' ============================

        Dim directSelectDifficulty As Nullable(Of AIDifficultyLevel) = Nothing

        Select Case e.KeyCode
            Case Keys.E, Keys.D1, Keys.NumPad1
                directSelectDifficulty = AIDifficultyLevel.Easy

            Case Keys.N, Keys.D2, Keys.NumPad2
                directSelectDifficulty = AIDifficultyLevel.Normal

            Case Keys.H, Keys.D3, Keys.NumPad3
                directSelectDifficulty = AIDifficultyLevel.Hard
        End Select

        If directSelectDifficulty.HasValue Then

            model.AISelection = directSelectDifficulty.Value

            ' Update AI mode factor
            SetAIModeFactor(model)

            ' Start match immediately
            match.StartNewMatch(audio, window, model, physics, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()

            Return
        End If


        ' ============================
        '   SELECT: SPACE / ENTER
        ' ============================
        If (e.KeyCode = Keys.Space AndAlso Not spaceKeyDown) OrElse
           (e.KeyCode = Keys.Enter AndAlso Not enterKeyDown) Then
            If e.KeyCode = Keys.Space Then
                spaceKeyDown = True
            Else
                enterKeyDown = True
            End If

            SetAIModeFactor(model)

            match.StartNewMatch(audio, window, model, physics, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()

            Return

        End If

        ' ============================
        '   ESCAPE → RETURN TO START
        ' ============================

        If e.KeyCode = Keys.Escape AndAlso Not escapeKeyDown Then
            escapeKeyDown = True

            model.State = GameState.StartScreen
            audio.PlaySelectSound()
            form.Invalidate()

            Return

        End If

    End Sub

    Private Sub HandleGameplay_KeyDown(e As KeyEventArgs,
                                    form As Form,
                                    audio As AudioController,
                                    match As MatchManager,
                                    window As WindowManager,
                                    physics As PhysicsEngine,
                                    model As GameModel)

        ' ============================================================
        ' 1. Paddle Movement (Left Player: W/S)
        ' ============================================================
        If e.KeyCode = Keys.W Then
            model.LeftPaddleUp = True
        End If

        If e.KeyCode = Keys.S Then
            model.LeftPaddleDown = True
        End If


        ' ============================================================
        ' 2. Paddle Movement (Right Player: Up/Down in 2‑Player mode)
        ' ============================================================
        If model.PlayerMode = NumberOfPlayers.TwoPlayers Then
            If e.KeyCode = Keys.Up Then
                model.RightPaddleUp = True
            End If

            If e.KeyCode = Keys.Down Then
                model.RightPaddleDown = True
            End If
        End If


        ' ============================================================
        ' 3. Pause Game (P, Pause/Break, MediaPlayPause)
        ' ============================================================
        If e.KeyCode = Keys.P Then

            If pKeyDown Then Return
            pKeyDown = True

            match.PauseGame(audio, window, physics, model)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Pause Then

            If pauseKeyDown Then Return
            pauseKeyDown = True

            match.PauseGame(audio, window, physics, model)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.MediaPlayPause Then

            If mediaPlayPauseKeyDown Then Return
            mediaPlayPauseKeyDown = True

            match.PauseGame(audio, window, physics, model)
            audio.PlaySelectSound()


            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 4. Escape (Pause only when windowed)
        ' ============================================================
        If form.FormBorderStyle <> FormBorderStyle.None AndAlso
           e.KeyCode = Keys.Escape Then

            If escapeKeyDown Then Return
            escapeKeyDown = True

            match.PauseGame(audio, window, physics, model)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If


    End Sub

    Private Sub HandlePause_KeyDown(e As KeyEventArgs,
                                    form As Form,
                                    audio As AudioController,
                                    window As WindowManager,
                                    physics As PhysicsEngine,
                                    renderer As Rendering,
                                    match As MatchManager,
                                    model As GameModel)

        ' ============================================================
        ' 1. Resume Game (P, Pause/Break, MediaPlayPause)
        ' ============================================================
        If e.KeyCode = Keys.P Then
            If pKeyDown Then Return
            pKeyDown = True

            match.ResumeGame(audio, window, physics, model)

            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Pause Then
            If pauseKeyDown Then Return
            pauseKeyDown = True

            match.ResumeGame(audio, window, physics, model)

            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.MediaPlayPause Then

            If mediaPlayPauseKeyDown Then Return
            mediaPlayPauseKeyDown = True

            match.ResumeGame(audio, window, physics, model)

            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 2. Pause Menu Navigation (Up/W and Down/S)
        ' ============================================================
        If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.W Then

            If model.PauseSelection > PauseMenu.ResumeMatch Then

                model.PauseSelection -= 1

                audio.PlayMenuUpSound()
                form.Invalidate()
            End If
            Return

        End If


        If e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.S Then

            If model.PauseSelection < PauseMenu.QuitMatch Then

                model.PauseSelection += 1

                audio.PlayMenuDownSound()
                form.Invalidate()
            End If
            Return
        End If



        ' ============================================================
        ' 3. Direct Hotkeys (R = Resume, N = New Match, Q = Quit)
        ' ============================================================
        If e.KeyCode = Keys.R Then

            match.ResumeGame(audio, window, physics, model)

            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.N Then

            match.StartNewMatch(audio, window, model, physics, renderer, form)

            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Q Then

            match.Quit2StartScreen(audio, physics, model, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 4. Escape Key (Quit to Start Screen)
        ' ============================================================
        If e.KeyCode = Keys.Escape Then
            If escapeKeyDown Then Return
            escapeKeyDown = True

            match.Quit2StartScreen(audio, physics, model, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 5. Confirm Selection (Enter / Space)
        ' ============================================================
        If e.KeyCode = Keys.Enter Then
            If enterKeyDown Then Return
            enterKeyDown = True

            Select Case model.PauseSelection
                Case PauseMenu.ResumeMatch : match.ResumeGame(audio, window, physics, model)
                Case PauseMenu.NewMatch : match.StartNewMatch(audio, window, model, physics, renderer, form)
                Case PauseMenu.QuitMatch : match.Quit2StartScreen(audio, physics, model, renderer, form)
            End Select


            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Space Then
            If spaceKeyDown Then Return
            spaceKeyDown = True

            Select Case model.PauseSelection
                Case PauseMenu.ResumeMatch : match.ResumeGame(audio, window, physics, model)
                Case PauseMenu.NewMatch : match.StartNewMatch(audio, window, model, physics, renderer, form)
                Case PauseMenu.QuitMatch : match.Quit2StartScreen(audio, physics, model, renderer, form)
            End Select

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

    End Sub

    Private Sub HandleEndScreen_KeyDown(e As KeyEventArgs,
                                        form As Form,
                                        audio As AudioController,
                                        model As GameModel)

        ' ============================================================
        ' 1. Return to Start Screen (Space)
        ' ============================================================
        If e.KeyCode = Keys.Space Then
            If spaceKeyDown Then Return
            spaceKeyDown = True

            audio.PlaySelectSound()
            model.State = GameState.StartScreen
            model.WinnerText = String.Empty

            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 2. Return to Start Screen (Enter)
        ' ============================================================
        If e.KeyCode = Keys.Enter Then
            If enterKeyDown Then Return
            enterKeyDown = True

            audio.PlaySelectSound()
            model.State = GameState.StartScreen
            model.WinnerText = String.Empty

            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 3. Return to Start Screen (Escape)
        ' ============================================================
        If e.KeyCode = Keys.Escape Then
            If escapeKeyDown Then Return
            escapeKeyDown = True

            audio.PlaySelectSound()
            model.State = GameState.StartScreen
            model.WinnerText = String.Empty

            form.Invalidate()
            Return
        End If

    End Sub

    ' ===========================
    '  MOUSE CLICK HANDLERS
    ' ===========================

    Private Sub HandleStartScreen_MouseClick(e As MouseEventArgs,
                                             form As Form,
                                             audio As AudioController,
                                             renderer As Rendering,
                                             match As MatchManager,
                                             window As WindowManager,
                                             physics As PhysicsEngine,
                                             model As GameModel)

        If model.State <> GameState.StartScreen Then Return

        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' Ignore middle-click logic unless needed`
        If e.Button = MouseButtons.Middle Then
            If model.PlayerSelection = NumberOfPlayers.OnePlayer Then

                HandleOnePlayerSelection(model, renderer, form)

            Else

                HandleTwoPlayerSelection(model, renderer, form, match, audio, window, physics)

            End If

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' One Player
        If renderer.OnePlayerOptionRect.Contains(e.Location) Then

            model.PlayerSelection = NumberOfPlayers.OnePlayer   ' ← update selection

            HandleOnePlayerSelection(model, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' Two Players
        If renderer.TwoPlayersOptionRect.Contains(e.Location) Then

            model.PlayerSelection = NumberOfPlayers.TwoPlayers   ' ← update selection

            HandleTwoPlayerSelection(model, renderer, form, match, audio, window, physics)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If


    End Sub

    Private Sub HandleAIDifficulty_MouseClick(e As MouseEventArgs,
                                              form As Form,
                                              audio As AudioController,
                                              match As MatchManager,
                                              window As WindowManager,
                                              model As GameModel,
                                              physics As PhysicsEngine,
                                              renderer As Rendering)

        If model.State <> GameState.AIDifficulty Then Return

        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' ------------------------------
        ' Middle-click = activate current selection
        ' ------------------------------
        If e.Button = MouseButtons.Middle Then

            ActivateAIDifficulty(model.AISelection, match, audio, window, model, physics, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' Easy
        If renderer.EasyRect.Contains(e.Location) Then
            model.AISelection = AIDifficultyLevel.Easy

            ActivateAIDifficulty(model.AISelection, match, audio, window, model, physics, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' Normal
        If renderer.NormalRect.Contains(e.Location) Then
            model.AISelection = AIDifficultyLevel.Normal

            ActivateAIDifficulty(model.AISelection, match, audio, window, model, physics, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' Hard
        If renderer.HardRect.Contains(e.Location) Then
            model.AISelection = AIDifficultyLevel.Hard
            ActivateAIDifficulty(model.AISelection, match, audio, window, model, physics, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

    End Sub

    Private Sub HandlePauseMenu_MouseClick(e As MouseEventArgs,
                                           form As Form,
                                           audio As AudioController,
                                           window As WindowManager,
                                           physics As PhysicsEngine,
                                           renderer As Rendering,
                                           match As MatchManager,
                                           model As GameModel)

        If model.State <> GameState.Pause Then Return

        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' ------------------------------
        ' Middle-click = activate current selection
        ' ------------------------------
        If e.Button = MouseButtons.Middle Then


            Select Case model.PauseSelection
                Case PauseMenu.ResumeMatch : match.ResumeGame(audio, window, physics, model)
                Case PauseMenu.NewMatch : match.StartNewMatch(audio, window, model, physics, renderer, form)
                Case PauseMenu.QuitMatch : match.Quit2StartScreen(audio, physics, model, renderer, form)
            End Select

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' Resume
        If renderer.ResumeRect.Contains(e.Location) Then

            model.PauseSelection = PauseMenu.ResumeMatch

            match.ResumeGame(audio, window, physics, model)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' New Match
        If renderer.NewMatchRect.Contains(e.Location) Then

            model.PauseSelection = PauseMenu.NewMatch

            match.StartNewMatch(audio, window, model, physics, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' Quit
        If renderer.QuitRect.Contains(e.Location) Then

            model.PauseSelection = PauseMenu.QuitMatch

            match.Quit2StartScreen(audio, physics, model, renderer, form)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

    End Sub

    Private Sub HandleEndScreen_MouseClick(e As MouseEventArgs,
                                           form As Form,
                                           audio As AudioController,
                                           model As GameModel)

        If model.State <> GameState.EndScreen Then Return

        audio.PlaySelectSound()
        model.State = GameState.StartScreen
        model.WinnerText = String.Empty

        form.Invalidate()

    End Sub

    ' ===========================
    '  MOUSE MOVE HANDLERS
    ' ===========================

    Private Sub HandleStartScreen_MouseMove(e As MouseEventArgs,
                                            renderer As Rendering,
                                            form As Form,
                                            audio As AudioController,
                                            model As GameModel)

        If model.State <> GameState.StartScreen Then Return

        Dim oldSelection = model.PlayerSelection

        If renderer.OnePlayerOptionRect.Contains(e.Location) Then
            model.PlayerSelection = NumberOfPlayers.OnePlayer

        ElseIf renderer.TwoPlayersOptionRect.Contains(e.Location) Then
            model.PlayerSelection = NumberOfPlayers.TwoPlayers

        End If

        If oldSelection <> model.PlayerSelection Then
            audio.PlayMenuMoveSound()

            form.Invalidate()
        End If

    End Sub

    Private Sub HandleAIDifficulty_MouseMove(e As MouseEventArgs,
                                             renderer As Rendering,
                                             form As Form,
                                             audio As AudioController,
                                             model As GameModel)

        If model.State <> GameState.AIDifficulty Then Return

        Dim oldSelection = model.AISelection


        If renderer.EasyRect.Contains(e.Location) Then
            model.AISelection = AIDifficultyLevel.Easy
        ElseIf renderer.NormalRect.Contains(e.Location) Then
            model.AISelection = AIDifficultyLevel.Normal

        ElseIf renderer.HardRect.Contains(e.Location) Then
            model.AISelection = AIDifficultyLevel.Hard

        End If


        If oldSelection <> model.AISelection Then
            audio.PlayMenuMoveSound()

            form.Invalidate()
        End If
    End Sub

    Private Sub HandlePauseMenu_MouseMove(e As MouseEventArgs,
                                          renderer As Rendering,
                                          form As Form,
                                          audio As AudioController,
                                          model As GameModel)

        If model.State <> GameState.Pause Then Return

        Dim oldSelection = model.PauseSelection

        If renderer.ResumeRect.Contains(e.Location) Then
            model.PauseSelection = PauseMenu.ResumeMatch
        ElseIf renderer.NewMatchRect.Contains(e.Location) Then
            model.PauseSelection = PauseMenu.NewMatch
        ElseIf renderer.QuitRect.Contains(e.Location) Then
            model.PauseSelection = PauseMenu.QuitMatch
        End If

        If oldSelection <> model.PauseSelection Then
            audio.PlayMenuMoveSound()
            form.Invalidate()
        End If
    End Sub

    ' ===========================
    '  MOUSE WHEEL HANDLERS
    ' ===========================

    Private Sub HandleStartScreenMouseWheel(delta As Integer,
                                            form As Form,
                                            audio As AudioController,
                                            model As GameModel)

        ' Accumulate wheel movement
        startScreenScrollAccum += delta

        Dim numberOfPlayersSelection = model.PlayerSelection

        ' Scroll up enough → select One Player
        If startScreenScrollAccum >= ScrollThreshold Then


            If numberOfPlayersSelection <> NumberOfPlayers.OnePlayer Then
                numberOfPlayersSelection = NumberOfPlayers.OnePlayer
                model.PlayerSelection = numberOfPlayersSelection

                audio.PlayMenuUpSound()
                form.Invalidate()
            End If

            startScreenScrollAccum = 0
            Return
        End If

        ' Scroll down enough → select Two Players
        If startScreenScrollAccum <= -ScrollThreshold Then


            If numberOfPlayersSelection <> NumberOfPlayers.TwoPlayers Then
                numberOfPlayersSelection = NumberOfPlayers.TwoPlayers
                model.PlayerSelection = numberOfPlayersSelection


                audio.PlayMenuDownSound()
                form.Invalidate()
            End If

            startScreenScrollAccum = 0
            Return
        End If

    End Sub

    Private Sub HandleAIDifficultyMouseWheel(delta As Integer,
                                             form As Form,
                                             audio As AudioController,
                                             model As GameModel)

        ' Accumulate wheel movement
        aiDifficultyScrollAccum += delta

        Dim aiDifficultySelection = model.AISelection

        ' ============================
        ' Scroll Up → Move Selection Up
        ' ============================
        If aiDifficultyScrollAccum >= ScrollThreshold Then

            If aiDifficultySelection > AIDifficultyLevel.Easy Then

                aiDifficultySelection -= 1

                model.AISelection = aiDifficultySelection

                audio.PlayMenuUpSound()
                form.Invalidate()
            End If

            aiDifficultyScrollAccum = 0
            Return
        End If

        ' ============================
        ' Scroll Down → Move Selection Down
        ' ============================
        If aiDifficultyScrollAccum <= -ScrollThreshold Then

            If aiDifficultySelection < AIDifficultyLevel.Hard Then
                aiDifficultySelection += 1
                model.AISelection = aiDifficultySelection


                audio.PlayMenuDownSound()
                form.Invalidate()
            End If

            aiDifficultyScrollAccum = 0
            Return
        End If

    End Sub

    Private Sub HandlePauseMouseWheel(delta As Integer,
                                      form As Form,
                                      audio As AudioController,
                                      model As GameModel)

        ' Accumulate wheel movement
        pauseScrollAccum += delta

        Dim pauseMenuSelection = model.PauseSelection


        ' ============================
        ' Scroll Up → Move Selection Up
        ' ============================
        If pauseScrollAccum >= ScrollThreshold Then

            If pauseMenuSelection > PauseMenu.ResumeMatch Then
                pauseMenuSelection -= 1
                model.PauseSelection = pauseMenuSelection

                audio.PlayMenuUpSound()
                form.Invalidate()
            End If

            pauseScrollAccum = 0
            Return
        End If

        ' ============================
        ' Scroll Down → Move Selection Down
        ' ============================
        If pauseScrollAccum <= -ScrollThreshold Then

            If pauseMenuSelection < PauseMenu.QuitMatch Then
                pauseMenuSelection += 1
                model.PauseSelection = pauseMenuSelection

                audio.PlayMenuDownSound()
                form.Invalidate()
            End If

            pauseScrollAccum = 0
            Return
        End If

    End Sub

    ' ===========================
    '  HELPER METHODS
    ' ===========================

    Private Sub ActivateAIDifficulty(level As AIDifficultyLevel,
                                     match As MatchManager,
                                     audio As AudioController,
                                     window As WindowManager,
                                     model As GameModel,
                                     physics As PhysicsEngine,
                                     renderer As Rendering,
                                     form As Form)

        model.AISelection = level
        SetAIModeFactor(model)

        match.StartNewMatch(audio, window, model, physics, renderer, form)

    End Sub

    Public Sub SetAIModeFactor(model As GameModel)
        Select Case model.AISelection()
            Case AIDifficultyLevel.Easy : model.AIModeFactor = 0.5
            Case AIDifficultyLevel.Normal : model.AIModeFactor = 0.5599
            Case AIDifficultyLevel.Hard : model.AIModeFactor = 0.5999
        End Select
    End Sub

End Class
