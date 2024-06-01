using System.Collections.Generic;
using System.Linq;
using CunningFox146.Animation.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor.Windows
{
    public class AnimationCreator : EditorWindowBase
    {
        private static List<Texture2D> _selectedTextures = new();

        [MenuItem("Assets/Create Animation", false, 101)]
        public static void CreateAnimation()
        {
            _selectedTextures = Selection.objects.Where(e => e is Texture2D).Cast<Texture2D>().ToList();
            ConvertTexturesToSprites(_selectedTextures);
            GetWindow<AnimationCreator>("Animation generator").Focus();
        }

        protected override void RenderWindow()
        {
            var listView = new ListView
            {
                itemsSource = _selectedTextures,
                selectionType = SelectionType.Multiple,
                fixedItemHeight = 64,
                
                makeItem = () =>
                {
                    var root = new VisualElement();
                    var label = new Label();
                    var image = new Image();
                    var uvPreview = new Image();

                    root.style.flexDirection = FlexDirection.Row;
                    root.style.SetPadding(4, 4, 4, 4);
                    
                    label.style.unityTextAlign = TextAnchor.MiddleLeft;
                    
                    image.style.flexGrow = 1;
                    uvPreview.style.flexGrow = 1;
                    uvPreview.name = "uvPreview";
                    
                    root.Add(label);
                    root.Add(image);
                    root.Add(uvPreview);
                    return root;
                },

                bindItem = (element, idx) =>
                {
                    var texture = _selectedTextures[idx];
                    var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
                    var uvPreview = element.Q<Image>("uvPreview");
                    
                    element.Q<Label>().text = texture.name;
                    element.Q<Image>().image = texture;
                    uvPreview.image = CreateUvTexture(texture);

                    // if (importer.secondarySpriteTextures.Any())
                    // {
                    //     var secondaryTexture = importer.secondarySpriteTextures.First();
                    //     uvPreview.image = secondaryTexture.texture;
                    // }
                    // else
                    // {
                    //     uvPreview.style.backgroundColor = Color.magenta;
                    // }
                },


                style =
                {
                    flexGrow = 1
                }
            };
            
            var foldout = new Foldout();
            foldout.text = "Textures";

            rootVisualElement.Add(foldout);
            foldout.Add(listView);
            
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        }

        private static void ConvertTexturesToSprites(List<Texture2D> textures)
        {
            Undo.IncrementCurrentGroup();
            
            foreach (var importer in textures.Select(texture => AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture))))
            {
                if (importer is not TextureImporter textureImporter)
                    continue;
                
                Undo.RecordObject(importer, $"Texture convert {importer.name}");
                SetupSpriteImporter(textureImporter);
            }
        }

        private static void SetupSpriteImporter(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.filterMode = FilterMode.Point;

            importer.compressionQuality = 75;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.crunchedCompression = true;
            importer.isReadable = true;
            importer.spritePixelsPerUnit = 32;
                
            importer.SaveAndReimport();
        }
        
        private Texture2D CreateUvTexture(Texture2D sourceTexture)
        {
            var width = sourceTexture.width;
            var height = sourceTexture.height;
            var texture = new Texture2D(width, height);
            
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var sourceColor = sourceTexture.GetPixel(x, y);
                if (Mathf.Approximately(sourceColor.a,  0f))
                    continue;
                
                var red = x / (float)width;
                var green = y / (float)height;
                var color = new Color(red, green, 0f);
                
                texture.SetPixel(x, y, color);
            }
            texture.Apply();
            return texture;
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Return:
                    Close();
                    break;
                case KeyCode.Escape:
                    _selectedTextures = null;
                    Close();
                    break;
            }
        }
    }
}