using UnityEngine;
using System.Collections;

public class TrollBall : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Canvas image to toggle when colliding.")]
    [SerializeField] private GameObject canvasImage;

    [Tooltip("Optional reference to the player (if not auto-assigned).")]
    [SerializeField] private Transform playerTransform;

    [Header("Settings")]
    [Tooltip("How fast the TrollBall rises upward.")]
    [SerializeField] private float riseSpeed = 1.5f;

    [Tooltip("How far to offset on the X and Z axis when attached.")]
    [SerializeField] private Vector3 attachOffset = new Vector3(0.5f, 0f, 0.5f);

    [Tooltip("Temporary gravity scale when attached to player.")]
    [SerializeField] private float heavyGravity = 20f;

    private float defaultGravity;
    private bool isAttached = false;

    private void Awake()
    {
        defaultGravity = Physics.gravity.y;
    }

    private void Start()
    {
        // Try to auto-find the player if not assigned
        if (!playerTransform && GameController.Instance && GameController.Instance.PlayerTransform)
            playerTransform = GameController.Instance.PlayerTransform;
        else if (!playerTransform)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found) playerTransform = found.transform;
        }
    }

    private void Update()
    {
        // Make the TrollBall rise over time
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime, Space.World);

        // If attached, follow the player
        if (isAttached && playerTransform)
        {
            transform.position = playerTransform.position + attachOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StartCoroutine(HandleCanvasFlash());

        // Attach to player
        if (playerTransform == null)
            playerTransform = other.transform;

        isAttached = true;

        // Increase gravity
        Physics.gravity = new Vector3(0, -heavyGravity, 0);
    }

    private IEnumerator HandleCanvasFlash()
    {
        if (canvasImage)
        {
            canvasImage.SetActive(true);
            yield return new WaitForSeconds(1f);
            canvasImage.SetActive(false);
        }
    }

    public void DetachFromPlayer()
    {
        isAttached = false;
        // Reset gravity to default
        Physics.gravity = new Vector3(0, defaultGravity, 0);
    }
}
