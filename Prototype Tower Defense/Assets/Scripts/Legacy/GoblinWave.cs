using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinWave : MonoBehaviour
{
    public List<GoblinSpawner> SpawnersInWave;

    public void SpawnWave(){
        foreach(GoblinSpawner spawner in SpawnersInWave){
            spawner.SpawnGroup();
        }
    }
}
