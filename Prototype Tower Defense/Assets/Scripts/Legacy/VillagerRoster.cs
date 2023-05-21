using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerRoster : MonoBehaviour
{
    public GameObject VillagerTemplate;
    public float ExpectedSpawnTime;
    public float Variation;

    public int BufferCount;

    public int _villagersSpawned;

    private List<Villager> _villagers;
    private int _nextVillagerIndex;
    private bool _isSendingVillagers;
    private float _timeUntilNextVillager;



    public void StartVillagers(){
        _isSendingVillagers = true;
    }

    void SpawnVillager(){
        _villagers[_nextVillagerIndex].Spawn();
        
        // reset the time until next villager to a new random value
        _timeUntilNextVillager = Random.Range(ExpectedSpawnTime - Variation, ExpectedSpawnTime + Variation);
        _nextVillagerIndex++;
        _nextVillagerIndex %= _villagers.Count;
        _villagersSpawned++;
    }

    // Start is called before the first frame update
    void Start()
    {
        _villagers = new List<Villager>();
        _villagersSpawned = 0;
        _timeUntilNextVillager = 0f;
        _isSendingVillagers = false;

        for(int n = 0; n < BufferCount; n++){
            // create a new villager object and keep track of it in the _villagers list
            _villagers.Add(GameObject.Instantiate(VillagerTemplate).GetComponent<Villager>());
        }
    }

    void Update(){
        if(_isSendingVillagers){
            _timeUntilNextVillager -= Time.deltaTime;
            if(_timeUntilNextVillager <= 0f){
                // it's time to spawn a new villager
                SpawnVillager();
            }
        }
        
    }
}
