using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    public SpriteRenderer WaypointIcon;

    // Start is called before the first frame update
    void Start()
    {
        // hide any sprite renderers that this waypoint has
        WaypointIcon.enabled = false;
    }

    
}
