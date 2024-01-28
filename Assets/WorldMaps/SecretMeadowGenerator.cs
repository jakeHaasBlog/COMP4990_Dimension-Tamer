using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SecretMeadowGenerator : BiomeGenerator
{

    List<Tuple<int, int>> circleSmall;
    List<Tuple<int, int>> circleLarge;

    public SecretMeadowGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {
        circleSmall = new List<Tuple<int, int>>() {Tuple.Create(1, 0), Tuple.Create(0, 1), Tuple.Create(2, 1), Tuple.Create(1, 2)};
        
        circleLarge = new List<Tuple<int, int>>() {
            Tuple.Create(2, 0), Tuple.Create(3, 0), Tuple.Create(4, 0),
            Tuple.Create(1, 1), Tuple.Create(5, 1),
            Tuple.Create(0, 2), Tuple.Create(6, 2),
            Tuple.Create(0, 3), Tuple.Create(6, 3),
            Tuple.Create(0, 4), Tuple.Create(6, 4),
            Tuple.Create(1, 5), Tuple.Create(5, 5),
            Tuple.Create(2, 6), Tuple.Create(3, 6), Tuple.Create(4, 6)
        };
    }

    override public BiomeID getBiome() {
        return BiomeID.SecretMeadow;
    }

    override public void generate() {
        Debug.Log("Secret Meadow generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        List<Tuple<int, int>> borderTiles = getBorderTiles();
        borderTiles = growSelection(borderTiles);

        replaceTiles(borderTiles, TileID.water, false);

        
        for (int j = 0; j < 20; j++) {
            
            Tuple<int, int> origin = getRandomWalkableTile();
            TileID grassType = TileID.secretGrass2;
            if (UnityEngine.Random.Range(0, 100) < 50) grassType = TileID.secretGrass3;

            
            if (UnityEngine.Random.Range(0, 100) < 50) {
                for (int i = 0; i < circleSmall.Count; i++) {
                    int px = origin.Item1 + circleSmall[i].Item1;
                    int py = origin.Item2 - circleSmall[i].Item2;

                    if (WorldMap.currentMap.getCellBranchID(px, py) != branchID) continue;

                    WorldMap.currentMap.setCellBackground(px, py, grassType);
                }
            } else {
                for (int i = 0; i < circleLarge.Count; i++) {
                    int px = origin.Item1 + circleLarge[i].Item1;
                    int py = origin.Item2 - circleLarge[i].Item2;

                    if (WorldMap.currentMap.getCellBranchID(px, py) != branchID) continue;

                    WorldMap.currentMap.setCellBackground(px, py, grassType);
                }
            }

        }


        for (int j = 0; j < 4; j++) {
            List<Tuple<int, int>> encounterZone1 = new List<Tuple<int, int>>();
            encounterZone1.Add(getRandomWalkableTile());

            for (int i = 0; i < 10; i++) {
                encounterZone1 = growSelection(encounterZone1, 0.2f);
            }
            replaceTiles(encounterZone1, TileID.meadowFlowersEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.water);
            elements.Add(ElementalType.grass);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }

    }
}
