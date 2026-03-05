using UnityEngine;

[RequireComponent(typeof(AlignmentSensor))]
public class RotationDialIndicator : MonoBehaviour
{
    public enum DialAxis { Roll, Pitch }

    [Header("Axis")]
    public DialAxis dialAxis = DialAxis.Roll;

    [Header("References")]
    public MeshRenderer ring;
    public MeshRenderer targetRect;
    public MeshRenderer toolRect;
    public Transform targetPlane;

    [Header("Rotation Arrows")]
    public GameObject arrowNegative;
    public GameObject arrowPositive;

    [Header("Error Thresholds")]
    public float toleranceDeg = 1f;
    public float maxErrorDeg  = 15f;

    [Header("Colors")]
    public Color successColor = Color.green;
    public Color warningColor = new Color(1f, 0.5f, 0f, 1f);
    public Color errorColor   = Color.red;

    private AlignmentSensor _sensor;
    private Vector3 _faceNormal;

    void Awake()
    {
        _sensor     = GetComponent<AlignmentSensor>();
        _faceNormal = ring.transform.forward;
    }

    void Update()
    {
        bool active = _sensor.IsInActiveZone;
        ring.enabled       = active;
        targetRect.enabled = active;
        toolRect.enabled   = active;

        if (!active)
        {
            if (arrowNegative) arrowNegative.SetActive(false);
            if (arrowPositive) arrowPositive.SetActive(false);
            return;
        }

        float signedError = (dialAxis == DialAxis.Roll)
            ? _sensor.RollErrorDeg
            : -_sensor.PitchErrorDeg;

        float absError = Mathf.Abs(signedError);

        // ── Target rect ───────────────────────────────────────────────────────
        Vector3 idealDir = Vector3.ProjectOnPlane(targetPlane.forward, _faceNormal);
        if (idealDir.sqrMagnitude < 0.001f) return;
        idealDir.Normalize();

        Quaternion offset = Quaternion.AngleAxis(90f, _faceNormal);
        targetRect.transform.rotation = offset * Quaternion.LookRotation(_faceNormal, idealDir);

        // ── Tool rect ─────────────────────────────────────────────────────────
        toolRect.transform.rotation = Quaternion.AngleAxis(-signedError, _faceNormal)
                                      * targetRect.transform.rotation;

        // ── Arrows ────────────────────────────────────────────────────────────
        if (arrowNegative && arrowPositive)
        {
            if (absError <= toleranceDeg)
            {
                arrowNegative.SetActive(false);
                arrowPositive.SetActive(false);
            }
            else if (signedError < 0)
            {
                arrowNegative.SetActive(true);
                arrowPositive.SetActive(false);
            }
            else
            {
                arrowNegative.SetActive(false);
                arrowPositive.SetActive(true);
            }
        }

        // ── Colors ────────────────────────────────────────────────────────────
        Color activeColor;
        if (absError <= toleranceDeg)
        {
            activeColor = successColor;
        }
        else
        {
            float t     = Mathf.Clamp01((absError - toleranceDeg) / (maxErrorDeg - toleranceDeg));
            activeColor = Color.Lerp(warningColor, errorColor, t);
        }

        ring.material.color = activeColor;
    }
}