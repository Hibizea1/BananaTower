#region

using System.Collections.Generic;
using Unity.VisualScripting;
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


    List<GameObject> _debugObjects = new List<GameObject>();

    public static AStarDebug Instance
    {
        get
        {
            if (_instance == null) _instance = FindAnyObjectByType<AStarDebug>();

            return _instance;
        }
    }

    public void CreateTiles(HashSet<Node> openList, HashSet<Node> closedList, Dictionary<Vector3Int, Node> allNodes,
        Vector3Int start, Vector3Int goal)
    {
        foreach (var node in openList) ColorTile(node.Position, openColor);

        foreach (Node node in closedList)
        {
            ColorTile(node.Position, closedColor);
        }

        print("Called");
        ColorTile(start, startColor);
        ColorTile(goal, goalColor);

        foreach (KeyValuePair<Vector3Int, Node> node in allNodes)
        {
            if (node.Value.Parent != null)
            {
                GameObject debugArrow = Instantiate(debugTextPrefab, canvas.transform);
                debugArrow.transform.position = grid.CellToWorld(node.Key);
                _debugObjects.Add(debugArrow);
                GenerateDebugText(node.Value, debugArrow.GetComponent<DebugText>());
            }
        }
    }

    void GenerateDebugText(Node node, DebugText debugText)
    {
        debugText.P.text = $"P:{node.Position.x},{node.Position.y}";
        debugText.F.text = $"F:{node.F}";
        debugText.G.text = $"G:{node.G}";
        debugText.H.text = $"H:{node.H}";
        
        Vector3Int diretion = node.Parent.Position - node.Position;
        debugText.Arrow.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(diretion.y, diretion.x) * Mathf.Rad2Deg);
    }

    public void ColorTile(Vector3Int position, Color color)
    {
        tilemap.SetTile(position, tile);
        tilemap.SetTileFlags(position, TileFlags.None);
        tilemap.SetColor(position, color);
        print("Tile as been set at : " + position);
    }
}