
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tower : MonoBehaviour
{
    public List<SpellSlot> spellSlots;

    public List<MageInterface> initialMages;

    public ButtonHider manageButton;
    public float fadeOutSpeed;
    public TowerManager towerManagerWindow;

    public void OpenTowerManager(){
        List<MageInterface> mages = new List<MageInterface>();

        foreach(SpellSlot slot in spellSlots){
            mages.Add(slot.Occupant);
        }

        towerManagerWindow.gameObject.SetActive(true);
        towerManagerWindow.Open(this, mages);
    }

    void OnMouseEnter(){
        ShowUI();
    }

    void OnMouseExit(){
        HideUI();
    }

    public void HideUI(){
        // hide all spell slots (the active spell slot will prevent itself from being hidden)
        foreach(SpellSlot slot in spellSlots){
            slot.Hide(fadeOutSpeed);
        }

        // hide the manage button
        manageButton.Hide(fadeOutSpeed);
    }

    public void ShowUI(){
        // show all spell slots
        foreach(SpellSlot slot in spellSlots){
            slot.Show();
        }

        // show the manage button
        manageButton.Show(0f);
    }

    public void Activate(int slotLocation){
        // activate the spell at the slotLocation
        // and deactivate all other spells
        for(int location = 0; location < spellSlots.Count; location++){
            if(location == slotLocation){
                spellSlots[location].Activate();
            } else {
                spellSlots[location].Deactivate();
            }
        }
    }

    public void ClearSlot(int slotLocation){
        spellSlots[slotLocation].Deactivate();
        spellSlots[slotLocation].Clear();
    }

    public void FillSlot(int slotLocation, MageInterface mage){
        spellSlots[slotLocation].Fill(mage);
        Activate(slotLocation);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        for(int location = 0; location < spellSlots.Count; location++){
            // give every spell slot a reference to this tower
            spellSlots[location].Tower = this;

            // load the spell slot with the corresponding initial mage
            if(location < initialMages.Count){
                spellSlots[location].Fill(initialMages[location]);
            }
        }
        
    }

}
