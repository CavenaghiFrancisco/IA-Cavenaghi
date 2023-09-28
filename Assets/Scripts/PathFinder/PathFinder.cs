using MinerSimulator.Admins;
using MinerSimulator.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MinerSimulator.Utils.Pathfinder
{
    public enum PawnType
    {
        VILLAGER = 0,
        CARRIAGE
    }

    public static class PathFinder
    {
        static Node[,] grid => AdminOfGame.GetMap().grid;
        static int gridSizeX => AdminOfGame.GetMap().SizeX;
        static int gridSizeY => AdminOfGame.GetMap().SizeY;
        static float gridSizeBetweenY => AdminOfGame.GetMap().SpaceBetweenY;
        static float gridSizeBetweenX => AdminOfGame.GetMap().SpaceBetweenX;

        private static void ConnectNeighbours()
        {

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Node node = grid[x, y];
                    if (x > 0) node.neighbours.Add(grid[x -1, y]);
                    if (x < gridSizeX - 1) node.neighbours.Add(grid[x + 1, y]);
                    if (y > 0) node.neighbours.Add(grid[x, y - 1]);
                    if (y < gridSizeY - 1) node.neighbours.Add(grid[x, y + 1]);
                }
            }
        }

        public static List<Vector3> FindPath(Vector3 start, Vector3 target, PawnType type)
        {
            Node startNode = null;
            Node targetNode = null;
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                { 
                    if(Mathf.Abs(grid[x,y].X - start.x) < gridSizeBetweenX/2f && Mathf.Abs(grid[x, y].Y - start.z) < gridSizeBetweenY / 2f)
                    {
                        startNode = grid[x, y];
                    }
                    if (Mathf.Abs(grid[x, y].X - target.x) < gridSizeBetweenX / 2f && Mathf.Abs(grid[x, y].Y - target.z) < gridSizeBetweenY / 2f)
                    {
                        targetNode = grid[x, y];
                    }
                }
            }

            ConnectNeighbours();

            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.OrderBy(node => node.totalCost).First();

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Node neighbour in currentNode.neighbours)
                {
                    if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                        continue;

                    float auxCost = currentNode.cost + CalculateMoveCost(currentNode, neighbour,type);
                    if (auxCost <= neighbour.cost)
                    {
                        neighbour.cost = auxCost;
                        neighbour.hCost = CalculateDistanceCost(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        private static float CalculateMoveCost(Node current, Node neighbour, PawnType type)
        {
            float baseCost = 1;
            float penalty = neighbour.penaltyCost;
            if(neighbour.tileType == TileType.SPIKY && type == PawnType.VILLAGER)
            {
                penalty *= 2;
            }
            if (neighbour.tileType == TileType.MUDDY && type == PawnType.CARRIAGE)
            {
                penalty *= 2;
            }

            return baseCost + penalty;
        }

        private static List<Vector3> RetracePath(Node start, Node end)
        {
            List<Node> path = new List<Node>();
            Node currentNode = end;

            while (currentNode != start)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            return GetPathPositions(path);
        }

        public static List<Vector3> GetPathPositions(List<Node> pathNodes)
        {
            List<Vector3> pathPositions = new List<Vector3>();
            foreach (Node node in pathNodes)
            {
                pathPositions.Add(new Vector3(node.X, 1, node.Y));
            }
            return pathPositions;
        }

        private static float CalculateDistanceCost(Node a, Node b)
        {
            float distanceX = Mathf.Abs(a.X - b.X);
            float distanceY = Mathf.Abs(a.Y - b.Y);
            return distanceX + distanceY;
        }

    }
}