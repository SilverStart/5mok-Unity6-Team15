using common;

using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private GameObject blackStonePrefab;
    [SerializeField] private GameObject whiteStonePrefab;

    [SerializeField] private GameObject lastStoneMarker;

    [SerializeField] private Transform startPoint;

    [SerializeField] private float cellSize = 0.4f;
    [SerializeField] private Transform positionSelector;

    public void ShowPositionToPlaceStoneMarker(int x, int y)
    {
        positionSelector.position = GetWorldPosition(x, y);
        positionSelector.gameObject.SetActive(true);
    }

    public void HidePositionToPlaceStoneMarker()
    {
        positionSelector.gameObject.SetActive(false);
    }

    public void PlaceStoneObj(int x, int y, Constants.StoneColor color)
    {
        Vector3 spawnPosition = GetWorldPosition(x, y);

        GameObject stonePrefab = color == Constants.StoneColor.White ? whiteStonePrefab : blackStonePrefab;
        Instantiate(stonePrefab, spawnPosition, Quaternion.identity, transform);

        ShowLastStoneMarker(x, y);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        float worldX = startPoint.position.x + (cellSize * x);
        float worldY = startPoint.position.y - (cellSize * y);

        return new Vector3(worldX, worldY, 0);
    }

    void ShowLastStoneMarker(int x, int y)
    {
        if (!lastStoneMarker.activeSelf) lastStoneMarker.SetActive(true);

        lastStoneMarker.transform.position = GetWorldPosition(x, y);
    }

    public Transform GetStartPoint()
    {
        return startPoint;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
}