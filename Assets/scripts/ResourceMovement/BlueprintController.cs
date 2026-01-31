using System.Collections.Generic;
using UnityEngine;

public class BlueprintController : MonoBehaviour
{
    [SerializeField] private List<BlueprintSlot> slots = new List<BlueprintSlot>();
    [SerializeField] private int sameEffectRequired = 2;

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
