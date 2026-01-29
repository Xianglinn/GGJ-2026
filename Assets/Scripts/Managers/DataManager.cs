using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 游戏存档数据结构
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public int currentLevel = 0;
    public List<string> unlockedLevels = new List<string>();
    public List<string> collectedItems = new List<string>();
    public Dictionary<string, bool> storyFlags = new Dictionary<string, bool>();
    public float musicVolume = 0.7f;
    public float sfxVolume = 1f;
    public string lastSaveTime;

    public GameSaveData()
    {
        lastSaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}

/// <summary>
/// 全局数据管理器，负责游戏数据的保存和加载
/// 特性：单例模式、JSON 序列化、多存档槽、自动保存
/// </summary>
public class DataManager : MonoSingleton<DataManager>
{
    /// <summary>
    /// 当前游戏数据
    /// </summary>
    private GameSaveData _currentData;

    /// <summary>
    /// 是否启用自动保存
    /// </summary>
    [SerializeField]
    private bool _enableAutoSave = true;

    /// <summary>
    /// 自动保存间隔（秒）
    /// </summary>
    [SerializeField]
    private float _autoSaveInterval = 300f; // 5 分钟

    /// <summary>
    /// 自动保存计时器
    /// </summary>
    private float _autoSaveTimer = 0f;

    /// <summary>
    /// 默认存档槽
    /// </summary>
    private int _currentSaveSlot = 0;

    /// <summary>
    /// 存档文件前缀
    /// </summary>
    private const string SAVE_FILE_PREFIX = "GameSave_";

    /// <summary>
    /// 存档文件扩展名
    /// </summary>
    private const string SAVE_FILE_EXTENSION = ".json";

    /// <summary>
    /// 获取当前游戏数据
    /// </summary>
    public GameSaveData CurrentData => _currentData;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // 初始化默认数据
        _currentData = new GameSaveData();

        Debug.Log("[DataManager] Initialized successfully.");
    }

    private void Update()
    {
        // 自动保存逻辑
        if (_enableAutoSave)
        {
            _autoSaveTimer += Time.deltaTime;
            if (_autoSaveTimer >= _autoSaveInterval)
            {
                AutoSave();
                _autoSaveTimer = 0f;
            }
        }
    }

    #region 存档管理

    /// <summary>
    /// 保存游戏到指定槽位
    /// </summary>
    /// <param name="slot">存档槽位（0-9）</param>
    /// <returns>是否保存成功</returns>
    public bool SaveGame(int slot)
    {
        if (slot < 0 || slot > 9)
        {
            Debug.LogError($"[DataManager] Invalid save slot: {slot}. Must be between 0 and 9.");
            return false;
        }

        try
        {
            _currentSaveSlot = slot;
            _currentData.lastSaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string json = JsonUtility.ToJson(_currentData, true);
            string filePath = GetSaveFilePath(slot);

            File.WriteAllText(filePath, json);
            Debug.Log($"[DataManager] Game saved to slot {slot}: {filePath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataManager] Failed to save game: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 从指定槽位加载游戏
    /// </summary>
    /// <param name="slot">存档槽位（0-9）</param>
    /// <returns>是否加载成功</returns>
    public bool LoadGame(int slot)
    {
        if (slot < 0 || slot > 9)
        {
            Debug.LogError($"[DataManager] Invalid save slot: {slot}. Must be between 0 and 9.");
            return false;
        }

        string filePath = GetSaveFilePath(slot);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[DataManager] Save file not found at slot {slot}: {filePath}");
            return false;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            _currentData = JsonUtility.FromJson<GameSaveData>(json);
            _currentSaveSlot = slot;

            Debug.Log($"[DataManager] Game loaded from slot {slot}: {filePath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataManager] Failed to load game: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 删除指定槽位的存档
    /// </summary>
    /// <param name="slot">存档槽位（0-9）</param>
    /// <returns>是否删除成功</returns>
    public bool DeleteSave(int slot)
    {
        if (slot < 0 || slot > 9)
        {
            Debug.LogError($"[DataManager] Invalid save slot: {slot}. Must be between 0 and 9.");
            return false;
        }

        string filePath = GetSaveFilePath(slot);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[DataManager] Save file not found at slot {slot}: {filePath}");
            return false;
        }

        try
        {
            File.Delete(filePath);
            Debug.Log($"[DataManager] Save file deleted at slot {slot}: {filePath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[DataManager] Failed to delete save file: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 检查指定槽位是否存在存档
    /// </summary>
    /// <param name="slot">存档槽位（0-9）</param>
    /// <returns>是否存在存档</returns>
    public bool HasSave(int slot)
    {
        if (slot < 0 || slot > 9)
        {
            return false;
        }

        string filePath = GetSaveFilePath(slot);
        return File.Exists(filePath);
    }

    /// <summary>
    /// 获取存档文件的完整路径
    /// </summary>
    /// <param name="slot">存档槽位</param>
    /// <returns>存档文件路径</returns>
    private string GetSaveFilePath(int slot)
    {
        string fileName = SAVE_FILE_PREFIX + slot + SAVE_FILE_EXTENSION;
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    /// <summary>
    /// 自动保存
    /// </summary>
    private void AutoSave()
    {
        SaveGame(_currentSaveSlot);
        Debug.Log("[DataManager] Auto-save completed.");
    }

    #endregion

    #region 数据操作

    /// <summary>
    /// 创建新游戏（重置数据）
    /// </summary>
    public void NewGame()
    {
        _currentData = new GameSaveData();
        Debug.Log("[DataManager] New game started.");
    }

    /// <summary>
    /// 设置当前关卡
    /// </summary>
    /// <param name="level">关卡编号</param>
    public void SetCurrentLevel(int level)
    {
        _currentData.currentLevel = level;
        Debug.Log($"[DataManager] Current level set to: {level}");
    }

    /// <summary>
    /// 解锁关卡
    /// </summary>
    /// <param name="levelName">关卡名称</param>
    public void UnlockLevel(string levelName)
    {
        if (!_currentData.unlockedLevels.Contains(levelName))
        {
            _currentData.unlockedLevels.Add(levelName);
            Debug.Log($"[DataManager] Level unlocked: {levelName}");
        }
    }

    /// <summary>
    /// 检查关卡是否已解锁
    /// </summary>
    /// <param name="levelName">关卡名称</param>
    /// <returns>是否已解锁</returns>
    public bool IsLevelUnlocked(string levelName)
    {
        return _currentData.unlockedLevels.Contains(levelName);
    }

    /// <summary>
    /// 添加收集物品
    /// </summary>
    /// <param name="itemName">物品名称</param>
    public void AddCollectedItem(string itemName)
    {
        if (!_currentData.collectedItems.Contains(itemName))
        {
            _currentData.collectedItems.Add(itemName);
            Debug.Log($"[DataManager] Item collected: {itemName}");
        }
    }

    /// <summary>
    /// 检查物品是否已收集
    /// </summary>
    /// <param name="itemName">物品名称</param>
    /// <returns>是否已收集</returns>
    public bool HasCollectedItem(string itemName)
    {
        return _currentData.collectedItems.Contains(itemName);
    }

    /// <summary>
    /// 设置剧情标记
    /// </summary>
    /// <param name="flagName">标记名称</param>
    /// <param name="value">标记值</param>
    public void SetStoryFlag(string flagName, bool value)
    {
        _currentData.storyFlags[flagName] = value;
        Debug.Log($"[DataManager] Story flag set: {flagName} = {value}");
    }

    /// <summary>
    /// 获取剧情标记
    /// </summary>
    /// <param name="flagName">标记名称</param>
    /// <returns>标记值（不存在则返回 false）</returns>
    public bool GetStoryFlag(string flagName)
    {
        if (_currentData.storyFlags.ContainsKey(flagName))
        {
            return _currentData.storyFlags[flagName];
        }

        return false;
    }

    /// <summary>
    /// 设置音频设置
    /// </summary>
    /// <param name="musicVolume">音乐音量</param>
    /// <param name="sfxVolume">音效音量</param>
    public void SetAudioSettings(float musicVolume, float sfxVolume)
    {
        _currentData.musicVolume = Mathf.Clamp01(musicVolume);
        _currentData.sfxVolume = Mathf.Clamp01(sfxVolume);
        Debug.Log($"[DataManager] Audio settings saved: Music={musicVolume}, SFX={sfxVolume}");
    }

    #endregion

    #region 设置

    /// <summary>
    /// 启用或禁用自动保存
    /// </summary>
    /// <param name="enable">是否启用</param>
    public void SetAutoSave(bool enable)
    {
        _enableAutoSave = enable;
        Debug.Log($"[DataManager] Auto-save {(enable ? "enabled" : "disabled")}.");
    }

    /// <summary>
    /// 设置自动保存间隔
    /// </summary>
    /// <param name="intervalSeconds">间隔（秒）</param>
    public void SetAutoSaveInterval(float intervalSeconds)
    {
        _autoSaveInterval = intervalSeconds;
        Debug.Log($"[DataManager] Auto-save interval set to: {intervalSeconds} seconds.");
    }

    #endregion
}
