using UnityEditor;

namespace GameEditor.Windows
{
    public abstract class EditorWindowBase : EditorWindow
    {
        private void CreateGUI()
        {
            RenderWindow();
        }

        protected abstract void RenderWindow();
    }
}