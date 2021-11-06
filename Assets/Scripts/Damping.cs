using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Damping : MonoBehaviour
{
    public float origin = 1f;
    public float amp = 0.5f;
    public float speed = 10f;
    //bool enabled = true;
    float startTime;

    public void Hit()
    {
        startTime = Time.time;
    }

    void Update()
    {
        var t = Time.time - startTime;  
        transform.localScale = Vector3.one * (amp * Mathf.Exp(-t) * Mathf.Sin(t * speed) + origin);
    }
}
