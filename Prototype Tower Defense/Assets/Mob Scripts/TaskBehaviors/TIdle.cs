using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIdle : TaskBehavior
{
    public override void LeadingUpdate(Task task)
    {
        _this.mobRenderer.Idle();

        // the mob should not be moving
        if(_this.rigidBody != null){
            _this.rigidBody.velocity = Vector2.zero;
        }

        _this.tasks.ClearAllTasks();

        _this.CreateTask();
    }

}
