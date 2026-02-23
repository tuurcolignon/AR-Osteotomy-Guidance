using UnityEngine;

public class AudioController : MonoBehaviour
{
    public Transform toolTip; 
    public Transform targetPlane; 
    public AudioSource audioSource;

    public AudioSource successAudioSource; // Optional: A separate audio source for continuous success sound

    
    [Header("Thresholds")]
    public float maxDistance = 0.05f; // 5cm active zone
    public float maxAngleDeg = 30f;   // Angle where pitch hits lowest   
    
    private float nextBeepTime;
    void Update()
    {
        if (!toolTip || !targetPlane) return;

        // 1. MATH FIX: Perpendicular distance from tool tip to the cutting plane
        float distance = Mathf.Abs(Vector3.Dot(toolTip.position - targetPlane.position, targetPlane.forward));
        
        // Calculate angular deviation of the flat blades
        float angle = Vector3.Angle(toolTip.forward, targetPlane.forward); 

        // 2. AMPLITUDE: Silence if out of range
        if (distance > maxDistance)
        {
            audioSource.mute = true;
            if (successAudioSource.isPlaying)
            {
                successAudioSource.Stop();
            }
            return;
        }
        audioSource.mute = false;

        // 3. PITCH = ANGLE: 1.0 pitch is perfect. Drops to 0.5 (low pitch) as angle worsens.
        float normAngle = Mathf.Clamp01(angle / maxAngleDeg);
        audioSource.pitch = Mathf.Lerp(5.0f, 0.3f, normAngle); 

        // 4. TEMPO = DISTANCE: Geiger counter approach
        if (distance <= 0.001f && angle < 1f) // Within 1mm of the cut plane and within 1 degree of perfect alignment
        {
            if (!successAudioSource.isPlaying)
            {
                audioSource.mute = true; // Mute the regular beep sound
                // Set the exact time gap between success sounds (1.0 = 1 second)
                float successInterval = 1.0f; 
                
                if (Time.time >= nextBeepTime)
                {
                    successAudioSource.pitch = 1.2f; // You can also modulate pitch here if desired
                    successAudioSource.PlayOneShot(successAudioSource.clip);
                    nextBeepTime = Time.time + successInterval;
                }
            }
        }
        else
        {
            if (successAudioSource.isPlaying)
            {
                successAudioSource.Stop();
            }            
            audioSource.mute = false; // Unmute the regular beep sound
            // Closer = shorter interval = faster beeps
            float beepInterval = Mathf.Lerp(0.6f, 2f, distance / maxDistance);
            
            nextBeepTime = Time.time;

            if (Time.time >= nextBeepTime)
            {
                audioSource.PlayOneShot(audioSource.clip);
                nextBeepTime = Time.time + beepInterval;
            }
        }
    }
}