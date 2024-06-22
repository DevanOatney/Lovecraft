using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainWindowTabController : MonoBehaviour
{
   // public List<GameObject> tabbedWindows;
    public GameObject currentWindow;

    public Color ActiveTabColor;
    public Color DeactiveTabColor;


    public void TabSelected(GameObject tabObject)
    {
        if( tabObject.transform.parent.gameObject == currentWindow ) { return; }

        currentWindow.transform.Find("Panel").gameObject.SetActive(false);
        tabObject.transform.parent.Find("Panel").gameObject.SetActive(true);

        currentWindow.transform.Find("Tab").GetComponent<Image>().color = DeactiveTabColor;
        tabObject.GetComponent<Image>().color = ActiveTabColor;

        currentWindow = tabObject.transform.parent.gameObject;
    }
}
