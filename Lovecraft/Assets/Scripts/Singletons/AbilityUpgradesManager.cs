using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilityUpgradesManager : MonoBehaviour
{
    public Dictionary<Abilities, AbilityUpgradePath> AbilityUpgrades;

    #region Singleton
    private static AbilityUpgradesManager _instance;
    public static AbilityUpgradesManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Find existing instance
                _instance = FindObjectOfType<AbilityUpgradesManager>();

                // If no instance was found, create one
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("AbilityUpgradesManager");
                    _instance = singleton.AddComponent<AbilityUpgradesManager>();
                    if( _instance.AbilityUpgrades.Count == 0)
                    {
                        _instance.Load();
                    }
                }
            }
            return _instance;
        }
    }
    #endregion



    private void Start()
    {
        Load();
    }

    private void OnLevelWasLoaded(int level)
    {
        Load();
    }

    private void Load()
    {
        if (AbilityUpgrades == null)
        {
            AbilityUpgrades = new Dictionary<Abilities, AbilityUpgradePath>();
        }
        foreach (AbilityUpgradePath aup in GameObject.FindObjectsOfType<AbilityUpgradePath>(true))
        {
            AbilityUpgrades[aup.associatedAbility] = aup;
        }
    }
}
