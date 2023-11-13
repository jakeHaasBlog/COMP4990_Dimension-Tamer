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
        mapGenerator.portal.transform.position = new Vector3(250, 245, mapGenerator.portal.transform.position.z);

        // Load map from file
        string path = Application.dataPath + "/WorldMaps/StartingWorld.txt";
        StreamReader reader = new StreamReader(path);
        
        string line;
        while ((line = reader.ReadLine()) != null) {
            
            var parts = line.Split(' ');
            char isBackground = parts[0][0];
            int x = Convert.ToInt32(parts[1]);
            int y = Convert.ToInt32(parts[2]);
            int tileID = Convert.ToInt32(parts[3]);

            if (isBackground == 'f') {
                if (TilemapData.foregroundTiles.ContainsKey((TileID)tileID)) {
                    mapGenerator.getWorldMap().setCellForeground(x, y, (TileID)tileID);
                }
            } else if (isBackground == 'b') {
                if (TilemapData.backgroundTiles.ContainsKey((TileID)tileID)) {
                    mapGenerator.getWorldMap().setCellBackground(x, y, (TileID)tileID);
                }
            }
            

        }

        reader.Close();

    }

}
