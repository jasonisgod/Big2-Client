using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Floating : MonoBehaviour
{
    public float origin = 1f;
    public float amp = 0.5f;
    public float speed = 10f;
    //bool enabled = true;

    void Update()
    {
        transform.localScale = Vector3.one * (amp * Mathf.Sin(Time.time * speed) + origin);
    }
}
