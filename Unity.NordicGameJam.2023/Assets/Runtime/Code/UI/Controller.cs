using UnityEngine;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour
{
    private UIDocument _doc;

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

        // grab refs to the ui buttons 
        _playBtn = _doc.rootVisualElement.Q<Button>("play-btn");
        _exitBtn = _doc.rootVisualElement.Q<Button>("exit-btn");
        _resumeBtn = _doc.rootVisualElement.Q<Button>("resume-btn");
        _restartBtn = _doc.rootVisualElement.Q<Button>("restart-btn");
        _quitBtn = _doc.rootVisualElement.Q<Button>("quit-btn");
        _tryAgainBtn = _doc.rootVisualElement.Q<Button>("try-again-btn");
        _quitAsLooserBtn = _doc.rootVisualElement.Q<Button>("quit-as-looser-btn");
        _playAgainBtn = _doc.rootVisualElement.Q<Button>("play-again-btn");
        _quitAsWinnerBtn = _doc.rootVisualElement.Q<Button>("quit-as-winner-btn");

        // wire the buttons' interaction logic
        _playBtn.clicked += () => Debug.Log("PLAY");
        _exitBtn.clicked += () => Debug.Log("EXIT");
        _resumeBtn.clicked += () => Debug.Log("RESUME");
        _restartBtn.clicked += () => Debug.Log("RESTART");
        _quitBtn.clicked += () => Debug.Log("QUIT");
        _tryAgainBtn.clicked += () => Debug.Log("TRY AGAIN");
        _quitAsLooserBtn.clicked += () => Debug.Log("QUIT AS A LOSER");
        _playAgainBtn.clicked += () => Debug.Log("PLAY AGAIN");
        _quitAsWinnerBtn.clicked += () => Debug.Log("QUIT AS A WINNER");

    }

}
