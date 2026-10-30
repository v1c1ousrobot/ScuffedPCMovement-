using UnityEngine;
using UnityEngine.InputSystem;

public class DesktopController : MonoBehaviour
{
    public Transform playerRoot;
    public Transform playerHead;
    public float moveSpeed = 5f;
    public float flySpeed = 5f;
    [Range(1, 10)]
    public float mouseSensitivity = 5f;

    private float rotX;
    private float rotY;
    private InputAction moveAction;
    private InputAction unlockAction;
    private bool isFlying;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        unlockAction = new InputAction("Unlock", InputActionType.Button);
        unlockAction.AddBinding("<Keyboard>/escape");

        moveAction.Enable();
        unlockAction.Enable();

        unlockAction.performed += _ =>
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        };
    }

    float GetLookSpeed()
    {
        float t = (mouseSensitivity - 1f) / 9f;
        return Mathf.Lerp(0.05f, 2f, t);
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Cursor.lockState != CursorLockMode.Locked) return;

        bool holdingE = Keyboard.current.eKey.isPressed;
        bool holdingQ = Keyboard.current.qKey.isPressed;

        if (!holdingE && !holdingQ)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            float speed = GetLookSpeed();
            rotY += mouseDelta.x * speed;
            rotX -= mouseDelta.y * speed;
            rotX = Mathf.Clamp(rotX, -90f, 90f);
            playerHead.localRotation = Quaternion.Euler(rotX, rotY, 0);
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = playerHead.right * moveInput.x + playerHead.forward * moveInput.y;
        move.y = 0;

        isFlying = Keyboard.current.spaceKey.isPressed;

        if (isFlying)
        {
            move += Vector3.up;
            move = playerHead.TransformDirection(move.normalized);
            playerRoot.position += move * flySpeed * Time.deltaTime;
        }
        else
        {
            playerRoot.position += move * moveSpeed * Time.deltaTime;
        }
    }

    void OnDestroy()
    {
        moveAction?.Disable();
        unlockAction?.Disable();
        moveAction?.Dispose();
        unlockAction?.Dispose();
    }
}

