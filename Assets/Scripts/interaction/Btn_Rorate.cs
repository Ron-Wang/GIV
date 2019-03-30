using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Btn_Rorate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool IsRorate = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsRorate = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        IsRorate = false;
    }

}
