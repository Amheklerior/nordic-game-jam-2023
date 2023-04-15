using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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

    // Waiting for Players Screen
    private VisualElement _waitingRoom;

    // Pause Menu Screen
    private VisualElement _pauseMenuScreen;
    private Button _resumeBtn;
    private Button _quitBtn;

    // Victory Screen
    private VisualElement _victoryScreen;
    private Label _winningTeamLabel;
    private Button _playAgainBtn;
    private Button _quitMatchBtn;

    // Cowntdown Screen
    private VisualElement _countdownScreen;
    private Label _counterLabel;


    private void GeUIElementRefs()
    {
        _doc = GetComponent<UIDocument>();
        var root = _doc.rootVisualElement;

        _mainMenuScreen = root.Q<VisualElement>("GameMenuUI");
        _playBtn = root.Q<Button>("play-btn");
        _exitBtn = root.Q<Button>("exit-btn");

        _waitingRoom = root.Q<VisualElement>("WaitingRoomUI");

        _pauseMenuScreen = root.Q<VisualElement>("PauseUI");
        _resumeBtn = root.Q<Button>("resume-btn");
        _quitBtn = root.Q<Button>("quit-btn");

        _victoryScreen = root.Q<VisualElement>("VictoryUI");
        _winningTeamLabel = root.Q<Label>("team-label");
        _playAgainBtn = root.Q<Button>("play-again-btn");
        _quitMatchBtn = root.Q<Button>("quit-match-btn");

        _countdownScreen = root.Q<VisualElement>("CountdownUI");
        _counterLabel = root.Q<Label>("counter");

        _currentScreen = _mainMenuScreen;
    }

    #endregion

    #region UI Interactions

    private void WireInteractionLogicToUIElements(GameController game)
    {
        _playBtn.clicked += () => game.StartGame();
        _exitBtn.clicked += () => game.Exit();
        _resumeBtn.clicked += () => game.Resume();
        _quitBtn.clicked += () => game.Quit();
        _playAgainBtn.clicked += () => game.Restart();
        _quitMatchBtn.clicked += () => game.Quit();
    }

    #endregion

    #region Reactions To State Changes

    private void WireReactionToGameStateChanges(GameController game)
    {
        game.onGameStart += () =>
        {
            HideCurrentScreen();
            Show(_waitingRoom);
        };
        game.onMatchReady += () =>
        {
            HideCurrentScreen();
            Show(_countdownScreen);
            StartCoroutine(Countdown());
        };
        game.onPause += () => Show(_pauseMenuScreen);
        game.onResume += () => HideCurrentScreen();
        game.onMatchEnd += (winningTeam) =>
        {
            _winningTeamLabel.text = winningTeam;
            Show(_victoryScreen);
        };
        game.onGameQuit += () =>
        {
            HideCurrentScreen();
            Show(_mainMenuScreen);
        };
    }

    private IEnumerator Countdown()
    {
        var counter = new string[] { "3", "2", "1" };
        foreach (var count in counter)
        {
            _counterLabel.text = count;
            yield return new WaitForSeconds(1f);
        }
        GameController.Instance.StartMatch();
        _counterLabel.text = "Go!";
        yield return new WaitForSeconds(2f);
        _counterLabel.text = "";
        HideCurrentScreen();
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