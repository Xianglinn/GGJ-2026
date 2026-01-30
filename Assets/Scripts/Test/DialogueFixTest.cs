using UnityEngine;

/// <summary>
/// 对话修复测试脚本
/// </summary>
public class DialogueFixTest : MonoBehaviour
{
    private void Start()
    {
        // 重置所有单例状态
        DialogueManager.ResetSingletonState();
        UIManager.ResetSingletonState();
        ResourceManager.ResetSingletonState();
        DataManager.ResetSingletonState();
        
        Debug.Log("[DialogueFixTest] 单例状态已重置");
        
        // 延迟测试
        Invoke("TestDialogue", 1f);
    }
    
    private void TestDialogue()
    {
        // 检查管理器
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[DialogueFixTest] DialogueManager仍为null");
            return;
        }
        
        if (UIManager.Instance == null)
        {
            Debug.LogError("[DialogueFixTest] UIManager仍为null");
            return;
        }
        
        // 加载测试对话
        DialogueData testDialogue = Resources.Load<DialogueData>("Dialogue/TestDialogue_Welcome");
        if (testDialogue == null)
        {
            Debug.LogError("[DialogueFixTest] 无法加载测试对话");
            return;
        }
        
        // 查找并注册DialogueUI
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>(true);
        if (dialogueUI != null)
        {
            if (!UIManager.Instance.IsPanelRegistered<DialogueUI>())
            {
                UIManager.Instance.RegisterPanel(dialogueUI);
            }
            
            // 开始对话
            DialogueManager.Instance.StartDialogue(testDialogue);
            UIManager.Instance.ShowPanel<DialogueUI>();
            
            Debug.Log("[DialogueFixTest] 对话测试启动成功！");
        }
        else
        {
            Debug.LogError("[DialogueFixTest] 未找到DialogueUI");
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestDialogue();
        }
    }
}