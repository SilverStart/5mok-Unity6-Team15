using common;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField] private BoardRenderer boardRenderer;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
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
            // TODO :: 테스트 용으로 직접 놔두는 것
            // 나중에는 GameManager 쪽으로 입력 올리기
            boardRenderer.PlaceStoneObj(x, y, Constants.StoneColor.White);
        }
    }
}