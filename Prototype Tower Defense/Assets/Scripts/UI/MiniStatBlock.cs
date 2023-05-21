using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniStatBlock : MonoBehaviour
{
    public StatBlock parentStatBlock;
    public int locationIndex;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI spellText;
    public TextMeshProUGUI costText;
    public Image spellImage;
    public Button selectionButton;

    private MageInterface _mage;
    private bool _playerCanAfford;

    private Color _originalCostColor;
    public Color highCostColor;

    public void FillMage(){
        if(_mage != null){
            // the player must spend the hiring cost to actually hire the mage
            if(MapManager.Instance.PlayerBank.SpendGold(_mage.HiringCost())){
                // fill the tower slot with this mage
                parentStatBlock.FillMage(_mage);

                // also remove this mage from the global list (as it has now been hired)
                parentStatBlock.globalMageGenerator.Clear(locationIndex);
            } else {
                Debug.Log("too costly");
            }
        }
    }

    public void SetMage(MageInterface mage){
        string fullName = "";

        if(mage != null){

            foreach(string name in mage.fullName){
                fullName += name + " ";
            }

            nameText.text = fullName;
            spellText.text = mage.SpellDescription();
            spellImage.sprite = mage.saturatedIcon;
            costText.text = mage.HiringCost().ToString();

            _mage = mage;
        }
    }

    void Start(){
        _playerCanAfford = false;
        selectionButton.enabled = false;
        _originalCostColor = costText.color;
        costText.color = highCostColor;
    }

    void Update(){
        // check to see if the player can afford this mage (disable the slot if they can't)
        if(_mage.HiringCost() <= MapManager.Instance.PlayerBank.Gold){
            if(!_playerCanAfford){
                // the player wasn't able to afford the mage, but now they can -- so allow the player to click on it
                selectionButton.enabled = true;
                _playerCanAfford = true;
                costText.color = _originalCostColor;
            }
        } else {
            if(_playerCanAfford){
                // the player could afford the mage previously, but now they can't
                selectionButton.enabled = false;
                _playerCanAfford = false;
                costText.color = highCostColor;
            }
        }
    
    
    }
  
}
