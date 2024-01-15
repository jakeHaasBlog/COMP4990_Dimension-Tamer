using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GenerateEndingWorld : MonoBehaviour
{
    public PlayerControls player;

    public void generate() {

        // Set Portal Position
        PortalManager.instance.removePortals();

        // Replace water with space background
        bool tmp = WorldMap.validateCoordsAsCircle;
        WorldMap.validateCoordsAsCircle = false;
        TileID[] spaceBGTiles = { TileID.endWorldBG1, TileID.endWorldBG2, TileID.endWorldBG3 };
        for (int y = 0; y < WorldMap.HEIGHT; y++) {
            for (int x = 0; x < WorldMap.WIDTH; x++) {
                WorldMap.currentMap.setCellBackground(x, y, spaceBGTiles[UnityEngine.Random.Range(0, spaceBGTiles.Length)]);
            }
        }
        WorldMap.validateCoordsAsCircle = tmp;

        // Load map from file
        string path = Application.dataPath + "/WorldMaps/EndingWorld.txt";
        LevelEditor.loadMapFromFile(path);

    }

}
