using MinerSimulator.Utils.MapUtils;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject homePrefab;
    [SerializeField] private int sizeX = 21;
    [SerializeField] private int sizeY = 21;
    [SerializeField] private int spaceBetweenX = 1;
    [SerializeField] private int spaceBetweenY = 1;
    [SerializeField] private int minesQuantity = 1;

    public Node[,] grid;
    public int SizeX { get => sizeX; }
    public int SizeY { get => sizeY; }
    public int SpaceBetweenX { get => spaceBetweenX; }
    public int SpaceBetweenY { get => spaceBetweenY; }

    List<Mine> minesAvailable = new List<Mine>();
    VillagerAdmin homeAvailable;

    List<Vector3> unusedTiles = new List<Vector3>();

    private void Start()
    {
        CreateStructureMap();
        CreateGameplayRepresentation();
    }

    private void CreateStructureMap()
    {
        grid = new Node[sizeX, sizeY];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Node node = new Node { X = x * spaceBetweenX / 2f, Y = y * spaceBetweenY / 2f, isWalkable = true };
                grid[x, y] = node;
                unusedTiles.Add(new Vector3(node.X, 1, node.Y));
            }
        }
        CreateEntities();
    }

    private void CreateGameplayRepresentation()
    {
        GameObject mapGO = new GameObject("Map");
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                GameObject aux = Instantiate(tilePrefab, new Vector3(i * spaceBetweenX / 2f, 0, j * spaceBetweenY / 2f), Quaternion.identity);
                aux.transform.localScale = new Vector3(spaceBetweenX / 2f, 1f, spaceBetweenY / 2f);
                aux.transform.SetParent(mapGO.transform);
            }
        }
        CreateGameplayEntities();
    }

    private void CreateGameplayEntities()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (grid[i, j].type == EntityType.MINE)
                {
                    GameObject mine = Instantiate(minePrefab, new Vector3(grid[i, j].X, 1, grid[i, j].Y), Quaternion.identity);
                    minesAvailable.Add(mine.AddComponent<Mine>());
                }
                else if (grid[i, j].type == EntityType.HOME)
                {
                    GameObject home = Instantiate(homePrefab, new Vector3(grid[i, j].X, 1, grid[i, j].Y), Quaternion.identity);
                    homeAvailable = home.AddComponent<VillagerAdmin>();
                }
            }
        }
    }

    private void CreateEntities()
    {
        Vector3 homePosition = GetRandomPosition(unusedTiles);
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                if (grid[i, j].X == homePosition.x && grid[i, j].Y == homePosition.z)
                {
                    grid[i, j].type = EntityType.HOME;
                    unusedTiles.Remove(homePosition);
                }
            }
        }

        for (int k = 0; k < minesQuantity; k++)
        {
            Vector3 minePosition = GetRandomPosition(unusedTiles);
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (grid[i, j].X == minePosition.x && grid[i, j].Y == minePosition.z)
                    {
                        grid[i, j].type = EntityType.MINE;
                        unusedTiles.Remove(minePosition);
                    }
                }
            }
        }
    }

    Vector3 GetRandomPosition(List<Vector3> positions)
    {
        int randomIndex = Random.Range(0, positions.Count);
        Vector3 position = positions[randomIndex];
        return position;
    }
}