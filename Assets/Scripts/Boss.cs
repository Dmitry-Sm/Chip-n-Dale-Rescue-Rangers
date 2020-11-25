using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float speed;
    public float[] height;
    public Enemy enemyPrefab;
    public int lifes;
    public Rect sceneBorders;
    // public EnemySpawner[] enemySpawners;
    public SpriteRenderer spriteRenderer;
    public Phase[] phases;
    public int currentPhase = 0;
    public new CustomCollider collider;

    private int _currentHeight = 0;
    private int _direction = 1;
    
    
    void Start()
    {
        foreach (EnemySpawner spawner in phases[currentPhase].EnemySpawners)
        {
            if (!spawner.spawned && (_direction == 1 && transform.position.x >= spawner.x ||
                                     _direction == -1 && transform.position.x <= spawner.x))
            {
                spawner.spawned = true;
            }
        }
    }

    private void SpawnEnemy(EnemySpawner spawner)
    {
        var enemy = Instantiate(enemyPrefab);
        enemy.transform.position = transform.position;
        enemy.direction = spawner.direction;
        spawner.spawned = true;
    }

    private void CheckSpawners()
    {
        foreach (EnemySpawner spawner in phases[currentPhase].EnemySpawners)
        {
            if (!spawner.spawned && (_direction == 1 && transform.position.x >= spawner.x ||
                                     _direction == -1 && transform.position.x <= spawner.x))
            {
                SpawnEnemy(spawner);
            }
        }
    }

    private void SwapDirection()
    {
        _direction *= -1;
        foreach (EnemySpawner spawner in phases[currentPhase].EnemySpawners)
        {
            spawner.spawned = false;
        }
        spriteRenderer.flipX = _direction == -1;
        _currentHeight = (++_currentHeight) % height.Length;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, sceneBorders.xMin, sceneBorders.xMax), 
            height[_currentHeight],
            transform.position.z);
    }
    
    void Update()
    {
        if (transform.position.x >= sceneBorders.xMin && transform.position.x <= sceneBorders.xMax)
        {
            transform.position += _direction * speed * Time.deltaTime * Vector3.right;
            CheckSpawners();
        }
        else
        {
            SwapDirection();
        }
    }
}