using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private void Awake()
    {
        var game = GameController.Instance;
        game.onGameStart += LoadGameScene;
        game.onPause += () => SetActiveScene(GameConstants.MAIN_SCENE_ID);
        game.onResume += () => SetActiveScene(GameConstants.GAMEPLAY_SCENE_ID);
        game.onMatchEnd += (_) => SetActiveScene(GameConstants.MAIN_SCENE_ID);
        game.onRestart += () =>
        {
            UnloadGameScene();
            LoadGameScene();
        };
        game.onGameQuit += UnloadGameScene;
    }

    private void LoadGameScene() => StartCoroutine(LoadGameSceneCoroutine());
    private void UnloadGameScene() => StartCoroutine(UnloadGameSceneCoroutine());

    private IEnumerator LoadGameSceneCoroutine()
    {
        var load = SceneManager.LoadSceneAsync(GameConstants.GAMEPLAY_SCENE_ID, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;
        SetActiveScene(GameConstants.GAMEPLAY_SCENE_ID);
    }

    private IEnumerator UnloadGameSceneCoroutine()
    {
        var unload = SceneManager.UnloadSceneAsync(GameConstants.GAMEPLAY_SCENE_ID);
        while (!unload.isDone) yield return null;
        SetActiveScene(GameConstants.MAIN_SCENE_ID);
    }

    private void SetActiveScene(int sceneId) => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneId));
}
