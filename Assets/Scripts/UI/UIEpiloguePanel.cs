using UnityEngine;

/// <summary>
/// 尾声 UI 面板：对应 Scene4
/// </summary>
public class UIEpiloguePanel : UIBasePanel<object>
{
    private bool isHandlingDialogue = false;
    private const string COMMON_DIALOGUE_PATH = "Data/Dialogues/Epilogue_Common";

    private void Awake()
    {
        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIEpiloguePanel>())
        {
            UIManager.Instance.RegisterPanel(this);
            Debug.Log("[UIEpiloguePanel] Auto-registered to UIManager");
        }
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);
        Debug.Log("[UIEpiloguePanel] Initialized");
        
        StartCoroutine(StartEpilogueSequence());
    }

    private System.Collections.IEnumerator StartEpilogueSequence()
    {
        // 延迟一帧确保UI完全准备好
        yield return null; 

        if (DialogueManager.Instance != null)
        {
            isHandlingDialogue = true;
            DialogueManager.Instance.OnDialogueEnded.AddListener(OnCommonDialogueEnded);
            
            Debug.Log("[UIEpiloguePanel] Starting Common Dialogue...");
            DialogueManager.Instance.LoadAndStartDialogue(COMMON_DIALOGUE_PATH);
        }
    }

    private void OnCommonDialogueEnded()
    {
        if (!isHandlingDialogue) return;
        
        // 移除监听，防止循环触发
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnCommonDialogueEnded);
        }

        isHandlingDialogue = false;
        
        // 开始分支对话
        StartBranchDialogue();
    }

    private void StartBranchDialogue()
    {
        if (GameFlowManager.Instance == null || DialogueManager.Instance == null) return;

        SpecialEffectType effect = GameFlowManager.Instance.LastTriggeredEffect;
        string dialoguePath = "";

        // 根据特效类型决定对话路径
        switch (effect)
        {
            case SpecialEffectType.小女孩的珍藏:
                dialoguePath = "Data/Dialogues/Epilogue_GirlCollection";
                break;
            case SpecialEffectType.井中之天:
                dialoguePath = "Data/Dialogues/Epilogue_WellSky";
                break;
            case SpecialEffectType.魔女的面具:
                dialoguePath = "Data/Dialogues/Epilogue_WitchMask";
                break;
            default:
                Debug.Log("[UIEpiloguePanel] No special effect triggered, no branch dialogue.");
                return;
        }

        if (!string.IsNullOrEmpty(dialoguePath))
        {
            Debug.Log($"[UIEpiloguePanel] Starting Branch Dialogue for effect: {effect}");
            DialogueManager.Instance.LoadAndStartDialogue(dialoguePath);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnCommonDialogueEnded);
        }

        // 从 UIManager 注销
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterPanel<UIEpiloguePanel>();
        }
    }
}
