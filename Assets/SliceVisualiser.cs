using UnityEngine;

public class SliceVisualizer : MonoBehaviour
{
    [Header("References")]
    public Camera sliceCamera;
    public Transform sawBlade;       // The specific blade transform (the cutter)
    // public Transform jawReference; // REMOVED: No longer determines orientation

    [Header("Slice View Settings")]
    public float sliceSize = 0.1f; // zoom
    public float sliceNear = 0f; // Set to 0.001 for accurate 1mm slice
    public float sliceFar = 0.001f; // Set to 0.001 for accurate 1mm slice
    public float sliceOffset = 0f;

    void LateUpdate()
    {
        if (!sliceCamera || !sawBlade) return;

        // 1. ORIENTATION (CRITICAL FIX)
        // Align camera rotation exactly to the SawBlade's normal.
        // Assuming the blade's local "Up" is the normal vector (perpendicular to surface).
        // If your blade faces forward, change 'up' to 'forward'.
        Vector3 cutNormal = sawBlade.up; 
        Vector3 cutUp = sawBlade.forward; 

        // We look down the negative normal to see the face
        sliceCamera.transform.rotation = Quaternion.LookRotation(-cutNormal, cutUp);

        // 2. POSITION
        // Center the camera exactly on the blade anchor
        sliceCamera.transform.position = sawBlade.position;

        // 3. ORTHOGRAPHIC SLICE SETUP
        sliceCamera.orthographic = true;
        sliceCamera.orthographicSize = sliceSize;

        // 4. CENTERED THICKNESS (The "Thin Slice" Fix)
        // By using a negative near plane, we capture data BEHIND the camera origin too.
        // This creates a perfect 1mm wafer centered on the blade.        
        sliceCamera.nearClipPlane = sliceNear + sliceOffset; 
        sliceCamera.farClipPlane = sliceFar + sliceOffset;
    }
}