using UnityEngine;
using System.Reflection;

/// <summary>
/// 快速对话系统修复工具
/// </summary>
public class QuickDialogueFix : MonoBehaviour
{
    [Header("测试对话")]
    public DialogueData testDialogue;

    private void Start()
    {
        // 延迟执行修复，确保所有管理器都已初始化
        Invoke("FixAndTest", 0.5f);
    }

    private void FixAndTest()
    {
        Debug.Log("[QuickDialogueFix] 开始修复对话系统...");
        
        // 修复单例状态
        FixSingletonState();
        
        // 测试对话系统
        TestDialogue();
    }

    private void FixSingletonState()
    {
        // 重置DialogueManager的退出标志
        try
        {
            System.Type managerType = typeof(DialogueManager);
            System.Type baseType = managerType.BaseType; // MonoSingleton<DialogueManager>
            
            FieldInfo quittingField = baseType.GetField("_applicationIsQuitting", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            if (quittingField != null)
            {
                quittingField.SetValue(null, false);
                Debug.Log("[QuickDialogueFix] 重置DialogueManager退出标志成功");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[QuickDialogueFix] 修复失败: {e.Message}");
        }
    }

    private void TestDialogue()
    {
        // 检查管理器状态
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[QuickDialogueFix] DialogueManager仍然为null！");
            return;
        }

        if (UIManager.Instance == null)
        {
            Debug.LogError("[QuickDialogueFix] UIManager为null！");
            return;
        }

        // 查找DialogueUI
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>(true);
        if (dialogueUI == null)
        {
            Debug.LogError("[QuickDialogueFix] 未找到DialogueUI！");
            return;
        }

        // 确保DialogueUI已注册
        if (!UIManager.Instance.IsPanelRegistered<DialogueUI>())
        {
            UIManager.Instance.RegisterPanel(dialogueUI);
            Debug.Log("[QuickDialogueFix] 注册DialogueUI成功");
        }

        // 使用测试对话数据
        if (testDialogue == null)
        {
            // 尝试加载默认测试对话
            testDialogue = Resources.Load<DialogueData>("Dialogue/TestDialogue_Welcome");
        }

        if (testDialogue != null)
        {
            Debug.Log($"[QuickDialogueFix] 开始测试对话: {testDialogue.dialogueID}");
            
            // 开始对话
            DialogueManager.Instance.StartDialogue(testDialogue);
            
            // 显示UI
            UIManager.Instance.ShowPanel<DialogueUI>();
        }
        else
        {
            Debug.LogError("[QuickDialogueFix] 未找到测试对话数据！");
        }
    }

    private void Update()
    {
        // 按T键测试对话
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestDialogue();
        }
    }
}