using Unity.Mathematics;
using UnityEngine;

namespace Editor.Utils
{
    public static class TextureUtils
    {
        private const float MinAlpha = 0.05f;
        
        public static Texture2D CreateUvTexture(Texture2D sourceTexture, bool useBounds = false)
        {
            var width = sourceTexture.width;
            var height = sourceTexture.height;
            var texture = new Texture2D(width, height, TextureFormat.RGB565, false);


            if (useBounds)
            {
                var (boundMin, boundMax) = GetTextureBounds(sourceTexture);
                
                for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, Color.black);
                }

                for (var x = boundMin.x; x <= boundMax.x; x++)
                for (var y = boundMin.y; y <= boundMax.y; y++)
                {
                    var red = math.remap(boundMin.x, boundMax.x, 0f, 1f, x);
                    var green = math.remap(boundMin.y, boundMax.y, 0f, 1f, y);
                    texture.SetPixel(x, y, new Color(red, green, 0f));
                }
            }
            else
            {
                for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    var red = x / (float)width;
                    var green = y / (float)height;
                    texture.SetPixel(x, y, new Color(red, green, 0f));
                }
            }

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();
            return texture;
        }

        public static (Vector2Int min, Vector2Int max) GetTextureBounds(Texture2D sourceTexture)
        {
            var min = Vector2Int.one * int.MaxValue;
            var max = Vector2Int.one * int.MinValue;
            
            for (var x = 0; x < sourceTexture.width; x++)
            for (var y = 0; y < sourceTexture.height; y++)
            {
                var sourceColor = sourceTexture.GetPixel(x, y);
                if (sourceColor.a > MinAlpha)
                {
                    if (min.x > x)
                        min.x = x;
                    
                    if (min.y > y)
                        min.y = y;
                    
                    if (max.x < x)
                        max.x = x;
                    
                    if (max.y < y)
                        max.y = y;
                }
            }

            return (min, max);
        }
    }
}