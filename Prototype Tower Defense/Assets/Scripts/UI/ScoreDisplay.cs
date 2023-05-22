using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _dateDisplay;
    [SerializeField] private GameObject _travelersDisplay;
    [SerializeField] private GameObject _goldDisplay;

    private int _travelers;
    private int _gold;
    private DateTime _scoreDate; 
    

    public void PrintScore(){
        
    }
}
