using UnityEngine;
using UnityEngine.Events;

public static class Events
{
    public static UnityAction<WaveManager.Wave> OnEnemySpawn;
    public static UnityAction<WaveManager.Wave> OnEnemyDeath;
}
