using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GenerateStartingWorld : MonoBehaviour
{
    public GenerateMap mapGenerator;
    public PlayerControls player;

    public void generate() {

        // Set Portal Position
        Vector3 ppos = player.getRealpos(250, 245);
        mapGenerator.portal.transform.position = new Vector3(ppos.x, ppos.y, mapGenerator.portal.transform.position.z);

        // Load map from file
        string path = Application.dataPath + "/WorldMaps/StartingWorld.txt";
        LevelEditor.loadMapFromFile(path);

    }

}
