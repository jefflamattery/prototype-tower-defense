using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceExplosion : MonoBehaviour
{
    public Mob _this;
    public List<SpriteRenderer> _allSprites;
    
    public float _duration;
    public float _speedMultiplier;
    private float _timeRemaining;

    private bool _isVisible;

    public void ToggleSprites(bool enable){
        _isVisible = enable;

        foreach(SpriteRenderer r in _allSprites){
            r.enabled = enable;
        }

        if(enable){
            _timeRemaining = _duration;
        }
    }

    public void SetSortingOrder(int sortingOrder){
        foreach(SpriteRenderer r in _allSprites){
            r.sortingOrder = sortingOrder;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // make sure all sprites are turned off
        ToggleSprites(false);
        
    }

    void Update(){
        int sortingOrder;
        if(_isVisible){
            _timeRemaining -= Time.deltaTime;
            if(_timeRemaining > 0f && _this.isAlive){
                sortingOrder = _this.mobRenderer.spriteRenderer.sortingOrder + 1;
                foreach(SpriteRenderer r in _allSprites){
                    r.sortingOrder = sortingOrder;
                }
            } else {
                // out of time
                ToggleSprites(false);
                _isVisible = false;
                _this.globalSpeedMultiplier = 1f;
            }
        }
    }

}
