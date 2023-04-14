using UnityEngine;
using UnityEngine.UIElements;
using Core;

namespace NordicGameJam2023.UI
{
    public class Controller : MonoBehaviour
    {

        private void Awake()
        {
            GeUIElementRefs();

            var game = GameController.Instance;
            WireInteractionLogicToUIElements(game);
            WireReactionToGameStateChanges(game);
        }

        #region UI Element Refs

        private UIDocument _doc;

        // Main Menu Screen
        private VisualElement _mainMenuScreen;
        private Button _playBtn;
        private Button _exitBtn;

        // Pause Menu Screen
        private VisualElement _pauseMenuScreen;
        private Button _resumeBtn;
        private Button _restartBtn;
        private Button _quitBtn;

        // Game Over Screen
        private VisualElement _gameOverScreen;
        private Button _tryAgainBtn;
        private Button _quitAsLooserBtn;

        // Victory Screen
        private VisualElement _victoryScreen;
        private Button _playAgainBtn;
        private Button _quitAsWinnerBtn;


        private void GeUIElementRefs()
        {
            _doc = GetComponent<UIDocument>();
            var root = _doc.rootVisualElement;

            _mainMenuScreen = root.Q<VisualElement>("GameMenuUI");
            _playBtn = root.Q<Button>("play-btn");
            _exitBtn = root.Q<Button>("exit-btn");

            _pauseMenuScreen = root.Q<VisualElement>("PauseUI");
            _resumeBtn = root.Q<Button>("resume-btn");
            _restartBtn = root.Q<Button>("restart-btn");
            _quitBtn = root.Q<Button>("quit-btn");

            _gameOverScreen = root.Q<VisualElement>("GameOverUI");
            _tryAgainBtn = root.Q<Button>("try-again-btn");
            _quitAsLooserBtn = root.Q<Button>("quit-as-looser-btn");

            _victoryScreen = root.Q<VisualElement>("WinUI");
            _playAgainBtn = root.Q<Button>("play-again-btn");
            _quitAsWinnerBtn = root.Q<Button>("quit-as-winner-btn");

            _currentScreen = _mainMenuScreen;
        }

        #endregion

        #region UI Interactions

        private void WireInteractionLogicToUIElements(GameController game)
        {
            _playBtn.clicked += () => game.StartGame();
            _exitBtn.clicked += () => game.Exit();
            _resumeBtn.clicked += () => game.Resume();
            _restartBtn.clicked += () => game.StartGame();
            _quitBtn.clicked += () => game.Quit();
            _tryAgainBtn.clicked += () => game.StartGame();
            _quitAsLooserBtn.clicked += () => game.Quit();
            _playAgainBtn.clicked += () => game.StartGame();
            _quitAsWinnerBtn.clicked += () => game.Quit();
        }

        #endregion

        #region Reactions To State Changes

        private void WireReactionToGameStateChanges(GameController game)
        {
            game.onGameStart += () => HideCurrentScreen();
            game.onPause += () => Show(_pauseMenuScreen);
            game.onResume += () => HideCurrentScreen();
            game.onGameOver += () => Show(_gameOverScreen);
            game.onGameWinning += () => Show(_victoryScreen);
            game.onGameQuit += () =>
            {
                HideCurrentScreen();
                Show(_mainMenuScreen);
            };
        }

        #endregion

        #region UI Controls

        private VisualElement _currentScreen = null;

        private bool NoCurrentScreen => _currentScreen == null;

        private void Show(VisualElement ui)
        {
            ui.visible = true;
            _currentScreen = ui;
        }

        private void Hide(VisualElement ui)
        {
            ui.visible = false;
            _currentScreen = null;
        }

        private void HideCurrentScreen()
        {
            if (NoCurrentScreen) return;
            Hide(_currentScreen);
        }

        #endregion

    }
}