using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

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
    public int Cost;
}

public class AbilityUpgradePath : MonoBehaviour
{
    public Abilities associatedAbility;
    public List<UpgradeNode> upgradePaths;
    public bool predefinedPath;
    public float undefinedUpgradeModifier; // this is only used if we dont have a predefined upgrade path
    public int currentAbilityLevel = 0;
    public GameObject purchaseIndicator;

    // UI Connections
    [SerializeField] private TMP_Text uiLevel;
    [SerializeField] private TMP_Text uiCost;
    // ------

    int currentCost = 0;
    PlayerCurrencyController PCC;

    private void Start()
    {
        currentCost = predefinedPath ? upgradePaths[0].Cost : currentAbilityLevel + 1 * 10;
        uiCost.text = currentCost.ToString();
        PCC = GameObject.FindObjectOfType<PlayerCurrencyController>();
    }

    public float GetModifiedValue(float ogValue)
    {
        return ogValue * (predefinedPath ? upgradePaths[currentAbilityLevel].Value : undefinedUpgradeModifier * (currentAbilityLevel +1));
    }

    public void UpgradeSelected()
    {
        if (PCC.SpendBloodCurrency(currentCost))
        {
            purchaseIndicator.GetComponent<TMP_Text>().text = "- " + currentCost.ToString();
            purchaseIndicator.SetActive(true);

            if (predefinedPath)
            {
                currentAbilityLevel++;
                if (uiLevel != null)
                {
                    uiLevel.text = (currentAbilityLevel + 1).ToString();
                }

                if (upgradePaths.Count == currentAbilityLevel + 1)
                {
                    uiCost.text = "Max Upgrades";
                    uiCost.GetComponentInParent<Button>().interactable = false;
                    currentCost = Int32.MaxValue;
                }
                else
                {
                    currentCost = upgradePaths[currentAbilityLevel + 1].Cost;
                    uiCost.text = upgradePaths[currentAbilityLevel + 1].Cost.ToString();
                }
            }
            else
            {
                currentAbilityLevel++;
                if (uiLevel)
                {
                    uiLevel.text = (currentAbilityLevel + 1).ToString();
                }
                uiCost.text = (currentAbilityLevel * 10).ToString();
                currentCost = currentAbilityLevel * 10;
            }
        } else
        {
            purchaseIndicator.GetComponent<TMP_Text>().text = "Anemic - not enough blood";
            purchaseIndicator.SetActive(true);
        }
    }
}
