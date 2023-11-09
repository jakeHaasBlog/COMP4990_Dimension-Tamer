using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum TileID {
    none,
    water,
    dirt,
    grass,
    rock,
    tree,
    gravelPath,
    ice,
    snow,
    snowTree,
    snowyGravelPath
}

class TileProperties {
    public TileProperties(Tile tile, bool isWalkable, bool isEncounterZone) {
        this.tileImage = tile;
        this.isWalkable = isWalkable;
        this.isEncounterZone = isEncounterZone;
    }

    public Tile tileImage;
    public bool isWalkable;
    public bool isEncounterZone;
}

struct TilemapData {
    public static Tilemap backgroundTilemap;
    public static Tilemap foregroundTilemap;
    public static Tile water;

    public static IDictionary<TileID, TileProperties> foregroundTiles;
    public static IDictionary<TileID, TileProperties> backgroundTiles;
}

class WorldCell { // cannot be a struct because they don't accept member initializers apparently
    public TileID backgroundTileID = TileID.none;
    public TileID foregroundTileID = TileID.none;
    public int encounterZoneID = -1;
};

public class WorldMap {

    public WorldMap() {
        worldCells = new WorldCell[500,500];
        for (int i = 0; i < 500; i++) {
            for (int j = 0; j < 500; j++) {
                worldCells[i, j] = new WorldCell();
            }
        }
    }

    public bool isValidCoord(int x, int y) {
        return x >= 0 && x < 500 && y >= 0 && y < 500;
    }

    public void setCellForeground(int x, int y, TileID foregroundTileID) {
        if (!isValidCoord(x, y)) return;
        worldCells[x, y].foregroundTileID = foregroundTileID;
        TilemapData.foregroundTilemap.SetTile(new Vector3Int(x, y, 0), TilemapData.foregroundTiles[foregroundTileID].tileImage);
    }

    public void setCellBackground(int x, int y, TileID backgroundTileID) {
        if (!isValidCoord(x, y)) return;
        worldCells[x, y].backgroundTileID = backgroundTileID;
        TilemapData.backgroundTilemap.SetTile(new Vector3Int(x, y, 0), TilemapData.backgroundTiles[backgroundTileID].tileImage);
    }

    public void setTileEncounterZoneID(int x, int y, int encounterZoneID) {
        if (!isValidCoord(x, y)) return;
        worldCells[x, y].encounterZoneID = encounterZoneID;
    }

    public TileID getTileForegroundID (int x, int y) {
        if (!isValidCoord(x, y)) return TileID.none;
        return worldCells[x, y].foregroundTileID;
    }

    public TileID getTileBackgroundID (int x, int y) {
        if (!isValidCoord(x, y)) return TileID.none;
        return worldCells[x, y].backgroundTileID;
    }

    public bool getTileIsWalkable(int x, int y) {
        if (!isValidCoord(x, y)) return false;
        
        TileID bgID = worldCells[x, y].backgroundTileID;
        TileID fgID = worldCells[x, y].foregroundTileID;

        if (TilemapData.foregroundTiles[fgID].isWalkable && TilemapData.backgroundTiles[bgID].isWalkable) {
            return true;
        } else {
            return false;
        }

    }

    public int getTileEncounterZoneID(int x, int y) {
        if (!isValidCoord(x, y)) return -1;
        
        TileID bgID = worldCells[x, y].backgroundTileID;

        if (TilemapData.backgroundTiles[bgID].isWalkable && TilemapData.backgroundTiles[bgID].isEncounterZone) {
            return worldCells[x, y].encounterZoneID;
        }

        return -1;
    }

    public void clearCell(int x, int y) {
        if (!isValidCoord(x, y)) return;
        worldCells[x, y] = new WorldCell();
        TilemapData.foregroundTilemap.SetTile(new Vector3Int(x, y, 0), null);
        TilemapData.backgroundTilemap.SetTile(new Vector3Int(x, y, 0), null);
    }

    public void clearAllCells() {
        for (int i = 0; i < worldCells.GetLength(0); i++) {
            for (int j = 0; j < worldCells.GetLength(1); j++) {
                worldCells[i, j] = new WorldCell();
                TilemapData.foregroundTilemap.SetTile(new Vector3Int(i, j, 0), null);
                TilemapData.backgroundTilemap.SetTile(new Vector3Int(i, j, 0), null);
            }
        }
    }

    private WorldCell[,] worldCells;
    
};

public class GenerateMap : MonoBehaviour
{

    public Tile water;
    public Tilemap foregroundTilemap;
    public Tilemap backgroundTilemap;

    private WorldMap worldMap;
    public ref WorldMap getWorldMap() {
        return ref worldMap;
    }

    public static Tile loadTileFromPath(String path) {
        return Resources.Load<Tile>(path);
    }
    
    void loadTiles() {
        TilemapData.foregroundTiles.Clear();
        TilemapData.backgroundTiles.Clear();
        TilemapData.backgroundTiles.Add(TileID.none, new TileProperties(null, false, false));
        TilemapData.foregroundTiles.Add(TileID.none, new TileProperties(null, true, false));

        TilemapData.water = water;
        TilemapData.backgroundTiles.Add(TileID.water, new TileProperties(water, false, false ));

        TilemapData.foregroundTiles.Add(TileID.rock, new TileProperties(loadTileFromPath("BiomeTiles/Forest/rock"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.gravelPath, new TileProperties(loadTileFromPath("BiomeTiles/Forest/gravelPath"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.tree, new TileProperties(loadTileFromPath("BiomeTiles/Forest/tree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.dirt, new TileProperties(loadTileFromPath("BiomeTiles/Forest/dirt"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.grass, new TileProperties(loadTileFromPath("BiomeTiles/Forest/grass"), true, true ));

        TilemapData.foregroundTiles.Add(TileID.snowyGravelPath, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snowyGravelPath"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.snowTree, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snowTree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.ice, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/ice"), true, true ));
        TilemapData.backgroundTiles.Add(TileID.snow, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snow"), true, true ));
    }

    public void generateTest() {
        // Example of how this will be used
        // I will eventually be putting each biome generator in its own file, this script is just for setup and helper functions

        TileID[] forestBG = { TileID.grass, TileID.dirt };
        TileID[] forestFG = { TileID.tree, TileID.gravelPath, TileID.rock };

        TileID[] icePondBG = { TileID.snow, TileID.ice };
        TileID[] icePondFG = { TileID.snowTree, TileID.snowyGravelPath };

        for (int x = 0; x < 500; x++) {
            for (int y = 0; y < 500; y++) {

                worldMap.setCellBackground(x, y, TileID.water);

                if (x < 250 && !(248 <= x && x <= 252)) {
                    worldMap.setCellBackground(x, y, icePondBG[UnityEngine.Random.Range(0, icePondBG.Length)]);

                    if (UnityEngine.Random.Range(0, 20) == 0) {
                        worldMap.setCellForeground(x, y, icePondFG[UnityEngine.Random.Range(0, icePondFG.Length)]);
                    }
                } else {
                    worldMap.setCellBackground(x, y, forestBG[UnityEngine.Random.Range(0, forestBG.Length)]);
                    
                    if (UnityEngine.Random.Range(0, 5) == 0) {
                        worldMap.setCellForeground(x, y, forestFG[UnityEngine.Random.Range(0, forestFG.Length)]);
                    }
                }

            }
        }
    }


    void Start()
    {
        worldMap = new WorldMap();

        TilemapData.backgroundTilemap = backgroundTilemap;
        TilemapData.foregroundTilemap = foregroundTilemap;
        TilemapData.foregroundTiles = new Dictionary<TileID, TileProperties>();
        TilemapData.backgroundTiles = new Dictionary<TileID, TileProperties>();

        loadTiles();

        generateTest();

    }

};

