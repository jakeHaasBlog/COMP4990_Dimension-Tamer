using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IcePondGenerator : BiomeGenerator
{

    public IcePondGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.IcePond;
    }

    override public void generate() {
        Debug.Log("Ice pond generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        replaceTiles(getAllTiles(), TileID.icePondSnow, false, true, true);

        List<Tuple<int, int>> borderTiles = getBorderTiles();
        borderTiles = growSelection(borderTiles);

        List<Tuple<int, int>> bTiles = new List<Tuple<int, int>>();
        for (int i = 0; i < borderTiles.Count; i++) {
            if (UnityEngine.Random.Range(0, 100) < 20) {
                bTiles.Add(borderTiles[i]);
            }
        }

        replaceTiles(bTiles, TileID.icePondTree, true, true, true);
        replaceTiles(bTiles, TileID.icePondSnow, false, true, true);

        float averageX = 0.0f;
        float averageY = 0.0f;
        List<Tuple<int, int>> allTiles = getAllTiles();
        for (int i = 0; i < allTiles.Count; i++) {
            averageX += allTiles[i].Item1;
            averageY += allTiles[i].Item2;
        }
        averageX /= allTiles.Count;
        averageY /= allTiles.Count;

        List<Tuple<int, int>> pondTiles = new List<Tuple<int, int>>();
        pondTiles.Add(new Tuple<int, int>((int)averageX, (int)averageY));
        for (int i = 0; i < 5; i++) {
            pondTiles = growSelection(pondTiles, 0.95f);
        }

        List<Tuple<int, int>> pondRingTiles = new List<Tuple<int, int>>();
        pondRingTiles = growSelection(pondTiles);
        pondRingTiles = growSelection(pondRingTiles, 0.1f);

        replaceTiles(pondRingTiles, TileID.icePondDeepSnowEZ, false);
        replaceTiles(pondTiles, TileID.icePondIce, false);

        for (int i = 0; i < pondTiles.Count; i++) {
            if (WorldMap.currentMap.getCellBackgroundID(pondTiles[i].Item1, pondTiles[i].Item2) == TileID.persistantPath) {
                WorldMap.currentMap.setCellBackground(pondTiles[i].Item1, pondTiles[i].Item2, TileID.icePondIce);
            }
        }

        for (int i = 0; i < pondTiles.Count; i++) {
            pondRingTiles.Remove(pondTiles[i]);
        }
        List<ElementalType> elements = new List<ElementalType>();
        elements.Add(ElementalType.ice);
        elements.Add(ElementalType.none);
        int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
        addEncounterZone(pondRingTiles, elements, encounterZoneLevel);

    }

}
