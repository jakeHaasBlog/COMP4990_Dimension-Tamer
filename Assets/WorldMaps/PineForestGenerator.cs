using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PineForestGenerator : BiomeGenerator
{
    public PineForestGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.PineForest;
    }

    override public void generate() {
        Debug.Log("Pine Forest generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        List<Tuple<int, int>> borderTiles = getBorderTiles();
        borderTiles = growSelection(borderTiles);        
        replaceTiles(borderTiles, TileID.water, false);


        // placing 2-heigh trees sparcely

        for (int y = boundY + boundHeight; y >= boundY; y--) {
            for (int x = boundX; x < boundX + boundWidth; x++) {

                if (WorldMap.currentMap.getCellBranchID(x, y) != branchID) continue;
                if (WorldMap.currentMap.getCellBackgroundID(x, y) == TileID.persistantPath) continue;
                if (WorldMap.currentMap.getCellBackgroundID(x, y + 1) == TileID.persistantPath) continue;
                if (!WorldMap.currentMap.getCellIsWalkable(x, y)) continue;
                if (!WorldMap.currentMap.getCellIsWalkable(x, y + 1)) continue;

                if (UnityEngine.Random.Range(0, 10) == 0) {
                    WorldMap.currentMap.setCellForeground(x, y + 1, TileID.pineTreeTop);
                    WorldMap.currentMap.setCellForeground(x, y, TileID.pineTreeBottom);
                }
            }
        }

        // placing needle-covered ground around trees
        for (int x = boundX; x < boundX + boundWidth; x++) {
            for (int y = boundY; y < boundY + boundHeight; y++) {
                if (WorldMap.currentMap.getCellBranchID(x, y) != branchID) continue;
                if (WorldMap.currentMap.getCellBackgroundID(x, y) == TileID.persistantPath) continue;

                if (WorldMap.currentMap.getCellForegroundID(x, y) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x-1, y-1) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x-1, y) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x-1, y+1) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x, y-1) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x, y) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x, y+1) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x+1, y-1) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x+1, y) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x+1, y+1) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);

                if (WorldMap.currentMap.getCellForegroundID(x, y+2) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x, y-2) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x-2, y) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
                if (WorldMap.currentMap.getCellForegroundID(x+2, y) == TileID.pineTreeBottom) WorldMap.currentMap.setCellBackground(x, y, TileID.needleCoveredGroud);
            }
        }


        for (int j = 0; j < 4; j++) {
            List<Tuple<int, int>> encounterZone1 = new List<Tuple<int, int>>();
            encounterZone1.Add(getRandomWalkableTile());

            for (int i = 0; i < 10; i++) {
                encounterZone1 = growSelection(encounterZone1, 0.2f);
            }
            replaceTiles(encounterZone1, TileID.pineForestShrubEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.grass);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }        

    }
}
