using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : MonoBehaviour, IPointerExitHandler
{

    public Bank playerBank;
    public List<StatBlock> towerSlots;

    private Tower _opener;

    void Start(){
        // hide the window (so that it doesn't have to be hidden in the editor)
        gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData){
        Close();
    }

    public void ClearSlot(int slotLocation){
        if(_opener != null){
            _opener.ClearSlot(slotLocation);
        }
    }

    public void FillSlot(int slotLocation, MageInterface mage){
        if(_opener != null){
            _opener.FillSlot(slotLocation, mage);
            towerSlots[slotLocation].SetMage(mage);
        }
    }

    public void Close(){
        MapManager.Instance.ResumeGame();
        gameObject.SetActive(false);
    }

    public void Open(Tower opener, List<MageInterface> occupants){

       Rect rect = gameObject.GetComponent<RectTransform>().rect;

        // move this window to the pointer location
        transform.position = Input.mousePosition - new Vector3(0f, transform.localScale.y * rect.height/2.5f, 0f);

        // hold on to a reference to the tower that opened this window
        _opener = opener;


        for(int location =  0; location < towerSlots.Count; location++){
            if(location < occupants.Count){
                towerSlots[location].SetMage(occupants[location]);
            } else {
                towerSlots[location].SetMage(null);
            }
        }

        // pause the game
        MapManager.Instance.PauseGame();
    }


}
