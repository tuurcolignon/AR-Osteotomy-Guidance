using UnityEngine;

/// <summary>
/// Sliding bar indicator for translational offset feedback.
///
/// UNITY SETUP
/// -----------
/// Create three child GameObjects under your cutting tool with simple meshes:
///
///   Bar        – a flat, horizontal capsule or scaled cube (the track).
///                Suggested local scale: (0.06, 0.004, 0.004) for a 6 cm bar.
///
///   Marker     – a thin, vertical capsule or scaled cube (the sliding crossbar).
///                Suggested local scale: (0.003, 0.015, 0.004).
///                Position it at the bar centre in the editor — the script
///                will slide it along local X at runtime.
///
///   CentreLine – identical shape to Marker, does NOT move.
///                Stays fixed at the bar centre as the target reference.
///
/// Assign all three MeshRenderers in the Inspector, then place this script
/// (along with AlignmentSensor) on the same GameObject.
///
/// HOW IT WORKS
/// ------------
///   error == 0   -> marker sits exactly on the centre line (aligned)
///   error > 0    -> marker slides right (tool in front of the plane)
///   error < 0    -> marker slides left  (tool behind the plane)
///   Bar + marker tint green -> orange -> red as error grows.
///   Everything hidden when outside the active feedback zone.
/// </summary>
[RequireComponent(typeof(AlignmentSensor))]
public class TranslationBarIndicator : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The long horizontal bar mesh (the track).")]
    public MeshRenderer bar;

    [Tooltip("The sliding vertical crossbar mesh (current position marker).")]
    public MeshRenderer marker;

    [Tooltip("Fixed vertical line at the bar centre (the target position).")]
    public MeshRenderer centreLine;

    [Header("Bar Settings")]
    [Tooltip("Half-length of the bar in world units. " +
             "The marker is clamped here so it never exits the bar.")]
    public float barHalfLength = 0.03f;   // 3 cm each side -> 6 cm total bar

    [Tooltip("Distance error (m) that maps to the full end of the bar.")]
    public float maxErrorDist = 0.015f;   // 15 mm

    [Tooltip("Error (m) within which the indicator shows success (green).")]
    public float toleranceDist = 0.001f;  // 1 mm

    [Header("Colors")]
    public Color successColor    = Color.green;
    public Color warningColor    = new Color(1f, 0.5f, 0f, 1f);
    public Color errorColor      = Color.red;
    [Tooltip("The centre reference line stays this color at all times.")]
    public Color centreLineColor = Color.white;

    // -------------------------------------------------------------------------

    private AlignmentSensor _sensor;
    private Vector3 _markerOriginLocal;

    void Awake()
    {
        _sensor = GetComponent<AlignmentSensor>();
        // Capture the marker's designed centre position once at startup.
        // Place the Marker object at the bar centre in the editor.
        _markerOriginLocal = marker.transform.localPosition;
    }

    void Update()
    {
        bool active = _sensor.IsInActiveZone;
        bar.enabled        = active;
        marker.enabled     = active;
        centreLine.enabled = active;

        if (!active) return;

        float error    = _sensor.OffsetErrorM;
        float absError = Mathf.Abs(error);

        // ── Marker position ──────────────────────────────────────────────────
        float tPos                     = Mathf.Clamp(error / maxErrorDist, -1f, 1f);
        Vector3 markerPos              = _markerOriginLocal;
        markerPos.x                    = tPos * barHalfLength;
        marker.transform.localPosition = markerPos;

        // ── Bar + marker color ───────────────────────────────────────────────
        Color activeColor;
        if (absError <= toleranceDist)
        {
            activeColor = successColor;
        }
        else
        {
            float tCol  = Mathf.Clamp01((absError - toleranceDist) /
                                        (maxErrorDist - toleranceDist));
            activeColor = Color.Lerp(warningColor, errorColor, tCol);
        }

        bar.material.color        = activeColor;
        //marker.material.color     = activeColor;
        centreLine.material.color = activeColor;
    }
}