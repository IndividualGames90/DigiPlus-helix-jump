using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System;

[DisallowMultipleComponent]
public class BounceAnimation : MonoBehaviour
{
    [Header("Targets")]
    [Tooltip("Transform to animate (Y position & optional scale). Defaults to this.transform.")]
    public Transform animatedTarget;
    [Tooltip("Optional: where to cast the downward ray from. Defaults to animatedTarget (or this.transform).")]
    public Transform raycastOrigin;
    [Tooltip("Optional: Multiple raycast origins. If empty, will use single raycastOrigin.")]
    public Transform[] raycastOrigins;
    [Tooltip("Optional: Rigidbody whose gravity/velocity are controlled during bounce. If null, we'll search on animatedTarget, then on this GameObject.")]
    public Rigidbody targetRigidbody;

    [Header("Bounce Settings")]
    public float rayDistance = 0.2f;
    public float bounceHeight = 1.5f;
    public float bounceDuration = 0.25f;
    public LayerMask helixLayerMask = ~0;

    [Header("FX (optional)")]
    public bool useSquashStretch = true;
    public float squashXz = 1.1f;
    public float stretchY = 0.9f;
    public float squashDuration = 0.08f;

    [Header("Safety")]
    public float bounceCooldown = 0.05f;

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

    [SerializeField, Tooltip("Minimum time in seconds between raycasts.")]
    private float raycastInterval = 0.03f;

    private float _lastRaycastTime;

    // Cached tweens (typed)
    private TweenerCore<Vector3, Vector3, VectorOptions> cachedMoveTween;
    private TweenerCore<Vector3, Vector3, VectorOptions> cachedSquashTween;
    private TweenerCore<Vector3, Vector3, VectorOptions> cachedUnsquashTween;
    private bool tweensInitialized;

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
        cachedMoveTween?.Kill();
        cachedSquashTween?.Kill();
        cachedUnsquashTween?.Kill();

        if (targetRigidbody) targetRigidbody.useGravity = true;
        if (animatedTarget) animatedTarget.localScale = initialScale;
        isBouncing = false;
        tweensInitialized = false;
    }

    void FixedUpdate()
    {
        if (GameController.IsGameOver || PowerMode.Instance.IsInPowerMode) return;
        if (!animatedTarget) return;
        if (isBouncing) return;
        if (Time.time - lastBounceTime < bounceCooldown) return;

        if (Time.time - _lastRaycastTime < raycastInterval)
            return;
        _lastRaycastTime = Time.time;

        if ((raycastOrigins == null || raycastOrigins.Length == 0) && raycastOrigin != null)
        {
            raycastOrigins = new Transform[] { raycastOrigin };
        }
        else if (raycastOrigins == null || raycastOrigins.Length == 0)
        {
            raycastOrigins = new Transform[] { transform };
        }

        foreach (var origin in raycastOrigins)
        {
            if (!origin) continue;

            if (Physics.Raycast(origin.position, Vector3.down, out RaycastHit hit, rayDistance, helixLayerMask))
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

                break;
            }
        }
    }

    private IEnumerator HandleFinishHelixHit(RaycastHit hit)
    {
        yield return new WaitForSeconds(1);
        try { Destroy(hit.transform.gameObject); }
        catch (NullReferenceException) { }
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
        Transform helixParent = hit.collider.transform.parent;
        if (!helixParent) return;

        HelixController controller = helixParent.GetComponent<HelixController>();
        if (controller != null)
        {
            controller.BreakApart();
            ScoreController.Instance.ResetStreak();
        }
    }

    private void InitTweens()
    {
        if (tweensInitialized || !animatedTarget) return;
        tweensInitialized = true;

        float startY = animatedTarget.position.y;
        float targetY = startY + bounceHeight;

        cachedMoveTween = animatedTarget.DOMoveY(targetY, bounceDuration)
            .SetEase(Ease.OutQuad)
            .SetAutoKill(false)
            .Pause();

        Vector3 squashed = new Vector3(initialScale.x * squashXz,
                                       initialScale.y * stretchY,
                                       initialScale.z * squashXz);

        cachedSquashTween = animatedTarget.DOScale(squashed, squashDuration)
            .SetEase(Ease.OutQuad)
            .SetAutoKill(false)
            .Pause();

        cachedUnsquashTween = animatedTarget.DOScale(initialScale, squashDuration)
            .SetEase(Ease.OutQuad)
            .SetAutoKill(false)
            .Pause();
    }

    private IEnumerator DoBounce(Vector3 contactPoint)
    {
        InitTweens();

        isBouncing = true;
        lastBounceTime = Time.time;

        if (bounceVFXPrefab)
        {
            Vector3 spawnPos = (raycastOrigin ? raycastOrigin.position : transform.position) + vfxOffset;
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

        if (useSquashStretch && animatedTarget)
        {
            cachedSquashTween.Restart();
        }

        cachedMoveTween.ChangeStartValue(new Vector3(animatedTarget.position.x, startY, animatedTarget.position.z))
                       .ChangeEndValue(new Vector3(animatedTarget.position.x, targetY, animatedTarget.position.z), bounceDuration)
                       .Restart();

        yield return cachedMoveTween.WaitForCompletion();

        if (useSquashStretch && animatedTarget)
        {
            cachedUnsquashTween.Restart();
        }

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

    public void SetTargets(Transform animate, Transform rayOrigin = null, Rigidbody rb = null)
    {
        animatedTarget = animate ? animate : transform;
        raycastOrigin = rayOrigin ? rayOrigin : animatedTarget;
        targetRigidbody = rb ? rb : (animatedTarget ? animatedTarget.GetComponent<Rigidbody>() : null);
        if (!targetRigidbody) targetRigidbody = GetComponent<Rigidbody>();
        initialScale = animatedTarget.localScale;
        tweensInitialized = false;
    }
}
