using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.IO;
using System;

public class StartupUI : MonoBehaviour
{
    
    public GameObject mainMenu;
    public GameObject attribsMenu;

    public void playButtonPressed() {
        bool canLoadFromServer = false;

        // pull creatures into creature manager
        if (canLoadFromServer) {
            // ask server for 10 creatures from each biome

        } else {
            // load placeholders for testing
            loadPlaceholderCreatures();
        }

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


    void loadPlaceholderCreatures() {

        string[] creaturePaths = new string[3] {"Creatures/placeholders/creature1", "Creatures/placeholders/creature2", "Creatures/placeholders/creature3"};

        foreach (BiomeID biome in Enum.GetValues(typeof(BiomeID))) {

            for (int i = 0; i < 10; i++) {
                string s = creaturePaths[UnityEngine.Random.Range(0, creaturePaths.Length)];
                
                CreatureManager.instance.biomeCreatures[biome].Add(new CreatureData(
                    loadCreatureTexture(s),
                    ElementalType.water,
                    60 + UnityEngine.Random.Range(-10, 10), // max hp
                    20 + UnityEngine.Random.Range(-10, 10), // defence
                    10 + UnityEngine.Random.Range(-10, 10), // speed
                    20 + UnityEngine.Random.Range(-10, 10) // attack
                ));
            }
        }

        CreatureManager.instance.playerCreatures[0] = CreatureManager.instance.generateRandomCreature();
        
    }

    Texture2D loadCreatureTexture (string path) {
        Texture2D tileTexture = Resources.Load<Texture2D>(path);
        if (tileTexture == null) Debug.Log("Could not find creature image at: " + path);
        return tileTexture;
    }

}
