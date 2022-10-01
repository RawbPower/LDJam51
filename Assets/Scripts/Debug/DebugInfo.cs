using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInfo
{
    static float sm_LengthScale = 17;
    static float sm_WidthScale = 8;

    public string debugText;
    public int debugLines;
    public int debugCharPerLine;

    public DebugInfo()
    {
        ResetDebug();
    }

    public void AddDebugLine(string debugLine)
    {
        debugText += debugLine + "\n";
        debugLines++;
        debugCharPerLine = Mathf.Max(debugLine.Length, debugCharPerLine);
    }

    public void ResetDebug()
    {
        debugText = "";
        debugLines = 0;
        debugCharPerLine = 0;
    }

    public void CreateTextBox(float x, float y)
    {
        GUI.Box(new Rect(x, y, debugCharPerLine * sm_WidthScale, debugLines * sm_LengthScale), debugText);
    }
}
