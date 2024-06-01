using UnityEngine.UIElements;

namespace CunningFox146.Animation.Util
{
    public static class UiToolkitExtensions
    {
        public static void SetPadding(this IStyle style, float left = 0f, float right = 0f, float top = 0f, float bottom = 0f)
        {
            style.paddingLeft = left;
            style.paddingRight = right;
            style.paddingTop = top;
            style.paddingBottom = bottom;
        }
    }
}