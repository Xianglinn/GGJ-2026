using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话行数据结构
/// </summary>
[System.Serializable]
public class DialogueLine
{
    [Header("角色信息")]
    [Tooltip("角色名称")]
    public string characterName = "旁白";

    [Tooltip("角色立绘（可选）")]
    public Sprite characterPortrait;

    [Header("对话内容")]
    [Tooltip("对话文本")]
    [TextArea(3, 10)]
    public string dialogueText;

    [Tooltip("语音片段（可选）")]
    public AudioClip voiceClip;

    [Header("显示设置")]
    [Tooltip("打字机速度（字符/秒）")]
    public float typewriterSpeed = 30f;

    [Tooltip("是否自动继续到下一行")]
    public bool autoContinue = false;

    [Tooltip("自动继续延迟（秒）")]
    public float autoContinueDelay = 2f;

    [Header("环境设置")]
    [Tooltip("BGM 名称")]
    public string bgmName;

    [Tooltip("背景图名称")]
    public string backgroundName;
}

/// <summary>
/// 对话选项数据结构
/// </summary>
[System.Serializable]
public class DialogueChoice
{
    [Tooltip("选项文本")]
    public string choiceText;

    [Tooltip("选择后跳转到的对话数据")]
    public DialogueData nextDialogue;

    [Tooltip("选择此选项需要的条件标记（可选）")]
    public string requiredFlag;

    [Tooltip("选择此选项后设置的标记（可选）")]
    public string setFlag;
}

/// <summary>
/// 对话数据 ScriptableObject
/// 用于存储对话内容、角色立绘、选项分支等信息
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Game/Dialogue Data", order = 1)]
public class DialogueData : ScriptableObject
{
    [Header("对话标识")]
    [Tooltip("对话唯一 ID，用于存档系统")]
    public string dialogueID;

    [Header("对话内容")]
    [Tooltip("对话行列表")]
    public List<DialogueLine> lines = new List<DialogueLine>();

    [Header("分支选择")]
    [Tooltip("玩家选择列表（如果为空则自动继续）")]
    public List<DialogueChoice> choices = new List<DialogueChoice>();

    [Header("对话流程")]
    [Tooltip("默认下一段对话（无选择时使用）")]
    public DialogueData nextDialogue;

    [Tooltip("是否结束对话")]
    public bool endsConversation = false;

    [Header("触发事件")]
    [Tooltip("对话结束时触发的事件名称（可选）")]
    public string onCompleteEventName;

    /// <summary>
    /// 验证对话数据完整性
    /// </summary>
    public bool Validate()
    {
        if (string.IsNullOrEmpty(dialogueID))
        {
            Debug.LogWarning($"[DialogueData] Dialogue ID is empty in {name}");
            return false;
        }

        if (lines == null || lines.Count == 0)
        {
            Debug.LogWarning($"[DialogueData] No dialogue lines in {name}");
            return false;
        }

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line.dialogueText))
            {
                Debug.LogWarning($"[DialogueData] Empty dialogue text in {name}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 获取有效的选择列表（检查条件）
    /// </summary>
    public List<DialogueChoice> GetAvailableChoices()
    {
        if (choices == null || choices.Count == 0)
            return new List<DialogueChoice>();

        List<DialogueChoice> availableChoices = new List<DialogueChoice>();

        foreach (var choice in choices)
        {
            // 检查是否需要特定标记
            if (!string.IsNullOrEmpty(choice.requiredFlag))
            {
                // 从 DataManager 检查标记
                if (DataManager.Instance != null && !DataManager.Instance.GetStoryFlag(choice.requiredFlag))
                {
                    continue; // 跳过此选项
                }
            }

            availableChoices.Add(choice);
        }

        return availableChoices;
    }
}
