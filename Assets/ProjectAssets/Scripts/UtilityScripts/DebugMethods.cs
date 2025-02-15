using UnityEngine;

public static class DebugMethods
{
    public static void DrawLineWithLabel(GameObject startObj, GameObject endObj, string label, Color lineColor, Color textColor, Vector3 offset)
    {
        if (startObj == null || endObj == null) return;

        Debug.DrawLine(startObj.transform.position, endObj.transform.position, lineColor);
        DebugGizmoDrawer.AddDebugLine
        (
            startObj.transform.position,
            endObj.transform.position,
            label,
            lineColor,
            textColor,
            offset
        );
    }
}
