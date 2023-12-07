using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScientistScript : MonoBehaviour
{

    public int spriteNum;
    public string[] dialogue;
    public string scientistName;

    public GameObject player;

    public GameObject[] sprites;
    public GameObject UICanvas;
    public GameObject textPrompt;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI speechText;
    
    const float promptDistance = 1.1f;

    void Start()
    {
        for (int i = 0; i < sprites.Length; i++)
            sprites[i].SetActive(false);

        sprites[spriteNum].SetActive(true);

        UICanvas.SetActive(false);
        textPrompt.SetActive(false);

        nameText.text = scientistName;
    }


    int speechIndex = -1;
    void Update()
    {

        // If the player is too far, hide all UI and exit
        float dx = player.transform.position.x - gameObject.transform.position.x;
        float dy = player.transform.position.y - gameObject.transform.position.y;
        float playerDistSQ = dx * dx + dy * dy;
        if (playerDistSQ > promptDistance * promptDistance) {
            speechIndex = -1;
            UICanvas.SetActive(false);
            textPrompt.SetActive(false);
            return;
        }

        // If speech hasn't started yet, prompt the user. Hide everything else
        if (speechIndex == -1) {
            UICanvas.SetActive(false);
            textPrompt.SetActive(true);

            if (Input.GetKeyDown("q")) speechIndex = 0; // start speech

            return;
        }

        if (Input.GetKeyDown("q")) {
            speechIndex++;
        }

        // if all dialogue has been shown, reset
        if (speechIndex >= dialogue.Length) {
            speechIndex = -1; // end dialogue
            return;
        }

        // show speech at current index
        UICanvas.SetActive(true);
        textPrompt.SetActive(false);
        speechText.text = dialogue[speechIndex];

    }

}