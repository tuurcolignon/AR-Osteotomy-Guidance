using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class MonitorPositioner : MonoBehaviour
{
    void Start()
    {
        // Add MRTK ObjectManipulator component
        ObjectManipulator manipulator = gameObject.AddComponent<ObjectManipulator>();
        
        // Configure for positioning
        manipulator.AllowFarManipulation = true;
        manipulator.HostTransform = transform.parent; // Keep relative to gun
        manipulator.SmoothingFar = true;
        manipulator.SmoothingNear = true;
        
        // Add collider if not present
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        }
        
        // Add NearInteractionGrabbable for hand interaction
        gameObject.AddComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>();
        
        Debug.Log("Monitor ready - Hold SPACE + move mouse, then click to grab");
    }
}