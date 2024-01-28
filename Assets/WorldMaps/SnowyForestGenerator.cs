using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SnowyForestGenerator : BiomeGenerator
{
    public SnowyForestGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.SnowyForest;
    }

    override public void generate() {
        Debug.Log("Snowy Forest generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        List<Tuple<int, int>> borderTiles = getBorderTiles();
        borderTiles = growSelection(borderTiles);        
        replaceTiles(borderTiles, TileID.water, false);

        for (int i = 0; i < (boundWidth * boundHeight) / 5; i++) {
            int dx = UnityEngine.Random.Range(0, boundWidth / 2) * 2;
            int dy = UnityEngine.Random.Range(0, boundHeight / 2) * 2;

            if (WorldMap.currentMap.getCellBranchID(boundX + dx, boundY + dy) != branchID) continue;
            if (!WorldMap.currentMap.getCellIsWalkable(boundX + dx, boundY + dy)) continue;

            WorldMap.currentMap.setCellForeground(boundX + dx, boundY + dy, TileID.snowTree);
        } 


        for (int j = 0; j < 4; j++) {
            List<Tuple<int, int>> encounterZone1 = new List<Tuple<int, int>>();
            encounterZone1.Add(getRandomWalkableTile());

            for (int i = 0; i < 10; i++) {
                encounterZone1 = growSelection(encounterZone1, 0.2f);
            }
            replaceTiles(encounterZone1, TileID.snowyForestDeepSnowEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.ice);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }

    }
}
