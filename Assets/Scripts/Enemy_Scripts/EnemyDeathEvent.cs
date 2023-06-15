using System;
using UnityEngine;

public class EnemyDeathEvent : MonoBehaviour
{
    public static event Action EnemyDied;

    public static void OnEnemyDied()
    {
        EnemyDied?.Invoke();
    }
}