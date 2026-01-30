using UnityEngine;

/// <summary>
/// 对话系统调试助手
/// 添加到场景中以输出详细的调试信息
/// </summary>
public class DialogueDebugHelper : MonoBehaviour
{
    [Header("调试设置")]
    [SerializeField] private bool logAllEvents = true;
    [SerializeField] private bool logUIUpdates = true;

    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueStarted.AddListener(OnDialogueStartedDebug);
            DialogueManager.Instance.OnLineDisplayed.AddListener(OnLineDisplayedDebug);
            DialogueManager.Instance.OnChoicesPresented.AddListener(OnChoicesPresentedDebug);
            DialogueManager.Instance.OnDialogueEnded.AddListener(OnDialogueEndedDebug);
            Debug.Log("[DialogueDebugHelper] Event listeners registered.");
        }
        else
        {
            Debug.LogError("[DialogueDebugHelper] DialogueManager.Instance is null!");
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueStarted.RemoveListener(OnDialogueStartedDebug);
            DialogueManager.Instance.OnLineDisplayed.RemoveListener(OnLineDisplayedDebug);
            DialogueManager.Instance.OnChoicesPresented.RemoveListener(OnChoicesPresentedDebug);
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnDialogueEndedDebug);
        }
    }

    private void OnDialogueStartedDebug(DialogueData data)
    {
        if (logAllEvents)
        {
            Debug.Log($"<color=cyan>[DEBUG] OnDialogueStarted fired!</color>");
            Debug.Log($"  - Dialogue ID: {data.dialogueID}");
            Debug.Log($"  - Lines count: {data.lines.Count}");
            Debug.Log($"  - Choices count: {data.choices.Count}");
        }
    }

    private void OnLineDisplayedDebug(DialogueLine line)
    {
        if (logAllEvents)
        {
            Debug.Log($"<color=cyan>[DEBUG] OnLineDisplayed fired!</color>");
            Debug.Log($"  - Character: {line.characterName}");
            Debug.Log($"  - Text: {line.dialogueText}");
            Debug.Log($"  - Typewriter Speed: {line.typewriterSpeed}");
            Debug.Log($"  - Has Portrait: {line.characterPortrait != null}");
        }

        // 检查 DialogueUI 是否存在并激活
        CheckDialogueUIState();
    }

    private void OnChoicesPresentedDebug(System.Collections.Generic.List<DialogueChoice> choices)
    {
        if (logAllEvents)
        {
            Debug.Log($"<color=cyan>[DEBUG] OnChoicesPresented fired!</color>");
            Debug.Log($"  - Choices count: {choices.Count}");
            foreach (var choice in choices)
            {
                Debug.Log($"    * {choice.choiceText}");
            }
        }
    }

    private void OnDialogueEndedDebug()
    {
        if (logAllEvents)
        {
            Debug.Log($"<color=cyan>[DEBUG] OnDialogueEnded fired!</color>");
        }
    }

    private void CheckDialogueUIState()
    {
        if (!logUIUpdates) return;

        if (UIManager.Instance != null)
        {
            DialogueUI dialogueUI = UIManager.Instance.GetPanel<DialogueUI>();
            if (dialogueUI != null)
            {
                Debug.Log($"<color=yellow>[DEBUG] DialogueUI State:</color>");
                Debug.Log($"  - GameObject Active: {dialogueUI.gameObject.activeSelf}");
                Debug.Log($"  - Enabled: {dialogueUI.enabled}");
                
                // 使用反射检查私有字段
                var characterNameField = typeof(DialogueUI).GetField("characterNameText", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var dialogueTextField = typeof(DialogueUI).GetField("dialogueText", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var continueButtonField = typeof(DialogueUI).GetField("continueButton", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (characterNameField != null)
                {
                    var nameText = characterNameField.GetValue(dialogueUI) as UnityEngine.UI.Text;
                    Debug.Log($"  - Character Name Text: {(nameText != null ? $"'{nameText.text}'" : "NULL")}");
                }

                if (dialogueTextField != null)
                {
                    var dlgText = dialogueTextField.GetValue(dialogueUI) as UnityEngine.UI.Text;
                    Debug.Log($"  - Dialogue Text: {(dlgText != null ? $"'{dlgText.text}'" : "NULL")}");
                }

                if (continueButtonField != null)
                {
                    var btn = continueButtonField.GetValue(dialogueUI) as UnityEngine.UI.Button;
                    Debug.Log($"  - Continue Button: {(btn != null ? $"Active={btn.gameObject.activeSelf}, Interactable={btn.interactable}" : "NULL")}");
                }
            }
            else
            {
                Debug.LogError("[DEBUG] DialogueUI panel not found in UIManager!");
            }
        }
        else
        {
            Debug.LogError("[DEBUG] UIManager.Instance is null!");
        }
    }

    [ContextMenu("手动检查 DialogueUI 状态")]
    public void ManualCheckUIState()
    {
        CheckDialogueUIState();
    }

    [ContextMenu("检查事件订阅数量")]
    public void CheckEventSubscribers()
    {
        if (DialogueManager.Instance != null)
        {
            Debug.Log("<color=cyan>[DEBUG] Event Subscriber Counts:</color>");
            
            var onDialogueStartedField = typeof(DialogueManager).GetField("OnDialogueStarted", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var onLineDisplayedField = typeof(DialogueManager).GetField("OnLineDisplayed", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (onDialogueStartedField != null)
            {
                var evt = onDialogueStartedField.GetValue(DialogueManager.Instance);
                Debug.Log($"  - OnDialogueStarted: {evt}");
            }

            if (onLineDisplayedField != null)
            {
                var evt = onLineDisplayedField.GetValue(DialogueManager.Instance);
                Debug.Log($"  - OnLineDisplayed: {evt}");
            }
        }
    }
}
