using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldUI : MonoBehaviour
{

    public GameObject overworld;
    public GameObject pauseMenu;
    public GameObject playerInventory;
    public GameObject battleScene;

    void disableAll() {
        overworld.SetActive(false);
        pauseMenu.SetActive(false);
        playerInventory.SetActive(false);
        battleScene.SetActive(false);
    }
    
    public void overworld_pauseClicked() {
        disableAll();
        pauseMenu.SetActive(true);
    }

    public void overworld_inventoryClicked() {
        disableAll();
        playerInventory.SetActive(true);
    }

    public void overworld_simulateBattleButtonClicked() {
        disableAll();
        battleScene.SetActive(true);
    }


    public void pause_resumeClicked() {
        disableAll();
        overworld.SetActive(true);
    }

    public void inventory_backToOverworldClicked() {
        disableAll();
        overworld.SetActive(true);
    }


    public void battle_fleeFromBattleClicked() {
        disableAll();
        overworld.SetActive(true);
    }

    public void switchToBattleScene() {
        disableAll();
        battleScene.SetActive(true);
    }

    public void switchToOverworld() {
        disableAll();
        overworld.SetActive(true);
    }

}
