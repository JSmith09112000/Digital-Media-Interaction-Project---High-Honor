using System.Collections.Generic;
using UnityEngine;


// Used to keep track of where units are.
public class UnitManager : MonoBehaviour
{
    // Keeps track of occupied cells
    // For not uses UnitMovement but will alter change to use a dedicated unit script once we have one ready.
    private Dictionary<Vector3Int, UnitMovement> occupiedCells = new Dictionary<Vector3Int, UnitMovement>();

    public void RegisterUnit(Vector3Int cell, UnitMovement unit)
    {
        occupiedCells[cell] = unit;
    }

    public void UnregisterUnit(Vector3Int cell)
    {
        occupiedCells.Remove(cell);
    }

    public bool HasUnit(Vector3Int cell)
    {
        return occupiedCells.ContainsKey(cell);
    }

    public UnitMovement GetUnit(Vector3Int cell)
    {
        occupiedCells.TryGetValue(cell, out UnitMovement unit);

        return unit;
    }
}
