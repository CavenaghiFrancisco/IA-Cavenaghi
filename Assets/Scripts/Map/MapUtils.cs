using System.Collections.Generic;

namespace MinerSimulator.Utils.MapUtils
{
    public enum EntityType
    {
        TILE = 0,
        HOME,
        MINE
    }

    public enum TileType
    {
        WATER = 0,
        GRASSLAND,
        SPIKY,
        MUDDY
    }

    public class Node
    {
        public float X;
        public float Y;
        public bool isWalkable;
        public List<Node> neighbours = new List<Node>();
        public float cost = int.MaxValue;
        public float hCost = 0;
        public float penaltyCost = 0;
        public Node parent = null;
        public bool transited;
        public EntityType type = 0;
        public TileType tileType = 0;

        public float totalCost => cost + hCost;
    }
}
