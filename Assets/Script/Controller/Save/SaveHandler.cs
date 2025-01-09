#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;

#endregion

namespace Script.Controller.Save
{
    public class SaveHandler : Singleton<SaveHandler>
    {
        [SerializeField] BoundsInt bounds;
        [SerializeField] string fileName = "SaveBananaDefense.JSON";
        readonly Dictionary<string, Tilemap> _tilemaps = new Dictionary<string, Tilemap>();

        void Start()
        {
            InitTilemap();
        }

        void InitTilemap()
        {
            Tilemap[] maps = FindObjectsByType<Tilemap>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            foreach (var map in maps) _tilemaps.Add(map.name, map);
        }

        public void OnSave()
        {
            var gameSave = new GameSave(
                SaveTurrets(),
                SaveTilemaps()
            );
            FileHandler.SaveToJSON(gameSave, fileName);
        }

        public void OnLoad()
        {
            var gameSave = FileHandler.ReadFromJSON<GameSave>(fileName);
            Time.timeScale = 0;
            LoadTileMap(gameSave);
        }

        async Task LoadTileMap(GameSave gameSave)
        {
            foreach (var mapData in gameSave.Tilemaps)
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
                        var tileBase = await Addressables.LoadAssetAsync<TileBase>(tile.GuidFromAssetDB).Task;
                        OnTileLoaded(tileBase, map, tile);
                    }
            }

            LoadTurrets(gameSave);
        }

        static void OnTileLoaded(TileBase tileBase, Tilemap map, TileInfo tile)
        {
            map.SetTile(tile.Position, tileBase);
        }

        void LoadTurrets(GameSave gameSave)
        {
            foreach (var turretSave in gameSave.Turrets)
            {
                // Find the parent tilemap based on the turret's cell position
                Tilemap parentTilemap = null;
                foreach (var tilemap in _tilemaps.Values)
                    if (tilemap.HasTile(tilemap.WorldToCell(turretSave.Position)))
                    {
                        parentTilemap = tilemap;
                        break;
                    }

                if (parentTilemap != null)
                {
                    // Find the existing turret at the position
                    Turret[] turrets = parentTilemap.GetComponentsInChildren<Turret>();
                    var turret = Array.Find(turrets,
                        t => Vector3.Distance(t.transform.position, turretSave.Position) < 0.1f);

                    if (turret != null)
                        // Set the turret's properties
                        turret.LoadData(turretSave.Damage, turretSave.Range, turretSave.ShootRate,
                            turretSave.MagazineSize, turretSave.ReloadTime, turretSave.Name);
                    else
                        Debug.LogError("No turret found at position " + turretSave.Position);
                }
                else
                {
                    Debug.LogError("No parent tilemap found for turret at position " + turretSave.Position);
                }
            }

            Time.timeScale = 1;
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
        public class TurretSave
        {
            public Vector3 Position;
            public string Name;
            public int Damage;
            public int Range;
            public float ShootRate;
            public int MagazineSize;
            public float ReloadTime;

            public TurretSave(Vector3 position, int damage, int range, float shootRate, int magazineSize,
                float reloadTime, string name)
            {
                Position = position;
                Damage = damage;
                Range = range;
                ShootRate = shootRate;
                MagazineSize = magazineSize;
                ReloadTime = reloadTime;
                Name = name;
            }
        }
        
        [Serializable]
        public class Waves
        {
            
        }

        [Serializable]
        public class GameSave
        {
            public List<TurretSave> Turrets;
            public List<TilemapData> Tilemaps;

            public GameSave(List<TurretSave> turrets, List<TilemapData> tilemaps)
            {
                Turrets = turrets;
                Tilemaps = tilemaps;
            }
        }

        #region SaveData

        List<TurretSave> SaveTurrets()
        {
            List<TurretSave> turretSaves = new List<TurretSave>();
            Turret[] turrets = FindObjectsByType<Turret>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            foreach (var turret in turrets)
            {
                Debug.Log(turret.transform.position);
                var turretSave = new TurretSave(
                    turret.transform.position,
                    turret.Damage,
                    turret.Range,
                    turret.TimeToShoot,
                    turret.MagazineSize,
                    turret.ReloadTime,
                    turret.name
                );
                turretSaves.Add(turretSave);
            }

            return turretSaves;
        }

        List<TilemapData> SaveTilemaps()
        {
            List<TilemapData> data = new List<TilemapData>();

            foreach (KeyValuePair<string, Tilemap> mapObj in _tilemaps)
            {
                var mapData = new TilemapData { Key = mapObj.Key };

                var boundsForThisMap = mapObj.Value.cellBounds;

                for (var x = boundsForThisMap.xMin; x < boundsForThisMap.xMax; x++)
                for (var y = boundsForThisMap.yMin; y < boundsForThisMap.yMax; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var tile = mapObj.Value.GetTile(pos);

                    if (tile != null)
                    {
                        if (tile is AStarTileRule)
                        {
                            var tileAddress = ((RuleTile)tile).name;
                            var tileInfo = new TileInfo(tile, pos, tileAddress);
                            mapData.Tiles.Add(tileInfo);
                        }
                        else
                        {
                            var tileAddress = ((Tile)tile).name;
                            var tileInfo = new TileInfo(tile, pos, tileAddress);
                            mapData.Tiles.Add(tileInfo);
                        }
                    }
                }

                data.Add(mapData);
            }

            return data;
        }

        #endregion
    }
}