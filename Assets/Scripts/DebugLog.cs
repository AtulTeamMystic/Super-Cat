
using UnityEngine;

public static class DebugLog
{
    public static void Log(string message, string color = "yellow")
    {
        Debug.Log($"<color={color}>{message}</color>");
    }
}

