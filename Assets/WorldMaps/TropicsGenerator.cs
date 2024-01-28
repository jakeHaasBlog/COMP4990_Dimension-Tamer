using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TropicsGenerator : BiomeGenerator
{
    public TropicsGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.Tropics;
    }

    override public void generate() {
        Debug.Log("Tropics generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);


        replaceTiles(getAllTiles(), TileID.water, false);

        List<Tuple<int, int>> islandSeeds = new List<Tuple<int, int>>();
        islandSeeds.Add(getRandomWalkableTile());
        islandSeeds.Add(getRandomWalkableTile());
        islandSeeds.Add(getRandomWalkableTile());
        islandSeeds.Add(getRandomWalkableTile());

        for (int i = 0; i < islandSeeds.Count; i++) {
            List<Tuple<int, int>> island = new List<Tuple<int, int>>();
            island.Add(islandSeeds[i]);

            for (int j = 0; j < Math.Max((boundWidth) / 10, 1); j++) {
                island = growSelection(island, 0.8f);
            }

            replaceTiles(island, TileID.dryGrass, false);
        }

        for (int x = boundX; x < boundX + boundWidth; x++) {
            for (int y = boundY; y < boundY + boundHeight; y++) {
                if (WorldMap.currentMap.getCellIsWalkable(x, y) && WorldMap.currentMap.getCellBranchID(x, y) == branchID && WorldMap.currentMap.getCellBackgroundID(x, y) != TileID.persistantPath) {

                    bool bordersWater = false;
                    if (WorldMap.currentMap.getCellBackgroundID(x - 1, y + 1) == TileID.water) bordersWater = true;
                    if (WorldMap.currentMap.getCellBackgroundID(x - 0, y + 1) == TileID.water) bordersWater = true;
                    if (WorldMap.currentMap.getCellBackgroundID(x + 1, y + 1) == TileID.water) bordersWater = true;

                    if (WorldMap.currentMap.getCellBackgroundID(x - 1, y + 0) == TileID.water) bordersWater = true;
                    if (WorldMap.currentMap.getCellBackgroundID(x - 0, y + 0) == TileID.water) bordersWater = true;
                    if (WorldMap.currentMap.getCellBackgroundID(x + 1, y + 0) == TileID.water) bordersWater = true;

                    if (WorldMap.currentMap.getCellBackgroundID(x - 1, y - 1) == TileID.water) bordersWater = true;
                    if (WorldMap.currentMap.getCellBackgroundID(x - 0, y - 1) == TileID.water) bordersWater = true;
                    if (WorldMap.currentMap.getCellBackgroundID(x + 1, y - 1) == TileID.water) bordersWater = true;


                    if (UnityEngine.Random.Range(0, 100) < 30 && bordersWater) {
                        WorldMap.currentMap.setCellForeground(x, y, TileID.palmTree);
                    }
                }
            }

        }


        for (int j = 0; j < 4; j++) {
            List<Tuple<int, int>> encounterZone1 = new List<Tuple<int, int>>();
            encounterZone1.Add(getRandomWalkableTile());

            for (int i = 0; i < 10; i++) {
                encounterZone1 = growSelection(encounterZone1, 0.2f);
            }
            replaceTiles(encounterZone1, TileID.tropicsWheatEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.grass);
            elements.Add(ElementalType.fire);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }

    }
}
