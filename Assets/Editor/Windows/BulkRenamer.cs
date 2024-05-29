using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GameEditor.Windows
{
    public class BulkRenamer : EditorWindow
    {
        private static Object[] _selectedObjects;
        private TextField _inputField;

        [MenuItem("Assets/Bulk Rename #f2", false, 100)]
        public static bool BulkRename()
        {
            if (Selection.count <= 1)
                return false;
            
            _selectedObjects = Selection.objects;
            GetWindow<BulkRenamer>(true, "Enter file name:");
            FocusWindowIfItsOpen<BulkRenamer>();
            
            return true;
        }

        private void OnFocus()
        {
            maxSize = new Vector2(300, 25);
            maxSize = new Vector2(300, 25);
            _inputField.Focus();
        }

        private void OnDestroy()
        {
            if (_selectedObjects is null)
                return;
            
            var newName = _inputField.value;
            if (string.IsNullOrWhiteSpace(newName))
                return;

            for (var i = 0; i < _selectedObjects.Length; i++)
            {
                var selectedObject = _selectedObjects[i];
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedObject), $"{newName}{i}");
            }
            AssetDatabase.SaveAssets();
        }

        private void CreateGUI()
        {
            var root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.flexDirection = FlexDirection.Row;
            
            _inputField = new TextField();
            _inputField.style.flexGrow = 1;

            var button = new Button(Close);
            button.text = "Rename";
            
            root.Add(_inputField);
            root.Add(button);
            rootVisualElement.Add(root);
            
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return)
                Close();
        }
    }
}