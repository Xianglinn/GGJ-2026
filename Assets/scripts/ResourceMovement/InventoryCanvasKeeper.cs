using UnityEngine;

// 让 InventoryCanvas 只创建一次并跨场景保留
public class InventoryCanvasKeeper : MonoBehaviour
{
    private static InventoryCanvasKeeper instance;

    private void Awake(){
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
