using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    public const int No_Filter = 0;
    public const int Gold_Filter = 1;
    public const int Villager_Filter = 2;
    public const int Leader_Filter = 3;
    public const int Goblin_Filter = 4;
    public const int Number_of_Filters = 5;

    public Rigidbody2D rigidBody;
    public MobRenderer mobRenderer;
    public MobController mobController;
    public Bank mobBank;

    public TaskManager tasks;

    public bool isGoblin;
    public bool isGold;
    public bool isVillager;
    public bool isLeader;

    public bool isAlive;
    public bool isMoving{
        get => rigidBody.velocity.magnitude > 0f;
    }

    public float globalSpeedMultiplier;

    private List<Mob> _observed;
    public float _observationDistance;
    public string _observableTag;

    private List<int>[] _filterHistogram;
    private Vector3 _heading;
    public Vector3 Heading{
        get => _heading;
        set => _heading = value;
    }

    private Vector3 _rangedHeading;
    public Vector3 RangedHeading{
        get => _rangedHeading;
        set => _rangedHeading = value;
    }

    
    public GameObject _observationBoundingRectangle;

    // CreateTask() is used to spawn a new mob, and to create a new task when the mob doesnt have any
    public void CreateTask(){
        if(!isAlive){
            // this mob just became alive
            isAlive = true;
            mobRenderer.Show();
        }
        mobController.SendTask();  // this starts the AI task manager
    }

    public void StopTasks(){
        // first set isAlive to false to stop the task manager from running
        isAlive = false;

        // then interrupt the task manager, this will also clear all tasks
        tasks.Interrupt();

    }

    public void Hit(Vector3 source){
        Task hit;
        hit.taskID = Task.Die;
        hit.duration = 0f;
        hit.position = source;

        tasks.Interrupt(hit);
    }

    public void Hit(){
        Hit(Random.insideUnitCircle);
    }

    public List<Mob> AllMobsMatchingFilter(int filter){
        List<Mob> matches = new List<Mob>();

        MakeObservation();

        foreach(int observedIndex in _filterHistogram[filter]){
            matches.Add(_observed[observedIndex]);
        }

        return matches;
    }

    public Mob FindClosestTarget(int filter){
        float shortestDistance = Mathf.Infinity;
        float squareDistance;
        int closestIndex;

        MakeObservation();

        if(_filterHistogram[filter].Count > 0){

            closestIndex = _filterHistogram[filter][0]; // default value
            
            // look for which target matching this filter is the closest
            for(int i = 0; i < _filterHistogram[filter].Count; i++){
                squareDistance = (_observed[_filterHistogram[filter][i]].transform.position - transform.position).sqrMagnitude;
                if(squareDistance < shortestDistance){
                    shortestDistance = squareDistance;
                    closestIndex = _filterHistogram[filter][i];
                }

            }
            return _observed[closestIndex];

        } else {
            return null;
        }
    }

    public Mob FindRandomTarget(int filter){
        MakeObservation();
        int randomIndex;
        List<int> indicesMatchingFilter = _filterHistogram[filter];



        if(_filterHistogram[filter].Count > 0){
            randomIndex = Random.Range(0, _filterHistogram[filter].Count - 1);
            return _observed[_filterHistogram[filter][randomIndex]];
        } else {
            return null;
        }
    }

    public int TargetsMatchingFilter(int filter){
        return _filterHistogram[filter].Count;
    }


    public void ClearFilterHistogram(){
        for(int i = 0; i < _filterHistogram.Length; i++){
            _filterHistogram[i].Clear();
        }
    }

    public void MakeObservation(){
        MakeObservation(transform.position, _observationDistance, _observationBoundingRectangle);
    }

    public void MakeObservation(Vector3 referencePoint, float observationDistance, GameObject boundary){
        Mob target;
        GameObject[] mobs = GameObject.FindGameObjectsWithTag(_observableTag);
        int observedIndex = 0;

        // the maximum vector holds the highest x and y values a mob can have and still be within the boundary
        // the minimum vector holds the smallest x a nd y values a mob can have and still be within the boundary
        Vector2 maximum = new Vector2(boundary.transform.position.x + boundary.transform.localScale.x / 2f,
                                        boundary.transform.position.y + boundary.transform.localScale.y / 2f);
        Vector2 minimum = new Vector2(boundary.transform.position.x - boundary.transform.localScale.x / 2f,
                                        boundary.transform.position.y - boundary.transform.localScale.y / 2f);                

        // re-create the observed array
        _observed.Clear();
        ClearFilterHistogram();

        // to be observed, the mob must be:
        // close enough
        // alive
        // not itself (a mob shouldn't observe itself)
        for(int i = 0; i < mobs.Length; i++){
            // first, check to see if the mob is within the observation boundary
            if(
                mobs[i].transform.position.y <= maximum.y &&
                mobs[i].transform.position.y >= minimum.y &&
                mobs[i].transform.position.x <= maximum.x &&
                mobs[i].transform.position.x >= minimum.x
            ) {

                if( mobs[i] != gameObject && (mobs[i].transform.position - referencePoint).magnitude <= observationDistance) {
                    // the mob is close enough to be observed
                    target = mobs[i].GetComponent<Mob>();

                    // keep track of the actual Interaction object
                    _observed.Add(target);
                    observedIndex = _observed.Count - 1;

                    if(target.isAlive){
                        // update the filter histogram
                        if(target.isGold){
                            _filterHistogram[Gold_Filter].Add(observedIndex);
                        }
                        
                        if(target.isVillager){
                            _filterHistogram[Villager_Filter].Add(observedIndex);
                        }
                        
                        if(target.isLeader){
                            _filterHistogram[Leader_Filter].Add(observedIndex);
                        }
                        
                        if(target.isGoblin){
                            _filterHistogram[Goblin_Filter].Add(observedIndex);
                        }
                        // add this index to the default filter
                        _filterHistogram[No_Filter].Add(observedIndex);
                    }
                }
            }
        }
    }

    public string FilterHistogramToString(){
        return "All observables: " + _filterHistogram[No_Filter].Count + 
        " Villagers: " + _filterHistogram[Villager_Filter].Count +
        " Goblins: " + _filterHistogram[Goblin_Filter].Count +
        " Gold: " + _filterHistogram[Gold_Filter].Count +
        " Leaders: " + _filterHistogram[Leader_Filter].Count;

    }


    // Start is called before the first frame update
    void Start()
    {
        _observed = new List<Mob>();
         _filterHistogram = new List<int>[Number_of_Filters];    // one list for each filter type
         
         // create a list of integers for each filter in the filter histogram array
        for(int i = 0; i < Number_of_Filters; i++){
            _filterHistogram[i] = new List<int>();
        }
    }

}
