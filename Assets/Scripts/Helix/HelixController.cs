using UnityEngine;

public class HelixController : MonoBehaviour
{
    [SerializeField] protected GameObject[] _allHelix;
    [SerializeField] private bool _enableDeadlies;
    [SerializeField] private int _deadlyCount = 2;

    private Transform _playerTransform;

    private bool _broken;
    protected bool _unbreakable;

    private void Awake()
    {
        _playerTransform = GameController.Instance.PlayerTransform;

        if (_enableDeadlies)
        {
            for (int i = 0; i < _deadlyCount; i++)
            {
                var index = Random.Range(0, _allHelix.Length);
                ColorChanger.Instance.ApplyLavaMaterial(_allHelix[index]);
                _allHelix[index].gameObject.tag = "Lava";
            }
        }
    }

    public void BreakApart()
    {
        if (_broken || _unbreakable) return;

        _broken = true;
        foreach (var helix in _allHelix)
        {
            BreakApartAnimator.Instance.TriggerBreak(helix, transform);
        }

        ScoreController.Instance.AddHelixBreak();
    }

    private void FixedUpdate()
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameController.Instance.FindPlayerInScene(gameObject).transform;
        }

        if (_playerTransform.position.y < transform.position.y && !_broken)
        {
            BreakApart();
        }
    }
}
