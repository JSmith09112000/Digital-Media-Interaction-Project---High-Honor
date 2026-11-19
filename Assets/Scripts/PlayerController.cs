using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputAction leftClickAction;

    [SerializeField]
    private InputAction rightClickAction;

    private PlayerUnit selectedUnit = null;

    // Used in highlighting selected tile
    [SerializeField]
    private GameObject hoverIndicator;

    [SerializeField]
    private UnitTracker unitTracker;

    [SerializeField]
    private TurnTracker turnTracker;

    //private UnitMovement unitMovement;

    private Camera mainCamera;

    [SerializeField]
    private Grid grid;

    public bool unitPerfomingAction = false;

    // A list of all player units
    public static List<PlayerUnit> playerUnits;

    //For which unit the cursor is currently over
    private Stats hoveredUnit;

    [SerializeField]
    private StatsUI statsUI;

    private void Awake()
    {

        mainCamera = Camera.main;

        playerUnits = Object.FindObjectsByType<PlayerUnit>().ToList<PlayerUnit>();

        //unitMovement = GetComponent<UnitMovement>();

    }

    private void OnEnable()
    {
        leftClickAction.Enable();
        leftClickAction.performed += LeftClicked;
        rightClickAction.Enable();
        rightClickAction.performed += RightClicked;
    }

    private void OnDisable()
    {
        leftClickAction.performed -= LeftClicked;
        leftClickAction.Disable();
        rightClickAction.Disable();
        rightClickAction.performed -= RightClicked;
    }

    private void Update()
    {
        if (turnTracker.enemyTurn)
        {
            return;
        }
        HoverHighlight();
    }

    // Refreshes the units on start of turn
    public void RefreshUnits()
    {
        for(int i = 0; i < playerUnits.Count(); i++)
        {
            playerUnits[i].hasAttacked = false;
            playerUnits[i].hasMoved = false;
            unitPerfomingAction = false;
        }
    }

    private void HoverHighlight()
    {
        Ray ray = mainCamera.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hoverIndicator.SetActive(true);
            Vector3 newPosition = hit.collider.transform.position;
            newPosition.y = 2;
            hoverIndicator.transform.position = newPosition;

            Terrain terrain = hit.collider.GetComponent<Terrain>();

            Vector3Int targetCell = grid.WorldToCell(terrain.transform.position);

            Stats detectedUnit = null;

            if (unitTracker.HasUnit(targetCell))
            {
                detectedUnit = unitTracker.GetUnit(targetCell).GetComponent<Stats>();
            }

            if (detectedUnit != hoveredUnit)
            {
                hoveredUnit = detectedUnit;

                if (hoveredUnit != null)
                {
                    statsUI.ShowStats(hoveredUnit);
                }
                else
                {
                    statsUI.HideStats();
                }
            }
        }
        else
        {
            hoverIndicator.SetActive(false);
        }
    }

    public void DeselectUnit()
    {
        selectedUnit = null;
    }

    private void LeftClicked(InputAction.CallbackContext context)
    {

        if (turnTracker.enemyTurn)
        {
            Debug.Log("Currently it is the enemy turn");
            return;
        }

        if (unitPerfomingAction)
        {
            Debug.Log("A unit is currently busy. Please wait for them to finish their action.");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();

            if (terrain == null)
            {
                Debug.Log("terrain null");
                return;
            }

            Vector3Int targetCell = grid.WorldToCell(terrain.transform.position);

            if (unitTracker.HasUnit(targetCell))
            {
                Debug.Log("Cell has unit");
                if (unitTracker.GetUnit(targetCell).TryGetComponent<PlayerUnit>(out _))
                {
                    selectedUnit = unitTracker.GetUnit(targetCell).GetComponent<PlayerUnit>();
                    return;
                }
            }

            if (selectedUnit == null)
            {
                Debug.Log("Selected unit null");
                return;
            }

            Debug.Log(selectedUnit);
            unitPerfomingAction = true;
            selectedUnit.Move();
            
        }
    }

    public void RightClicked(InputAction.CallbackContext context)
    {
        if (turnTracker.enemyTurn)
        {
            Debug.Log("Currently it is the enemy turn");
            return;
        }
        if (unitPerfomingAction)
        {
            Debug.Log("A unit is currently busy. Please wait for them to finish their action.");
            return;
        }


        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();

            if (terrain == null)
            {
                return;
            }

            Vector3Int targetCell = grid.WorldToCell(terrain.transform.position);

            if (unitTracker.HasUnit(targetCell))
            {
            }

            if (selectedUnit == null)
            {
                return;
            }

            Debug.Log(selectedUnit);
            unitPerfomingAction = true;
            selectedUnit.Attack();

        }
    }

    /*private IEnumerator PlayerMoveTowards(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            Vector3 destination = Vector3.MoveTowards(
                transform.position,
                target,
                playerSpeed * Time.deltaTime
            );

            transform.position = destination;

            yield return null;
        }

    }*/

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(targetPosition, 1);
    }*/
}
