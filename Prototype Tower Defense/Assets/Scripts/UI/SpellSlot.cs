using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlot : MonoBehaviour
{
    public GameObject observationBoundary;
    public GameObject mageLocation;
    public Vector3 spellHeading;
    public int slotLocation;

    public Image saturatedIcon;
    public Image desaturatedIcon;
    public Image frame;
    public float fadeDuration;

    public Sprite defaultSaturatedIcon;
    public Sprite defaultDesaturatedIcon;

    private MageInterface _interface;
    private bool _isActive;

    private Tower _tower;

    public Tower Tower{
        get => _tower;
        set => _tower = value;
    }

    public MageInterface Occupant{
        get=>_interface;
    }

    public void Request(){
        _tower.Activate(slotLocation);
    }


    public void Activate(){
        // make the icon look saturated (by fading away the desaturation)

        desaturatedIcon.CrossFadeAlpha(0f, fadeDuration, true);

        saturatedIcon.CrossFadeAlpha(1f, 0f, true);
        frame.CrossFadeAlpha(1f, 0f, true);
        

         if(_interface != null){

            // wake up the mage in this slot
            _isActive = true;
            _interface.mage._observationBoundingRectangle = observationBoundary;
            _interface.mage.RangedHeading = spellHeading;
            _interface.mage.mobRenderer.Hide();
            _interface.mage.transform.position = _interface.transform.position;
            _interface.mage.CreateTask();
        } else {
            // open the tower manager so that the player can hire a new mage for this slot
            _tower.OpenTowerManager();
        }
    }

    public void Deactivate(){
        // make the icon look desaturated (by fading in the desaturation)

         desaturatedIcon.CrossFadeAlpha(1f, fadeDuration, true);

         if(_interface != null){
            // turn off the mage
            _isActive = false;
            _interface.mage.StopTasks();
        }
    }

    public void Fill(MageInterface mage){
        saturatedIcon.sprite = mage.saturatedIcon;
        desaturatedIcon.sprite = mage.desaturatedIcon;
        mage.transform.position = mageLocation.transform.position;
        _interface = mage;
    }

    public void Clear(){
        saturatedIcon.sprite = defaultSaturatedIcon;
        desaturatedIcon.sprite = defaultDesaturatedIcon;
        _interface = null;
    }

    public void Hide(float speed){
        // don't hide this slot if it's active
        if(!_isActive){
            saturatedIcon.CrossFadeAlpha(0f, 0f, true);
            desaturatedIcon.CrossFadeAlpha(0f, speed, true);
            frame.CrossFadeAlpha(0f, speed, true);
        }

    }

    public void Show(){
        if(!_isActive){
            saturatedIcon.CrossFadeAlpha(1f, 0f, true);
            desaturatedIcon.CrossFadeAlpha(1f, 0f, true);
            frame.CrossFadeAlpha(1f, 0f, true);
        }
    }

    public void Start(){
        _isActive = false;
    }
}
