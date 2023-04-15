using System;

public class GameController
{
    #region Game States and transitions

    public enum GameState
    {
        MAIN_MENU,
        WITING_FOR_PLAYERS,
        PLAYING,
        PAUSED,
        MATCH_COMPLETED
    }

    public GameState State { get; private set; }

    public void StartGame()
    {
        if (State != GameState.MAIN_MENU) return;
        onGameStart?.Invoke();
        State = GameState.WITING_FOR_PLAYERS;
    }

    public void GetReady() => onMatchReady?.Invoke();

    public void StartMatch()
    {
        if (State != GameState.WITING_FOR_PLAYERS) return;
        onMatchStart?.Invoke();
        State = GameState.PLAYING;
    }

    public void EndMatch(string winningTeam)
    {
        if (State != GameState.PLAYING) return;
        onMatchEnd?.Invoke(winningTeam);
        State = GameState.MATCH_COMPLETED;
    }

    public void Pause()
    {
        if (State != GameState.PLAYING) return;
        onPause?.Invoke();
        State = GameState.PAUSED;
    }

    public void Resume()
    {
        if (State != GameState.PAUSED) return;
        onResume?.Invoke();
        State = GameState.PLAYING;
    }

    public void Restart()
    {
        if (State != GameState.MATCH_COMPLETED) return;
        onRestart?.Invoke();
        State = GameState.PLAYING;
    }

    public void Quit()
    {
        if (State != GameState.PAUSED && State != GameState.MATCH_COMPLETED) return;
        onGameQuit?.Invoke();
        State = GameState.MAIN_MENU;
    }

    public void Exit()
    {
        if (State != GameState.MAIN_MENU) return;
        onExit?.Invoke();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Game Events

    public Action onGameStart;
    public Action onMatchReady;
    public Action onMatchStart;
    public Action onPause;
    public Action onResume;
    public Action onRestart;
    public Action<string> onMatchEnd;
    public Action onGameQuit;
    public Action onExit;

    #endregion

    #region Singleton

    private static GameController _instance;

    public static GameController Instance => _instance ??= new GameController();

    #endregion

    #region

    private float distance = 9999f;
    public float DistanceFromTheFinishLine
    {
        get => distance;
        set
        {
            if (value <= distance) distance = value;
        }
    }

    #endregion
}