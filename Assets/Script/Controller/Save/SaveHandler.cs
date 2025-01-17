#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

#endregion

namespace Script.Controller.Save
{
    public class SaveHandler : Singleton<SaveHandler>
    {
        [SerializeField] BoundsInt bounds;
        [SerializeField] string fileName = "SaveBananaDefense.JSON";
        readonly Dictionary<string, Tilemap> _tilemaps = new Dictionary<string, Tilemap>();
        string _saveFilePath;

        void Start()
        {
            _saveFilePath = FileHandler.GetPath(fileName);
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
                SaveTilemaps(),
                SaveWaves(),
                SaveDataPath(),
                SaveMoney()
            );
            FileHandler.SaveToJSON(gameSave, fileName);
            SceneManager.LoadScene(0);

        }


        public bool HasSave()
        {
            return File.Exists(_saveFilePath);
        }

        void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            var gameSave = FileHandler.ReadFromJSON<GameSave>(fileName);
            LoadMoney(gameSave);
        }

        public void OnLoad()
        {
            Time.timeScale = 0;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(1);
            var gameSave = FileHandler.ReadFromJSON<GameSave>(fileName);
            LoadTileMap(gameSave);
        }

        void LoadWaves(GameSave gameSave)
        {
            Debug.Log("Loading Waves");
            var waveManager = WaveManager.GetInstance();
            waveManager.LoadData(
                gameSave.WavesData.WaveCount,
                gameSave.WavesData.BagaSinge,
                gameSave.WavesData.ArtiSinge,
                gameSave.WavesData.MastoSinge,
                gameSave.WavesData.ColosSinge
            );
            Debug.Log("Loading End Waves");
            Debug.Log("Loading Ends");
            Time.timeScale = 1;
        }

        void LoadMoney(GameSave gameSave)
        {
            Debug.Log("Loading Money");
            var moneyManager = MoneyManager.GetInstance();
            moneyManager.LoadData(
                gameSave.MoneyManagerSaveData.Money
            );
            Debug.Log("Loading End Money");
            LoadDataPath(gameSave);
        }

        void LoadDataPath(GameSave gameSave)
        {
            Debug.Log("Loading Path");
            BuildingCreator.GetInstance().LoadData(
                gameSave.DataPaths.WallTiles,
                gameSave.DataPaths.WaterTiles,
                gameSave.DataPaths.TurretTiles);
            Debug.Log("Loading End Path");
            LoadWaves(gameSave);
        }

        async Task LoadTileMap(GameSave gameSave)
        {
            Debug.Log("Loading TileMaps");
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

            Debug.Log("Loading End TileMaps");
            LoadTurrets(gameSave);
        }

        static void OnTileLoaded(TileBase tileBase, Tilemap map, TileInfo tile)
        {
            map.SetTile(tile.Position, tileBase);
        }

        void LoadTurrets(GameSave gameSave)
        {
            Debug.Log("Loading Turret");
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

            Debug.Log("Loading End Turret");
        }


        [Serializable]
        public class TilemapData
        {
            public string Key;
            public List<TileInfo> Tiles = new List<TileInfo>();
        }

        [Serializable]
        public class MoneyManagerSave
        {
            public int Money;

            public MoneyManagerSave(int money)
            {
                Money = money;
            }
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
            public float Range;
            public float ShootRate;
            public int MagazineSize;
            public float ReloadTime;

            public TurretSave(Vector3 position, int damage, float range, float shootRate, int magazineSize,
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
        public class DataPath
        {
            public List<Vector3Int> WallTiles;
            public List<Vector3Int> WaterTiles;
            public List<Vector3Int> TurretTiles;

            public DataPath(List<Vector3Int> wallTiles, List<Vector3Int> waterTiles, List<Vector3Int> turretTiles)
            {
                WallTiles = wallTiles;
                WaterTiles = waterTiles;
                TurretTiles = turretTiles;
            }
        }

        [Serializable]
        public class Waves
        {
            public int WaveCount;
            public int BagaSinge;
            public int ArtiSinge;
            public int MastoSinge;
            public int ColosSinge;

            public Waves(int waveCount, int bagaSinge, int artiSinge, int mastoSinge, int colosSinge)
            {
                WaveCount = waveCount;
                BagaSinge = bagaSinge;
                ArtiSinge = artiSinge;
                MastoSinge = mastoSinge;
                ColosSinge = colosSinge;
            }
        }

        [Serializable]
        public class GameSave
        {
            public List<TurretSave> Turrets;
            public List<TilemapData> Tilemaps;
            public Waves WavesData;
            public DataPath DataPaths;
            public MoneyManagerSave MoneyManagerSaveData;

            public GameSave(List<TurretSave> turrets, List<TilemapData> tilemaps, Waves waves, DataPath dataPath,
                MoneyManagerSave money)
            {
                Turrets = turrets;
                Tilemaps = tilemaps;
                WavesData = waves;
                DataPaths = dataPath;
                MoneyManagerSaveData = money;
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

        Waves SaveWaves()
        {
            var waveManager = WaveManager.GetInstance();
            return new Waves(
                waveManager.WaveCount,
                waveManager.BagaSinge1,
                waveManager.ArtiSinge1,
                waveManager.MastoSinge1,
                waveManager.ColosSinge1
            );
        }

        DataPath SaveDataPath()
        {
            var buildingCreator = BuildingCreator.GetInstance();
            return new DataPath(
                buildingCreator.WallTiles,
                buildingCreator.WaterTiles,
                buildingCreator.TurretTiles
                );
        }


        MoneyManagerSave SaveMoney()
        {
            var moneyManager = MoneyManager.GetInstance();
            return new MoneyManagerSave(
                moneyManager.GetMoneyCount()
            );
        }

        #endregion
    }
}