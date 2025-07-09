using UnityEngine;

public class TapUI : MonoBehaviour
{
    public static TapUI Instance { get; private set; }

    public RectTransform canvasRoot;
    public GameObject uiPrefab;
    public float minDistanceToShowAgain = 1.0f;

    private GameObject currentUI;
    private Vector3 lastShownWorldPos = Vector3.positiveInfinity;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // กันซ้ำ
        }
        else
        {
            Instance = this;
        }
    }

    void OnEnable()
    {
        TapManager.OnTap += ShowTapUI;
    }

    void OnDisable()
    {
        TapManager.OnTap -= ShowTapUI;
    }


    void ShowTapUI(Vector2 screenPos, Vector3 worldPos)
    {
        if (currentUI != null && Vector3.Distance(worldPos, lastShownWorldPos) < minDistanceToShowAgain)
            return;

        if (currentUI != null)
            Destroy(currentUI);

        currentUI = Instantiate(uiPrefab, canvasRoot);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRoot,
            screenPos,
            null,
            out Vector2 localPoint
        );

        currentUI.GetComponent<RectTransform>().anchoredPosition = localPoint;
        lastShownWorldPos = worldPos;
    }

    public void HideUI()
    {
        if (currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
            lastShownWorldPos = Vector3.positiveInfinity;
        }
    }

    void LateUpdate()
    {
        if (currentUI == null || canvasRoot == null || Camera.main == null)
            return;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(lastShownWorldPos);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRoot,
            screenPos,
            null,
            out Vector2 localPoint
        ))
        {
            RectTransform rt = currentUI.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = localPoint;
            }
        }
    }
}
