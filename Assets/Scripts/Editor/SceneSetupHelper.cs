using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 场景设置辅助工具
/// 提供场景管理和 Build Settings 配置的便捷方法
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
#if UNITY_EDITOR
    /// <summary>
    /// 设置 Scene1 为启动场景
    /// </summary>
    [MenuItem("Tools/场景设置/设置 Scene1 为启动场景")]
    public static void SetScene1AsStartScene()
    {
        string scene1Path = "Assets/Scenes/Scene1.unity";
        
        // 检查场景是否存在
        if (!System.IO.File.Exists(scene1Path))
        {
            Debug.LogError($"[SceneSetupHelper] Scene1 不存在于路径: {scene1Path}");
            return;
        }

        // 获取当前 Build Settings 中的场景
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        
        // 检查 Scene1 是否已经在 Build Settings 中
        bool scene1Found = false;
        int scene1Index = -1;
        
        for (int i = 0; i < buildScenes.Length; i++)
        {
            if (buildScenes[i].path == scene1Path)
            {
                scene1Found = true;
                scene1Index = i;
                break;
            }
        }

        if (scene1Found)
        {
            if (scene1Index == 0)
            {
                Debug.Log("[SceneSetupHelper] Scene1 已经是启动场景（索引 0）");
            }
            else
            {
                // 将 Scene1 移到第一位
                EditorBuildSettingsScene scene1 = buildScenes[scene1Index];
                EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[buildScenes.Length];
                newScenes[0] = scene1;
                
                int newIndex = 1;
                for (int i = 0; i < buildScenes.Length; i++)
                {
                    if (i != scene1Index)
                    {
                        newScenes[newIndex] = buildScenes[i];
                        newIndex++;
                    }
                }
                
                EditorBuildSettings.scenes = newScenes;
                Debug.Log($"[SceneSetupHelper] Scene1 已移动到索引 0（启动场景），原索引: {scene1Index}");
            }
        }
        else
        {
            // 将 Scene1 添加到 Build Settings 的第一位
            EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[buildScenes.Length + 1];
            newScenes[0] = new EditorBuildSettingsScene(scene1Path, true);
            
            for (int i = 0; i < buildScenes.Length; i++)
            {
                newScenes[i + 1] = buildScenes[i];
            }
            
            EditorBuildSettings.scenes = newScenes;
            Debug.Log("[SceneSetupHelper] Scene1 已添加到 Build Settings 并设为启动场景（索引 0）");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 打开 Scene1
    /// </summary>
    [MenuItem("Tools/场景设置/打开 Scene1")]
    public static void OpenScene1()
    {
        string scene1Path = "Assets/Scenes/Scene1.unity";
        
        if (!System.IO.File.Exists(scene1Path))
        {
            Debug.LogError($"[SceneSetupHelper] Scene1 不存在于路径: {scene1Path}");
            return;
        }

        // 提示保存当前场景
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            if (EditorUtility.DisplayDialog("保存场景", "当前场景有未保存的更改。是否保存？", "保存", "不保存"))
            {
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }
        }

        EditorSceneManager.OpenScene(scene1Path);
        Debug.Log("[SceneSetupHelper] 已打开 Scene1");
    }

    /// <summary>
    /// 显示当前 Build Settings 中的场景列表
    /// </summary>
    [MenuItem("Tools/场景设置/显示 Build Settings 场景列表")]
    public static void ShowBuildScenesList()
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        
        Debug.Log("========== Build Settings 场景列表 ==========");
        Debug.Log($"总共 {buildScenes.Length} 个场景");
        
        for (int i = 0; i < buildScenes.Length; i++)
        {
            string status = buildScenes[i].enabled ? "✓ 启用" : "✗ 禁用";
            string startMark = (i == 0) ? " [启动场景]" : "";
            Debug.Log($"[{i}] {status} - {buildScenes[i].path}{startMark}");
        }
        
        Debug.Log("==========================================");
    }

    /// <summary>
    /// 添加所有场景到 Build Settings
    /// </summary>
    [MenuItem("Tools/场景设置/添加所有场景到 Build Settings")]
    public static void AddAllScenesToBuildSettings()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[sceneGuids.Length];
        
        for (int i = 0; i < sceneGuids.Length; i++)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            newScenes[i] = new EditorBuildSettingsScene(scenePath, true);
        }
        
        // 确保 Scene1 在第一位
        System.Array.Sort(newScenes, (a, b) =>
        {
            if (a.path.Contains("Scene1.unity")) return -1;
            if (b.path.Contains("Scene1.unity")) return 1;
            return string.Compare(a.path, b.path);
        });
        
        EditorBuildSettings.scenes = newScenes;
        
        Debug.Log($"[SceneSetupHelper] 已添加 {sceneGuids.Length} 个场景到 Build Settings");
        ShowBuildScenesList();
    }
#endif
}
