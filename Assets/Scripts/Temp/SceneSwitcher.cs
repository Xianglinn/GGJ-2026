using UnityEngine;
public class SceneSwitcher : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.F2)) {
            GameFlowManager.Instance.SwitchState(GameState.Prologue);
        }
    }
}