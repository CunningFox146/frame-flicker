using Editor.Utils;
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
            AssetUtils.RenameAssets(_selectedObjects, _inputField.value);
        }

        protected override void RenderWindow()
        {
            _inputField = new TextField();
            _inputField.style.flexGrow = 1;

            rootVisualElement.Add(_inputField);
        }

        protected override void OnEscapePressed()
        {
            base.OnEscapePressed();
            _inputField.Clear();
            _selectedObjects = null;
        }
    }
}