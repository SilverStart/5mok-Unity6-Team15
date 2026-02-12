using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameLogic 이벤트를 구독하여 UI를 갱신합니다.
/// - 현재 턴 표시 (Black/White)
/// - 남은 시간 텍스트 및 게이지 바
/// - 게임 오버 패널
/// Inspector에서 UI 참조를 넣지 않으면 테스트용 UI를 런타임에 자동 생성합니다.
/// </summary>
public class GameUIManager : MonoBehaviour
{
    // Inspector
    [Header("UI References (선택사항 - 비워두면 테스트용 UI 자동 생성)")]
    [SerializeField] private Text _turnText;
    [SerializeField] private Text _timerText;
    [SerializeField] private Image _timeGaugeFill;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private Text _gameOverText;

    // 상수 (Magic Number 제거)
    private const int TurnTextFontSize = 24;
    private const int TimerTextFontSize = 20;
    private const int GameOverTextFontSize = 36;
    private const float TurnTextOffsetY = -40f;
    private const float TimerTextOffsetY = -80f;
    private const float GaugeOffsetY = -120f;
    private const float GaugeWidth = 300f;
    private const float GaugeHeight = 20f;
    private const float TextWidth = 400f;
    private const float TextHeight = 40f;
    private const float GaugeBgColorValue = 0.2f;
    private const float GaugeFillColorR = 0.2f;
    private const float GaugeFillColorG = 0.6f;
    private const float GaugeFillColorB = 1f;
    private const float GameOverPanelAlpha = 0.7f;

    // Unity Events
    private void Start()
    {
        if (!HasValidReferences())
            CreateTestUI();

        if (GameLogic.Instance == null)
        {
            Debug.LogWarning("GameUIManager: GameLogic이 씬에 없습니다. GameLogic 오브젝트를 추가하세요.");
            return;
        }

        SubscribeToGameLogic();
        UpdateTurnDisplay(GameLogic.Instance.CurrentTurn);
        UpdateTimerDisplay(1f);

        Debug.Log("GameUIManager 시작 - Space: 턴 전환 테스트 (UI용)");
    }

    private void Update()
    {
        HandleTestInput();
    }

    private void OnDestroy()
    {
        if (GameLogic.Instance != null)
        {
            GameLogic.Instance.RemoveTurnChangedListener(UpdateTurnDisplay);
            GameLogic.Instance.RemoveTimerUpdateListener(UpdateTimerDisplay);
            GameLogic.Instance.RemoveGameOverListener(ShowGameOver);
        }
    }

    // Public Methods
    // (없음)

    // Private Methods
    private bool HasValidReferences()
    {
        return _turnText != null && (_timerText != null || _timeGaugeFill != null);
    }

    private void SubscribeToGameLogic()
    {
        GameLogic.Instance.AddTurnChangedListener(UpdateTurnDisplay);
        GameLogic.Instance.AddTimerUpdateListener(UpdateTimerDisplay);
        GameLogic.Instance.AddGameOverListener(ShowGameOver);
    }

    /// <summary> 테스트용: Space 키로 턴 전환 시뮬레이션 </summary>
    private void HandleTestInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameLogic.Instance != null &&
            GameLogic.Instance.CurrentState == GameState.Playing)
        {
            GameLogic.Instance.SwitchTurn();
        }
    }

    private void UpdateTurnDisplay(Constants.StoneColor turn)
    {
        if (_turnText != null)
            _turnText.text = $"현재 턴: {turn}";
    }

    private void UpdateTimerDisplay(float ratio)
    {
        if (_timerText != null && GameLogic.Instance != null)
        {
            float remaining = GameLogic.Instance.CurrentTime;
            _timerText.text = $"남은 시간: {remaining:F1}초";
        }

        if (_timeGaugeFill != null)
        {
            _timeGaugeFill.type = Image.Type.Filled;
            _timeGaugeFill.fillMethod = Image.FillMethod.Horizontal;
            _timeGaugeFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            _timeGaugeFill.fillAmount = Mathf.Clamp01(ratio);
        }
    }

    private void ShowGameOver(string message)
    {
        if (_gameOverText != null)
            _gameOverText.text = message;

        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(true);
    }

    private void CreateTestUI()
    {
        if (!TryCreateCanvas(out Canvas canvas))
        {
            Debug.LogWarning("GameUIManager: UI 생성 실패. Canvas를 수동으로 만들고 UI 참조를 지정하세요.");
            return;
        }

        _turnText = CreateText(canvas.transform, "TurnText", "현재 턴: Black",
            new Vector2(0, 1), new Vector2(0.5f, 1f), new Vector2(200, TurnTextOffsetY), TurnTextFontSize);

        _timerText = CreateText(canvas.transform, "TimerText", "남은 시간: 30.0초",
            new Vector2(0, 1), new Vector2(0.5f, 1f), new Vector2(200, TimerTextOffsetY), TimerTextFontSize);

        Image gaugeBg = CreateImage(canvas.transform, "GaugeBg",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, GaugeOffsetY),
            new Vector2(GaugeWidth, GaugeHeight), new Color(GaugeBgColorValue, GaugeBgColorValue, GaugeBgColorValue));

        GameObject gaugeFillObj = new GameObject("GaugeFill");
        gaugeFillObj.transform.SetParent(gaugeBg.transform, false);
        _timeGaugeFill = gaugeFillObj.AddComponent<Image>();
        _timeGaugeFill.color = new Color(GaugeFillColorR, GaugeFillColorG, GaugeFillColorB);
        _timeGaugeFill.type = Image.Type.Filled;
        _timeGaugeFill.fillMethod = Image.FillMethod.Horizontal;
        _timeGaugeFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        _timeGaugeFill.fillAmount = 1f;
        _timeGaugeFill.sprite = CreateWhiteSprite();

        RectTransform gaugeFillRt = gaugeFillObj.GetComponent<RectTransform>();
        gaugeFillRt.anchorMin = Vector2.zero;
        gaugeFillRt.anchorMax = Vector2.one;
        gaugeFillRt.offsetMin = Vector2.zero;
        gaugeFillRt.offsetMax = Vector2.zero;

        _gameOverPanel = new GameObject("GameOverPanel");
        _gameOverPanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRt = _gameOverPanel.AddComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = Vector2.zero;
        panelRt.offsetMax = Vector2.zero;
        Image panelImg = _gameOverPanel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, GameOverPanelAlpha);
        panelImg.sprite = CreateWhiteSprite();

        _gameOverText = CreateText(_gameOverPanel.transform, "GameOverText", "Game Over",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, GameOverTextFontSize);

        _gameOverPanel.SetActive(false);

        Debug.Log("GameUIManager: 테스트용 UI 자동 생성 완료");
    }

    private bool TryCreateCanvas(out Canvas canvas)
    {
        canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
            return true;

        GameObject canvasObj = new GameObject("GameUICanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return true;
    }

    private Text CreateText(Transform parent, string name, string content,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, int fontSize)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(TextWidth, TextHeight);

        Text text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Font.CreateDynamicFontFromOSFont("Arial", fontSize);
        text.color = Color.white;

        return text;
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
    }

    private Image CreateImage(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos,
        Vector2 sizeDelta, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        Image img = go.AddComponent<Image>();
        img.color = color;
        img.sprite = CreateWhiteSprite();

        return img;
    }
}
