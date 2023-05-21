using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBankUI : MonoBehaviour
{

    public TextMeshProUGUI _goldAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _goldAmount.text = MapManager.Instance.PlayerBank.Gold.ToString();
    }
}
