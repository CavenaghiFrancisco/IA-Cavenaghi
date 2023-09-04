using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private static int sizeX = 21;
    [SerializeField] private static int sizeY = 21;

    private void Start()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                Instantiate(tilePrefab, new Vector3(i, 0, j), Quaternion.identity);
            }
        }
    }

    public static int GetSizeX()
    {
        return sizeX;
    }

    public static int GetSizeY()
    {
        return sizeY;
    }
}
