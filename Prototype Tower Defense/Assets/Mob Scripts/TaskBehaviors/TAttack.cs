using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAttack : TaskBehavior
{

    public bool _attacksGoblins;
    public bool _attacksVillagers;
    
    public override void LeadingUpdate(Task task)
    {
        Vector3 heading;
        Mob target = _this.mobController.Target;


        if(target != null){

            if((target.isGoblin && _attacksGoblins) || (target.isVillager && _attacksVillagers)){

                // face the target
                heading = target.transform.position - _this.transform.position;
                heading.Normalize();

                _this.Heading = heading;
                _this.mobRenderer.ChangeHeading(heading);

                // make sure that this mob is no longer moving
                _this.rigidBody.velocity = Vector2.zero;

                // play the attack animation
                _this.mobRenderer.Attack();

                // hit the target if it's close enough
                if((_this.transform.position - target.transform.position).magnitude <= _this.mobController.reach){
                    target.Hit(_this.transform.position - target.transform.position);
                }
            }
        }
    }

    public override void TrailingUpdate(Task task)
    {
        base.TrailingUpdate(task);

        
    }
}
