using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 오목 게임의 흐름을 관리합니다.
/// - 턴 관리, 타이머, 게임 오버
/// - BoardData, BoardRenderer, OmokRule와 연동
///
/// === 보드 팀 연동 가이드 ===
/// 1. 착수 시: OmokRule.IsValidMove() → BoardData.SetStone() → BoardRenderer.PlaceStoneObj()
/// 2. 승리 판정: OmokRule.CheckWin() == true → GameOver(color) 호출
/// 3. 턴 전환: SwitchTurn() 호출
/// 4. 현재 턴: CurrentTurn (Constants.StoneColor) → BoardRenderer.PlaceStoneObj(x, y, color)에 사용
/// </summary>
public enum GameState { Ready, Playing, GameOver }

public class GameLogic : MonoBehaviour
{
    // Singleton
    public static GameLogic Instance { get; private set; }

    // Inspector
    [Header("Game Settings")]
    [SerializeField] private float _maxTime = 30f;

    [Header("Current Status")]
    private Constants.StoneColor _currentTurn;
    private GameState _currentState;
    private float _currentTime;

    [Header("Events")]
    [SerializeField] private UnityEvent _onGameStarted;               // 게임 시작 (StartGame 호출 시)
    [SerializeField] private UnityEvent<float> _onTimerUpdate;
    [SerializeField] private UnityEvent<Constants.StoneColor> _onTurnChanged;
    [SerializeField] private UnityEvent<string> _onGameOver;          // 게임 종료 (승리 메시지)

    // 읽기 전용 공개 (보드 팀: CurrentTurn → PlaceStoneObj(x, y, color)에 사용)
    public Constants.StoneColor CurrentTurn => _currentTurn;
    public GameState CurrentState => _currentState;
    public float CurrentTime => _currentTime;

    // Unity Events
    private void Awake()
    {
        _onGameStarted ??= new UnityEvent();
        _onTimerUpdate ??= new UnityEvent<float>();
        _onTurnChanged ??= new UnityEvent<Constants.StoneColor>();
        _onGameOver ??= new UnityEvent<string>();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (_currentState == GameState.Playing)
            HandleTimer();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Public Methods (보드 팀에서 호출)
    /// <summary> 게임 시작 초기화 </summary>
    public void StartGame()
    {
        _currentState = GameState.Playing;
        _currentTurn = Constants.StoneColor.Black;
        _currentTime = _maxTime;

        _onGameStarted?.Invoke();
        _onTurnChanged?.Invoke(_currentTurn);
        _onTimerUpdate?.Invoke(Mathf.Clamp01(_currentTime / _maxTime));
    }

    /// <summary> 턴 넘기기 - 돌을 둔 뒤 호출 (BoardData.SetStone 후) </summary>
    public void SwitchTurn()
    {
        if (_currentState != GameState.Playing) return;

        _currentTime = _maxTime;
        _currentTurn = _currentTurn == Constants.StoneColor.Black ? Constants.StoneColor.White : Constants.StoneColor.Black;

        _onTurnChanged?.Invoke(_currentTurn);
    }

    /// <summary> 게임 오버 - OmokRule.CheckWin() true 시 또는 시간 초과 시 호출 </summary>
    /// <param name="winner">승리한 돌 색상 (Constants.StoneColor)</param>
    public void GameOver(Constants.StoneColor winner)
    {
        _currentState = GameState.GameOver;
        string winnerMsg = winner switch
        {
            Constants.StoneColor.Black => "Black Wins!",
            Constants.StoneColor.White => "White Wins!",
            _ => "Draw!"
        };

        _onGameOver?.Invoke(winnerMsg);
    }

    public void AddGameStartedListener(UnityAction callback) => _onGameStarted.AddListener(callback);
    public void RemoveGameStartedListener(UnityAction callback) => _onGameStarted.RemoveListener(callback);
    public void AddTimerUpdateListener(UnityAction<float> callback) => _onTimerUpdate.AddListener(callback);
    public void RemoveTimerUpdateListener(UnityAction<float> callback) => _onTimerUpdate.RemoveListener(callback);
    public void AddTurnChangedListener(UnityAction<Constants.StoneColor> callback) => _onTurnChanged.AddListener(callback);
    public void RemoveTurnChangedListener(UnityAction<Constants.StoneColor> callback) => _onTurnChanged.RemoveListener(callback);
    public void AddGameOverListener(UnityAction<string> callback) => _onGameOver.AddListener(callback);
    public void RemoveGameOverListener(UnityAction<string> callback) => _onGameOver.RemoveListener(callback);

    // Private Methods
    private void HandleTimer()
    {
        _currentTime -= Time.deltaTime;
        _onTimerUpdate?.Invoke(Mathf.Clamp01(_currentTime / _maxTime));

        if (_currentTime <= 0)
        {
            Constants.StoneColor winner = _currentTurn == Constants.StoneColor.Black ? Constants.StoneColor.White : Constants.StoneColor.Black;
            GameOver(winner);
        }
    }
}
