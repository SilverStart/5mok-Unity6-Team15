using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private GameObject blackStonePrefab;
    [SerializeField] private GameObject whiteStonePrefab;

    [SerializeField] private Transform startPoint;

    [SerializeField] private float cellSize = 0.4f;

    public void PlaceStoneObj(int x, int y, StoneColor color)
    {
        Vector3 spawnPosition = GetWorldPosition(x, y);

        GameObject stonePrefab = color == StoneColor.White ? whiteStonePrefab : blackStonePrefab;
        Instantiate(stonePrefab, spawnPosition, Quaternion.identity, transform);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        float worldX = startPoint.position.x + (cellSize * x);
        float worldY = startPoint.position.y - (cellSize * y);

        return new Vector3(worldX, worldY, 0);
    }

    public void ShowLastStoneMarker()
    {
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

public enum StoneColor
{
    Black,
    White,
}