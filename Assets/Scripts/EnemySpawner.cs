using System;
using UnityEngine;

[Serializable]
public class EnemySpawner
{
    public float x;
    public int direction;
    [HideInInspector]
    public bool spawned;
}