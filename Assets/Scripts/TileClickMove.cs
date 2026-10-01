using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileClickMove : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClickAction;

    [SerializeField]
    private float playerSpeed = 10f;

    private Camera mainCamera;
    private Coroutine coroutine;

    // This was missing
    private Vector3 targetPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        mouseClickAction.Enable();
        mouseClickAction.performed += Move;
    }

    private void OnDisable()
    {
        mouseClickAction.performed -= Move;
        mouseClickAction.Disable();
    }

    private void Move(InputAction.CallbackContext context)
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
        {

            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            targetPosition = hit.point;

            Debug.Log("Moving to: " + targetPosition);

            coroutine = StartCoroutine(PlayerMoveTowards(hit.point));
        }
        else
        {
            Debug.Log("Raycast hit NOTHING");
        }
    }

    private IEnumerator PlayerMoveTowards(Vector3 target)
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(targetPosition, 1);
    }
}
