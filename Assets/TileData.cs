using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

// ==== The order of these enums does not matter, but make sure that once a new tile is added that its assigned value never changes (Will cause incorrect tiles to load in the starting and end world) ====
// To add a new tile: Add an enum name for it below, then load the tile into backgroundTiles or foregroundTiles (look at loadTiles() function)
public enum TileID {
    // Starting World / endworld / universal (these ids must be in order for the level editor, other tiles will not be loaded by the editor)
    labFloor = 000,
    labWall = 001,
    bookshelfBottom = 002,
    bookshelfTop = 003,
    labWallFront = 004,
    lockerTop = 005,
    lockerBottom = 006,
    labTable = 007,
    labTable1 = 008,
    labTable2 = 009,
    labTable3 = 010,
    labTable4 = 011,
    starting_grass1 = 012,
    starting_grass2 = 013,
    starting_grass3 = 014,
    starting_grass4 = 015,
    starting_grass5 = 016,
    none = 017,
    water = 018,
    persistantPath = 019,
    endWorldGround = 020,
    endWorldWall = 021,

    // Grassland
    grasslandGrass = 300,
    grasslandTallGrass = 301,

    // Forest
    dirt = 402,
    grass = 403,
    rock = 404,
    tree = 405,
    gravelPath = 406,

    // PineForest
    pineForestDirt = 500,
    needleCoveredGroud = 501,
    pineTreeBottom = 502,
    pineTreeTop = 503,

    // Mountain
    mountainSide = 600,
    rockyGround = 601,
    boulder = 602,

    // Cave
    caveGround = 700,
    caveWall = 701,
    caveFilling = 702,

    // Tundra
    permafrost = 800,
    tundraIce = 801,

    // SnowyForest
    snowTree = 900,
    snowyNeedles = 901,
    snowyForestGround = 902,


    // Ice Pond
    icePondTree = 1000,
    icePondIce = 1001,
    icePondSnow = 1002,
    snowyGravelPath = 1003,

    // Volcano
    volcanoRockyGround = 1100,
    volcanoCrackedGround = 1101,
    volcanoLavaPitBottom = 1102,
    volcanoLavaPitTop = 1103,
    volcanoLavaPitMiddle = 1104,

    // Tropics
    palmTree = 1200,
    dryGrass = 1201,

    // Desert
    sand = 1300,
    crackedDirt = 1301,

    // Secret Meadow
    secretGrass = 1400,
    secretRock = 1401,
}

public class TileProperties {
    public TileProperties(Tile tile, bool isWalkable, bool isEncounterZone) {
        this.tileImage = tile;
        this.isWalkable = isWalkable;
        this.isEncounterZone = isEncounterZone;
    }

    public Tile tileImage;
    public bool isWalkable;
    public bool isEncounterZone;
}

public static class TilemapData {
    public static Tilemap backgroundTilemap;
    public static Tilemap foregroundTilemap;
    public static IDictionary<TileID, TileProperties> foregroundTiles;
    public static IDictionary<TileID, TileProperties> backgroundTiles;

    public static Tile loadTileFromPath(String path) {

        Texture2D tileTexture = Resources.Load<Texture2D>(path);
        if (tileTexture == null) Debug.Log("Could not find texture at: " + path);
        int pixelsPerUnit = tileTexture.width;
        Sprite tileSprite = Sprite.Create(tileTexture, new Rect(0, 0, tileTexture.width, tileTexture.height), new Vector2(0, 0), pixelsPerUnit, 0, SpriteMeshType.Tight);

        Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        tile.sprite = tileSprite;

        return tile;
    }

    public static void loadTiles() {
        TilemapData.foregroundTiles.Clear();
        TilemapData.backgroundTiles.Clear();

        // Universal
        TilemapData.backgroundTiles.Add(TileID.none, new TileProperties(null, false, false));
        TilemapData.foregroundTiles.Add(TileID.none, new TileProperties(null, true, false));
        TilemapData.backgroundTiles.Add(TileID.water, new TileProperties(loadTileFromPath("BiomeTiles/water"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.persistantPath, new TileProperties(loadTileFromPath("BiomeTiles/PersistantPath"), true, false ));

        // Grassland
        TilemapData.backgroundTiles.Add(TileID.grasslandGrass, new TileProperties(loadTileFromPath("BiomeTiles/Grassland/grasslandGrass"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.grasslandTallGrass, new TileProperties(loadTileFromPath("BiomeTiles/Grassland/grasslandTallGrass"), true, false ));

        // Forest
        TilemapData.foregroundTiles.Add(TileID.rock, new TileProperties(loadTileFromPath("BiomeTiles/Forest/rock"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.gravelPath, new TileProperties(loadTileFromPath("BiomeTiles/Forest/gravelPath"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.tree, new TileProperties(loadTileFromPath("BiomeTiles/Forest/tree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.dirt, new TileProperties(loadTileFromPath("BiomeTiles/Forest/dirt"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.grass, new TileProperties(loadTileFromPath("BiomeTiles/Forest/grass"), true, false ));

        // PineForest
        TilemapData.backgroundTiles.Add(TileID.pineForestDirt, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/pineForestDirt"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.needleCoveredGroud, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/needleCoveredGround"), true, false ));
        //TilemapData.backgroundTiles.Add(TileID.pineTreeBottom, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/pineTreeBottom"), true, true ));
        //TilemapData.backgroundTiles.Add(TileID.pineTreeTop, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/pineTreeTop"), true, true ));

        // Mountain
        TilemapData.backgroundTiles.Add(TileID.mountainSide, new TileProperties(loadTileFromPath("BiomeTiles/Mountain/mountainSide"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.rockyGround, new TileProperties(loadTileFromPath("BiomeTiles/Mountain/rockyGround"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.boulder, new TileProperties(loadTileFromPath("BiomeTiles/Mountain/boulder"), false, false ));

        // Cave
        TilemapData.backgroundTiles.Add(TileID.caveGround, new TileProperties(loadTileFromPath("BiomeTiles/Cave/caveGround"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.caveWall, new TileProperties(loadTileFromPath("BiomeTiles/Cave/caveWall"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.caveFilling, new TileProperties(loadTileFromPath("BiomeTiles/Cave/caveFilling"), false, false ));

        // Tundra
        TilemapData.backgroundTiles.Add(TileID.permafrost, new TileProperties(loadTileFromPath("BiomeTiles/Tundra/permafrost"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.tundraIce, new TileProperties(loadTileFromPath("BiomeTiles/Tundra/tundraIce"), true, false ));

        // SnowyForest
        TilemapData.backgroundTiles.Add(TileID.snowyForestGround, new TileProperties(loadTileFromPath("BiomeTiles/SnowyForest/snowyForestGround"), true, false ));
        
        // Ice Pond
        TilemapData.foregroundTiles.Add(TileID.snowyGravelPath, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snowyGravelPath"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.snowTree, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snowTree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.icePondIce, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/ice"), true, true ));
        TilemapData.backgroundTiles.Add(TileID.icePondSnow, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snow"), true, true ));

        // Volcano
        TilemapData.backgroundTiles.Add(TileID.volcanoCrackedGround, new TileProperties(loadTileFromPath("BiomeTiles/Volcano/volcanoCrackedGround"), true, true ));

        // Tropics
        TilemapData.backgroundTiles.Add(TileID.dryGrass, new TileProperties(loadTileFromPath("BiomeTiles/Tropics/dryGrass"), true, true ));

        // Desert
        TilemapData.backgroundTiles.Add(TileID.sand, new TileProperties(loadTileFromPath("BiomeTiles/Desert/sand"), true, true ));
        TilemapData.backgroundTiles.Add(TileID.crackedDirt, new TileProperties(loadTileFromPath("BiomeTiles/Desert/crackedDirt"), true, true ));

        // Secret Meadow
        TilemapData.backgroundTiles.Add(TileID.secretGrass, new TileProperties(loadTileFromPath("BiomeTiles/SecretMeadow/secretGrass"), true, true ));

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
}
