using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickPlayer : MonoBehaviour, IPointerClickHandler
{
    public int id;

    public void OnPointerClick(PointerEventData eventData)
    {
        Game.id = id;
    }
}
