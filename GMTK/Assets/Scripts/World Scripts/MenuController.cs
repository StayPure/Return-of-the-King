using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private GameObject mainMenu, codeMenu;
    public UnityEngine.EventSystems.EventSystem myEventSystem;

    // Default to starting on Main Menu
    void Start()
    {
        GameObject[] menus = GameObject.FindGameObjectsWithTag("Menu");
        if (menus[0].name.StartsWith("M"))
        {
            mainMenu = menus[0];
            codeMenu = menus[1];
        }
        else
        {
            mainMenu = menus[1];
            codeMenu = menus[0];
        }
        codeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    //Leave code menu and select the "Code" option on the Main Menu
    public void ExitCode()
    {
        myEventSystem.SetSelectedGameObject(mainMenu.transform.GetChild(2).gameObject);
        codeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    //Enter code menu and select the user input field, if game completed show all codes
    public void EnterCode()
    {
        mainMenu.SetActive(false);
        codeMenu.SetActive(true);
        myEventSystem.SetSelectedGameObject(codeMenu.transform.GetChild(0).gameObject);
        if (FindObjectOfType<GlobalController>().gameCompleted)
            codeMenu.transform.GetChild(2).gameObject.SetActive(true);
    }
}
