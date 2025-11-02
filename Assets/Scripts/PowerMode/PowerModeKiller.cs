using UnityEngine;

public class PowerModeKiller : MonoBehaviour
{
    [SerializeField] private PowerMode PowerMode;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerMode.IsInPowerMode = false;
        }
    }
}
