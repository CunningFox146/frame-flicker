using System.Collections.Generic;
using System.IO;
using System.Linq;
using CunningFox146.Animation.Util;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Windows
{
    public class AnimationCreator : EditorWindowBase
    {
        private static List<Texture2D> _selectedTextures = new();

        [MenuItem("Assets/Create Animation", false, 101)]
        public static void CreateAnimation()
        {
            _selectedTextures = GetSelectedObjects()?.Where(e => e is Texture2D).Cast<Texture2D>().ToList();
            ConvertTexturesToSprites(_selectedTextures);
            GetWindow<AnimationCreator>("Animation generator").Focus();
        }

        protected override void RenderWindow()
        {
            var nameLength = Length.Percent(10);
            var toggleLength = Length.Percent(5);
            
            var listView = new ListView
            {
                itemsSource = _selectedTextures,
                selectionType = SelectionType.Multiple,
                fixedItemHeight = 64,
                
                makeItem = () =>
                {
                    var root = new VisualElement();
                    var label = new Label();
                    var toggle = new Toggle();
                    var image = new Image();
                    var uvOverlay = new Image();
                    var uvPreview = new Image();

                    root.style.flexDirection = FlexDirection.Row;
                    root.style.SetPadding(4, 4, 4, 4);
                    
                    label.style.unityTextAlign = TextAnchor.MiddleLeft;
                    label.style.whiteSpace = WhiteSpace.Normal;
                    label.style.width = nameLength;

                    toggle.style.width = toggleLength;
                    image.style.flexGrow = 1;
                    
                    uvOverlay.AddToClassList("uv");
                    uvOverlay.AddToClassList("uvOverlay");
                    uvOverlay.style.position = Position.Absolute;
                    uvOverlay.style.flexGrow = 1;
                    uvOverlay.style.width = Length.Percent(100);
                    uvOverlay.style.height = Length.Percent(100);
                    uvOverlay.visible = false;
                    
                    uvPreview.AddToClassList("uv");
                    uvPreview.style.flexGrow = 1;
                    
                    root.Add(label);
                    root.Add(toggle);
                    root.Add(image);
                    root.Add(uvPreview);
                    image.Add(uvOverlay);
                    return root;
                },

                bindItem = (element, idx) =>
                {
                    var texture = _selectedTextures[idx];
                    var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
                    var uvPreview = element.Q<Image>(className: "uvOverlay");
                    
                    element.Q<Label>().text = texture.name;
                    element.Q<Image>().image = texture;
                    element.Q<Toggle>().RegisterValueChangedCallback(val => uvPreview.visible = val.newValue);
                    
                    var uv = CreateUvTexture(texture);
                    foreach (var image in element.Query<Image>(className: "uv").ToList())
                    {
                        image.image = uv;
                    }

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

            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            
            header.Add(new Label("Texture Name")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    width = nameLength
                }
            });
            
            header.Add(new Label("Show UV")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    width = toggleLength
                }
            });
            
            header.Add(new Label("Source Texture")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    flexGrow = 1
                }
            });
            
            header.Add(new Label("UV Texture")
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    flexGrow = 1
                }
            });

            foldout.Add(header);
            foldout.Add(listView);
            
            rootVisualElement.Add(foldout);
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
                {
                    texture.SetPixel(x, y, new Color(0, 0, 0, 0 ));
                    continue;
                }
                
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

        private static IEnumerable<Object> GetSelectedObjects()
        {
            IEnumerable<Object> selectedObjects = null;

            if (Selection.objects.Length > 1)
                selectedObjects = Selection.objects;
            else
            {
                var folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
                if (Directory.Exists(folderPath))
                {
                    var files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly);
                    selectedObjects = files.Select(AssetDatabase.LoadAssetAtPath<Object>).ToList();
                }
            }

            return selectedObjects;
        }
    }
}