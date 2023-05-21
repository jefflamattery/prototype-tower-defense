using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMove : TaskBehavior
{
    public float _walkSpeed;
    public float _runSpeed;
    public float _speedVariance;

    public float _walkMetersPerAnimationCycle;
    public float _runMetersPerAnimationCycle;

    public float _maximumAngleDeviation;
    public string _colliderTagName;
    public float _collisionBounceDistance;

    public List<PropRenderer> _colliders;


    void Start(){
        BuildColliderList();
    }

    public void BuildColliderList(){
        GameObject[] colliderGameObjects = GameObject.FindGameObjectsWithTag(_colliderTagName);

        _colliders = new List<PropRenderer>();

        for(int i = 0; i < colliderGameObjects.Length; i++){
            _colliders.Add(colliderGameObjects[i].GetComponent<PropRenderer>());
        }
    }


    public override void LeadingUpdate(Task task)
    {
        Vector3 heading;
        float speed;

        // determine what direction this mob is moving in
        heading = RandomHeading(task.position, _maximumAngleDeviation);



        // ensure the heading is unit length, so that its magnitude doesn't scale the velocity vector
        heading.Normalize();

        // set the heading of this mob
        _this.mobRenderer.ChangeHeading(heading);
        _this.Heading = heading;


        // determine how fast the mob is moving
        // if the target is a villager or a leader, then run. otherwise, walk
        if(_this.mobController.TargetIs(Mob.Villager_Filter) || _this.mobController.TargetIs(Mob.Leader_Filter)){
            // run!
            speed = _this.globalSpeedMultiplier * (_runSpeed + Random.Range(-_speedVariance, _speedVariance));
            _this.rigidBody.velocity =  speed * heading;
            _this.mobRenderer.Run(speed / _runMetersPerAnimationCycle);

        } else {
            // walk!
            speed = _this.globalSpeedMultiplier * (_walkSpeed + Random.Range(-_speedVariance, _speedVariance));
            _this.rigidBody.velocity =  speed * heading;
            _this.mobRenderer.Walk(speed / _walkMetersPerAnimationCycle);
        }
    }

    public override void TaskFixedUpdate(Task task)
    {
        Task bounce;
        bounce.position = Vector3.zero;

        base.TaskFixedUpdate(task);
        bool collisionDetected = false;

        // change the sorting order of this sprite based on its vertical position on the screen
        // sprites at a LOWER verticla position should be ABOVE other sprites
        _this.mobRenderer.ChangeElevation(_this.transform.position);

        foreach(PropRenderer collider in _colliders){
            if(collider.IsColliding(_this.transform.position)){
                // a collision has been detected!
                collisionDetected = true;
                bounce.position = _this.transform.position - collider.transform.position;
            }
        }

        if(collisionDetected){
            // create a new task to tell the mob to move in the opposite direction
            task.position.Normalize();

            bounce.taskID = Task.Move;
            bounce.duration = task.duration;

            _this.tasks.Interrupt(bounce);
            
        }

    }

    
    public Vector3 RandomHeading(Vector3 heading, float maximumAngleDeviation){
        float theta = Random.Range(-maximumAngleDeviation, maximumAngleDeviation);

        return Quaternion.Euler(0f, 0f, theta) * heading;
    }

}

