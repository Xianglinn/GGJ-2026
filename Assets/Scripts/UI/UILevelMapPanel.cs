using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡地图 UI 面板
/// </summary>
public class UILevelMapPanel : UIBasePanel<object>
{
    [Header("UI References")]
    [SerializeField] private Button toScene1Btn;
    [SerializeField] private Button showImageBtn;
    [SerializeField] private GameObject imageOverlay;

    private void Awake()
    {
        // 自动注册
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UILevelMapPanel>())
        {
            UIManager.Instance.RegisterPanel(this);
        }

        if (toScene1Btn == null)
        {
            // 尝试在子物体中查找，包括隐藏的
            var btnTrans = transform.Find("ToScene1Btn");
            if (btnTrans != null) 
            {
                toScene1Btn = btnTrans.GetComponent<Button>();
            }
            else
            {
                // 全局查找
                var btnObj = GameObject.Find("ToScene1Btn");
                if (btnObj != null) toScene1Btn = btnObj.GetComponent<Button>();
            }
            
            if (toScene1Btn != null)
            {
                toScene1Btn.onClick.AddListener(OnToScene1BtnClicked);
                Debug.Log("[UILevelMapPanel] Auto-found ToScene1Btn");
            }
        }
        else
        {
            toScene1Btn.onClick.AddListener(OnToScene1BtnClicked);
        }

        if (showImageBtn == null)
        {
            // 尝试在子物体中查找
            var btnTrans = transform.Find("ShowImageBtn");
            if (btnTrans != null)
            {
                showImageBtn = btnTrans.GetComponent<Button>();
            }
            else
            {
                // 全局查找
                var btnObj = GameObject.Find("ShowImageBtn");
                if (btnObj != null) showImageBtn = btnObj.GetComponent<Button>();
            }

            if (showImageBtn != null)
            {
                showImageBtn.onClick.AddListener(OnShowImageBtnClicked);
                Debug.Log("[UILevelMapPanel] Auto-found ShowImageBtn");
            }
        }
        else
        {
            showImageBtn.onClick.AddListener(OnShowImageBtnClicked);
        }

        if (imageOverlay == null)
        {
            // 尝试在子物体中查找
            var overlayTrans = transform.Find("ImageOverlay");
            if (overlayTrans != null)
            {
                imageOverlay = overlayTrans.gameObject;
            }
            else
            {
                // 全局查找
                imageOverlay = GameObject.Find("ImageOverlay");
            }

            if (imageOverlay != null)
            {
                Debug.Log("[UILevelMapPanel] Auto-found ImageOverlay");
            }
        }

        // 默认隐藏
        if (imageOverlay != null)
        {
            imageOverlay.SetActive(false);
        }
    }

    private void Update()
    {
        // 如果图片正在显示，点击任何键或鼠标则关闭
        if (imageOverlay != null && imageOverlay.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                imageOverlay.SetActive(false);
                Debug.Log("[UILevelMapPanel] Image Overlay closed by user input.");
            }
        }
    }

    private void OnShowImageBtnClicked()
    {
        if (imageOverlay != null)
        {
            imageOverlay.SetActive(true);
            Debug.Log("[UILevelMapPanel] Image Overlay shown.");
        }
    }

    private void OnToScene1BtnClicked()
    {
        Debug.Log("[UILevelMapPanel] To Scene1 Clicked");
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SwitchState(GameState.Home);
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (toScene1Btn != null) toScene1Btn.onClick.RemoveListener(OnToScene1BtnClicked);
        if (showImageBtn != null) showImageBtn.onClick.RemoveListener(OnShowImageBtnClicked);
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterPanel<UILevelMapPanel>();
        }
    }
}
