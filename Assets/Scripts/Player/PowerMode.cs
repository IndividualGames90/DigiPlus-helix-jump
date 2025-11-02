using System.Collections;
using UnityEngine;

public class PowerMode : Singleton<PowerMode>
{
    [SerializeField] private GameObject _powerModeFx;
    [SerializeField] private Collider _playerCollider;
    [SerializeField] private int powermodeDuration = 2;
    public bool IsInPowerMode;

    public void StartPowerMode()
    {
        _powerModeFx.SetActive(true);
        _playerCollider.gameObject.layer = LayerMask.NameToLayer("PowerMode");
        StartCoroutine(WaitPowerModeDuration());
    }

    private IEnumerator WaitPowerModeDuration()
    {
        IsInPowerMode = true;

        float elapsed = 0f;
        while (elapsed < powermodeDuration && IsInPowerMode)
        {
            // Counts real seconds regardless of Time.timeScale
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        IsInPowerMode = false;
        _playerCollider.gameObject.layer = LayerMask.NameToLayer("Default");
        _powerModeFx.SetActive(false);
    }
}
