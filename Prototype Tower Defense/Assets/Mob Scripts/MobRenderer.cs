using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRenderer : MonoBehaviour
{
    public Animator spriteAnimator;
    public SpriteRenderer spriteRenderer;
    public TrailRenderer trailRenderer;

    public string _directionParameterName;
    public string _idleStateName;
    public string _walkStateName;
    public string _runStateName;
    public string _attackStateName;
    public string _dieStateName;

    public float _idleAnimationSpeed;
    public float _attackAnimationSpeed;
    public float _dieAnimationSpeed;
    public float _distanceWithoutElevationChange;

    public string _deadLayerName;

    private Vector3[] _compassBasis;


    public void Hide(){
        spriteRenderer.enabled = false;
    }

    public void Show(){
        spriteRenderer.enabled = true;
        if(trailRenderer != null){
            trailRenderer.enabled = true;
        }
    }

    public void Idle(){
        spriteAnimator.speed = _idleAnimationSpeed;
        spriteAnimator.Play(_idleStateName);
    }

    public void Walk(float cyclesPerSecond){
        spriteAnimator.speed = cyclesPerSecond;
        spriteAnimator.Play(_walkStateName);
    }

    public void Run(float cyclesPerSecond){
        spriteAnimator.speed = cyclesPerSecond;
        spriteAnimator.Play(_runStateName);
    }
    
    public void Attack(){
        spriteAnimator.speed = _attackAnimationSpeed;
        spriteAnimator.Play(_attackStateName);
    }

    public void Die(){
        spriteAnimator.speed = _dieAnimationSpeed;
        spriteAnimator.Play(_dieStateName);

        // also put this sprite on the Dead sorting layer
        spriteRenderer.sortingLayerName = _deadLayerName;
    }

    public void ChangeElevation(Vector3 position){
        spriteRenderer.sortingOrder = SortingOrder(position);
    }

    public int SortingOrder(Vector3 position){
        return Mathf.RoundToInt( -position.y / _distanceWithoutElevationChange);

    }
    

     public bool ChangeHeading(Vector3 heading){
        // project the heading vector onto each basis vector, and determine which projection is the largest
        // the closer the basis vector is to the heading, the larger their dot product will be

        float largestDotProduct = 0f;
        float dotProduct;
        int closestBasisIndex = 0;
        int lastClosestBasisIndex = spriteAnimator.GetInteger(_directionParameterName);


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
        spriteAnimator.SetInteger(_directionParameterName, closestBasisIndex);

        // this method only returns true if the heading change was enough to cause a direction change
        return lastClosestBasisIndex != closestBasisIndex;
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
        BuildCompassBasis();
    }
}
