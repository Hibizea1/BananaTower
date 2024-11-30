#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#endregion

public class AStarDebug : MonoBehaviour
{
    static AStarDebug _instance;

    [SerializeField] Grid grid;
    [SerializeField] Canvas canvas;
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile tile;
    [SerializeField] Color openColor, closedColor, pathColor, currentColor, startColor, goalColor;
    [SerializeField] GameObject debugTextPrefab;


    readonly List<GameObject> _debugObjects = new List<GameObject>();

    public static AStarDebug Instance
    {
        get
        {
            if (_instance == null) _instance = FindAnyObjectByType<AStarDebug>();

            return _instance;
        }
    }

    public void CreateTiles(HashSet<Node> openList, HashSet<Node> closedList, Dictionary<Vector3Int, Node> allNodes,
        Vector3Int start, Vector3Int goal, Stack<Vector3Int> path = null)
    {
        foreach (var node in openList) ColorTile(node.Position, openColor);

        foreach (var node in closedList) ColorTile(node.Position, closedColor);

        if (path != null)
            foreach (var pos in path)
                if (pos != start && pos != goal)
                    ColorTile(pos, pathColor);

        print("Called");
        ColorTile(start, startColor);
        ColorTile(goal, goalColor);

        foreach (KeyValuePair<Vector3Int, Node> node in allNodes)
            if (node.Value.Parent != null)
            {
                var debugArrow = Instantiate(debugTextPrefab, canvas.transform);
                debugArrow.transform.position = grid.CellToWorld(node.Key);
                _debugObjects.Add(debugArrow);
                GenerateDebugText(node.Value, debugArrow.GetComponent<DebugText>());
            }
    }

    void GenerateDebugText(Node node, DebugText debugText)
    {
        debugText.P.text = $"P:{node.Position.x},{node.Position.y}";
        debugText.F.text = $"F:{node.F}";
        debugText.G.text = $"G:{node.G}";
        debugText.H.text = $"H:{node.H}";

        var diretion = node.Parent.Position - node.Position;
        debugText.Arrow.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(diretion.y, diretion.x) * Mathf.Rad2Deg);
    }

    public void ColorTile(Vector3Int position, Color color)
    {
        tilemap.SetTile(position, tile);
        tilemap.SetTileFlags(position, TileFlags.None);
        tilemap.SetColor(position, color);
        print("Tile as been set at : " + position);
    }

    public void ShowDebugMode()
    {
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
        Color c = tilemap.color;
        c.a = c.a != 0 ? 0 : 1;
        tilemap.color = c;
    }


    public void Reset()
    {
        foreach (GameObject debugObject in _debugObjects)
        {
            Destroy(debugObject);
        }
    }
}