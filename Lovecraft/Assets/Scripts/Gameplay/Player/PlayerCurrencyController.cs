using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurrencyController : MonoBehaviour
{
    public int Blood_Currency_Count = 0;


    public bool SpendBloodCurrency(int amount)
    {
        if( Blood_Currency_Count - amount >= 0 )
        {
            Blood_Currency_Count -= amount;
            return true;
        }
        return false;
    }

    public void AddBloodCurrency(int amount)
    {
        Blood_Currency_Count += amount;
    }
}
