using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class EnemyAI : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private Grid grid;

    // An offset applied to the y Axis so models don't clip into the terrain or float above it
    [SerializeField]
    private float yOffset = 2.5f;

    [SerializeField]
    private Pathfinder pathfinder;

    private Camera mainCamera;
    private Coroutine movementRoutine;

    private UnitMovement unitMovement;
    private Vector3 targetPosition;

    [SerializeField]
    private Tilemap tilemap;

    private Dictionary<Vector3Int, PathNode> nodes = new Dictionary<Vector3Int, PathNode>();

    [SerializeField]
    private UnitTracker unitManager;

    //Used to decide which order enemies will move in
    public int initiative = 5;
    // Debug code used so we can read the debug log and tell which enemy is moving
    public int enemyNumber = 1;

    // Updated to utilize the stats tied to the Unit
    private Stats stats;

    // Used in testing only to make sure enemy can actually move
    // Commented out as it should only be used if something needs to be tested seperatly from the turn system for whatever reason
    //public InputActionReference moveTestAction;
    void Awake()
    {
        stats = GetComponent<Stats>();
        unitMovement = GetComponent<UnitMovement>();

        initiative = (Random.Range(0, 100) + stats.agility);
    }

    /*private void OnEnable()
    {
        moveTestAction.Enable();
        moveTestAction.performed += TestMove;
    }

    private void OnDisable()
    {
        moveTestAction.performed -= TestMove;
        moveTestAction.Disable();
    }*/

    // Debug test code from before implmentation of turns
    // Left in just incase it's needed again for testing purposes
    /*private void Update()
    {
        if (moveTestAction.action.WasPressedThisFrame())
        {
            Debug.Log("Enemy move start");
            PlayerUnit target = FindNearestPlayerUnit();
            MoveTowardsNearestPlayerUnit(target);
        }
    }*/

    public void runEnemyAI()
    {
        /*
        * First checks if there is a player within the unit's attack range and if so attacks that unit then ends this enemies turn
        * If not then it will attempt to move to a player then attack if possible
        * If there is no player in range then the enemy will move as close as possible to the nearest player then end it's turn
        */

        Debug.Log("Beginning Turn for Enemy:" + enemyNumber);
        PlayerUnit target = FindNearestPlayerUnit();
        MoveTowardsNearestPlayerUnit(target);
    }

    private void MoveTowardsNearestPlayerUnit(PlayerUnit target)
    {

        if (target == null)
        {
            Debug.Log("No player found");
            return;
        }

       /* Vector3Int startCell = grid.WorldToCell(transform.position);

        Vector3Int targetCell = grid.WorldToCell(target.transform.position);*/

        List<PathNode> fullPath = FindPathToAttackPosition(target);

        if (fullPath == null)
        {
            Debug.Log("No path found");
            return;
        }


        // Gets a path that is reachable within the enemy unit's movement cost.
        List<PathNode> reachablePath = GetReachablePath(fullPath);

        if (reachablePath.Count == 0)
        {
            Debug.Log("No Reachable path found");
            return;
        }

        if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
        }

        movementRoutine = StartCoroutine(FollowPath(reachablePath));
    }

    private PlayerUnit FindNearestPlayerUnit()
    {
        PlayerUnit[] playerUnits = FindObjectsByType<PlayerUnit>();

        PlayerUnit closestPlayerUnit = null;

        int cheapestPathCost = int.MaxValue;

        Vector3Int enemyCell = grid.WorldToCell(transform.position);

        foreach (PlayerUnit playerUnit in playerUnits)
        {
            Vector3Int playerUnitCell = grid.WorldToCell(playerUnit.transform.position);

            List<PathNode> path = pathfinder.FindPlayer(enemyCell, playerUnitCell, unitMovement);

            Debug.Log($"Pathcost:{playerUnit}");
            if (path == null)
            {
                Debug.Log("Path null");
            }
            Debug.Log(path.Count);

            if (path == null || path.Count == 0)
            {
                continue;
            }

            int pathCost = path[path.Count - 1].gCost;

            Debug.Log($"Pathcost:{pathCost}");

            if (pathCost < cheapestPathCost)
            {
                cheapestPathCost =
                    pathCost;

                closestPlayerUnit =
                    playerUnit;
            }
        }

        return closestPlayerUnit;
    }

    private List<PathNode> FindPathToAttackPosition(PlayerUnit player)
    {
        Vector3Int enemyCell = grid.WorldToCell(transform.position);

        Vector3Int playerCell = grid.WorldToCell(player.transform.position);

        List<PathNode> bestPath = null;

        int cheapestCost = int.MaxValue;

        foreach (Vector3Int attackCell in GetAttackPositions(playerCell, stats.minRange, stats.maxRange))
        {
            if (unitManager.HasUnit(attackCell))
            {
                continue;
            }

            List<PathNode> path = pathfinder.FindPath(enemyCell, attackCell, unitMovement);

            if (path == null || path.Count == 0)
            {
                continue;
            }

            int cost = path[path.Count - 1].gCost;

            if (cost < cheapestCost)
            {
                cheapestCost = cost;
                bestPath = path;
            }
        }

        return bestPath;

    }

    private List<Vector3Int> GetAttackPositions(Vector3Int targetCell, int minRange, int maxRange)
    {
        /*return new List<Vector3Int>()
        {
            centerCell + new Vector3Int( 1,  0, 0),
            centerCell + new Vector3Int(-1,  0, 0),
            centerCell + new Vector3Int( 0,  1, 0),
            centerCell + new Vector3Int( 0, -1, 0)
        };*/

        List<Vector3Int> positions = new List<Vector3Int>();

        // Despite it looking like this is changing the y position it actually changes the z position of enemies.
        // This is just a quirk of how Unity handles tile maps with a XZY swizzle.

        for(int x = -maxRange; x <= maxRange; x++)
        {
            for(int y = -maxRange; y <= maxRange; y++)
            {
                int distance = Mathf.Abs(x) + Mathf.Abs(y);

                if (distance < minRange)
                {
                    continue;
                }

                if (distance > maxRange)
                {
                    continue;
                }

                positions.Add(targetCell + new Vector3Int(x, y, 0));
            }
        }

        return positions;
    }

    private List<PathNode> GetReachablePath(List<PathNode> fullPath)
    {
        List<PathNode> reachablePath = new();

        int remainingMovement = unitMovement.movementPoints;

        foreach (PathNode node in fullPath)
        {
            int terrainCost = unitMovement.GetTerrainCost(node.terrainType);

            if (terrainCost > remainingMovement)
            {
                break;
            }

            reachablePath.Add(node);

            remainingMovement -= terrainCost;
        }

        return reachablePath;
    }

    private IEnumerator FollowPath(List<PathNode> path)
    {

        unitManager.UnregisterUnit(grid.WorldToCell(transform.position));

        foreach (PathNode node in path)
        {
            targetPosition =GetWorldPosition(node.cellPosition);

            while (Vector3.Distance(transform.position,targetPosition) > 0.05f)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position,targetPosition,moveSpeed * Time.deltaTime);

                yield return null;
            }

            transform.position = targetPosition;
        }

        unitManager.RegisterUnit(grid.WorldToCell(transform.position), unitMovement);

        movementRoutine = null;
    }

    private Vector3 GetWorldPosition(Vector3Int cell)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(cell);

        worldPosition.y = yOffset;

        return worldPosition;
    }
}
