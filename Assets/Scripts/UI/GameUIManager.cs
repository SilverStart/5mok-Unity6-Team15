using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameManager 이벤트를 구독하여 UI를 갱신합니다.
/// - 현재 턴 표시 (Black/White)
/// - 남은 시간 텍스트 및 게이지 바
/// - 게임 오버 패널
/// Inspector에서 UI 참조를 넣지 않으면 테스트용 UI를 런타임에 자동 생성합니다.
/// </summary>
public class GameUIManager : MonoBehaviour
{
    // ========== UI 참조 (Inspector에서 연결, 비워두면 자동 생성) ==========
    [Header("UI References (선택사항 - 비워두면 테스트용 UI 자동 생성)")]
    [SerializeField] Text turnText;           // 현재 턴 표시용 텍스트
    [SerializeField] Text timerText;          // 남은 시간 표시용 텍스트
    [SerializeField] Image timeGaugeFill;     // 남은 시간 게이지 (Image Type = Filled, Fill Method = Horizontal)
    [SerializeField] GameObject gameOverPanel;// 게임 오버 시 표시할 전체 화면 패널
    [SerializeField] Text gameOverText;       // 게임 오버 메시지 텍스트

    // ========== Unity 생명주기 ==========
    void Start()
    {
        // UI 참조가 없으면 테스트용 UI 자동 생성
        if (!HasValidReferences())
            CreateTestUI();

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameUIManager: GameManager가 씬에 없습니다. GameManager 오브젝트를 추가하세요.");
            return;
        }

        SubscribeToGameManager();
        UpdateTurnDisplay(GameManager.Instance.currentTurn);
        UpdateTimerDisplay(1f);

        Debug.Log("GameUIManager 시작 - Space: 턴 전환 테스트 (UI용)");
    }

    /// <summary> 테스트용: Space 키로 턴 전환 시뮬레이션 </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GameManager.Instance != null &&
            GameManager.Instance.currentState == GameState.Playing)
        {
            GameManager.Instance.SwitchTurn();
        }
    }

    /// <summary> 메모리 누수 방지: GameManager 이벤트 구독 해제 </summary>
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTurnChanged.RemoveListener(UpdateTurnDisplay);
            GameManager.Instance.OnTimerUpdate.RemoveListener(UpdateTimerDisplay);
            GameManager.Instance.OnGameOver.RemoveListener(ShowGameOver);
        }
    }

    // ========== GameManager 연동 ==========
    /// <summary> 최소 UI 참조(turnText + timerText 또는 timeGaugeFill)가 있는지 확인 </summary>
    bool HasValidReferences()
    {
        return turnText != null && (timerText != null || timeGaugeFill != null);
    }

    /// <summary> GameManager의 OnTurnChanged, OnTimerUpdate, OnGameOver 이벤트 구독 </summary>
    void SubscribeToGameManager()
    {
        GameManager.Instance.OnTurnChanged.AddListener(UpdateTurnDisplay);
        GameManager.Instance.OnTimerUpdate.AddListener(UpdateTimerDisplay);
        GameManager.Instance.OnGameOver.AddListener(ShowGameOver);
    }

    /// <summary> OnTurnChanged 콜백 - 현재 턴 텍스트 갱신 </summary>
    void UpdateTurnDisplay(PlayerTurn turn)
    {
        if (turnText != null)
            turnText.text = $"현재 턴: {turn}";
    }

    /// <summary> OnTimerUpdate 콜백 - 남은 시간 텍스트 및 게이지 바 갱신 (ratio: 0~1) </summary>
    void UpdateTimerDisplay(float ratio)
    {
        if (timerText != null && GameManager.Instance != null)
        {
            float remaining = GameManager.Instance.currentTime;
            timerText.text = $"남은 시간: {remaining:F1}초";
        }

        if (timeGaugeFill != null)
        {
            timeGaugeFill.type = Image.Type.Filled;
            timeGaugeFill.fillMethod = Image.FillMethod.Horizontal;
            timeGaugeFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            timeGaugeFill.fillAmount = Mathf.Clamp01(ratio);
        }
    }

    /// <summary> OnGameOver 콜백 - 게임 오버 패널 표시 </summary>
    void ShowGameOver(string message)
    {
        if (gameOverText != null)
            gameOverText.text = message;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    // ========== 테스트용 UI 자동 생성 ==========
    /// <summary>
    /// Inspector에 UI가 없을 때 런타임에 기본 UI 생성.
    /// Canvas가 없으면 자동 생성하며, TurnText, TimerText, GaugeFill, GameOverPanel을 만듭니다.
    /// </summary>
    void CreateTestUI()
    {
        if (!TryCreateCanvas(out Canvas canvas))
        {
            Debug.LogWarning("GameUIManager: UI 생성 실패. Canvas를 수동으로 만들고 UI 참조를 지정하세요.");
            return;
        }

        // 턴 표시
        turnText = CreateText(canvas.transform, "TurnText", "현재 턴: Black",
            new Vector2(0, 1), new Vector2(0.5f, 1f), new Vector2(200, -40), 24);

        // 타이머 텍스트
        timerText = CreateText(canvas.transform, "TimerText", "남은 시간: 30.0초",
            new Vector2(0, 1), new Vector2(0.5f, 1f), new Vector2(200, -80), 20);

        // 게이지 바 배경
        var gaugeBg = CreateImage(canvas.transform, "GaugeBg",
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -120),
            new Vector2(300, 20), new Color(0.2f, 0.2f, 0.2f));

        // 게이지 바 채움
        var gaugeFillObj = new GameObject("GaugeFill");
        gaugeFillObj.transform.SetParent(gaugeBg.transform, false);
        timeGaugeFill = gaugeFillObj.AddComponent<Image>();
        timeGaugeFill.color = new Color(0.2f, 0.6f, 1f);
        timeGaugeFill.type = Image.Type.Filled;
        timeGaugeFill.fillMethod = Image.FillMethod.Horizontal;
        timeGaugeFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        timeGaugeFill.fillAmount = 1f;
        timeGaugeFill.sprite = CreateWhiteSprite();

        var rt = gaugeFillObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // 게임 오버 패널
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        var panelRt = gameOverPanel.AddComponent<RectTransform>();
        panelRt.anchorMin = Vector2.zero;
        panelRt.anchorMax = Vector2.one;
        panelRt.offsetMin = Vector2.zero;
        panelRt.offsetMax = Vector2.zero;
        var panelImg = gameOverPanel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.7f);
        panelImg.sprite = CreateWhiteSprite();

        gameOverText = CreateText(gameOverPanel.transform, "GameOverText", "Game Over",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, 36);

        gameOverPanel.SetActive(false);

        Debug.Log("GameUIManager: 테스트용 UI 자동 생성 완료");
    }

    /// <summary> 씬에 Canvas가 있으면 반환, 없으면 새로 생성 후 반환 </summary>
    bool TryCreateCanvas(out Canvas canvas)
    {
        canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            return true;
        }

        var canvasObj = new GameObject("GameUICanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return true;
    }

    /// <summary> 런타임에 uGUI Text 생성 (테스트용 UI에 사용) </summary>
    Text CreateText(Transform parent, string name, string content,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(400, 40);

        var text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Font.CreateDynamicFontFromOSFont("Arial", fontSize);
        text.color = Color.white;

        return text;
    }

    /// <summary> 1x1 흰색 스프라이트 생성 (Image용, 에셋 없이 UI 그리기) </summary>
    Sprite CreateWhiteSprite()
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
    }

    /// <summary> 런타임에 uGUI Image 생성 (테스트용 게이지 바 등에 사용) </summary>
    Image CreateImage(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos,
        Vector2 sizeDelta, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;

        var img = go.AddComponent<Image>();
        img.color = color;
        img.sprite = CreateWhiteSprite();

        return img;
    }
}
