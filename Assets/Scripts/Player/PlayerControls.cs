using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class PlayerControls : MonoBehaviour
{
    [Header("Tower Reference")]
    [Tooltip("The Transform you want to rotate (the tower root).")]
    public Transform tower;

    [Header("Rotation Settings")]
    [Tooltip("Degrees per second at full drag distance.")]
    public float rotationSpeed = 150f;
    [Tooltip("Smooth damping after releasing drag.")]
    public float inertiaDuration = 0.25f;

    private Vector2 lastInputPos;
    private float currentVelocity;
    private float dragDelta;
    private bool isDragging;
    private Tween inertiaTween;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouse();
#endif

#if UNITY_ANDROID || UNITY_IOS
        HandleTouch();
#endif
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastInputPos = Input.mousePosition;
            inertiaTween?.Kill();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            StartInertia();
        }

        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastInputPos;
            dragDelta = -delta.x; // invert for natural feel
            ApplyRotation(dragDelta);
            lastInputPos = Input.mousePosition;
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount != 1) return;

        Touch t = Input.GetTouch(0);
        switch (t.phase)
        {
            case TouchPhase.Began:
                isDragging = true;
                lastInputPos = t.position;
                inertiaTween?.Kill();
                break;

            case TouchPhase.Moved:
                Vector2 delta = t.deltaPosition;
                dragDelta = -delta.x;
                ApplyRotation(dragDelta);
                lastInputPos = t.position;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isDragging = false;
                StartInertia();
                break;
        }
    }

    private void ApplyRotation(float delta)
    {
        if (!tower) return;

        float rotation = delta * rotationSpeed * Time.deltaTime;
        tower.Rotate(0f, rotation, 0f, Space.World);
        currentVelocity = rotation;
    }

    private void StartInertia()
    {
        if (!tower) return;
        if (Mathf.Abs(currentVelocity) < 0.01f) return;

        float startVel = currentVelocity;
        inertiaTween = DOTween.To(() => startVel, v =>
        {
            float delta = v * Time.deltaTime;
            tower.Rotate(0f, delta, 0f, Space.World);
        }, 0f, inertiaDuration).SetEase(Ease.OutQuad);

        currentVelocity = 0f;
    }
}
