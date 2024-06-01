using UnityEngine;
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
        
        public static void SetBorderWidth(this IStyle style, float left, float right, float top, float bottom)
        {
            style.borderLeftWidth = left;
            style.borderRightWidth = right;
            style.borderTopWidth = top;
            style.borderBottomWidth = bottom;
        }

        public static void SetBorderWidth(this IStyle style, float width)
            => SetBorderWidth(style, width, width, width, width);
        
        public static void SetBorderColor(this IStyle style, Color left, Color right, Color top, Color bottom)
        {
            style.borderLeftColor = left;
            style.borderRightColor = right;
            style.borderTopColor = top;
            style.borderBottomColor = bottom;
        }

        public static void SetBorderColor(this IStyle style, Color color)
            => SetBorderColor(style, color, color, color, color);
    }
}