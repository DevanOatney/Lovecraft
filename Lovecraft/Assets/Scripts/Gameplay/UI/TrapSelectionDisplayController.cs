using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapSelectionDisplayController : MonoBehaviour
{
    public GameObject Indicator;

    // Start is called before the first frame update
    void Start()
    {
        GameEventSystem.Instance.RegisterListener(GameEvent.BUILDING_OBJECT_PLACE, TrapPlaced);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TrapPlaced(object obj)
    {
        if (Indicator.GetComponent<Image>().enabled)
        {
            Indicator.GetComponent<Image>().enabled = false;
        }
    }

    public void TrapButtonSelected(Button selectedButton)
    {
        Indicator.transform.position = selectedButton.transform.position;
        if( !Indicator.GetComponent<Image>().enabled )
        {
            Indicator.GetComponent<Image>().enabled = true;
        }
    }
}
