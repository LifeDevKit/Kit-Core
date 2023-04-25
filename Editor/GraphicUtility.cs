using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Kit.Editor
{
    [CreateAssetMenu(fileName = "EditorResources", menuName = "BOM/EditorResources", order = 1)]
    public class EditorResourceObject : ScriptableObject
    {
        [FormerlySerializedAs("register")] public List<Object> registered;
    
        public T GetObject<T>(string type, string path) where T : Object
        {
            var assetPath = AssetDatabase.GetAssetPath(registered.Find(x => x.name == type));
            if (!string.IsNullOrEmpty(assetPath))
            {
                var loaded =  AssetDatabase.LoadAssetAtPath<T>(Path.Combine(assetPath, path));
                return loaded;
            } 
            return null;
        }
    } 
}