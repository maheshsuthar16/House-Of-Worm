using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject emptyPrefabs;
    public GameObject wallPrefabs;
    public GameObject entryPrefabs;
    public GameObject exitPrefabs;
    public GameObject breakableWall;
    public int width;
    public int height;
    public int[,] grid;

    public Vector2Int entryPosition;
    public Vector2Int exitPosition;

    public void BuildGrid(LevelData levelData)
    {

        width = levelData.width;
        height = levelData.height;
        grid = new int[width, height];
        int expected = width * height;

        if (levelData.gridData.Length != expected)
        {
            Debug.LogError(
                "GRID DATA SIZE WRONG!\n" +
                "Expected: " + expected +
                "\nActual: " + levelData.gridData.Length
            );
            return;
        }


        int index = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int value = levelData.gridData[index];
                grid[x, y] = value;

                if (value == 2)
                {
                    entryPosition = new Vector2Int(x, y);
                }
                if (value == 3)
                {
                    exitPosition = new Vector2Int(x, y);

                }
                   index++;

            }
        }
        SpawnVisualGrid();
        FindObjectOfType<CameraController>().gridwidth= width;
        FindObjectOfType<CameraController>().gridheight= height;
        FindObjectOfType<CameraController>().FitCamera();

    }
    public bool IsInside(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public int GetCell(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public void SpawnVisualGrid()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x, y, 0);
                GameObject prefabs = emptyPrefabs;

                switch (grid[x, y])
                {
                    case 1: prefabs = wallPrefabs; break;
                    case 2: prefabs = entryPrefabs; break;
                    case 3: prefabs = exitPrefabs; break;
                    case 4: prefabs = breakableWall; break;

                }

                Instantiate(prefabs, pos, Quaternion.identity, transform);

            }
        }
    }

    public void BrakingWall()
    {
       
    
    }


}
