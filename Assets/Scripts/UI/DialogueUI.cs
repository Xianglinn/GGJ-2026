using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 对话 UI 面板
/// 显示对话文本、角色立绘、选择按钮，支持打字机效果
/// </summary>
public class DialogueUI : UIBasePanel<DialogueData>
{
    [Header("UI 元素")]
    [SerializeField]
    private Text characterNameText;
    [SerializeField]
    private Text dialogueText;
    [SerializeField]
    private Image characterPortrait;
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private GameObject continueIndicator;
    [Header("选择按钮")]
    [SerializeField]
    private Transform choiceButtonContainer;
    [SerializeField]
    private Button choiceButtonPrefab;
    [Header("动画设置")]
    [SerializeField]
    private float fadeInDuration = 0.3f;
    [SerializeField]
    private CanvasGroup canvasGroup;
    /// <summary>
    /// 打字机协程
    /// </summary>
    private Coroutine _typewriterCoroutine;
    /// <summary>
    /// 当前显示的对话行
    /// </summary>
    private DialogueLine _currentLine;
    /// <summary>
    /// 打字机是否完成
    /// </summary>
    private bool _typewriterComplete = false;
    /// <summary>
    /// 当前创建的选择按钮列表
    /// </summary>
    private List<Button> _activeChoiceButtons = new List<Button>();

    public override CanvasType PanelCanvasType => CanvasType.Persistent;

    private void Awake()
    {
        // 确保有 CanvasGroup
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        // 绑定继续按钮事件
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
        // 初始隐藏选择按钮模板
        if (choiceButtonPrefab != null)
        {
            choiceButtonPrefab.gameObject.SetActive(false);
        }
        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<DialogueUI>())
        {
            UIManager.Instance.RegisterPanel(this);
            Debug.Log("[DialogueUI] Registered self to UIManager in Awake.");
        }

        // 订阅事件（在Awake中一次性订阅，确保隐藏时也能接收消息）
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        // 取消订阅事件
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueStarted.RemoveListener(OnDialogueStarted);
            DialogueManager.Instance.OnLineDisplayed.RemoveListener(OnLineDisplayed);
            DialogueManager.Instance.OnChoicesPresented.RemoveListener(OnChoicesPresented);
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnDialogueEnded);
            DialogueManager.Instance.OnTypewriterSkipped.RemoveListener(OnTypewriterSkipped);
        }
    }
    public override void OnInitialize(DialogueData data)
    {
        base.OnInitialize(data);
        // 清空 UI
        ClearUI();
    }
    public override void Show()
    {
        // 确保事件已订阅
        SubscribeToEvents();
        base.Show();
        // 淡入动画
        if (canvasGroup != null)
        {
            StartCoroutine(FadeInCoroutine());
        }
    }
    /// <summary>
    /// 订阅对话管理器事件
    /// </summary>
    private void SubscribeToEvents()
    {
        if (DialogueManager.Instance != null)
        {
            // 先取消订阅，避免重复
            DialogueManager.Instance.OnDialogueStarted.RemoveListener(OnDialogueStarted);
            DialogueManager.Instance.OnLineDisplayed.RemoveListener(OnLineDisplayed);
            DialogueManager.Instance.OnChoicesPresented.RemoveListener(OnChoicesPresented);
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnDialogueEnded);
            DialogueManager.Instance.OnTypewriterSkipped.RemoveListener(OnTypewriterSkipped);
            // 重新订阅
            DialogueManager.Instance.OnDialogueStarted.AddListener(OnDialogueStarted);
            DialogueManager.Instance.OnLineDisplayed.AddListener(OnLineDisplayed);
            DialogueManager.Instance.OnChoicesPresented.AddListener(OnChoicesPresented);
            DialogueManager.Instance.OnDialogueEnded.AddListener(OnDialogueEnded);
            DialogueManager.Instance.OnTypewriterSkipped.AddListener(OnTypewriterSkipped);
            Debug.Log("[DialogueUI] Events subscribed.");
        }
        else
        {
            Debug.LogError("[DialogueUI] DialogueManager.Instance is null!");
        }
    }
    public override void Hide()
    {
        // 停止打字机协程
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
            _typewriterCoroutine = null;
        }
        base.Hide();
    }
    #region 事件处理
    /// <summary>
    /// 对话开始事件处理
    /// </summary>
    private void OnDialogueStarted(DialogueData dialogueData)
    {
        Debug.Log("[DialogueUI] Dialogue started.");
        
        // 显示对话面板
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPanel<DialogueUI>();
        }
        
        ClearUI();
    }
    /// <summary>
    /// 显示对话行事件处理
    /// </summary>
    private void OnLineDisplayed(DialogueLine line)
    {
        _currentLine = line;
        _typewriterComplete = false;
        // 更新角色名称
        if (characterNameText != null)
        {
            characterNameText.text = line.characterName;
        }
        // 更新角色立绘
        if (characterPortrait != null)
        {
            if (line.characterPortrait != null)
            {
                characterPortrait.sprite = line.characterPortrait;
                characterPortrait.gameObject.SetActive(true);
            }
            else
            {
                characterPortrait.gameObject.SetActive(false);
            }
        }
        // 隐藏选择按钮
        HideChoiceButtons();
        // 显示继续指示器
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }
        // 启动打字机效果
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
        }
        _typewriterCoroutine = StartCoroutine(TypewriterCoroutine(line.dialogueText, line.typewriterSpeed));
    }
    /// <summary>
    /// 显示选择列表事件处理
    /// </summary>
    private void OnChoicesPresented(List<DialogueChoice> choices)
    {
        Debug.Log($"[DialogueUI] Presenting {choices.Count} choices.");
        // 隐藏继续按钮
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
        }
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }
        // 显示选择按钮
        DisplayChoiceButtons(choices);
    }
    /// <summary>
    /// 对话结束事件处理
    /// </summary>
    private void OnDialogueEnded()
    {
        Debug.Log("[DialogueUI] Dialogue ended.");
        // 隐藏面板
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HidePanel<DialogueUI>();
        }
    }
    /// <summary>
    /// 打字机跳过事件处理
    /// </summary>
    private void OnTypewriterSkipped()
    {
        // 立即完成打字机
        if (!_typewriterComplete && _typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
            _typewriterCoroutine = null;
            if (dialogueText != null && _currentLine != null)
            {
                dialogueText.text = _currentLine.dialogueText;
            }
            _typewriterComplete = true;
            OnTypewriterComplete();
        }
    }
    #endregion
    #region UI 交互
    /// <summary>
    /// 继续按钮点击事件
    /// </summary>
    private void OnContinueClicked()
    {
        if (!_typewriterComplete)
        {
            // 跳过打字机动画
            DialogueManager.Instance?.SkipTypewriter();
        }
        else
        {
            // 继续下一行
            DialogueManager.Instance?.ShowNextLine();
        }
    }
    /// <summary>
    /// 选择按钮点击事件
    /// </summary>
    private void OnChoiceButtonClicked(int choiceIndex)
    {
        Debug.Log($"[DialogueUI] Choice {choiceIndex} clicked.");
        // 隐藏所有选择按钮
        HideChoiceButtons();
        // 延迟通知对话管理器，让UI有时间更新状态
        StartCoroutine(SelectChoiceAfterUIUpdate(choiceIndex));
    }
    /// <summary>
    /// 在UI更新后选择选项
    /// </summary>
    private System.Collections.IEnumerator SelectChoiceAfterUIUpdate(int choiceIndex)
    {
        // 等待一帧，让UI完成状态更新
        yield return null;
        Debug.Log($"[DialogueUI] Notifying DialogueManager of choice selection: {choiceIndex}");
        
        // 通知对话管理器
        DialogueManager.Instance?.SelectChoice(choiceIndex);
    }
    #endregion
    #region 打字机效果
    /// <summary>
    /// 打字机效果协程
    /// </summary>
    private IEnumerator TypewriterCoroutine(string text, float speed)
    {
        if (dialogueText == null)
            yield break;
        dialogueText.text = "";
        float delay = 1f / speed;
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }
        _typewriterComplete = true;
        OnTypewriterComplete();
    }
    /// <summary>
    /// 打字机完成回调
    /// </summary>
    private void OnTypewriterComplete()
    {
        // 显示继续指示器
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(true);
        }
    }
    #endregion
    #region 选择按钮管理
    /// <summary>
    /// 显示选择按钮
    /// </summary>
    private void DisplayChoiceButtons(List<DialogueChoice> choices)
    {
        // 清空现有按钮
        HideChoiceButtons();
        if (choiceButtonPrefab == null || choiceButtonContainer == null)
        {
            Debug.LogWarning("[DialogueUI] Choice button prefab or container is null.");
            return;
        }
        // 创建新按钮
        for (int i = 0; i < choices.Count; i++)
        {
            int choiceIndex = i; // 捕获索引
            DialogueChoice choice = choices[i];
            Button choiceButton = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            choiceButton.gameObject.SetActive(true);
            // 设置按钮文本
            Text buttonText = choiceButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = choice.choiceText;
            }
            // 绑定点击事件
            choiceButton.onClick.AddListener(() => OnChoiceButtonClicked(choiceIndex));
            _activeChoiceButtons.Add(choiceButton);
        }
    }
    /// <summary>
    /// 隐藏并销毁选择按钮
    /// </summary>
    private void HideChoiceButtons()
    {
        foreach (Button button in _activeChoiceButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        _activeChoiceButtons.Clear();
        // 显示继续按钮
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
    }
    #endregion
    #region 辅助方法
    /// <summary>
    /// 清空 UI
    /// </summary>
    private void ClearUI()
    {
        if (characterNameText != null)
            characterNameText.text = "";
        if (dialogueText != null)
            dialogueText.text = "";
        if (characterPortrait != null)
            characterPortrait.gameObject.SetActive(false);
        if (continueIndicator != null)
            continueIndicator.SetActive(false);
        HideChoiceButtons();
    }
    /// <summary>
    /// 淡入协程
    /// </summary>
    private IEnumerator FadeInCoroutine()
    {
        canvasGroup.blocksRaycasts = true; // 确保可以接受点击
        canvasGroup.interactable = true;   // 确保可交互

        canvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
    #endregion
}
