using UnityEngine;

public class TranslationAlignmentChecker : MonoBehaviour
{
    [Header("References")]
    public Transform targetPlane; 
    public Transform toolTip;    
    
    [Header("Offset Arrows (Move Left/Right)")]
    public MeshRenderer arrowNegative; // Arrow pointing against the plane's normal
    public MeshRenderer arrowPositive; // Arrow pointing with the plane's normal

    [Header("Settings (in Meters)")]
    public float toleranceDist = 0.001f; // 1mm safe zone
    public float maxErrorDist = 0.015f;  // 15mm max red zone

    [Header("Scale Settings")]
    public Vector3 alignedScale = new Vector3(0.05f, 0.05f, 0.01f);
    public Vector3 warningScale = new Vector3(0.055f, 0.055f, 0.055f);
    public Vector3 maxScale = new Vector3(0.08f, 0.08f, 0.08f);

    void Update()
    {
        if (!targetPlane || !toolTip) return;

        // Calculate signed perpendicular distance from the plane
        Vector3 planeNormal = targetPlane.forward;
        float offsetError = Vector3.Dot(toolTip.position - targetPlane.position, planeNormal);
        float absError = Mathf.Abs(offsetError);

        Color passive = new Color(1, 1, 1, 0.1f);
        Color success = Color.green;
        Color warningStart = new Color(1f, 0.5f, 0f, 1f); 
        Color activeRed = Color.red;

        Color activeColor;
        Vector3 currentScale;

        // 1. Calculate Active Color and Scale
        if (absError <= toleranceDist) 
        {
            activeColor = success;
            currentScale = alignedScale;
        } 
        else 
        {
            float t = Mathf.Clamp01((absError - toleranceDist) / (maxErrorDist - toleranceDist));
            activeColor = Color.Lerp(warningStart, activeRed, t);
            currentScale = Vector3.Lerp(warningScale, maxScale, t);
        }

        // 2. Apply Scale
        arrowNegative.transform.localScale = currentScale;
        arrowPositive.transform.localScale = currentScale;

        // 3. Drive UI Colors
        if (absError <= toleranceDist)
        {
            arrowNegative.material.color = success;
            arrowPositive.material.color = success;
        }
        else if (offsetError < 0) // Tool is on the positive side of the plane, move negative
        {
            arrowNegative.material.color = activeColor;
            arrowPositive.material.color = passive;
        }
        else // Tool is on the negative side of the plane, move positive
        {
            arrowNegative.material.color = passive;
            arrowPositive.material.color = activeColor;
        }
    }
}