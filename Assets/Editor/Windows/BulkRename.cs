using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor.Windows
{
    public class BulkRename : EditorWindowBase
    {
        private static Object[] _selectedObjects;
        private TextField _inputField;

        [MenuItem("Assets/Bulk Rename #f2", false, 100)]
        public static void BulkRenameSelected()
        {
            if (Selection.count <= 1)
                return;
            
            _selectedObjects = Selection.objects;
            GetWindow<BulkRename>(true, "Enter file name:").Focus();
        }

        private void OnFocus()
        {
            maxSize = new Vector2(300, 25);
            maxSize = new Vector2(300, 25);
            _inputField.Focus();
        }

        private void OnDestroy()
        {
            RenameAssets(_selectedObjects, _inputField.value);
        }

        protected override void RenderWindow()
        {
            _inputField = new TextField();
            _inputField.style.flexGrow = 1;

            rootVisualElement.focusable = true;
            rootVisualElement.Add(_inputField);
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Return:
                    Close();
                    break;
                case KeyCode.Escape:
                    _inputField.Clear();
                    _selectedObjects = null;
                    Close();
                    break;
            }
        }

        private static void RenameAssets(Object[] objects, string newName)
        {
            if (objects is null || string.IsNullOrWhiteSpace(newName))
                return;

            for (var i = 0; i < objects.Length; i++)
            {
                var selectedObject = objects[i];
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedObject), $"{newName}{i}");
            }
            AssetDatabase.SaveAssets();
        }
    }
}