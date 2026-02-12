using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환을 포함한 게임의 전체 흐름을 관리합니다.
/// - 씬 로드, 전환
/// - 게임 진입/종료 흐름
/// 오목 게임의 구체적인 로직(턴, 타이머 등)은 GameLogic에서 관리합니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    // 상수 (게임 씬 판별용)
    private const string DefaultGameSceneName = "SampleScene";
    private const string GameSceneKeyword = "Game";

    // Inspector (씬 전환 이벤트)
    [Header("Events")]
    [SerializeField] private UnityEvent<string> _onSceneLoaded;      // 씬 로드 완료 (sceneName)
    [SerializeField] private UnityEvent _onGameSceneEntered;         // 게임 씬 진입
    [SerializeField] private UnityEvent _onGameSceneExited;          // 게임 씬 이탈 (로드 전)

    // Unity Events
    private void Awake()
    {
        _onSceneLoaded ??= new UnityEvent<string>();
        _onGameSceneEntered ??= new UnityEvent();
        _onGameSceneExited ??= new UnityEvent();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        SceneManager.sceneUnloaded += HandleSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneUnloaded -= HandleSceneUnloaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Public Methods
    /// <summary> 지정한 씬 로드 </summary>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary> 지정한 씬 인덱스로 로드 </summary>
    public void LoadScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }

    /// <summary> 현재 씬 다시 로드 (재시작 등) </summary>
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Event Listener API
    public void AddSceneLoadedListener(UnityAction<string> callback) => _onSceneLoaded.AddListener(callback);
    public void RemoveSceneLoadedListener(UnityAction<string> callback) => _onSceneLoaded.RemoveListener(callback);
    public void AddGameSceneEnteredListener(UnityAction callback) => _onGameSceneEntered.AddListener(callback);
    public void RemoveGameSceneEnteredListener(UnityAction callback) => _onGameSceneEntered.RemoveListener(callback);
    public void AddGameSceneExitedListener(UnityAction callback) => _onGameSceneExited.AddListener(callback);
    public void RemoveGameSceneExitedListener(UnityAction callback) => _onGameSceneExited.RemoveListener(callback);

    // Private Methods
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _onSceneLoaded?.Invoke(scene.name);

        if (IsGameScene(scene.name))
            _onGameSceneEntered?.Invoke();
    }

    private void HandleSceneUnloaded(Scene scene)
    {
        if (IsGameScene(scene.name))
            _onGameSceneExited?.Invoke();
    }

    /// <summary> 게임 플레이 씬 여부 (씬 이름 규칙에 따라 수정) </summary>
    private bool IsGameScene(string sceneName)
    {
        return sceneName == DefaultGameSceneName || sceneName.Contains(GameSceneKeyword);
    }
}
