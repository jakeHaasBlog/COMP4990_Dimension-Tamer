using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


// ==== The order of these enums does not matter, but make sure that once a new tile is added that its assigned value never changes (Will cause incorrect tiles to load in the starting and end world) ====
// To add a new tile: Add an enum name for it below, then load the tile into backgroundTiles or foregroundTiles (look at loadTiles() function)
public enum TileID {
    // Universal
    none = 0,
    water = 1,

    // Forest
    dirt = 2,
    grass = 3,
    rock = 4,
    tree = 5,
    gravelPath = 6,

    // Ice Pond
    ice = 7,
    snow = 8,
    snowTree = 9,
    snowyGravelPath = 10,

    // Starting World
    labFloor = 11,
    labWall = 12,
    bookshelfBottom = 13,
    bookshelfTop = 14,
    labWallFront = 15,
    lockerTop = 16,
    lockerBottom = 17,
    labTable = 18,
    labTable1 = 19,
    labTable2 = 20,
    labTable3 = 21,
    labTable4 = 22,
    starting_grass1 = 23,
    starting_grass2 = 24,
    starting_grass3 = 25,
    starting_grass4 = 26,
    starting_grass5 = 27,
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

    public Tilemap foregroundTilemap;
    public Tilemap backgroundTilemap;
    public GenerateStartingWorld startingWorldGenerator;
    public GenerateEndingWorld endingWorldGenerator;
    public int currentWorldNum;
    public GameObject portal;

    private WorldMap worldMap;
    public ref WorldMap getWorldMap() {
        return ref worldMap;
    }

    public static Tile loadTileFromPath(String path) {

        // Texture2D Tex2D;
        // byte[] FileData;
 
        // if (File.Exists(FilePath))
        // {
        //     FileData = File.ReadAllBytes(FilePath);
        //     Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
        //     if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
        //         return Tex2D;                 // If data = readable -> return texture
        // }
        // return null;                     // Return null if load failed

        Texture2D tileTexture = Resources.Load<Texture2D>(path);
        int pixelsPerUnit = tileTexture.width;
        Sprite tileSprite = Sprite.Create(tileTexture, new Rect(0, 0, tileTexture.width, tileTexture.height), new Vector2(0, 0), pixelsPerUnit, 0, SpriteMeshType.Tight);

        Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        tile.sprite = tileSprite;

        return tile;
    }
    
    void loadTiles() {
        TilemapData.foregroundTiles.Clear();
        TilemapData.backgroundTiles.Clear();

        // Universal
        TilemapData.backgroundTiles.Add(TileID.none, new TileProperties(null, false, false));
        TilemapData.foregroundTiles.Add(TileID.none, new TileProperties(null, true, false));
        TilemapData.backgroundTiles.Add(TileID.water, new TileProperties(loadTileFromPath("BiomeTiles/water"), false, false ));

        // Forest
        TilemapData.foregroundTiles.Add(TileID.rock, new TileProperties(loadTileFromPath("BiomeTiles/Forest/rock"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.gravelPath, new TileProperties(loadTileFromPath("BiomeTiles/Forest/gravelPath"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.tree, new TileProperties(loadTileFromPath("BiomeTiles/Forest/tree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.dirt, new TileProperties(loadTileFromPath("BiomeTiles/Forest/dirt"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.grass, new TileProperties(loadTileFromPath("BiomeTiles/Forest/grass"), true, true ));

        // Ice Pond
        TilemapData.foregroundTiles.Add(TileID.snowyGravelPath, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snowyGravelPath"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.snowTree, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snowTree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.ice, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/ice"), true, true ));
        TilemapData.backgroundTiles.Add(TileID.snow, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snow"), true, true ));

        // Starting World
        TilemapData.backgroundTiles.Add(TileID.labFloor, new TileProperties(loadTileFromPath("StartingWorldTiles/LabFloor"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.labWall, new TileProperties(loadTileFromPath("StartingWorldTiles/LabWall"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.bookshelfBottom, new TileProperties(loadTileFromPath("StartingWorldTiles/bookshelfBottom"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.bookshelfTop, new TileProperties(loadTileFromPath("StartingWorldTiles/bookshelfTop"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.labWallFront, new TileProperties(loadTileFromPath("StartingWorldTiles/labWallFront"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.lockerTop, new TileProperties(loadTileFromPath("StartingWorldTiles/lockerTop"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.lockerBottom, new TileProperties(loadTileFromPath("StartingWorldTiles/lockerBottom"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.labTable, new TileProperties(loadTileFromPath("StartingWorldTiles/labTable"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.labTable1, new TileProperties(loadTileFromPath("StartingWorldTiles/labTable1"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.labTable2, new TileProperties(loadTileFromPath("StartingWorldTiles/labTable2"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.labTable3, new TileProperties(loadTileFromPath("StartingWorldTiles/labTable3"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.labTable4, new TileProperties(loadTileFromPath("StartingWorldTiles/labTable4"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.starting_grass1, new TileProperties(loadTileFromPath("StartingWorldTiles/grass1"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.starting_grass2, new TileProperties(loadTileFromPath("StartingWorldTiles/grass2"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.starting_grass3, new TileProperties(loadTileFromPath("StartingWorldTiles/grass3"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.starting_grass4, new TileProperties(loadTileFromPath("StartingWorldTiles/grass4"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.starting_grass5, new TileProperties(loadTileFromPath("StartingWorldTiles/grass5"), false, false ));

    }

    public void generateTest() {
        // Example of how this will be used
        // I will eventually be putting each biome generator in its own file, this script is just for setup and helper functions

        portal.transform.position = new Vector3(250, 250, portal.transform.position.z);

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

    public void generateWorld(int worldNumber) {

        worldMap.clearAllCells();

        if (worldNumber < 0 || worldNumber > 6) {
            Debug.Log("Error: Trying to set invalid world number: " + worldNumber);
            return;
        }

        currentWorldNum = worldNumber;

        if (currentWorldNum == 0) {
            startingWorldGenerator.generate();
        } else if (currentWorldNum == 6) {
            endingWorldGenerator.generate();
        } else {
            generateTest(); // procedurally generate map
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

        generateWorld(currentWorldNum);

    }

};

