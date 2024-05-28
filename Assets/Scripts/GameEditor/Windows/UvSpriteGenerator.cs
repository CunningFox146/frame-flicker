using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace GameEditor.Windows
{
    public class UvSpriteGenerator : EditorWindowBase
    {
        [MenuItem("Tools/Uv Sprite Generator")]
        public static void ShowWindow() 
            => GetWindow(typeof(UvSpriteGenerator), false, "Uv Sprite Generator", true);

        protected override void RenderWindow()
        {
            var loadSpritesButton = new Button
            {
                text = "Load all sprites",
            };
            
            loadSpritesButton.clicked += () =>
            {
                foreach (var spritePath in AssetDatabase.FindAssets("glob:\"Assets/**/*.png\"", new []{ "Assets" }).Select(AssetDatabase.GUIDToAssetPath))
                {
                    var container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Row;
                    
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                    
                    
                    // var importer = (TextureImporter)AssetImporter.GetAtPath(spritePath);
                    // AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
                    // EditorUtility.SetDirty(importer);
                    // importer.SaveAndReimport();

                    var image = new Image();
                    image.image = sprite.texture;
                    
                    image.style.borderBottomColor = Color.red;
                    image.style.borderTopColor = Color.red;
                    image.style.borderLeftColor = Color.red;
                    image.style.borderRightColor = Color.red;
                    
                    image.style.borderBottomWidth = 1f;
                    image.style.borderTopWidth = 1f;
                    image.style.borderLeftWidth = 1f;
                    image.style.borderRightWidth = 1f;
                    
                    container.Add(image);
                    
                    var sinCosTexture = new Image();
                    sinCosTexture.image = CreateTexture(sprite.texture.width, sprite.texture.height);
                    container.Add(sinCosTexture);

                    sinCosTexture.style.borderBottomColor = Color.green;
                    sinCosTexture.style.borderTopColor = Color.green;
                    sinCosTexture.style.borderLeftColor = Color.green;
                    sinCosTexture.style.borderRightColor = Color.green;
                    sinCosTexture.style.borderBottomWidth = 1f;
                    sinCosTexture.style.borderTopWidth = 1f;
                    sinCosTexture.style.borderLeftWidth = 1f;
                    sinCosTexture.style.borderRightWidth = 1f;
                    
                    rootVisualElement.Add(container);
                }
                
            };

            rootVisualElement.Add(loadSpritesButton);
        }
        
        private Texture2D CreateTexture(int width = 1024, int height = 1024)
        {
            var tex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var red = x / (float)width;
                    var green = y / (float)height;
                    var color = new Color(red, green, 0f);
                    tex.SetPixel(x, y, color);
                }
            }
            tex.Apply();
            return tex;
        }
    }
}