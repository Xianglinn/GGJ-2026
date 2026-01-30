using UnityEngine;

/// <summary>
/// 综合对话系统测试脚本
/// 用于验证 DialogueManager 的所有功能
/// </summary>
public class ComprehensiveDialogueTester : MonoBehaviour
{
    [Header("测试配置")]
    [SerializeField] private bool runTestsOnStart = false;
    [SerializeField] private bool verboseLogging = true;

    private int testsPassed = 0;
    private int testsFailed = 0;

    private void Start()
    {
        if (runTestsOnStart)
        {
            StartCoroutine(RunAllTests());
        }
    }

    [ContextMenu("运行所有测试")]
    public void RunAllTestsMenu()
    {
        StartCoroutine(RunAllTests());
    }

    private System.Collections.IEnumerator RunAllTests()
    {
        Log("=== 开始对话系统综合测试 ===");
        testsPassed = 0;
        testsFailed = 0;

        yield return new WaitForSeconds(0.5f);

        // 测试 1: 管理器初始化检查
        TestManagerInitialization();
        yield return new WaitForSeconds(0.5f);

        // 测试 2: 对话数据加载
        TestDialogueDataLoading();
        yield return new WaitForSeconds(0.5f);

        // 测试 3: 对话开始和事件触发
        yield return TestDialogueStartAndEvents();
        yield return new WaitForSeconds(1f);

        // 测试 4: 对话进度
        yield return TestDialogueProgression();
        yield return new WaitForSeconds(1f);

        // 测试 5: UI 集成
        TestUIIntegration();
        yield return new WaitForSeconds(0.5f);

        // 输出测试结果
        Log($"=== 测试完成 ===");
        Log($"通过: {testsPassed} | 失败: {testsFailed}");
    }

    private void TestManagerInitialization()
    {
        Log("\n--- 测试 1: 管理器初始化检查 ---");

        // 检查 DialogueManager
        if (DialogueManager.Instance != null)
        {
            Pass("DialogueManager 已初始化");
        }
        else
        {
            Fail("DialogueManager 未找到");
            return;
        }

        // 检查 UIManager
        if (UIManager.Instance != null)
        {
            Pass("UIManager 已初始化");
        }
        else
        {
            Fail("UIManager 未找到");
        }

        // 检查 ResourceManager
        if (ResourceManager.Instance != null)
        {
            Pass("ResourceManager 已初始化");
        }
        else
        {
            Fail("ResourceManager 未找到");
        }

        // 检查可选管理器
        if (AudioManager.Instance != null)
        {
            Pass("AudioManager 已初始化 (可选)");
        }
        else
        {
            Log("AudioManager 未找到 (可选，不影响测试)");
        }

        if (DataManager.Instance != null)
        {
            Pass("DataManager 已初始化 (可选)");
        }
        else
        {
            Log("DataManager 未找到 (可选，不影响测试)");
        }
    }

    private void TestDialogueDataLoading()
    {
        Log("\n--- 测试 2: 对话数据加载 ---");

        if (ResourceManager.Instance == null)
        {
            Fail("ResourceManager 不可用，跳过测试");
            return;
        }

        // 测试加载 Welcome 对话
        DialogueData welcomeDialogue = ResourceManager.Instance.Load<DialogueData>("Dialogue/TestDialogue_Welcome");
        if (welcomeDialogue != null)
        {
            Pass($"成功加载 TestDialogue_Welcome (ID: {welcomeDialogue.dialogueID})");
            
            if (welcomeDialogue.Validate())
            {
                Pass("Welcome 对话数据验证通过");
            }
            else
            {
                Fail("Welcome 对话数据验证失败");
            }
        }
        else
        {
            Fail("无法加载 TestDialogue_Welcome");
        }

        // 测试加载 PathA 对话
        DialogueData pathADialogue = ResourceManager.Instance.Load<DialogueData>("Dialogue/TestDialogue_PathA");
        if (pathADialogue != null)
        {
            Pass($"成功加载 TestDialogue_PathA (ID: {pathADialogue.dialogueID})");
        }
        else
        {
            Fail("无法加载 TestDialogue_PathA");
        }

        // 测试加载 PathB 对话
        DialogueData pathBDialogue = ResourceManager.Instance.Load<DialogueData>("Dialogue/TestDialogue_PathB");
        if (pathBDialogue != null)
        {
            Pass($"成功加载 TestDialogue_PathB (ID: {pathBDialogue.dialogueID})");
        }
        else
        {
            Fail("无法加载 TestDialogue_PathB");
        }
    }

    private System.Collections.IEnumerator TestDialogueStartAndEvents()
    {
        Log("\n--- 测试 3: 对话开始和事件触发 ---");

        if (DialogueManager.Instance == null || ResourceManager.Instance == null)
        {
            Fail("必要的管理器不可用，跳过测试");
            yield break;
        }

        DialogueData testDialogue = ResourceManager.Instance.Load<DialogueData>("Dialogue/TestDialogue_Welcome");
        if (testDialogue == null)
        {
            Fail("无法加载测试对话数据");
            yield break;
        }

        bool eventTriggered = false;
        void OnDialogueStarted(DialogueData data)
        {
            eventTriggered = true;
            Log($"对话开始事件触发: {data.dialogueID}");
        }

        DialogueManager.Instance.OnDialogueStarted.AddListener(OnDialogueStarted);
        DialogueManager.Instance.StartDialogue(testDialogue);

        yield return new WaitForSeconds(0.2f);

        if (eventTriggered)
        {
            Pass("对话开始事件正确触发");
        }
        else
        {
            Fail("对话开始事件未触发");
        }

        if (DialogueManager.Instance.IsDialogueActive)
        {
            Pass("对话状态已激活");
        }
        else
        {
            Fail("对话状态未激活");
        }

        DialogueManager.Instance.OnDialogueStarted.RemoveListener(OnDialogueStarted);
    }

    private System.Collections.IEnumerator TestDialogueProgression()
    {
        Log("\n--- 测试 4: 对话进度 ---");

        if (DialogueManager.Instance == null)
        {
            Fail("DialogueManager 不可用，跳过测试");
            yield break;
        }

        if (!DialogueManager.Instance.IsDialogueActive)
        {
            Log("没有激活的对话，启动新对话");
            DialogueData testDialogue = ResourceManager.Instance.Load<DialogueData>("Dialogue/TestDialogue_Welcome");
            if (testDialogue != null)
            {
                DialogueManager.Instance.StartDialogue(testDialogue);
                yield return new WaitForSeconds(0.2f);
            }
        }

        if (DialogueManager.Instance.IsDialogueActive)
        {
            float initialProgress = DialogueManager.Instance.GetProgress();
            Log($"初始进度: {initialProgress * 100:F0}%");

            // 测试进度推进
            if (DialogueManager.Instance.IsWaitingForInput())
            {
                DialogueManager.Instance.ShowNextLine();
                yield return new WaitForSeconds(0.2f);

                float newProgress = DialogueManager.Instance.GetProgress();
                if (newProgress > initialProgress)
                {
                    Pass($"对话进度正确推进: {initialProgress * 100:F0}% -> {newProgress * 100:F0}%");
                }
                else
                {
                    Fail("对话进度未推进");
                }
            }
            else
            {
                Log("对话未在等待输入状态");
            }
        }
        else
        {
            Fail("无法测试对话进度：对话未激活");
        }
    }

    private void TestUIIntegration()
    {
        Log("\n--- 测试 5: UI 集成 ---");

        if (UIManager.Instance == null)
        {
            Fail("UIManager 不可用，跳过测试");
            return;
        }

        // 检查 DialogueUI 是否已注册
        if (UIManager.Instance.IsPanelRegistered<DialogueUI>())
        {
            Pass("DialogueUI 已注册到 UIManager");

            DialogueUI dialogueUI = UIManager.Instance.GetPanel<DialogueUI>();
            if (dialogueUI != null)
            {
                Pass("成功获取 DialogueUI 实例");
            }
            else
            {
                Fail("无法获取 DialogueUI 实例");
            }
        }
        else
        {
            Fail("DialogueUI 未注册到 UIManager");
        }
    }

    private void Pass(string message)
    {
        testsPassed++;
        Log($"<color=green>[✓]</color> {message}");
    }

    private void Fail(string message)
    {
        testsFailed++;
        Log($"<color=red>[✗]</color> {message}");
    }

    private void Log(string message)
    {
        if (verboseLogging)
        {
            Debug.Log($"[ComprehensiveDialogueTester] {message}");
        }
    }
}
