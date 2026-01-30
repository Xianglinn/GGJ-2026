using UnityEngine;
using UnityEditor;

/// <summary>
/// 对话数据诊断工具
/// 用于检查对话数据的完整性
/// </summary>
public class DialogueDataDiagnostic : EditorWindow
{
    private DialogueData dialogueToCheck;

    [MenuItem("Tools/对话系统/Dialogue Data Diagnostic")]
    public static void ShowWindow()
    {
        GetWindow<DialogueDataDiagnostic>("Dialogue Diagnostic");
    }

    private void OnGUI()
    {
        GUILayout.Label("对话数据诊断工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        dialogueToCheck = (DialogueData)EditorGUILayout.ObjectField("对话数据", dialogueToCheck, typeof(DialogueData), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("诊断对话数据"))
        {
            DiagnoseDialogue();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("检查所有对话数据"))
        {
            CheckAllDialogues();
        }
    }

    private void DiagnoseDialogue()
    {
        if (dialogueToCheck == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择对话数据", "确定");
            return;
        }

        Debug.Log("=== 对话数据诊断 ===");
        Debug.Log($"对话名称: {dialogueToCheck.name}");
        Debug.Log($"对话ID: {dialogueToCheck.dialogueID}");

        // 检查 lines
        if (dialogueToCheck.lines == null)
        {
            Debug.LogError("❌ lines 列表为 null!");
        }
        else if (dialogueToCheck.lines.Count == 0)
        {
            Debug.LogError("❌ lines 列表为空! 对话必须至少有一行文本。");
        }
        else
        {
            Debug.Log($"✓ lines 数量: {dialogueToCheck.lines.Count}");

            for (int i = 0; i < dialogueToCheck.lines.Count; i++)
            {
                var line = dialogueToCheck.lines[i];
                if (line == null)
                {
                    Debug.LogError($"  ❌ Line {i} 为 null!");
                }
                else if (string.IsNullOrEmpty(line.dialogueText))
                {
                    Debug.LogError($"  ❌ Line {i} 的对话文本为空!");
                }
                else
                {
                    Debug.Log($"  ✓ Line {i}: {line.characterName} - \"{line.dialogueText.Substring(0, Mathf.Min(30, line.dialogueText.Length))}...\"");
                }
            }
        }

        // 检查 choices
        if (dialogueToCheck.choices == null)
        {
            Debug.Log("choices 列表为 null (这是正常的，如果没有选择)");
        }
        else
        {
            Debug.Log($"✓ choices 数量: {dialogueToCheck.choices.Count}");
            for (int i = 0; i < dialogueToCheck.choices.Count; i++)
            {
                var choice = dialogueToCheck.choices[i];
                if (choice == null)
                {
                    Debug.LogError($"  ❌ Choice {i} 为 null!");
                }
                else
                {
                    Debug.Log($"  ✓ Choice {i}: \"{choice.choiceText}\" → {(choice.nextDialogue != null ? choice.nextDialogue.name : "null")}");
                }
            }
        }

        // 检查 nextDialogue
        if (dialogueToCheck.nextDialogue != null)
        {
            Debug.Log($"✓ nextDialogue: {dialogueToCheck.nextDialogue.name}");
        }
        else
        {
            Debug.Log("nextDialogue 为 null (如果这是最后一段对话，这是正常的)");
        }

        // 运行验证
        bool isValid = dialogueToCheck.Validate();
        if (isValid)
        {
            Debug.Log("✓✓✓ 对话数据验证通过!");
        }
        else
        {
            Debug.LogError("❌❌❌ 对话数据验证失败!");
        }

        Debug.Log("=== 诊断完成 ===");
    }

    private void CheckAllDialogues()
    {
        string[] guids = AssetDatabase.FindAssets("t:DialogueData");
        Debug.Log($"=== 检查所有对话数据 (找到 {guids.Length} 个) ===");

        int validCount = 0;
        int invalidCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogueData dialogue = AssetDatabase.LoadAssetAtPath<DialogueData>(path);

            if (dialogue != null)
            {
                bool isValid = dialogue.Validate();
                if (isValid)
                {
                    validCount++;
                    Debug.Log($"✓ {dialogue.name} - 有效 ({dialogue.lines?.Count ?? 0} 行)");
                }
                else
                {
                    invalidCount++;
                    Debug.LogError($"❌ {dialogue.name} - 无效!");
                    
                    // 详细诊断
                    if (dialogue.lines == null || dialogue.lines.Count == 0)
                    {
                        Debug.LogError($"  原因: 没有对话行");
                    }
                    else
                    {
                        for (int i = 0; i < dialogue.lines.Count; i++)
                        {
                            if (dialogue.lines[i] == null || string.IsNullOrEmpty(dialogue.lines[i].dialogueText))
                            {
                                Debug.LogError($"  原因: Line {i} 为空或文本为空");
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"=== 检查完成: {validCount} 有效, {invalidCount} 无效 ===");

        if (invalidCount > 0)
        {
            EditorUtility.DisplayDialog("检查结果", 
                $"发现 {invalidCount} 个无效的对话数据!\n请查看 Console 了解详情。", 
                "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("检查结果", 
                $"所有 {validCount} 个对话数据都有效!", 
                "确定");
        }
    }
}
