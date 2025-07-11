using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CameraDrag : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public float dragSpeed = 0.5f;
    public float smoothTime = 0.2f;
    public float rotationSpeed = 0.2f;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private PlayerInputAction dragAction;

    private bool isMobileRotating = false;
    private bool isPcRotating = false;

    void Awake()
    {
        dragAction = new PlayerInputAction();
        targetPosition = mainCamera.transform.position;
    }
    void OnEnable()
    {
        dragAction?.Enable();
        EnhancedTouchSupport.Enable();
    }
    void OnDisable()
    {
        dragAction?.Disable();
        EnhancedTouchSupport.Disable();
    }
    void Update()
    {
        if (Application.isMobilePlatform)
            HandleMobileInput();
        else
            HandlePcInput();

        SmoothMoveCamera();
    }
    // ------------------------
    // PC Input Handling
    // ------------------------
    private void HandlePcInput()
    {
        bool tapPressed = dragAction?.GamePlay.Tap?.IsPressed() ?? false;
        bool rotatePressed = dragAction?.GamePlay.TapRotation?.IsPressed() ?? false;

        if (rotatePressed)
        {
            OnCameraRotationPc();
        }
        else
        {
            isPcRotating = false;
        }

        if (tapPressed && !isPcRotating && !IsPointerOverUI())
        {
            OnCameraDrag();
        }
    }

    // ------------------------
    // Mobile Input Handling
    // ------------------------
    private void HandleMobileInput()
    {
        int touchCount = Touch.activeTouches.Count;

        if (touchCount == 2)
        {
            OnCameraRotationMb();
        }
        else
        {
            isMobileRotating = false;
        }

        bool tapPressed = dragAction?.GamePlay.Tap?.IsPressed() ?? false;

        if (touchCount == 1 && tapPressed && !isMobileRotating && !IsPointerOverUI())
        {
            OnCameraDrag();
        }
    }

    // ------------------------
    // Drag camera
    // ------------------------
    private void OnCameraDrag()
    {
        Vector2 delta = dragAction.GamePlay.Delta.ReadValue<Vector2>();

        Vector3 right = mainCamera.transform.right;
        Vector3 forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;

        Vector3 move = (-right * delta.x + -forward * delta.y) * dragSpeed * Time.deltaTime;
        targetPosition += move;
    }

    // ------------------------
    // Rotate camera - PC
    // ------------------------
    private void OnCameraRotationPc()
    {
        Vector2 delta = dragAction.GamePlay.Delta.ReadValue<Vector2>();
        float yaw = -delta.x * rotationSpeed * Time.deltaTime;

        mainCamera.transform.Rotate(Vector3.up, yaw, Space.World);
        isPcRotating = true;
    }

    // ------------------------
    // Rotate camera - Mobile (2 fingers same direction)
    // ------------------------
    private void OnCameraRotationMb()
    {
        if (Touch.activeTouches.Count < 2) return;

        var touch1 = Touch.activeTouches[0];
        var touch2 = Touch.activeTouches[1];

        Vector2 delta1 = touch1.delta;
        Vector2 delta2 = touch2.delta;

        float similarity = Vector2.Dot(delta1.normalized, delta2.normalized);

        if (similarity > 0.85f)
        {
            float avgDeltaX = (delta1.x + delta2.x) / 2f;
            float normalizedDeltaX = avgDeltaX / Screen.width;

            mainCamera.transform.Rotate(Vector3.up, normalizedDeltaX * rotationSpeed * 100f * Time.deltaTime, Space.World);
            isMobileRotating = true;
        }
    }

    // ------------------------
    // Smooth camera movement
    // ------------------------
    private void SmoothMoveCamera()
    {
        mainCamera.transform.position = Vector3.SmoothDamp(
            mainCamera.transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }

    // ------------------------
    // UI detection (for drag-block)
    // ------------------------
    private bool IsPointerOverUI()
    {
#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject();
#else
        return Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
    }
}
