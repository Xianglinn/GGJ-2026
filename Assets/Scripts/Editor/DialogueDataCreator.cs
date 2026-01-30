using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 对话数据创建辅助工具
/// 用于在编辑器中快速创建示例对话数据
/// </summary>
public class DialogueDataCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/对话系统/创建示例对话数据")]
    public static void CreateExampleDialogueData()
    {
        // 创建欢迎对话
        DialogueData welcomeDialogue = ScriptableObject.CreateInstance<DialogueData>();
        welcomeDialogue.dialogueID = "test_welcome";
        
        // 添加对话行
        welcomeDialogue.lines.Add(new DialogueLine
        {
            characterName = "系统",
            dialogueText = "欢迎来到对话系统测试！这是第一行测试文本。",
            typewriterSpeed = 30f,
            autoContinue = false
        });

        welcomeDialogue.lines.Add(new DialogueLine
        {
            characterName = "旁白",
            dialogueText = "这是第二行对话。打字机效果正在工作中...",
            typewriterSpeed = 40f,
            autoContinue = false
        });

        welcomeDialogue.lines.Add(new DialogueLine
        {
            characterName = "系统",
            dialogueText = "接下来你将看到一些选项。请做出你的选择！",
            typewriterSpeed = 35f,
            autoContinue = false
        });

        // 创建路径 A 对话
        DialogueData pathADialogue = ScriptableObject.CreateInstance<DialogueData>();
        pathADialogue.dialogueID = "test_path_a";
        pathADialogue.lines.Add(new DialogueLine
        {
            characterName = "系统",
            dialogueText = "你选择了路径 A！这是一个勇敢的选择。",
            typewriterSpeed = 30f
        });
        pathADialogue.endsConversation = true;

        // 创建路径 B 对话
        DialogueData pathBDialogue = ScriptableObject.CreateInstance<DialogueData>();
        pathBDialogue.dialogueID = "test_path_b";
        pathBDialogue.lines.Add(new DialogueLine
        {
            characterName = "系统",
            dialogueText = "你选择了路径 B！这是一个谨慎的选择。",
            typewriterSpeed = 30f
        });
        pathBDialogue.endsConversation = true;

        // 保存资源 (先创建资源，再建立引用)
        string folderPath = "Assets/Resources/Dialogue";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogue");
        }

        // 先创建空的资源或仅含基础数据的资源
        AssetDatabase.CreateAsset(welcomeDialogue, $"{folderPath}/TestDialogue_Welcome.asset");
        AssetDatabase.CreateAsset(pathADialogue, $"{folderPath}/TestDialogue_PathA.asset");
        AssetDatabase.CreateAsset(pathBDialogue, $"{folderPath}/TestDialogue_PathB.asset");

        // 此时对象已成为资源，建立引用会被正确记录
        welcomeDialogue.choices.Add(new DialogueChoice
        {
            choiceText = "选择路径 A（勇敢）",
            nextDialogue = pathADialogue,
            setFlag = "chose_path_a"
        });

        welcomeDialogue.choices.Add(new DialogueChoice
        {
            choiceText = "选择路径 B（谨慎）",
            nextDialogue = pathBDialogue,
            setFlag = "chose_path_b"
        });

        EditorUtility.SetDirty(welcomeDialogue);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[DialogueDataCreator] 示例对话数据已创建！");
        Debug.Log($"- {folderPath}/TestDialogue_Welcome.asset");
        Debug.Log($"- {folderPath}/TestDialogue_PathA.asset");
        Debug.Log($"- {folderPath}/TestDialogue_PathB.asset");

        // 选中欢迎对话
        Selection.activeObject = welcomeDialogue;
        EditorGUIUtility.PingObject(welcomeDialogue);
    }

    [MenuItem("Tools/对话系统/创建简单对话（无选项）")]
    public static void CreateSimpleDialogueData()
    {
        DialogueData simpleDialogue = ScriptableObject.CreateInstance<DialogueData>();
        simpleDialogue.dialogueID = "test_simple";

        simpleDialogue.lines.Add(new DialogueLine
        {
            characterName = "测试",
            dialogueText = "这是一个简单的对话测试。",
            typewriterSpeed = 30f
        });

        simpleDialogue.lines.Add(new DialogueLine
        {
            characterName = "测试",
            dialogueText = "没有选项，对话会自动结束。",
            typewriterSpeed = 30f
        });

        simpleDialogue.endsConversation = true;

        string folderPath = "Assets/Resources/Dialogue";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Dialogue");
        }

        AssetDatabase.CreateAsset(simpleDialogue, $"{folderPath}/TestDialogue_Simple.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[DialogueDataCreator] 简单对话数据已创建！");
        Selection.activeObject = simpleDialogue;
        EditorGUIUtility.PingObject(simpleDialogue);
    }
#endif
}
