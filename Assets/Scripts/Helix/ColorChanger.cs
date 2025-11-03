using UnityEngine;

[DisallowMultipleComponent]
public class ColorChanger : Singleton<ColorChanger>
{
    [Header("Material Settings")]
    [Tooltip("Material to apply when the mesh is broken.")]
    public Material brokenMaterial;

    [SerializeField] private Material _lavaMaterial;

    public void ApplyLavaMaterial(GameObject meshObject)
    {
        Renderer rend = meshObject.GetComponent<Renderer>();
        rend.material = brokenMaterial;
    }

    /// <summary>
    /// Replaces the given mesh object's material with the broken material.
    /// </summary>
    /// <param name="meshObject">The GameObject that has a Renderer.</param>
    public void ApplyBrokenMaterial(GameObject meshObject)
    {
        if (!meshObject || !brokenMaterial)
        {
            //Debug.LogWarning("ColorChanger: Missing meshObject or brokenMaterial.");
            return;
        }

        Renderer rend = meshObject.GetComponent<Renderer>();
        if (!rend)
        {
            //Debug.LogWarning($"ColorChanger: No Renderer found on {meshObject.name}.");
            return;
        }

        // Create a unique material instance for that mesh (optional but safer)
        rend.material = brokenMaterial;
    }
}
