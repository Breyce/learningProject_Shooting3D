using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;
    public Color dotHeighLightColor;
    Color originalColor;
    public SpriteRenderer dot;

    private void Start()
    {
        Cursor.visible = false;
        originalColor = dot.color;    
    }

    void Update()
    { 
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);       
    }

    public void DetectedTarget(Ray ray)
    {
        if(Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHeighLightColor;
        }
        else
        {
            dot.color = originalColor;
        }
    }
}
