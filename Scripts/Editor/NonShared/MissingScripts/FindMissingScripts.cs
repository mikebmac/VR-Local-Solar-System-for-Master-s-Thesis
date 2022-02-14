using System.Linq;
using UnityEngine;
using UnityEditor;

namespace MacKay.Editor
{
    /// <summary>
    /// FindMissingScripts will find all scripts that are missing in the project.
    /// </summary>
    public class FindMissingScripts
    {
        [MenuItem(("Tools/Scripting/Find Missing Scripts in Scene"))]
        private static void FindMissingScriptsInSceneMenuItem()
        {
            foreach (GameObject gameObject in GameObject.FindObjectsOfType<GameObject>(true))
            {
                foreach (Component component in gameObject.GetComponentsInChildren<Component>())
                {
                    if (component == null)
                    {
                        Debug.Log("GameObject found with missing script: " + gameObject.name, gameObject);
                        break;
                    }
                }
            }
        }
        
        [MenuItem(("Tools/Scripting/Find Missing Scripts in Project"))]
        private static void FindMissingScriptsInProjectMenuItem()
        {
            string[] prefabPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (string path in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                foreach (Component component in prefab.GetComponentsInChildren<Component>())
                {
                    if (component == null)
                    {
                        Debug.Log("Prefab found with missing script: " + path, prefab);
                        break;
                    }
                }
            }
        }
    }
}
