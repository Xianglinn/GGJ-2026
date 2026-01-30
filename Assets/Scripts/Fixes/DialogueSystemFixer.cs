using UnityEngine;
using System.Reflection;

/// <summary>
/// 对话系统修复工具
/// 解决单例状态异常和UI显示问题
/// </summary>
public class DialogueSystemFixer : MonoBehaviour
{
    [Header("修复选项")]
    [Tooltip("启动时自动修复")]
    public bool autoFixOnStart = true;
    
    [Tooltip("重置单例状态")]
    public bool resetSingletonState = true;
    
    [Tooltip("重新绑定UI元素")]
    public bool rebindUIElements = true;
    
    [Tooltip("测试对话数据")]
    public DialogueData testDialogue;

    private void Start()
    {
        if (autoFixOnStart)
        {
            FixDialogueSystem();
        }
    }

    /// <summary>
    /// 修复对话系统
    /// </summary>
    [ContextMenu("修复对话系统")]
    public void FixDialogueSystem()
    {
        Debug.Log("[DialogueSystemFixer] 开始修复对话系统...");

        if (resetSingletonState)
        {
            ResetSingletonStates();
        }

        if (rebindUIElements)
        {
            RebindUIElements();
        }

        ValidateSystemIntegrity();
        
        Debug.Log("[DialogueSystemFixer] 对话系统修复完成！");
    }

    /// <summary>
    /// 重置单例状态
    /// </summary>
    private void ResetSingletonStates()
    {
        Debug.Log("[DialogueSystemFixer] 重置单例状态...");

        // 重置DialogueManager单例状态
        ResetSingletonState<DialogueManager>();
        
        // 重置UIManager单例状态
        ResetSingletonState<UIManager>();
        
        // 重置其他管理器
        ResetSingletonState<ResourceManager>();
        ResetSingletonState<DataManager>();
        ResetSingletonState<AudioManager>();
    }

    /// <summary>
    /// 重置指定类型的单例状态
    /// </summary>
    private void ResetSingletonState<T>() where T : MonoBehaviour
    {
        try
        {
            System.Type singletonType = typeof(T);
            
            // 使用反射重置_applicationIsQuitting标志
            FieldInfo applicationQuittingField = singletonType.BaseType.GetField("_applicationIsQuitting", 
                BindingFlags.NonPublic | BindingFlags.Static);
            
            if (applicationQuittingField != null)
            {
                applicationQuittingField.SetValue(null, false);
                Debug.Log($"[DialogueSystemFixer] 重置 {singletonType.Name} 的退出标志");
            }

            // 验证实例是否可用
            PropertyInfo instanceProperty = singletonType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instanceProperty != null)
            {
                T instance = instanceProperty.GetValue(null) as T;
                if (instance != null)
                {
                    Debug.Log($"[DialogueSystemFixer] {singletonType.Name} 实例已恢复");
                }
                else
                {
                    Debug.LogWarning($"[DialogueSystemFixer] {singletonType.Name} 实例仍为null");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DialogueSystemFixer] 重置 {typeof(T).Name} 单例状态失败: {e.Message}");
        }
    }

    /// <summary>
    /// 重新绑定UI元素
    /// </summary>
    private void RebindUIElements()
    {
        Debug.Log("[DialogueSystemFixer] 重新绑定UI元素...");

        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>(true);
        if (dialogueUI == null)
        {
            Debug.LogError("[DialogueSystemFixer] 未找到DialogueUI组件！");
            return;
        }

        // 确保DialogueUI已注册到UIManager
        if (UIManager.Instance != null)
        {
            if (!UIManager.Instance.IsPanelRegistered<DialogueUI>())
            {
                UIManager.Instance.RegisterPanel(dialogueUI);
                Debug.Log("[DialogueSystemFixer] 重新注册DialogueUI到UIManager");
            }
        }

        // 激活DialoguePanel以便检查绑定
        GameObject dialoguePanel = dialogueUI.gameObject;
        bool wasActive = dialoguePanel.activeSelf;
        dialoguePanel.SetActive(true);

        // 验证UI元素绑定
        ValidateUIBindings(dialogueUI);

        // 恢复原始激活状态
        dialoguePanel.SetActive(wasActive);
    }

    /// <summary>
    /// 验证UI绑定
    /// </summary>
    private void ValidateUIBindings(DialogueUI dialogueUI)
    {
        System.Type dialogueUIType = typeof(DialogueUI);
        
        // 检查关键UI元素字段
        string[] criticalFields = { "characterNameText", "dialogueText", "characterPortrait", 
                                   "continueButton", "continueIndicator", "choiceButtonContainer", 
                                   "choiceButtonPrefab", "canvasGroup" };

        foreach (string fieldName in criticalFields)
        {
            FieldInfo field = dialogueUIType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                object value = field.GetValue(dialogueUI);
                if (value == null)
                {
                    Debug.LogWarning($"[DialogueSystemFixer] {fieldName} 未绑定！");
                    
                    // 尝试自动查找并绑定
                    TryAutoBindField(dialogueUI, field, fieldName);
                }
                else
                {
                    Debug.Log($"[DialogueSystemFixer] {fieldName} 绑定正常");
                }
            }
        }
    }

    /// <summary>
    /// 尝试自动绑定字段
    /// </summary>
    private void TryAutoBindField(DialogueUI dialogueUI, FieldInfo field, string fieldName)
    {
        Transform dialoguePanel = dialogueUI.transform;
        
        // 根据字段名查找对应的子对象
        string childName = GetChildNameForField(fieldName);
        Transform child = dialoguePanel.Find(childName);
        
        if (child != null)
        {
            object component = GetComponentForField(child, field.FieldType);
            if (component != null)
            {
                field.SetValue(dialogueUI, component);
                Debug.Log($"[DialogueSystemFixer] 自动绑定 {fieldName} 成功");
            }
        }
    }

    /// <summary>
    /// 根据字段名获取子对象名称
    /// </summary>
    private string GetChildNameForField(string fieldName)
    {
        switch (fieldName)
        {
            case "characterNameText": return "CharacterName";
            case "dialogueText": return "DialogueText";
            case "characterPortrait": return "CharacterPortrait";
            case "continueButton": return "ContinueButton";
            case "continueIndicator": return "ContinueIndicator";
            case "choiceButtonContainer": return "ChoiceButtonContainer";
            case "choiceButtonPrefab": return "ChoiceButtonPrefab";
            default: return fieldName;
        }
    }

    /// <summary>
    /// 根据字段类型获取对应组件
    /// </summary>
    private object GetComponentForField(Transform target, System.Type fieldType)
    {
        if (fieldType == typeof(UnityEngine.UI.Text))
            return target.GetComponent<UnityEngine.UI.Text>();
        else if (fieldType == typeof(UnityEngine.UI.Image))
            return target.GetComponent<UnityEngine.UI.Image>();
        else if (fieldType == typeof(UnityEngine.UI.Button))
            return target.GetComponent<UnityEngine.UI.Button>();
        else if (fieldType == typeof(GameObject))
            return target.gameObject;
        else if (fieldType == typeof(Transform))
            return target;
        else if (fieldType == typeof(CanvasGroup))
            return target.GetComponent<CanvasGroup>();
        
        return null;
    }

    /// <summary>
    /// 验证系统完整性
    /// </summary>
    private void ValidateSystemIntegrity()
    {
        Debug.Log("[DialogueSystemFixer] 验证系统完整性...");

        // 检查管理器实例
        bool dialogueManagerOK = DialogueManager.Instance != null;
        bool uiManagerOK = UIManager.Instance != null;
        bool resourceManagerOK = ResourceManager.Instance != null;

        Debug.Log($"[DialogueSystemFixer] DialogueManager: {(dialogueManagerOK ? "正常" : "异常")}");
        Debug.Log($"[DialogueSystemFixer] UIManager: {(uiManagerOK ? "正常" : "异常")}");
        Debug.Log($"[DialogueSystemFixer] ResourceManager: {(resourceManagerOK ? "正常" : "异常")}");

        // 检查DialogueUI
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>(true);
        bool dialogueUIOK = dialogueUI != null;
        Debug.Log($"[DialogueSystemFixer] DialogueUI: {(dialogueUIOK ? "正常" : "异常")}");

        if (dialogueManagerOK && uiManagerOK && dialogueUIOK)
        {
            Debug.Log("[DialogueSystemFixer] 系统完整性验证通过！");
        }
        else
        {
            Debug.LogError("[DialogueSystemFixer] 系统完整性验证失败！");
        }
    }

    /// <summary>
    /// 测试对话系统
    /// </summary>
    [ContextMenu("测试对话系统")]
    public void TestDialogueSystem()
    {
        if (testDialogue == null)
        {
            Debug.LogError("[DialogueSystemFixer] 未设置测试对话数据！");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[DialogueSystemFixer] DialogueManager不可用！");
            return;
        }

        if (UIManager.Instance == null)
        {
            Debug.LogError("[DialogueSystemFixer] UIManager不可用！");
            return;
        }

        Debug.Log("[DialogueSystemFixer] 开始测试对话系统...");
        
        // 开始对话
        DialogueManager.Instance.StartDialogue(testDialogue);
        
        // 显示UI
        UIManager.Instance.ShowPanel<DialogueUI>();
    }
}