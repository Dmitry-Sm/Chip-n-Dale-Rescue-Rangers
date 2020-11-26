using System;
using System.Management.Instrumentation;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject sprite;
    public float speed;
    public float gravity;
    public float jumpVelocity;
    public float jumpAdditiveVelocity;
    public Rect sceneBorders;

    public Vector2 damageVelocity;
    // public Collider2D collider;

    public new CustomCollider collider;
    // public Rect collider;
    public Ball ball;
    public GameObject ballPlace;
    public Boss boss;
    public UI UI;
    public Game game;

    private Progress _controlLockProgress = new Progress();
    private Progress _saveTimeProgress = new Progress();
    private Progress _ballCathcTime = new Progress();
    private SpriteRenderer _renderer;
    private Animator _animator;
    private Vector3 _velocity;
    private int _direction = 1;
    
    public bool _onBall;
    private bool _ball;
    private bool _ground;
    private bool _sit;

    private bool _run;
    private bool _move;
    private bool _jump;
    private bool _jumpPress;
    private bool _throwUp;
    private bool _throwFront;
    private bool _controlLock;
    
    private bool _ballHit;
    private bool _damage;
    private bool _death;
    private bool _catch;
    private int _lifes = 3;

    private void Start()
    {
        _animator = sprite.GetComponent<Animator>();
        _renderer = sprite.GetComponent<SpriteRenderer>();
        _controlLockProgress.duration = 1f;
        _controlLockProgress.progress = 1f;
        _saveTimeProgress.duration = 1.2f;
        _saveTimeProgress.progress = 1f;
        _ballCathcTime.duration = 0.5f;
        UI.LifeCounter.SetHeartsNum(_lifes);
        _death = false;
    }

    private void CheckCollisions()
    {
        Rect playerRect = collider.GetRect();

        var enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            Rect r2 = enemy.collider.GetRect();
            if (RectsCollided(playerRect, r2))
            {
                SetDamage();
            }
        }
        if (RectsCollided(playerRect, boss.collider.GetRect()) && boss.isActiveAndEnabled)
        {
            SetDamage();
        }
        if (ball.fly && Input.GetButtonDown("Fire1") && 
            _ballCathcTime.IsComplete() &&
            RectsCollided(playerRect, ball.catchCollider.GetRect()))
        {
            CatchBall();
        }
        if (ball.fly && !_ball && RectsCollided(playerRect, ball.collider.GetRect()))
        {
            SetBallHit();
        }
    }

    private void CatchBall()
    {
        _ball = true;
        ball.Grab();
        _animator.SetBool("Catch", true);
    }

    private void SetBallHit()
    {
        _ballHit = true;
        _controlLockProgress.Start();
        _saveTimeProgress.Start();
        _animator.SetBool("Ball Hit", _ballHit);
    }
    
    private void SetDamage()
    {
        _damage = true;
        _controlLockProgress.Start();
        _saveTimeProgress.Start();
        _animator.SetBool("Damage", _damage);
        _velocity.x = -damageVelocity.x * _direction;
        _velocity.y = damageVelocity.y;
        
        UI.LifeCounter.SetHeartsNum(Mathf.Max(0, --_lifes));
        if (_lifes <= 0)
        {
            _death = true;
            game.Loose();
        }
    }

    private bool RectsCollided(Rect rect1, Rect rect2)
    {
        return rect1.x < rect2.x + rect2.width &&
               rect1.x + rect1.width > rect2.x &&
               rect1.y < rect2.y + rect2.height &&
               rect1.y + rect1.height > rect2.y;
    }

    private void CheckGround()
    {
        float ballX = ball.transform.position.x;
        float x = transform.position.x;

        if (ball.ground && Mathf.Abs(ballX - x) < ball.topWidth && _velocity.y <= 0.0f)
        {
            _ground = transform.position.y <= sceneBorders.y + ball.width + 0.001f;
            _onBall = _ground;
        }
        else
        {
            _ground = transform.position.y <= sceneBorders.y;
            _onBall = false;
        }
    }

    private void SetDefaultStates()
    {
        _jump = false;
        _run = false;
        _sit = false;
        _move = false;
        _jumpPress = false;
        _throwUp = false;
        _throwFront = false;
        _catch = false;
        _damage = false;
        _ballHit = false;
    }

    private void PlayerInput()
    {
        if (!_sit && Input.GetButtonDown("Fire1"))
        {
            if (_ball)
            {
                _ball = false;
                _ballCathcTime.Start();
                if (Input.GetAxis("Vertical") > 0.1f)
                {
                    _throwUp = true;
                }
                else
                {
                    _throwFront = true;
                }
            }
            else
            {
                float ballX = ball.transform.position.x;
                float x = transform.position.x;
                if (ball.ground && _ground && !_onBall && Mathf.Abs(ballX - x) - ball.width < 0.01f)
                {
                    ball.Grab();
                    _ball = true;
                }
            }
        }
        if (_ground && Input.GetButtonDown("Jump"))
        {
            _jump = true;
        }

        if (_ground && !_ball && Input.GetAxis("Vertical") < -0.1f)
        {
            _sit = true;
        }
    
        if (Input.GetAxis("Horizontal") < -0.1f || Input.GetAxis("Horizontal") > 0.1f)
        {
            if (!_sit)
            {
                _move = true;
            }
            if (_ground)
            {
                _run = true;
            }
            if (Input.GetAxis("Horizontal") < -0.1f)
            {
                _direction = -1;
            }
            else
            {
                _direction = 1;
            }
        }
        if (!_ground && Input.GetButton("Jump"))
        {
            _jumpPress = true;
        }
    }

    private void SyncAnimations()
    {
        _animator.SetBool("Ball Hit", _ballHit);
        _animator.SetBool("Damage", _damage);
        _animator.SetBool("Catch", _catch);
        _animator.SetBool("Run", _run);
        _animator.SetBool("Sit", _sit);
        _animator.SetBool("Ball", _ball);
        _animator.SetBool("Ground", _ground);
        _animator.SetBool("Throw Front", _throwFront);
        _animator.SetBool("Throw Up", _throwUp);
    }
    
    private void DeathAnimation()
    {
        _velocity.y -= gravity * Time.deltaTime;
        _velocity *= 0.9f;
        transform.position += _velocity * Time.deltaTime;
    }

    private void UpdateVelocity()
    {
        if (_jump)
        {
            _velocity.y = jumpVelocity;
        }

        if (!_ground)
        {
            _velocity.y -= gravity * Time.deltaTime;
            if (_jumpPress)
            {
                _velocity.y += jumpAdditiveVelocity * Time.deltaTime;
            }
        }
        
        if (_move)
        {
            _velocity.x = speed * _direction;
        }
        else 
        {
            if (_controlLockProgress.IsComplete())
            {
                _velocity.x = 0;
            }
            else
            {
                if (_ground)
                {
                    _velocity.x *= 0.8f;
                }
                else
                {
                    _velocity.x *= 0.98f;
                }
            }
        }
    }
    private void UpdatePosition()
    {
        Vector3 position = transform.position + _velocity * Time.deltaTime;

        float ballX = ball.transform.position.x;
        float x = position.x;
        float y = position.y;
        if (_ground && ball.ground && !_onBall)
        {
            if (x < ballX)
            {
                x = Mathf.Clamp(x, sceneBorders.xMin, ballX - ball.width);
            }
            else
            {
                x = Mathf.Clamp(x, ballX + ball.width, sceneBorders.xMax);
            }
        }
        else
        {
            x = Mathf.Clamp(x, sceneBorders.xMin, sceneBorders.xMax);
        }
        
        if (ball.ground && Mathf.Abs(ballX - x) < ball.topWidth)
        {
            y = Mathf.Max(sceneBorders.y + ball.width, y);
        }
        else
        {
            y = Mathf.Max(sceneBorders.y, y);
        }
        
        transform.position = new Vector3(
            x, 
            y,
            position.z);
    }
    
    private void Update()
    {
        if (_death)
        {
            DeathAnimation();
        }
        if (!game.active)
        {
            return;
        }
        
        CheckGround();
        SetDefaultStates();

        if (_controlLockProgress.IsComplete())
        {
            PlayerInput();
        }

        SyncAnimations();
        
        _renderer.flipX = _direction == -1;

        UpdateVelocity();
        UpdatePosition();

        if (_throwUp)
        {
            ball.ThrowUp();
        }
        if (_throwFront)
        {
            ball.ThrowFront(_direction);
        }
        if (_ball)
        {
            ball.transform.position += (ballPlace.transform.position - ball.transform.position) * 0.8f;
        }

        if (_saveTimeProgress.IsComplete())
        {
            CheckCollisions();
        }
    }
}