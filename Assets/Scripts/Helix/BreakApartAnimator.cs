using UnityEngine;

[DisallowMultipleComponent]
public class BreakApartAnimator : Singleton<BreakApartAnimator>
{
    [Header("Break Settings")]
    [Tooltip("Force pushing the piece away from its local center.")]
    [SerializeField] private float kickForce = 6f;

    [Tooltip("Upward force added to give lift.")]
    [SerializeField] private float upwardForce = 2f;

    [Tooltip("Random torque strength for spinning pieces.")]
    [SerializeField] private float torqueStrength = 5f;

    [Tooltip("Lifetime before the piece is destroyed.")]
    [SerializeField] private float lifetime = 2f;

    [Tooltip("Optional: remove collider to prevent re-hit.")]
    [SerializeField] private bool removeCollider = true;

    [SerializeField] private ColorChanger colorChanger;

    /// <summary>
    /// Kicks the given helix piece away from its local center.
    /// </summary>
    public void TriggerBreak(GameObject piece, Transform localCenter)
    {
        if (!piece || !localCenter) return;

        // Change material to broken version
        colorChanger.ApplyBrokenMaterial(piece);

        // Cache transform and detach from parent
        Transform t = piece.transform;
        t.SetParent(null, true);

        // Compute outward direction from this HelixController's center (XZ only)
        Vector3 outwardDir = (t.position - localCenter.position);
        outwardDir.y = 0f;
        outwardDir.Normalize();

        // Remove collider if requested
        Collider col = piece.GetComponent<Collider>();
        if (col && removeCollider)
            Destroy(col);

        // Ensure a rigidbody exists
        Rigidbody rb = piece.GetComponent<Rigidbody>();
        if (!rb) rb = piece.AddComponent<Rigidbody>();

        // Reset Rigidbody state
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Apply outward impulse + upward lift
        Vector3 forceDir = outwardDir * kickForce + Vector3.up * upwardForce;
        rb.AddForce(forceDir, ForceMode.Impulse);

        // Add random spin
        //rb.AddTorque(Random.insideUnitSphere * torqueStrength, ForceMode.Impulse);

        // Destroy piece after lifetime
        Destroy(piece, lifetime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.15f);
    }
#endif
}
