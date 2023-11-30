using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum BiomeID {
    none,
    Grasslands,
    Forest,
    PineForest,
    Mountain,
    Cave,
    Tundra,
    SnowyForest,
    IcePond,
    Volcano,
    Tropics,
    Desert,
    SecretMeadow,
}


class WorldCell {
    public TileID backgroundTileID = TileID.none;
    public TileID foregroundTileID = TileID.none;
    public int encounterZoneID = -1;
    public int branchID = -1;
};

public class WorldMap {

    public const int WIDTH = 500;
    public const int HEIGHT = 500;

    public WorldMap() {
        worldCells = new WorldCell[WIDTH,HEIGHT];
        for (int i = 0; i < WIDTH; i++) {
            for (int j = 0; j < HEIGHT; j++) {
                worldCells[i, j] = new WorldCell();
            }
        }
    }

    // returns true if the given coordinate is within the circular map
    public bool isValidCoord(int x, int y) {
        //return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT; <-- old function when world was square

        float rad = (float)WIDTH / 2;

        float tx = (float)x - (float)WIDTH / 2;
        float ty = (float)y - (float)HEIGHT / 2;
        if (tx * tx + ty * ty < rad * rad) {
            return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
        }

        return false;
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

    public void setCellBiomeID(int x, int y, int branchID) {
        worldCells[x, y].branchID = branchID;
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

    public int getCellBiomeID(int x, int y) {
        return worldCells[x, y].branchID;
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

    void Start()
    {
        worldMap = new WorldMap();

        TilemapData.backgroundTilemap = backgroundTilemap;
        TilemapData.foregroundTilemap = foregroundTilemap;
        TilemapData.foregroundTiles = new Dictionary<TileID, TileProperties>();
        TilemapData.backgroundTiles = new Dictionary<TileID, TileProperties>();

        TilemapData.loadTiles();

        generateWorld(currentWorldNum);

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
            generate(); // procedurally generate map
        }
    }


    // ensures all plotted tiles have a cardinal neighbour
    private void placeLinePoint(TileID tile, bool foreground, int x, int y) {
        if (foreground) worldMap.setCellForeground(x, y, tile);
        else worldMap.setCellBackground(x, y, tile);
        
        // if top-left has no neighbors, fill top cell
        if (worldMap.getTileBackgroundID(x - 1, y + 1) == tile)
            if (worldMap.getTileBackgroundID(x, y + 1) != tile && worldMap.getTileBackgroundID(x - 1, y) != tile) worldMap.setCellBackground(x, y + 1, tile);

        // if top-right has no neighbors, fill top cell
        if (worldMap.getTileBackgroundID(x + 1, y + 1) == tile)
            if (worldMap.getTileBackgroundID(x, y + 1) != tile && worldMap.getTileBackgroundID(x + 1, y) != tile) worldMap.setCellBackground(x, y + 1, tile);

        // if bottom-left has no neighbors, fill bottom cell
        if (worldMap.getTileBackgroundID(x - 1, y - 1) == tile)
            if (worldMap.getTileBackgroundID(x, y - 1) != tile && worldMap.getTileBackgroundID(x - 1, y) != tile) worldMap.setCellBackground(x, y - 1, tile);

        // if bottom-right has no neighbors, fill bottom cell
        if (worldMap.getTileBackgroundID(x + 1, y - 1) == tile)
            if (worldMap.getTileBackgroundID(x, y - 1) != tile && worldMap.getTileBackgroundID(x + 1, y) != tile) worldMap.setCellBackground(x, y - 1, tile);

    }

    // Thank you Wikipedia for this implementation of Bresenham's line algorithm
    // This function will ensure all generated paths are walkable
    public void placeLine(TileID tile, bool foreground, int x0, int y0, int x1, int y1) {
        
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

    public void generate() {

        // place portal
        portal.transform.position = new Vector3(WorldMap.WIDTH / 2, WorldMap.HEIGHT / 2, portal.transform.position.z);

        // custom data structure to hold node graph
        // nodes have x, y position
        // nodes have biome integer
        BiomeGraph graph = new BiomeGraph();

        // parameters: graphBoundaries  edgeLength minDist startingNodePos max branch length
        // - could randomize these values to make each world have differnet feel
        //graph.generateMapGraph(new Vector4(10, 10, 480, 480), 10, 9.8f, new Vector2(WorldMap.WIDTH / 2, WorldMap.HEIGHT / 2), 5); <-- normal generation backup
        graph.generateMapGraph(new Vector4(20, 20, 460, 460), 15, 14.5f, new Vector2(WorldMap.WIDTH / 2, WorldMap.HEIGHT / 2), 4);


        // BranchIDs have been set for each node, now choose what biome belongs to which branch based on rules (i.e. ice pond should not be next to a volcano, secret meadow should be on leaf)
        // For now, randomly assign biomes
        // In the future, could use a naive approach because its a fast operation (randomly assign biomes, then validate and try again if conditions are not met)
        List<BiomeID> biomeMappings = new List<BiomeID>();
        Array biomes = Enum.GetValues(typeof(BiomeID));
        for (int i = 0; i < graph.getNodeCount() * 2; i++) {
            biomeMappings.Add((BiomeID)biomes.GetValue(UnityEngine.Random.Range(1, biomes.Length)));
        }

        // We need to connect the biomes to eachother to make loops (base on rules, ie. secret meadow only has one entrance, ice pond should have many)
        // look through each node in the graph, find all neighbors in a radius
        // if the neighbor meets conditions for connection, give it a chance to create a connection (edge)


        // clear all tiles
        worldMap.clearAllCells();

        // create path with tiles along every edge (will ensure connectivity between all biomes)
        for (int node = 0; node < graph.getNodeCount(); node++) {
            float ax = graph.getNode(node).x;
            float ay = graph.getNode(node).y;
            for (int connection = 0; connection < graph.getNode(node).connections.Count; connection++) {
                float bx = graph.getNode(graph.getNode(node).connections[connection]).x;
                float by = graph.getNode(graph.getNode(node).connections[connection]).y;

                placeLine(TileID.persistantPath, false, (int)ax, (int)ay, (int)bx, (int)by);
            }
        }


        // fill shape of every biome starting from its node, when meeting another biome, create border tile (and don't overwrite other biome's tiles)
        // use round robin to expand each biome at the same rate (subject to change; could have some biomes expand faster than others, could have some biomes expand in a certain shape, etc..)
        // every biome will have similar shape at this point
        // can then fill biomes from borders-in to make more specific shape and add rivers

        for (int growingRadius = 0; growingRadius < 30; growingRadius++) {
            for (int i = 0; i < graph.getNodeCount(); i++) {
                growNode(graph, i, biomeMappings);
            }
        }



        // run generator for each branch
        // - the biome generators will be given the containment of their branch

        HashSet<int> visitedBranches = new HashSet<int>();
        List<BiomeGenerator> biomeGenerators = new List<BiomeGenerator>();

        for (int i = 0; i < graph.getNodeCount(); i++) {
            int currentBranch = graph.getNode(i).branchID;

            // if this branch has already been seen, continue
            if (visitedBranches.Contains(currentBranch)) continue;
            visitedBranches.Add(currentBranch);

            // get the containment of this branch, then store its biomeGenerator in a list
            int selectionX = 0, selectionY = 0, selectionWidth = 0, selectionHeight = 0;
            getBranchSelection((int)graph.getNode(i).x, (int)graph.getNode(i).y, currentBranch, ref selectionX, ref selectionY, ref selectionWidth, ref selectionHeight);
        
            switch (biomeMappings[currentBranch]) {
                case BiomeID.Grasslands: 
                    //biomeGenerators.add(new GenGrassland(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Forest:
                    //biomeGenerators.add(new GenForest(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.PineForest:
                    //biomeGenerators.add(new GenPineForest(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Mountain:
                    //biomeGenerators.add(new GenMountain(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Cave:
                    //biomeGenerators.add(new GenCave(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Tundra:
                    //biomeGenerators.add(new GenTundra(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.SnowyForest:
                    //biomeGenerators.add(new GenSnowyForest(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.IcePond:
                    biomeGenerators.Add(new GenIcePond(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Volcano:
                    //biomeGenerators.add(new GenVolcano(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Tropics:
                    //biomeGenerators.add(new GenSnowyTropics(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Desert:
                    //biomeGenerators.add(new GenDesert(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.SecretMeadow:
                    //biomeGenerators.add(new GenSecretMeadow(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
            }

        }

        // Generation order might matter for some biomes, can remove generators from list after generate is called
        for (int i = 0; i < biomeGenerators.Count; i++) {
            biomeGenerators[i].generate();
        }


        // fill any unset tiles with water

        return;
    }

    void growNode(BiomeGraph graph, int node, List<BiomeID> biomeMappings) {
        int branchID = graph.getNode(node).branchID; // will expand based on unmapped biome (each branch instead of each biome)

        worldMap.setCellBiomeID((int)graph.getNode(node).x, (int)graph.getNode(node).y, branchID);

        // gets the full containment of the branch tiles in a rectangle
        int selectionX = 0, selectionY = 0, selectionWidth = 0, selectionHeight = 0;
        getBranchSelection((int)graph.getNode(node).x, (int)graph.getNode(node).y, branchID, ref selectionX, ref selectionY, ref selectionWidth, ref selectionHeight);
        
        // for every tile in selection: 
            // if it is empty and borders a tile with same biome, have chance to fill it (chance will make shape non-uniform)
            // if it is not empty, is in this biome, and borders a different biome

        for (int x = selectionX; x < selectionX + selectionWidth; x++) {
            for (int y = selectionY; y < selectionY + selectionHeight; y++) {

                if (x < 0 || x >= WorldMap.WIDTH) continue;
                if (y < 0 || y >= WorldMap.HEIGHT) continue;

                if (worldMap.getCellBiomeID(x, y) == -1 && tileBorderContainsBiome(x, y, branchID)) {
                    if (UnityEngine.Random.Range(0, 10) > 6) {

                        BiomeID biome = biomeMappings[branchID];

                        // Only change background tile when it is not a path
                        if (worldMap.getTileBackgroundID(x, y) != TileID.persistantPath){
                            if (biome == BiomeID.Grasslands) worldMap.setCellBackground(x, y, TileID.grasslandGrass);
                            else if (biome == BiomeID.Forest) worldMap.setCellBackground(x, y, TileID.grass);
                            else if (biome == BiomeID.PineForest) worldMap.setCellBackground(x, y, TileID.pineForestDirt);
                            else if (biome == BiomeID.Mountain) worldMap.setCellBackground(x, y, TileID.rockyGround);
                            else if (biome == BiomeID.Cave) worldMap.setCellBackground(x, y, TileID.caveGround);
                            else if (biome == BiomeID.Tundra) worldMap.setCellBackground(x, y, TileID.permafrost);
                            else if (biome == BiomeID.SnowyForest) worldMap.setCellBackground(x, y, TileID.snowyForestGround);
                            else if (biome == BiomeID.IcePond) worldMap.setCellBackground(x, y, TileID.icePondIce);
                            else if (biome == BiomeID.Volcano) worldMap.setCellBackground(x, y, TileID.volcanoCrackedGround);
                            else if (biome == BiomeID.Tropics) worldMap.setCellBackground(x, y, TileID.dryGrass);
                            else if (biome == BiomeID.Desert) worldMap.setCellBackground(x, y, TileID.sand);
                            else if (biome == BiomeID.SecretMeadow) worldMap.setCellBackground(x, y, TileID.secretGrass);
                            else worldMap.setCellBackground(x, y, TileID.water);
                        }

                        worldMap.setCellBiomeID(x, y, branchID);
                    }
                }

            }
        }

    }

    // returns a selection that fully contains the tiles in a specified branch (edges do not contain branch tiles)
    void getBranchSelection(int x, int y, int branchID, ref int selectionX, ref int selectionY, ref int selectionWidth, ref int selectionHeight) {
        selectionX = x; 
        selectionY = y;
        selectionWidth = 1;
        selectionHeight = 1;

        // expand selection until none of the selection edges contain tiles from this biome
        bool edgesAreEmpty = false;
        while (!edgesAreEmpty) {
            edgesAreEmpty = true;

            // if left edge contains a biome tile, move left edge further left
            if (areaContainsBiomeTile(selectionX, selectionY, 1, selectionHeight, branchID)) {
                selectionX--;
                selectionWidth++;
                edgesAreEmpty = false;
            }

            // if right edge contains a biome tile, move right edge further right
            if (areaContainsBiomeTile(selectionX + selectionWidth - 1, selectionY, 1, selectionHeight, branchID)) {
                selectionWidth++;
                edgesAreEmpty = false;
            }

            // if top edge contains a biome tile, move top edge further up
            if (areaContainsBiomeTile(selectionX, selectionY + selectionHeight - 1, selectionWidth, 1, branchID)) {
                selectionHeight++;
                edgesAreEmpty = false;
            }

            // if bottom edge contains a biome tile, move bottom edge further down
            if (areaContainsBiomeTile(selectionX, selectionY, selectionWidth, 1, branchID)) {
                selectionY--;
                selectionHeight++;
                edgesAreEmpty = false;
            }

        }
    }

    bool areaContainsBiomeTile(int x, int y, int width, int height, int branchID) {
        for (int xi = x; xi < x + width; xi++) {
            for (int yi = y; yi < y + height; yi++) {
                if (xi < 0 || xi >= WorldMap.WIDTH) continue;
                if (yi < 0 || yi >= WorldMap.HEIGHT) continue;
                if (worldMap.getCellBiomeID(xi, yi) == branchID) return true;
            }
        }
        return false;
    }

    bool tileBorderContainsBiome(int x, int y, int branchID, bool checkContainsOtherBiome = false) {
        for (int xi = x - 1; xi <= x + 1; xi++){
            for (int yi = y - 1; yi <= y + 1; yi++){
                if (xi == x && yi == y) continue;

                if (xi < 0 || xi >= WorldMap.WIDTH) continue;
                if (yi < 0 || yi >= WorldMap.HEIGHT) continue;

                if (xi != x && yi != y) continue; // prevents checking diagonals

                if (checkContainsOtherBiome) {
                    if (worldMap.getCellBiomeID(xi, yi) != branchID && worldMap.getCellBiomeID(xi, yi) != -1) return true;
                } else {
                    if (worldMap.getCellBiomeID(xi, yi) == branchID) return true;
                }

            }
        }

        return false;
    }

};

