using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{

    public PlayerResources _resources;
    public List<Waypoint> _waypoints;
    public SpriteAnimator _villagerAnimator;
    public float _speed;     // in units of StepLength per FixedUpdate
    public float _deathAnimationSpeed;
    public float _variation;
    public float _stepLength;
    public float _precision;
    public float _walkCyclesPerMeter;

    private Vector3 _spawnLocation;
    private int _currentWaypoint;
    private Vector3 _nextStep;
    private float _waypointError;
    private int _updatesUntilNextStep;

    private bool _isPathing;
    private Observables _thisVillager;



    public void TakeStep(){
        // determine the final location of this step
        Vector3 step = _waypoints[_currentWaypoint].transform.localPosition - transform.localPosition;

        // check to see if this object is close enough to the waypoint
        if(step.sqrMagnitude < _waypointError){
            // this object has arrived at the waypoint, go to the next waypoint
            NextWaypoint();
        } else {
            step.Normalize();
            step *= _stepLength;

            step.x += Random.Range(-_variation, _variation);
            step.y += Random.Range(-_variation, _variation);

            _nextStep = step;
            _updatesUntilNextStep = Mathf.RoundToInt(_speed);
            _isPathing = true;

            // change the villager's animation speed to match its physical speed
            _villagerAnimator.SetSpeed(_speed, _walkCyclesPerMeter);

            // set the correction direction of the villager sprite
            if(_villagerAnimator.ChangeHeading(step)){
                // the direction of the sprite changed, reset the state
                _villagerAnimator.Hop();
            }
        }
    }

    public void NextWaypoint(){
        _currentWaypoint++;
        if(_waypoints.Count > _currentWaypoint){
            TakeStep();
        } else {
            // the villager has reached the final waypoint!
            _resources.AddGold(1);
            Despawn();
        }

    }

    public void Despawn(){
        transform.localPosition = _spawnLocation;
    }

    public void Spawn(){
        if(!_thisVillager.IsSpawned){
            _thisVillager.IsSpawned = true;
            _waypointError = Mathf.Pow(_variation, 2f) / _precision;
            _currentWaypoint = 0;
            _isPathing = false;
            _spawnLocation = transform.localPosition;

            TakeStep();
        }
    }

    public bool Hit(Vector3 source){
        if(_thisVillager.IsSpawned){
            _thisVillager.IsSpawned = false;
            _currentWaypoint = 0;
            _isPathing = false;

            // the villager dies when hit
            _villagerAnimator.SetSpeed(_deathAnimationSpeed, 1f);
            _villagerAnimator.ChangeHeading(source - transform.position);
            _villagerAnimator.Die();
        }
        return true;
    }

    void FixedUpdate(){
        if(_isPathing){
            if(_updatesUntilNextStep > 0){
                transform.localPosition += _speed * Time.fixedDeltaTime * _nextStep;
                _updatesUntilNextStep--;
            } else {
                TakeStep();
            }
        }    
    }

    // Start is called before the first frame update
    void Start()
    {
        _thisVillager = gameObject.GetComponent<Observables>();
    }
}
