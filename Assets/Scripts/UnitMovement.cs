using UnityEngine;

public class UnitMovement : MonoBehaviour
{

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private UnitManager unitManager;

    // Updated to utilize the stats tied to the Unit
    private Stats stats;

    public int movementPoints
    {
        get { return stats.movement; }
    }

    public void Awake()
    {
        stats = GetComponent<Stats>();
        unitManager.RegisterUnit(grid.WorldToCell(transform.position), this);
    }


    // For now this will onyl have one value for each terrain type but later may be edited so that some units ignore the movement penalty for some or all terrain types.
    public int GetTerrainCost(TerrainType terrain)
    {
        switch (terrain)
        {

            case TerrainType.Grass:
                return 1;

            case TerrainType.Forest:
                return 2;

                // High cost so that Mountains can not be entered
            case TerrainType.Mountain:
                return 999;

            default:
                return 1;
        }
    }
}
