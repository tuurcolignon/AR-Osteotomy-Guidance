using UnityEngine;

/// Drives audio feedback (beeping + success tone) based on alignment data
/// supplied by AlignmentSensor.

[RequireComponent(typeof(AlignmentSensor))]
public class AudioController : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource beepAudioSource;
    public AudioSource successAudioSource;

    [Header("Pitch Mapping")]
    [Tooltip("Pitch when angle error = 0 (perfect alignment)")]
    public float pitchAtPerfect = 3.0f;
    [Tooltip("Pitch when angle error >= maxAngleDeg")]
    public float pitchAtWorst   = 0.2f;
    [Tooltip("Angle (deg) at which pitch reaches its lowest value")]
    public float maxAngleDeg    = 15f;

    [Header("Tempo Mapping")]
    public float beepIntervalMin = 0.2f;   // fastest beep (right at the plane)
    public float beepIntervalMax = 2.0f;   // slowest beep (at edge of active zone)

    [Header("Success Sound")]
    public float successInterval = 1.0f;

    private AlignmentSensor _sensor;
    private float _nextBeepTime;
    private float _nextSuccessTime;

    void Awake()
    {
        _sensor = GetComponent<AlignmentSensor>();
    }

    void Update()
    {
        if (!_sensor.IsInActiveZone)
        {
            beepAudioSource.mute = true;
            successAudioSource.Stop();
            return;
        }

        if (_sensor.IsSuccess)
        {
            beepAudioSource.mute = true;

            if (Time.time >= _nextSuccessTime)
            {
                successAudioSource.PlayOneShot(successAudioSource.clip);
                _nextSuccessTime = Time.time + successInterval;
            }
            return;
        }

        // Regular proximity beep
        successAudioSource.Stop();
        beepAudioSource.mute = false;

        float normAngle = Mathf.Clamp01(_sensor.AngleDeg / maxAngleDeg);
        beepAudioSource.pitch = Mathf.Lerp(pitchAtPerfect, pitchAtWorst, normAngle);

        if (Time.time >= _nextBeepTime)
        {
            float t = _sensor.Distance / _sensor.maxFeedbackDistanceM;
            float beepInterval = Mathf.Lerp(beepIntervalMin, beepIntervalMax, t);
            beepAudioSource.PlayOneShot(beepAudioSource.clip);
            _nextBeepTime = Time.time + beepInterval;
        }
    }
}