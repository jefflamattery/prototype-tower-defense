using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public Leaderboard leaderboard;
    public int leaderboardSize;

    void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);

        if (!Directory.Exists(Application.persistentDataPath + "/HighScores/")){
            Directory.CreateDirectory(Application.persistentDataPath + "/HighScores/");
        }
    }

     public void SaveScores(List<ScoreRecord> scoreList)
    {
        leaderboard.list = scoreList;
        XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
        FileStream stream = new FileStream(Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Create);
        serializer.Serialize(stream, leaderboard);
        stream.Close();
    }

    public List<ScoreRecord> LoadScores()
    {
        if (File.Exists(Application.persistentDataPath + "/HighScores/highscores.xml"))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
            FileStream stream = new FileStream(Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Open);
            leaderboard = serializer.Deserialize(stream) as Leaderboard;
        }
        return leaderboard.list;
    }

    public ScoreRecord Record(int travelers, int gold){
        ScoreRecord score = new ScoreRecord(travelers, gold);
        
        // load the scores from file, and add this score to the list
        LoadScores().Add(score);

        // sort the list and remove the lowest score if there are too many
        leaderboard.list.Sort((ScoreRecord n, ScoreRecord m) => n.score.CompareTo(m.score));

        if(leaderboard.list.Count > leaderboardSize){
            leaderboard.list.RemoveAt(leaderboardSize);
        }

        return score;
    }

}

[System.Serializable]
public class Leaderboard{
    public List<ScoreRecord> list = new List<ScoreRecord>();

    public void Sort(){

    }
}


public class ScoreRecord {
    public DateTime date;
    public int travelers;
    public int gold;
    public int score;

    public ScoreRecord(int nTravelers, int nGold){
        date = System.DateTime.Now;
        travelers = nTravelers;
        gold = nGold;
        score = Mathf.RoundToInt(Mathf.Sqrt(travelers * travelers + gold * gold));
    }
}
