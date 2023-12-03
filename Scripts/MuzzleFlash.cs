using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder;

    public float flashTime;


    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        flashHolder.SetActive(true);

        Invoke("Deactivate", flashTime);    
    }

    public void Deactivate()
    {
        flashHolder.SetActive(false);
    }
}
