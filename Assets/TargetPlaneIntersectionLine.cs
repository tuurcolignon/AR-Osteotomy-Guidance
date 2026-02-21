using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TargetPlaneIntersectionLine : MonoBehaviour
{
    [Header("References")]
    public Transform targetPlane; // The original pink plane guiding the cut
    public Camera sliceCam;       // The camera outputting to the monitor

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true; 
    }

    void LateUpdate()
    {
        if (!targetPlane || !sliceCam) return;

        // Plane 1: The Target Cut (using forward based on your previous fix)
        Vector3 n1 = targetPlane.forward; 
        Vector3 p1 = targetPlane.position;

        // Plane 2: The Camera's Slice Plane 
        // We place this exactly halfway between your near and far clipping planes so it never gets clipped.
        Vector3 n2 = sliceCam.transform.forward;
        float midClip = (sliceCam.nearClipPlane + sliceCam.farClipPlane) * 0.5f;
        Vector3 p2 = sliceCam.transform.position + n2 * midClip;

        // Calculate intersection line direction via Cross Product
        Vector3 dir = Vector3.Cross(n1, n2);
        float det = dir.sqrMagnitude;

        // If planes are perfectly parallel, there is no intersection line to draw
        if (det < 0.0001f) 
        {
            line.enabled = false;
            return;
        }
        line.enabled = true;

        // Calculate a point on the intersection line
        float d1 = -Vector3.Dot(n1, p1);
        float d2 = -Vector3.Dot(n2, p2);
        Vector3 point = Vector3.Cross(d2 * n1 - d1 * n2, dir) / det;

        // Normalize direction for accurate distance projection
        dir.Normalize();

        // Get the actual physical bounds of the plane mesh
        MeshFilter mf = targetPlane.GetComponent<MeshFilter>();
        if (!mf) return;

        Bounds b = mf.sharedMesh.bounds;
        Vector3[] corners = {
            new Vector3(b.min.x, b.min.y, b.min.z), new Vector3(b.max.x, b.min.y, b.min.z),
            new Vector3(b.min.x, b.max.y, b.min.z), new Vector3(b.max.x, b.max.y, b.min.z),
            new Vector3(b.min.x, b.min.y, b.max.z), new Vector3(b.max.x, b.min.y, b.max.z),
            new Vector3(b.min.x, b.max.y, b.max.z), new Vector3(b.max.x, b.max.y, b.max.z)
        };

        float tMin = float.MaxValue;
        float tMax = float.MinValue;

        // Project all 8 corners of the bounding box onto the infinite line
        foreach (Vector3 corner in corners)
        {
            Vector3 worldCorner = targetPlane.TransformPoint(corner);
            float t = Vector3.Dot(worldCorner - point, dir);
            if (t < tMin) tMin = t;
            if (t > tMax) tMax = t;
        }

        // Clamp the LineRenderer to the exact edges of the plane
        line.SetPosition(0, point + dir * tMin);
        line.SetPosition(1, point + dir * tMax);
    }
}