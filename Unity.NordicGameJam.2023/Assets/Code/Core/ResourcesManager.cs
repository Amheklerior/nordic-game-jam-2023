using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NordicGameJam2023.Utils;

[RequireComponent(typeof(BoxCollider2D))]
public class ResourcesManager : MonoBehaviour
{
    [SerializeField]
    private int _howManyToSpawnAtStartup;

    [SerializeField]
    private int _howLongToWaitBeforeSpawningNewOnes;

    [SerializeField]
    private float _spawnToSpawnInterval;

    private void Start() => StartSpawning();

    private void StartSpawning() => _spawner = StartCoroutine(SpawnResources());
    private void StopSpawning() => StopCoroutine(_spawner);

    private IEnumerator SpawnResources()
    {
        InitialSpawn();
        yield return new WaitForSeconds(_howLongToWaitBeforeSpawningNewOnes);
        while (true)
        {
            SpawnNewResource();
            yield return new WaitForSeconds(_spawnToSpawnInterval);
        }
    }

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
        rot = Quaternion.Euler(0, 0, rot.z);
        resourceObj.transform.rotation = rot;
        resourceObj.GetComponent<Resource>().onConsume += () =>
        {
            _pool.Put(resourceObj);
        };
    }

    #region Internals

    private GameObjectPool _pool;
    private Bounds _spawningArea;
    public List<Bounds> _occupiedAreas;
    private Coroutine _spawner;

    private float RandomX => Random.Range(_spawningArea.center.x - _spawningArea.extents.x, _spawningArea.center.x + _spawningArea.extents.x);
    private float RandomY => Random.Range(_spawningArea.center.y - _spawningArea.extents.y, _spawningArea.center.y + _spawningArea.extents.y);

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
