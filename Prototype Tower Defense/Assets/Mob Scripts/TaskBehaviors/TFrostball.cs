using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFrostball : TaskBehavior
{
    public SpriteRenderer _frostball;
    public TrailRenderer _frostballTrail;
    public float speedMultiplier;
    public int retryTargetAttempts;
    public float trailPersistTime;
    public Color frozenColor;

    private Mob _target;
    private Vector3 _initialPosition;
    private Vector3 _launch;

    private float _elapsedTime;
    private float _timeScale;

    private bool _isCast;


    public override void LeadingUpdate(Task task)
    {
        base.LeadingUpdate(task);

        _frostballTrail.Clear();


        // set the target of this spell
        _target = FindUnfrozenTarget(_this.mobController.Target, retryTargetAttempts);
        _initialPosition = transform.position;
        _launch = _this.RangedHeading;

        _elapsedTime = 0f;

        // set the time scale
        _timeScale = 1f / task.duration;

        // first, check to see if the spell can be cast
        _isCast = true;
        
        if(!_target.isAlive){
            _isCast = false;
        }

        if(_isCast){
            // make the fireball visible
            _frostballTrail.Clear();
            _frostballTrail.emitting = true;

            _frostballTrail.time = trailPersistTime * _timeScale;
        }


    }

    private Mob FindUnfrozenTarget(Mob target, int attemptsRemaining){
        if(target != null){
            if(target.globalSpeedMultiplier < 1f && attemptsRemaining > 0){
                // target is currently frozen, try finding another target
                return FindUnfrozenTarget(_this.mobController.FindMobTarget(), attemptsRemaining - 1);
            } else {
                // either the target is not frozen, or we have run out of attempts to find a good target
                // either way, return the current target
                return target;
            }
        } else {
            return null;
        }
    }

    public override void TaskFixedUpdate(Task task)
    {
        base.TaskFixedUpdate(task);
        float squareTime;
        if(_isCast){
            _elapsedTime += _timeScale * Time.fixedDeltaTime;

            squareTime = Mathf.Pow(_elapsedTime, 2f);


            _frostball.transform.position = (1f - squareTime) * _initialPosition + 
                                        2f * _elapsedTime * (1f - squareTime) * _launch +
                                        squareTime * _target.transform.position;
        }
                                        
    }

    public override void TrailingUpdate(Task task)
    {
        if(_isCast){
            // hide the fireball
            _frostballTrail.emitting = false;
            _frostballTrail.Clear();

            // move the fireball back to the initial position
            _frostball.transform.position = _initialPosition;

            // freeze the target
            if(_target != null){

                _target.globalSpeedMultiplier = speedMultiplier;

                // change the target's color to represent being frozen
                _target.mobRenderer.spriteRenderer.color = frozenColor;

                _target.tasks.Interrupt();

            }

            // add an idle task to control how long it will take before the mage casts again
            _this.mobController.SendIdleTask();
        }

        
        base.TrailingUpdate(task);
    }

    public override void Interrupted(Task task)
    {

        // hide the fireball on interrupt
        _frostballTrail.emitting = false;
        _frostballTrail.Clear();

        // move the fireball back to the initial position
        _frostball.transform.position = _initialPosition;
        
        base.Interrupted(task);
    }

}
