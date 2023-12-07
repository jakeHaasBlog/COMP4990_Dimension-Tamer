using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupUI : MonoBehaviour
{
    
    public GameObject mainMenu;
    public GameObject attribsMenu;

    public void playButtonPressed() {

        // pull creatures from server into a global 2d array

        SceneManager.LoadScene("Overworld");
    }

    public void mainMenu_attribsButtonPressed() {
        attribsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void attribs_backButtonPressed() {
        attribsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

}
