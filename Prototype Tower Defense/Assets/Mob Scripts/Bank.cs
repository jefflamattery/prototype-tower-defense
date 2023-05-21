using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{

    public int initialLives;
    public int initialGold;
    private int _lives;
    private int _gold;

    public int Lives{
        get => _lives;
    }

    public int Gold{
        get => _gold;
    }

    public bool SpendLives(int amount){
        if(amount >= _lives){
            _lives -= amount;
            return true;
        } else {
            return false;
        }
    }

    public bool SpendGold(int amount){
        if(amount <= _gold){
            _gold -= amount;
            return true;
        } else {
            return false;
        }
    }

    public void Deposit(BankNote deposit){
        _lives += deposit.Lives;
        _gold += deposit.Gold;
        deposit.Erase();
    }

    public BankNote WithdrawGold(){
        // withdraw all gold from this bank
        return Withdraw(0, _gold);
    }

    public BankNote Withdraw(int lives, int gold){
        if(lives > _lives){
            // can't withdraw that many lives
            lives = 0;
        } else {
            _lives -= lives;
        }

        if(gold > _gold){
            // can't withdraw that much gold
            gold = 0;
        } else {
            _gold -= gold;
        }
        return new BankNote(lives, gold);
    }

    // Start is called before the first frame update
    void Start()
    {
        _lives = initialLives;
        _gold = initialGold;
    }
}


public class BankNote{
    private int _lives;
    private int _gold;

    public int Lives{
        get => _lives;
    }

    public int Gold{
        get => _gold;
    }

    public BankNote(int lives, int gold){
        _lives = lives;
        _gold = gold;
    }

    public BankNote(){
        _lives = 0;
        _gold = 0;
    }

    public void Erase(){
        _lives = 0;
        _gold = 0;
    }

}
