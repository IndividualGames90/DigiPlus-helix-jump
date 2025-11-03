using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button nextLevelButton;

    [Header("Optional")]
    [Tooltip("Scene name or index to load when Restart is clicked.")]
    [SerializeField] private string restartSceneName = "Level1";

    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject score;

    [SerializeField] private GameController gameController;

    [SerializeField] private TextMeshProUGUI _scoreNumber;

    [Header("ScorePanel")]
    [SerializeField] private TextMeshProUGUI _scoreCurrent;
    [SerializeField] private TextMeshProUGUI _scoreMultiplier;

    [Tooltip("Scene name or index to load when Next Level is clicked.")]
    [SerializeField] private string nextLevelSceneName;


    private bool _raiseScoreLocked;

    public void UpdateCurrentScore(int newScore)
    {
        _scoreCurrent.text = newScore.ToString();
    }

    public void UpdateScoreMultiplier(int multiplier)
    {
        _scoreMultiplier.text = "x" + multiplier.ToString();
        DoBounceAnimation(_scoreMultiplier.gameObject);
    }

    private void Awake()
    {
        gameController.GameOverEvent += OnGameOver;

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
    }


    private void OnDestroy()
    {
        gameController.GameOverEvent -= OnGameOver;

        // clean up listeners
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClicked);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
        if (nextLevelButton != null)
            nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
    }


    public void OnGameOver(int score, bool failed)
    {
        if (!failed)
        {
            nextLevelButton.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(false);
        }

        this.score.SetActive(false);
        gameOverMenu.SetActive(true);
        StartCoroutine(RaiseScore(ScoreController.Instance.GetScore()));
    }

    private IEnumerator RaiseScore(int finalScore)
    {
        if (_raiseScoreLocked)
            yield break;
        _raiseScoreLocked = true;

        int displayedScore = 0;
        float duration = .7f; // Duration of the score animation
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(0, finalScore, t));
            _scoreNumber.text = displayedScore.ToString();
            yield return null;
        }
        // Ensure the final score is set
        _scoreNumber.text = finalScore.ToString();
        DoBounceAnimation(_scoreNumber.gameObject);
    }

    private void OnRestartClicked()
    {
        // Reload level or load given scene
        if (!string.IsNullOrEmpty(restartSceneName))
        {

            SceneManager.UnloadSceneAsync(gameObject.scene);
            SceneManager.LoadSceneAsync(restartSceneName, LoadSceneMode.Additive);
        }
        else
        {

            SceneManager.UnloadSceneAsync(gameObject.scene);
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Additive);

        }
    }

    private void OnNextLevelClicked()
    {
        // Load the next level scene by name, if provided
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.UnloadSceneAsync(gameObject.scene);
            SceneManager.LoadSceneAsync(nextLevelSceneName, LoadSceneMode.Additive);
        }
        else
        {
            // fallback: load next build index
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.UnloadSceneAsync(gameObject.scene);
                SceneManager.LoadSceneAsync(nextIndex, LoadSceneMode.Additive);
            }
            else
            {
                // if no next scene, return to Main Menu (index 0)
                SceneManager.UnloadSceneAsync(gameObject.scene);
                SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
            }
        }
    }


    public void DoBounceAnimation(GameObject uiElement, float bounceScale = 1.2f, float duration = 0.25f)
    {
        if (uiElement == null) return;

        RectTransform rect = uiElement.GetComponent<RectTransform>();
        if (rect == null) return;

        rect.DOKill();
        rect.localScale = Vector3.one;

        rect.DOScale(Vector3.one * bounceScale, duration * 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                rect.DOScale(Vector3.one, duration * 0.5f)
                    .SetEase(Ease.InOutQuad);
            });
    }



    private void OnQuitClicked()
    {
        // Call actual quit (ignored in editor)
        Debug.Log("Quit button clicked!");
        ApplicationController.Instance.QuitApplication();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
