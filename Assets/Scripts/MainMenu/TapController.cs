using UnityEngine;

public class TapController : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera mainCamera;
    public float maxDistance = 100f;
    public LayerMask helixLayerMask; // Assign your Helix layer here in the Inspector

    [SerializeField] private LevelLoader levelLoader;

    private void Awake()
    {
        if (!mainCamera) mainCamera = Camera.main;
    }

    void Update()
    {
        // Check for mouse click (PC) or tap (mobile)
        if (Input.GetMouseButtonDown(0))
            HandleInput(Input.mousePosition);
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            HandleInput(Input.GetTouch(0).position);
    }

    void HandleInput(Vector3 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, helixLayerMask))
        {
            if (hit.collider.CompareTag("Helix"))
            {
                // Try to get the SimpleBounce script and call PositionPlayer
                SimpleBounce bounce = hit.collider.GetComponent<SimpleBounce>();
                if (bounce != null)
                {

                    levelLoader.SelectLevel(bounce.LevelName);
                    bounce.PositionPlayer();
                }
                else
                {
                    //Debug.LogWarning("No SimpleBounce found on Helix object: " + hit.collider.name);
                }
            }
        }
    }
}
