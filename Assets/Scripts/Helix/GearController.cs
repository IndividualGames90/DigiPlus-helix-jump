using UnityEngine;

public class GearController : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Rotation Settings")]
    [Tooltip("Select which axis the gear should rotate around.")]
    public RotationAxis rotationAxis = RotationAxis.Y;

    [Tooltip("Degrees per second to rotate the gear.")]
    public float rotationSpeed = 180f; // speed in degrees per second

    [Tooltip("Reverse the rotation direction if true.")]
    public bool reverse = false;


    void Update()
    {
        float direction = reverse ? -1f : 1f;
        Vector3 axis = GetAxisVector(rotationAxis);

        transform.Rotate(axis * direction * rotationSpeed * Time.deltaTime, Space.Self);
    }

    private Vector3 GetAxisVector(RotationAxis axis)
    {
        switch (axis)
        {
            case RotationAxis.X:
                return Vector3.right;
            case RotationAxis.Y:
                return Vector3.up;
            case RotationAxis.Z:
                return Vector3.forward;
            default:
                return Vector3.up;
        }
    }
}
