using UnityEngine;

/// <summary>
/// 原材料数据结构：用于原材料搜集与面具拼图逻辑的一一对应
/// </summary>
[System.Serializable]
public class ItemData
{
    [Header("基础信息")]
    [Tooltip("原材料唯一 ID（与场景物体、拼图槽一一对应）")]
    public string itemId;

    [Header("UI 显示")]
    [Tooltip("素材栏中显示的缩略图精灵")]
    public Sprite iconSprite;

    [Header("面具拼图参数")]
    [Tooltip("该原材料在面具上的目标局部坐标（以面具 RectTransform 为参考，单位：px）")]
    public Vector2 targetMaskLocalPosition;

    [Tooltip("允许投放的误差半径（像素），用于拖拽判定容错")]
    public float placeToleranceRadius = 40f;
}