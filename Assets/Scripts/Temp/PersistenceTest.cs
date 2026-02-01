using UnityEngine;

public class PersistenceTest : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("--- Persistence Test Start ---");
        
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager not found!");
            return;
        }

        // 1. Print current state
        var unlocked = DataManager.Instance.GetUnlockedSpecialEffects();
        Debug.Log($"Current Unlocked Count: {unlocked.Count}");
        foreach(var effect in unlocked)
        {
            Debug.Log($" - {effect}");
        }

        // 2. Simulate unlock
        Debug.Log("Simulating unlock: 小女孩的珍藏");
        DataManager.Instance.RecordSpecialEffectUnlock(SpecialEffectType.小女孩的珍藏);

        // 3. Print new state
        unlocked = DataManager.Instance.GetUnlockedSpecialEffects();
        Debug.Log($"New Unlocked Count: {unlocked.Count}");

        // 4. Manual Save (RecordSpecialEffectUnlock already saves, but good to be explicit for test)
        // DataManager.Instance.SaveGame(0); 

        Debug.Log("--- Persistence Test End. Stop and Play again to verify persistence. ---");
    }
}
