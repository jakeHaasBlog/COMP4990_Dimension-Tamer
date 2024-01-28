using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DesertGenerator : BiomeGenerator
{
    public DesertGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.Desert;
    }

    override public void generate() {
        Debug.Log("Desert generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        for (int i = 0; i < 50; i++) {
            Tuple<int, int> tile = getRandomWalkableTile();
            tryPlaceSkull(tile.Item1, tile.Item2);
        }


        for (int j = 0; j < 10; j++) {
            List<Tuple<int, int>> encounterZone1 = new List<Tuple<int, int>>();
            encounterZone1.Add(getRandomWalkableTile());

            for (int i = 0; i < 5; i++) {
                encounterZone1 = growSelection(encounterZone1, 0.3f);
            }
            replaceTiles(encounterZone1, TileID.darkSandEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.ice);
            elements.Add(ElementalType.fire);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }

    }

    void tryPlaceSkull(int x, int y) {

        List<Tuple<int, int>> skullImage = new List<Tuple<int, int>>() 
        {
            Tuple.Create(2, 0), Tuple.Create(3, 0), Tuple.Create(4, 0), Tuple.Create(5, 0), Tuple.Create(6, 0), Tuple.Create(7, 0), Tuple.Create(3, 0),
            Tuple.Create(1, 1), Tuple.Create(9, 1),
            Tuple.Create(0, 2), Tuple.Create(10, 2),
            Tuple.Create(0, 3), Tuple.Create(2, 3), Tuple.Create(3, 3), Tuple.Create(7, 3), Tuple.Create(8, 3), Tuple.Create(10, 3),
            Tuple.Create(0, 4), Tuple.Create(5, 4), Tuple.Create(10, 4),
            Tuple.Create(0, 5), Tuple.Create(10, 5),
            Tuple.Create(1, 6), Tuple.Create(2, 6), Tuple.Create(4, 6), Tuple.Create(6, 6), Tuple.Create(8, 6), Tuple.Create(9, 6),
            Tuple.Create(2, 7), Tuple.Create(4, 7), Tuple.Create(6, 7), Tuple.Create(8, 7),
            Tuple.Create(2, 8), Tuple.Create(3, 8), Tuple.Create(4, 8), Tuple.Create(5, 8), Tuple.Create(6, 8), Tuple.Create(7, 8), Tuple.Create(8, 8)
        };

        bool canPlace = true;
        for (int i = 0; i < skullImage.Count; i++) {
            int px = x + skullImage[i].Item1;
            int py = y - skullImage[i].Item2;

            if (!WorldMap.currentMap.getCellIsWalkable(px, py) 
            || WorldMap.currentMap.getCellBackgroundID(px, py) == TileID.persistantPath 
            || WorldMap.currentMap.getCellBranchID(px, py) != branchID
            || WorldMap.currentMap.getCellBackgroundID(px, py) == TileID.crackedDirt
            ){
                canPlace = false;
                break;
            }
        }

        if (canPlace) {
            for (int i = 0; i < skullImage.Count; i++) {
                int px = x + skullImage[i].Item1;
                int py = y - skullImage[i].Item2;

                WorldMap.currentMap.setCellBackground(px, py, TileID.crackedDirt);
            }
        }

    }

}
