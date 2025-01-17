#region

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

#endregion

public class BuildingCreator : Singleton<BuildingCreator>
{
    #region Var

    static readonly int CellSize = Shader.PropertyToID("_CellSize");
    [SerializeField] Tilemap previewMap, defaultMap, debugMap;
    [SerializeField] TileBase goalTile, startTile, wallTile, grassTile, pathTileBase;
    [SerializeField] TextScroller interdictionText;
    [SerializeField] List<Tilemap> forbidPlacingWithMaps;
    [SerializeField] Renderer gridRenderer;
    [SerializeField] float cellSize;
    [SerializeField] BuildingObjectBase pathTile;
    Dictionary<Vector3Int, Node> _allNodes = new Dictionary<Vector3Int, Node>();
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
    HashSet<Vector3Int> _changedTile = new HashSet<Vector3Int>();
    Stack<Vector3Int> _path;
    Stack<Vector3Int> _pathOld;


    [SerializeField] List<Vector3Int> PathList = new List<Vector3Int>();


    List<Vector3Int> _waterTiles = new List<Vector3Int>();
    [SerializeField] List<Vector3Int> _wallTiles = new List<Vector3Int>();
    List<Vector3Int> _turretTiles = new List<Vector3Int>();
    BuildingObjectBase _selectedObj;
    Vector3Int _startPos, _goalPos;


    TileBase _tileBase;
    TileType _tileType;
    bool _startPosSet, _goalPosSet;
    [SerializeField] List<Tilemap> _mapsPathFinding = new List<Tilemap>();

    public List<Tilemap> MapsPathFinding => _mapsPathFinding;

    EventMaster _eventMaster;
    [SerializeField] int width;
    [SerializeField] int height;
    Vector3Int _holdPosition;
    [SerializeField] GameObject PausePanel;

    #endregion

    #region Setter

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


    public Stack<Vector3Int> PathOld
    {
        get => _pathOld;
    }

    public Dictionary<Vector3Int, Node> AllNodes
    {
        get => _allNodes;
    }

    public Node Current
    {
        get => _current;
    }

    public HashSet<Node> OpenNodes
    {
        get => _openNodes;
    }

    public HashSet<Node> ClosedNodes
    {
        get => _closedNodes;
    }

    public HashSet<Vector3Int> ChangedTile
    {
        get => _changedTile;
    }

    public List<Vector3Int> WaterTiles
    {
        get => _waterTiles;
    }

    public List<Vector3Int> WallTiles
    {
        get => _wallTiles;
    }

    public List<Vector3Int> TurretTiles
    {
        get => _turretTiles;
    }

    public Vector3Int StartPos
    {
        get => _startPos;
    }

    public Vector3Int GoalPos
    {
        get => _goalPos;
    }

    public Stack<Vector3Int> Path
    {
        get => _path;
    }

    #endregion

    #region UnityFunction

    protected override void Awake()
    {
        base.Awake();
        _input = new PlayerInput();
        _camera = Camera.main;
    }

    public void TilemapForPathFinding(Tilemap arg0)
    {
        _mapsPathFinding.Add(arg0);
    }

    void Start()
    {
        _eventMaster = EventMaster.GetInstance();
        UpdateGridVisual();
        EnableGridVisual(false);
        TilemapForPathFinding(defaultMap);
        _eventMaster.CreateNewEvent("StartPath");
        _eventMaster.CreateNewEvent("ReloadPath");
        _eventMaster.GetEvent("StartPath").AddListener(GetGoalAndStart);
        _eventMaster.GetEvent("ReloadPath").AddListener(Reset);
        PlaceWallLineWithRandomHole();
    }

    void PlaceWallLineWithRandomHole()
    {
        int lineLength = 9; // Length of the wall line
        int randomHoleIndex = Random.Range(0, lineLength); // Random index for the hole
        int randomHoleIndex2 = Random.Range(0, lineLength); // Random index for the hole

        for (int i = -8; i < lineLength; i++)
        {
            if (i != randomHoleIndex)
            {
                Vector3Int position = new Vector3Int(-17, i, 0); // Adjust the position as needed
                defaultMap.SetTile(position, wallTile);
                _wallTiles.Add(position);
            }
        }

        for (int i = -8; i < lineLength; i++)
        {
            if (i != randomHoleIndex2)
            {
                Vector3Int newPos = new Vector3Int(17, i, 0); // Adjust the position as needed
                defaultMap.SetTile(newPos, wallTile);
                _wallTiles.Add(newPos);
            }
        }
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

            if (!_selectedObj.IsWall && _selectedObj.CategoryType1 != Category.Tool)
            {
                Tile turretTile = (Tile)_selectedObj.Tile;
                GameObject turretGameObject = turretTile.gameObject;
                turretGameObject.gameObject.GetComponent<Turret>().enabled = false;
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
        _input.Player.DebugModeGame.performed += DebugMod;
        _input.Player.AddMoney.performed += SetCoin;
        _input.Player.Pause.performed += Pause;
        // _input.Player.LoadPathDebug.performed += Reset;
        _input.Player.DebugMode.performed += ShowAndHideDebugMode;
    }

    void OnDisable()
    {
        _input.Disable();
        _input.Player.MousePosition.performed -= OnMouseMove;
        _input.Player.MouseLeftClick.performed -= OnLeftClick;
        _input.Player.MouseRightClick.performed -= OnRightClick;
        _input.Player.MouseLeftClick.started -= OnLeftClick;
        _input.Player.MouseLeftClick.canceled -= OnLeftClick;
        _input.Player.DebugModeGame.canceled -= DebugMod;
        _input.Player.AddMoney.canceled -= SetCoin;
        _input.Player.Pause.canceled -= Pause;
        // _input.Player.LoadPathDebug.performed -= Reset;
        _input.Player.DebugMode.performed += ShowAndHideDebugMode;
        // _pathFinding.RemoveListener(Algorithm);
    }
#if UNITY_EDITOR
    void OnValidate()
    {
        UpdateGridVisual();
    }
#endif


    public void Pause(InputAction.CallbackContext ctx)
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            PausePanel.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            PausePanel.SetActive(true);
        }

    }


    public void LoadData(List<Vector3Int> wallTiles, List<Vector3Int> waterTiles, List<Vector3Int> turretTiles)
    {
        _wallTiles = wallTiles;
        _waterTiles = waterTiles;
        _turretTiles = turretTiles;
    }

    #endregion

    #region A*

    // InputAction.CallbackContext obj
    public void Algorithm()
    {
        if (_current == null) Initialize();

        while (_openNodes.Count > 0 && _path == null)
        {
            List<Node> neighbors = FindNeighbors(_current.Position);
            ExamineNeighbors(neighbors, _current);
            UpdateTiles(ref _current);

            _path = GeneratePath(_current);
        }

        if (_path != null)
        {
            foreach (Vector3Int pos in _path)
            {
                if (pos != _goalPos)
                {
                    pathTile.Category.Tilemap.SetTile(pos, pathTile.Tile);
                }
            }
        }
        else
        {
            _path = _pathOld;
            foreach (Vector3Int pos in _path)
            {
                if (pos != _goalPos)
                {
                    pathTile.Category.Tilemap.SetTile(pos, pathTile.Tile);
                }
            }

            _selectedObj.Category.Tilemap.SetTile(_holdPosition, null);
            _wallTiles.Remove(_holdPosition);
            Debug.Log("You can't Place Her");
        }

        AStarDebug.Instance.CreateTiles(_openNodes, _closedNodes, _allNodes, _startPos, _goalPos, _path);
    }

    void Initialize()
    {
        _current = GetNode(_startPos);
        _openNodes = new HashSet<Node>();
        _closedNodes = new HashSet<Node>();
        _openNodes.Add(_current);
    }

    void ShowAndHideDebugMode(InputAction.CallbackContext obj)
    {
        AStarDebug.Instance.ShowDebugMode();
    }

    List<Node> FindNeighbors(Vector3Int parentPosition)
    {
        List<Node> neighbors = new List<Node>();

        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0), // Right
            new Vector3Int(-1, 0, 0), // Left
            new Vector3Int(0, 1, 0), // Up
            new Vector3Int(0, -1, 0) // Down
        };

        foreach (var direction in directions)
        {
            var neighborPos = parentPosition + direction;
            if (neighborPos != _startPos && TilemapForPath(neighborPos) && !_wallTiles.Contains(neighborPos) &&
                !_turretTiles.Contains(neighborPos))
            {
                var neighbor = GetNode(neighborPos);
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    TileBase TilemapForPath(Vector3Int pos)
    {
        foreach (Tilemap tilemap in _mapsPathFinding)
        {
            return tilemap.GetTile(pos);
        }

        return null;
    }

    void ClearTilemapFromPos(Vector3Int pos)
    {
        foreach (Tilemap tilemap in _mapsPathFinding)
        {
            if (tilemap.GetTile(pos))
            {
                tilemap.SetTile(pos, null);
            }
        }
    }

    void ExamineNeighbors(List<Node> neighbors, Node current)
    {
        for (var i = 0; i < neighbors.Count; i++)
        {

            var node = neighbors[i];

            if (!ConnectedDiagonally(current, node))
            {
                continue;
            }

            var gScore = DetermineGScore(neighbors[i].Position, current.Position);

            if (_openNodes.Contains(node))
            {
                if (current.G + gScore < node.G) CalculateValues(current, node, gScore);
            }
            else if (!_closedNodes.Contains(node))
            {
                CalculateValues(current, node, gScore);

                _openNodes.Add(node);
            }
        }
    }

    void UpdateTiles(ref Node current)
    {
        _openNodes.Remove(current);
        _closedNodes.Add(current);

        if (_openNodes.Count > 0) current = _openNodes.OrderBy(x => x.F).First();
    }

    Stack<Vector3Int> GeneratePath(Node current)
    {
        if (current.Position == _goalPos)
        {
            Stack<Vector3Int> finalPath = new Stack<Vector3Int>();
            while (current.Position != _startPos)
            {
                finalPath.Push(current.Position);

                current = current.Parent;
            }

            return finalPath;
        }

        return null;
    }

    bool ConnectedDiagonally(Node currentNode, Node neighborNode)
    {
        Vector3Int direction = currentNode.Position - neighborNode.Position;

        Vector3Int first = new Vector3Int(_current.Position.x + (direction.x * -1), _current.Position.y,
            _current.Position.z);
        Vector3Int second = new Vector3Int(_current.Position.x, _current.Position.y + (direction.y * -1),
            _current.Position.z);

        if (_waterTiles.Contains(first) || _waterTiles.Contains(second))
        {
            return false;
        }

        return true;
    }

    void CalculateValues(Node parent, Node neighbor, int cost)
    {
        neighbor.Parent = parent;

        neighbor.G = parent.G + cost;

        neighbor.H = (Mathf.Abs(neighbor.Position.x - _goalPos.x) + Mathf.Abs(neighbor.Position.y - _goalPos.y)) * 10;

        neighbor.F = neighbor.G + neighbor.H;
    }

    int DetermineGScore(Vector3Int neighbor, Vector3Int current)
    {
        var gScore = 0;

        var x = current.x - neighbor.x;
        var y = current.y - neighbor.y;

        if (Mathf.Abs(x - y) % 2 == 1)
            gScore = 10;
        else
            gScore = 14;

        return gScore;
    }

    Node GetNode(Vector3Int position)
    {
        if (_allNodes != null && _allNodes.ContainsKey(position)) return _allNodes[position];

        var node = new Node(position);
        _allNodes.Add(position, node);
        return node;
    }

    #endregion

    #region MouseInteraction

    void OnMouseMove(InputAction.CallbackContext ctx)
    {
        _mousePos = ctx.ReadValue<Vector2>();
    }

    void OnLeftClick(InputAction.CallbackContext obj)
    {

        if (WaveManager.GetInstance().WaveStarted)
        {
            return;
        }

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
        if (WaveManager.GetInstance().WaveStarted)
        {
            return;
        }


        SelectedObj = obj;
        if (_selectedObj.IsWall)
        {
            var selected = (AStarTileRule)_selectedObj.Tile;
            _tileType = selected.Type;
        }
        else
        {
            var selected = (AStarTile)_selectedObj.Tile;
            _tileType = selected.Type;
        }


        EnableGridVisual(true);
    }

    #endregion

    #region DrawItem

    void GetGoalAndStart()
    {
        foreach (Vector3Int pos in defaultMap.cellBounds.allPositionsWithin)
        {
            if (defaultMap.GetTile(pos) == wallTile)
            {
                _wallTiles.Add(pos);
            }
        }

        foreach (var pos in defaultMap.cellBounds.allPositionsWithin)
        {
            if (defaultMap.GetTile(pos) == goalTile)
            {
                _goalPos = pos;
                _goalPosSet = true;
            }
            else if (defaultMap.GetTile(pos) == startTile)
            {
                _startPos = pos;
                _startPosSet = true;
            }
        }

        if (_startPosSet && _goalPosSet)
        {
            Algorithm();
        }
    }


    void UpdatePreview()
    {
        previewMap.SetTile(_lastGridPosition, null);

        if (!IsForbidden(_currentGridPosition))
        {
            previewMap.SetTile(_currentGridPosition, _tileBase);
        }

    }

    bool IsForbidden(Vector3Int pos)
    {
        if (_selectedObj == null) return false;

        List<BuildingCategory> restrictedCategories = _selectedObj.PlacementRestriction;
        List<Tilemap> restrictedMaps = restrictedCategories.ConvertAll(category => category.Tilemap);
        List<Tilemap> allMaps = forbidPlacingWithMaps.Concat(restrictedMaps).ToList();

        foreach (Tilemap map in allMaps)
        {
            if (map.HasTile(pos))
            {
                if (map.GetTile(pos) == pathTile.Tile)
                {
                    return false;
                }

                return true;
            }
        }

        return false;
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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="position"></param>
    /// <param name="tileBase"></param>
    void DrawItem(Tilemap map, Vector3Int position, TileBase tileBase)
    {
        _holdPosition = position;
        if (MoneyManager.GetInstance().GetMoneyCount() < _selectedObj.BananaCost)
        {
            interdictionText.LaunchTextScroll();
            SelectedObj = null;
            EnableGridVisual(false);
            return;
        }

        if (_selectedObj.GetType() == typeof(BuildingTool))
        {
            var tool = (BuildingTool)_selectedObj;
            tool.Use(position);
            if (_wallTiles.Contains(position))
            {
                _wallTiles.Remove(position);
            }

            if (_turretTiles.Contains(position))
            {
                _turretTiles.Remove(position);
            }
            
        }
        else if (!IsForbidden(position))
        {
            if (_tileType != TileType.Wall)
            {
                var tile = (AStarTile)_tileBase;
                if (tile.gameObject != null)
                {
                    tile.color = Color.clear;
                    var itemSprite = tile.gameObject.GetComponent<SpriteRenderer>();
                    itemSprite.sortingOrder = _selectedObj.Category.SortingOrder;
                    tile.gameObject.name = "Turret " + _index;
                    _index++;
                    _turretTiles.Add(position);
                    tile.gameObject.GetComponent<Turret>().enabled = true;
                    BuildingHUD.GetInstance().Turrets1.Add(tile.gameObject.GetComponent<Turret>());
                }

                TileBase newTileBase = tile;
                map.SetTile(position, newTileBase);
            }
            else
            {
                var tile = (AStarTileRule)_tileBase;
                TileBase newTileBase = tile;
                _wallTiles.Add(position);
                map.SetTile(position, newTileBase);
            }

            _eventMaster.InvokeEventInt("RemoveMoney", _selectedObj.BananaCost);
        }

        _eventMaster.InvokeEvent("ReloadPath");
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

    #endregion

    #region DebugMode

    public void DebugMod(InputAction.CallbackContext ctx)
    {
        EventMaster.GetInstance().InvokeEvent("DebugMode");
    }

    public void SetCoin(InputAction.CallbackContext ctx)
    {
        EventMaster.GetInstance().InvokeEventInt("AddMoney", 1000);
    }

    #endregion


    public void Reset()
    {
        AStarDebug.Instance.Reset();
        Tilemap map = _mapsPathFinding.FirstOrDefault(t => t.name == "Tilemap_Map");

        foreach (Tilemap tilemap in _mapsPathFinding)
        {
            if (tilemap != null && map != null && _path != null)
            {
                foreach (Vector3Int nodesKey in _allNodes.Keys)
                {
                    if (tilemap.GetTile(nodesKey) != grassTile && tilemap.GetTile(nodesKey) != null)
                    {
                        map.SetTile(nodesKey, null);
                    }
                }

                foreach (Vector3Int tilePos in _path)
                {
                    if (tilemap.GetTile(tilePos) != grassTile && tilemap.GetTile(tilePos) != null)
                    {
                        map.SetTile(tilePos, null);
                    }
                }
            }
        }

        _pathOld = _path;
        _allNodes.Clear();
        _path = null;
        _current = null;
        Algorithm();
    }
}