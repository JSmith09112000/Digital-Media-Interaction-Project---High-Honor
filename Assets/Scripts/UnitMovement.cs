using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    // Update this later so that movement points is taken from the stat block rather than being hard coded into the unit in this specific class.
    public int movementPoints = 6;


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
