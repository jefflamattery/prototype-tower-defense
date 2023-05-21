using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBehavior : MonoBehaviour
{
    public Mob _this;

    public virtual void LeadingUpdate(Task task){}

    public virtual void TrailingUpdate(Task task){
        // create a new task when this task is done
        if(_this.isAlive){
            _this.CreateTask();
        }
    }

    public virtual void TaskFixedUpdate(Task task){}

    public virtual void Interrupted(Task task){}


    // Start is called before the first frame update
    void Start()
    {
        
    }

}
