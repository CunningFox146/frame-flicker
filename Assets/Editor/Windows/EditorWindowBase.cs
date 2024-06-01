using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Windows
{
    public abstract class EditorWindowBase : EditorWindow
    {
        protected abstract void RenderWindow();
        protected virtual void OnEnterPressed() { }

        protected virtual void OnEscapePressed() 
            => Close();

        private void CreateGUI()
        {
            RenderWindow();

            rootVisualElement.focusable = true;
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Return:
                    OnEnterPressed();
                    break;
                case KeyCode.Escape:
                    OnEscapePressed();
                    break;
            }
        }
    }
}