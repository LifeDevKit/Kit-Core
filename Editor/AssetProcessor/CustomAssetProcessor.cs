using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kit
{
    public class CustomAssetProcessor : AssetPostprocessor
    { 
        private static string RuleApplyDirectoryPath {
            get
            {
                return "Kenshi";
            }   
        }
        private static void Process(string asset)
        { 
            var splitPath = asset.Split(Path.AltDirectorySeparatorChar); 
            if (splitPath.Skip(1).First() != RuleApplyDirectoryPath) return;
        
            splitPath = splitPath.Skip(1).SkipLast(1)
                .Where(x => x != RuleApplyDirectoryPath).ToArray();
                
            
            if (splitPath.Length != 0)
            {
                var loaded = AssetDatabase.LoadAssetAtPath<Object>(asset);
                // old label
                var labels = AssetDatabase.GetLabels(loaded);
                var newLabels = new string[] {RuleApplyDirectoryPath, string.Join('-', splitPath).ToLower()}; 
          
                // 두 배열 스트링이 같은지 확인한다. (라벨이 다른 배열 참조로 할당되면 import가 다시 일어나는듯)
                if (newLabels.SequenceEqual(labels))
                    return;  
                AssetDatabase.SetLabels(loaded, newLabels);
            }
        }
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
            string[] movedAssets, string[] movedFromAssetPaths)
        { 
            foreach (var asset in importedAssets) 
                Process(asset); 
            foreach (var asset in movedAssets) 
                Process(asset); 
        }
 
    }
}