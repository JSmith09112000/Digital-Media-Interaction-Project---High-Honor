using System.Collections.Generic;
using UnityEngine;


// Discovers all terrain tiles and creates nodes for them for the purpose of using an a star pathfinding algorithm.
public class Pathfinder : MonoBehaviour
{
    private Dictionary<Vector3Int, PathNode> nodes = new Dictionary<Vector3Int, PathNode>();

    [SerializeField]
    private Grid grid;

    private void Awake()
    {
        BuildGrid();
    }


    // Builds the Grid
    private void BuildGrid()
    {
        Terrain[] terrains = FindObjectsByType<Terrain>();

        foreach (Terrain terrain in terrains)
        {
            Vector3Int cellPos = grid.WorldToCell(terrain.transform.position);

            PathNode node = new PathNode(cellPos, terrain.terrainType);

            nodes[cellPos] = node;
        }
    }


    // Finds the path
    public List<PathNode> FindPath(Vector3Int startCell, Vector3Int targestCell, UnitMovement unitMovement)
    {
        if (!nodes.ContainsKey(startCell))
        {
            return null;
        }

        if (!nodes.ContainsKey(targestCell))
        {
            return null;
        }

        foreach (PathNode node in nodes.Values)
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }

        PathNode startNode = nodes[startCell];

        PathNode targetNode = nodes[targestCell];

        startNode.gCost = 0;

        List<PathNode> openSet = new();
        HashSet<PathNode> closedSet = new();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            PathNode current = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost)
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach(PathNode neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                if (unitMovement == null)
                {
                    Debug.LogError("unitMovement is NULL");
                }

                if (neighbor == null)
                {
                    Debug.LogError("neighbor is NULL");
                }

                int terrainCost = unitMovement.GetTerrainCost(neighbor.terrainType);

                int newCost = current.gCost + terrainCost;

                if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCost;

                    neighbor.hCost = GetDistance(neighbor, targetNode);

                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();

        Vector3Int[] directions =
        {
        new Vector3Int( 1, 0,  0), // Right
        new Vector3Int(-1, 0,  0), // Left
        new Vector3Int( 0, 1,  0), // Forward
        new Vector3Int( 0, -1, 0)  // Back
        };
        foreach (Vector3Int dir in directions)
        {
            Vector3Int position = node.cellPosition + dir;

            if (nodes.TryGetValue(position, out PathNode neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // Used to get the distance between two tiles
    private int GetDistance(PathNode a, PathNode b)
    {
        return Mathf.Abs(a.cellPosition.x - b.cellPosition.x) + Mathf.Abs(a.cellPosition.y - b.cellPosition.y);
    }

    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();

        PathNode current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();

        return path;
    }
}
