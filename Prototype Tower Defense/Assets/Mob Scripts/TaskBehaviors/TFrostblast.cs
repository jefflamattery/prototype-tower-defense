using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFrostblast : TaskBehavior
{
    
    public Animator _frostWaveAnimation;
    public string _blastStateName;
    public string _hiddenStateName;
    public GameObject _frostblastBoundary;
    public float _speedMultiplier;
    private List<Mob> _targets;


    private bool _isCast;

    public void HitTarget(Mob target){
        TMove goblinMoveBehavior;
        IceExplosion ice;
        goblinMoveBehavior = target.GetComponentInChildren<TMove>();
        ice = target.GetComponentInChildren<IceExplosion>();

        target.globalSpeedMultiplier = _speedMultiplier;

        target.tasks.Interrupt();

        if(ice != null){
            ice.ToggleSprites(true);
        }
    } 

    public override void LeadingUpdate(Task task)
    {
        base.LeadingUpdate(task);
        
        // play the frost wave animation
        _frostWaveAnimation.Play(_blastStateName);

         

        // change the mob's observation boundary so that all mobs on the field are affected
        _this._observationBoundingRectangle = _frostblastBoundary;

        // find all goblins to prepare to hit them with the spell
        _targets = _this.AllMobsMatchingFilter(Mob.Goblin_Filter);


        for(int i = 0; i < _targets.Count; i++){

            HitTarget(_targets[i]);
        }

    }

    public override void TaskFixedUpdate(Task task)
    {
        base.TaskFixedUpdate(task);
    }


    public override void TrailingUpdate(Task task)
    {

        _frostWaveAnimation.Play(_hiddenStateName);

        _this.mobController.SendIdleTask();

        base.TrailingUpdate(task);
    }

    public override void Interrupted(Task task)
    {
        base.Interrupted(task);

        _frostWaveAnimation.Play(_hiddenStateName);
        _this.mobController.SendIdleTask();


    }

}
