using System.Collections.Generic;
using UnityEngine;

// 蓝图控制器：统计槽位效果并计算触发
public class BlueprintController : MonoBehaviour
{
    [SerializeField] private List<BlueprintSlot> slots = new List<BlueprintSlot>(); // 蓝图槽位列表
    [SerializeField] private int sameEffectRequired = 2; // 触发所需相同效果数

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
