using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PathFinder
{
    public class Node
    {
        public int X;
        public int Y;
        public bool isWalkable;
        public List<Node> neighbours = new List<Node>();
        public int cost = int.MaxValue;
        public int hCost = 0;
        public int penaltyCost = 0;
        public Node parent = null;
        public bool transited;

        public int totalCost => cost + hCost;
    }

    public static Node[,] grid;
    public static int gridSizeX;
    public static int gridSizeY;

    private static void ConnectNeighbours()
    {
       
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = grid[x, y];
                if (x > 0) node.neighbours.Add(grid[x - 1, y]);
                if (x < gridSizeX - 1) node.neighbours.Add(grid[x + 1, y]);
                if (y > 0) node.neighbours.Add(grid[x, y - 1]);
                if (y < gridSizeY - 1) node.neighbours.Add(grid[x, y + 1]);
            }
        }
    }

    public static List<Vector3> FindPath(Vector3 start, Vector3 target)
    {
        gridSizeX = MapGenerator.GetSizeX();
        gridSizeY = MapGenerator.GetSizeY();

        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePosition = new Vector3(x, 0, y);
                Node node = new Node { X = x, Y = y, isWalkable = true};
                grid[x, y] = node;
            }
        }

        Node startNode = grid[(int)start.x, (int)start.z];
        Node targetNode = grid[(int)target.x, (int)target.z];

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

                int auxCost = currentNode.cost + CalculateMoveCost(currentNode, neighbour); ;
                if (auxCost < neighbour.cost)
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

    private static int CalculateMoveCost(Node current, Node neighbour)
    {
        int baseCost = 1;
        int penalty = neighbour.penaltyCost;

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

    private static int CalculateDistanceCost(Node a, Node b)
    {
        int distanceX = Mathf.Abs(a.X - b.X);
        int distanceY = Mathf.Abs(a.Y - b.Y);
        return distanceX + distanceY;
    }

}

