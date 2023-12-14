using MinerSimulator.Admins;
using MinerSimulator.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MinerSimulator.Map
{
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
        [SerializeField] private int waterQuantity = 10;
        [SerializeField] private int spikeQuantity = 2;
        [SerializeField] private int muddyQuantity = 2;
        [SerializeField] private int homesQuantity = 1;
        [SerializeField] private Button emergencyButton;
        [SerializeField] private GameObject planeEmergency;

        private const int GROUND_LEVEL = 1;

        public Node[,] grid;
        public int SizeX { get => sizeX; }
        public int SizeY { get => sizeY; }
        public int SpaceBetweenX { get => spaceBetweenX; }
        public int SpaceBetweenY { get => spaceBetweenY; }
        public List<Mine> MinesAvailable { get => minesAvailable; }

        List<Mine> minesAvailable = new List<Mine>();

        List<Vector3> unusedTiles = new List<Vector3>();

        static MapGenerator instance = null;

        public static MapGenerator Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<MapGenerator>();

                return instance;
            }
        }

        private void Start()
        {
            emergencyButton.onClick.AddListener(() =>
            {
                VillagerAdmin.Instance.SetEmergency(VillagerAdmin.Instance.Emergency);
                planeEmergency.GetComponent<MeshRenderer>().material.color = (VillagerAdmin.Instance.Emergency ? Color.red : Color.white);
            });
        }

        private void OnDestroy()
        {
            emergencyButton.onClick.RemoveAllListeners();
        }

        public void CreateMap(int sizeX, int sizeY, int spaceBetween, int minesQuantity)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.spaceBetweenX = spaceBetween;
            this.spaceBetweenY = spaceBetween;
            this.minesQuantity = minesQuantity;
            CreateMapData();
            CreateGameplayMapRepresentation();
        }

        private void CreateMapData()
        {
            grid = new Node[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Node node = new Node { X = x * spaceBetweenX / 2f, Y = y * spaceBetweenY / 2f, isWalkable = true };
                    grid[x, y] = node;
                    if (!(x == sizeX || y == sizeY || x == 0 || y == 0))
                        unusedTiles.Add(new Vector3(node.X, 1, node.Y));
                }
            }
            CreateEntities();
        }

        private void CreateGameplayMapRepresentation()
        {
            GameObject mapGO = new GameObject("Map");
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    GameObject aux = Instantiate(tilePrefab, new Vector3(i * spaceBetweenX / 2f, 0, j * spaceBetweenY / 2f), Quaternion.identity);
                    aux.transform.localScale = new Vector3(spaceBetweenX / 2f, GROUND_LEVEL, spaceBetweenY / 2f);
                    aux.transform.SetParent(mapGO.transform);
                    if (!grid[i, j].isWalkable)
                    {
                        aux.transform.GetComponent<MeshRenderer>().material.color = Color.blue;
                    }
                    else if(grid[i,j].tileType == TileType.SPIKY)
                    {
                        aux.transform.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                    else if (grid[i, j].tileType == TileType.MUDDY)
                    {
                        aux.transform.GetComponent<MeshRenderer>().material.color = Color.black;
                    }
                    else
                    {
                        aux.transform.GetComponent<MeshRenderer>().material.color = Color.green;
                    }
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
                        GameObject mine = Instantiate(minePrefab, new Vector3(grid[i, j].X, GROUND_LEVEL, grid[i, j].Y), Quaternion.identity);
                        minesAvailable.Add(mine.AddComponent<Mine>());
                    }
                    else if (grid[i, j].type == EntityType.HOME)
                    {
                        GameObject home = Instantiate(homePrefab, new Vector3(grid[i, j].X, GROUND_LEVEL, grid[i, j].Y), Quaternion.identity);
                        
                    }
                }
            }
        }

        public Mine GetMostWorkedMine()
        {
            return minesAvailable.OrderBy(node => node.Mined).Last();
        }

        private void CreateEntities()
        {
            CreateEntityAtRandomPosition(EntityType.HOME, homesQuantity, SetHomeEntity);
            CreateEntityAtRandomPosition(EntityType.MINE, minesQuantity, SetMineEntity);
            CreateTileAtRandomPosition(TileType.WATER, waterQuantity, SetWaterTile);
            CreateTileAtRandomPosition(TileType.SPIKY, spikeQuantity, SetSpikyTile);
            CreateTileAtRandomPosition(TileType.MUDDY, muddyQuantity, SetMuddyTile);
        }

        private void CreateTileAtRandomPosition(TileType entityType, int quantity, Action<Vector3> setEntityAction)
        {
            for (int k = 0; k < quantity; k++)
            {
                Vector3 entityPosition = GetRandomPosition(unusedTiles);
                setEntityAction(entityPosition);
            }
        }

        private void CreateEntityAtRandomPosition(EntityType entityType, int quantity, Action<Vector3> setEntityAction)
        {
            for (int k = 0; k < quantity; k++)
            {
                Vector3 entityPosition = GetRandomPosition(unusedTiles);
                setEntityAction(entityPosition);
            }
        }

        private void SetHomeEntity(Vector3 position)
        {
            SetEntityAtPosition(position, EntityType.HOME);
            unusedTiles.Remove(position);
        }

        private void SetWaterTile(Vector3 position)
        {
            SetTileAtPosition(position, TileType.WATER,false,0);
            unusedTiles.Remove(position);
        }

        private void SetMuddyTile(Vector3 position)
        {
            SetTileAtPosition(position, TileType.MUDDY, true, 80);
            unusedTiles.Remove(position);
        }

        private void SetSpikyTile(Vector3 position)
        {
            SetTileAtPosition(position, TileType.SPIKY,true,80);
            unusedTiles.Remove(position);
        }

        private void SetMineEntity(Vector3 position)
        {
            SetEntityAtPosition(position, EntityType.MINE);
            unusedTiles.Remove(position);
        }

        private void SetEntityAtPosition(Vector3 position, EntityType entityType)
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (grid[i, j].X == position.x && grid[i, j].Y == position.z)
                    {
                        grid[i, j].type = entityType;
                    }
                }
            }
        }

        private void SetTileAtPosition(Vector3 position, TileType tileType, bool isWalkable = true, float penaltyCost = 0)
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (grid[i, j].X == position.x && grid[i, j].Y == position.z)
                    {
                        grid[i, j].tileType = tileType;
                        grid[i, j].isWalkable = isWalkable;
                        grid[i, j].penaltyCost = penaltyCost;
                    }
                }
            }
        }

        Vector3 GetRandomPosition(List<Vector3> positions)
        {
            int randomIndex = UnityEngine.Random.Range(0, positions.Count);
            Vector3 position = positions[randomIndex];
            return position;
        }
    }
}