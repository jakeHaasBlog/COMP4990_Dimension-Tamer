using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

// Generate map and LevelEditor scripts have had their execution order set, so this script can depend on the GenerateMap Start method being called first

public class LevelEditor : MonoBehaviour
{
    public bool editModeEnabled;
    public GameObject levelEditorDisplay;
    public GenerateMap mapGenerator;
    public PlayerControls player;

    public TMPro.TextMeshProUGUI currentTileHeadingText;
    public TMPro.TextMeshProUGUI currentTileText;

    private string startWorldFileName = "StartingWorld";
    private string endWorldFileName = "EndingWorld";

    bool addingForegroundElements = false;
    int currentEquippedTileFG = 0;
    int currentEquippedTileBG = 0;

    void Start()
    {
        if (!editModeEnabled) return;
        
        levelEditorDisplay.SetActive(true);
        readMapFromFIle();
    }

    void readMapFromFIle() {
        string path;
        if (mapGenerator.currentWorldNum == 0) path = Application.dataPath + "/WorldMaps/" + startWorldFileName + ".txt";
        else if (mapGenerator.currentWorldNum == 6) path = Application.dataPath + "/WorldMaps/" + endWorldFileName + ".txt";
        else return;

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

    void saveCurrentMapToFile() {
        string path;
        if (mapGenerator.currentWorldNum == 0) path = Application.dataPath + "/WorldMaps/" + startWorldFileName + ".txt";
        else if (mapGenerator.currentWorldNum == 6) path = Application.dataPath + "/WorldMaps/" + endWorldFileName + ".txt";
        else return;

        StreamWriter writer = new StreamWriter(path, append:false);

        for (int x = 0; x < 500; x++) {
            for (int y = 0; y < 500; y++) {

                if (mapGenerator.getWorldMap().getTileBackgroundID(x, y) != TileID.none) {
                    // write the background tile
                    writer.WriteLine("b " + x + " " + y + " " + (int)mapGenerator.getWorldMap().getTileBackgroundID(x, y));
                }

                if (mapGenerator.getWorldMap().getTileForegroundID(x, y) != TileID.none) {
                    // write the foreground tile
                    writer.WriteLine("f " + x + " " + y + " " + (int)mapGenerator.getWorldMap().getTileForegroundID(x, y));
                }

            }
        }

        writer.Close();

        // Make backup at most every minute when the file is saved

        string dateTimeString = "" + DateTime.Now.Year;
        dateTimeString += "-" + DateTime.Now.Month;
        dateTimeString += "-" + DateTime.Now.Day;
        dateTimeString += "[" + DateTime.Now.Hour;
        dateTimeString += " " + DateTime.Now.Minute + "]";

        string backupPath;
        if (mapGenerator.currentWorldNum == 0) backupPath = Application.dataPath + "/WorldMaps/Backups/" + startWorldFileName + "-" + dateTimeString + ".txt";
        else if (mapGenerator.currentWorldNum == 6) backupPath = Application.dataPath + "/WorldMaps/Backups/" + endWorldFileName + "-" + dateTimeString + ".txt";
        else backupPath = Application.dataPath + "/WorldMaps/Backups/NotStartWorldOrEndWorld-" + dateTimeString + ".txt";

        if (!File.Exists(backupPath)) File.Copy(path, backupPath);
    }


    int frame;
    void Update()
    {
        if (!editModeEnabled) return;
        if (mapGenerator.currentWorldNum != 0 && mapGenerator.currentWorldNum != 6) return;

        player.setGhostMode(true);


        if (addingForegroundElements) currentTileHeadingText.text = "Foreground";
        else currentTileHeadingText.text = "Background";

        if (addingForegroundElements) currentTileText.text = Enum.GetNames(typeof(TileID))[currentEquippedTileFG];
        else currentTileText.text = Enum.GetNames(typeof(TileID))[currentEquippedTileBG];

        if (Input.GetKeyDown(KeyCode.Return)) {
            addingForegroundElements = !addingForegroundElements;
        }

        if (Input.GetKeyDown("]")) {
            if (addingForegroundElements) {
                currentEquippedTileFG++; // TODO: if the current tile is not a foreground tile, itterate again
                if (currentEquippedTileFG >= Enum.GetNames(typeof(TileID)).Length) currentEquippedTileFG = Enum.GetNames(typeof(TileID)).Length - 1;
            } else {
                currentEquippedTileBG++; // TODO: if the current tile is not a background tile, itterate again
                if (currentEquippedTileBG >= Enum.GetNames(typeof(TileID)).Length) currentEquippedTileBG = Enum.GetNames(typeof(TileID)).Length - 1;
            }
        }

        if (Input.GetKeyDown("[")) {
            if (addingForegroundElements && currentEquippedTileFG > 0) currentEquippedTileFG--; // if the current tile is not a foreground tile, itterate again
            else if (currentEquippedTileBG > 0) currentEquippedTileBG--; // if the current tile is not a background tile, itterate again
        }

        if (addingForegroundElements) {
            if (Input.GetKey("p") && TilemapData.foregroundTiles.ContainsKey((TileID)currentEquippedTileFG)) mapGenerator.getWorldMap().setCellForeground(player.getTileX(), player.getTileY(), (TileID)currentEquippedTileFG);
        } else {
            if (Input.GetKey("p") && TilemapData.backgroundTiles.ContainsKey((TileID)currentEquippedTileBG)) mapGenerator.getWorldMap().setCellBackground(player.getTileX(), player.getTileY(), (TileID)currentEquippedTileBG);
        }


        // Save every roughly 2 seconds
        frame++;
        if (frame > 100) {
            frame = 0;
            saveCurrentMapToFile();
        }

    }
}
