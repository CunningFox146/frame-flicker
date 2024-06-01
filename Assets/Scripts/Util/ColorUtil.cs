using UnityEngine;

namespace CunningFox146.Animation.Util
{
    public static class ColorUtil
    {
        public static bool IsClear(this Color color) 
            => Mathf.Approximately(color.a, 0f);
    }
}