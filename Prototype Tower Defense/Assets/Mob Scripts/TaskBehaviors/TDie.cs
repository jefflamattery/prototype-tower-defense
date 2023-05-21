using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDie : TaskBehavior
{
    public override void LeadingUpdate(Task task)
    {
        // face away from the direction of the hit
        _this.mobRenderer.ChangeHeading(task.position);
        
        // stop the mob from moving
        _this.rigidBody.velocity = Vector2.zero;

        // play the death animation
        _this.mobRenderer.Die();

        _this.tasks.ClearAllTasks();
        _this.isAlive = false;

    }

    public override void TrailingUpdate(Task task)
    {
        base.TrailingUpdate(task);

    }
}
