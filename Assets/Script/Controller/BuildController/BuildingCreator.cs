#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

#endregion

public class BuildingCreator : Singleton<BuildingCreator>
{
    static readonly int CellSize = Shader.PropertyToID("_CellSize");
    [SerializeField] Tilemap previewMap, defaultMap, debugMap;

    [SerializeField] List<Tilemap> forbidPlacingWithMaps;
    [SerializeField] Renderer gridRenderer;
    [SerializeField] float cellSize;
    readonly Dictionary<Vector3Int, Node> _allNodes = new Dictionary<Vector3Int, Node>();
    BoundsInt _bounds;

    Camera _camera;
    Node _current;
    Vector3Int _currentGridPosition;

    bool _holdActive;
    Vector3Int _holdStartPosition;
    int _index;
    PlayerInput _input;
    Vector3Int _lastGridPosition;

    Vector2 _mousePos;

    HashSet<Node> _openNodes, _closedNodes;
    BuildingObjectBase _selectedObj;
    Vector3Int _startPos, _goalPos;

    TileBase _tileBase;
    TileType _tileType;

    public BuildingObjectBase SelectedObj
    {
        set
        {
            _selectedObj = value;

            _tileBase = _selectedObj != null ? _selectedObj.Tile : null;

            UpdatePreview();
        }
    }

    Tilemap Tilemap
    {
        get
        {
            if (_selectedObj != null && _selectedObj.Category != null && _selectedObj.Category.Tilemap != null)
                return _selectedObj.Category.Tilemap;

            return defaultMap;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _input = new PlayerInput();
        _camera = Camera.main;

    }

    void Start()
    {
        UpdateGridVisual();
        EnableGridVisual(false);
    }

    void Update()
    {
        if (_selectedObj != null)
        {
            var pos = _camera.ScreenToWorldPoint(_mousePos);
            var gridPos = previewMap.WorldToCell(pos);

            if (gridPos != _currentGridPosition)
            {
                _lastGridPosition = _currentGridPosition;
                _currentGridPosition = gridPos;

                UpdatePreview();

                if (_holdActive) HandleDrawing();
            }
        }
    }

    void OnEnable()
    {
        _input.Enable();
        _input.Player.MousePosition.performed += OnMouseMove;
        _input.Player.MouseLeftClick.performed += OnLeftClick;
        _input.Player.MouseLeftClick.started += OnLeftClick;
        _input.Player.MouseLeftClick.canceled += OnLeftClick;
        _input.Player.MouseRightClick.performed += OnRightClick;
        _input.Player.LoadPathDebug.performed += Algorithm;
    }

    void OnDisable()
    {
        _input.Disable();
        _input.Player.MousePosition.performed -= OnMouseMove;
        _input.Player.MouseLeftClick.performed -= OnLeftClick;
        _input.Player.MouseRightClick.performed -= OnRightClick;
        _input.Player.MouseLeftClick.started -= OnLeftClick;
        _input.Player.MouseLeftClick.canceled -= OnLeftClick;
        _input.Player.LoadPathDebug.performed -= Algorithm;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        UpdateGridVisual();
    }
#endif

    void Algorithm(InputAction.CallbackContext obj)
    {
        debugMap.ClearAllTiles();
        if (_current == null) Initialize();

        List<Node> neighbors = FindNeighbors(_current.Position);
        ExamineNeighbors(neighbors, _current);

        UpdateTiles(ref _current);

        AStarDebug.Instance.CreateTiles(_openNodes, _closedNodes, _allNodes, _startPos, _goalPos);
     }

    List<Node> FindNeighbors(Vector3Int parentPosition)
    {
        List<Node> neighbors = new List<Node>();

        for (var x = -1; x <= 1; x++)
        for (var y = -1; y <= 1; y++)
        {
            var neighborPos = new Vector3Int(parentPosition.x - x, parentPosition.y - y, parentPosition.z);
            if (y != 0 || x != 0)
                if (neighborPos != _startPos && Tilemap.GetTile(neighborPos))
                {
                    var neighbor = GetNode(neighborPos);
                    neighbors.Add(neighbor);
                }
        }

        return neighbors;
    }

    void ExamineNeighbors(List<Node> neighbors, Node current)
    {
        for (var i = 0; i < neighbors.Count; i++)
        {
            _openNodes.Add(neighbors[i]);

            int gScore = DetermineGScore(neighbors[i].Position, current.Position);
            
            CalculateValues(_current, neighbors[i], 0);
        }
    }

    void CalculateValues(Node parent, Node neighbor, int cost)
    {
        neighbor.Parent = parent;

        neighbor.G = parent.G + cost;

        neighbor.H = (Mathf.Abs(neighbor.Position.x - _goalPos.x) + Mathf.Abs(neighbor.Position.y - _goalPos.y)) * 10;

        neighbor.F = neighbor.G + neighbor.H;
    }

    private int DetermineGScore(Vector3Int neighbor, Vector3Int current)
    {
        int gScore = 0;

        int x = current.x - neighbor.x;
        int y = current.y - neighbor.y;

        if (Mathf.Abs(x - y) % 2 == 1)
        {
            gScore = 10;
        }
        else
        {
            gScore = 14;
        }

        return gScore;
    }

    void UpdateTiles(ref Node current)
    {
        _openNodes.Remove(current);
        _closedNodes.Add(current);
    }

    void Initialize()
    {
        _current = GetNode(_startPos);
        _openNodes = new HashSet<Node>();
        _closedNodes = new HashSet<Node>();
        _openNodes.Add(_current);
    }

    Node GetNode(Vector3Int position)
    {
        if (_allNodes.ContainsKey(position))
        {
            return _allNodes[position];
        }

        var node = new Node(position);
        _allNodes.Add(position, node);
        return node;
    }

    void OnMouseMove(InputAction.CallbackContext ctx)
    {
        _mousePos = ctx.ReadValue<Vector2>();
    }

    void OnLeftClick(InputAction.CallbackContext obj)
    {
        Debug.Log(obj.interaction + " / " + obj.phase);

        if (_selectedObj != null && !EventSystem.current.IsPointerOverGameObject())
        {
            if (obj.phase == InputActionPhase.Started)
            {
                _holdActive = true;

                if (obj.interaction is TapInteraction) _holdStartPosition = _currentGridPosition;

                HandleDrawing();
            }
            else
            {
                if (obj.interaction is SlowTapInteraction ||
                    (obj.interaction is TapInteraction && obj.phase == InputActionPhase.Performed))
                {
                    _holdActive = false;
                    HandleDrawRelease();
                }

            }
        }
    }

    void OnRightClick(InputAction.CallbackContext obj)
    {
        SelectedObj = null;
        EnableGridVisual(false);
    }

    public void ObjectSelected(BuildingObjectBase obj)
    {
        if (obj.IsWall)
        {
            SelectedObj = obj;
            var selelected = (AStarTileRule)_selectedObj.Tile;
            _tileType = selelected.Type;
        }
        else
        {
            SelectedObj = obj;
            var selected = (AStarTile)_selectedObj.Tile;
            _tileType = selected.Type;
        }


        EnableGridVisual(true);
    }

    void UpdatePreview()
    {
        previewMap.SetTile(_lastGridPosition, null);

        if (!IsForbidden(_currentGridPosition)) previewMap.SetTile(_currentGridPosition, _tileBase);

    }

    bool IsForbidden(Vector3Int pos)
    {
        if (_selectedObj == null) return false;

        List<BuildingCategory> restrictedCategories = _selectedObj.PlacementRestriction;
        List<Tilemap> restrictedMaps = restrictedCategories.ConvertAll(category => category.Tilemap);
        List<Tilemap> allMaps = forbidPlacingWithMaps.Concat(restrictedMaps).ToList();

        return allMaps.Any(map => { return map.HasTile(pos); });
    }

    void HandleDrawing()
    {
        if (_selectedObj != null)
            switch (_selectedObj.PlaceType)
            {
                case PlaceType.Line:
                    LineRenderer();
                    break;
                case PlaceType.Rectangle:
                    RectangleRenderer();
                    break;
            }

    }

    void HandleDrawRelease()
    {
        if (_selectedObj != null)
            switch (_selectedObj.PlaceType)
            {
                case PlaceType.Line:
                case PlaceType.Rectangle:
                    DrawBounds(Tilemap);
                    previewMap.ClearAllTiles();
                    break;
                case PlaceType.Single:
                default:
                    DrawItem(Tilemap, _currentGridPosition, _tileBase);
                    break;
            }
    }

    void RectangleRenderer()
    {
        previewMap.ClearAllTiles();

        _bounds.xMin = _currentGridPosition.x < _holdStartPosition.x ? _currentGridPosition.x : _holdStartPosition.x;
        _bounds.xMax = _currentGridPosition.x > _holdStartPosition.x ? _currentGridPosition.x : _holdStartPosition.x;
        _bounds.yMin = _currentGridPosition.y < _holdStartPosition.y ? _currentGridPosition.y : _holdStartPosition.y;
        _bounds.yMax = _currentGridPosition.y > _holdStartPosition.y ? _currentGridPosition.y : _holdStartPosition.y;

        DrawBounds(previewMap);
    }

    void LineRenderer()
    {
        previewMap.ClearAllTiles();

        float diffX = Mathf.Abs(_currentGridPosition.x - _holdStartPosition.x);
        float diffY = Mathf.Abs(_currentGridPosition.y - _holdStartPosition.y);

        var lineIsHorizontal = diffX >= diffY;

        if (lineIsHorizontal)
        {
            _bounds.xMin = _currentGridPosition.x < _holdStartPosition.x
                ? _currentGridPosition.x
                : _holdStartPosition.x;
            _bounds.xMax = _currentGridPosition.x > _holdStartPosition.x
                ? _currentGridPosition.x
                : _holdStartPosition.x;
            _bounds.yMin = _holdStartPosition.y;
            _bounds.yMax = _holdStartPosition.y;
        }
        else
        {
            _bounds.xMin = _holdStartPosition.x;
            _bounds.xMax = _holdStartPosition.x;
            _bounds.yMin = _currentGridPosition.y < _holdStartPosition.y
                ? _currentGridPosition.y
                : _holdStartPosition.y;
            _bounds.yMax = _currentGridPosition.y > _holdStartPosition.y
                ? _currentGridPosition.y
                : _holdStartPosition.y;
        }

        DrawBounds(previewMap);
    }

    void DrawBounds(Tilemap map)
    {
        for (var x = _bounds.xMin; x <= _bounds.xMax; x++)
        for (var y = _bounds.yMin; y <= _bounds.yMax; y++)
            DrawItem(map, new Vector3Int(x, y, 0), _tileBase);
    }

    void DrawItem(Tilemap map, Vector3Int position, TileBase tileBase)
    {

        if (map != previewMap && _selectedObj.GetType() == typeof(BuildingTool))
        {
            var tool = (BuildingTool)_selectedObj;
            tool.Use(position);
        }
        else if (!IsForbidden(position))
        {
            if (_tileType != TileType.Water)
            {
                var tile = (AStarTile)_tileBase;
                if (tile.gameObject != null)
                {
                    tile.color = Color.clear;
                    var itemSprite = tile.gameObject.GetComponent<SpriteRenderer>();
                    itemSprite.sortingOrder = _selectedObj.Category.SortingOrder;
                    tile.gameObject.name = "Turret " + _index;
                    _index++;
                }

                TileBase newTileBase = tile;
                map.SetTile(position, newTileBase);
                if (_tileType == TileType.Start)
                    _startPos = position;
                else if (_tileType == TileType.Goal) _goalPos = position;
            }
            else
            {
                var tile = (AStarTileRule)_tileBase;
                TileBase newTileBase = tile;
                map.SetTile(position, newTileBase);
            }
        }
    }


    void EnableGridVisual(bool on)
    {
        if (gridRenderer == null) return;
        gridRenderer.gameObject.SetActive(on);
    }

    void UpdateGridVisual()
    {
        if (gridRenderer == null) return;
        gridRenderer.sharedMaterial.SetVector(
            CellSize, new Vector4(cellSize, cellSize, 0, 0));
    }
}