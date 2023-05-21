using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject _mobTemplate; // i.e. a villager
    public int _totalMobs;
    public int _mobsPerSpawn;
    public float _meanSpawnTime;
    public float _variance;
    public float _spawnRadius;
    public bool _spawnOnWake;

    private bool _isSpawning;
    private float _timeUntilSpawn;

    public Spawner _nextSpawner;
    public float _triggerNextSpawnChance;

    private Queue<Mob> _mobs;


    public void SpawnAll(){
        _isSpawning = true;
        _timeUntilSpawn = 0f;
    }

    public void Spawn(){
        // spawns the next mob set of mobs in the queue
        Mob m;
        Vector2 randomOffset;

        TriggerNextSpawner();

        for(int i = 0; i < _mobsPerSpawn; i++){
            if(_mobs.Count > 0){
                randomOffset = _spawnRadius * Random.insideUnitCircle;
                m = _mobs.Dequeue();
                m.transform.position = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
                m.CreateTask();

            } else {
                // no more mobs to spawn
                _isSpawning = false;
            }
        }
    }

    public void TriggerNextSpawner(){
        if(_nextSpawner != null && Random.Range(0f, 1f) <= _triggerNextSpawnChance){
            // active the next spawner in line
            _nextSpawner.SpawnAll();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _mobs = new Queue<Mob>();

        // hide the template
        _mobTemplate.GetComponentInChildren<MobRenderer>().Hide();   

        for(int i = 0; i < _totalMobs; i++){
            _mobs.Enqueue(GameObject.Instantiate(_mobTemplate).GetComponent<Mob>());
        }

        if(_spawnOnWake){
            SpawnAll();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(_isSpawning){
            _timeUntilSpawn -= Time.deltaTime;

            if(_timeUntilSpawn <= 0f){
                // it's time to spawn a new group of mobs
                Spawn();

                // choose a new random time to spawn the next mob
                _timeUntilSpawn = Random.Range(-1f, 1f) * _variance + _meanSpawnTime;
            } 
        }
    }
}
