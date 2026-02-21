using UnityEngine;

public class SlicePlaneController : MonoBehaviour
{
    [Header("References")]
    public Transform sawBlade;
    public Renderer jawRenderer;
    public Renderer capRenderer;

    void LateUpdate()
    {
        if (!sawBlade || !jawRenderer || !capRenderer) return;

        Vector3 normal = sawBlade.up;
        Vector3 point = sawBlade.position;

        Vector4 plane = new Vector4(
            normal.x,
            normal.y,
            normal.z,
            -Vector3.Dot(normal, point)
        );

        jawRenderer.material.SetVector("_SlicePlane", plane);
        capRenderer.material.SetVector("_SlicePlane", plane);

        // Move & orient cap
        capRenderer.transform.position = point;
        capRenderer.transform.rotation = Quaternion.LookRotation(normal);
    }
}
