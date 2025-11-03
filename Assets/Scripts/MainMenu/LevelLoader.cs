using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playButton;

    private string selectedLevelName = "Level1";


    private void Awake()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);
        else { }
            //Debug.LogWarning("Play button not assigned in LevelLoader!");
    }

    /// <summary>
    /// Called externally (e.g. by level selection buttons) to set which level will load.
    /// </summary>
    /// <param name="levelName">Scene name (e.g. "Level1")</param>
    public void SelectLevel(string levelName)
    {
        selectedLevelName = levelName;
        Debug.Log($"Selected level: {selectedLevelName}");
    }

    /// <summary>
    /// Called when the Play button is pressed; loads the selected scene.
    /// </summary>
    public void OnPlay()
    {
        if (string.IsNullOrEmpty(selectedLevelName))
        {
            //Debug.LogWarning("No level selected! Please call SelectLevel before playing.");
            return;
        }

        // Load the selected scene
        SceneManager.LoadSceneAsync(selectedLevelName, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
}
