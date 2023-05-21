using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatBlock : MonoBehaviour, IPointerExitHandler
{
    public int slotLocation;
    public TowerManager windowManager;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI initialsText;
    public TextMeshProUGUI descriptionText;
    public Image spellImage;

    public GameObject slotFilledPanel;
    public GameObject hirePanel;
    public List<MiniStatBlock> hirePanelBlocks;
    public MageGenerator globalMageGenerator;

    private MageInterface _mage;

    public void OnPointerExit(PointerEventData eventData){
        hirePanel.SetActive(false);
    }

    public void ShowHireList(){
        hirePanel.SetActive(true);

        for(int bufferLocation = 0; bufferLocation < hirePanelBlocks.Count; bufferLocation++){
            hirePanelBlocks[bufferLocation].SetMage(globalMageGenerator.generatedMages[bufferLocation]);
        }
    }

    public void ClearMage(){
        windowManager.ClearSlot(slotLocation);
        _mage = null;
        slotFilledPanel.SetActive(false);
        hirePanel.SetActive(false);
    }

    public void FillMage(MageInterface mage){
        windowManager.FillSlot(slotLocation, mage);
        _mage = mage;
        slotFilledPanel.SetActive(true);
    }

    public void SetMage(MageInterface mage){
        // update the stat block with info from the new mage
        string constructedName = "";
        string initials;

        if(mage == null){
            // there is no mage in this slot
            slotFilledPanel.SetActive(false);
        } else {
            slotFilledPanel.SetActive(true);
        

            foreach(string name in mage.fullName){
                constructedName += name + " ";
            }

            if(mage.fullName.Count > 1){
                initials = mage.fullName[0].Substring(0,1).ToUpper() + mage.fullName[mage.fullName.Count - 1].Substring(0,1).ToUpper();
            } else {
                initials = mage.fullName[0].Substring(0,1).ToUpper();
            }

            nameText.text = constructedName;
            initialsText.text = initials;
            spellImage.sprite = mage.saturatedIcon;
            descriptionText.text = mage.SpellDescription();

            _mage = mage;
        }
    }

    void Start(){
        // hide the hiring panel so that it doesn't need to be hidden in the editor
        hirePanel.SetActive(false);
    }

}
