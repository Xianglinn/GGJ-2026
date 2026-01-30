using UnityEngine;
using UnityEditor;

/// <summary>
/// 对话系统的编辑器菜单命令
/// 提供快速触发对话测试的菜单项
/// </summary>
public class DialogueEditorMenu
{
    [MenuItem("Tools/对话系统/Play Mode 测试/触发欢迎对话")]
    public static void TriggerWelcomeDialogue()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueEditorMenu] 请先进入 Play Mode！");
            return;
        }

        DialogueTester tester = Object.FindObjectOfType<DialogueTester>();
        if (tester != null)
        {
            tester.StartTestDialogue();
            Debug.Log("[DialogueEditorMenu] 已触发测试对话");
        }
        else
        {
            Debug.LogError("[DialogueEditorMenu] 未找到 DialogueTester 组件！");
        }
    }

    [MenuItem("Tools/对话系统/Play Mode 测试/触发路径 A")]
    public static void TriggerPathA()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueEditorMenu] 请先进入 Play Mode！");
            return;
        }

        DialogueTester tester = Object.FindObjectOfType<DialogueTester>();
        if (tester != null)
        {
            tester.TestPathA();
            Debug.Log("[DialogueEditorMenu] 已触发路径 A");
        }
        else
        {
            Debug.LogError("[DialogueEditorMenu] 未找到 DialogueTester 组件！");
        }
    }

    [MenuItem("Tools/对话系统/Play Mode 测试/触发路径 B")]
    public static void TriggerPathB()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueEditorMenu] 请先进入 Play Mode！");
            return;
        }

        DialogueTester tester = Object.FindObjectOfType<DialogueTester>();
        if (tester != null)
        {
            tester.TestPathB();
            Debug.Log("[DialogueEditorMenu] 已触发路径 B");
        }
        else
        {
            Debug.LogError("[DialogueEditorMenu] 未找到 DialogueTester 组件！");
        }
    }

    [MenuItem("Tools/对话系统/Play Mode 测试/结束当前对话")]
    public static void EndCurrentDialogue()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("[DialogueEditorMenu] 请先进入 Play Mode！");
            return;
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.EndDialogue();
            Debug.Log("[DialogueEditorMenu] 已结束对话");
        }
        else
        {
            Debug.LogError("[DialogueEditorMenu] DialogueManager 未初始化！");
        }
    }
}
