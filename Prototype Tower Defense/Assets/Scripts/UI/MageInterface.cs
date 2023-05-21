using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageInterface : MonoBehaviour
{
    public List<string> fullName;
    public Mob mage;
    public Tower tower;
    public Sprite saturatedIcon;
    public Sprite desaturatedIcon;
    public float hiringCostScale;
    

    public int HiringCost(){
        float spellDuration = mage.mobController.taskDuration + mage.mobController.idleDuration;
        return Mathf.RoundToInt(hiringCostScale / spellDuration);
    }

    public string SpellDescription(){
        float taskDuration = mage.mobController.taskDuration;
        float idleDuration = mage.mobController.idleDuration;

        float castsPerMinute = 60f / (taskDuration + idleDuration);

        return Mathf.RoundToInt(castsPerMinute) + " spells per minute.";


    }

    

}
