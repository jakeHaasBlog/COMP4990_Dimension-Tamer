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
        if (WorldMap.currentMap.worldNumber == 0) path = Application.dataPath + "/WorldMaps/" + startWorldFileName + ".txt";
        else if (WorldMap.currentMap.worldNumber == 6) path = Application.dataPath + "/WorldMaps/" + endWorldFileName + ".txt";
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
                    WorldMap.currentMap.setCellForeground(x, y, (TileID)tileID);
                }
            } else if (isBackground == 'b') {
                if (TilemapData.backgroundTiles.ContainsKey((TileID)tileID)) {
                    WorldMap.currentMap.setCellBackground(x, y, (TileID)tileID);
                }
            }
            

        }

        reader.Close();

    }

    void saveCurrentMapToFile() {
        string path;
        if (WorldMap.currentMap.worldNumber == 0) path = Application.dataPath + "/WorldMaps/" + startWorldFileName + ".txt";
        else if (WorldMap.currentMap.worldNumber == 6) path = Application.dataPath + "/WorldMaps/" + endWorldFileName + ".txt";
        else return;

        StreamWriter writer = new StreamWriter(path, append:false);

        for (int x = 0; x < WorldMap.WIDTH; x++) {
            for (int y = 0; y < WorldMap.HEIGHT; y++) {

                if (WorldMap.currentMap.getCellBackgroundID(x, y) != TileID.none) {
                    // write the background tile
                    writer.WriteLine("b " + x + " " + y + " " + (int)WorldMap.currentMap.getCellBackgroundID(x, y));
                }

                if (WorldMap.currentMap.getCellForegroundID(x, y) != TileID.none) {
                    // write the foreground tile
                    writer.WriteLine("f " + x + " " + y + " " + (int)WorldMap.currentMap.getCellForegroundID(x, y));
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
        if (WorldMap.currentMap.worldNumber == 0) backupPath = Application.dataPath + "/WorldMaps/Backups/" + startWorldFileName + "-" + dateTimeString + ".txt";
        else if (WorldMap.currentMap.worldNumber == 6) backupPath = Application.dataPath + "/WorldMaps/Backups/" + endWorldFileName + "-" + dateTimeString + ".txt";
        else backupPath = Application.dataPath + "/WorldMaps/Backups/NotStartWorldOrEndWorld-" + dateTimeString + ".txt";

        if (!File.Exists(backupPath)) File.Copy(path, backupPath);
    }

    public static void loadMapFromFile(string filepath) {
        StreamReader reader = new StreamReader(filepath);
        
        string line;
        while ((line = reader.ReadLine()) != null) {
            
            var parts = line.Split(' ');
            char isBackground = parts[0][0];
            int x = Convert.ToInt32(parts[1]);
            int y = Convert.ToInt32(parts[2]);
            int tileID = Convert.ToInt32(parts[3]);

            if (isBackground == 'f') {
                if (TilemapData.foregroundTiles.ContainsKey((TileID)tileID)) {
                    WorldMap.currentMap.setCellForeground(x, y, (TileID)tileID);
                }
            } else if (isBackground == 'b') {
                if (TilemapData.backgroundTiles.ContainsKey((TileID)tileID)) {
                    WorldMap.currentMap.setCellBackground(x, y, (TileID)tileID);
                }
            }
            

        }

        reader.Close();
    }

    
    // ensures all plotted tiles have a cardinal neighbour
    private static void placeLinePoint(TileID tile, bool foreground, int x, int y) {
        if (foreground) WorldMap.currentMap.setCellForeground(x, y, tile);
        else WorldMap.currentMap.setCellBackground(x, y, tile);
        
        // if top-left has no neighbors, fill top cell
        if (WorldMap.currentMap.getCellBackgroundID(x - 1, y + 1) == tile)
            if (WorldMap.currentMap.getCellBackgroundID(x, y + 1) != tile && WorldMap.currentMap.getCellBackgroundID(x - 1, y) != tile) WorldMap.currentMap.setCellBackground(x, y + 1, tile);

        // if top-right has no neighbors, fill top cell
        if (WorldMap.currentMap.getCellBackgroundID(x + 1, y + 1) == tile)
            if (WorldMap.currentMap.getCellBackgroundID(x, y + 1) != tile && WorldMap.currentMap.getCellBackgroundID(x + 1, y) != tile) WorldMap.currentMap.setCellBackground(x, y + 1, tile);

        // if bottom-left has no neighbors, fill bottom cell
        if (WorldMap.currentMap.getCellBackgroundID(x - 1, y - 1) == tile)
            if (WorldMap.currentMap.getCellBackgroundID(x, y - 1) != tile && WorldMap.currentMap.getCellBackgroundID(x - 1, y) != tile) WorldMap.currentMap.setCellBackground(x, y - 1, tile);

        // if bottom-right has no neighbors, fill bottom cell
        if (WorldMap.currentMap.getCellBackgroundID(x + 1, y - 1) == tile)
            if (WorldMap.currentMap.getCellBackgroundID(x, y - 1) != tile && WorldMap.currentMap.getCellBackgroundID(x + 1, y) != tile) WorldMap.currentMap.setCellBackground(x, y - 1, tile);

    }

    // Thank you Wikipedia for this implementation of Bresenham's line algorithm
    // This function will ensure all generated paths are walkable
    public static void placeLine(TileID tile, bool foreground, int x0, int y0, int x1, int y1) {
        
        if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0)) {
            if (x0 > x1) {
                int dx = x0 - x1;
                int dy = y0 - y1;
                int yi = 1;
                if (dy < 0) {
                    yi = -1;
                    dy = -dy;
                }
                int D = (2 * dy) - dx;
                int y = y1;

                for (int x = x1; x < x1; x++) {
                    
                    placeLinePoint(tile, foreground, x, y);

                    if (D > 0) {
                        y = y + yi;
                        D = D + (2 * (dy - dx));
                    } else {
                        D = D + 2 * dy;
                    }
                }

            } else {
                int dx = x1 - x0;
                int dy = y1 - y0;
                int yi = 1;
                if (dy < 0) {
                    yi = -1;
                    dy = -dy;
                }
                int D = (2 * dy) - dx;
                int y = y0;

                for (int x = x0; x < x1; x++) {
                    
                    placeLinePoint(tile, foreground, x, y);

                    if (D > 0) {
                        y = y + yi;
                        D = D + (2 * (dy - dx));
                    } else {
                        D = D + 2 * dy;
                    }
                }
            }
        } else {
            if (y0 > y1) {
                int dx = x0 - x1;
                int dy = y0 - y1;
                int xi = 1;
                if (dx < 0) {
                    xi = -1;
                    dx = -dx;
                }
                int D = (2 * dx) - dy;
                int x = x1;

                for (int y = y1; y < y1; y++) {
                    
                    placeLinePoint(tile, foreground, x, y);

                    if (D > 0) {
                        x = x + xi;
                        D = D + (2 * (dx - dy));
                    } else {
                        D = D + 2 * dx;
                    }
                }

            } else {
                int dx = x1 - x0;
                int dy = y1 - y0;
                int xi = 1;
                if (dx < 0) {
                    xi = -1;
                    dx = -dx;
                }
                int D = (2 * dx) - dy;
                int x = x0;

                for (int y = y0; y < y1; y++) {
                    
                    placeLinePoint(tile, foreground, x, y);

                    if (D > 0) {
                        x = x + xi;
                        D = D + (2 * (dx - dy));
                    } else {
                        D = D + 2 * dx;
                    }
                }

            }
        }

        // have to plot starting and ending tile individually
        placeLinePoint(tile, foreground, x0, y0);
        placeLinePoint(tile, foreground, x1, y1);

    }


    int frame;
    void Update()
    {
        if (!editModeEnabled) return;
        if (WorldMap.currentMap.worldNumber != 0 && WorldMap.currentMap.worldNumber != 6) return;

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
            if (Input.GetKey("p") && TilemapData.foregroundTiles.ContainsKey((TileID)currentEquippedTileFG)) 
                WorldMap.currentMap.setCellForeground(player.getTileX(), player.getTileY(), (TileID)currentEquippedTileFG);
        } else {
            if (Input.GetKey("p") && TilemapData.backgroundTiles.ContainsKey((TileID)currentEquippedTileBG)) 
                WorldMap.currentMap.setCellBackground(player.getTileX(), player.getTileY(), (TileID)currentEquippedTileBG);
        }


        // Save every roughly 10 seconds
        frame++;
        if (frame > 60 * 10) {
            frame = 0;
            saveCurrentMapToFile();
        }

    }
}
