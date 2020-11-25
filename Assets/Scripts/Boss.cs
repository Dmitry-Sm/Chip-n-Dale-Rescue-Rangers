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
    public Ball ball;
    // public EnemySpawner[] enemySpawners;
    public Phase[] phases;
    public new CustomCollider collider;
    public SpriteRenderer sprite;
    public int nextPhase;

    private int _currentPhase = 0;
    private int _currentHeight = 0;
    private int _direction = 1;
    private Animator _animator;
    private Progress _damageDelay = new Progress();
    public Game game;
    
    void Start()
    {
        nextPhase = _currentPhase;
        _damageDelay.duration = 0.5f;
        _damageDelay.progress = 1f;
        _animator = sprite.GetComponent<Animator>();
        foreach (EnemySpawner spawner in phases[_currentPhase].EnemySpawners)
        {
            if (!spawner.spawned && (_direction == 1 && transform.position.x >= spawner.x ||
                                     _direction == -1 && transform.position.x <= spawner.x))
            {
                spawner.spawned = true;
            }
        }
    }

    private void CheckCollisions()
    {
        Rect bossRect = collider.GetRect();
        
        if (ball.fly && _damageDelay.IsComplete() && RectsCollided(bossRect, ball.collider.GetRect()))
        {
            SetDamage();
        }
    }
    
    private void SetDamage()
    {
        lifes--;
        if (lifes <= 0)
        {
            game.Win();
        }

        if (lifes == 4)
        {
            nextPhase = 1;
        }

        if (lifes == 2)
        {
            nextPhase = 2;
        }
        _damageDelay.Start();
        _animator.SetBool("Damage", true);
    }

    private void SetDefailtAnimation()
    {
        _animator.SetBool("Damage", false);
    }
    

    private bool RectsCollided(Rect rect1, Rect rect2)
    {
        return rect1.x < rect2.x + rect2.width &&
               rect1.x + rect1.width > rect2.x &&
               rect1.y < rect2.y + rect2.height &&
               rect1.y + rect1.height > rect2.y;
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
        foreach (EnemySpawner spawner in phases[_currentPhase].EnemySpawners)
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
        _currentPhase = nextPhase;
        foreach (EnemySpawner spawner in phases[_currentPhase].EnemySpawners)
        {
            spawner.spawned = false;
        }
        sprite.flipX = _direction == -1;
        _currentHeight = (++_currentHeight) % height.Length;
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, sceneBorders.xMin, sceneBorders.xMax), 
            height[_currentHeight],
            transform.position.z);
    }
    
    void Update()
    {
        if (!game.active)
        {
            return;
        }
        _damageDelay.Update();
        SetDefailtAnimation();
        CheckCollisions();
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