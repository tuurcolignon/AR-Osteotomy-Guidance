using UnityEngine;

/// Central alignment sensor.
/// All modality scripts (audio, arrows, haptics) read
/// from this component instead of recomputing errors themselves.
///
/// Errors computed each frame:
///   Distance   – perpendicular distance (m) from toolTip to the cutting plane
///   RollError  – signed angle (deg) around the blade's long axis (sawBlade.right)
///   PitchError – signed angle (deg) around the blade's vertical axis (sawBlade.up)
///   OffsetError – signed lateral offset (m) of toolTip along the plane normal
/// </summary>
public class AlignmentSensor : MonoBehaviour
{
    [Header("Scene References")]
    public Transform targetPlane;
    public Transform toolTip;
    public Transform sawBlade;

    [Header("Success Thresholds")]
    [Tooltip("Perpendicular distance (m) that counts as 'on the plane'")]
    public float successDistanceM = 0.001f;   // 1 mm
    [Tooltip("Angular error (deg) that counts as 'aligned'")]
    public float successAngleDeg = 1f;

    // ── Raw outputs (read by modality scripts) ──────────────────────────────

    /// <summary>Perpendicular distance from toolTip to the cutting plane (m, always ≥ 0).</summary>
    public float Distance { get; private set; }

    /// <summary>
    /// Overall angular error between sawBlade.forward and targetPlane.forward (deg, always ≥ 0).
    /// Used by audio and haptics for coarse alignment feedback.
    /// </summary>
    public float AngleDeg { get; private set; }

    /// <summary>
    /// Signed roll error (deg) – rotation around sawBlade.right.
    /// Negative = tilt left, positive = tilt right.
    /// </summary>
    public float RollErrorDeg { get; private set; }

    /// <summary>
    /// Signed pitch error (deg) – rotation around sawBlade.up.
    /// Negative = pitch down, positive = pitch up.
    /// </summary>
    public float PitchErrorDeg { get; private set; }

    /// <summary>
    /// Signed lateral offset (m) along the plane normal.
    /// Negative = tool is behind the plane, positive = in front.
    /// </summary>
    public float OffsetErrorM { get; private set; }

    /// <summary>True when the tool is within both the distance and angle success thresholds.</summary>
    public bool IsSuccess { get; private set; }

    /// <summary>True when the tool is within the active feedback zone (Distance ≤ maxFeedbackDistance).</summary>
    public bool IsInActiveZone { get; private set; }

    [Header("Active Feedback Zone")]
    [Tooltip("Beyond this distance the modalities go silent/passive")]
    public float maxFeedbackDistanceM = 0.10f;  // 10 cm

    // ────────────────────────────────────────────────────────────────────────

    void Update()
    {
        if (!targetPlane || !toolTip || !sawBlade) return;

        Vector3 planeNormal = targetPlane.forward;

        // ── Distance & offset ───────────────────────────────────────────────
        float signedOffset = Vector3.Dot(toolTip.position - targetPlane.position, planeNormal);
        OffsetErrorM = signedOffset;
        Distance     = Mathf.Abs(signedOffset);

        // ── Coarse angle (unsigned) ─────────────────────────────────────────
        AngleDeg = Vector3.Angle(sawBlade.forward, planeNormal);

        // ── Roll error (around sawBlade.right, i.e. the long axis) ─────────
        Vector3 toolLength   = sawBlade.right;
        Vector3 idealRollDir = Vector3.ProjectOnPlane(planeNormal, toolLength);
        if (idealRollDir.sqrMagnitude >= 0.001f)
        {
            idealRollDir.Normalize();
            RollErrorDeg = Vector3.SignedAngle(sawBlade.forward, idealRollDir, toolLength);
        }

        // ── Pitch error (around sawBlade.up, i.e. the vertical axis) ───────
        Vector3 toolEdge      = sawBlade.up;
        Vector3 idealPitchDir = Vector3.ProjectOnPlane(planeNormal, toolEdge);
        if (idealPitchDir.sqrMagnitude >= 0.001f)
        {
            idealPitchDir.Normalize();
            PitchErrorDeg = Vector3.SignedAngle(sawBlade.forward, idealPitchDir, toolEdge);
        }

        // ── Derived states ──────────────────────────────────────────────────
        IsInActiveZone = Distance <= maxFeedbackDistanceM;
        IsSuccess      = Distance <= successDistanceM && AngleDeg < successAngleDeg;
    }
}