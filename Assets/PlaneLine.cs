using UnityEngine;

[ExecuteInEditMode]
public class PlaneLine : MonoBehaviour
{
    public Transform cutPlane;
    public Renderer jawRenderer;

    void Update()
    {
        if (cutPlane == null || jawRenderer == null) return;

        Material mat = jawRenderer.sharedMaterial;
        mat.SetVector("_PlanePosition", cutPlane.position);
        mat.SetVector("_PlaneNormal", cutPlane.forward); 
        mat.SetVector("_PlaneRight", cutPlane.right);
        mat.SetVector("_PlaneUp", cutPlane.up);
        mat.SetVector("_PlaneScale", cutPlane.lossyScale);
    }
}