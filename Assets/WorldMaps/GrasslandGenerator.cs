using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrasslandGenerator : BiomeGenerator
{
    public GrasslandGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.Grasslands;
    }

    override public void generate() {
        Debug.Log("Grassland generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        for (int x = boundX; x < boundX + boundWidth; x++) {
            for (int y = boundY; y < boundY + boundHeight; y++) {

                if (!WorldMap.currentMap.isValidCoord(x, y)) continue;
                if (WorldMap.currentMap.getCellBranchID(x, y) != branchID) continue;
                if (!WorldMap.currentMap.getCellIsWalkable(x, y)) continue;
                if (WorldMap.currentMap.getCellBackgroundID(x, y) == TileID.persistantPath) continue;

                int waveV = y % 9;
                int waveX = x - boundX;
                if (waveV == (int)((4.5f * Mathf.Sin((float)waveX / 5.0f)) + 4.5f))
                    WorldMap.currentMap.setCellBackground(x, y, TileID.grasslandTallGrass);


                waveV = (y + 3) % 9;
                if (waveV == (int)((4.5f * Mathf.Sin((float)waveX / 5.0f)) + 4.5f))
                    WorldMap.currentMap.setCellBackground(x, y, TileID.grasslandTallGrass);


                waveV = (y + 6) % 9;
                if (waveV == (int)((4.5f * Mathf.Sin((float)waveX / 5.0f)) + 4.5f))
                    WorldMap.currentMap.setCellBackground(x, y, TileID.grasslandTallGrass);

            }
        }

        for (int j = 0; j < 7; j++) {
            List<Tuple<int, int>> encounterZone1 = new List<Tuple<int, int>>();
            encounterZone1.Add(getRandomWalkableTile());

            for (int i = 0; i < 6; i++) {
                encounterZone1 = growSelection(encounterZone1, 0.2f);
            }
            replaceTiles(encounterZone1, TileID.grasslandDarkGrassEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.grass);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }

    }
}
