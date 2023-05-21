using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{

    public Animator _animator;
    public string _directionParameterName;
    public string _idleStateName;
    public string _hopStateName;
    public string _chargeStateName;
    public string _dieStateName;
    public string _attackStateName;
    private Vector3[] _compassBasis;

    private bool _isIdle;

    private bool _allowInterrupts;
    private float _blockTimeRemaining;
    
    public void SetSpeed(float speed, float cyclesPerMeter){
        _animator.speed = speed * cyclesPerMeter;
    }

    public void BlockInterrupts(float time){
        _allowInterrupts = false;
        _blockTimeRemaining = time;
    }
   

    public bool ChangeHeading(Vector3 heading){
        // find the dot product of the heading with each basis vector
        // the closer the basis vector is to the heading, the larger their dot product will be

        float largestDotProduct = 0f;
        float dotProduct;
        int closestBasisIndex = 0;
        int lastClosestBasisIndex = _animator.GetInteger(_directionParameterName);


        if(_compassBasis == null){
            BuildCompassBasis();
        }

        for(int i = 0; i < _compassBasis.Length; i++){
            dotProduct = Vector3.Dot(heading, _compassBasis[i]);
            if(dotProduct > largestDotProduct){
                // this is the largest dot product so far
                largestDotProduct = dotProduct;
                closestBasisIndex = i;
            }
        }


        // now that the closest basis vector has been determined, 
        // animate the sprite so that it faces that direction
        _animator.SetInteger(_directionParameterName, closestBasisIndex);

        // check to see if the direction changed from where it was previously
        return lastClosestBasisIndex != closestBasisIndex;
    }

    public void Attack(float duration){
        if(_allowInterrupts){
            _isIdle = false;
            _animator.speed = 1f / duration;
            _animator.Play(_attackStateName, 0);
            BlockInterrupts(duration);
        }
    }

    public void Hop(){
        if(_allowInterrupts){
            _isIdle = false;
            _animator.Play(_hopStateName, 0);
        }
    }

    public void Idle(){
        if(!_isIdle && _allowInterrupts){
            _animator.speed = 1f;
            _animator.Play(_idleStateName, 0);
            _isIdle = true;
        }
    }

    public void Charge(){
        if(_allowInterrupts){
            _isIdle = false;
            _animator.Play(_chargeStateName, 0);
        }
    }

    public void Die(){
        _isIdle = false;
        _animator.Play(_dieStateName, 0);
    }
    void BuildCompassBasis(){
        float theta;
        // the compass basis consists of 8 unit vectors that point in the cardinal and ordinal directions
        _compassBasis = new Vector3[8];

        for(int n = 0; n < 8; n++){
            theta = (n / 4f) * Mathf.PI;
            _compassBasis[n] = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _isIdle = false;
        BuildCompassBasis();
        _allowInterrupts = true;
        _blockTimeRemaining = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_allowInterrupts){
            // interrupts are currently being blocked, count down the amount of time remaining
            _blockTimeRemaining -= Time.deltaTime;
            if(_blockTimeRemaining <= 0f){
                _allowInterrupts = true;
                _blockTimeRemaining = 0f;
            }
        }
    }
}
