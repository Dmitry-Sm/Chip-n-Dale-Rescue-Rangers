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

    // [HideInInspector]
    public bool ground = true;

    private Vector3 _velocity;
    private bool _throwUp = false;
    private bool _throwFront = false;
    private int _direction;
    private int _boundCount = 0;

    public void ThrowUp()
    {
        _throwUp = true;
        _velocity = throwSpeed * Vector3.up;
    }

    public void ThrowFront(int direction)
    {
        _throwFront = true;
        _direction = direction;
        _boundCount = 0;
    }
    
    void Update()
    {
        if (_throwUp)
        {
            if (transform.position.y >= sceneBorders.yMin)
            {
                _velocity += gravity * Time.deltaTime * Vector3.down;
                transform.position += _velocity;
            }
            else
            {
                transform.position = new Vector3(
                    transform.position.x,
                    Mathf.Max(sceneBorders.yMin, transform.position.y),
                    transform.position.z);
                ground = true;
                _throwUp = false;
            }
            
        }

        if (_throwFront)
        {
            if (_boundCount < 2)
            {
                if (transform.position.x >= sceneBorders.xMin && transform.position.x <= sceneBorders.xMax)
                {
                    transform.position += _direction * flySpeed * Time.deltaTime * Vector3.right;
                }
                else
                {
                    transform.position = new Vector3(
                        Mathf.Clamp(transform.position.x, sceneBorders.xMin, sceneBorders.xMax),
                        transform.position.y,
                        transform.position.z);
                    _direction *= -1;
                    _boundCount++;
                }
            }
            else
            {
                _throwFront = false;
                _throwUp = true;
                _velocity = Vector3.zero;
            }
        }
        
        
    }
}
