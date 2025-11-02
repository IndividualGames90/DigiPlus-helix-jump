using UnityEngine;

public class SimpleBounce : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _bounceLocation;

    [Header("Bounce Settings")]
    public float bounceForce = 10f;       // strength of the bounce
    public ForceMode forceMode = ForceMode.Impulse; // impulse for instant bounce

    public void PositionPlayer()
    {
        _player.transform.position = _bounceLocation.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only bounce if the colliding object is tagged "Player"
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Clear any downward velocity and apply upward force
        Vector3 velocity = rb.linearVelocity;
        if (velocity.y < 0f) velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * bounceForce, forceMode);
    }
}
