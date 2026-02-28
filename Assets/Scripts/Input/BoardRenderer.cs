using System.Collections.Generic;
using common;

using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    [SerializeField] private GameObject blackStonePrefab;
    [SerializeField] private GameObject whiteStonePrefab;
    [SerializeField] private GameObject markerXPrefab;

    [SerializeField] private GameObject lastStoneMarker;

    [SerializeField] private Transform startPoint;

    [SerializeField] private float cellSize = 0.4f;
    [SerializeField] private Transform positionSelector;

    private Dictionary<(int x, int y), GameObject> _placedStones = new();
    private Dictionary<(int x, int y), GameObject> _placedXs = new();

    public void PlaceStoneObj(int x, int y, Constants.StoneColor color)
    {
        if (_placedStones.ContainsKey((x, y)))
            return;

        HideAllMarkerX();
        Vector3 spawnPosition = GetWorldPosition(x, y);

        GameObject stonePrefab = color == Constants.StoneColor.White ? whiteStonePrefab : blackStonePrefab;
        var newStone = Instantiate(stonePrefab, spawnPosition, Quaternion.identity, transform);

        _placedStones[(x, y)] = newStone;

        ShowLastStoneMarker(x, y);
    }

    public void RemoveStoneObj((int x, int y) move, (int x, int y)? last = null)
    {
        if (_placedStones.TryGetValue(move, out var stone))
        {
            HideAllMarkerX();
            Destroy(stone);
            _placedStones.Remove(move);

            if (last.HasValue) ShowLastStoneMarker(last.Value.x, last.Value.y);
        }
    }

    public void ShowPositionToPlaceStoneMarker(int x, int y)
    {
        positionSelector.position = GetWorldPosition(x, y);
        positionSelector.gameObject.SetActive(true);
    }

    public void HidePositionToPlaceStoneMarker()
    {
        positionSelector.gameObject.SetActive(false);
    }

    public void PlaceMarkerX(int x, int y)
    {
        if (_placedXs.TryGetValue((x, y), out var markerX))
        {
            markerX.SetActive(true);
            return;
        }

        Vector3 spawnPosition = GetWorldPosition(x, y);
        var newMarkerX = Instantiate(markerXPrefab, spawnPosition, Quaternion.identity, transform);

        _placedXs[(x, y)] = newMarkerX;
    }

    private void HideAllMarkerX()
    {
        foreach (var markerX in _placedXs.Values)
        {
            markerX.SetActive(false);
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        float worldX = startPoint.position.x + (cellSize * x);
        float worldY = startPoint.position.y - (cellSize * y);

        return new Vector3(worldX, worldY, 0);
    }

    private void ShowLastStoneMarker(int x, int y)
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