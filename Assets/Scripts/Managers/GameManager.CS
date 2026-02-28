using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static common.Constants;

public enum GameState { Ready, Playing, GameOver }

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private GameObject gameTypePanelPrefab;
    [SerializeField] private GameObject matchResultPanelPrefab;

    private Canvas _canvas;

    // Game Logic
    private GameLogic _gameLogic;

    private BoardRenderer _boardRenderer;
    private InputController _inputController;
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
            _boardRenderer = FindFirstObjectByType<BoardRenderer>();
            _inputController = FindFirstObjectByType<InputController>();

            // 게임 타입 설정 팝업 창 띄우기
            var gameTypePanelObject = Instantiate(gameTypePanelPrefab, _canvas.transform);
            gameTypePanelObject.GetComponent<GameTypePanelController>().Show();
        }
    }

    public void OnGameStartClick(GameType gameType)
    {
        _gameType = gameType;
        _gameLogic = new GameLogic(_gameType);
        _gameLogic.OnStonePlaced += (x, y, color) => {
            // soundManager.PlaySFX("PlaceStoneSound");
            _boardRenderer.PlaceStoneObj(x, y, color);
        };
        _timerUIManager.OnTurnTimeOver += _gameLogic.TimeOver;
        _inputController.OnClick += _gameLogic.SetInputResult;
        // soundManager.PlayBGM("GameBGM");
        _inputController.SetRun(true);
    }

    public void GameOver(string resultStr)
    {
        // 게임 결과 팝업 창 띄우기
        _timerUIManager.StopTimer();
        _inputController.SetRun(false);
        var matchResultPanelObject = Instantiate(matchResultPanelPrefab, _canvas.transform);
        matchResultPanelObject.GetComponent<MatchResultUIManager>().Show(resultStr, RestartGame);
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