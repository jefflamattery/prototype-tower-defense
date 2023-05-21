using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    public LineRenderer ObservationBoundaryLine;
    public LineRenderer TargetLine;
    public SpriteAnimator GoblinSpriteAnimator;
    public int BoundaryLinePrecision;
    public float ObservationDistance;
    public float Speed;
    public float SpeedVariation;
    public float HeadingVariation;
    public float _stepSize;
    public float _attackTime;
    
    public float Reach;
    public float UnhideChance;
    public Vector3 InitialHeading;
    public float _minimumStepLength;
    public bool DrawDebugLines;

    private List<Observables> _observables;
    private Observables _target;
    private Observables _thisGoblin;

    private Vector3 _stepEndpoint;
    private Vector3 _step;
    private Vector3 _heading;

    private bool _findTargetNextUpdate;
    private bool _hitTargetNextUpdate;

    private Vector3 _spawnPosition;
    

    public void SetSpawn(Vector3 position){
        _spawnPosition = position;
        transform.position = position;
    }
    public void Spawn(){

        _thisGoblin = gameObject.GetComponent<Observables>();

        if(!_thisGoblin.IsSpawned){
            _observables = new List<Observables>();
            _target = null;
            _thisGoblin.IsMoving = false;
            _findTargetNextUpdate = false;
            _hitTargetNextUpdate = false;
            _step = new Vector3();
            _stepEndpoint = new Vector3();
            _heading = InitialHeading;

            _thisGoblin.IsSpawned = true;

            NextStep(null);
        }
    }

    public void Despawn(){
        transform.position = _spawnPosition;
        _target = null;
        _thisGoblin.IsMoving = false;
        _step = Vector3.zero;

        _thisGoblin.IsSpawned = false;
        

    }

    public void Kill(){
        Debug.Log("Goblin Killed, TODO: Death animation");

        // for now, despawn the goblin
        Despawn();
    }

    // Start is called before the first frame update
    void Start()
    {
        _thisGoblin = gameObject.GetComponent<Observables>();
    }

    void FixedUpdate(){
        if(_thisGoblin.IsMoving){
            if((_stepEndpoint - transform.position).sqrMagnitude <= Reach){
                // the target is within reach
                FinishedStep();
            } else {
                // keep moving toward the target
                transform.position += Speed * Time.fixedDeltaTime * _step;
            }
        }
    }

    void Update(){
        if(_thisGoblin.IsSpawned){
            if(_findTargetNextUpdate || !_thisGoblin.IsMoving){
                FindTarget();
                _findTargetNextUpdate = false;
            }

            if(_hitTargetNextUpdate){
                if(_target != null){

                
                    // if the target is a villager, hit it
                    if(_target.IsVillager){
                        
                        GoblinSpriteAnimator.Attack(_attackTime);
                         _thisGoblin.IsMoving = false;

                        if(_target.GetComponent<Villager>().Hit(transform.position)) {
                            // the hit on the villager was successful (the villager no longer exists)
                            
                            _target = null;
                        }
                    } else if(_target.IsGoblin){
                        // if the target is a goblin, run away instead
                        NextStep(null);
                    }
                }

                _hitTargetNextUpdate = false;
            }

            
            if(DrawDebugLines){
                DrawObservationBoundary();
                DrawTargetLine();
            }
        }
    }

    private void FinishedStep(){

        GoblinSpriteAnimator.Idle();      

        // check to see if the goblin is within reach of the target
        if(_target != null){
            if((_target.transform.position - transform.position).sqrMagnitude <= Reach){
                // the target is within reach!
                _hitTargetNextUpdate = true;

            } else {
                // the target is not within reach, the goblin will need to search for it again
                _findTargetNextUpdate = true;
            }
        } else {
            _findTargetNextUpdate = true;
        }
        
    }

    public void Observe(){
        Observables observables;

        // find all game objects tagged with Observable
        GameObject[] allObservableObjects = GameObject.FindGameObjectsWithTag("Observable");

        // clear the observables list before it gets re-populated
        _observables.Clear();


        // repopulate the observables list with only those observables that are in range
        foreach(GameObject observed in allObservableObjects){

            if(observed != gameObject){
                // ensure that this goblin isn't about to add itself to the list of observations

                if((observed.transform.localPosition - transform.localPosition).sqrMagnitude <= ObservationDistance){
                    // this observable object is close enough to be observed
                    observables = observed.GetComponent<Observables>();
                    if(observables.IsSpawned){
                        _observables.Add(observables);
                    }
                }
            }
        }
    }

    public void FindTarget(){
        
        List<int> goldFilter = new List<int>();
        List<int> villagerFilter = new List<int>();
        List<int> leaderFilter = new List<int>();
        List<int> goblinFilter = new List<int>();

        // first, observe all entities within observation range
        Observe();

        if(_observables.Count == 0){
            // no entities were within range
            NextStep(null);

        } else {
            // filter all the observables

            for(int i = 0; i < _observables.Count; i++){
                if(_observables[i].IsGold){
                    goldFilter.Add(i);
                }
                if(_observables[i].IsVillager){
                    villagerFilter.Add(i);
                }
                if(_observables[i].IsLeader){
                    leaderFilter.Add(i);
                }
                if(_observables[i].IsGoblin){
                    goblinFilter.Add(i);
                }

            }

            // determine what the target should be
            // Target priority is:
            // (1) Gold
            // (2) Villager
            // (3) Leader
            // (4) Goblin

            if(goldFilter.Count > 0){
                // go to the closest source of gold
                _target = _observables[FilterByClosest(goldFilter)];
            } else if(villagerFilter.Count > 0) {
                // go to the closest villager
                _target = _observables[FilterByClosest(villagerFilter)];
            } else if(leaderFilter.Count > 0 && !_thisGoblin.IsLeader) {
                // go to the closest leader (unless this goblin is also a leader)
                _target = _observables[FilterByClosest(leaderFilter)];
            } else {
                // go toward a random goblin
                // leaders don't go toward goblins
                if( _thisGoblin.IsLeader){
                    _target = null;
                } else {
                    _target = _observables[FilterByRandom(goblinFilter)];
                }

            }

            // now that the target has been established, determine how the goblin should get there
            NextStep(_target);
        }
    }

    private void NextStep(Observables target){
        bool isCharging;
        
        // if the target is null, then the goblin takes a random step
        if(target == null){
            _stepEndpoint = _stepSize * RandomHeading(_heading, HeadingVariation) + transform.position;
            isCharging = false;

            // try finding a target next step
            _findTargetNextUpdate = true;
        } else {
            // otherwise, the goblin takes a step to the target's position
            _stepEndpoint = RandomHeading(_heading, HeadingVariation) + target.transform.position;
            isCharging = true;
        }

        // determine the step vector
        _step = _stepEndpoint - transform.position;

        
        _step.Normalize();
        _heading = _step; // the heading vector points in the same direction as the step vector
        _step *= (Random.Range(-SpeedVariation, SpeedVariation) + Speed) * _stepSize;

        // ensure the goblin is moving
        _thisGoblin.IsMoving = true;

        // use the heading vector to change which direction the goblin sprite is facing
        GoblinSpriteAnimator.ChangeHeading(_heading);
        if(isCharging){
            GoblinSpriteAnimator.Charge();
        } else {
            GoblinSpriteAnimator.Hop();
        }
       
    }

    private int FilterByClosest(List<int> filteredIndices){
        float distanceSquared;
        float shortestDistance = Mathf.Infinity;
        int closestObservableIndex = filteredIndices[0];    // default return value

        foreach(int i in filteredIndices){

            // determine the distance (squared) of the observation and this goblin
            distanceSquared = (_observables[i].transform.position - transform.position).sqrMagnitude;

            if(distanceSquared < shortestDistance ){
                // this is the closest position so far
                shortestDistance = distanceSquared;
                closestObservableIndex = i;
            }
        }

        return closestObservableIndex;
    }

    private int FilterByRandom(List<int> filteredIndices){
        return Random.Range(0, filteredIndices.Count - 1);
    }

    public void DrawTargetLine(){
        // draw a line to the target that the goblin will be moving to
        // this line will only need two points
        Vector3[] points = new Vector3[2];

        if(_target != null){
            points[0] = transform.position;
            points[1] = _target.transform.position;

            TargetLine.positionCount = 2;
            TargetLine.SetPositions(points);
        } else {
            TargetLine.positionCount = 0;
        }
    }

    public void DrawObservationBoundary(){
        Vector3[] points = CirclePoints(transform.position, Mathf.Sqrt(ObservationDistance), BoundaryLinePrecision);

        ObservationBoundaryLine.positionCount = BoundaryLinePrecision;
        ObservationBoundaryLine.SetPositions(points);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////// Methods below are functions with no side effects ////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////

    public Vector3 RandomHeading(Vector3 heading, float degreeSpread){
        float theta = Random.Range(-degreeSpread / 2f, degreeSpread / 2f);

        return Quaternion.Euler(0f, 0f, theta) * heading;
    }

    public Vector3[] CirclePoints(Vector3 center, float radius, int nSegments){
        
        float d_theta = 2f * Mathf.PI / nSegments;

        Vector3[] points = new Vector3[nSegments];

        for(int n = 0; n < nSegments; n++){
            points[n] = new Vector3(radius * Mathf.Cos(n * d_theta) + center.x, radius * Mathf.Sin(n * d_theta) + center.y, center.z);
        }
        
        return points;
    }


}
