Imports Pong.Enums
Imports Pong.SettingsManager
Imports Pong.GameStateManager
Imports Pong.Rendering

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

    Public Sub New()
        ' Initialize any necessary variables or state here
    End Sub

    ' ===========================
    '  PUBLIC INTERFACE
    ' ===========================

    Public Sub OnKeyDown(e As KeyEventArgs,
                         form As Form,
                         settings As SettingsManager,
                         state As GameStateManager,
                         match As MatchManager,
                         audio As AudioController)

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
            Form1.ToggleFullScreen()
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
            Form1.ToggleFullScreen()
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

            'showKeyboardHints = Not showKeyboardHints
            'Form1.renderer.ToggleKeyboardHints()
            settings.toggleShowKeyboardHints()

            form.Invalidate()

            Return
        End If


        ' ============================================================
        ' 5. State‑based Input Dispatch
        ' ============================================================
        Select Case state.GetCurrentState()

            Case GameState.StartScreen
                HandleStartScreenInput(e, settings, state, form, audio)
                Return

            Case GameState.EndScreen
                HandleEndScreenInput(e, state, form, match, audio)
                Return

            Case GameState.Playing
                HandleGameplayInput(e, settings, form, audio)
                Return

            Case GameState.Pause
                HandlePauseInput(e, settings, form, audio)
                Return

            Case GameState.AIDifficulty
                HandleAIDifficultyInput(e, form, settings, state, audio)
                Return

        End Select


    End Sub

    Public Sub OnKeyUp(e As KeyEventArgs,
                       settings As SettingsManager)

        ' ============================================================
        ' 1. Release Paddle Movement Keys
        ' ============================================================
        If e.KeyCode = Keys.W Then
            Form1.moveLeftPaddleUp = False
            wKeyDown = False
        End If

        If e.KeyCode = Keys.S Then
            Form1.moveLeftPaddleDown = False
            sKeyDown = False
        End If

        If settings.GetPlayerMode() = 2 Then
            If e.KeyCode = Keys.Up Then
                Form1.moveRightPaddleUp = False
                upKeyDown = False
            End If

            If e.KeyCode = Keys.Down Then
                Form1.moveRightPaddleDown = False
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
                            settings As SettingsManager,
                            state As GameStateManager,
                            form As Form,
                            match As MatchManager,
                            audio As AudioController)

        HandleAIDifficulty_MouseClick(e, settings, state, form, audio)
        HandleStartScreen_MouseClick(e, settings, state, form, audio)
        HandlePauseMenu_MouseClick(e, settings, state, form, audio)
        HandleEndScreen_MouseClick(e, state, form, match, audio)

    End Sub

    Public Sub OnMouseUp()

        mouseIsClicking = False

    End Sub

    Public Sub OnMouseMove(e As MouseEventArgs,
                           renderer As Rendering,
                           settings As SettingsManager,
                           state As GameStateManager,
                           form As Form,
                           audio As AudioController)

        HandleStartScreen_MouseMove(e, renderer, settings, state, form, audio)
        HandleAIDifficulty_MouseMove(e, renderer, settings, state, form, audio)
        HandlePauseMenu_MouseMove(e, renderer, settings, state, form, audio)

    End Sub

    Public Sub OnMouseWheel(e As MouseEventArgs,
                            settings As SettingsManager,
                            state As GameStateManager,
                            form As Form,
                            audio As AudioController)

        If state.GetCurrentState() = GameState.Pause Then
            HandlePauseMouseWheel(e.Delta, settings, form, audio)
            Return
        End If

        If state.GetCurrentState() = GameState.StartScreen Then
            HandleStartScreenMouseWheel(e.Delta, settings, form, audio)
            Return
        End If

        If state.GetCurrentState() = GameState.AIDifficulty Then
            HandleAIDifficultyMouseWheel(e.Delta, settings, form, audio)
            Return
        End If

    End Sub

    ' ===========================
    '  KEYBOARD HANDLERS
    ' ===========================

    Private Sub HandleStartScreenInput(e As KeyEventArgs,
                                       settings As SettingsManager,
                                       state As GameStateManager,
                                       form As Form,
                                       audio As AudioController)

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

                If settings.GetNumberOfPlayersSelection() <> NumberOfPlayers.OnePlayer Then
                    settings.SetNumberOfPlayersSelection(NumberOfPlayers.OnePlayer)
                    'renderer.SetStartMenuSelection(settings.GetNumberOfPlayersSelection())

                    audio.PlayMenuUpSound()
                    form.Invalidate()
                End If
                Return

            Case Keys.W
                If wKeyDown Then Return
                wKeyDown = True

                If settings.GetNumberOfPlayersSelection() <> NumberOfPlayers.OnePlayer Then
                    settings.SetNumberOfPlayersSelection(NumberOfPlayers.OnePlayer)
                    'renderer.SetStartMenuSelection(settings.GetNumberOfPlayersSelection())

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

                If settings.GetNumberOfPlayersSelection() <> NumberOfPlayers.TwoPlayers Then
                    settings.SetNumberOfPlayersSelection(NumberOfPlayers.TwoPlayers)
                    'renderer.SetStartMenuSelection(settings.GetNumberOfPlayersSelection())

                    audio.PlayMenuDownSound()
                    form.Invalidate()
                End If
                Return

            Case Keys.S
                If sKeyDown Then Return
                sKeyDown = True

                If settings.GetNumberOfPlayersSelection() <> NumberOfPlayers.TwoPlayers Then
                    settings.SetNumberOfPlayersSelection(NumberOfPlayers.TwoPlayers)
                    'renderer.SetStartMenuSelection(settings.GetNumberOfPlayersSelection())

                    audio.PlayMenuDownSound()
                    form.Invalidate()
                End If
                Return


        ' ============================================================
        ' 2. Direct Selection via Number Keys (1 or 2)
        ' ============================================================
            Case Keys.D1, Keys.NumPad1
                settings.SetNumberOfPlayersSelection(NumberOfPlayers.OnePlayer)
                'renderer.SetStartMenuSelection(settings.GetNumberOfPlayersSelection())

                settings.SetPlayerMode(1)
                state.SetCurrentState(GameState.AIDifficulty)

                audio.PlaySelectSound()
                form.Invalidate()
                Return

            Case Keys.D2, Keys.NumPad2
                settings.SetNumberOfPlayersSelection(NumberOfPlayers.TwoPlayers)
                'renderer.SetStartMenuSelection(settings.GetNumberOfPlayersSelection())

                settings.SetPlayerMode(2)
                Form1.StartNewMatch()

                audio.PlaySelectSound()
                form.Invalidate()
                Return


        ' ============================================================
        ' 3. Confirm Selection (Space / Enter)
        ' ============================================================
            Case Keys.Space
                If spaceKeyDown Then Return
                spaceKeyDown = True

                If settings.GetNumberOfPlayersSelection() = NumberOfPlayers.OnePlayer Then
                    settings.SetPlayerMode(1)
                    state.SetCurrentState(GameState.AIDifficulty)
                Else
                    settings.SetPlayerMode(2)
                    Form1.StartNewMatch()
                End If

                audio.PlaySelectSound()
                form.Invalidate()
                Return

            Case Keys.Enter
                If enterKeyDown Then Return
                enterKeyDown = True

                If settings.GetNumberOfPlayersSelection() = NumberOfPlayers.OnePlayer Then
                    settings.SetPlayerMode(1)
                    state.SetCurrentState(GameState.AIDifficulty)
                Else
                    settings.SetPlayerMode(2)
                    Form1.StartNewMatch()
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

    Private Sub HandleAIDifficultyInput(e As KeyEventArgs,
                                        form As Form,
                                        settings As SettingsManager,
                                        state As GameStateManager,
                                        audio As AudioController)

        ' -------------------------------
        '  AI Difficulty Menu Navigation
        ' -------------------------------

        ' Menu Up (Arrow Up or W)
        If (e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.W) Then

            If (e.KeyCode = Keys.Up AndAlso upKeyDown) OrElse
               (e.KeyCode = Keys.W AndAlso wKeyDown) Then Return

            If e.KeyCode = Keys.Up Then upKeyDown = True Else wKeyDown = True

            If settings.GetAIDifficultySelection() > 0 Then
                settings.SetAIDifficultySelection(settings.GetAIDifficultySelection() - 1)
                Form1.renderer.SetAIDifficultySelection(settings.GetAIDifficultySelection())

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

            If settings.GetAIDifficultySelection() < settings.GetAIOptionsLength() - 1 Then
                settings.SetAIDifficultySelection(settings.GetAIDifficultySelection() + 1)
                Form1.renderer.SetAIDifficultySelection(settings.GetAIDifficultySelection())

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

            settings.SetAIDifficultySelection(directSelectDifficulty.Value)
            'Form1.renderer.SetAIDifficultySelection(settings.GetAIDifficultySelection())

            ' Update AI mode factor
            Form1.SetAIModeFactor()

            ' Start match immediately
            state.SetCurrentState(GameState.Playing)
            Form1.StartNewMatch()

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

            Form1.SetAIModeFactor()

            state.SetCurrentState(GameState.Playing)
            Form1.StartNewMatch()
            audio.PlaySelectSound()
            form.Invalidate()

            Return

        End If

        ' ============================
        '   ESCAPE → RETURN TO START
        ' ============================

        If e.KeyCode = Keys.Escape AndAlso Not escapeKeyDown Then
            escapeKeyDown = True

            state.SetCurrentState(GameState.StartScreen)
            audio.PlaySelectSound()
            form.Invalidate()

            Return

        End If

    End Sub

    Private Sub HandleGameplayInput(e As KeyEventArgs,
                                    settings As SettingsManager,
                                    form As Form,
                                    audio As AudioController)

        ' ============================================================
        ' 1. Paddle Movement (Left Player: W/S)
        ' ============================================================
        If e.KeyCode = Keys.W Then
            Form1.moveLeftPaddleUp = True
        End If

        If e.KeyCode = Keys.S Then
            Form1.moveLeftPaddleDown = True
        End If


        ' ============================================================
        ' 2. Paddle Movement (Right Player: Up/Down in 2‑Player mode)
        ' ============================================================
        If settings.GetPlayerMode() = 2 Then
            If e.KeyCode = Keys.Up Then
                Form1.moveRightPaddleUp = True
            End If

            If e.KeyCode = Keys.Down Then
                Form1.moveRightPaddleDown = True
            End If
        End If


        ' ============================================================
        ' 3. Pause Game (P, Pause/Break, MediaPlayPause)
        ' ============================================================
        If e.KeyCode = Keys.P Then
            If pKeyDown Then Return
            pKeyDown = True

            audio.PlaySelectSound()
            Form1.PauseGame()
            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Pause Then
            If pauseKeyDown Then Return
            pauseKeyDown = True

            audio.PlaySelectSound()
            Form1.PauseGame()
            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.MediaPlayPause Then
            If mediaPlayPauseKeyDown Then Return
            mediaPlayPauseKeyDown = True

            audio.PlaySelectSound()
            Form1.PauseGame()
            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 4. Escape (Pause only when windowed)
        ' ============================================================
        If Form1.FormBorderStyle <> FormBorderStyle.None AndAlso
           e.KeyCode = Keys.Escape Then
            If escapeKeyDown Then Return
            escapeKeyDown = True

            audio.PlaySelectSound()
            Form1.PauseGame()
            form.Invalidate()
            Return
        End If


    End Sub

    Private Sub HandlePauseInput(e As KeyEventArgs,
                                 settings As SettingsManager,
                                 form As Form,
                                 audio As AudioController)

        ' ============================================================
        ' 1. Resume Game (P, Pause/Break, MediaPlayPause)
        ' ============================================================
        If e.KeyCode = Keys.P Then
            If pKeyDown Then Return
            pKeyDown = True

            Form1.ResumeGame()
            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Pause Then
            If pauseKeyDown Then Return
            pauseKeyDown = True

            Form1.ResumeGame()
            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.MediaPlayPause Then
            If mediaPlayPauseKeyDown Then Return
            mediaPlayPauseKeyDown = True

            Form1.ResumeGame()
            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If


        ' ============================================================
        ' 2. Pause Menu Navigation (Up/W and Down/S)
        ' ============================================================
        If e.KeyCode = Keys.Up OrElse e.KeyCode = Keys.W Then
            If settings.GetPauseMenuSelection() > 0 Then
                settings.SetPauseMenuSelection(settings.GetPauseMenuSelection() - 1)
                'renderer.SetPauseMenuSelection(settings.GetPauseMenuSelection())

                audio.PlayMenuUpSound()
                form.Invalidate()
            End If
            Return
        End If

        If e.KeyCode = Keys.Down OrElse e.KeyCode = Keys.S Then
            If settings.GetPauseMenuSelection() < 2 Then
                settings.SetPauseMenuSelection(settings.GetPauseMenuSelection() + 1)
                'renderer.SetPauseMenuSelection(settings.GetPauseMenuSelection())

                audio.PlayMenuDownSound()
                form.Invalidate()
            End If
            Return
        End If


        ' ============================================================
        ' 3. Direct Hotkeys (R = Resume, N = New Match, Q = Quit)
        ' ============================================================
        If e.KeyCode = Keys.R Then
            Form1.ResumeGame()
            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.N Then
            Form1.StartNewMatch()
            audio.PlaySelectSound()

            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Q Then
            Form1.Quit2StartScreen()
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

            Form1.Quit2StartScreen()
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

            Select Case settings.GetPauseMenuSelection()
                Case 0 : Form1.ResumeGame()
                Case 1 : Form1.StartNewMatch()
                Case 2 : Form1.Quit2StartScreen()
            End Select
            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        If e.KeyCode = Keys.Space Then
            If spaceKeyDown Then Return
            spaceKeyDown = True

            Select Case settings.GetPauseMenuSelection()
                Case 0 : Form1.ResumeGame()
                Case 1 : Form1.StartNewMatch()
                Case 2 : Form1.Quit2StartScreen()
            End Select
            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

    End Sub

    Private Sub HandleEndScreenInput(e As KeyEventArgs,
                                     state As GameStateManager,
                                     form As Form,
                                     match As MatchManager,
                                     audio As AudioController)

        ' ============================================================
        ' 1. Return to Start Screen (Space)
        ' ============================================================
        If e.KeyCode = Keys.Space Then
            If spaceKeyDown Then Return
            spaceKeyDown = True

            audio.PlaySelectSound()
            state.SetCurrentState(GameState.StartScreen)
            match.SetWinnerText("")
            'renderer.SetWinnerText(match.GetWinnerText())

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
            state.SetCurrentState(GameState.StartScreen)
            match.SetWinnerText("")
            'renderer.SetWinnerText(MatchManager.GetWinnerText())

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
            state.SetCurrentState(GameState.StartScreen)
            match.SetWinnerText("")
            'renderer.SetWinnerText(MatchManager.GetWinnerText())

            form.Invalidate()
            Return
        End If

    End Sub

    ' ===========================
    '  MOUSE CLICK HANDLERS
    ' ===========================

    Private Sub HandleStartScreen_MouseClick(e As MouseEventArgs,
                                             settings As SettingsManager,
                                             state As GameStateManager,
                                             form As Form,
                                             audio As AudioController)

        If state.GetCurrentState() <> GameState.StartScreen Then Return

        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' Ignore middle-click logic unless needed
        If e.Button = MouseButtons.Middle Then
            If settings.GetNumberOfPlayersSelection = NumberOfPlayers.OnePlayer Then
                settings.SetPlayerMode(1)
                state.SetCurrentState(GameState.AIDifficulty)
            Else
                settings.SetPlayerMode(2)
                state.SetCurrentState(GameState.Playing)
                Form1.StartNewMatch()

            End If

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' One Player
        If Form1.renderer.OnePlayerOptionRect.Contains(e.Location) Then
            settings.SetNumberOfPlayersSelection(NumberOfPlayers.OnePlayer)   ' ← update selection

            settings.SetPlayerMode(1)
            state.SetCurrentState(GameState.AIDifficulty)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' Two Players
        If Form1.renderer.TwoPlayersOptionRect.Contains(e.Location) Then
            settings.SetNumberOfPlayersSelection(NumberOfPlayers.TwoPlayers)   ' ← update selection

            settings.SetPlayerMode(2)
            Form1.StartNewMatch()

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If


    End Sub

    Private Sub HandleAIDifficulty_MouseClick(e As MouseEventArgs,
                                              settings As SettingsManager,
                                              state As GameStateManager,
                                              form As Form,
                                              audio As AudioController)

        If state.GetCurrentState() <> GameState.AIDifficulty Then Return

        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' ------------------------------
        ' Middle-click = activate current selection
        ' ------------------------------
        If e.Button = MouseButtons.Middle Then
            ActivateAIDifficulty(settings.GetAIDifficultySelection, settings)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' Easy
        If Form1.renderer.EasyRect.Contains(e.Location) Then
            settings.SetAIDifficultySelection(AIDifficultyLevel.Easy)
            ActivateAIDifficulty(settings.GetAIDifficultySelection, settings)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' Normal
        If Form1.renderer.NormalRect.Contains(e.Location) Then
            settings.SetAIDifficultySelection(AIDifficultyLevel.Normal)
            ActivateAIDifficulty(settings.GetAIDifficultySelection, settings)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' Hard
        If Form1.renderer.HardRect.Contains(e.Location) Then
            settings.SetAIDifficultySelection(AIDifficultyLevel.Hard)
            ActivateAIDifficulty(settings.GetAIDifficultySelection, settings)

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

    End Sub

    Private Sub HandlePauseMenu_MouseClick(e As MouseEventArgs,
                                           settings As SettingsManager,
                                           state As GameStateManager,
                                           form As Form,
                                           audio As AudioController)

        If state.GetCurrentState() <> GameState.Pause Then Return

        If mouseIsClicking Then Return
        mouseIsClicking = True

        ' ------------------------------
        ' Middle-click = activate current selection
        ' ------------------------------
        If e.Button = MouseButtons.Middle Then
            Select Case settings.GetPauseMenuSelection()
                Case 0 : Form1.ResumeGame()
                Case 1 : Form1.StartNewMatch()
                Case 2 : Form1.Quit2StartScreen()
            End Select

            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' ------------------------------
        ' Left-click: check menu items
        ' ------------------------------

        ' Resume
        If Form1.renderer.ResumeRect.Contains(e.Location) Then
            settings.SetPauseMenuSelection(0)

            Form1.ResumeGame()
            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' New Match
        If Form1.renderer.NewMatchRect.Contains(e.Location) Then
            settings.SetPauseMenuSelection(1)

            Form1.StartNewMatch()
            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

        ' Quit
        If Form1.renderer.QuitRect.Contains(e.Location) Then
            settings.SetPauseMenuSelection(2)

            Form1.Quit2StartScreen()
            audio.PlaySelectSound()
            form.Invalidate()
            Return
        End If

    End Sub

    Private Sub HandleEndScreen_MouseClick(e As MouseEventArgs,
                                           state As GameStateManager,
                                           form As Form,
                                           match As MatchManager,
                                           audio As AudioController)

        If state.GetCurrentState() <> GameState.EndScreen Then Return

        audio.PlaySelectSound()
        state.SetCurrentState(GameState.StartScreen)
        match.SetWinnerText("")

        form.Invalidate()

    End Sub

    ' ===========================
    '  MOUSE MOVE HANDLERS
    ' ===========================

    Private Sub HandleStartScreen_MouseMove(e As MouseEventArgs,
                                            renderer As Rendering,
                                            settings As SettingsManager,
                                            state As GameStateManager,
                                            form As Form,
                                            audio As AudioController)

        If state.GetCurrentState <> GameState.StartScreen Then Return

        Dim oldSelection = settings.GetNumberOfPlayersSelection

        If renderer.OnePlayerOptionRect.Contains(e.Location) Then
            settings.SetNumberOfPlayersSelection(NumberOfPlayers.OnePlayer)

        ElseIf renderer.TwoPlayersOptionRect.Contains(e.Location) Then
            settings.SetNumberOfPlayersSelection(NumberOfPlayers.TwoPlayers)

        End If

        If oldSelection <> settings.GetNumberOfPlayersSelection Then
            audio.PlayMenuMoveSound()

            form.Invalidate()
        End If

    End Sub

    Private Sub HandleAIDifficulty_MouseMove(e As MouseEventArgs,
                                             renderer As Rendering,
                                             settings As SettingsManager,
                                             state As GameStateManager,
                                             form As Form,
                                             audio As AudioController)

        If state.GetCurrentState <> GameState.AIDifficulty Then Return

        Dim oldSelection = settings.GetAIDifficultySelection()


        If renderer.EasyRect.Contains(e.Location) Then
            settings.SetAIDifficultySelection(AIDifficultyLevel.Easy)
        ElseIf renderer.NormalRect.Contains(e.Location) Then
            settings.SetAIDifficultySelection(AIDifficultyLevel.Normal)

        ElseIf renderer.HardRect.Contains(e.Location) Then
            settings.SetAIDifficultySelection(AIDifficultyLevel.Hard)

        End If


        If oldSelection <> settings.GetAIDifficultySelection() Then
            audio.PlayMenuMoveSound()

            form.Invalidate()
        End If
    End Sub

    Private Sub HandlePauseMenu_MouseMove(e As MouseEventArgs,
                                          renderer As Rendering,
                                          settings As SettingsManager,
                                          state As GameStateManager,
                                          form As Form,
                                          audio As AudioController)

        If state.GetCurrentState() <> GameState.Pause Then Return

        Dim oldSelection = settings.GetPauseMenuSelection()

        If renderer.ResumeRect.Contains(e.Location) Then
            settings.SetPauseMenuSelection(0)

        ElseIf renderer.NewMatchRect.Contains(e.Location) Then
            settings.SetPauseMenuSelection(1)

        ElseIf renderer.QuitRect.Contains(e.Location) Then
            settings.SetPauseMenuSelection(2)
        End If


        If oldSelection <> settings.GetPauseMenuSelection() Then
            audio.PlayMenuMoveSound()

            form.Invalidate()
        End If
    End Sub

    ' ===========================
    '  MOUSE WHEEL HANDLERS
    ' ===========================

    Private Sub HandleStartScreenMouseWheel(delta As Integer,
                                            settings As SettingsManager,
                                            form As Form,
                                            audio As AudioController)

        ' Accumulate wheel movement
        startScreenScrollAccum += delta

        Dim numberOfPlayersSelection = settings.GetNumberOfPlayersSelection

        ' Scroll up enough → select One Player
        If startScreenScrollAccum >= ScrollThreshold Then


            If numberOfPlayersSelection <> NumberOfPlayers.OnePlayer Then
                numberOfPlayersSelection = NumberOfPlayers.OnePlayer
                settings.SetNumberOfPlayersSelection(numberOfPlayersSelection)

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
                settings.SetNumberOfPlayersSelection(numberOfPlayersSelection)


                audio.PlayMenuDownSound()
                form.Invalidate()
            End If

            startScreenScrollAccum = 0
            Return
        End If

    End Sub

    Private Sub HandleAIDifficultyMouseWheel(delta As Integer,
                                             settings As SettingsManager,
                                             form As Form,
                                             audio As AudioController)

        ' Accumulate wheel movement
        aiDifficultyScrollAccum += delta

        Dim aiDifficultySelection = settings.GetAIDifficultySelection

        ' ============================
        ' Scroll Up → Move Selection Up
        ' ============================
        If aiDifficultyScrollAccum >= ScrollThreshold Then

            If aiDifficultySelection > AIDifficultyLevel.Easy Then
                aiDifficultySelection -= 1
                settings.SetAIDifficultySelection(aiDifficultySelection)

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
                settings.SetAIDifficultySelection(aiDifficultySelection)


                audio.PlayMenuDownSound()
                form.Invalidate()
            End If

            aiDifficultyScrollAccum = 0
            Return
        End If

    End Sub

    Private Sub HandlePauseMouseWheel(delta As Integer,
                                      settings As SettingsManager,
                                      form As Form,
                                      audio As AudioController)

        ' Accumulate wheel movement
        pauseScrollAccum += delta

        Dim pauseMenuSelection = settings.GetPauseMenuSelection


        ' ============================
        ' Scroll Up → Move Selection Up
        ' ============================
        If pauseScrollAccum >= ScrollThreshold Then

            If pauseMenuSelection > 0 Then
                pauseMenuSelection -= 1
                settings.SetPauseMenuSelection(pauseMenuSelection)

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

            If pauseMenuSelection < 2 Then
                pauseMenuSelection += 1
                settings.SetPauseMenuSelection(pauseMenuSelection)

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
                                     settings As SettingsManager)

        settings.SetAIDifficultySelection(level)

        Form1.SetAIModeFactor()
        'state.SetCurrentState(GameState.Playing)

        Form1.StartNewMatch()
    End Sub


End Class
