using UnityEngine;
using UnityEngine.UI; // <--- 必须添加这行才能使用 Image 组件
/// <summary>
/// 尾声 UI 面板：对应 Scene4
/// </summary>
public class UIEpiloguePanel : UIBasePanel<object>
{
    [Header("UI Components")]
    [SerializeField] private Image backgroundImage; // [新增] 指向面板上的背景 Image 组件

    [Header("Background Sprites")]
    [SerializeField] private Sprite commonBackground; // [新增] 通用剧情背景图
    [SerializeField] private Sprite girlEffectBackground;  // [新增] 对应：小女孩的珍藏
    [SerializeField] private Sprite wellEffectBackground;  // [新增] 对应：井中之天
    [SerializeField] private Sprite witchEffectBackground; // [新增] 对应：魔女的面具
    
    private bool isHandlingDialogue = false;
    private const string COMMON_DIALOGUE_PATH = "Data/Dialogues/Dialogue_102";

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

        // 先开始分支对话
        StartBranchDialogue();
    }

    private void OnBranchDialogueEnded()
    {
        if (!isHandlingDialogue) return;
        
        // 移除监听
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnBranchDialogueEnded);
        }

        isHandlingDialogue = false;
        
        // 分支结束后，播放通用对话
        PlayCommonDialogue();
    }

    private void StartBranchDialogue()
    {
        if (GameFlowManager.Instance == null || DialogueManager.Instance == null) return;

        SpecialEffectType effect = GameFlowManager.Instance.LastTriggeredEffect;
        string dialoguePath = "";

        // [修改] 根据特效类型决定对话路径 以及 切换背景
        switch (effect)
        {
            case SpecialEffectType.小女孩的珍藏:
                dialoguePath = "Data/Dialogues/Dialogue_1022";
                SetBackground(girlEffectBackground); // 切换背景
                break;
            case SpecialEffectType.井中之天:
                dialoguePath = "Data/Dialogues/Dialogue_1023";
                SetBackground(wellEffectBackground); // 切换背景
                break;
            case SpecialEffectType.魔女的面具:
                dialoguePath = "Data/Dialogues/Dialogue_1021";
                SetBackground(witchEffectBackground); // 切换背景
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(dialoguePath))
        {
            isHandlingDialogue = true;
            DialogueManager.Instance.OnDialogueEnded.AddListener(OnBranchDialogueEnded);
            Debug.Log($"[UIEpiloguePanel] Starting Branch Dialogue for effect: {effect}");
            DialogueManager.Instance.LoadAndStartDialogue(dialoguePath);
        }
        else
        {
            // 如果没有分支对话，直接播放通用对话
            Debug.Log("[UIEpiloguePanel] No branch dialogue to play, skipping to Common Dialogue.");
            PlayCommonDialogue();
        }
    }

    // [新增] 辅助方法：安全设置背景
    private void SetBackground(Sprite sprite)
    {
        if (backgroundImage != null && sprite != null)
        {
            backgroundImage.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("[UIEpiloguePanel] Failed to set background. Image component or Sprite is missing.");
        }
    }

    private void PlayCommonDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            Debug.Log($"[UIEpiloguePanel] Starting Common Dialogue: {COMMON_DIALOGUE_PATH}");
            
            // [新增] 切换到通用剧情背景
            SetBackground(commonBackground);
            // 监听通用对话结束
            // 注意：先移除之前的监听（如果有），防止重复
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnCommonDialogueEnded);
            DialogueManager.Instance.OnDialogueEnded.AddListener(OnCommonDialogueEnded);
            
            DialogueManager.Instance.LoadAndStartDialogue(COMMON_DIALOGUE_PATH);
        }
    }

    private void OnCommonDialogueEnded()
    {
        Debug.Log("[UIEpiloguePanel] Common Dialogue Ended. Transitioning to LevelMap (Scene 5)...");
        
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnCommonDialogueEnded);
        }

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SwitchState(GameState.LevelMap);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnBranchDialogueEnded);
        }

        // 从 UIManager 注销
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterPanel<UIEpiloguePanel>();
        }
    }
}
