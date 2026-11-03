using UnityEngine;


// Used to store the type of terrain for the purposes of calculating tile effects.
public enum TerrainType
{
    Grass,
    Forest,
    Mountain,
}

public class Terrain : MonoBehaviour
{
    public TerrainType terrainType;
}
