using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        var a = GetComponent<Rigidbody>();

        BuildDebugger.Instance.Log(a.useGravity + "/" +  a.isKinematic);
    }
}
