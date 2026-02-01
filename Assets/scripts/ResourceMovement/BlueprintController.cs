using System.Collections.Generic;
using UnityEngine;

// 蓝图控制器：统计槽位效果并计算触发
public class BlueprintController : MonoBehaviour
{
    [SerializeField] private List<BlueprintSlot> slots = new List<BlueprintSlot>(); // 蓝图槽位列表
    [SerializeField] private int sameEffectRequired = 2; // 触发所需相同效果数

    public event System.Action<bool> OnCompletionChanged;

    private bool isComplete;

    public bool IsComplete => isComplete;

    private void Awake(){
        BindSlots();
        UpdateCompletionState(true);
    }

    private void OnEnable(){
        UpdateCompletionState(true);
    }

    private void OnValidate(){
        BindSlots();
    }

    // 绑定控制器到各槽位
    private void BindSlots(){
        if(slots == null)
        {
            return;
        }
        foreach(BlueprintSlot slot in slots)
        {
            if(slot == null)
            {
                continue;
            }
            slot.SetController(this);
        }
    }

    // 槽位变化时调用
    public void NotifySlotChanged(){
        UpdateCompletionState(false);
    }

    // 判断是否全部槽位已填满
    public bool AreAllSlotsFilled(){
        if(slots == null || slots.Count == 0)
        {
            return false;
        }
        foreach(BlueprintSlot slot in slots)
        {
            if(slot == null)
            {
                return false;
            }
            if(slot.GetItemInfo() == null)
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateCompletionState(bool forceNotify){
        bool allSlotsFilled = AreAllSlotsFilled();
        List<SpecialEffectType> triggered = GetTriggeredEffects();
        
        // 判定条件：五个槽位都有物品即可切换 (不再强制要求特殊效果)
        bool complete = allSlotsFilled;

        if(forceNotify || complete != isComplete)
        {
            isComplete = complete;
            if(isComplete)
            {
                // 获取触发的特效（如果有的话，否则为 None）
                SpecialEffectType mainEffect = triggered.Count > 0 ? triggered[0] : SpecialEffectType.None;
                Debug.Log($"[BlueprintController] Complete! Main effect: {mainEffect}");

                if (DataManager.Instance != null)
                {
                    // 只有在触发了特殊效果时才记录解锁
                    if (mainEffect != SpecialEffectType.None)
                    {
                        DataManager.Instance.RecordSpecialEffectUnlock(mainEffect);
                        
                        string flagKey = "Effect_Unlocked_" + mainEffect.ToString();
                        if (!DataManager.Instance.GetStoryFlag(flagKey))
                        {
                            DataManager.Instance.SetStoryFlag(flagKey, true);
                            Debug.Log($"[BlueprintController] First time unlocking effect: {mainEffect}");
                        }
                    }
                    
                    // 总是记录面具制作个数
                    DataManager.Instance.RecordMaskCrafted();
                }

                if (GameFlowManager.Instance != null)
                {
                    // 设置当前运行的特效供 Scene4 使用
                    GameFlowManager.Instance.LastTriggeredEffect = mainEffect;

                    Debug.Log("Blueprint Complete! Switching to Epilogue.");
                    GameFlowManager.Instance.SwitchState(GameState.Epilogue);
                }
                else
                {
                    GameFlowManager.Instance.LastTriggeredEffect = SpecialEffectType.None;
                   Debug.LogError("GameFlowManager instance not found!");
                }
            }
            OnCompletionChanged?.Invoke(isComplete);
        }
    }

    // 统计所有槽位的特效数量
    public Dictionary<SpecialEffectType, int> CollectEffectCounts(){
        Dictionary<SpecialEffectType, int> counts = new Dictionary<SpecialEffectType, int>();
        foreach(BlueprintSlot slot in slots)
        {
            if(slot == null)
            {
                continue;
            }

            ItemInfo info = slot.GetItemInfo();
            if(info == null)
            {
                continue;
            }

            SpecialEffectType effects = info.SpecialEffects;
            if(effects == SpecialEffectType.None)
            {
                continue;
            }

            foreach(SpecialEffectType effect in GetFlags(effects))
            {
                if(!counts.ContainsKey(effect))
                {
                    counts[effect] = 0;
                }
                counts[effect] += 1;
            }
        }
        return counts;
    }

    // 获取满足条件的特效列表
    public List<SpecialEffectType> GetTriggeredEffects(){
        List<SpecialEffectType> triggered = new List<SpecialEffectType>();
        Dictionary<SpecialEffectType, int> counts = CollectEffectCounts();
        foreach(var kvp in counts)
        {
            if(kvp.Value >= sameEffectRequired)
            {
                triggered.Add(kvp.Key);
            }
        }
        return triggered;
    }

    // 拆分多选特效枚举
    private IEnumerable<SpecialEffectType> GetFlags(SpecialEffectType value){
        foreach(SpecialEffectType effect in System.Enum.GetValues(typeof(SpecialEffectType)))
        {
            if(effect == SpecialEffectType.None)
            {
                continue;
            }

            if(value.HasFlag(effect))
            {
                yield return effect;
            }
        }
    }
}
