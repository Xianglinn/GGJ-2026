using UnityEngine;

/// <summary>
/// 对话系统测试脚本
/// 用于在编辑器或运行时测试对话功能
/// </summary>
public class DialogueTester : MonoBehaviour
{
    [Header("测试对话数据")]
    [Tooltip("要测试的对话数据")]
    public DialogueData testDialogue;

    [Header("测试设置")]
    [Tooltip("启动时自动开始对话")]
    public bool autoStartOnAwake = false;

    [Tooltip("按键触发对话")]
    public KeyCode triggerKey = KeyCode.Space;

    [Header("UI 引用")]
    [Tooltip("DialogueUI 面板引用（可选，不设置则自动查找）")]
    public DialogueUI dialogueUIPanel;

    private void Awake()
    {
        // 查找 DialogueUI
        if (dialogueUIPanel == null)
        {
            dialogueUIPanel = FindObjectOfType<DialogueUI>(true);
        }

        // 确保 DialogueUI 已注册
        if (dialogueUIPanel != null && UIManager.Instance != null)
        {
            if (!UIManager.Instance.IsPanelRegistered<DialogueUI>())
            {
                UIManager.Instance.RegisterPanel(dialogueUIPanel);
                Debug.Log("[DialogueTester] Registered DialogueUI panel.");
            }
        }
    }

    private void Start()
    {
        if (autoStartOnAwake && testDialogue != null)
        {
            StartTestDialogue();
        }
    }

    private void Update()
    {
        // 按键触发
        if (Input.GetKeyDown(triggerKey) && testDialogue != null)
        {
            StartTestDialogue();
        }
    }

    /// <summary>
    /// 开始测试对话
    /// </summary>
    public void StartTestDialogue()
    {
        if (testDialogue == null)
        {
            Debug.LogError("[DialogueTester] No test dialogue assigned!");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[DialogueTester] DialogueManager not found!");
            return;
        }

        if (UIManager.Instance == null)
        {
            Debug.LogError("[DialogueTester] UIManager not found!");
            return;
        }

        Debug.Log($"[DialogueTester] Starting test dialogue: {testDialogue.dialogueID}");

        // 重要：先显示 UI，确保事件订阅完成，然后再开始对话
        UIManager.Instance.ShowPanel<DialogueUI>();
        
        // 等待一帧确保 UI 完全激活和事件订阅完成
        StartCoroutine(StartDialogueAfterUIReady(testDialogue));
    }

    /// <summary>
    /// 在 UI 准备好后开始对话
    /// </summary>
    private System.Collections.IEnumerator StartDialogueAfterUIReady(DialogueData dialogue)
    {
        // 等待一帧，确保 DialogueUI 的 OnEnable 和 Show 方法已执行
        yield return null;
        
        Debug.Log("[DialogueTester] UI ready, starting dialogue now...");
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    /// <summary>
    /// 通过路径加载并开始对话
    /// </summary>
    public void StartDialogueByPath(string dialoguePath)
    {
        if (string.IsNullOrEmpty(dialoguePath))
        {
            Debug.LogError("[DialogueTester] Dialogue path is empty!");
            return;
        }

        Debug.Log($"[DialogueTester] Loading dialogue from: {dialoguePath}");

        // 先显示 UI
        UIManager.Instance?.ShowPanel<DialogueUI>();
        
        // 然后加载并开始对话
        StartCoroutine(LoadAndStartDialogueCoroutine(dialoguePath));
    }

    /// <summary>
    /// 加载并开始对话的协程
    /// </summary>
    private System.Collections.IEnumerator LoadAndStartDialogueCoroutine(string dialoguePath)
    {
        // 等待 UI 准备好
        yield return null;
        
        DialogueManager.Instance?.LoadAndStartDialogue(dialoguePath);
    }

    /// <summary>
    /// GUI 测试按钮（仅编辑器）
    /// </summary>
    
    /// <summary>
    /// 编辑器快捷键测试
    /// </summary>
    [UnityEngine.ContextMenu("触发测试对话")]
    public void TriggerTestDialogueFromMenu()
    {
        StartTestDialogue();
    }

    /// <summary>
    /// 编辑器快捷键 - 加载路径 A
    /// </summary>
    [UnityEngine.ContextMenu("直接测试路径 A")]
    public void TestPathA()
    {
        StartDialogueByPath("Dialogue/TestDialogue_PathA");
    }

    /// <summary>
    /// 编辑器快捷键 - 加载路径 B
    /// </summary>
    [UnityEngine.ContextMenu("直接测试路径 B")]
    public void TestPathB()
    {
        StartDialogueByPath("Dialogue/TestDialogue_PathB");
    }

    private void OnGUI()
    {
        // 在屏幕左上角显示测试按钮
        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        GUILayout.BeginVertical("box");

        GUILayout.Label("对话系统测试", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });

        if (GUILayout.Button($"触发对话 [{triggerKey}]"))
        {
            StartTestDialogue();
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            GUILayout.Label($"对话进度: {DialogueManager.Instance.GetProgress() * 100:F0}%");

            if (GUILayout.Button("结束对话"))
            {
                DialogueManager.Instance.EndDialogue();
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
