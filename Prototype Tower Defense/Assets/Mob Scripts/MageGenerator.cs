using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageGenerator : MonoBehaviour
{
    public List<GameObject> templates;
    public List<float> templateProbabilities;
    public List<string> firstNames;
    public List<string> lastNames;
    public MageInterface[] generatedMages;
    public int bufferSize;
    public Vector2 taskDurationLimits;
    public Vector2 idleDurationLimits;

    public void Clear(int index){
        generatedMages[index] = RandomMage();

    }

    private int RandomIndex(){
        float roll = Random.Range(0f, 1f);
        float runningSum = 0f;

        for(int templateIndex = 0; templateIndex < templates.Count; templateIndex++){
            runningSum += templateProbabilities[templateIndex];
            if(roll <= runningSum){
                // this index has been chosen
                return templateIndex;
            }
        }

        // something went terribly wrong, return 0 anyway
        return 0;
    }


    public MageInterface RandomMage(){
        MageInterface mage;
        mage = GameObject.Instantiate(templates[RandomIndex()]).GetComponent<MageInterface>();
       
       // create a new random name
        mage.fullName.Clear();

        mage.fullName.Add(firstNames[Random.Range(0, firstNames.Count - 1)]);
        mage.fullName.Add(lastNames[Random.Range(0, lastNames.Count - 1)]);

        // set a random value for the task duration and idle duration of the mage
        mage.mage.mobController.idleDuration = Random.Range(idleDurationLimits.x, idleDurationLimits.y);
        mage.mage.mobController.taskDuration = Random.Range(taskDurationLimits.x, taskDurationLimits.y);

        return mage;
    }

    public void GenerateMages(){
        for(int i = 0; i < bufferSize; i++){
            generatedMages[i] = RandomMage();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        generatedMages = new MageInterface[bufferSize];
        GenerateMages();
    }
}
