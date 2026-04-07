using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public LevelData[] level;
    private int currentLevelIndex;
    public GridManager gridManager;
    public WormController worm;
    [SerializeField] private TextMeshProUGUI levelText; 
  

    public void Start()
    {


        LoadLevel(0);

    }

    public void LoadLevel(int index)
    {
        currentLevelIndex = index;
        UpdateLevelUI();
        gridManager.BuildGrid(level[index]);
        worm.Spawn(gridManager.entryPosition);

    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= level.Length)
            currentLevelIndex = 0;
        LoadLevel(currentLevelIndex);
    }
    public void ReloadLevel()
    {
        LoadLevel(currentLevelIndex);
    }
    private void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"Level {currentLevelIndex + 1}";
    }
    
    public void SetLevelVisible(bool visible)
    {
        if (gridManager) gridManager.gameObject.SetActive(visible);
        if (worm) worm.gameObject.SetActive(visible); ;
    }

    public void DevReloadCurrentLevel()
    {
        Debug.Log("DEV: Reloading current level data");
        LoadLevel(currentLevelIndex);
    }



}
