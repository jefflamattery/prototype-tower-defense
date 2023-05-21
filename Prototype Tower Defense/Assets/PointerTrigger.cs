using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Tower tower;

    public void OnPointerEnter(PointerEventData eventData){
        tower.ShowUI();
    }

    public void OnPointerExit(PointerEventData eventData){
        tower.HideUI();
    }
}
