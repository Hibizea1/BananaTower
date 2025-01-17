using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : Singleton<WaveManager>
{
    public int WaveCount
    {
        get => _waveCount;
        set => _waveCount = value;
    }

    public int BagaSinge1
    {
        get => BagaSinge;
        set => BagaSinge = value;
    }

    public int ArtiSinge1
    {
        get => ArtiSinge;
        set => ArtiSinge = value;
    }

    public int MastoSinge1
    {
        get => MastoSinge;
        set => MastoSinge = value;
    }

    public int ColosSinge1
    {
        get => ColosSinge;
        set => ColosSinge = value;
    }

    int _waveCount = 1;
    [SerializeField] int _enemyCount;
    [SerializeField] int MaxEnemiesSpawn;
    [SerializeField] float TimeBetweenSpawnMin;
    [SerializeField] float TimeBetweenSpawnMax;
    [SerializeField] TextMeshProUGUI NextWaveText;
    [SerializeField] TextMeshProUGUI WaveCountText;

    public int EnemyCount
    {
        get => _enemyCount;
        set => _enemyCount = value;
    }

    [SerializeField] int BagaSinge;
    [SerializeField] int ArtiSinge;
    [SerializeField] int MastoSinge;
    [SerializeField] int ColosSinge;

    int _bageSingeCount;
    int _artiSingeCount;
    int _mastoSingeCount;
    int _colosSingeCount;
    bool isStarted;

    [SerializeField] List<GameObject> Monkeys = new List<GameObject>();

    bool _waveStarted;
    float _waitTime = 30f;

    public float WaitTime => _waitTime;

    public bool WaveStarted => _waveStarted;

    [SerializeField] Vector3Int SpawnTile;
    Vector3Int _spawnTilePos;

    EventMaster _eventMaster;

    void Awake()
    {
        _eventMaster = EventMaster.GetInstance();
        _eventMaster.CreateNewEvent("CheckCount");
        _eventMaster.GetEvent("CheckCount").AddListener(CheckMonkeyNumber);
    }


    public void LoadData(int waveCount, int bagaSinge, int artiSinge, int mastoSinge, int colosSinge)
    {
        WaveCount = waveCount;
        BagaSinge1 = bagaSinge;
        ArtiSinge1 = artiSinge;
        MastoSinge1 = mastoSinge;
        ColosSinge1 = colosSinge;
    }

    void Start()
    {
        _bageSingeCount = BagaSinge;
        _artiSingeCount = ArtiSinge;
        _mastoSingeCount = MastoSinge;
        _colosSingeCount = ColosSinge;
        StartCoroutine(WaitForSpawn());
    }

    void SpawnBagaSinge()
    {
        if (_bageSingeCount >= 0)
        {
            Instantiate(Monkeys[0], SpawnTile, Quaternion.identity);
            _enemyCount++;
            _bageSingeCount--;
            Debug.Log("Spawn" + _bageSingeCount);
        }
    }

    void SpawnArtiSinge()
    {
        if (_waveCount >= 5 && _artiSingeCount >= 0)
        {
            Instantiate(Monkeys[1], SpawnTile, Quaternion.identity);
            _enemyCount++;
            _artiSingeCount--;
            Debug.Log("Spawn" + _artiSingeCount);
        }
    }

    void SpawnMastoSinge()
    {
        if (_waveCount >= 10 && _mastoSingeCount >= 0)
        {
            Instantiate(Monkeys[2], SpawnTile, Quaternion.identity);
            _enemyCount++;
            _mastoSingeCount--;
            Debug.Log("Spawn");
        }
    }

    void SpawnColosSinge()
    {
        if (_waveCount >= 15 && _waveCount % 5 == 0 && _colosSingeCount >= 0)
        {
            Instantiate(Monkeys[3], SpawnTile, Quaternion.identity);
            _enemyCount++;
            _colosSingeCount--;
            Debug.Log("Spawn");
        }
    }


    void CheckMonkeyNumber()
    {
        if (_enemyCount <= 0 && !isStarted)
        {
            isStarted = true;
            _waitTime = 30f;
            _waveStarted = false;
            WaveCountText.text = _waveCount.ToString();
            BagaSinge += 5;
            ArtiSinge += 2;
            MastoSinge += 1;
            _bageSingeCount = BagaSinge;
            _artiSingeCount = ArtiSinge;
            _mastoSingeCount = MastoSinge;
            _colosSingeCount = ColosSinge;
            StartCoroutine(WaitForSpawn());
        }
    }

    IEnumerator WaitForSpawn()
    {
        while (_waitTime >= 0)
        {
            yield return new WaitForSeconds(1);
            _waitTime -= 1;
            NextWaveText.text = _waitTime.ToString();
        }

        isStarted = false;
        _waveCount++;
        _waitTime = 0;
        NextWaveText.text = _waitTime.ToString();
        _waveStarted = true;
        StartCoroutine(SpawnMonkeys());
    }

    IEnumerator SpawnMonkeys()
    {
        while (_waveStarted)
        {
            SpawnBagaSinge();
            SpawnArtiSinge();
            SpawnMastoSinge();
            SpawnColosSinge();
            yield return new WaitForSeconds(Random.Range(TimeBetweenSpawnMin, TimeBetweenSpawnMax));
        }
    }
}