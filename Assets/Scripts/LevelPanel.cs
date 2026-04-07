using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform gridLayoutGroup;

    public int totalLevel = 30;
    public int currentPage = 0;
    public int levelsPerPage = 15;

    public Text titleText;

    void Start()
    {
        GenerateTheGrid();
        UpdateTitlePage();
    }

    public void GenerateTheGrid()
    {
        foreach (Transform child in gridLayoutGroup)
            Destroy(child.gameObject);

        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        int startLevel = currentPage * levelsPerPage + 1;
        int endLevel = Mathf.Min(startLevel + levelsPerPage - 1, totalLevel);

        for (int i = startLevel; i <= endLevel; i++)
        {
            GameObject btnObj = Instantiate(levelButtonPrefab, gridLayoutGroup);

            LevelButton levelButton = btnObj.GetComponent<LevelButton>();

            bool locked = i > reachedLevel;

            levelButton.Setup(i, locked);
        }
    }

    public void NextPage()
    {
        int maxPage = Mathf.CeilToInt((float)totalLevel / levelsPerPage) - 1;

        if (currentPage < maxPage)
        {
            currentPage++;
            GenerateTheGrid();
            UpdateTitlePage();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            GenerateTheGrid();
            UpdateTitlePage();
        }
    }

    public void UpdateTitlePage()
    {
        if (titleText == null) return;

        int start = currentPage * levelsPerPage + 1;
        int end = Mathf.Min((currentPage + 1) * levelsPerPage, totalLevel);

        titleText.text = "Levels " + start + " - " + end;
    }
}