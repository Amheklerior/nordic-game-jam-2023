using System.Collections.Generic;
using UnityEngine;
using NordicGameJam2023.Utils;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D))]
public class ResourcesManager : MonoBehaviour
{
    [SerializeField]
    private int _howManyToSpawnAtStartup;

    [SerializeField]
    private AnimationCurve _spawnFrequency;

    private Timer _timer;
    private bool _spawning = false;

    private void Start()
    {
        GameController.Instance.onMatchStart += StartSpawning;
    }

    private void OnDisable()
    {
        GameController.Instance.onMatchStart -= StartSpawning;
    }

    private void Update()
    {
        if(!_spawning) return;
        
        _timer.Tick(Time.deltaTime);
    }

    private void StartSpawning()
    {
        _spawning = true;
        InitialSpawn();
        _timer = new Timer(SpawnRate, () =>
        {
            SpawnNewResource();
            _timer.Restart(SpawnRate);
        });
        _timer.Start();
    }

    private void StopSpawning() => _timer.Stop();

    private void InitialSpawn()
    {
        for (int i = 0; i < _howManyToSpawnAtStartup; i++)
            SpawnNewResource();
    }

    private void SpawnNewResource()
    {
        if (!_pool.HasMore()) return;
        var resourceObj = _pool.Get();
        resourceObj.transform.position = RandomPosition();
        var rot = Random.rotation;
        rot = Quaternion.Euler(0, 0, rot.z * 360f);
        resourceObj.transform.rotation = rot;
        resourceObj.GetComponent<Resource>().onConsume += () =>
        {
            resourceObj.GetComponent<Resource>().IsTaken = false;
            _pool.Put(resourceObj);
        };
    }

    #region Internals

    private GameObjectPool _pool;
    private Bounds _spawningArea;
    private List<Bounds> _occupiedAreas;

    private float RandomX => Random.Range(_spawningArea.center.x - _spawningArea.extents.x, _spawningArea.center.x + _spawningArea.extents.x);
    private float RandomY => Random.Range(_spawningArea.center.y - _spawningArea.extents.y, _spawningArea.center.y + _spawningArea.extents.y);
    private float SpawnRate => _spawnFrequency.Evaluate(GameController.Instance.DistanceFromTheFinishLine);

    private void Awake()
    {
        _pool = GetComponent<GameObjectPool>();
        _spawningArea = GetComponent<Collider2D>().bounds;
        _occupiedAreas = new List<Bounds>();
        GameController.Instance.onMatchEnd += OnMatchEndCallback;
    }

    private void OnDestroy() => GameController.Instance.onMatchEnd -= OnMatchEndCallback;

    private void OnMatchEndCallback(string _winningTeam) => StopSpawning();

    private Vector3 RandomPosition()
    {
        Vector2? pos = null;
        while (pos == null)
        {
            pos = new Vector2(RandomX, RandomY);
            foreach (var area in _occupiedAreas)
            {
                if (area.Contains((Vector2)pos))
                {
                    pos = null;
                    break;
                }
            }
        }
        return (Vector3)pos;
    }

    #endregion


}
