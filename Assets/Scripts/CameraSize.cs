using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSize : MonoBehaviour
{
    private new Camera camera;
    public Vector3 offset;
    private float aspect;
 
    void Awake()
    {
        camera = GetComponent<Camera>();
        UpdateSize();
    }
    
    void Update()
    {
        if (camera.aspect != aspect)
        {
            UpdateSize();
        }
    }

    private void UpdateSize()
    {
        float hg = (float)Screen.height / Screen.width;
        aspect = camera.aspect;
        camera.orthographicSize = 0.5f / aspect;
        transform.position = new Vector3(offset.x, hg/2 - offset.y, offset.z);
    }
}
