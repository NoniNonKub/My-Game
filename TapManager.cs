using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public class TapManager : MonoBehaviour
{
    public static TapManager Instance { get; private set; }

    public Camera mainCamera;
    public LayerMask groundLayer;

    public float dragThreshold = 10f; // 🎯 ปรับตามความเหมาะสม (หน่วยเป็นพิกเซล)

    public Vector2 LastScreenPosition { get; private set; }
    public Vector3 LastWorldPosition { get; private set; }

    public static event Action<Vector2, Vector3> OnTap;

    private Vector2 dragStartPos;
    private bool isTouching = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        // กดทับ UI → ไม่ต้องทำงาน
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2 screenPos = Vector2.zero;

        // 🟠 TOUCH
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                dragStartPos = touch.position.ReadValue();
                isTouching = true;
            }

            if (touch.press.wasReleasedThisFrame && isTouching)
            {
                screenPos = touch.position.ReadValue();
                isTouching = false;

                if (Vector2.Distance(screenPos, dragStartPos) < dragThreshold)
                    TryTap(screenPos);
            }
        }

        // 🟢 MOUSE
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                dragStartPos = Mouse.current.position.ReadValue();
                isTouching = true;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && isTouching)
            {
                screenPos = Mouse.current.position.ReadValue();
                isTouching = false;

                if (Vector2.Distance(screenPos, dragStartPos) < dragThreshold)
                    TryTap(screenPos);
            }
        }
    }

    void TryTap(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            LastScreenPosition = screenPos;
            LastWorldPosition = hit.point;

            OnTap?.Invoke(screenPos, hit.point);
        }
    }
}
