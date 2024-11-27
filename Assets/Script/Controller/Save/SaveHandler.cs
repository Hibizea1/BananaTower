#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#endregion

public class SaveHandler : Singleton<SaveHandler>
{
    [SerializeField] BoundsInt bounds;
    [SerializeField] string fileName = "tilemapdata.JSON";
    readonly Dictionary<string, Tilemap> _tilemaps = new Dictionary<string, Tilemap>();

    void Start()
    {
        InitTilemap();
    }

    void InitTilemap()
    {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach (var map in maps) _tilemaps.Add(map.name, map);
    }

    // void InitTurret()
    // {
    //     List<Turret> turrets = FindObjectsByType(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID );
    //     
    // }

    public void OnSave()
    {
        List<TilemapData> data = new List<TilemapData>();

        foreach (KeyValuePair<string, Tilemap> mapObj in _tilemaps)
        {
            var mapData = new TilemapData();
            mapData = TilemapSave(mapObj);

            data.Add(mapData);
        }

        FileHandler.SaveToJSON(data, fileName);
    }

    TilemapData TilemapSave(KeyValuePair<string, Tilemap> mapObj)
    {
        var mapData = new TilemapData();
        mapData.Key = mapObj.Key;

        var boundsForThisMap = mapObj.Value.cellBounds;

        for (var x = boundsForThisMap.xMin; x < boundsForThisMap.xMax; x++)
        for (var y = boundsForThisMap.yMin; y < boundsForThisMap.yMax; y++)
        {
            var pos = new Vector3Int(x, y, 0);
            var tile = mapObj.Value.GetTile(pos);

            if (tile != null)
            {
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tile, out var guid, out var localId))
                {
                    var ti = new TileInfo(tile, pos, guid);
                    mapData.Tiles.Add(ti);
                }
                else
                {
                    Debug.LogError("Could not get GUID for tile " + tile.name);
                }
            }
        }

        return mapData;
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
                foreach (var tile in mapData.Tiles)
                {
                    var tileBase = tile.Tile;
                    if (tileBase == null)
                    {
                        Debug.Log("[Loading Tilemap]: InstanceID not found - looking in AssetDatabase");
                        var path = AssetDatabase.GUIDToAssetPath(tile.GuidFromAssetDB);
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
        Tile = tile;
        Position = pos;
        GuidFromAssetDB = guid;
    }
}

[Serializable]
public class Turret
{
    public Vector3Int Position;
}