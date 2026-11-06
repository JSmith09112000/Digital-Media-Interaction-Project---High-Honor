using System.Collections;
using System.Collections.Generic;
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
    private UnitManager unitManager;

    // Used in testing only to make sure enemy can actually move
    public InputActionReference moveTestAction;
    void Awake()
    {
        unitMovement = GetComponent<UnitMovement>();
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

    private void Update()
    {
        if (moveTestAction.action.WasPressedThisFrame())
        {
            Debug.Log("Enemy move start");
            MoveTowardsNearestPlayerUnit();
        }
    }

    private void MoveTowardsNearestPlayerUnit()
    {
        PlayerUnit target = FindNearestPlayerUnit();

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

        foreach (Vector3Int attackCell in GetAdjacentCells(playerCell))
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

    private List<Vector3Int> GetAdjacentCells(Vector3Int centerCell)
    {
        return new List<Vector3Int>()
        {
            centerCell + new Vector3Int( 1,  0, 0),
            centerCell + new Vector3Int(-1,  0, 0),
            centerCell + new Vector3Int( 0,  1, 0),
            centerCell + new Vector3Int( 0, -1, 0)
        };
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

    private List<Vector3Int> GetAdjacentPlayerCells(Vector3Int centerCell)
    {
        return new List<Vector3Int>()
        {
            centerCell + new Vector3Int( 1,  0, 0),
            centerCell + new Vector3Int(-1,  0, 0),
            centerCell + new Vector3Int( 0,  1, 0),
            centerCell + new Vector3Int( 0, -1, 0)
        };
    }
}
