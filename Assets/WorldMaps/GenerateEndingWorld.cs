using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GenerateEndingWorld : MonoBehaviour
{
    public GenerateMap mapGenerator;
    public PlayerControls player;

    public void generate() {

        // Set Portal Position
        mapGenerator.portal.transform.position = new Vector3(0, 0, mapGenerator.portal.transform.position.z);

        // Load map from file
        string path = Application.dataPath + "/WorldMaps/EndingWorld.txt";
        LevelEditor.loadMapFromFile(path);

    }

}
