using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCounter : MonoBehaviour
{
    public GameObject[] Hearts;
    
    public void SetHeartsNum(int num)
    {
        int i = 0;
        foreach (GameObject heart in Hearts)
        {
            if (i++ < num)
            {
                heart.SetActive(true);
            }
            else
            {
                heart.SetActive(false);
            }
        }
    }
}
