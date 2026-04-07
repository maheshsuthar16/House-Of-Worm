using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelManager levelManager;
    public static GameManager Instance;
    public enum GameState { Playing, Win, Fail, Main };
    public LevelManager levelLoading;
    public GameState currentState;
    public GameObject winPanel;
    public GameObject gamePanel;
    public GameObject failPanel;
    public GameObject mainPanel;
    void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {  GoToMain();
        // currentState=GameState.Playing;
        // UpadateScore(0);

    }

    public void StartGAme()
    {
        UpdateState(GameState.Playing);
        levelManager.LoadLevel(0);
    }
    public void Win()
    {
        Debug.Log("reached hear");
        if (currentState != GameState.Playing) return;
        // currentState  = GameState.Win ;

        // Invoke(nameof(NextLevel),1f);
        UpdateState(GameState.Win);
    }
    public void Fail()
    {
        if (currentState != GameState.Playing) return;
        UpdateState(GameState.Fail);
    }
    public void NextLevel()
    {
        UpdateState(GameState.Playing);
        levelLoading.LoadNextLevel();
        // currentState = GameState.Playing;
    }

    public void Restart()
    {
        UpdateState(GameState.Playing);
        levelLoading.ReloadLevel();
    }
    public void GoToMain()
    {
        UpdateState(GameState.Main);
       
    }
    public void Exit()
    {
        Application.Quit();
    }



    public void UpdateState(GameState newState)
    {
        currentState = newState;
        if (winPanel)
        {
            winPanel.SetActive(newState == GameState.Win);
        }

        if (gamePanel)
        {
            gamePanel.SetActive(newState == GameState.Playing);

        }
        if (levelManager) levelManager.SetLevelVisible(newState == GameState.Playing);

        if (failPanel)
        {
            failPanel.SetActive(newState == GameState.Fail);
        }
        if (mainPanel)
        {
            mainPanel.SetActive(newState == GameState.Main);
        }
    }


}
