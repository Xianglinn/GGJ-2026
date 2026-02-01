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
    
    [Header("Specific Elements")]
    [SerializeField] private GameObject lockWitch; // 对应魔女成就的遮挡图
    [SerializeField] private GameObject lockWell; // 对应井成就的遮挡图
    [SerializeField] private GameObject lockGirl; // 对应女成就的遮挡图

    [SerializeField] private Text countingText;    // 进度计数文本

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

    private void OnEnable()
    {
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
        int totalAchievements = 3; // 总共有3个成就
        int unlockedCount = unlocked.Count;
        
        Debug.Log($"[UIAchivementPanel] Refreshing UI. Unlocked count: {unlockedCount}");

        // 刷新列表项 (保留原有通用逻辑)
        foreach (var item in achievementItems)
        {
            if (item == null) continue;
            
            bool isUnlocked = unlocked.Contains(item.targetEffect);
            
            if (item.unlockedState != null) item.unlockedState.SetActive(isUnlocked);
            if (item.lockedState != null) item.lockedState.SetActive(!isUnlocked);
        }

        // 处理魔女面具的特殊遮挡逻辑
        bool isWitchUnlocked = unlocked.Contains(SpecialEffectType.魔女的面具);
        if (lockWitch != null)
        {
            // 如果未解锁，显示 lockWitch (遮挡图)
            // 如果已解锁，隐藏 lockWitch (露出下方的 unlockWitch)
            lockWitch.SetActive(!isWitchUnlocked);
        }

        // 处理井的特殊遮挡逻辑
        bool isWellUnlocked = unlocked.Contains(SpecialEffectType.井中之天);
        if (lockWell != null)
        {
            lockWell.SetActive(!isWellUnlocked);
        }

        // 处理女的特殊遮挡逻辑
        bool isGirlUnlocked = unlocked.Contains(SpecialEffectType.小女孩的珍藏);
        if (lockGirl != null)
        {
            lockGirl.SetActive(!isGirlUnlocked);
        }

        // 2. 更新进度文本
        if (countingText != null)
        {
            // 防止解锁数量超过总数 (理论上不应该，但为了显示安全)
            int displayCount = Mathf.Min(unlockedCount, totalAchievements); 
            countingText.text = $"{displayCount}/{totalAchievements}";
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
