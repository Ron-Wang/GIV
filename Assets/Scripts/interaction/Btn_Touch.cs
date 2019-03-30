using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Btn_Touch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool IsTouch = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsTouch = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        IsTouch = false;
    }

}
