using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugGizmoDrawer : MonoBehaviour
{
    public struct DebugLine
    {
        public Vector3 start;
        public Vector3 end;
        public string label;
        public Color lineColor;
        public Color textColor;
        public Vector3 offset;

        public DebugLine(Vector3 start, Vector3 end, string label, Color lineColor, Color textColor, Vector3 offset)
        {
            this.start = start;
            this.end = end;
            this.label = label;
            this.lineColor = lineColor;
            this.textColor = textColor;
            this.offset = offset;
        }
    }

    private static List<DebugLine> debugLines = new List<DebugLine>();

    public static void AddDebugLine(Vector3 start, Vector3 end, string label, Color lineColor, Color textColor, Vector3 offset)
    {
        debugLines.Add(new DebugLine(start, end, label, lineColor, textColor, offset));
    }

    public static void ClearDebugLines()
    {
        debugLines.Clear();
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        for (int i = 0; i < debugLines.Count; ++i)
        {
            DebugLine line = debugLines[i];
            Gizmos.color = line.lineColor;
            Gizmos.DrawLine(line.start, line.end);

            Vector3 midPoint = (line.start + line.end) / 2f + line.offset;
            Handles.color = line.textColor;
            Handles.Label(midPoint, line.label);
        }
        #endif
    }
}