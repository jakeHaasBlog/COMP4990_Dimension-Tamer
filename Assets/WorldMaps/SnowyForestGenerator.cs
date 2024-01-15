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