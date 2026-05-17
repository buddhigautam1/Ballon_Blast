using UnityEngine;

public class MobileSafeArea : MonoBehaviour
{
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;
    private RectTransform rectTransform;

    public static RectTransform GetOrCreateSafeAreaRoot(Transform canvasTransform)
    {
        Transform existingRoot = canvasTransform.Find("SafeArea");
        if (existingRoot != null)
        {
            RectTransform existingRect = existingRoot as RectTransform;
            if (existingRect != null && existingRoot.GetComponent<MobileSafeArea>() == null)
            {
                existingRoot.gameObject.AddComponent<MobileSafeArea>();
            }

            return existingRect;
        }

        GameObject rootObject = new GameObject("SafeArea", typeof(RectTransform), typeof(MobileSafeArea));
        rootObject.transform.SetParent(canvasTransform, false);

        RectTransform rootRect = rootObject.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        return rootRect;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void Update()
    {
        Rect currentSafeArea = Screen.safeArea;
        Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);

        if (currentSafeArea != lastSafeArea || currentScreenSize != lastScreenSize)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        lastSafeArea = Screen.safeArea;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);

        Vector2 anchorMin = lastSafeArea.position;
        Vector2 anchorMax = lastSafeArea.position + lastSafeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
