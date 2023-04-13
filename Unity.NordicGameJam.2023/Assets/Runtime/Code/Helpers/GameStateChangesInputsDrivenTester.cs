using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateChangesInputsDrivenTester: MonoBehaviour
{
    public InputAction StartNewGame;
    public InputAction Pause;
    public InputAction Resume;
    public InputAction GameOver;
    public InputAction Win;
    public InputAction Quit;
    public InputAction Exit;

    private GameController _game;

    private void Awake()
    {
        _game = GameController.Instance;

        StartNewGame.performed += ctx => _game.StartGame();
        Pause.performed += ctx => _game.Pause();
        Resume.performed += ctx => _game.Resume();
        GameOver.performed += ctx => _game.GameOver();
        Win.performed += ctx => _game.Win();
        Quit.performed += ctx => _game.Quit();
        Exit.performed += ctx => _game.Exit();

        _game.onGameStart += () => Debug.Log("New game started");
        _game.onPause += () => Debug.Log("Game paused");
        _game.onResume += () => Debug.Log("Game resumed");
        _game.onGameOver += () => Debug.Log("Game Over");
        _game.onGameWinning += () => Debug.Log("You won!");
        _game.onGameQuit += () => Debug.Log("Back to main menu");
        _game.onExit += () => Debug.Log("EXIT");
    }

    public void OnEnable()
    {
        StartNewGame.Enable();
        Pause.Enable();
        Resume.Enable();
        GameOver.Enable();
        Win.Enable();
        Quit.Enable();
        Exit.Enable();
    }

    public void OnDisable()
    {
        StartNewGame.Disable();
        Pause.Disable();
        Resume.Disable();
        GameOver.Disable();
        Win.Disable();
        Quit.Disable();
        Exit.Disable();
    }
}
