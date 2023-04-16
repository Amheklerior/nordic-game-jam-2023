using System;
using UnityEngine;

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
        AkSoundEngine.SetState("Track", "StartLevel");
    }

    public void GetReady() => onMatchReady?.Invoke();

    public void StartMatch()
    {
        if (State != GameState.WITING_FOR_PLAYERS) return;
        onMatchStart?.Invoke();
        State = GameState.PLAYING;
        startDistance = distance;
    }

    public void EndMatch(string winningTeam)
    {
        if (State != GameState.PLAYING) return;
        onMatchEnd?.Invoke(winningTeam);
        firstMusicChange = false;
        secondMusicChange = false;
        moved = false;
        distance = 9999f;
        AkSoundEngine.SetState("Track", "RaceWon");
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

    private float startDistance;
    private bool firstMusicChange = false;
    private bool secondMusicChange = false;
    private bool moved = false;
    
    private float distance = 9999f;
    public float DistanceFromTheFinishLine
    {
        get => distance;
        set
        {
            if (value <= distance) distance = value;
            if (distance <= startDistance * .98f && !moved)
            {
                moved = true;
                AkSoundEngine.SetState("Track", "FirstWormMoves");
            }
            
            if (distance <= startDistance * .66 && !firstMusicChange)
            {
                AkSoundEngine.SetState("Track", "Increase1");
                firstMusicChange = true;
            }
            
            if (distance <= startDistance * .33 && !secondMusicChange)
            {
                AkSoundEngine.SetState("Track", "Increase2");
                secondMusicChange = true;
            }
        }
    }

    #endregion
}