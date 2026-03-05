using UnityEngine;

/// <summary>
/// Translational offset arrows – show which direction to move the tool
/// along the plane's normal to get it onto the cutting plane.
/// Reads OffsetErrorM from AlignmentSensor.
/// </summary>
[RequireComponent(typeof(AlignmentSensor))]
public class TranslationAlignmentChecker : MonoBehaviour
{
    [Header("Arrow Renderers")]
    public MeshRenderer arrowNegative; // Points against plane normal
    public MeshRenderer arrowPositive; // Points with plane normal

    [Header("Settings (in Metres)")]
    public float toleranceDist = 0.001f;  // 1 mm safe zone
    public float maxErrorDist  = 0.015f;  // 15 mm max red zone

    [Header("Scale Settings")]
    public Vector3 alignedScale = new Vector3(0.050f, 0.050f, 0.010f);
    public Vector3 warningScale = new Vector3(0.055f, 0.055f, 0.055f);
    public Vector3 maxScale     = new Vector3(0.080f, 0.080f, 0.080f);

    private static readonly Color Success      = Color.green;
    private static readonly Color WarningStart = new Color(1f, 0.5f, 0f, 1f);
    private static readonly Color ActiveRed    = Color.red;

    private AlignmentSensor _sensor;

    void Awake() => _sensor = GetComponent<AlignmentSensor>();

    void Update()
    {
        float error    = _sensor.OffsetErrorM;
        float absError = Mathf.Abs(error);

        Color   activeColor;
        Vector3 scale;

        if (absError <= toleranceDist)
        {
            activeColor = Success;
            scale       = alignedScale;
        }
        else
        {
            float t = Mathf.Clamp01((absError - toleranceDist) / (maxErrorDist - toleranceDist));
            activeColor = Color.Lerp(WarningStart, ActiveRed, t);
            scale       = Vector3.Lerp(warningScale, maxScale, t);
        }

        arrowNegative.transform.localScale = scale;
        arrowPositive.transform.localScale = scale;

        if (absError <= toleranceDist)
        {
            arrowNegative.enabled = true;
            arrowPositive.enabled = true;
            arrowNegative.material.color = Success;
            arrowPositive.material.color = Success;
        }
        else if (error < 0) // Tool is behind the plane → move in positive direction
        {
            arrowNegative.enabled = true;
            arrowPositive.enabled = false;
            arrowNegative.material.color = activeColor;
        }
        else // Tool is in front of the plane → move in negative direction
        {
            arrowNegative.enabled = false;
            arrowPositive.enabled = true;
            arrowPositive.material.color = activeColor;
        }
    }
}