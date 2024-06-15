using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum Abilities
{
    ATTACK_SPEED,
    ATTACK_DAMAGE,
    TRAP_COOLDOWN,
    BLOODLETTING,
    TRAP_DURATION,
    TRAP_EFFECTIVENESS
} 

[Serializable]
public struct UpgradeNode
{
    public float Value;
    public float Cost;
}

public class AbilityUpgradePath : MonoBehaviour
{
    public Abilities associatedAbility;
    public List<UpgradeNode> upgradePaths;
    public bool predefinedPath;
    public float undefinedUpgradeModifier; // this is only used if we dont have a predefined upgrade path
    public int currentAbilityLevel = 0;


    // UI Connections
    [SerializeField] private TMP_Text uiLevel;
    [SerializeField] private TMP_Text uiCost;
    // ------

    public float GetModifiedValue(float ogValue)
    {
        return ogValue * (predefinedPath ? upgradePaths[currentAbilityLevel].Value : undefinedUpgradeModifier * (currentAbilityLevel +1));
    }

    public void UpgradeSelected()
    {
        currentAbilityLevel++;
        uiLevel.text = currentAbilityLevel.ToString();
        uiCost.text = upgradePaths[currentAbilityLevel + 1].Cost.ToString();
    }
}
