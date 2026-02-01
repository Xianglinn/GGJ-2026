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
        if(dragItem == null)
        {
            SetBinState(false);
            return;
        }

        if(dragItem.ItemInfo == null)
        {
            SetBinState(false);
            return;
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFXByName("Rubbish");
        }
        Destroy(dragItem.gameObject);

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
