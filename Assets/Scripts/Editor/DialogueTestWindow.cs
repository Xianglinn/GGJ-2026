using UnityEngine;
using UnityEditor;

/// <summary>
/// 对话系统测试工具 - 编辑器窗口
/// 用于快速测试对话系统功能
/// </summary>
public class DialogueTestWindow : EditorWindow
{
    private DialogueData testDialogue;
    private bool autoStart = false;

    [MenuItem("Tools/对话系统/Dialogue Test Window")]
    public static void ShowWindow()
    {
        GetWindow<DialogueTestWindow>("Dialogue Test");
    }

    private void OnGUI()
    {
        GUILayout.Label("对话系统测试工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        testDialogue = (DialogueData)EditorGUILayout.ObjectField("测试对话数据", testDialogue, typeof(DialogueData), false);
        autoStart = EditorGUILayout.Toggle("自动开始", autoStart);

        EditorGUILayout.Space();

        if (GUILayout.Button("开始测试对话"))
        {
            StartTestDialogue();
        }

        EditorGUILayout.Space();
        GUILayout.Label("快速测试", EditorStyles.boldLabel);

        if (GUILayout.Button("测试 Welcome 对话"))
        {
            LoadAndTest("Dialogue/TestDialogue_Welcome");
        }

        if (GUILayout.Button("测试 Path A 对话"))
        {
            LoadAndTest("Dialogue/TestDialogue_PathA");
        }

        if (GUILayout.Button("测试 Path B 对话"))
        {
            LoadAndTest("Dialogue/TestDialogue_PathB");
        }

        EditorGUILayout.Space();
        GUILayout.Label("调试信息", EditorStyles.boldLabel);

        if (Application.isPlaying)
        {
            if (DialogueManager.Instance != null)
            {
                EditorGUILayout.LabelField("DialogueManager", "已初始化");
                EditorGUILayout.LabelField("对话激活", DialogueManager.Instance.IsDialogueActive.ToString());
                if (DialogueManager.Instance.IsDialogueActive)
                {
                    EditorGUILayout.LabelField("当前进度", $"{DialogueManager.Instance.GetProgress() * 100:F0}%");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("DialogueManager 未找到", MessageType.Warning);
            }

            if (UIManager.Instance != null)
            {
                EditorGUILayout.LabelField("UIManager", "已初始化");
                EditorGUILayout.LabelField("DialogueUI 已注册", UIManager.Instance.IsPanelRegistered<DialogueUI>().ToString());
            }
            else
            {
                EditorGUILayout.HelpBox("UIManager 未找到", MessageType.Warning);
            }

            if (ResourceManager.Instance != null)
            {
                EditorGUILayout.LabelField("ResourceManager", "已初始化");
            }
            else
            {
                EditorGUILayout.HelpBox("ResourceManager 未找到", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("请先进入播放模式", MessageType.Info);
        }
    }

    private void StartTestDialogue()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("错误", "请先进入播放模式", "确定");
            return;
        }

        if (testDialogue == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择测试对话数据", "确定");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            EditorUtility.DisplayDialog("错误", "DialogueManager 未找到", "确定");
            return;
        }


        Debug.Log($"[DialogueTestWindow] Starting test dialogue: {testDialogue.dialogueID}");
        
        // 重要：先显示 UI，再开始对话
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPanel<DialogueUI>();
            
            // 使用 MonoBehaviour 的协程而不是 EditorApplication.delayCall
            // 找到场景中的任意 MonoBehaviour 来启动协程
            var helper = FindObjectOfType<DialogueManager>();
            if (helper != null)
            {
                helper.StartCoroutine(StartDialogueAfterDelay(testDialogue));
            }
            else
            {
                Debug.LogError("[DialogueTestWindow] Cannot find MonoBehaviour to start coroutine!");
            }
        }
    }

    private System.Collections.IEnumerator StartDialogueAfterDelay(DialogueData dialogue)
    {
        yield return null;
        
        if (DialogueManager.Instance != null && dialogue != null)
        {
            Debug.Log($"[DialogueTestWindow] Now starting dialogue: {dialogue.dialogueID}");
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    private void LoadAndTest(string path)
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("错误", "请先进入播放模式", "确定");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            EditorUtility.DisplayDialog("错误", "ResourceManager 未找到", "确定");
            return;
        }

        DialogueData dialogue = ResourceManager.Instance.Load<DialogueData>(path);
        if (dialogue != null)
        {
            testDialogue = dialogue;
            StartTestDialogue();
        }
        else
        {
            EditorUtility.DisplayDialog("错误", $"无法加载对话数据: {path}", "确定");
        }
    }
}
