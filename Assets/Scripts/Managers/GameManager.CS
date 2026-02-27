using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static common.Constants;

public enum GameState { Ready, Playing, GameOver }

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private BoardRenderer boardRenderer;
    [SerializeField] private InputController inputController;

    private Canvas _canvas;

    // Game Logic
    private GameLogic _gameLogic;

    private TimerUIManager _timerUIManager;

    // 게임의 종류
    private GameType _gameType;

    protected override void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == (int)SceneName.InGame)
        {

            // 새로운 씬에서 Canvas 참조 가져오기
            _canvas = FindFirstObjectByType<Canvas>();

            _timerUIManager = FindFirstObjectByType<TimerUIManager>();

            // TODO: 게임 타입 설정 팝업 창 띄우기
        }
    }

    public void OnGameStartClick(GameType gameType)
    {
        _gameType = gameType;
        _gameLogic = new GameLogic(_gameType);
        _gameLogic.OnStonePlaced += (x, y, color) => {
            soundManager.PlaySFX("PlaceStoneSound");
            boardRenderer.PlaceStoneObj(x, y, color);
        };
        _timerUIManager.OnTurnTimeOver += _gameLogic.TimeOver;
        inputController.OnClick += _gameLogic.SetInputResult;
        soundManager.PlayBGM("GameBGM");
    }

    public void GameOver(string resultStr)
    {
        // TODO: 게임 결과 팝업 창 띄우기
        // GameManager.Instance.OpenConfirmPanel(resultStr, () =>
        // {
        //     GameManager.Instance.RestartGame();
        // });
    }

    public void RestartGame()
    {
        SceneManager.LoadScene((int)SceneName.InGame);
    }

    // TimerUI 업데이트
    public void SetGameTurn(StoneColor color)
    {
        _timerUIManager.ChangeTurn(color);
    }

    public void ChangeToLoginScene()
    {
        _gameLogic?.Dispose();
        _gameLogic = null;
        SceneManager.LoadScene((int)SceneName.Login);
    }
}