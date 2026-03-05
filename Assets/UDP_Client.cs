using UnityEngine;
using System.Net.Sockets;
using System.Text;

/// Sends haptic effect IDs over UDP to an ESP microcontroller.
/// Reads all alignment data from AlignmentSensor.

[RequireComponent(typeof(AlignmentSensor))]
public class HapticGuidanceController : MonoBehaviour
{
    [Header("Network Settings")]
    public string espIP = "192.168.X.X";
    public int port = 8888;

    [Header("Haptic Timing")]
    public float hapticIntervalMin = 0.2f;
    public float hapticIntervalMax = 2.0f;
    public float successHapticInterval = 1.0f;

    [Header("Angle Mapping")]
    public float maxAngleDeg = 15f;

    // Effect IDs mapped to decreasing intensity (index 0 = best alignment)
    private static readonly int[] EffectCurve = { 1, 2, 3, 8, 9 };

    private AlignmentSensor _sensor;
    private UdpClient _udpClient;
    private float _nextFireTime;

    void Start()
    {
        _sensor    = GetComponent<AlignmentSensor>();
        _udpClient = new UdpClient();
    }

    void Update()
    {
        if (!_sensor.IsInActiveZone) return;

        if (Time.time < _nextFireTime) return;

        if (_sensor.IsSuccess)
        {
            SendHapticCommand(17); // Effect 17: Distinct Double Click
            _nextFireTime = Time.time + successHapticInterval;
            return;
        }

        // Map angle to an effect in the intensity curve
        float normAngle = Mathf.Clamp01(_sensor.AngleDeg / maxAngleDeg);
        int   index     = Mathf.RoundToInt(normAngle * (EffectCurve.Length - 1));
        int   effectID  = EffectCurve[index];

        SendHapticCommand(effectID);

        float t            = _sensor.Distance / _sensor.maxFeedbackDistanceM;
        float hapticInterval = Mathf.Lerp(hapticIntervalMin, hapticIntervalMax, t);
        _nextFireTime      = Time.time + hapticInterval;

        Debug.Log($"Distance: {_sensor.Distance:F3} m | Angle: {_sensor.AngleDeg:F1} deg | Effect: {effectID} | Next in: {hapticInterval:F2} s");
    }

    private void SendHapticCommand(int effectID)
    {
        byte[] data = System.Text.Encoding.UTF8.GetBytes(effectID.ToString());
        _udpClient.Send(data, data.Length, espIP, port);
    }

    void OnApplicationQuit()
    {
        _udpClient?.Close();
    }
}