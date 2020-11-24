using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        public GameObject sprite;
        public float speed;
        public float gravity;
        public float jumpVelocity;
        public float jumpAdditiveVelocity;
        public Rect sceneBorders;
        public Ball ball;
        public GameObject ballPlace;

        private SpriteRenderer _renderer;
        private Animator _animator;
        private Vector3 _velocity;
        private Vector3 _prevVelocity;
        private int _direction;
        
        private bool _onBall;
        private bool _ball;
        private bool _ground;
        private bool _sit;

        private bool _run;
        private bool _move;
        private bool _jump;
        private bool _jumpPress;
        private bool _throwUp;
        private bool _throwFront;
        
        // private bool _ballHit;
        // private bool _damage;
        // private bool _death;
        // private bool _catch;

        private void Start()
        {
            _animator = sprite.GetComponent<Animator>();
            _renderer = sprite.GetComponent<SpriteRenderer>();
        }

        private void PlayerInput()
        {
            _jump = false;
            _run = false;
            _sit = false;
            _move = false;
            _jumpPress = false;
            _throwUp = false;
            _throwFront = false;
            // _ballHit = false;
            // _damage = false;
            // _death = false;
            // _catch = false;
            _onBall = false;

            float ballX = ball.transform.position.x;
            float x = transform.position.x;
            if (ball.ground && ballX - ball.topWidth < x && x < ballX + ball.topWidth && _prevVelocity.y <= 0)
            {
                _ground = transform.position.y <= sceneBorders.y + ball.width;
                _onBall = _ground;
            }
            else
            {
                _ground = transform.position.y <= sceneBorders.y;
            }

            if (!_sit && Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (_ball)
                {
                    _ball = false;
                    if (Input.GetKey(KeyCode.W))
                    {
                        _throwUp = true;
                        ball.ThrowUp();
                    }
                    else
                    {
                        _throwFront = true;
                        ball.ThrowFront(_direction);
                    }
                }
                else
                {
                    if (ballX - ball.width - 0.01f <= x && x < ballX || x > ballX && x < ballX + ball.width + 0.01f)
                    {
                        ball.ground = false;
                        _ball = true;
                    }
                }
            }
            
            if (_ground && Input.GetKeyDown(KeyCode.Space))
            {
                _jump = true;
            }
            
            if (!_ground && Input.GetKey(KeyCode.Space))
            {
                _jumpPress = true;
            }

            if (_ground && !_ball && Input.GetKey(KeyCode.S))
            {
                _sit = true;
            }
            
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (!_sit)
                {
                    _move = true;
                }
                if (_ground)
                {
                    _run = true;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    _renderer.flipX = true;
                    _direction = -1;
                }
                else
                {
                    _direction = 1;
                    _renderer.flipX = false;
                }
            }

            _animator.SetBool("Run", _run);
            _animator.SetBool("Sit", _sit);
            _animator.SetBool("Ball", _ball);
            _animator.SetBool("Ground", _ground);
            _animator.SetBool("Throw Front", _throwFront);
            _animator.SetBool("Throw Up", _throwUp);
        }

        private void Update()
        {
            PlayerInput();
            _velocity = new Vector3(0, 0, 0);
            
            if (_jump)
            {
                _velocity += jumpVelocity * Vector3.up;
            }

            if (!_ground)
            {
                _velocity += _prevVelocity.y * Vector3.up;
                _velocity -= gravity * Time.deltaTime * Vector3.up;
                if (_jumpPress)
                {
                    _velocity += jumpAdditiveVelocity * Time.deltaTime * Vector3.up;
                }
            }
            
            if (_move)
            {
                _velocity += speed * _direction * Vector3.right;
            }
            
            _prevVelocity = _velocity;
            transform.position += _velocity;

            float ballX = ball.transform.position.x;
            float x = transform.position.x;
            float y = transform.position.y;
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
                x = Mathf.Clamp(transform.position.x, sceneBorders.xMin, sceneBorders.xMax);
            }
            
            if (ball.ground && ballX - ball.topWidth < x && x < ballX + ball.topWidth)
            {
                y = Mathf.Max(sceneBorders.y + ball.width, y);
            }
            else
            {
                y = Mathf.Max(sceneBorders.y, y);
            }

            if (_ball)
            {
                ball.transform.position += (ballPlace.transform.position - ball.transform.position) / 0.8f;
            }
            
            
            transform.position = new Vector3(
                x, 
                y,
                transform.position.z);
        }
    }
}