using UnityEngine;

public class SnapPointConfig : MonoBehaviour
{
    [SerializeField] private float targetScale = 1f;

    public float TargetScale => targetScale;
}
