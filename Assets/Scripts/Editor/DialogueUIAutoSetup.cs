#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Reflection;

/// <summary>
/// DialogueUI自动设置工具
/// </summary>
public class DialogueUIAutoSetup : EditorWindow
{
    [MenuItem("Tools/对话系统/自动配置 DialogueUI")]
    public static void AutoSetupDialogueUI()
    {
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>(true);
        if (dialogueUI == null)
        {
            Debug.LogError("[DialogueUIAutoSetup] 未找到DialogueUI组件！");
            return;
        }

        Debug.Log("[DialogueUIAutoSetup] 开始自动配置DialogueUI...");

        // 激活GameObject以便查找子对象
        bool wasActive = dialogueUI.gameObject.activeSelf;
        dialogueUI.gameObject.SetActive(true);

        // 使用反射设置私有字段
        System.Type dialogueUIType = typeof(DialogueUI);
        
        // 绑定UI元素
        SetField(dialogueUI, dialogueUIType, "characterNameText", FindChildComponent<Text>(dialogueUI.transform, "CharacterName"));
        SetField(dialogueUI, dialogueUIType, "dialogueText", FindChildComponent<Text>(dialogueUI.transform, "DialogueText"));
        SetField(dialogueUI, dialogueUIType, "characterPortrait", FindChildComponent<Image>(dialogueUI.transform, "CharacterPortrait"));
        SetField(dialogueUI, dialogueUIType, "continueButton", FindChildComponent<Button>(dialogueUI.transform, "ContinueButton"));
        SetField(dialogueUI, dialogueUIType, "continueIndicator", FindChildGameObject(dialogueUI.transform, "ContinueIndicator"));
        SetField(dialogueUI, dialogueUIType, "choiceButtonContainer", FindChildTransform(dialogueUI.transform, "ChoiceButtonContainer"));
        SetField(dialogueUI, dialogueUIType, "choiceButtonPrefab", FindChildComponent<Button>(dialogueUI.transform, "ChoiceButtonPrefab"));
        
        // 设置CanvasGroup
        CanvasGroup canvasGroup = dialogueUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = dialogueUI.gameObject.AddComponent<CanvasGroup>();
        }
        SetField(dialogueUI, dialogueUIType, "canvasGroup", canvasGroup);

        // 恢复原始激活状态
        dialogueUI.gameObject.SetActive(wasActive);

        // 标记为脏数据以保存更改
        EditorUtility.SetDirty(dialogueUI);

        Debug.Log("[DialogueUIAutoSetup] DialogueUI配置完成！");
    }

    private static void SetField(object target, System.Type type, string fieldName, object value)
    {
        FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(target, value);
            if (value != null)
            {
                Debug.Log($"[DialogueUIAutoSetup] 绑定 {fieldName}: {value.GetType().Name}");
            }
            else
            {
                Debug.LogWarning($"[DialogueUIAutoSetup] 未找到 {fieldName} 对应的UI元素");
            }
        }
    }

    private static T FindChildComponent<T>(Transform parent, string childName) where T : Component
    {
        Transform child = parent.Find(childName);
        return child != null ? child.GetComponent<T>() : null;
    }

    private static GameObject FindChildGameObject(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        return child != null ? child.gameObject : null;
    }

    private static Transform FindChildTransform(Transform parent, string childName)
    {
        return parent.Find(childName);
    }
}
#endif