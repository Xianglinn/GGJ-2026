using UnityEngine;

public class DragItem : MonoBehaviour
{
    private Vector2 startPosition;

    [SerializeField] private Transform[] snapPoints;
    [SerializeField] private float snapDistance = 0.5f;
    [SerializeField] private bool isFinished;
    private Vector2 lastLockedPosition;
    private bool hasLockedPosition;
    private bool isDragging;

    // 缓存初始位置
    private void Start(){
        startPosition = transform.position;
    }

    // 鼠标按下开始拖拽
    private void OnMouseDown(){
        isFinished = false;
        isDragging = true;
    }

    // 拖拽时跟随鼠标
    private void OnMouseDrag(){
        if(isDragging)
        {
            transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 
                                            Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
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
            return;
        }

        if(hasLockedPosition)
        {
            transform.position = lastLockedPosition;
        }
        else
        {
            transform.position = startPosition;
        }
    }
}
