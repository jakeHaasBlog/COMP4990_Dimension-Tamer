using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GenerateStartingWorld : MonoBehaviour
{
    public PlayerControls player;

    public void generate() {

        // Set Portal Position
        PortalManager.instance.addPortal(50, 40);

        // Load map from file
        string path = Application.dataPath + "/WorldMaps/StartingWorld.txt";
        LevelEditor.loadMapFromFile(path);

    }

}
