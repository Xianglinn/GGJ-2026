using System;

public enum MaskPartType
{
    Crystal,
    Tassel,
    PatternA,
    PatternB,
    MetalFrame
}

[Flags]
public enum SpecialEffectType
{
    None = 0,
    Fire = 1 << 0,
    Ice = 1 << 1,
    Wind = 1 << 2,
    Thunder = 1 << 3,
    Healing = 1 << 4
}
