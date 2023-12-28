using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PortalScript : MonoBehaviour
{

    public GameObject player;   
    public GameObject promptObject;
    public float promptDistance;
    public GenerateMap mapGenerator;
    public GameObject loadingScreen;

    private AudioSource portalSoundEffect;

    void Start() {
        portalSoundEffect = GetComponent<AudioSource>();
        loadingScreen.SetActive(false);
    }

    bool tpNextFrame = false;
    void Update()
    {
        if (tpNextFrame) {
            tpNextFrame = false;
            mapGenerator.generateWorld(WorldMap.currentMap.worldNumber + 1);
            player.GetComponent<PlayerControls>().setPosition(250, 250);
            if (WorldMap.currentMap.worldNumber == 6) gameObject.SetActive(false);
            return;
        }

        loadingScreen.SetActive(false);

        float dx = player.transform.position.x - gameObject.transform.position.x;
        float dy = player.transform.position.y - gameObject.transform.position.y;
        float distSQ = dx * dx + dy * dy;

        if (distSQ * distSQ > promptDistance) {
            promptObject.SetActive(false);
            return;
        }
        
        promptObject.SetActive(true);

        if (Input.GetKeyDown("q")) {
            portalSoundEffect.Play();
            loadingScreen.SetActive(true);
            tpNextFrame = true;
        }

    }

}
