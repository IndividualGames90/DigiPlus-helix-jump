using TMPro;
using UnityEngine;

public class BuildDebugger : Singleton<BuildDebugger>
{
    [SerializeField] private TextMeshProUGUI debugText;

    public void Log(string log)
    {
        debugText.text = log;
    }
}
