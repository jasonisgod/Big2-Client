using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // OnClick code goes here ...
        //Debug.Log("OnClick");
        var t = gameObject.transform.Find("Mask").transform;
        t.position += Vector3.up * (t.localPosition.y == 0? 100: -100);
    }
}
