using System;
using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    private Rect _rect = new Rect(0,0, 1, 1);

    private void Start()
    {
        _rect.size = transform.localScale;
        _rect.center = transform.position;
    }

    public Rect GetRect()
    {
        _rect.center = transform.position;
        return _rect;
    }
}
