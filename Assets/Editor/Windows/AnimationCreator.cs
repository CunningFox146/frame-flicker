using System.Collections.Generic;
using System.Linq;
using CunningFox146.Animation.Util;
using Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Windows
{
    public class AnimationCreator : EditorWindowBase
    {
        private const string UvOverlayClass = "uvOverlay";
        private const string UvClass = "uv";
        
        private static List<Texture2D> _selectedTextures = new();

        [MenuItem("Assets/Create Animation", false, 101)]
        public static void CreateAnimation()
        {
            var selectedObjects = AssetUtils.GetSelectedObjects();
            _selectedTextures = selectedObjects?.Where(e => e is Texture2D).Cast<Texture2D>().ToList();

            if (_selectedTextures is null || !_selectedTextures.Any())
                return;
            
            // ConvertTexturesToSprites(_selectedTextures);
            GetWindow<AnimationCreator>("Animation generator").Focus();
        }

        private void OnFocus()
        {
            rootVisualElement.Focus();
        }

        protected override void RenderWindow()
        {
            var nameLength = Length.Percent(10);
            var toggleLength = Length.Percent(5);
            
            var listView = RenderListView(nameLength, toggleLength);
            var header = RenderHeader(nameLength, toggleLength);
            
            var foldout = new Foldout();
            foldout.text = "Textures";

            foldout.Add(header);
            foldout.Add(listView);

            rootVisualElement.Add(foldout);
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

        private static ListView RenderListView(Length nameLength, Length toggleLength)
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
                    
                    uvOverlay.AddToClassList(UvClass);
                    uvOverlay.AddToClassList(UvOverlayClass);
                    uvOverlay.style.position = Position.Absolute;
                    uvOverlay.style.flexGrow = 1;
                    uvOverlay.style.width = Length.Percent(100);
                    uvOverlay.style.height = Length.Percent(100);
                    uvOverlay.visible = false;
                    
                    uvPreview.AddToClassList(UvClass);
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
                    var uvPreview = element.Q<Image>(className: UvOverlayClass);
                    
                    element.Q<Label>().text = texture.name;
                    element.Q<Image>().image = texture;
                    element.Q<Toggle>().RegisterValueChangedCallback(val => uvPreview.visible = val.newValue);
                    
                    // var uv = CreateUvTexture(texture);
                    if (importer.secondarySpriteTextures.Any())
                    {
                        var secondaryTexture = importer.secondarySpriteTextures.First();
                        foreach (var image in element.Query<Image>(className: UvClass).ToList())
                        {
                            image.image = secondaryTexture.texture;
                            image.style.backgroundColor = Color.clear;
                        }
                    }
                    else
                    {
                        foreach (var image in element.Query<Image>(className: UvClass).ToList())
                        {
                            image.image = null;
                            image.style.backgroundColor = Color.magenta;
                        }
                    }
                },

                style =
                {
                    flexGrow = 1
                }
            };
            return listView;
        }

        private static VisualElement RenderHeader(Length nameLength, Length toggleLength)
        {
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
            
            header.Add(new Label("Overlay UV")
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
            return header;
        }

        protected override void OnEnterPressed() 
            => Close();

        protected override void OnEscapePressed()
        {
            _selectedTextures = null;
            base.OnEscapePressed();
        }
    }
}