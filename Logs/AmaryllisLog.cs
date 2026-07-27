using UnityEngine;

namespace Amaryllis.Logs
{
    public static class AmaryllisLog
    {
        public static bool IsLogEnable = false;
        
        public static void Log(string message)
        {
            if (IsLogEnable)
            {
                Debug.Log($"[Amaryllis] {message}");
            }
        }
    }
}