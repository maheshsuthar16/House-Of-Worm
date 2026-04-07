using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Text levelText;
    public GameObject lockIcon;
    public LevelManager levelManager;
    int levelIndex;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setup(int levelNumber, bool locked)
    {
        levelIndex = levelNumber;
        levelText.text = levelNumber.ToString();
        lockIcon.SetActive(locked);

        button.interactable = !locked;
    }

    public void OnClick()
    {
        levelManager.LoadLevel(levelIndex - 1);
    }
}