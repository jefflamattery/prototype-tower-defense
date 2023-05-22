using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalUI : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI _villagerCount;
    [SerializeField] private TextMeshProUGUI _villagerSaved;
    [SerializeField] private TextMeshProUGUI _goldCount;

    [SerializeField] private string villagerCountLabel;
    [SerializeField] private string villagersSavedLabel;
    [SerializeField] private string goldLabel;
    

    // Update is called once per frame
    void Update()
    {
        _villagerCount.text = MapManager.Instance.PlayerBank.Lives + villagerCountLabel;
        _villagerSaved.text = MapManager.Instance.VillagersSaved + villagersSavedLabel;

        _goldCount.text = MapManager.Instance.PlayerBank.Gold + goldLabel;

    }
}
