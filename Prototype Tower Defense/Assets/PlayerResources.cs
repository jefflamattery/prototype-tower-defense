using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
   
   public int _gold;
   public int _villagers;

   public void AddGold(int gold){
    _gold += gold;
   }

   public void AddVillagers(int amount){
    _villagers += amount;
   }
}
