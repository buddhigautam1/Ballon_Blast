using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string menuSceneName = "Menu";

    private readonly Color primaryColor = new Color(1f, 0.35f, 0.46f);
    private readonly Color secondaryColor = new Color(0.1f, 0.17f, 0.28f, 0.9f);

    private void Start()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 60;
        StyleCamera();
        StyleExistingScreen();
        AddResultsPanel();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void restartGame()
    {
        RestartGame();
    }

    public void MenuGame()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    public void menuGame()
    {
        MenuGame();
    }

    private void StyleCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = new Color(0.48f, 0.78f, 1f);
        }
    }

    private void StyleExistingScreen()
    {
        foreach (Text text in FindObjectsOfType<Text>())
        {
            string upperText = text.text.ToUpperInvariant();
            if (upperText.Contains("GAME ON"))
            {
                text.text = "GAME OVER";
                text.fontSize = 54;
                text.fontStyle = FontStyle.Bold;
                text.color = Color.white;
            }
            else if (upperText.Contains("RESTART"))
            {
                text.text = "PLAY AGAIN";
                StyleButtonLabel(text);
            }
            else if (upperText.Contains("MENU"))
            {
                text.text = "MENU";
                StyleButtonLabel(text);
            }
        }

        foreach (Button button in FindObjectsOfType<Button>())
        {
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = button.name.ToUpperInvariant().Contains("MAIN") ? secondaryColor : primaryColor;
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
        text.fontSize = 30;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
    }

    private void AddResultsPanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        EnsureScaler(canvas);
        CreatePanel(canvas.transform);

        int lastScore = GameSessionStats.LastScore;
        int bestScore = GameSessionStats.BestScore;
        int bestStreak = GameSessionStats.LastBestStreak;
        string headline = GameSessionStats.LastWasBest ? "NEW BEST RUN!" : GetEncouragement(lastScore);
        string resultText = headline + "\nSCORE: " + lastScore + "\nBEST: " + bestScore + "\nBEST COMBO: x" + bestStreak;

        CreateText(canvas.transform, "ResultsText", resultText, new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(760f, 260f), 34, TextAnchor.MiddleCenter, Color.white);
        CreateText(canvas.transform, "RetryTip", "Tip: keep pops inside the combo window to multiply every point.", new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(860f, 100f), 24, TextAnchor.MiddleCenter, new Color(0.95f, 0.98f, 1f));
    }

    private string GetEncouragement(int lastScore)
    {
        if (lastScore >= 40)
        {
            return "AMAZING!";
        }

        if (lastScore >= 20)
        {
            return "GREAT RUN!";
        }

        if (lastScore > 0)
        {
            return "GOOD START!";
        }

        return "READY FOR ANOTHER RUN?";
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
        GameObject panelObject = new GameObject("ResultsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.56f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.56f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(820f, 300f);

        Image image = panelObject.GetComponent<Image>();
        image.color = new Color(0.05f, 0.08f, 0.14f, 0.68f);
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
