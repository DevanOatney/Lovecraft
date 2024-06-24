using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrapSelectionGridController : MonoBehaviour
{
    public Image lockedIndicator;
    public TRAP_TYPES AssociatedTrapType;

    UnlockedTrapsManager UTM;
    bool unlockedStatus = true;

    // Start is called before the first frame update
    void Start()
    {
        UTM = GameObject.FindObjectOfType<UnlockedTrapsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if( UTM.Traps[(int)AssociatedTrapType].unlocked )
        {
            GetComponent<Button>().interactable = true;
            lockedIndicator.gameObject.SetActive(false);
            unlockedStatus = true;
        } else if(!UTM.Traps[(int)AssociatedTrapType].unlocked )
        {
            GetComponent<Button>().interactable = false;
            lockedIndicator.gameObject.SetActive(true);
            unlockedStatus = false;
        }
    }
}
