using UnityEngine;
using UnityEngine.Events;

public enum PlayerTurn { Black, White, None }
public enum GameState { Ready, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float maxTime = 30f; // 턴 당 제한시간 (30초)

    [Header("Current Status")]
    public PlayerTurn currentTurn;
    public GameState currentState;
    public float currentTime;

    public UnityEvent<float> OnTimerUpdate; // 시간 갱신 알림
    public UnityEvent<PlayerTurn> OnTurnChanged; // 턴 변경 알림
    public UnityEvent<string> OnGameOver; // 게임 오버 알림

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            HandleTimer();
        }
    }

    // 게임 시작 초기화
    public void StartGame()
    {
        currentState = GameState.Playing;
        currentTurn = PlayerTurn.Black; // 흑돌 선
        currentTime = maxTime;
        
        OnTurnChanged?.Invoke(currentTurn);
        OnTimerUpdate?.Invoke(currentTime / maxTime);
    }

    // 타이머 로직
    void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        
        // UI 매니저에게 남은 시간 비율(0.0 ~ 1.0) 전달
        OnTimerUpdate?.Invoke(Mathf.Clamp01(currentTime / maxTime));

        if (currentTime <= 0)
        {
            // 시간 초과 처리 (상대방 승리 혹은 턴 넘기기 등 규칙에 따라)
            GameOver(currentTurn == PlayerTurn.Black ? PlayerTurn.White : PlayerTurn.Black);
        }
    }

    // 턴 넘기기 (돌을 두는 팀원이 이 함수를 호출해야 함)
    public void SwitchTurn()
    {
        if (currentState != GameState.Playing) return;

        currentTime = maxTime; // 시간 초기화
        currentTurn = (currentTurn == PlayerTurn.Black) ? PlayerTurn.White : PlayerTurn.Black;
        
        OnTurnChanged?.Invoke(currentTurn); // 턴 변경 알림
    }

    // 게임 오버 처리
    public void GameOver(PlayerTurn winner)
    {
        currentState = GameState.GameOver;
        string winnerMsg = (winner == PlayerTurn.Black) ? "Black Wins!" : "White Wins!";
        Debug.Log(winnerMsg);
        
        OnGameOver?.Invoke(winnerMsg);
    }
}