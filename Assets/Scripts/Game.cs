using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public bool active = true;
    public UI UI;
    public Sounds Sounds;
    public Progress endScreenDelay = new Progress();

    private bool _win = false;
    private bool _loose = false;

    public void Win()
    {
        endScreenDelay.Start();
        active = false;
        _win = true;
        Sounds.win.Play();
        Sounds.theme.Pause();
    }

    public void Loose()
    {
        endScreenDelay.Start();
        active = false;
        _loose = true;
        Sounds.loose.Play();
        Sounds.theme.Pause();
    }
    
    
    private void Start()
    {
        endScreenDelay.duration = 1;
    }

    private void Update()
    {
        if (endScreenDelay.IsComplete())
        {
            endScreenDelay.progress = 0;
            if (_win)
            {
                UI.EndScreen.gameObject.SetActive(true);
            }
            if (_loose)
            {
                UI.EndScreen.gameObject.SetActive(true);
            }
        }
    }
}
