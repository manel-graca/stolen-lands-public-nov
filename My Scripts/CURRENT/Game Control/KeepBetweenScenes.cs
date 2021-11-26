using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepBetweenScenes : MonoBehaviour
{
    private void Awake()
    {
        int instance = FindObjectsOfType<KeepBetweenScenes>().Length;
        if (instance > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            Instantiate(this);
        }
        DontDestroyOnLoad(gameObject);
    }
}
