using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    //[field: SerializeField] public string HorizontalAxis { get; private set; } = "Horizontal";
    //[field: SerializeField] public string VerticalAxis { get; private set; } = "Vertical";

    //private Camera _camera;

    //private CameraControls cameraControls;

    /*private void Awake() {
        cameraControls = new CameraControls();
    }

    private void OnEnable()
    {
        cameraControls.Enable();
    }*/

    public Vector3 position;

    public Transform[] Transforms;

    public InputActionReference move;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
        //_camera = GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        if (move.action.WasPressedThisFrame())
        {
            Vector2 inputVector = move.action.ReadValue<Vector2>();
            // Correction to ensure no bugs where camera moves half a tile
            if (inputVector.x < 0)
            {
                inputVector.x = -1;
            }
            if (inputVector.x > 0)
            {
                inputVector.x = 1;
            }
            if (inputVector.y < 0)
            {
                inputVector.y = -1;
            }
            if (inputVector.y > 0)
            {
                inputVector.y = 1;
            }
            Vector3 movement = new Vector3(inputVector.x, 0, inputVector.y);
            position += movement;
            this.transform.position = position;
        }

        //_camera.transform.Translate(inputVector);
        //cameraMovement();
    }

    /*void cameraMovement(InputAction.CallbackContext context)
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            position.x += 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            position.x -= 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            position.z -= 1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            position.z += 1;
        }
    }*/
}
