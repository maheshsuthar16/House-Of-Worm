using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Worm/Level")]
public class LevelData : ScriptableObject
{
    public int width;
    public int height;


    public int[] gridData;
}
