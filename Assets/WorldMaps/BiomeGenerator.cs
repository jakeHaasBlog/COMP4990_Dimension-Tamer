using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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

// Abstract class that is the basis for each specific biome's generation algorithm
public abstract class BiomeGenerator {

    private static int encounterZoneCounter = 0;
    protected int boundX, boundY, boundWidth, boundHeight;
    protected int branchID;

    public BiomeGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) {
        this.boundX = boundX;
        this.boundY = boundY;
        this.boundWidth = boundWidth;
        this.boundHeight = boundHeight;
        this.branchID = branchID;
    }


    protected void addEncounterZone(List<Tuple<int, int>> encounterZoneTiles, List<ElementalType> usableElements, int level) {

        // Add creatures to encounter zone
        List<Tuple<BiomeID, int>> creatures = new List<Tuple<BiomeID, int>>();
        creatures.Add(new Tuple<BiomeID, int>(getBiome(), UnityEngine.Random.Range(0, CreatureManager.instance.biomeCreatures[getBiome()].Count))); // update these to load random creatures from biomeCreatures lists
        creatures.Add(new Tuple<BiomeID, int>(getBiome(), UnityEngine.Random.Range(0, CreatureManager.instance.biomeCreatures[getBiome()].Count)));

        CreatureManager.instance.encounterZones.Add(encounterZoneCounter, creatures);
        CreatureManager.instance.encounterZoneElements.Add(encounterZoneCounter, usableElements);
        CreatureManager.instance.encounterZoneLevel.Add(encounterZoneCounter, level);

        // Set set encounter zone id for encounter zone tiles
        foreach (Tuple<int, int> a in encounterZoneTiles) {
            WorldMap.currentMap.setTileEncounterZoneID(a.Item1, a.Item2, encounterZoneCounter);
        }

        encounterZoneCounter++;
    }

    public abstract BiomeID getBiome();
    public abstract void generate();


    // ===== support methods =====

    protected List<Tuple<int, int>> getBorderTiles() {
        List<Tuple<int, int>> tileIndices = new List<Tuple<int, int>>();
        for (int y = boundY; y < boundY + boundHeight; y++) {
            for (int x = boundX; x < boundX + boundWidth; x++) {
                if (!WorldMap.currentMap.isValidCoord(x, y)) continue;
                if (WorldMap.currentMap.getCellBranchID(x, y) != branchID) continue;

                bool isBorderTile = false;
                if (WorldMap.currentMap.getCellBranchID(x - 1, y - 1) != branchID) isBorderTile = true;
                if (WorldMap.currentMap.getCellBranchID(x + 0, y - 1) != branchID) isBorderTile = true;
                if (WorldMap.currentMap.getCellBranchID(x + 1, y - 1) != branchID) isBorderTile = true;
                if (WorldMap.currentMap.getCellBranchID(x - 1, y + 0) != branchID) isBorderTile = true;
                
                if (WorldMap.currentMap.getCellBranchID(x + 1, y + 0) != branchID) isBorderTile = true;
                if (WorldMap.currentMap.getCellBranchID(x - 1, y + 1) != branchID) isBorderTile = true;
                if (WorldMap.currentMap.getCellBranchID(x + 0, y + 1) != branchID) isBorderTile = true;
                if (WorldMap.currentMap.getCellBranchID(x + 1, y + 1) != branchID) isBorderTile = true;

                if (isBorderTile) tileIndices.Add(new Tuple<int, int>(x, y));

            }
        }
        return tileIndices;
    }

    protected List<Tuple<int, int>> getAllTiles() {
        List<Tuple<int, int>> tileIndices = new List<Tuple<int, int>>();
        for (int y = boundY; y < boundY + boundHeight; y++) {
            for (int x = boundX; x < boundX + boundWidth; x++) {
                if (!WorldMap.currentMap.isValidCoord(x, y)) continue;

                if (WorldMap.currentMap.getCellBranchID(x, y) != branchID) continue;

                tileIndices.Add(new Tuple<int, int>(x, y));
            }
        }
        return tileIndices;
    }

    protected List<Tuple<int, int>> getFloodFill(int x, int y, TileID tileType) {
        return new List<Tuple<int, int>>();
    }

    protected List<Tuple<int, int>> growSelection(List<Tuple<int, int>> selection, float growthChance = 1.0f) {
        List<Tuple<int, int>> tileIndices = new List<Tuple<int, int>>();
        foreach (Tuple<int, int> a in selection) {
            tileIndices.Add(a);

            int neighborX = a.Item1;
            int neighborY = a.Item2 + 1;
            if (WorldMap.currentMap.getCellBranchID(neighborX, neighborY) == branchID)
                if (!tileIndices.Contains(new Tuple<int, int>(neighborX, neighborY)))
                    if (UnityEngine.Random.Range(0, 100) < (int)(growthChance * 100.0f))
                        tileIndices.Add(new Tuple<int, int>(neighborX, neighborY));

            neighborX = a.Item1 - 1;
            neighborY = a.Item2;
            if (WorldMap.currentMap.getCellBranchID(neighborX, neighborY) == branchID)
                if (!tileIndices.Contains(new Tuple<int, int>(neighborX, neighborY)))
                    if (UnityEngine.Random.Range(0, 100) < (int)(growthChance * 100))
                        tileIndices.Add(new Tuple<int, int>(neighborX, neighborY));

            neighborX = a.Item1 + 1;
            neighborY = a.Item2;
            if (WorldMap.currentMap.getCellBranchID(neighborX, neighborY) == branchID)
                if (!tileIndices.Contains(new Tuple<int, int>(neighborX, neighborY)))
                    if (UnityEngine.Random.Range(0, 100) < (int)(growthChance * 100))
                        tileIndices.Add(new Tuple<int, int>(neighborX, neighborY));

            neighborX = a.Item1;
            neighborY = a.Item2 - 1;
            if (WorldMap.currentMap.getCellBranchID(neighborX, neighborY) == branchID)
                if (!tileIndices.Contains(new Tuple<int, int>(neighborX, neighborY)))
                    if (UnityEngine.Random.Range(0, 100) < (int)(growthChance * 100))
                        tileIndices.Add(new Tuple<int, int>(neighborX, neighborY));

        }
        return tileIndices;
    }

    protected void replaceTiles(List<Tuple<int, int>> tileIndices, TileID tileType, bool foreground, bool overwriteWalkableTiles = true, bool overwriteUnWalkableTiles = true) {
        foreach (Tuple<int, int> a in tileIndices) {

            if (WorldMap.currentMap.getCellBackgroundID(a.Item1, a.Item2) == TileID.persistantPath) continue;

            if (WorldMap.currentMap.getCellIsWalkable(a.Item1, a.Item2)) {
                if (overwriteWalkableTiles) {
                    if (foreground) WorldMap.currentMap.setCellForeground(a.Item1, a.Item2, tileType);
                    else WorldMap.currentMap.setCellBackground(a.Item1, a.Item2, tileType);
                }
            } else {
                if (overwriteUnWalkableTiles) {
                    if (foreground) WorldMap.currentMap.setCellForeground(a.Item1, a.Item2, tileType);
                    else WorldMap.currentMap.setCellBackground(a.Item1, a.Item2, tileType);
                }
            }

        }
    }

    protected void replaceTiles(List<Tuple<int, int>> tileIndices, List<TileID> tileTypes, bool foreground, bool overwriteWalkableTiles = true, bool overwriteUnWalkableTiles = true) {
        foreach (Tuple<int, int> a in tileIndices) {

            if (WorldMap.currentMap.getCellBackgroundID(a.Item1, a.Item2) == TileID.persistantPath) continue;

            if (WorldMap.currentMap.getCellIsWalkable(a.Item1, a.Item2)) {
                if (overwriteWalkableTiles) {
                    if (foreground) WorldMap.currentMap.setCellForeground(a.Item1, a.Item2, tileTypes[UnityEngine.Random.Range(0, tileTypes.Count)]);
                    else WorldMap.currentMap.setCellBackground(a.Item1, a.Item2, tileTypes[UnityEngine.Random.Range(0, tileTypes.Count)]);
                }
            } else {
                if (overwriteUnWalkableTiles) {
                    if (foreground) WorldMap.currentMap.setCellForeground(a.Item1, a.Item2, tileTypes[UnityEngine.Random.Range(0, tileTypes.Count)]);
                    else WorldMap.currentMap.setCellBackground(a.Item1, a.Item2, tileTypes[UnityEngine.Random.Range(0, tileTypes.Count)]);
                }
            }

        }
    }

    protected Tuple<int, int> getRandomWalkableTile() {

        int x = 0, y = 0;
        do {
            x = UnityEngine.Random.Range(boundX, boundX + boundWidth);
            y = UnityEngine.Random.Range(boundY, boundY + boundHeight);
        } while (!WorldMap.currentMap.isValidCoord(x, y) || WorldMap.currentMap.getCellBranchID(x, y) != branchID || !WorldMap.currentMap.getCellIsWalkable(x, y));

        return new Tuple<int, int>(x, y);
    }

    protected bool validateWalkability() {
        return false;
    }

}