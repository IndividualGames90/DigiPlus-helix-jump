using UnityEngine;

/// <summary>
/// A reusable generic Singleton base class for MonoBehaviours.
/// Inherit from this to make any component globally accessible via Instance.
/// Example:
///   public class BreakApartAnimator : Singleton<BreakApartAnimator> { }
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _quitting = false;

    /// <summary>
    /// Global access to this instance.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_quitting) return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    // Try to find existing instance in the scene
                    _instance = FindObjectOfType<T>();

                    // If still none found, create one automatically
                    if (_instance == null)
                    {
                        GameObject singletonObj = new GameObject(typeof(T).Name);
                        _instance = singletonObj.AddComponent<T>();
                        DontDestroyOnLoad(singletonObj);
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Unity Awake() — ensures singleton uniqueness.
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // enforce one instance
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _quitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
