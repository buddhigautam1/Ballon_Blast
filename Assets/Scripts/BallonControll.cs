using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallonControll : MonoBehaviour
{
    [Header("Movement")]
    [Min(0.01f)]
    public float upSpeed = 0.05f;
    [SerializeField] private float speedIncreasePerPoint = 0.0018f;
    [SerializeField] private float maxUpSpeed = 0.15f;

    [Header("Session")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private float comboWindow = 1.2f;
    [SerializeField] private float topMissPadding = 0.8f;
    [SerializeField] private float sidePadding = 0.65f;
    [SerializeField] private float bottomSpawnPadding = 0.9f;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    private readonly Color defaultBalloonColor = new Color(1f, 0.28f, 0.45f);
    private readonly Color bonusBalloonColor = new Color(1f, 0.76f, 0.2f);
    private readonly Color panelColor = new Color(0.05f, 0.08f, 0.14f, 0.72f);

    private AudioSource audioSource;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem popParticles;

    private TextMeshProUGUI livesText;
    private TextMeshProUGUI bestText;
    private TextMeshProUGUI comboText;
    private TextMeshProUGUI levelText;
    private TextMeshProUGUI messageText;
    private TextMeshProUGUI pauseButtonText;
    private TextMeshProUGUI pauseOverlayText;

    private GameObject pauseOverlay;
    private Vector3 originalScale;
    private float currentSpeed;
    private float lastPopTime = -10f;
    private int score;
    private int lives;
    private int streak;
    private int bestStreak;
    private bool isBonusBalloon;
    private bool isPaused;
    private bool isEnding;
    private Coroutine messageRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        originalScale = transform.localScale;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;

        score = 0;
        lives = Mathf.Max(1, startingLives);
        streak = 0;
        bestStreak = 0;
        currentSpeed = Mathf.Max(0.01f, upSpeed);

        ConfigureCamera();
        BuildHud();
        CreatePopParticles();
        ResetPosition();
        UpdateHud();
        ShowMessage("Tap the balloon before it escapes!", 2.4f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        if (isPaused || isEnding)
        {
            return;
        }

        if (transform.position.y >= GetWorldTop() + topMissPadding)
        {
            HandleMissedBalloon();
        }
    }

    private void FixedUpdate()
    {
        if (isPaused || isEnding)
        {
            return;
        }

        transform.Translate(Vector3.up * currentSpeed);
    }

    private void OnMouseDown()
    {
        if (isPaused || isEnding)
        {
            return;
        }

        PopBalloon();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void ResetPosition()
    {
        float randomX = Random.Range(GetWorldLeft() + sidePadding, GetWorldRight() - sidePadding);
        float spawnY = GetWorldBottom() - bottomSpawnPadding;

        transform.position = new Vector2(randomX, spawnY);
        transform.localScale = originalScale * Random.Range(0.88f, 1.12f);

        isBonusBalloon = score > 0 && Random.value <= Mathf.Clamp(0.08f + score * 0.003f, 0.08f, 0.2f);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isBonusBalloon ? bonusBalloonColor : defaultBalloonColor;
        }
    }

    private void PopBalloon()
    {
        float timeSinceLastPop = Time.time - lastPopTime;
        streak = timeSinceLastPop <= comboWindow ? streak + 1 : 1;
        bestStreak = Mathf.Max(bestStreak, streak);
        lastPopTime = Time.time;

        int multiplier = 1 + Mathf.Min(streak / 5, 4);
        int points = isBonusBalloon ? multiplier * 3 : multiplier;
        score += points;

        currentSpeed = Mathf.Clamp(upSpeed + score * speedIncreasePerPoint, upSpeed, maxUpSpeed);

        PlayPopFeedback(points);
        ResetPosition();
        UpdateHud();
    }

    private void HandleMissedBalloon()
    {
        lives--;
        streak = 0;
        lastPopTime = -10f;
        UpdateHud();

        if (lives <= 0)
        {
            EndGame();
            return;
        }

        ShowMessage("Balloon escaped! " + lives + " lives left", 1.4f);
        ResetPosition();
    }

    private void EndGame()
    {
        if (isEnding)
        {
            return;
        }

        isEnding = true;
        GameSessionStats.SaveGameResult(score, bestStreak);
        ShowMessage(score > 0 ? "Great run!" : "Try again!", 0.35f);
        StartCoroutine(LoadGameOverAfterDelay());
    }

    private System.Collections.IEnumerator LoadGameOverAfterDelay()
    {
        yield return new WaitForSeconds(0.45f);
        SceneManager.LoadScene("GameOver");
    }

    private void TogglePause()
    {
        if (isEnding)
        {
            return;
        }

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pauseOverlay != null)
        {
            pauseOverlay.SetActive(isPaused);
        }

        if (pauseButtonText != null)
        {
            pauseButtonText.text = isPaused ? "RESUME" : "PAUSE";
        }
    }

    private void ConfigureCamera()
    {
        if (mainCamera == null)
        {
            return;
        }

        mainCamera.backgroundColor = new Color(0.48f, 0.78f, 1f);
        mainCamera.orthographicSize = Mathf.Max(mainCamera.orthographicSize, 5.6f);
    }

    private void BuildHud()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = canvas.gameObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        if (scoreText == null)
        {
            scoreText = CreateHudText(canvas.transform, "ScoreText", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(42f, -44f), new Vector2(420f, 72f), 40, TextAlignmentOptions.Left, Color.white);
        }
        else
        {
            ConfigureHudText(scoreText, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(42f, -44f), new Vector2(420f, 72f), 40, TextAlignmentOptions.Left, Color.white);
        }

        livesText = CreateHudText(canvas.transform, "LivesText", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(42f, -112f), new Vector2(420f, 58f), 30, TextAlignmentOptions.Left, new Color(1f, 0.87f, 0.38f));
        bestText = CreateHudText(canvas.transform, "BestScoreText", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-42f, -44f), new Vector2(460f, 72f), 32, TextAlignmentOptions.Right, Color.white);
        comboText = CreateHudText(canvas.transform, "ComboText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -126f), new Vector2(560f, 58f), 30, TextAlignmentOptions.Center, new Color(1f, 0.56f, 0.92f));
        levelText = CreateHudText(canvas.transform, "LevelText", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -72f), new Vector2(420f, 54f), 26, TextAlignmentOptions.Center, new Color(0.81f, 0.94f, 1f));
        messageText = CreateHudText(canvas.transform, "MessageText", new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 120f), 38, TextAlignmentOptions.Center, Color.white);

        CreatePauseButton(canvas.transform);
        CreatePauseOverlay(canvas.transform);
    }

    private TextMeshProUGUI CreateHudText(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, TextAlignmentOptions alignment, Color color)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        ConfigureHudText(text, anchorMin, anchorMax, pivot, anchoredPosition, sizeDelta, fontSize, alignment, color);
        return text;
    }

    private void ConfigureHudText(TextMeshProUGUI text, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta, int fontSize, TextAlignmentOptions alignment, Color color)
    {
        RectTransform rectTransform = text.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;

        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        text.enableWordWrapping = false;
        text.outlineWidth = 0.18f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.55f);
    }

    private void CreatePauseButton(Transform parent)
    {
        GameObject buttonObject = new GameObject("PauseButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(1f, 0f);
        rectTransform.anchoredPosition = new Vector2(-42f, 42f);
        rectTransform.sizeDelta = new Vector2(220f, 76f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = panelColor;

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.05f, 0.08f, 0.14f, 0.72f);
        colors.highlightedColor = new Color(0.12f, 0.2f, 0.32f, 0.9f);
        colors.pressedColor = new Color(0.02f, 0.04f, 0.08f, 0.95f);
        button.colors = colors;
        button.onClick.AddListener(TogglePause);

        pauseButtonText = CreateHudText(buttonObject.transform, "Text", Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, 24, TextAlignmentOptions.Center, Color.white);
        pauseButtonText.text = "PAUSE";
    }

    private void CreatePauseOverlay(Transform parent)
    {
        pauseOverlay = new GameObject("PauseOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        pauseOverlay.transform.SetParent(parent, false);

        RectTransform rectTransform = pauseOverlay.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = pauseOverlay.GetComponent<Image>();
        image.color = new Color(0.02f, 0.04f, 0.08f, 0.78f);
        image.raycastTarget = false;

        pauseOverlayText = CreateHudText(pauseOverlay.transform, "PauseOverlayText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(880f, 220f), 44, TextAlignmentOptions.Center, Color.white);
        pauseOverlayText.text = "PAUSED\n<size=28>Tap Resume or press P to continue</size>";
        pauseOverlay.SetActive(false);
    }

    private void CreatePopParticles()
    {
        GameObject particleObject = new GameObject("PopParticles", typeof(ParticleSystem));
        particleObject.transform.SetParent(transform, false);
        popParticles = particleObject.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule main = popParticles.main;
        main.duration = 0.35f;
        main.loop = false;
        main.startLifetime = 0.35f;
        main.startSpeed = 4f;
        main.startSize = 0.18f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = popParticles.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = popParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.3f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = popParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(bonusBalloonColor, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
    }

    private void PlayPopFeedback(int points)
    {
        if (audioSource != null)
        {
            audioSource.pitch = Mathf.Clamp(0.92f + streak * 0.025f, 0.92f, 1.35f);
            audioSource.Play();
        }

        if (popParticles != null)
        {
            ParticleSystem.MainModule main = popParticles.main;
            main.startColor = isBonusBalloon ? bonusBalloonColor : defaultBalloonColor;
            popParticles.Emit(isBonusBalloon ? 28 : 18);
        }

        string bonusText = isBonusBalloon ? " BONUS" : string.Empty;
        string streakText = streak >= 3 ? " x" + streak : string.Empty;
        ShowMessage("+" + points + bonusText + streakText, 0.75f);
    }

    private void UpdateHud()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE " + score;
        }

        if (livesText != null)
        {
            livesText.text = "LIVES " + Mathf.Max(0, lives) + "/" + startingLives;
        }

        if (bestText != null)
        {
            bestText.text = "BEST " + Mathf.Max(GameSessionStats.BestScore, score);
        }

        if (comboText != null)
        {
            comboText.text = streak >= 2 ? "COMBO x" + streak : "BUILD A COMBO";
        }

        if (levelText != null)
        {
            int challengeLevel = 1 + Mathf.FloorToInt(score / 12f);
            levelText.text = "LEVEL " + challengeLevel + "  SPEED " + Mathf.RoundToInt((currentSpeed / upSpeed) * 100f) + "%";
        }
    }

    private void ShowMessage(string message, float duration)
    {
        if (messageText == null)
        {
            return;
        }

        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }

        messageRoutine = StartCoroutine(ShowMessageRoutine(message, duration));
    }

    private System.Collections.IEnumerator ShowMessageRoutine(string message, float duration)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        yield return new WaitForSecondsRealtime(duration);
        messageText.gameObject.SetActive(false);
        messageRoutine = null;
    }

    private float GetWorldLeft()
    {
        return GetViewportWorldPoint(new Vector3(0f, 0.5f, 0f)).x;
    }

    private float GetWorldRight()
    {
        return GetViewportWorldPoint(new Vector3(1f, 0.5f, 0f)).x;
    }

    private float GetWorldBottom()
    {
        return GetViewportWorldPoint(new Vector3(0.5f, 0f, 0f)).y;
    }

    private float GetWorldTop()
    {
        return GetViewportWorldPoint(new Vector3(0.5f, 1f, 0f)).y;
    }

    private Vector3 GetViewportWorldPoint(Vector3 viewportPoint)
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return viewportPoint;
        }

        viewportPoint.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ViewportToWorldPoint(viewportPoint);
    }
}
