using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SaveHandler : Singleton<SaveHandler>
{
    readonly Dictionary<string, Tilemap> _tilemaps = new Dictionary<string, Tilemap>();
    [SerializeField] BoundsInt bounds;
    [SerializeField] string fileName = "TilemapData.JSON";

    void Start()
    {
        initTilemap();
    }

    void initTilemap()
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach (var map in maps)
        {
            _tilemaps.Add(map.name, map);
        }
    }

    public void OnSave()
    {
        List<TilemapData> data = new List<TilemapData>();

        foreach (var mapObj in _tilemaps)
        {
            TilemapData mapData = new TilemapData();
            mapData.Key = mapObj.Key;

            BoundsInt boundsForThisMap = mapObj.Value.cellBounds;

            for (int x = boundsForThisMap.xMin; x < boundsForThisMap.xMax; x++)
            {
                for (int y = boundsForThisMap.yMin; y < boundsForThisMap.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase tile = mapObj.Value.GetTile(pos);

                    if (tile != null)
                    {
                        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tile, out string guid, out long localId))
                        {
                            TileInfo ti = new TileInfo(tile, pos, guid);
                            mapData.Tiles.Add(ti);
                        }
                        else
                        {
                            Debug.LogError("Could not get GUID for tile " + tile.name);
                        }
                    }
                }
            }

            data.Add(mapData);
        }

        FileHandler.SaveToJSON<TilemapData>(data, fileName);
    }

    public void OnLoad()
    {
        List<TilemapData> data = FileHandler.ReadListFromJSON<TilemapData>(fileName);

        foreach (var mapData in data)
        {
            if (!_tilemaps.ContainsKey(mapData.Key))
            {
                Debug.LogError("Found saved data for tilemap called '" + mapData.Key +
                               "', but tilemaps does not exist. Skip");
                continue;
            }

            var map = _tilemaps[mapData.Key];

            map.ClearAllTiles();
            if (mapData.Tiles != null && mapData.Tiles.Count > 0)
            {
                foreach (TileInfo tile in mapData.Tiles)
                {
                    TileBase tileBase = tile.Tile;
                    if (tileBase == null)
                    {
                        Debug.Log("[Loading Tilemap]: InstanceID not found - looking in AssetDatabase");
                        string path = AssetDatabase.GUIDToAssetPath(tile.GuidFromAssetDB);
                        tileBase = AssetDatabase.LoadAssetAtPath<TileBase>(path);

                        if (tileBase == null)
                        {
                            Debug.LogError("[Loading Tilemap]: Tile not found in AssetDatabase");
                            continue;
                        }
                    }

                    map.SetTile(tile.Position, tileBase);
                }
            }
        }
    }
}

[Serializable]
public class TilemapData
{
    public string Key;
    public List<TileInfo> Tiles = new List<TileInfo>();
}

[Serializable]
public class TileInfo
{
    public TileBase Tile;
    public string GuidFromAssetDB;
    public Vector3Int Position;

    public TileInfo(TileBase tile, Vector3Int pos, string guid)
    {
        this.Tile = tile;
        Position = pos;
        GuidFromAssetDB = guid;
    }
}