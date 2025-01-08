using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveManager : Singleton<WaveManager>
{
    int _waveCount;
    int _enemyCount;
    [SerializeField] int BagaSinge;
    [SerializeField] int ArtiSinge;
    [SerializeField] int MastoSinge;
    [SerializeField] int ColosSinge;

    [SerializeField] List<GameObject> Monkeys = new List<GameObject>();

    bool _waveStarted;

    public bool WaveStarted => _waveStarted;

    [SerializeField] Tile SpawnTile;
    Vector3Int _spawnTilePos;
    [SerializeField] Tilemap DefaultMap;

    EventMaster _eventMaster;
    void Awake()
    {
        _eventMaster = EventMaster.GetInstance();
        _eventMaster.CreateNewEvent("CheckCount");
        _eventMaster.GetEvent("CheckCount").AddListener(CheckMonkeyNumber);
    }

    void Start()
    {
        foreach (Vector3Int pos in DefaultMap.cellBounds.allPositionsWithin)
        {
            if (DefaultMap.GetTile(pos) == SpawnTile)
            {
                _spawnTilePos = pos;
            }
        }
        StartCoroutine(WaitForSpawn());
    }

    void SpawnBagaSinge()
    {
        for (int i = 0; i < BagaSinge; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(-3f, 3f),
                0f
            );
            Vector3 spawnPosition = DefaultMap.CellToWorld(_spawnTilePos) + randomOffset;
            Instantiate(Monkeys[0], spawnPosition, Quaternion.identity);
            _enemyCount++;
        }

        _waveStarted = true;
    }

    void CheckMonkeyNumber()
    {
        if (_enemyCount <= 0)
        {
            _waveCount++;
            _waveStarted = false;
            StartCoroutine(WaitForSpawn());
        }
    }

    IEnumerator WaitForSpawn()
    {
        float waitTime = 30f;
        while (waitTime > 0)
        {
            // Debug.Log("Time remaining: " + waitTime + " seconds");
            yield return new WaitForSeconds(1);
            waitTime -= 1;
        }
        BagaSinge += 5;
        SpawnBagaSinge();
    }
}