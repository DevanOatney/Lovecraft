using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TRAP_TYPES
{
    SLOW = 0,
    FLASH,
    STUN,
    TURRET,
    LAUNCHER,
}


[System.Serializable]
public struct TrapUnlock
{
    public bool unlocked;
    public TRAP_TYPES type;
    public int costToUnlock;

    public TrapUnlock(bool _unlocked, TRAP_TYPES _type, int _cost)
    {
        unlocked = _unlocked;
        type = _type;
        costToUnlock = _cost;
    }

    public void Unlock()
    {
        unlocked = true;
    }
}

public class UnlockedTrapsManager : MonoBehaviour
{
    public List<TrapUnlock> Traps;
    PlayerCurrencyController PCC;

    private void Start()
    {
        PCC = GameObject.FindObjectOfType<PlayerCurrencyController>();
    }

    public bool AttemptToUnlockTrap(TRAP_TYPES _type)
    {
        if( Traps.Count > (int)_type)
        {
            if( !Traps[(int)_type].unlocked )
            {
                if(PCC.SpendBloodCurrency(Traps[(int)_type].costToUnlock))
                {
                    TrapUnlock tu = Traps[(int)_type];
                    tu.unlocked = true;
                    Traps[(int)_type] = tu;
                    return true;
                }
            }
        }
        return false;
    }
}
