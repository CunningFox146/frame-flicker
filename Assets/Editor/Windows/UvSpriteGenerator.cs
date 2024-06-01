using System;
using System.IO;
using System.Linq;
using Editor.Windows;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace GameEditor.Windows
{
    public class UvSpriteGenerator : EditorWindowBase
    {
        [MenuItem("Tools/Uv Sprite Generator")]
        public static void ShowWindow() 
            => GetWindow(typeof(UvSpriteGenerator), false, "Uv Sprite Generator", true);

        private Color GetRandomColor()
        {
            var colors = new[]
            {
                Color.red,
                Color.blue,
                Color.green,
                Color.yellow,
                Color.cyan,
                Color.magenta,
                Color.white,
                new Color(0.5f, 1f, 1f),
            };
            return colors[Random.Range(0, colors.Length)];
        }

        protected override void RenderWindow()
        {
            var loadSpritesButton = new Button
            {
                text = "Load all sprites",
            };
            
            loadSpritesButton.clicked += () =>
            {
                foreach (var spritePath in AssetDatabase.FindAssets("t:spriteatlas", new []{ "Assets" }).Select(AssetDatabase.GUIDToAssetPath))
                {
                    var container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Row;
                    container.style.flexGrow = 1;
                    container.style.width = new Length(100f, LengthUnit.Percent);
                    container.style.height = new Length(100f, LengthUnit.Percent);
                    
                    var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(spritePath);
                    var sprites = new Sprite[atlas.spriteCount];
                    atlas.GetSprites(sprites);
                    
                    // var importer = (TextureImporter)AssetImporter.GetAtPath(spritePath);
                    // AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
                    // EditorUtility.SetDirty(importer);
                    // importer.SaveAndReimport();

                    var atlasTexture = sprites.First().texture;
                    var texture = new Texture2D(atlasTexture.width, atlasTexture.height);
                    
                    foreach (var sprite in sprites)
                    {
                        // Debug.Log($"{sprite.name} {sprite.rect.position}");
                        var color = GetRandomColor();
                        var rect = sprite.rect;
                        
                        for (var x = 0; x < rect.width; x++)
                        for (var y = 0; y < rect.width; y++)
                        {
                            texture.SetPixel((int)rect.position.x + x,(int)rect.position.y + y, color);
                        }

                    }
                    texture.Apply();

                    var main = new Image();
                    main.style.flexGrow = 1;
                    main.image = atlasTexture;

                    var generated = new Image();
                    generated.style.flexGrow = 1;
                    generated.image = texture;
                    
                    container.Add(main);
                    container.Add(generated);
                    
                    // var sinCosTexture = new Image();
                    // sinCosTexture.image = CreateTexture(atlas.texture.width, atlas.texture.height);
                    // container.Add(sinCosTexture);
                    
                    rootVisualElement.Add(container);
                }
                
            };

            rootVisualElement.Add(loadSpritesButton);
        }
        
        private Texture2D CreateTexture(int width = 1024, int height = 1024)
        {
            var tex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var red = x / (float)width;
                var green = y / (float)height;
                var color = new Color(red, green, 0f);
                tex.SetPixel(x, y, color);
            }
            tex.Apply();
            return tex;
        }
    }
}