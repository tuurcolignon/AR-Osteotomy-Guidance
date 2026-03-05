using UnityEngine;

/// <summary>
/// Pitch correction arrows – show which way to rotate the blade around its vertical axis.
/// Reads PitchErrorDeg from AlignmentSensor.
///
/// Note: This script was previously called "YawAlignmentChecker" but computes
/// pitch error (rotation around sawBlade.up). The class is renamed for clarity.
/// </summary>
[RequireComponent(typeof(AlignmentSensor))]
public class PitchAlignmentChecker : MonoBehaviour
{
    [Header("Arrow Renderers")]
    public MeshRenderer arrowLeft;
    public MeshRenderer arrowRight;

    [Header("Settings")]
    public float toleranceDeg = 1f;
    public float maxErrorDeg  = 15f;

    [Header("Scale Settings")]
    public Vector3 alignedScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 warningScale = new Vector3(1.2f, 1.2f, 1.2f);
    public Vector3 maxScale     = new Vector3(2.5f, 2.5f, 2.5f);

    private static readonly Color Success      = Color.green;
    private static readonly Color WarningStart = new Color(1f, 0.5f, 0f, 1f);
    private static readonly Color ActiveRed    = Color.red;

    private AlignmentSensor _sensor;

    void Awake() => _sensor = GetComponent<AlignmentSensor>();

    void Update()
    {
        float error    = _sensor.PitchErrorDeg;
        float absError = Mathf.Abs(error);

        Color   activeColor;
        Vector3 scale;

        if (absError <= toleranceDeg)
        {
            activeColor = Success;
            scale       = alignedScale;
        }
        else
        {
            float t = Mathf.Clamp01((absError - toleranceDeg) / (maxErrorDeg - toleranceDeg));
            activeColor = Color.Lerp(WarningStart, ActiveRed, t);
            scale       = Vector3.Lerp(warningScale, maxScale, t);
        }

        arrowLeft.transform.localScale  = scale;
        arrowRight.transform.localScale = scale;

        if (absError <= toleranceDeg)
        {
            arrowLeft.enabled  = true;
            arrowRight.enabled = true;
            arrowLeft.material.color  = Success;
            arrowRight.material.color = Success;
        }
        else if (error < 0)
        {
            arrowLeft.enabled  = true;
            arrowRight.enabled = false;
            arrowLeft.material.color = activeColor;
        }
        else
        {
            arrowLeft.enabled  = false;
            arrowRight.enabled = true;
            arrowRight.material.color = activeColor;
        }
    }
}