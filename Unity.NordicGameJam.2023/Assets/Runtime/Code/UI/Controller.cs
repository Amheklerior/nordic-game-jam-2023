using UnityEngine;
using UnityEngine.UIElements;

// TODO: refactor:
// Separate logic into separate components or at least encapsulate it into readable functions
public class Controller : MonoBehaviour
{
    private UIDocument _doc;

    // Main screens
    private VisualElement _mainMenuScreen;
    private VisualElement _pauseMenuScreen;
    private VisualElement _gameOverScreen;
    private VisualElement _victoryScreen;

    // Main Menu Buttons
    private Button _playBtn;
    private Button _exitBtn;

    // Pause Screen Buttons 
    private Button _resumeBtn;
    private Button _restartBtn;
    private Button _quitBtn;

    // Game Over Screen Buttons
    private Button _tryAgainBtn;
    private Button _quitAsLooserBtn;

    // Winning Screen Buttons
    private Button _playAgainBtn;
    private Button _quitAsWinnerBtn;

    private void Awake()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;
        var game = Game.Instance;
            
        // Grab refs to the main screens 
        _mainMenuScreen = root.Q<VisualElement>("GameMenuUI");
        _pauseMenuScreen = root.Q<VisualElement>("PauseUI");
        _gameOverScreen = root.Q<VisualElement>("GameOverUI");
        _victoryScreen = root.Q<VisualElement>("WinUI");

        // grab refs to the ui buttons 
        _playBtn = root.Q<Button>("play-btn");
        _exitBtn = root.Q<Button>("exit-btn");
        _resumeBtn = root.Q<Button>("resume-btn");
        _restartBtn = root.Q<Button>("restart-btn");
        _quitBtn = root.Q<Button>("quit-btn");
        _tryAgainBtn = root.Q<Button>("try-again-btn");
        _quitAsLooserBtn = root.Q<Button>("quit-as-looser-btn");
        _playAgainBtn = root.Q<Button>("play-again-btn");
        _quitAsWinnerBtn = root.Q<Button>("quit-as-winner-btn");

        // wire the buttons' interaction logic
        _playBtn.clicked += () => game.StartGame();
        _exitBtn.clicked += () => game.Exit();
        _resumeBtn.clicked += () => game.Resume();
        _restartBtn.clicked += () => game.StartGame();
        _quitBtn.clicked += () => game.Quit();
        _tryAgainBtn.clicked += () => game.StartGame();
        _quitAsLooserBtn.clicked += () => game.Quit();
        _playAgainBtn.clicked += () => game.StartGame();
        _quitAsWinnerBtn.clicked += () => game.Quit();

        // Integrate to react to game state changes
        game.onGameStart += () =>
        {
            _mainMenuScreen.visible = false;
            _pauseMenuScreen.visible = false;
            _gameOverScreen.visible = false;
            _victoryScreen.visible = false;
        };
        game.onPause += () => _pauseMenuScreen.visible = true;
        game.onResume += () => _pauseMenuScreen.visible = false;
        game.onGameOver += () => _gameOverScreen.visible = true;
        game.onGameWinning += () => _victoryScreen.visible = true;
        game.onGameQuit += () =>
        {
            _pauseMenuScreen.visible = false;
            _gameOverScreen.visible = false;
            _victoryScreen.visible = false;
            _mainMenuScreen.visible = true;
        };
    }        

}
