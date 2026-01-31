using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 成就显示面板：根据 DataManager 中的解锁数据刷新显示
/// </summary>
public class UIAchivementPanel : UIBasePanel<object>
{
    [System.Serializable]
    public class AchievementUIItem
    {
        public SpecialEffectType targetEffect;
        public GameObject unlockedState;
        public GameObject lockedState;
    }

    [Header("UI References")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private List<AchievementUIItem> achievementItems;

    private void Awake()
    {
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
        }
        else
        {
            // Auto search if null
            var btnTransform = transform.Find("CloseBtn");
            if(btnTransform != null)
            {
                closeBtn = btnTransform.GetComponent<Button>();
                if(closeBtn != null) closeBtn.onClick.AddListener(OnCloseBtnClicked);
            }
        }

        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIAchivementPanel>())
        {
            UIManager.Instance.RegisterPanel(this);
            // 初始隐藏
            gameObject.SetActive(false);
        }
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);
        RefreshUI();
    }
    
    /// <summary>
    /// 刷新UI显示状态
    /// </summary>
    public void RefreshUI()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("[UIAchivementPanel] DataManager instance null.");
            return;
        }
        
        List<SpecialEffectType> unlocked = DataManager.Instance.GetUnlockedSpecialEffects();
        
        Debug.Log($"[UIAchivementPanel] Refreshing UI. Unlocked count: {unlocked.Count}");

        foreach (var item in achievementItems)
        {
            if (item == null) continue;
            
            bool isUnlocked = unlocked.Contains(item.targetEffect);
            
            if (item.unlockedState != null) item.unlockedState.SetActive(isUnlocked);
            if (item.lockedState != null) item.lockedState.SetActive(!isUnlocked);
        }
    }

    private void OnCloseBtnClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HidePanel<UIAchivementPanel>();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
