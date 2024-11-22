using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

public class BuildingCreator : Singleton<BuildingCreator>
{
    [SerializeField] Tilemap previewMap, defaultMap;
    PlayerInput _input;

    [SerializeField] List<Tilemap> forbidPlacingWithMaps;

    TileBase _tileBase;
    BuildingObjectBase _selectedObj;

    Camera _camera;

    Vector2 _mousePos;
    Vector3Int _currentGridPosition;
    Vector3Int _lastGridPosition;

    bool _holdActive;
    Vector3Int _holdStartPosition;
    BoundsInt _bounds;

    public BuildingObjectBase SelectedObj
    {
        set
        {
            _selectedObj = value;

            _tileBase = _selectedObj != null ? _selectedObj.Tile : null;

            UpdatePreview();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _input = new PlayerInput();
        _camera = Camera.main;

    }

    void OnEnable()
    {
        _input.Enable();
        _input.Player.MousePosition.performed += OnMouseMove;
        _input.Player.MouseLeftClick.performed += OnLeftClick;
        _input.Player.MouseLeftClick.started += OnLeftClick;
        _input.Player.MouseLeftClick.canceled += OnLeftClick;
        _input.Player.MouseRightClick.performed += OnRightClick;
    }


    void OnDisable()
    {
        _input.Disable();
        _input.Player.MousePosition.performed -= OnMouseMove;
        _input.Player.MouseLeftClick.performed -= OnLeftClick;
        _input.Player.MouseRightClick.performed -= OnRightClick;
        _input.Player.MouseLeftClick.started -= OnLeftClick;
        _input.Player.MouseLeftClick.canceled -= OnLeftClick;
    }

    void Update()
    {
        if (_selectedObj != null)
        {
            Vector3 pos = _camera.ScreenToWorldPoint(_mousePos);
            Vector3Int gridPos = previewMap.WorldToCell(pos);

            if (gridPos != _currentGridPosition)
            {
                _lastGridPosition = _currentGridPosition;
                _currentGridPosition = gridPos;

                UpdatePreview();

                if (_holdActive)
                {
                    HandleDrawing();
                }
            }
        }
    }

    private void OnMouseMove(InputAction.CallbackContext ctx)
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

                if (obj.interaction is TapInteraction)
                {
                    _holdStartPosition = _currentGridPosition;
                }

                HandleDrawing();
            }
            else
            {
                if (obj.interaction is SlowTapInteraction ||
                    obj.interaction is TapInteraction && obj.phase == InputActionPhase.Performed)
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
    }

    public void ObjectSelected(BuildingObjectBase obj)
    {
        SelectedObj = obj;
    }

    private void UpdatePreview()
    {
        previewMap.SetTile(_lastGridPosition, null);

        if (!IsForbidden(_currentGridPosition))
        {
            previewMap.SetTile(_currentGridPosition, _tileBase);
        }
    }

    private bool IsForbidden(Vector3Int pos)
    {
        if (_selectedObj == null)
        {
            return false;
        }
        else
        {
            List<BuildingCategory> restrictedCategories = _selectedObj.PlacementRestriction;
            List<Tilemap> restrictedMaps = restrictedCategories.ConvertAll(category => category.Tilemap);
            List<Tilemap> allMaps = forbidPlacingWithMaps.Concat(restrictedMaps).ToList();

            return allMaps.Any(map => { return map.HasTile(pos); });
        }
    }

    Tilemap Tilemap
    {
        get
        {
            if (_selectedObj != null && _selectedObj.Category != null && _selectedObj.Category.Tilemap != null)
            {
                return _selectedObj.Category.Tilemap;
            }

            return defaultMap;
        }
    }

    private void HandleDrawing()
    {
        if (_selectedObj != null)
        {
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

    }

    void HandleDrawRelease()
    {
        if (_selectedObj != null)
        {
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
    }

    private void RectangleRenderer()
    {
        previewMap.ClearAllTiles();

        _bounds.xMin = _currentGridPosition.x < _holdStartPosition.x ? _currentGridPosition.x : _holdStartPosition.x;
        _bounds.xMax = _currentGridPosition.x > _holdStartPosition.x ? _currentGridPosition.x : _holdStartPosition.x;
        _bounds.yMin = _currentGridPosition.y < _holdStartPosition.y ? _currentGridPosition.y : _holdStartPosition.y;
        _bounds.yMax = _currentGridPosition.y > _holdStartPosition.y ? _currentGridPosition.y : _holdStartPosition.y;

        DrawBounds(previewMap);
    }

    private void LineRenderer()
    {
        previewMap.ClearAllTiles();

        float diffX = Mathf.Abs(_currentGridPosition.x - _holdStartPosition.x);
        float diffY = Mathf.Abs(_currentGridPosition.y - _holdStartPosition.y);

        bool lineIsHorizontal = diffX >= diffY;

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
        for (int x = _bounds.xMin; x <= _bounds.xMax; x++)
        {
            for (int y = _bounds.yMin; y <= _bounds.yMax; y++)
            {
                DrawItem(map, new Vector3Int(x, y, 0), _tileBase);
            }
        }
    }

    void DrawItem(Tilemap map, Vector3Int position, TileBase tileBase)
    {

        if (map != previewMap && _selectedObj.GetType() == typeof(BuildingTool))
        {
            BuildingTool tool = (BuildingTool)_selectedObj;
            tool.Use(position);
        }
        else if (!IsForbidden(position))
        {
            map.SetTile(position, _tileBase);
        }
    }
}