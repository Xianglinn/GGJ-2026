using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 重新生成对话数据的辅助脚本
/// </summary>
public class RegenerateDialogueAssets : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/对话系统/重新生成示例对话数据")]
    public static void Regenerate()
    {
        // 删除旧文件
        string folderPath = "Assets/Resources/Dialogue";
        
        string[] assetPaths = new string[]
        {
            $"{folderPath}/TestDialogue_Welcome.asset",
            $"{folderPath}/TestDialogue_PathA.asset",
            $"{folderPath}/TestDialogue_PathB.asset"
        };

        foreach (string path in assetPaths)
        {
            if (AssetDatabase.LoadAssetAtPath<DialogueData>(path) != null)
            {
                AssetDatabase.DeleteAsset(path);
                Debug.Log($"[RegenerateDialogueAssets] Deleted: {path}");
            }
        }

        AssetDatabase.Refresh();

        // 调用创建方法
        DialogueDataCreator.CreateExampleDialogueData();
        
        Debug.Log("[RegenerateDialogueAssets] 对话数据重新生成完成！");
    }
#endif
}
