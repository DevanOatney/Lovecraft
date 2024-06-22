using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BloodCurrencyDisplay : MonoBehaviour
{
    PlayerCurrencyController PCC;
    TMP_Text displayText;

    private int cachedValue;

    // Start is called before the first frame update
    void Start()
    {
        PCC = GameObject.FindAnyObjectByType<PlayerCurrencyController>();
        displayText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cachedValue != PCC.Blood_Currency_Count)
        {
            displayText.text = PCC.Blood_Currency_Count.ToString();
            cachedValue = PCC.Blood_Currency_Count;
        }
    }
}
