using UnityEngine;

public class PowerModeKiller : MonoBehaviour
{
    [SerializeField] private PowerMode powerMode;

    private void Awake()
    {
        // If not assigned in the Inspector, try to find it in the scene
        if (powerMode == null)
        {
            powerMode = FindFirstObjectByType<PowerMode>();
            if (powerMode == null)
            {
                //Debug.LogWarning("PowerModeKiller: No PowerMode found in scene!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && powerMode != null)
        {
            powerMode.IsInPowerMode = false;
        }
    }
}
