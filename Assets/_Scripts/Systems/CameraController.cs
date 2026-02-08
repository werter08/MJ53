using UnityEngine;

public class CameraController : StaticInstance<CameraController>
{
    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;

    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = false;

    [Space]
    [Tooltip("The higher it is, the faster the camera moves. Recommended to increase for joystick games.")]
    public float sensitivity = 3f;

    [Tooltip("Camera vertical rotation limits (X = min up, Y = max down)")]
    public Vector2 cameraLimit = new Vector2(-45f, 40f);

    [Tooltip("When true → use arrow keys (←→↑↓) instead of mouse for rotation")]
    public bool useArrows = false;

    [Space]
    [Tooltip("Optional: min/max field of view when zooming")]
    public Vector2 fovLimits = new Vector2(20f, 100f);

    float mouseX;
    float mouseY;
    public float offsetDistanceY;

    Transform player;

    void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("CameraController: No GameObject with tag 'Player' found!");
        }

        offsetDistanceY = transform.position.y - (player != null ? player.position.y : 0f);

        // Lock and hide cursor only when using mouse look
        if (!useArrows && !clickToMoveCamera)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Skip input if right-click mode is enabled but not holding RMB
        if (clickToMoveCamera && Input.GetAxisRaw("Fire2") <= 0f)
            return;

        if (useArrows)
        {
            // Only arrow keys – no WASD
            float horizontal = Input.GetKey(KeyCode.RightArrow) ? 1f :
                               Input.GetKey(KeyCode.LeftArrow)  ? -1f : 0f;

            float vertical =   Input.GetKey(KeyCode.UpArrow)   ? 1f :
                               Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;

            // Scale with time so it feels smooth and roughly mouse-like
            mouseX += horizontal * sensitivity * 80f * Time.deltaTime;
            mouseY += vertical   * sensitivity * 60f * Time.deltaTime;  // vertical usually slower
        }
        else
        {
            // Original mouse look
            mouseX += Input.GetAxis("Mouse X") * sensitivity;
            mouseY += Input.GetAxis("Mouse Y") * sensitivity;
        }

        // Always clamp vertical angle
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Follow player – only vertical offset
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        // Zoom (with limits)
        if (canZoom)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                Camera.main.fieldOfView -= scroll * sensitivity * 40f;
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, fovLimits.x, fovLimits.y);
            }
        }

        // Apply rotation
        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0f);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}