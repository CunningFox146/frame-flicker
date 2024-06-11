using CunningFox146.Animation.Animation;
using Editor.Windows;
using UnityEditor;
using UnityEngine.UIElements;

namespace CunningFox146.Animation.Editor
{
    [CustomEditor(typeof(FrameAnimation), true)]
    public class FrameAnimationEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
            root.Add(new Label("Frame Animation Editor"));
            return root;
        }
    }
}
