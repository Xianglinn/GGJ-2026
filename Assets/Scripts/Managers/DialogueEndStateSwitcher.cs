using UnityEngine;

/// <summary>
/// 负责在一段对话结束后，根据当前阶段切换 GameState
/// 场景2（序章）可配置为切到 Gameplay，场景4（尾声）配置为切到 LevelMap。
/// </summary>
public class DialogueEndStateSwitcher : MonoBehaviour
{
    [Header("对话结束后切换到的目标状态")]
    public GameState targetStateOnEnd = GameState.Gameplay;

    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded.AddListener(OnDialogueEnded);
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnDialogueEnded);
        }
    }

    private void OnDialogueEnded()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SwitchState(targetStateOnEnd);
        }
    }
}