using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    private readonly Color primaryColor = new Color(1f, 0.35f, 0.46f);
    private readonly Color secondaryColor = new Color(0.1f, 0.17f, 0.28f, 0.9f);

    private void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 60;
        StyleCamera();
        StyleExistingMenu();
        AddMenuDetails();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void startGame()
    {
        StartGame();
    }

    public void StopGame()
    {
        Application.Quit();
    }

    public void stopGame()
    {
        StopGame();
    }

    private void StyleCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = new Color(0.48f, 0.78f, 1f);
        }
    }

    private void StyleExistingMenu()
    {
        foreach (Text text in FindObjectsOfType<Text>())
        {
            if (text.text.ToUpperInvariant().Contains("GAME ON"))
            {
                text.text = "BALLOON RUSH";
                text.fontSize = 52;
                text.fontStyle = FontStyle.Bold;
                text.color = Color.white;
            }
            else if (text.text.ToUpperInvariant().Contains("START"))
            {
                text.text = "PLAY";
                StyleButtonLabel(text);
            }
            else if (text.text.ToUpperInvariant().Contains("QUIT"))
            {
                text.text = "QUIT";
                StyleButtonLabel(text);
            }
        }

        foreach (Button button in FindObjectsOfType<Button>())
        {
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = button.name.ToUpperInvariant().Contains("QUIT") ? secondaryColor : primaryColor;
            }

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.88f, 0.94f);
            colors.pressedColor = new Color(0.88f, 0.22f, 0.34f);
            button.colors = colors;
        }
    }

    private void StyleButtonLabel(Text text)
    {
        text.fontSize = 34;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
    }

    private void AddMenuDetails()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        EnsureScaler(canvas);
        Transform safeAreaRoot = MobileSafeArea.GetOrCreateSafeAreaRoot(canvas.transform);
        CreatePanel(safeAreaRoot);

        string bestScore = "BEST SCORE: " + GameSessionStats.BestScore;
        string gamesPlayed = "RUNS PLAYED: " + GameSessionStats.GamesPlayed;
        CreateText(safeAreaRoot, "BestScore", bestScore + "\n" + gamesPlayed, new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.62f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(720f, 120f), 30, TextAnchor.MiddleCenter, Color.white);
        CreateText(safeAreaRoot, "HowToPlay", "Tap balloons quickly, chain combos, catch golden bonus balloons, and survive with 3 lives.", new Vector2(0.5f, 0.18f), new Vector2(0.5f, 0.18f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(820f, 120f), 26, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f));
        CreateText(safeAreaRoot, "MobileReady", "MOBILE READY: iPhone safe area + Android touch controls", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 42f), new Vector2(760f, 60f), 22, TextAnchor.MiddleCenter, new Color(0.81f, 0.94f, 1f));
    }

    private void EnsureScaler(Canvas canvas)
    {
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;
    }

    private void CreatePanel(Transform parent)
    {
        GameObject panelObject = new GameObject("MenuStatsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.62f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.62f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, -10f);
        rectTransform.sizeDelta = new Vector2(780f, 150f);

        Image image = panelObject.GetComponent<Image>();
        image.color = new Color(0.05f, 0.08f, 0.14f, 0.62f);
        image.raycastTarget = false;
    }

    private Text CreateText(Transform parent, string objectName, string content, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, TextAnchor alignment, Color color)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }
}
