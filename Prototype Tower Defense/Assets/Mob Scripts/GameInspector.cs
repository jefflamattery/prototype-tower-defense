using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInspector : MonoBehaviour
{
    public static GameInspector instance;

    public GameObject _gameOverWindow;

    [SerializeField] private string _observableTag;

    void Awake(){
        instance = this;
    }

    void Update()
    {
        GameObject[] entities;
        Mob m;
        bool foundVillager = false;

        // the game is over if the player has run out of lives (travelers) AND there are no living villagers
        if(MapManager.Instance.PlayerBank.Lives <= 0){
            entities = GameObject.FindGameObjectsWithTag(_observableTag);

            foreach(GameObject entity in entities){
                m = entity.GetComponent<Mob>();

                if(m.isVillager && m.isAlive){
                    // there is still a living villager, the game is not over yet
                    foundVillager = true;
                    break;
                }
            }

            if(!foundVillager){
                // a villager wasn't found, game over
                GameOver();
            }
        }
    }

    public void GameOver(){

        // pause the game
        MapManager.Instance.PauseGame();

        // record the score
        ScoreRecord score = ScoreManager.instance.Record(MapManager.Instance.VillagersSaved, MapManager.Instance.PlayerBank.Gold);

        // show the game over window
        Debug.Log("Game Over! Travelers Safe = " + score.travelers + " Gold = " + score.gold + " Score = " + score.score);

    }
}
