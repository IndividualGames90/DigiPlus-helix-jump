using UnityEngine;

[DisallowMultipleComponent]
public class ScoreController : Singleton<ScoreController>
{
    [SerializeField] private CanvasController _canvas;

    [Header("Base Settings")]
    [SerializeField] private int baseScore = 1;
    [SerializeField] private int fireModeThreshold = 5;

    [Header("Multipliers")]
    [SerializeField] private float streakMultiplierStep = 0.5f;
    [SerializeField] private float maxMultiplier = 5f;

    [Header("Debug Info")]
    [SerializeField] private int currentScore;
    [SerializeField] private int currentStreak;
    [SerializeField] private float currentMultiplier = 1f;
    [SerializeField] private bool isFireMode;

    public int CurrentScore => currentScore;
    public bool IsFireMode => isFireMode;

    private void Start()
    {
        _canvas.UpdateCurrentScore(0);
    }

    private void OnEnable()
    {
        ResetStreak();
    }

    public void AddHelixBreak()
    {
        currentStreak++;

        currentMultiplier = 1f + (currentStreak - 1) * streakMultiplierStep;
        currentMultiplier = Mathf.Min(currentMultiplier, maxMultiplier);

        int points = Mathf.RoundToInt(baseScore * currentMultiplier);
        currentScore += points;

        if (currentStreak >= fireModeThreshold && !isFireMode)
        {
            isFireMode = true;
        }

        UpdateCanvas();
    }

    public void AddBonusHelixBreak(int bonus)
    {
        int bonusPoints = Mathf.RoundToInt(currentScore * bonus);

        currentScore += bonusPoints;

        UpdateCanvas();
    }

    private void UpdateCanvas()
    {
        _canvas.UpdateCurrentScore(currentScore);
        _canvas.UpdateScoreMultiplier(Mathf.RoundToInt(currentMultiplier));
    }

    public void ResetStreak()
    {
        currentStreak = 0;
        currentMultiplier = 1f;
        isFireMode = false;
    }

    public int GetScore()
    {
        return currentScore;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 250, 30), $"Score: {currentScore}");
        GUI.Label(new Rect(20, 40, 250, 30), $"Streak: {currentStreak}");
        GUI.Label(new Rect(20, 60, 250, 30), $"Multiplier: x{currentMultiplier:F1}");
        GUI.Label(new Rect(20, 80, 250, 30), isFireMode ? "🔥 FIRE MODE 🔥" : "");
    }
#endif
}
