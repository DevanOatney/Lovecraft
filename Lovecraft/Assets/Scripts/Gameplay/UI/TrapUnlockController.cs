using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrapUnlockController : MonoBehaviour
{
    public Button SelectButton;
    public UnlockedTrapsManager TrapsManager;
    public TRAP_TYPES trapType;
    public Image unlockedIcon;
    public Image currencyIcon;

    public Sprite checkmarkSprite;

    // Start is called before the first frame update
    void Start()
    {
        if (!TrapsManager.Traps[(int)trapType].unlocked)
        {
            SelectButton.GetComponentInChildren<TMPro.TMP_Text>().text = TrapsManager.Traps[(int)trapType].costToUnlock.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TrapUnlockSelected(Button _button)
    {
        if( _button == SelectButton)
        {
            if(TrapsManager.AttemptToUnlockTrap(trapType))
            {
                _button.interactable = false;
                _button.GetComponentInChildren<TMPro.TMP_Text>().text = "Unlocked";
                unlockedIcon.sprite = checkmarkSprite;
                currencyIcon.gameObject.SetActive(false);
            }
        }
    }
}
