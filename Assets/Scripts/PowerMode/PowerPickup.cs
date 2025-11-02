using UnityEngine;

public class PowerPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerMode.Instance.StartPowerMode(); // Activate power mode on the player
            Destroy(gameObject); // Remove the pickup from the scene
        }
    }
}
