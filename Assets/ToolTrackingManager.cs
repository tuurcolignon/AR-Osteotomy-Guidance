using UnityEngine;

public class ToolTrackingManager : MonoBehaviour
{
    // 1. Drag your 'Tool-Tip' and 'Tool-Axis' GameObjects 
    //    from the Hierarchy onto these slots in the Inspector.
    public Transform toolTip;
    public Transform toolAxis;

    void Update()
    {
        // Check if the tool is being actively tracked
        if (toolTip == null || toolAxis == null)
        {
            Debug.LogWarning("Tool-Tip or Tool-Axis not assigned.");
            return;
        }

        // --- This is the core data for your thesis ---

        // 1. Get the WORLD position of the tool's tip
        Vector3 tipPosition = toolTip.position;

        // 2. Get the WORLD direction vector of the tool's axis
        //    (Using .forward assumes the cylinder's Z-axis is the axis.
        //     If you used X, use .right; if Y, use .up)
        Vector3 axisDirection = toolAxis.forward;

        // --- End of core data ---


        // Log the data for debugging
        // You can view this in the Unity console (when in Play mode)
        // or in the HoloLens Device Portal (when deployed)
        Debug.Log($"TOOL TIP (World): {tipPosition.ToString("F4")}");
        Debug.Log($"TOOL AXIS (World): {axisDirection.ToString("F4")}");
    }
}
