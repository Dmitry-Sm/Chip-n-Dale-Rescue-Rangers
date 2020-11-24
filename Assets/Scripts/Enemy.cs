using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public float speed;
    public int direction;
    public float gravity;
    public Rect sceneBorders;
    public SpriteRenderer renderer;

    private Vector3 _velocity;
    private bool _fly = true;

    private void Start()
    {
        _velocity = new Vector3(Random.value * 0.002f - 0.001f, Random.value * 0.002f);
        renderer.flipX = direction == -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (_fly)
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
                _fly = false;
            }
        }
        else
        {
            if (transform.position.x >= sceneBorders.xMin && transform.position.x <= sceneBorders.xMax)
            {
                transform.position += direction * speed * Time.deltaTime * Vector3.right;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
