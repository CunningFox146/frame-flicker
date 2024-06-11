using System.Collections.Generic;
using System.IO;
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
        private const string UvMaskName = "_UvMask";
        private const string UvFolderName = "UVs";
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
            
            GetWindow<AnimationCreator>("Animation generator").Focus();
        }

        private void OnFocus() 
            => rootVisualElement.Focus();

        protected override void RenderWindow()
        {
            var nameLength = Length.Percent(10);
            var toggleLength = Length.Percent(5);
            
            var listView = RenderListView(nameLength, toggleLength);
            var header = RenderHeader(nameLength, toggleLength);

            var textField = new TextField("Animation name");
            var button = new Button();
            button.text = "Generate";
            button.clicked += StartAnimationGeneration;
            
            var foldout = new Foldout();
            foldout.text = "Textures";
            foldout.value = false;

            foldout.Add(header);
            foldout.Add(listView);

            rootVisualElement.Add(textField);
            rootVisualElement.Add(button);
            rootVisualElement.Add(foldout);
        }

        private void StartAnimationGeneration()
        {
            var animationName = rootVisualElement.Q<TextField>().value;
            if (string.IsNullOrWhiteSpace(animationName))
                return;
            
            rootVisualElement.Q<Foldout>().value = true;
            
            AssetUtils.RenameAssets(_selectedTextures.Cast<Object>().ToArray(), animationName);
            SetupTextures(_selectedTextures);
            GenerateUvMasks(_selectedTextures);
            CleanupTextures(_selectedTextures);
            AssetDatabase.Refresh();
        }

        private static void GenerateUvMasks(List<Texture2D> selectedTextures)
        {
            var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_selectedTextures.FirstOrDefault()));
            var folderPath = $"{assetPath}/{UvFolderName}";

            if (!Directory.Exists(folderPath))
                AssetDatabase.CreateFolder(assetPath, UvFolderName);
            else
                foreach (var file in Directory.GetFiles(folderPath))
                    File.Delete(file);

            AssetDatabase.Refresh();
            
            foreach (var texture in selectedTextures)
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
                importer.secondarySpriteTextures = new[]
                {
                    new SecondarySpriteTexture
                    {
                        texture = GenerateUvMask(texture),
                        name = UvMaskName
                    }
                };
            }
        }

        private static Texture2D GenerateUvMask(Texture2D sourceTexture)
        {
            var texture = TextureUtils.CreateUvTexture(sourceTexture);
            var assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(sourceTexture));
            var folderPath = $"{assetPath}/{UvFolderName}";
            var filePath = $"{folderPath}/{sourceTexture.name}_UV.png";

            var pngBytes = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, pngBytes);
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();

            var importer = (TextureImporter)AssetImporter.GetAtPath(filePath);
            importer.filterMode = FilterMode.Point;
            importer.alphaSource = TextureImporterAlphaSource.None;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
            
            return AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
        }

        private static void SetupTextures(List<Texture2D> textures)
        {
            foreach (var importer in textures.Select(texture => AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture))))
                if (importer is  TextureImporter textureImporter)
                   SetupSpriteImporter(textureImporter);
        }

        private static void SetupSpriteImporter(TextureImporter importer)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.filterMode = FilterMode.Point;

            importer.compressionQuality = 100;
            importer.crunchedCompression = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.isReadable = true;
            importer.spritePixelsPerUnit = 32;
            importer.spriteImportMode = SpriteImportMode.Single;
                
            importer.SaveAndReimport();
        }
        
        private static void CleanupTextures(List<Texture2D> textures)
        {
            foreach (var importer in textures.Select(texture => AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture))))
                if (importer is  TextureImporter textureImporter)
                    CleanupTexture(textureImporter, importer);
        }

        private static void CleanupTexture(TextureImporter textureImporter, AssetImporter importer)
        {
            textureImporter.isReadable = false;
            AssetDatabase.ImportAsset(importer.assetPath, ImportAssetOptions.ForceUpdate);
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
                    uvOverlay.style.opacity = 0.5f;
                    
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
            => StartAnimationGeneration();

        protected override void OnEscapePressed()
        {
            _selectedTextures = null;
            base.OnEscapePressed();
        }
    }
}