using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Transactions;
using System.Security.Cryptography;
using System;

[DisallowMultipleComponent]
public class BounceAnimation : MonoBehaviour
{
    [Header("Targets")]
    [Tooltip("Transform to animate (Y position & optional scale). Defaults to this.transform.")]
    public Transform animatedTarget;
    [Tooltip("Optional: where to cast the downward ray from. Defaults to animatedTarget (or this.transform).")]
    public Transform raycastOrigin;
    [Tooltip("Optional: Rigidbody whose gravity/velocity are controlled during bounce. If null, we'll search on animatedTarget, then on this GameObject.")]
    public Rigidbody targetRigidbody;

    [Header("Bounce Settings")]
    public float rayDistance = 0.2f;      // short downward check
    public float bounceHeight = 1.5f;     // how high to go
    public float bounceDuration = 0.25f;  // time to apex
    public LayerMask helixLayerMask = ~0; // filter; ~0 = everything

    [Header("FX (optional)")]
    public bool useSquashStretch = true;
    public float squashXz = 1.1f;         // >1 widens XZ at contact
    public float stretchY = 0.9f;         // <1 shortens Y at contact
    public float squashDuration = 0.08f;

    [Header("Safety")]
    public float bounceCooldown = 0.05f;   // avoid double-trigger

    [Header("VFX")]
    [Tooltip("Prefab to spawn when the bounce begins (impact).")]
    public GameObject bounceVFXPrefab;
    [Tooltip("Offset from the raycast origin where the VFX will spawn.")]
    public Vector3 vfxOffset = Vector3.zero;
    [Tooltip("Time (in seconds) before spawned VFX is destroyed.")]
    public float vfxLifetime = 2f;

    private bool isBouncing;
    private float lastBounceTime = -999f;
    private Tween moveTween;
    private Tween scaleTween;
    private Vector3 initialScale;

    void Awake()
    {
        if (!animatedTarget) animatedTarget = transform;
        if (!raycastOrigin) raycastOrigin = animatedTarget ? animatedTarget : transform;

        if (!targetRigidbody)
        {
            if (animatedTarget) targetRigidbody = animatedTarget.GetComponent<Rigidbody>();
            if (!targetRigidbody) targetRigidbody = GetComponent<Rigidbody>();
        }

        initialScale = animatedTarget ? animatedTarget.localScale : Vector3.one;
    }

    void OnDisable()
    {
        moveTween?.Kill();
        scaleTween?.Kill();
        if (targetRigidbody) targetRigidbody.useGravity = true;
        if (animatedTarget) animatedTarget.localScale = initialScale;
        isBouncing = false;
    }

    void FixedUpdate()
    {
        if (GameController.IsGameOver || PowerMode.Instance.IsInPowerMode) return;

        if (!animatedTarget || !raycastOrigin) return;
        if (isBouncing) return;
        if (Time.time - lastBounceTime < bounceCooldown) return;

        // short downward ray from the chosen origin
        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out RaycastHit hit, rayDistance, helixLayerMask))
        {
            if (hit.collider.CompareTag("Helix"))
            {
                HandleHelixHit(hit);
                StartCoroutine(DoBounce(hit.point));
            }
            else if (hit.collider.CompareTag("Gear"))
            {
                HandleHelixHit(hit);
                StartCoroutine(DoBounce(hit.point));
                ScoreController.Instance.ResetStreak();
            }
            else if (hit.collider.CompareTag("Lava"))
            {
                GameController.Instance.GameOver(0);
            }
            else if (hit.collider.CompareTag("FinishGame"))
            {
                StartCoroutine(HandleFinishHelixHit(hit));
            }
            else if (hit.collider.CompareTag("BonusHelix"))
            {
                HandleBonusHelixHit(hit);
            }
        }
    }

    private IEnumerator HandleFinishHelixHit(RaycastHit hit)
    {
        yield return new WaitForSeconds(1);
        try
        {
            Destroy(hit.transform.gameObject);
        }
        catch (NullReferenceException e) { }
    }

    private void HandleBonusHelixHit(RaycastHit hit)
    {
        Transform helixParent = hit.collider.transform.parent;
        if (!helixParent) return;
        var controller = helixParent.GetComponent<BonusHelixController>();
        controller.BonusHelixHit();
    }

    private void HandleHelixHit(RaycastHit hit)
    {
        // Find HelixController on the parent
        Transform helixParent = hit.collider.transform.parent;
        if (!helixParent) return;

        HelixController controller = helixParent.GetComponent<HelixController>();
        if (controller != null)
        {
            controller.BreakApart();
            ScoreController.Instance.ResetStreak();
        }
    }

    private IEnumerator DoBounce(Vector3 contactPoint)
    {
        isBouncing = true;
        lastBounceTime = Time.time;

        // spawn VFX at the contact point (or near the ray origin)
        if (bounceVFXPrefab)
        {
            Vector3 spawnPos = (raycastOrigin ? raycastOrigin.position : transform.position) + vfxOffset;

            // prefer to place VFX at actual hit point if close enough
            if (Vector3.Distance(spawnPos, contactPoint) < 1f)
                spawnPos = contactPoint + vfxOffset;

            GameObject vfx = Instantiate(bounceVFXPrefab, spawnPos, Quaternion.identity);
            Destroy(vfx, vfxLifetime);
        }

        if (targetRigidbody)
        {
            targetRigidbody.useGravity = false;
            targetRigidbody.linearVelocity = Vector3.zero;
        }

        float startY = animatedTarget.position.y;
        float targetY = startY + bounceHeight;

        moveTween?.Kill();
        scaleTween?.Kill();

        // optional squash at contact
        if (useSquashStretch && animatedTarget)
        {
            Vector3 squashed = new Vector3(initialScale.x * squashXz,
                                           initialScale.y * stretchY,
                                           initialScale.z * squashXz);
            animatedTarget.localScale = initialScale;
            scaleTween = animatedTarget.DOScale(squashed, squashDuration)
                                       .SetEase(Ease.OutQuad);
        }

        // go up with ease-out
        moveTween = animatedTarget.DOMoveY(targetY, bounceDuration)
                                  .SetEase(Ease.OutQuad);

        yield return moveTween.WaitForCompletion();

        // restore scale toward normal at apex
        if (useSquashStretch && animatedTarget)
        {
            scaleTween?.Kill();
            scaleTween = animatedTarget.DOScale(initialScale, squashDuration)
                                       .SetEase(Ease.OutQuad);
        }

        // tiny buffer at apex to avoid immediate re-hit
        yield return new WaitForSeconds(0.03f);

        if (targetRigidbody) targetRigidbody.useGravity = true;
        isBouncing = false;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Transform origin = raycastOrigin ? raycastOrigin : (animatedTarget ? animatedTarget : transform);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin.position, origin.position + Vector3.down * rayDistance);
    }
#endif

    // optional: set targets at runtime
    public void SetTargets(Transform animate, Transform rayOrigin = null, Rigidbody rb = null)
    {
        animatedTarget = animate ? animate : transform;
        raycastOrigin = rayOrigin ? rayOrigin : animatedTarget;
        targetRigidbody = rb ? rb : (animatedTarget ? animatedTarget.GetComponent<Rigidbody>() : null);
        if (!targetRigidbody) targetRigidbody = GetComponent<Rigidbody>();
        initialScale = animatedTarget.localScale;
    }
}
