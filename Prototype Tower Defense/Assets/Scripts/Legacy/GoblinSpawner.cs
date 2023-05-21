using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSpawner : MonoBehaviour
{

    public Goblin GoblinTemplate;
    public Goblin GoblinLeaderTemplate;
    public int GoblinCount;
    public int LeaderCount;
    public Vector2 SpawnZoneSize;

    private List<Goblin> _goblins;

    public void SpawnGroup(){

        Vector3 randomPosition; 

        foreach(Goblin goblin in _goblins){
            randomPosition = new Vector3(0f, 0f, 0f);

            randomPosition.x = Random.Range(-SpawnZoneSize.x / 2f, SpawnZoneSize.x / 2f);
            randomPosition.y = Random.Range(-SpawnZoneSize.y / 2f, SpawnZoneSize.y / 2f);

            //goblin.transform.position = randomPosition;

            // spawn the goblin
            goblin.Spawn();
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Goblin goblin;
        _goblins = new List<Goblin>();
        
        
        for(int i = 0; i < LeaderCount; i++){
            // first, add goblin leaders to the goblin list
            goblin = GameObject.Instantiate(GoblinLeaderTemplate).GetComponent<Goblin>();

            _goblins.Add(goblin);

            goblin.SetSpawn(transform.position);
        }

        for(int i = 0; i < GoblinCount; i++){
            goblin = GameObject.Instantiate(GoblinTemplate).GetComponent<Goblin>();
            // use the goblin template to create a new object, and add the goblin component to the list of goblins
            _goblins.Add(goblin);

            // also, set the spawn location of each goblin
            goblin.SetSpawn(transform.position);
        }
    }

    void Update(){

    }

}
