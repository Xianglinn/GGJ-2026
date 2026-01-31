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
    小女孩的珍藏 = 1 << 0,
    井中之天 = 1 << 1,
    魔女的面具 = 1 << 2
}
