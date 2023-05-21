using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{

    public Mob mob;
    public TaskManager mobManager;
    public bool seeksWaypoints;
    public List<Waypoint> waypointList;
    private Queue<Waypoint> _waypoints;

    public bool seeksGold;
    public bool seeksVillagers;
    public bool seeksLeaders;
    public bool seeksGoblins;

    public float reach;
    public float taskDuration;
    public float idleDuration;

    private Vector3 _targetAzimuth;
    private Mob _target;
    
    public Mob Target{
        get => _target;
    }

    public bool TargetIs(int filter){
        
        if (_target == null){
            return false;
        }

        // returns true if this mob's target matches the filter
        switch(filter){
            case Mob.Gold_Filter:
            return _target.isGoblin;

            case Mob.Goblin_Filter:
            return _target.isGoblin;

            case Mob.Villager_Filter:
            return _target.isVillager;

            case Mob.Leader_Filter:
            return _target.isLeader;

            default:
            return false;
        }
    }

    public bool TargetIsMoving(){
        return _target.isMoving;
    }

    public void SendIdleTask(){
        Task idle = new Task();

        idle.taskID = Task.Idle;
        idle.duration = idleDuration;
        mobManager.EnqueueTask(idle);
    }

    public void SendTask(){
        mobManager.EnqueueTask(BuildTask());
    }

    public void SendTask(Mob target){
        mobManager.EnqueueTask(BuildTask(target));
    }


    // Build Task: If a target isn't supplied, then SetMobTarget will look for one
    public Task BuildTask(){
        return BuildTask(null);
    }

    public Task BuildTask(Mob target){
        Task task = new Task();

        task.duration = taskDuration;


        if(seeksWaypoints && _waypoints.Count > 0){
            if((_waypoints.Peek().transform.position - transform.position).magnitude <= reach){
                
                // the mob has reached the waypoint, remove the waypoint from the queue
                _waypoints.Dequeue();

                if(_waypoints.Count == 0){
                    // the waypoint that just got dequeued was the last waypoint
                    // add the mob's gold to the player's gold
                    MapManager.Instance.PlayerBank.Deposit(mob.mobBank.WithdrawGold());

                    // remove the villager
                    mob.isAlive = false;

                } else {
                    // go to the next waypoint
                    task.taskID = Task.Move;
                    task.position = _waypoints.Peek().transform.position - transform.position;
                }

            } else {
                // move toward waypoint
                task.taskID = Task.Move;
                task.position = _waypoints.Peek().transform.position - transform.position;
            }
        } else if (SetMobTarget(target)) {
            // the target azimuth is currently pointing to the mob's target
            if(_targetAzimuth.magnitude <= reach){
                // the target is within reach
                task.taskID = Task.Attack;
                task.position = _targetAzimuth;
            } else {
                // the target is not within reach
                task.taskID = Task.Move;
                task.position = _targetAzimuth;
            }
        } else {
            // default (idle)
            task.taskID = Task.Idle;
            task.duration = idleDuration;
        }

        return task;
    }



    public bool SetMobTarget(Mob target){
        // in the first pass, try to find a target if the target is null
        if(target == null){
            target = FindMobTarget();
        }

        // if the target is still null, then there isn't a target to find
        // so this mob won't have a target for this task
        if(target != null){
            _targetAzimuth = target.transform.position - transform.position;
            _target = target;
            return true;
        } else {
            _targetAzimuth = Vector3.zero;
            _target = null;
            return false;
        }

    }


    public Mob FindMobTarget(){
        mob.MakeObservation();

        // look for any targets matching the various filters
        if(seeksGold && mob.TargetsMatchingFilter(Mob.Gold_Filter) > 0){
            // gold target(s) found
            return mob.FindClosestTarget(Mob.Gold_Filter);

        } else if(seeksVillagers && mob.TargetsMatchingFilter(Mob.Villager_Filter) > 0){
            // villager target(s) found
            return mob.FindClosestTarget(Mob.Villager_Filter);

        } else if(seeksLeaders && mob.TargetsMatchingFilter(Mob.Leader_Filter) > 0){
            // leader target(s) found
            return mob.FindClosestTarget(Mob.Leader_Filter);

        } else if(seeksGoblins && mob.TargetsMatchingFilter(Mob.Goblin_Filter) > 0){
            // goblin target(s) found
            return mob.FindRandomTarget(Mob.Goblin_Filter);
        } else {
            return null;
        }
    
    }

    public void AddWaypoint(Waypoint waypoint){
        _waypoints.Enqueue(waypoint);
    }


    // Start is called before the first frame update
    void Start()
    {
        _waypoints = new Queue<Waypoint>();

        foreach(Waypoint w in waypointList){
            AddWaypoint(w);
        }
    }



}
