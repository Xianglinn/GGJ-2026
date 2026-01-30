using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 对话 UI 布局配置工具
/// 自动设置 DialoguePanel 的正确布局
/// </summary>
public class DialogueUILayoutConfigurator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/对话系统/配置 DialogueUI 布局")]
    public static void ConfigureDialogueUILayout()
    {
        // 查找 DialoguePanel
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>(true);
        if (dialogueUI == null)
        {
            Debug.LogError("[DialogueUILayoutConfigurator] 未找到 DialogueUI 组件！");
            return;
        }

        GameObject dialoguePanel = dialogueUI.gameObject;
        RectTransform panelRect = dialoguePanel.GetComponent<RectTransform>();

        // 配置 DialoguePanel - 底部停靠，占据屏幕下方 35%
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 0.35f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panelRect.pivot = new Vector2(0.5f, 0);

        // 查找并配置 Background
        Transform background = dialoguePanel.transform.Find("Background");
        if (background != null)
        {
            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // 设置背景颜色为半透明黑色
            Image bgImage = background.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0, 0, 0, 0.8f);
            }
        }

        // 配置 CharacterName - 左上角
        Transform characterName = dialoguePanel.transform.Find("CharacterName");
        if (characterName != null)
        {
            RectTransform nameRect = characterName.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 1);
            nameRect.anchorMax = new Vector2(0, 1);
            nameRect.pivot = new Vector2(0, 1);
            nameRect.anchoredPosition = new Vector2(30, -20);
            nameRect.sizeDelta = new Vector2(400, 50);

            Text nameText = characterName.GetComponent<Text>();
            if (nameText != null)
            {
                nameText.fontSize = 28;
                nameText.fontStyle = FontStyle.Bold;
                nameText.color = Color.yellow;
                nameText.alignment = TextAnchor.MiddleLeft;
            }
        }

        // 配置 DialogueText - 主要对话区域
        Transform dialogueText = dialoguePanel.transform.Find("DialogueText");
        if (dialogueText != null)
        {
            RectTransform textRect = dialogueText.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(30, 30);
            textRect.offsetMax = new Vector2(-30, -80);

            Text text = dialogueText.GetComponent<Text>();
            if (text != null)
            {
                text.fontSize = 24;
                text.color = Color.white;
                text.alignment = TextAnchor.UpperLeft;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
            }
        }

        // 配置 CharacterPortrait - 左侧
        Transform portrait = dialoguePanel.transform.Find("CharacterPortrait");
        if (portrait != null)
        {
            RectTransform portraitRect = portrait.GetComponent<RectTransform>();
            portraitRect.anchorMin = new Vector2(0, 0);
            portraitRect.anchorMax = new Vector2(0, 1);
            portraitRect.pivot = new Vector2(0, 0.5f);
            portraitRect.anchoredPosition = new Vector2(30, 0);
            portraitRect.sizeDelta = new Vector2(200, 0);
            portrait.gameObject.SetActive(false); // 默认隐藏
        }

        // 配置 ContinueButton - 右下角
        Transform continueBtn = dialoguePanel.transform.Find("ContinueButton");
        if (continueBtn != null)
        {
            RectTransform btnRect = continueBtn.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(1, 0);
            btnRect.anchorMax = new Vector2(1, 0);
            btnRect.pivot = new Vector2(1, 0);
            btnRect.anchoredPosition = new Vector2(-30, 20);
            btnRect.sizeDelta = new Vector2(150, 50);

            // 添加 Text 子对象
            Transform btnText = continueBtn.Find("Text");
            if (btnText == null)
            {
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(continueBtn);
                btnText = textObj.transform;
                Text text = textObj.AddComponent<Text>();
                text.text = "继续 ►";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 20;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
            }

            // 设置按钮颜色
            Image btnImage = continueBtn.GetComponent<Image>();
            if (btnImage != null)
            {
                btnImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            }
        }

        // 配置 ChoiceButtonContainer - 中下区域
        Transform choiceContainer = dialoguePanel.transform.Find("ChoiceButtonContainer");
        if (choiceContainer != null)
        {
            RectTransform containerRect = choiceContainer.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0);
            containerRect.anchorMax = new Vector2(0.5f, 0);
            containerRect.pivot = new Vector2(0.5f, 0);
            containerRect.anchoredPosition = new Vector2(0, 30);
            containerRect.sizeDelta = new Vector2(800, 200);

            // 添加 VerticalLayoutGroup
            VerticalLayoutGroup layout = choiceContainer.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = choiceContainer.gameObject.AddComponent<VerticalLayoutGroup>();
            }
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
        }

        // 配置 ChoiceButtonPrefab
        Transform choicePrefab = dialoguePanel.transform.Find("ChoiceButtonPrefab");
        if (choicePrefab != null)
        {
            RectTransform prefabRect = choicePrefab.GetComponent<RectTransform>();
            prefabRect.sizeDelta = new Vector2(700, 60);

            // 添加 Text 子对象
            Transform prefabText = choicePrefab.Find("Text");
            if (prefabText == null)
            {
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(choicePrefab);
                prefabText = textObj.transform;
                Text text = textObj.AddComponent<Text>();
                text.text = "选项文本";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 22;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
            }

            // 设置按钮颜色
            Image prefabImage = choicePrefab.GetComponent<Image>();
            if (prefabImage != null)
            {
                prefabImage.color = new Color(0.1f, 0.3f, 0.5f, 0.9f);
            }

            choicePrefab.gameObject.SetActive(false); // 确保模板隐藏
        }

        EditorUtility.SetDirty(dialoguePanel);
        Debug.Log("[DialogueUILayoutConfigurator] DialogueUI 布局配置完成！");
    }
#endif
}
