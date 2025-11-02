using System;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private string _currentSceneName = "MainMenu";

    public Transform PlayerTransform;

    public Action<int> GameOverEvent;

    public static bool IsGameOver { get; private set; }

    private void Start()
    {
        IsGameOver = false;
    }

    public void GameOver(int score)
    {
        if (IsGameOver) return;

        _playerControls.enabled = false;
        IsGameOver = true;
        GameOverEvent?.Invoke(score);
        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        if (ApplicationController.Instance != null)
        {
            ApplicationController.Instance.QuitApplication();
        }
        else
        {
            Debug.LogError($"Application Quit Failed. Instance Null.");
        }
    }

    public void QuitApplication()
    {
        if (ApplicationController.Instance != null)
        {
            ApplicationController.Instance.QuitApplication();
        }
        else
        {
            Debug.LogError($"Application Quit Failed. Instance Null.");
        }
    }
}
