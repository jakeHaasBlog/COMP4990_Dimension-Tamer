using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{

    public GameObject player;   
    public GameObject promptObject;
    public float promptDistance;
    public GenerateMap mapGenerator;

    void Update()
    {
        float dx = player.transform.position.x - gameObject.transform.position.x;
        float dy = player.transform.position.y - gameObject.transform.position.y;
        float dist = Mathf.Sqrt(dx * dx + dy * dy);

        if (dist > promptDistance) {
            promptObject.SetActive(false);
            return;
        }
        
        promptObject.SetActive(true);

        if (Input.GetKeyDown("q")) {
            mapGenerator.generateWorld(mapGenerator.currentWorldNum + 1);
            player.GetComponent<PlayerControls>().setPosition(250, 250);

            if (mapGenerator.currentWorldNum == 6) gameObject.SetActive(false);
        }

    }

}
