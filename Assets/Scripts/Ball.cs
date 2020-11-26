using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Vector3 offset;
    public float topWidth;
    public float width;
    public Rect sceneBorders;
    public float gravity;
    public float throwSpeed;
    public float flySpeed;
    public new CustomCollider collider;
    public CustomCollider catchCollider;
    public Vector3 throwFrontOffset;

    [HideInInspector]
    public bool fly = false;
    [HideInInspector]
    public bool ground = true;

    private Vector3 _velocity;
    private bool _throwUp = false;
    private bool _throwFront = false;
    private int _direction;
    private int _boundCount = 0;

    public void ThrowUp()
    {
        _throwUp = true;
        fly = true;
        _boundCount = 0;
        _velocity = throwSpeed * Vector3.up;
    }

    public void ThrowFront(int direction)
    {
        _throwFront = true;
        fly = true;
        _direction = direction;
        _boundCount = 0;
        throwFrontOffset.x = Mathf.Abs(throwFrontOffset.x) * direction;
        transform.position += throwFrontOffset;
    }

    public void Grab()
    {
        ground = false;
        fly = false;
        _throwFront = false;
        _throwUp = false;
    }

    private void ClampPosition()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, sceneBorders.xMin, sceneBorders.xMax),
            Mathf.Max(sceneBorders.yMin, transform.position.y),
            transform.position.z); 
    }

    void Update()
    {
        if (_throwUp)
        {
            if (transform.position.y > sceneBorders.yMin)
            {
                _velocity += gravity * Time.deltaTime * Vector3.down;
                transform.position += Mathf.Sign(_velocity.y) * Math.Min(flySpeed, Mathf.Abs(_velocity.y)) *
                                      Time.deltaTime * Vector3.up;
            }
            else
            {
                if (_boundCount++ < 2)
                {
                    _velocity.y *= -0.12f;
                    // ground = true;
                    fly = false;
                    transform.position += Mathf.Sign(_velocity.y) * Math.Min(flySpeed, Mathf.Abs(_velocity.y)) *
                                          Time.deltaTime * Vector3.up;
                }
                else
                {
                    ground = true;
                    fly = false;
                    _throwUp = false;
                }
            }
        }
        
        if (_throwFront)
        {
            if (_boundCount < 2)
            {
                transform.position += _direction * flySpeed * Time.deltaTime * Vector3.right;

                if (transform.position.x <= sceneBorders.xMin || sceneBorders.xMax <= transform.position.x)
                {
                    _boundCount++;
                    _direction *= -1;
                }
            }
            else
            {
                _throwFront = false;
                _throwUp = true;
                _velocity = Vector3.zero;
            }
        }
        ClampPosition();
    }
}
