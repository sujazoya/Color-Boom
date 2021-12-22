using UnityEngine;
using UnityEditor;

public class ApplyPrefabChanges : MonoBehaviour
{
    [MenuItem("Tools/Apply Prefab Changes %#x")]
    static public void applyPrefabChanges()
    {
        var obj = Selection.activeGameObject;
        if (obj != null)
        {
            var prefab_root = PrefabUtility.FindPrefabRoot(obj);
            var prefab_src = PrefabUtility.GetPrefabParent(prefab_root);
            if (prefab_src != null)
            {
                PrefabUtility.ReplacePrefab(prefab_root, prefab_src, ReplacePrefabOptions.ConnectToPrefab);
                Debug.Log(AssetDatabase.GetAssetPath(prefab_src) + " Updated");
            }
            else
            {
                Debug.Log("Selected has no prefab");
            }
        }
        else
        {
            Debug.Log("Nothing selected");
        }
    }
}