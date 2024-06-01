using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.Utils
{
    public static class AssetUtils
    {
        public static void RenameAssets(Object[] objects, string newName)
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
        
        public static IEnumerable<Object> GetSelectedObjects()
        {
            IEnumerable<Object> selectedObjects = null;

            var folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if (Directory.Exists(folderPath))
            {
                var files = Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly);
                selectedObjects = files.Select(AssetDatabase.LoadAssetAtPath<Object>).ToList();
            }
            else if (Selection.objects.Length > 0)
                selectedObjects = Selection.objects;

            return selectedObjects;
        }
    }
}