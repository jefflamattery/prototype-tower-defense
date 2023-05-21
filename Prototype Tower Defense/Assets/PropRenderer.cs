using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRenderer : MonoBehaviour
{
    public GameObject _propCollider;
    public SpriteRenderer _propSprite;
    public float _distanceWithoutElevationChange;

    public float _collisionRadius;


    public void SetSortingLayer(){
        _propSprite.sortingOrder = SortingOrder(_propCollider.transform.position);
    }

    private int SortingOrder(Vector3 position){
        return Mathf.RoundToInt( -position.y / _distanceWithoutElevationChange);
    }

    public bool IsColliding(Vector3 mobPosition){
        return (_propCollider.transform.position - mobPosition).magnitude <= _collisionRadius;
    }

    // Start is called before the first frame update
    void Start()
    {
        // determine the collision radius from how the collider was scaled
        _collisionRadius = _propCollider.transform.localScale.x / 2f;

        // hide the collider sprite
        _propCollider.GetComponent<SpriteRenderer>().enabled = false;

        SetSortingLayer();
    }
}
