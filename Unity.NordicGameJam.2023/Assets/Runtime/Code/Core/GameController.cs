using System;
using UnityEngine;

namespace Amheklerior.NordicGameJam2023.Core
{
    public class GameController
    {

        #region Game States and transitions

        public enum GameState
        {
            MAIN_MENU,
            PLAYING,
            PAUSED,
            LOST,
            WON
        }

        public GameState State { get; private set; }

        public void StartGame()
        {
            if (State == GameState.PLAYING) return;
            onGameStart?.Invoke();
            State = GameState.PLAYING;
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

        public void GameOver()
        {
            if (State != GameState.PLAYING) return;
            onGameOver?.Invoke();
            State = GameState.LOST;
        }

        public void Win()
        {
            if (State != GameState.PLAYING) return;
            onGameWinning?.Invoke();
            State = GameState.WON;
        }

        public void Quit()
        {
            if (State != GameState.PAUSED &&
                State != GameState.LOST &&
                State != GameState.WON)
            {
                Debug.Log(State);
                return;
            }
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
        public Action onPause;
        public Action onResume;
        public Action onGameOver;
        public Action onGameWinning;
        public Action onGameQuit;
        public Action onExit;

        #endregion

        #region Singleton

        private static GameController _instance;

        public static GameController Instance => _instance ??= new GameController();

        #endregion

    }
}