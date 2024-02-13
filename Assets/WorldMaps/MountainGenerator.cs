using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MountainGenerator : BiomeGenerator
{
    public MountainGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.Mountain;
    }

    override public void generate() {
        Debug.Log("Mountain generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);


        for (int x = boundX; x < boundX + boundWidth; x++) {
            for (int y = boundY; y < boundY + boundHeight; y++) {
                if (WorldMap.currentMap.getCellBranchID(x, y) != branchID) continue;
                if (WorldMap.currentMap.getCellBackgroundID(x, y) == TileID.persistantPath) continue;


                if (WorldMap.currentMap.getCellBranchID(x, y - 1) != branchID || WorldMap.currentMap.getCellBackgroundID(x, y - 1) == TileID.water || !WorldMap.currentMap.isValidCoord(x, y-1)) {
                    growCliffside(x, y);
                } else {
                    if (WorldMap.currentMap.getCellIsWalkable(x, y) && WorldMap.currentMap.getCellBackgroundID(x, y) != TileID.persistantPath) {
                        if (UnityEngine.Random.Range(0, 100) < 10) {
                            WorldMap.currentMap.setCellForeground(x, y, TileID.boulder);
                        }
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
            replaceTiles(encounterZone1, TileID.rockyGroundEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.dark);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }
    }

    void growCliffside(int x, int y) {
        for (int i = 0; i < 5; i++) {
            if (WorldMap.currentMap.getCellBackgroundID(x, y + i) == TileID.persistantPath) break;

            WorldMap.currentMap.setCellBackground(x, y, TileID.mountainSide);
            y++;
        }

        if (WorldMap.currentMap.getCellBackgroundID(x, y) != TileID.persistantPath)
            WorldMap.currentMap.setCellBackground(x, y, TileID.mountainSide);

    }
}
