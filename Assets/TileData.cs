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
    labFloor = 0,
    labWall = 1,
    bookshelfBottom = 2,
    bookshelfTop = 3,
    labWallFront = 4,
    lockerTop = 5,
    lockerBottom = 6,
    labTable = 7,
    labTable1 = 8,
    labTable2 = 9,
    labTable3 = 10,
    labTable4 = 11,
    starting_grass1 = 12,
    starting_grass2 = 13,
    starting_grass3 = 14,
    starting_grass4 = 15,
    starting_grass5 = 16,
    none = 17,
    water = 18,
    persistantPath = 19,
    endWorldGround = 20,
    endWorldWall = 21,
    endWorldBG1 = 22,
    endWorldBG2 = 23,
    endWorldBG3 = 24,
    endWorldInterlock = 25,
    endWorldPillarBottom = 26,
    endWorldPillarMiddle = 27,
    endWorldPillarTop = 28,
    endWorldGroundEdgeBottom = 29,
    endWorldGroundEdgeTL = 30,
    endWorldGroundEdgeTR = 31,
    endWorldGroundEdgeBL = 32,
    endWorldGroundEdgeBR = 33,
    endWorldGroundEdgeTop = 34,
    endWorldGroundEdgeLeft = 35,
    endWorldGroundEdgeRight = 36,

    // Grassland
    grasslandGrass = 300,
    grasslandTallGrass = 301,
    grasslandDarkGrassEZ = 302,

    // Forest
    dirt = 402,
    grass = 403,
    rock = 404,
    tree = 405,
    gravelPath = 406,
    tallGrassEZ = 407,

    // PineForest
    pineForestDirt = 500,
    needleCoveredGroud = 501,
    pineForestShrubEZ = 502,
    pineTreeBottom = 503,
    pineTreeTop = 504,

    // Mountain
    mountainSide = 600,
    rockyGroundEZ = 601,
    boulder = 602,
    mountainGround = 603,

    // Cave
    caveGround = 700,
    caveWall = 701,
    caveFilling = 702,
    caveRoughGroundEZ = 703,

    // Tundra
    permafrost = 800,
    tundraIce = 801,
    tundraRoughGroundEZ = 802,
    tundraGrass = 803,
    tundraRedGrass = 804,
    tundraRocks = 805,

    // SnowyForest
    snowTree = 900,
    snowyNeedles = 901,
    snowyForestGround = 902,
    snowyForestDeepSnowEZ = 903,


    // Ice Pond
    icePondTree = 1000,
    icePondIce = 1001,
    icePondSnow = 1002,
    snowyGravelPath = 1003,
    icePondDeepSnowEZ = 1004,

    // Volcano
    volcanoRockyGround = 1100,
    volcanoCrackedGround = 1101,
    volcanoLavaPitBottom = 1102,
    volcanoLavaPitTop = 1103,
    volcanoLavaPitMiddle = 1104,
    volcanoDeepSiltEZ = 1105,

    // Tropics
    palmTree = 1200,
    dryGrass = 1201,
    tropicsWheatEZ = 1202,

    // Desert
    sand = 1300,
    crackedDirt = 1301,
    darkSandEZ = 1302,

    // Secret Meadow
    secretGrass = 1400,
    secretGrass2 = 1401,
    secretGrass3 = 1402,
    secretRock = 1403,
    meadowFlowersEZ = 1404,
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
    public static bool loadFromServer;


    public static Tile loadTileFromPath(String path) {

        //if (!loadFromServer) {
            Texture2D tileTexture = Resources.Load<Texture2D>(path);
            if (tileTexture == null) Debug.Log("Could not find texture at: " + path);
            int pixelsPerUnit = tileTexture.width;
            Sprite tileSprite = Sprite.Create(tileTexture, new Rect(0, 0, tileTexture.width, tileTexture.height), new Vector2(0, 0), pixelsPerUnit, 0, SpriteMeshType.Tight);

            Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
            tile.sprite = tileSprite;

            return tile;
        //} else {

            // request image: "server/c:/images/" + path

            // Create sprite from loaded image

            // create Tile from new Sprite

            // return new Tile

        //}
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
        TilemapData.backgroundTiles.Add(TileID.grasslandDarkGrassEZ, new TileProperties(loadTileFromPath("BiomeTiles/Grassland/darkGrass-ez"), true, true ));

        // Forest
        TilemapData.foregroundTiles.Add(TileID.rock, new TileProperties(loadTileFromPath("BiomeTiles/Forest/rock"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.gravelPath, new TileProperties(loadTileFromPath("BiomeTiles/Forest/gravelPath"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.tree, new TileProperties(loadTileFromPath("BiomeTiles/Forest/tree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.dirt, new TileProperties(loadTileFromPath("BiomeTiles/Forest/dirt"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.grass, new TileProperties(loadTileFromPath("BiomeTiles/Forest/grass"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.tallGrassEZ, new TileProperties(loadTileFromPath("BiomeTiles/Forest/tallGrass-ez"), true, true ));

        // PineForest
        TilemapData.backgroundTiles.Add(TileID.pineForestDirt, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/pineForestDirt"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.needleCoveredGroud, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/needleCoveredGround"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.pineForestShrubEZ, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/pineForestShrub-ez"), true, true ));
        TilemapData.foregroundTiles.Add(TileID.pineTreeBottom, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/pineTreeBottom"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.pineTreeTop, new TileProperties(loadTileFromPath("BiomeTiles/PineForest/pineTreeTop"), false, false ));

        // Mountain
        TilemapData.backgroundTiles.Add(TileID.mountainSide, new TileProperties(loadTileFromPath("BiomeTiles/Mountain/mountainSide"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.rockyGroundEZ, new TileProperties(loadTileFromPath("BiomeTiles/Mountain/rockyGround-ez"), true, true ));
        TilemapData.foregroundTiles.Add(TileID.boulder, new TileProperties(loadTileFromPath("BiomeTiles/Mountain/boulder"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.mountainGround, new TileProperties(loadTileFromPath("BiomeTiles/Mountain/mountainGround"), true, false ));

        // Cave
        TilemapData.backgroundTiles.Add(TileID.caveGround, new TileProperties(loadTileFromPath("BiomeTiles/Cave/caveGround"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.caveWall, new TileProperties(loadTileFromPath("BiomeTiles/Cave/caveWall"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.caveFilling, new TileProperties(loadTileFromPath("BiomeTiles/Cave/caveFilling"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.caveRoughGroundEZ, new TileProperties(loadTileFromPath("BiomeTiles/Cave/roughGround-ez"), true, true ));

        // Tundra
        TilemapData.backgroundTiles.Add(TileID.permafrost, new TileProperties(loadTileFromPath("BiomeTiles/Tundra/permafrost"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.tundraIce, new TileProperties(loadTileFromPath("BiomeTiles/Tundra/tundraIce"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.tundraRoughGroundEZ, new TileProperties(loadTileFromPath("BiomeTiles/Tundra/roughGround-ez"), true, true ));
        TilemapData.backgroundTiles.Add(TileID.tundraGrass, new TileProperties(loadTileFromPath("BiomeTiles/Tundra/tundraGrass"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.tundraRedGrass, new TileProperties(loadTileFromPath("BiomeTiles/Tundra/tundraRedGrass"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.tundraRocks, new TileProperties(loadTileFromPath("BiomeTiles/Tundra/tundraRocks"), true, false ));

        // SnowyForest
        TilemapData.backgroundTiles.Add(TileID.snowyForestGround, new TileProperties(loadTileFromPath("BiomeTiles/SnowyForest/snowyForestGround"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.snowyForestDeepSnowEZ, new TileProperties(loadTileFromPath("BiomeTiles/SnowyForest/deepSnow-ez"), true, true ));
        TilemapData.foregroundTiles.Add(TileID.snowTree, new TileProperties(loadTileFromPath("BiomeTiles/SnowyForest/snowTree"), false, false ));
        
        // Ice Pond
        TilemapData.foregroundTiles.Add(TileID.snowyGravelPath, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snowyGravelPath"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.icePondTree, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snowTree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.icePondIce, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/ice"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.icePondSnow, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/snow"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.icePondDeepSnowEZ, new TileProperties(loadTileFromPath("BiomeTiles/IcePond/deepSnow-ez"), true, true ));

        // Volcano
        TilemapData.backgroundTiles.Add(TileID.volcanoCrackedGround, new TileProperties(loadTileFromPath("BiomeTiles/Volcano/volcanoCrackedGround"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.volcanoDeepSiltEZ, new TileProperties(loadTileFromPath("BiomeTiles/Volcano/volcanoDeepSilt-ez"), true, true ));
        TilemapData.backgroundTiles.Add(TileID.volcanoLavaPitBottom, new TileProperties(loadTileFromPath("BiomeTiles/Volcano/volcanoLavaPitBottom"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.volcanoLavaPitMiddle, new TileProperties(loadTileFromPath("BiomeTiles/Volcano/volcanoLavaPitMiddle"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.volcanoLavaPitTop, new TileProperties(loadTileFromPath("BiomeTiles/Volcano/volcanoLavaPitTop"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.volcanoRockyGround, new TileProperties(loadTileFromPath("BiomeTiles/Volcano/volcanoRockyGround"), false, false ));

        // Tropics
        TilemapData.foregroundTiles.Add(TileID.palmTree, new TileProperties(loadTileFromPath("BiomeTiles/Tropics/palmTree"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.dryGrass, new TileProperties(loadTileFromPath("BiomeTiles/Tropics/dryGrass"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.tropicsWheatEZ, new TileProperties(loadTileFromPath("BiomeTiles/Tropics/tropicsWheat-ez"), true, true ));

        // Desert
        TilemapData.backgroundTiles.Add(TileID.sand, new TileProperties(loadTileFromPath("BiomeTiles/Desert/sand"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.crackedDirt, new TileProperties(loadTileFromPath("BiomeTiles/Desert/crackedDirt"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.darkSandEZ, new TileProperties(loadTileFromPath("BiomeTiles/Desert/darkSand-ez"), true, true ));

        // Secret Meadow
        TilemapData.backgroundTiles.Add(TileID.secretGrass, new TileProperties(loadTileFromPath("BiomeTiles/SecretMeadow/secretGrass"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.secretGrass2, new TileProperties(loadTileFromPath("BiomeTiles/SecretMeadow/secretGrass2"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.secretGrass3, new TileProperties(loadTileFromPath("BiomeTiles/SecretMeadow/secretGrass3"), true, false ));
        TilemapData.backgroundTiles.Add(TileID.meadowFlowersEZ, new TileProperties(loadTileFromPath("BiomeTiles/SecretMeadow/meadowFlowers-ez"), true, true ));

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

        // Ending World
        TilemapData.backgroundTiles.Add(TileID.endWorldBG1, new TileProperties(loadTileFromPath("EndingWorldTiles/background1"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.endWorldBG2, new TileProperties(loadTileFromPath("EndingWorldTiles/background2"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.endWorldBG3, new TileProperties(loadTileFromPath("EndingWorldTiles/background3"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.endWorldInterlock, new TileProperties(loadTileFromPath("EndingWorldTiles/interlock"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldPillarTop, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldPillarTop"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldPillarMiddle, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldPillarMiddle"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldPillarBottom, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldPillarBottom"), false, false ));
        TilemapData.backgroundTiles.Add(TileID.endWorldGround, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGround"), true, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldGroundEdgeBottom, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGroundEdgeBottom"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldGroundEdgeTop, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGroundEdgeTop"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldGroundEdgeLeft, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGroundEdgeLeft"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldGroundEdgeRight, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGroundEdgeRight"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldGroundEdgeTL, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGroundEdgeTL"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldGroundEdgeTR, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGroundEdgeTR"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldGroundEdgeBL, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGroundEdgeBL"), false, false ));
        TilemapData.foregroundTiles.Add(TileID.endWorldGroundEdgeBR, new TileProperties(loadTileFromPath("EndingWorldTiles/endWorldGroundEdgeBR"), false, false ));


    }
}
