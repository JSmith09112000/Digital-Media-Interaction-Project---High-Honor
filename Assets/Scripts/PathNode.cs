using UnityEngine;

public class PathNode
{
    public Vector3Int cellPosition;

    public TerrainType terrainType;

    public int gCost;
    public int hCost;

    public int fCost => gCost + hCost;

    public PathNode parent;

    public PathNode(
        Vector3Int cellPosition,
        TerrainType terrainType)
    {
        this.cellPosition = cellPosition;
        this.terrainType = terrainType;
    }
}
