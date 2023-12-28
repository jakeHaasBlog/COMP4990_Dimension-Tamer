using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ForestGenerator : BiomeGenerator
{
    public ForestGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.Forest;
    }

    override public void generate() {
        Debug.Log("Forest generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        List<Tuple<int, int>> borderTiles = getBorderTiles();
        borderTiles = growSelection(borderTiles, 0.6f);
        borderTiles = growSelection(borderTiles, 0.6f);
        replaceTiles(borderTiles, TileID.tree, true, true, true);
        borderTiles = growSelection(borderTiles);
        replaceTiles(borderTiles, TileID.dirt, false, true, true);

        for (int i = 0; i < (boundWidth / 10) + 1; i++) {
            List<Tuple<int, int>> tallGrassTiles = new List<Tuple<int, int>>();
            tallGrassTiles.Add(getRandomWalkableTile());
            for (int r = UnityEngine.Random.Range(3, 10); r >= 0; r--)
                tallGrassTiles = growSelection(tallGrassTiles, 0.7f);

            replaceTiles(tallGrassTiles, TileID.tallGrassEZ, false, true, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.grass);
            elements.Add(ElementalType.poison);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(tallGrassTiles, elements, encounterZoneLevel);

        }
        

    }
}
