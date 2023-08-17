using UnityEngine;

public class Level : MonoBehaviour
{
    public LevelMode levelMode;
    public enum LevelMode
    {
        Both,
        Runner,
        Defence
    }
    private void OnEnable()
    {
        if (levelMode == LevelMode.Defence)
        {
            GameManager.gameModeOption = GameManager.GameMode.Defence;
            UIManager.Instance.defenceButtonOutline.SetActive(true);
            UIManager.Instance.runnerModeButton.gameObject.SetActive(false);
            GameManager.modeData = 0;
        }
        if (levelMode == LevelMode.Runner)
        {
            GameManager.gameModeOption = GameManager.GameMode.Runner;
            UIManager.Instance.runnerButtonOutline.SetActive(true);
            UIManager.Instance.defenceModeButton.gameObject.SetActive(false);
            GameManager.modeData = 1;
        }
    }
}
