using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 对话管理器，负责对话流程控制
/// 特性：单例模式、对话流程管理、选项处理、事件系统
/// </summary>
public class DialogueManager : MonoSingleton<DialogueManager>
{
    /// <summary>
    /// 当前对话数据
    /// </summary>
    private DialogueData _currentDialogue;

    /// <summary>
    /// 当前对话行索引
    /// </summary>
    private int _currentLineIndex = 0;

    /// <summary>
    /// 对话是否正在进行中
    /// </summary>
    private bool _isDialogueActive = false;

    /// <summary>
    /// 当前是否在等待玩家输入
    /// </summary>
    private bool _waitingForInput = false;

    #region 事件定义

    /// <summary>
    /// 对话开始事件
    /// </summary>
    public UnityEvent<DialogueData> OnDialogueStarted = new UnityEvent<DialogueData>();

    /// <summary>
    /// 显示新对话行事件
    /// </summary>
    public UnityEvent<DialogueLine> OnLineDisplayed = new UnityEvent<DialogueLine>();

    /// <summary>
    /// 显示选择列表事件
    /// </summary>
    public UnityEvent<List<DialogueChoice>> OnChoicesPresented = new UnityEvent<List<DialogueChoice>>();

    /// <summary>
    /// 对话结束事件
    /// </summary>
    public UnityEvent OnDialogueEnded = new UnityEvent();

    /// <summary>
    /// 打字机跳过事件
    /// </summary>
    public UnityEvent OnTypewriterSkipped = new UnityEvent();

    #endregion

    /// <summary>
    /// 获取对话是否激活
    /// </summary>
    public bool IsDialogueActive => _isDialogueActive;

    /// <summary>
    /// 获取当前对话数据
    /// </summary>
    public DialogueData CurrentDialogue => _currentDialogue;

    /// <summary>
    /// 获取当前对话行
    /// </summary>
    public DialogueLine CurrentLine
    {
        get
        {
            if (_currentDialogue != null && _currentLineIndex < _currentDialogue.lines.Count)
            {
                return _currentDialogue.lines[_currentLineIndex];
            }
            return null;
        }
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        Debug.Log("[DialogueManager] Initialized successfully.");
    }

    #region 对话控制方法

    /// <summary>
    /// 开始新对话
    /// </summary>
    /// <param name="dialogueData">对话数据</param>
    public void StartDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null)
        {
            Debug.LogError("[DialogueManager] Cannot start dialogue: DialogueData is null.");
            return;
        }

        Debug.Log($"[DialogueManager] ========== StartDialogue called ==========");
        Debug.Log($"[DialogueManager] Attempting to start dialogue: {dialogueData.dialogueID}");
        Debug.Log($"[DialogueManager] Current _isDialogueActive state: {_isDialogueActive}");
        Debug.Log($"[DialogueManager] Dialogue has {dialogueData.lines?.Count ?? 0} lines");
        Debug.Log($"[DialogueManager] Dialogue has {dialogueData.choices?.Count ?? 0} choices");

        if (!dialogueData.Validate())
        {
            Debug.LogError($"[DialogueManager] Dialogue data validation failed: {dialogueData.name}");
            Debug.LogError($"[DialogueManager] Please check that the dialogue has valid lines with non-empty text.");
            return;
        }

        // 如果已有对话在进行，先结束它
        if (_isDialogueActive)
        {
            Debug.LogWarning("[DialogueManager] A dialogue is already active! Ending it before starting new one.");
            Debug.LogWarning($"[DialogueManager] This might indicate a timing issue or duplicate call.");
            EndDialogue();
        }

        _currentDialogue = dialogueData;
        _currentLineIndex = 0;
        _isDialogueActive = true;
        _waitingForInput = false;

        Debug.Log($"[DialogueManager] Dialogue state set: _isDialogueActive = true");
        Debug.Log($"[DialogueManager] Started dialogue: {dialogueData.dialogueID}");

        // 触发对话开始事件
        OnDialogueStarted?.Invoke(dialogueData);
        Debug.Log($"[DialogueManager] OnDialogueStarted event invoked");

        // 显示第一行对话
        Debug.Log($"[DialogueManager] About to call ShowCurrentLine()...");
        ShowCurrentLine();
    }

    /// <summary>
    /// 显示当前对话行
    /// </summary>
    private void ShowCurrentLine()
    {
        if (_currentDialogue == null)
        {
            Debug.LogError("[DialogueManager] ShowCurrentLine called but _currentDialogue is null!");
            return;
        }

        if (_currentDialogue.lines == null)
        {
            Debug.LogError($"[DialogueManager] Dialogue '{_currentDialogue.dialogueID}' has null lines list!");
            HandleDialogueEnd();
            return;
        }

        if (_currentLineIndex >= _currentDialogue.lines.Count)
        {
            // 已经到达对话末尾
            Debug.Log($"[DialogueManager] Reached end of dialogue lines ({_currentLineIndex}/{_currentDialogue.lines.Count})");
            HandleDialogueEnd();
            return;
        }

        DialogueLine currentLine = _currentDialogue.lines[_currentLineIndex];

        Debug.Log($"[DialogueManager] Showing line {_currentLineIndex + 1}/{_currentDialogue.lines.Count}: {currentLine.characterName}");

        // 播放语音（如果有）
        if (currentLine.voiceClip != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(currentLine.voiceClip);
            }
            else
            {
                Debug.LogWarning("[DialogueManager] AudioManager not found. Voice clip will not play.");
            }
        }

        // 触发显示对话行事件
        OnLineDisplayed?.Invoke(currentLine);

        _waitingForInput = true;

        // 如果设置了自动继续，启动协程
        if (currentLine.autoContinue)
        {
            StartCoroutine(AutoContinueCoroutine(currentLine.autoContinueDelay));
        }
    }

    /// <summary>
    /// 自动继续协程
    /// </summary>
    private IEnumerator AutoContinueCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_waitingForInput && _isDialogueActive)
        {
            ShowNextLine();
        }
    }

    /// <summary>
    /// 显示下一行对话
    /// </summary>
    public void ShowNextLine()
    {
        if (!_isDialogueActive)
        {
            Debug.LogWarning("[DialogueManager] No active dialogue.");
            return;
        }

        if (!_waitingForInput)
        {
            Debug.LogWarning("[DialogueManager] Not ready for next line yet.");
            return;
        }

        _waitingForInput = false;
        _currentLineIndex++;

        ShowCurrentLine();
    }

    /// <summary>
    /// 处理对话结束
    /// </summary>
    private void HandleDialogueEnd()
    {
        Debug.Log("[DialogueManager] HandleDialogueEnd called");

        if (_currentDialogue == null)
        {
            Debug.LogError("[DialogueManager] HandleDialogueEnd called but _currentDialogue is null!");
            EndDialogue();
            return;
        }

        // 检查是否有选择
        List<DialogueChoice> availableChoices = _currentDialogue.GetAvailableChoices();

        if (availableChoices.Count > 0)
        {
            // 显示选择
            Debug.Log($"[DialogueManager] Presenting {availableChoices.Count} choices.");
            OnChoicesPresented?.Invoke(availableChoices);
            _waitingForInput = true;
        }
        else
        {
            // 没有选择，检查是否有下一段对话
            if (_currentDialogue.nextDialogue != null)
            {
                // 自动进入下一段对话
                Debug.Log($"[DialogueManager] Continuing to next dialogue: {_currentDialogue.nextDialogue.dialogueID}");
                StartDialogue(_currentDialogue.nextDialogue);
            }
            else
            {
                // 结束对话
                Debug.Log("[DialogueManager] No choices and no next dialogue. Ending dialogue.");
                EndDialogue();
            }
        }
    }

    /// <summary>
    /// 选择选项
    /// </summary>
    /// <param name="choiceIndex">选项索引</param>
    public void SelectChoice(int choiceIndex)
    {
        if (!_isDialogueActive)
        {
            Debug.LogWarning("[DialogueManager] No active dialogue.");
            return;
        }

        List<DialogueChoice> availableChoices = _currentDialogue.GetAvailableChoices();

        if (choiceIndex < 0 || choiceIndex >= availableChoices.Count)
        {
            Debug.LogError($"[DialogueManager] Invalid choice index: {choiceIndex}");
            return;
        }

        DialogueChoice selectedChoice = availableChoices[choiceIndex];

        Debug.Log($"[DialogueManager] Selected choice: {selectedChoice.choiceText}");

        // 设置标记（如果有）
        if (!string.IsNullOrEmpty(selectedChoice.setFlag))
        {
            if (DataManager.Instance != null)
            {
                DataManager.Instance.SetStoryFlag(selectedChoice.setFlag, true);
                Debug.Log($"[DialogueManager] Set story flag: {selectedChoice.setFlag}");
            }
            else
            {
                Debug.LogWarning($"[DialogueManager] DataManager not found. Cannot set flag: {selectedChoice.setFlag}");
            }
        }

        _waitingForInput = false;

        // 进入下一段对话
        if (selectedChoice.nextDialogue != null)
        {
            StartDialogue(selectedChoice.nextDialogue);
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>
    /// 结束对话
    /// </summary>
    public void EndDialogue()
    {
        if (!_isDialogueActive)
        {
            return;
        }

        Debug.Log($"[DialogueManager] Ending dialogue: {_currentDialogue?.dialogueID}");

        // 触发完成事件（如果有）
        if (!string.IsNullOrEmpty(_currentDialogue?.onCompleteEventName))
        {
            Debug.Log($"[DialogueManager] Triggering event: {_currentDialogue.onCompleteEventName}");
            // 可以在这里添加事件系统集成
        }

        _isDialogueActive = false;
        _currentDialogue = null;
        _currentLineIndex = 0;
        _waitingForInput = false;

        // 触发对话结束事件
        OnDialogueEnded?.Invoke();
    }

    /// <summary>
    /// 跳过打字机动画
    /// </summary>
    public void SkipTypewriter()
    {
        OnTypewriterSkipped?.Invoke();
    }

    /// <summary>
    /// 检查是否在等待输入
    /// </summary>
    public bool IsWaitingForInput()
    {
        return _isDialogueActive && _waitingForInput;
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 获取当前进度百分比
    /// </summary>
    public float GetProgress()
    {
        if (_currentDialogue == null || _currentDialogue.lines.Count == 0)
            return 0f;

        return (float)_currentLineIndex / _currentDialogue.lines.Count;
    }

    /// <summary>
    /// 加载并开始对话（从 Resources 路径）
    /// </summary>
    /// <param name="dialoguePath">对话资源路径</param>
    public void LoadAndStartDialogue(string dialoguePath)
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("[DialogueManager] ResourceManager not found.");
            return;
        }

        DialogueData dialogue = ResourceManager.Instance.Load<DialogueData>(dialoguePath);

        if (dialogue != null)
        {
            StartDialogue(dialogue);
        }
        else
        {
            Debug.LogError($"[DialogueManager] Failed to load dialogue at path: {dialoguePath}");
        }
    }

    #endregion
}
