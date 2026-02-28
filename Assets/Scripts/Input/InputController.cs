using System;
using common;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField] private BoardRenderer boardRenderer;

    // 입력 처리 액션; 이름은 추후 수정 필요
    public Action<PlayerInput> OnClick;

    private Camera mainCamera;
    private int X, Y;
    private bool isStoneReady = false;
    private bool isRunning = false;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isRunning) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            DetectTouch();
        }
    }

    public void DetectTouch()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        float floatX = (mousePosition.x - boardRenderer.GetStartPoint().position.x) / boardRenderer.GetCellSize();
        float floatY = (boardRenderer.GetStartPoint().position.y - mousePosition.y) / boardRenderer.GetCellSize();

        int x = Mathf.RoundToInt(floatX);
        int y = Mathf.RoundToInt(floatY);

        if (0 <= x && x < 15 && 0 <= y && y < 15)
        {
            // Valid Position!
            X = x;
            Y = y;
            boardRenderer.ShowPositionToPlaceStoneMarker(x, y);
            isStoneReady = true;
            // TODO: ConfirmButton.SetActive(true);
        }
    }

    public void SetRun(bool isRunning)
    {
        this.isRunning = isRunning;
    }

    public void OnUndoButtoneClick()
    {
        if (!isRunning) return;
        OnClick?.Invoke(PlayerInput.Undo());
    }

    public void OnConfirmButtonClick()
    {
        if (!isRunning) return;
        if (!isStoneReady) return;
        OnClick?.Invoke(PlayerInput.Move(X, Y));
        isStoneReady = false;
    }

    public void OnResignButtonClick()
    {
        if (!isRunning) return;
        OnClick?.Invoke(PlayerInput.Resign());
    }
}