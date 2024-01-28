using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TundraGenerator : BiomeGenerator
{
    public TundraGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.Tropics;
    }

    override public void generate() {
        Debug.Log("Tundra generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        List<Tuple<int, int>> borderTiles = getBorderTiles();
        borderTiles = growSelection(borderTiles);        
        replaceTiles(borderTiles, TileID.water, false);

        // grass splotches
        for (int i = 0; i < 30; i++) {
            Tuple<int, int> p = getRandomWalkableTile();
            List<Tuple<int, int>> splotch = new List<Tuple<int, int>>();
            splotch.Add(p);

            for (int j = 0; j < 10; j++) {
                splotch = growSelection(splotch, 0.5f);
            } 

            p = splotch[UnityEngine.Random.Range(0, splotch.Count)];
            List<Tuple<int, int>> redGrass = new List<Tuple<int, int>>();
            redGrass.Add(p);

            for (int j = 0; j < 5; j++) {
                redGrass = growSelection(redGrass, 0.5f);
            } 

            replaceTiles(splotch, TileID.tundraGrass, false);
            replaceTiles(redGrass, TileID.tundraRedGrass, false);

        }

        // rocks
        for (int i = 0; i < 30; i++) {
            int dx = UnityEngine.Random.Range(0, boundWidth);
            int dy = UnityEngine.Random.Range(0, boundHeight);
            if (WorldMap.currentMap.getCellBranchID(boundX + dx, boundY + dy) != branchID) continue;

            WorldMap.currentMap.setCellForeground(boundX + dx, boundY + dy, TileID.tundraRocks);
        }



        for (int j = 0; j < 4; j++) {
            List<Tuple<int, int>> encounterZone1 = new List<Tuple<int, int>>();
            encounterZone1.Add(getRandomWalkableTile());

            for (int i = 0; i < 10; i++) {
                encounterZone1 = growSelection(encounterZone1, 0.2f);
            }
            replaceTiles(encounterZone1, TileID.tundraRoughGroundEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.ice);
            elements.Add(ElementalType.grass);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }

    }
}
