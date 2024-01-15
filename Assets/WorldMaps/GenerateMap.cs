using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GenerateMap : MonoBehaviour {

    public Tilemap foregroundTilemap;
    public Tilemap backgroundTilemap;
    public GenerateStartingWorld startingWorldGenerator;
    public GenerateEndingWorld endingWorldGenerator;

    public GameObject scientists;

    void Start()
    {
        TilemapData.backgroundTilemap = backgroundTilemap;
        TilemapData.foregroundTilemap = foregroundTilemap;
        TilemapData.foregroundTiles = new Dictionary<TileID, TileProperties>();
        TilemapData.backgroundTiles = new Dictionary<TileID, TileProperties>();

        TilemapData.loadTiles();

        generateWorld(WorldMap.currentMap.worldNumber);

    }

    public void generateWorld(int worldNumber) {

        WorldMap.currentMap.clearAllCells();
        PortalManager.instance.removePortals();

        if (worldNumber < 0 || worldNumber > 6) {
            Debug.Log("Error: Trying to set invalid world number: " + worldNumber);
            return;
        }

        WorldMap.currentMap.worldNumber = worldNumber;

        if (WorldMap.currentMap.worldNumber == 0) {
            startingWorldGenerator.generate();
        } else if (WorldMap.currentMap.worldNumber == 6) {
            endingWorldGenerator.generate();
        } else {
            generate(); // procedurally generate map
        }

        if (WorldMap.currentMap.worldNumber == 0) {
            scientists.SetActive(true);
        } else {
            scientists.SetActive(false);
        }
    }

    public void generate() {

        // reset encounter zone data
        CreatureManager.instance.encounterZones.Clear();
        CreatureManager.instance.encounterZoneElements.Clear();
        CreatureManager.instance.encounterZoneLevel.Clear();

        // custom data structure to hold node graph
        // nodes have x, y position
        // nodes have biome integer
        BiomeGraph graph = new BiomeGraph();

        // parameters: graphBoundaries  edgeLength minDist startingNodePos max branch length
        // - could randomize these values to make each world have differnet feel
        //graph.generateMapGraph(new Vector4(10, 10, 480, 480), 10, 9.8f, new Vector2(WorldMap.WIDTH / 2, WorldMap.HEIGHT / 2), 5); <-- normal generation backup
        int nodeMinDistFromEdge = 15;
        int boundsX = WorldMap.WATER_RING_THICKNESS + nodeMinDistFromEdge;
        int boundsY = WorldMap.WATER_RING_THICKNESS + nodeMinDistFromEdge;
        int boundsWidth = WorldMap.WIDTH - WorldMap.WATER_RING_THICKNESS * 2 - nodeMinDistFromEdge * 2;
        int boundsHeight = WorldMap.HEIGHT - WorldMap.WATER_RING_THICKNESS * 2 - nodeMinDistFromEdge * 2;
        float edgeLength = 15;
        float minNodeDistance = 15.5f;
        graph.generateMapGraph(new Vector4(boundsX, boundsY, boundsWidth, boundsHeight), minNodeDistance, edgeLength, new Vector2(WorldMap.WIDTH / 2, WorldMap.HEIGHT / 2), 4);


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

        float maxConnectionDist = minNodeDistance * 1.5f;
        for (int a = 0; a < graph.getNodeCount(); a++) {
            for (int b = 0; b < graph.getNodeCount(); b++) {
                if (a == b) continue;
                if (graph.getNode(a).distToSquared(graph.getNode(b).x, graph.getNode(b).y) > maxConnectionDist * maxConnectionDist) continue;
                if (UnityEngine.Random.Range(0, 100) > 10) continue; // connects 10% of nodes

                graph.connectNodes(a, b);
            }
        }


        // clear all tiles
        WorldMap.currentMap.clearAllCells();

        // create path with tiles along every edge (will ensure connectivity between all biomes)
        // is random at the moment
        for (int node = 0; node < graph.getNodeCount(); node++) {

            float ax = graph.getNode(node).x;
            float ay = graph.getNode(node).y;
            for (int connection = 0; connection < graph.getNode(node).connections.Count; connection++) {
                float bx = graph.getNode(graph.getNode(node).connections[connection]).x;
                float by = graph.getNode(graph.getNode(node).connections[connection]).y;

                LevelEditor.placeLine(TileID.persistantPath, false, (int)ax, (int)ay, (int)bx, (int)by);
            }
        }


        // fill shape of every biome starting from its node, when meeting another biome, create border tile (and don't overwrite other biome's tiles)
        // use round robin to expand each biome at the same rate (subject to change; could have some biomes expand faster than others, could have some biomes expand in a certain shape, etc..)
        // every biome will have similar shape at this point
        // can then fill biomes from borders-in to make more specific shape and add rivers

        bool stillGrowing = true;
        while (stillGrowing) {
            stillGrowing = false;
            for (int i = 0; i < graph.getNodeCount(); i++) {
                if (growNode(graph, i, biomeMappings)) {
                    stillGrowing = true;
                }
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
                    biomeGenerators.Add(new GrasslandGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Forest:
                    biomeGenerators.Add(new ForestGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch));
                    break;
                case BiomeID.PineForest:
                    biomeGenerators.Add(new PineForestGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Mountain:
                    biomeGenerators.Add(new MountainGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Cave:
                    biomeGenerators.Add(new CaveGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Tundra:
                    biomeGenerators.Add(new TundraGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.SnowyForest:
                    biomeGenerators.Add(new SnowyForestGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.IcePond:
                    biomeGenerators.Add(new IcePondGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Volcano:
                    biomeGenerators.Add(new VolcanoGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Tropics:
                    biomeGenerators.Add(new TropicsGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.Desert:
                    biomeGenerators.Add(new DesertGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
                case BiomeID.SecretMeadow:
                    biomeGenerators.Add(new SecretMeadowGenerator(selectionX, selectionY, selectionWidth, selectionHeight, currentBranch)); 
                    break;
            }

        }

        // Generation order might matter for some biomes, can remove generators from list after generate is called
        for (int i = 0; i < biomeGenerators.Count; i++) {
            biomeGenerators[i].generate();

            // add portal to some biomes
            if (UnityEngine.Random.Range(0, 100) < 20) {
                biomeGenerators[i].addPortal();
            }
        }


        // any unset tiles are already filled with water

        return;
    }

    bool growNode(BiomeGraph graph, int node, List<BiomeID> biomeMappings) {

        int branchID = graph.getNode(node).branchID; // will expand based on unmapped biome (each branch instead of each biome)

        WorldMap.currentMap.setCellBiomeID((int)graph.getNode(node).x, (int)graph.getNode(node).y, branchID);

        // gets the full containment of the branch tiles in a rectangle
        int selectionX = 0, selectionY = 0, selectionWidth = 0, selectionHeight = 0;
        getBranchSelection((int)graph.getNode(node).x, (int)graph.getNode(node).y, branchID, ref selectionX, ref selectionY, ref selectionWidth, ref selectionHeight);
        

        // for every tile in selection: 
            // if it is empty and borders a tile with same biome, have chance to fill it (chance will make shape non-uniform)
            // if it is not empty, is in this biome, and borders a different biome, then this is a border tile

        bool canGrow = false;
        for (int x = selectionX; x < selectionX + selectionWidth; x++) {
            for (int y = selectionY; y < selectionY + selectionHeight; y++) {

                if (x < 0 || x >= WorldMap.WIDTH) continue;
                if (y < 0 || y >= WorldMap.HEIGHT) continue;

                if (WorldMap.currentMap.getCellBranchID(x, y) == -1 && tileBorderContainsBiome(x, y, branchID)) {
                    canGrow = true;
                    if (UnityEngine.Random.Range(0, 10) > 6) {

                        BiomeID biome = biomeMappings[branchID];

                        // Only change background tile when it is not a path
                        if (WorldMap.currentMap.getCellBackgroundID(x, y) != TileID.persistantPath){
                            if (biome == BiomeID.Grasslands) WorldMap.currentMap.setCellBackground(x, y, TileID.grasslandGrass);
                            else if (biome == BiomeID.Forest) WorldMap.currentMap.setCellBackground(x, y, TileID.grass);
                            else if (biome == BiomeID.PineForest) WorldMap.currentMap.setCellBackground(x, y, TileID.pineForestDirt);
                            else if (biome == BiomeID.Mountain) WorldMap.currentMap.setCellBackground(x, y, TileID.rockyGroundEZ);
                            else if (biome == BiomeID.Cave) WorldMap.currentMap.setCellBackground(x, y, TileID.caveGround);
                            else if (biome == BiomeID.Tundra) WorldMap.currentMap.setCellBackground(x, y, TileID.permafrost);
                            else if (biome == BiomeID.SnowyForest) WorldMap.currentMap.setCellBackground(x, y, TileID.snowyForestGround);
                            else if (biome == BiomeID.IcePond) WorldMap.currentMap.setCellBackground(x, y, TileID.icePondIce);
                            else if (biome == BiomeID.Volcano) WorldMap.currentMap.setCellBackground(x, y, TileID.volcanoCrackedGround);
                            else if (biome == BiomeID.Tropics) WorldMap.currentMap.setCellBackground(x, y, TileID.dryGrass);
                            else if (biome == BiomeID.Desert) WorldMap.currentMap.setCellBackground(x, y, TileID.sand);
                            else if (biome == BiomeID.SecretMeadow) WorldMap.currentMap.setCellBackground(x, y, TileID.secretGrass);
                            else WorldMap.currentMap.setCellBackground(x, y, TileID.water);
                        }

                        WorldMap.currentMap.setCellBiomeID(x, y, branchID);
                    }
                }

            }
        }

        return canGrow;

    }

    // returns a bounding that fully contains the tiles in a specified branch
    void getBranchSelection(int x, int y, int branchID, ref int selectionX, ref int selectionY, ref int selectionWidth, ref int selectionHeight) {

        if (branchID == -1) return;

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
                if (WorldMap.currentMap.getCellBranchID(xi, yi) == branchID) return true;
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
                    if (WorldMap.currentMap.getCellBranchID(xi, yi) != branchID && WorldMap.currentMap.getCellBranchID(xi, yi) != -1) return true;
                } else {
                    if (WorldMap.currentMap.getCellBranchID(xi, yi) == branchID) return true;
                }

            }
        }

        return false;
    }

};

