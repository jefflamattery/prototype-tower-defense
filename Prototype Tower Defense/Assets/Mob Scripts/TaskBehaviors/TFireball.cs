using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TFireball : TaskBehavior
{
    public SpriteRenderer _fireball;
    public TrailRenderer _fireballTrail;

    public GameObject _explosionTemplate;
    private ParticleSystem _explosion;
    public float trailPersistTime;

    private Mob _target;
    private Vector3 _initialPosition;
    private Vector3 _launch;

    private float _elapsedTime;
    private float _timeScale;

    private bool _isCast;


    public override void LeadingUpdate(Task task)
    {
        base.LeadingUpdate(task);

        _fireballTrail.Clear();


        // set the target of this spell
        _target = _this.mobController.Target;
        _initialPosition = transform.position;
        _launch = _this.RangedHeading;

        // set the time scale
        _timeScale = 1f / task.duration;

        _elapsedTime = 0f;

        // first, check to see if the spell can be cast
        _isCast = true;
        
        if(!_target.isAlive){
            _isCast = false;
        }

        if(_isCast){
            // make the fireball visible
            _fireballTrail.Clear();
            _fireballTrail.emitting = true;

            // the amount of time that the trail should persist depends on how fast the fireball is moving
            // it will need to persist longer as the fireball moves slower
            _fireballTrail.time = trailPersistTime * task.duration;
        }

        // create a new explosion in preperation for when the fireball hits the target
        _explosion = GameObject.Instantiate(_explosionTemplate).GetComponent<ParticleSystem>();
        
    }

    public override void TaskFixedUpdate(Task task)
    {
        base.TaskFixedUpdate(task);
        float squareTime;
        if(_isCast){
            _elapsedTime += _timeScale * Time.fixedDeltaTime;

            squareTime = Mathf.Pow(_elapsedTime, 2f);


            _fireball.transform.position = (1f - squareTime) * _initialPosition + 
                                        2f * _elapsedTime * (1f - squareTime) * _launch +
                                        squareTime * _target.transform.position;
        }
                                        
    }

    public override void TrailingUpdate(Task task)
    {
        if(_isCast){
            // hide the fireball
            _fireballTrail.emitting = false;
            _fireballTrail.Clear();

            // move the fireball back to the initial position
            _fireball.transform.position = _initialPosition;

            // hit the target
            if(_target != null){
                _target.Hit(Random.insideUnitCircle);

                // place the explosion
                _explosion.transform.position = _target.transform.position;
                _explosion.Play();
            }

            // add an idle task to control how long it will take before the mage casts again
            _this.mobController.SendIdleTask();
        }

        
        base.TrailingUpdate(task);
    }

    public override void Interrupted(Task task)
    {

        // hide the fireball on interrupt
        _fireballTrail.emitting = false;
        _fireballTrail.Clear();

        // move the fireball back to the initial position
        _fireball.transform.position = _initialPosition;
        
        base.Interrupted(task);
    }

}
