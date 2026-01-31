using UnityEngine;

public class JSONGenerationTest : MonoBehaviour
{
    private void Start()
    {
        if (DataManager.Instance != null)
        {
            Debug.Log("[Test] Adding dummy data and triggering manual save...");
            DataManager.Instance.SetStoryFlag("TestFlag_1", true);
            DataManager.Instance.SetStoryFlag("TestFlag_2", false);
            DataManager.Instance.RecordSpecialEffectUnlock(SpecialEffectType.井中之天);
            
            DataManager.Instance.SaveGame(0);
        }
        else
        {
            Debug.LogError("[Test] DataManager instance not found!");
        }
    }
}
