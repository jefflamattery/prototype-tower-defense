using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public Mob _this;
    public TaskBehavior idleBehavior;
    public TaskBehavior moveBehavior;
    public TaskBehavior attackBehavior;
    public TaskBehavior dieBehavior;


    private float _remainingTaskTime;
    private Queue<Task> _tasks;
    private Task _activeTask;
    private bool _taskIsActive;

    public void EnqueueTask(Task task){
        if(_this.isAlive){
            _tasks.Enqueue(task);
        }
    }

    public void Interrupt(){
        Interrupt(new Task());
    }

    public void Interrupt(Task task){
        // interrupt the current task
        if(_taskIsActive){
            GetBehavior(_activeTask).Interrupted(task);
        }

        // clear any current tasks
        ClearAllTasks();

        // immediately make a LeadingUpdate call for this task
        if(_this.isAlive){
            _taskIsActive = true;
            _activeTask = task;
            CallLeadingUpdate();
        }
    }

    public void ClearAllTasks(){
        _tasks.Clear();
        _taskIsActive = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _tasks = new Queue<Task>();
        _remainingTaskTime = 0f;
        _taskIsActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_this.isAlive){

            if(_remainingTaskTime > 0f){
                _remainingTaskTime -= Time.deltaTime;
            } else {
                // the duration of the task has now passed
                _remainingTaskTime = 0f;
                CallTrailingUpdate();

                // call the next task
                NextTask();
            }
        }
    }

    void FixedUpdate(){
        if(_taskIsActive && _this.isAlive){
            GetBehavior(_activeTask).TaskFixedUpdate(_activeTask);
        }
    }

    private void CallLeadingUpdate(){
        if(_taskIsActive && _this.isAlive){
            GetBehavior(_activeTask).LeadingUpdate(_activeTask);
            
            // set the task duration as well
            _remainingTaskTime = _activeTask.duration;
        }
    }

    private void CallTrailingUpdate(){
        if(_taskIsActive && _this.isAlive){
            GetBehavior(_activeTask).TrailingUpdate(_activeTask);
        }
    }

    private TaskBehavior GetBehavior(Task task){
        switch(task.taskID){
            
            case Task.Move:
            return moveBehavior;

            case Task.Attack:
            return attackBehavior;

            case Task.Die:
            return dieBehavior;

            default:
            return idleBehavior;
        }
    }

    private void NextTask(){
        if(_this.isAlive){
            if(_tasks.Count > 0){

                _taskIsActive = true;
                _activeTask = _tasks.Dequeue();
                CallLeadingUpdate();
            } else {
                _taskIsActive = false;
                _activeTask.taskID = Task.Idle;
                _activeTask.duration = 0f;
                _activeTask.position = Vector3.zero;
            }
        }
    }
}

public struct Task{
    
    public const int Idle = 0;
    public const int Move = 1;
    public const int Attack = 2;
    public const int Die = 3;

    public int taskID;
    public float duration;
    public Vector3 position;

}
