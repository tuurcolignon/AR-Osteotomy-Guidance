using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class IndicatorPositioner : MonoBehaviour
{
    void Start()
    {
        ObjectManipulator manipulator = gameObject.AddComponent<ObjectManipulator>();
        
        manipulator.AllowFarManipulation = true;
        manipulator.HostTransform = transform;
        manipulator.SmoothingFar = true;
        manipulator.SmoothingNear = true;
        
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
        
        gameObject.AddComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>();
    }
}