using System;

// 面具部件类型
public enum MaskPartType
{
    Crystal,
    Tassel,
    PatternA,
    PatternB,
    MetalFrame
}

[Flags]
// 物品特效类型（可叠加）
public enum SpecialEffectType
{
    None = 0,
    Fire = 1 << 0,
    Ice = 1 << 1,
    Wind = 1 << 2,
    Thunder = 1 << 3,
    Healing = 1 << 4
}
