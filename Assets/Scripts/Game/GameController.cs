using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private string _currentSceneName = "MainMenu";

    public Transform PlayerTransform;

    public GameObject FindPlayerInScene(GameObject context)
    {
        Scene scene = context.scene;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.CompareTag("Player"))
                return root;

            Transform[] children = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.CompareTag("Player"))
                    return child.gameObject;
            }
        }

        return null;
    }

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
