using UnityEngine;

public class YawAlignmentChecker : MonoBehaviour
{
    [Header("References")]
    public Transform targetPlane; 
    public Transform sawBlade;    
    public MeshRenderer arrowLeft;
    public MeshRenderer arrowRight;

    [Header("Settings")]
    public float toleranceDeg = 1f;
    public float maxErrorDeg = 15f; 

    [Header("Scale Settings")]
    public Vector3 alignedScale = new Vector3(0.5f, 0.5f, 0.5f); 
    public Vector3 warningScale = new Vector3(1.2f, 1.2f, 1.2f); 
    public Vector3 maxScale = new Vector3(2.5f, 2.5f, 2.5f);     

    void Update()
    {
        if (!targetPlane || !sawBlade) return;

        Vector3 planeNormal = targetPlane.forward; 
        Vector3 toolNormal = sawBlade.forward;     
        Vector3 toolEdge = sawBlade.up; // The 3rd axis (Pitch/Tilt)

        // Project plane normal onto the tool's vertical cross-section
        Vector3 idealPitchDir = Vector3.ProjectOnPlane(planeNormal, toolEdge);
        if (idealPitchDir.sqrMagnitude < 0.001f) return;
        idealPitchDir.Normalize();

        // Calculate pitch error
        float pitchError = Vector3.SignedAngle(toolNormal, idealPitchDir, toolEdge);
        float absError = Mathf.Abs(pitchError);

        Color passive = new Color(1, 1, 1, 0.1f);
        Color success = Color.green;
        Color warningStart = new Color(1f, 0.5f, 0f, 1f); 
        Color activeRed = Color.red;

        Color activeColor;
        Vector3 currentScale;

        // 1. Calculate Active Color and Scale
        if (absError <= toleranceDeg) 
        {
            activeColor = success;
            currentScale = alignedScale;
        } 
        else 
        {
            float t = Mathf.Clamp01((absError - toleranceDeg) / (maxErrorDeg - toleranceDeg));
            activeColor = Color.Lerp(warningStart, activeRed, t);
            currentScale = Vector3.Lerp(warningScale, maxScale, t);
        }

        // 2. Apply Scale globally to both arrows
        arrowLeft.transform.localScale = currentScale;
        arrowRight.transform.localScale = currentScale;

        // 3. Drive UI Colors
        if (absError <= toleranceDeg)
        {
            arrowLeft.material.color = success;
            arrowRight.material.color = success;
        }
        else if (pitchError < 0) // Swap < and > if arrows point backwards
        {
            arrowLeft.material.color = activeColor;
            arrowRight.material.color = passive;
        }
        else
        {
            arrowLeft.material.color = passive;
            arrowRight.material.color = activeColor;
        }
    }
}