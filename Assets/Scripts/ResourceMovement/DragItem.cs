using UnityEngine;

public class DragItem : MonoBehaviour
{
    private Vector2 startPosition; // 初始位置
    private Vector3 startScale; // 初始缩放
    private Vector2 baseItemSize; // 物体初始尺寸
    private bool hasBaseItemSize; // 是否有初始尺寸

    [SerializeField] private Transform[] snapPoints; // 可吸附的位置
    [SerializeField] private float snapDistance = 0.5f; // 吸附距离阈值
    [SerializeField] private bool isFinished; // 是否已锁定
    private Vector2 lastLockedPosition; // 上次锁定位置
    private Vector3 lastLockedScale; // 上次锁定缩放
    private bool hasLockedPosition; // 是否已有锁定点
    private bool isDragging; // 是否正在拖拽
    private Vector3 preDragScale; // 拖拽前缩放
    [SerializeField] private float dragScaleFactor = 0.9f; // 拖拽时缩小比例

    // 缓存初始位置
    private void Start(){
        startPosition = transform.position;
        startScale = transform.localScale;
        lastLockedScale = startScale;
        hasBaseItemSize = TryGetBoxWorldSize(transform, out baseItemSize);
    }

    // 鼠标按下开始拖拽
    private void OnMouseDown(){
        isFinished = false;
        isDragging = true;
        preDragScale = transform.localScale;
        transform.localScale = preDragScale * dragScaleFactor;
    }

    // 拖拽时跟随鼠标
    private void OnMouseDrag(){
        if(isDragging)
        {
            transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 
                                            Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            transform.localScale = lastLockedScale * 0.9f;              
        } 
    }

    // 吸附到最近点或回退
    private void OnMouseUp(){
        isDragging = false;

        Transform nearestPoint = null;
        float nearestDistance = float.MaxValue;

        if(snapPoints != null)
        {
            for(int i = 0; i < snapPoints.Length; i++)
            {
                Transform point = snapPoints[i];
                if(point == null)
                {
                    continue;
                }

                float distance = Vector2.Distance(transform.position, point.position);
                if(distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPoint = point;
                }
            }
        }

        if(nearestPoint != null && nearestDistance <= snapDistance)
        {
            isFinished = true;
            lastLockedPosition = nearestPoint.position;
            hasLockedPosition = true;
            transform.position = lastLockedPosition;
            lastLockedScale = startScale;
            float scaleFactor = 1f;
            Vector2 targetSize;
            if(hasBaseItemSize && TryGetBoxWorldSize(nearestPoint, out targetSize))
            {
                float baseAxis = Mathf.Max(baseItemSize.x, baseItemSize.y);
                float targetAxis = Mathf.Max(targetSize.x, targetSize.y);
                if(baseAxis > 0f)
                {
                    scaleFactor = targetAxis / baseAxis;
                }
            }
            else
            {
                SnapPointConfig config = nearestPoint.GetComponent<SnapPointConfig>();
                if(config != null)
                {
                    scaleFactor = config.TargetScale;
                }
            }
            lastLockedScale = startScale * scaleFactor;
            transform.localScale = lastLockedScale;
            return;
        }

        if(hasLockedPosition)
        {
            transform.position = lastLockedPosition;
            transform.localScale = lastLockedScale;
        }
        else
        {
            transform.position = startPosition;
            transform.localScale = startScale;
        }
    }

    // 获取目标的BoxCollider世界尺寸
    private bool TryGetBoxWorldSize(Transform target, out Vector2 size){
        BoxCollider2D box2D = target.GetComponent<BoxCollider2D>();
        if(box2D != null)
        {
            Vector3 boundsSize = box2D.bounds.size;
            size = new Vector2(boundsSize.x, boundsSize.y);
            return true;
        }

        BoxCollider box3D = target.GetComponent<BoxCollider>();
        if(box3D != null)
        {
            Vector3 boundsSize = box3D.bounds.size;
            size = new Vector2(boundsSize.x, boundsSize.y);
            return true;
        }

        size = Vector2.zero;
        return false;
    }
}
