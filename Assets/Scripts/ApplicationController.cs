using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class ApplicationController : Singleton<ApplicationController>
{
    public static readonly string SceneName_MainMenu = "MainMenu";
    public static readonly string SceneName_Level1 = "Level1";
    public static readonly string SceneName_Level2 = "Level2";
    public static readonly string SceneName_Level3 = "Level3";

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(SceneName_MainMenu);
    }

    public void LoadLevel(string levelName)
    {
        switch (levelName)
        {
            case "Level1":
                SceneManager.LoadSceneAsync(SceneName_Level1);
                break;
            case "Level2":
                SceneManager.LoadSceneAsync(SceneName_Level2);
                break;
            case "Level3":
                SceneManager.LoadSceneAsync(SceneName_Level3);
                break;
            default:
                SceneManager.LoadSceneAsync(SceneName_Level1);
                break;
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
