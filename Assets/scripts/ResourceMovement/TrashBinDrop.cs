using UnityEngine;
using UnityEngine.EventSystems;

// 垃圾桶：拖拽物品放入后销毁，显示 BinOn
public class TrashBinDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject binOn;
    [SerializeField] private GameObject binOff;

    private void Awake(){
        SetBinState(false);
    }

    public void OnPointerEnter(PointerEventData eventData){
        SetBinState(true);
    }

    public void OnPointerExit(PointerEventData eventData){
        SetBinState(false);
    }

    public void OnDrop(PointerEventData eventData){
        SetBinState(true);

        if(eventData.pointerDrag == null)
        {
            SetBinState(false);
            return;
        }

        DragByInterface dragItem = eventData.pointerDrag.GetComponent<DragByInterface>();
        ItemInfo itemInfo = eventData.pointerDrag.GetComponent<ItemInfo>();
        if(itemInfo == null)
        {
            itemInfo = eventData.pointerDrag.GetComponentInParent<ItemInfo>();
        }
        if(itemInfo == null)
        {
            itemInfo = eventData.pointerDrag.GetComponentInChildren<ItemInfo>();
        }

        if(dragItem != null && itemInfo != null)
        {
            Destroy(dragItem.gameObject);
        }

        SetBinState(false);
    }

    private void SetBinState(bool isOn){
        if(binOn != null)
        {
            binOn.SetActive(isOn);
        }
        if(binOff != null)
        {
            binOff.SetActive(!isOn);
        }
    }
}
