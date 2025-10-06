using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Shared")]
    [SerializeField] private SharedFloat _ticks;
    private float ticks
    {
        get => _ticks.shared;
        set => _ticks.shared = value;
    }
    [SerializeField] private SharedTransform _player;
    private Transform player => _player.shared;

    [Header("Waves")]
    [SerializeField] private List<Wave> waves = new List<Wave>();

    [Header("Spawning")]
    [SerializeField] private float spawnRadius = 20f;

    [Header("Ticks")]
    [SerializeField] private float tickRate = 1f;
    [SerializeField] private float tickTotal = 300f;
    private float timer = 0f;

    private Dictionary<Wave, int> spawned = new Dictionary<Wave, int>();

    private void Awake()
    {
        ticks = tickTotal;
    }

    private void OnEnable()
    {
        Events.OnEnemySpawn += RegisterEnemy;
        Events.OnEnemyDeath += UnregisterEnemy;
    }

    private void OnDisable()
    {
        Events.OnEnemySpawn -= RegisterEnemy;
        Events.OnEnemyDeath -= UnregisterEnemy;
    }

    private void Update()
    {
        timer += Time.deltaTime * tickRate;
        if(timer > 1f)
        {
            timer -= 1f;
            Tick();
        }
    }

    private void Tick()
    {
        ticks -= 1f;
        int currentTick = Mathf.FloorToInt(ticks);
        foreach(Wave wave in waves)
        {
            if(wave.from >= currentTick && wave.to <= currentTick && currentTick % wave.cooldown == 0)
            {
                // Valid Spawn
                if (!spawned.ContainsKey(wave)) spawned.TryAdd(wave, 0);
                if(spawned.TryGetValue(wave, out int amount) && amount < wave.total)
                {
                    int amountToSpawn = Mathf.Min(wave.amount, wave.total - amount);
                    for(int i = 0; i < amountToSpawn; i++)
                    {
                        Spawn(wave);
                    }
                }
            }
        }
    }

    public void Spawn(Wave wave)
    {
        Vector3 spawnPoint = GetRandomPoint(player.position, spawnRadius);
        Enemy enemy = (Enemy)ObjectPools.Instance.Get(wave.spawn);
        enemy.Spawn(wave, spawnPoint);
    }

    public Vector3 GetRandomPoint(Vector3 position, float radius)
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float x = Mathf.Cos(randomAngle);
        float z = Mathf.Sin(randomAngle);
        return position + (new Vector3(x, 0f, z) * radius);
    }

    private void RegisterEnemy(Wave wave)
    {
        if (spawned.ContainsKey(wave)) spawned[wave] += 1;
    }

    private void UnregisterEnemy(Wave wave)
    {
        if (spawned.ContainsKey(wave)) spawned[wave] -= 1;
    }

    [System.Serializable]
    public struct Wave
    {
        public Enemy spawn;
        public int from;
        public int to;
        public int amount;
        public int total;
        public int cooldown;
    }
}
